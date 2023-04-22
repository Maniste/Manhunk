using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{

    public EnemyPatrolState(EnemyAI enemy_ai) : base(enemy_ai)
    {
    }

    private int lastPoint = 0;

    private float currentTime = 0f;

    private float waitTimer = 5f;

    private Vector3 _currentPoint = Vector3.zero;
    public Vector3 currentPoint 
    {
        get { return _currentPoint; }
        set 
        { 
            _currentPoint = CheckIfPointIsInNavMesh(value); 
        }
    }


    private Vector3 enemyPos = Vector3.zero;

    private PatrolPath _patrolPath = null;

    public override void StateStart()
    {
        base.StateStart();

        if (_patrolPath == null)
            _patrolPath = GetNewPatrolPath();

        if (currentPoint == Vector3.zero)
            currentPoint = GetClosesestPoint();

        _enemy_AI.Agent.speed = _enemy_AI.WanderSpeed;
        _enemy_AI.Agent.isStopped = false;
        _enemy_AI.Agent.angularSpeed = 99999;
        _enemy_AI.Agent.stoppingDistance = 0f;

        SetAnimation(_enemy_AI.ClipsArray[1], 0.6f);
    }

    private bool WaitAtPointTimer()
    {
        if (currentTime < waitTimer)
            currentTime += Time.deltaTime;
        else if (currentTime >= waitTimer)
        {
            return true;
        }

        return false;
    }

    private bool ComparePosition()
    {
        Vector2 enemyCurrent = new Vector2(enemyPos.x, enemyPos.z);
        Vector2 targetCurrent = new Vector2(currentPoint.x, currentPoint.z);

        if (enemyCurrent == targetCurrent)
            return true;
        else
            return false;

        /*
        float distance = GetDist(enemyPos, currentPoint);

        if (distance < 1.5f)
            return true;
        else
            return false;
        */
    }

    private float GetDist(Vector3 pointPos, Vector3 playerPos)
    {
        float x, y, z;
        x = ((pointPos.x - playerPos.x) * (pointPos.x - playerPos.x));
        y = ((pointPos.y - playerPos.y) * (pointPos.y - playerPos.y));
        z = ((pointPos.z - playerPos.z) * (pointPos.z - playerPos.z));

        return Mathf.Sqrt(x + y + z);
    }

    private PatrolPath GetNewPatrolPath()
    {
        int shortestPath = 0;
        float dist1, dist2;

        //Itterate 3 times just incase
        for (int i = 0; i < 3; i++)
        {
            for (int y = 0; y < _enemy_AI.PointsOfIntrest.Paths.Count - 1; y++)
            {
                dist1 = GetDist(_enemy_AI.PointsOfIntrest.Paths[shortestPath].transform.position, _enemy_AI.transform.position);
                dist2 = GetDist(_enemy_AI.PointsOfIntrest.Paths[y + 1].transform.position, _enemy_AI.transform.position);

                if (dist1 < dist2)
                    shortestPath = y;
                else if (dist1 > dist2)
                    shortestPath = y + 1;

            }
        }

        return _enemy_AI.PointsOfIntrest.Paths[shortestPath];
    }

    private Vector3 GetClosesestPoint()
    {
        int shortestPoint = 0;
        float dist1, dist2;
     
       //Itterate 3 times just incase
       for(int i = 0; i <  3; i++)
        {
            for (int y = 0; y < _patrolPath.Points.Count - 1; y++)
            {
                dist1 = GetDist(_patrolPath.Points[shortestPoint].position,_enemy_AI.transform.position);
                dist2 = GetDist(_patrolPath.Points[y + 1].position, _enemy_AI.transform.position);

                if (dist1 < dist2)
                    shortestPoint = y;
                else if(dist1 > dist2)
                    shortestPoint = y + 1;
            }
        }

        lastPoint = shortestPoint;
        return new Vector3(_patrolPath.Points[shortestPoint].position.x, 0f, _patrolPath.Points[shortestPoint].position.z);
    }

    private Vector3 MoveToNextPoint()
    {
        if (lastPoint >= _enemy_AI.PointsOfIntrest.Paths[0].Points.Count - 1)
            lastPoint = 0;
        else if (lastPoint < _enemy_AI.PointsOfIntrest.Paths[0].Points.Count - 1)
            lastPoint++;
        else
            lastPoint--;

        return _patrolPath.Points[lastPoint].position;
    }

    private Vector3 CheckIfPointIsInNavMesh(Vector3 point)
    {
        Debug.Log("Look for point");

        bool canGetThere = UnityEngine.AI.NavMesh.SamplePosition(point, out UnityEngine.AI.NavMeshHit hit1, 0f, UnityEngine.AI.NavMesh.AllAreas);

        if(canGetThere)
        {
            return point;
        }
        else
        {
            //find closest edge if point is not on navmesh
            if (UnityEngine.AI.NavMesh.FindClosestEdge(point, out UnityEngine.AI.NavMeshHit hit2, UnityEngine.AI.NavMesh.AllAreas))
            {
                Debug.Log("Found point: " + hit2.position);
                return (hit2.position);
            }
            else //if all else fails just return current pos
                return _enemy_AI.transform.position;
        }
    }

    public override void SetAnimation(AnimationClip clip, float speed = 1)
    {
        base.SetAnimation(clip);
        _enemy_AI._Animator.clip = clip;
        _enemy_AI._Animator.AddClip(clip,"clip");
        _enemy_AI._Animator["clip"].speed = speed;
        _enemy_AI._Animator.Play("clip");
    }


    public override void StateUpdate()
    {
        base.StateUpdate();
        enemyPos = _enemy_AI.transform.position;

        if (_enemy_AI.CanSeePlayer)
        {
            _enemy_AI.SetState(_enemy_AI.ChaseState);
            _enemy_AI.TimesPlayerHasBeenSeen++;
        }


        if(!ComparePosition())
        {
            Debug.Log("Patroling");
            _enemy_AI.Agent.SetDestination(currentPoint);
        }
        else if (ComparePosition())
        {
            if (waitTimer > 0.01f)
                if (_enemy_AI._Animator.clip != _enemy_AI.ClipsArray[0])
                    SetAnimation(_enemy_AI.ClipsArray[0], 0.6f);

            if (WaitAtPointTimer())
            {
                SetAnimation(_enemy_AI.ClipsArray[1], 0.6f);
                currentPoint = MoveToNextPoint();
                currentTime = 0f;
            }
        }
    }
}
