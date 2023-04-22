using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearch : EnemyBaseState
{
    public EnemySearch(EnemyAI enemy_ai) : base(enemy_ai)
    {
    }

    private int pointsSearched = 0;

    private float _searchTime = 4f;

    private float currentTime = 0f;

    private float stuckTimer = 15f;
    private float currentStuckTimer = 0f;

    private Vector3 currentTarget = Vector3.zero;

    public override void StateStart()
    {
        base.StateStart();
        currentTime = 0f;
        _enemy_AI.Agent.speed = _enemy_AI.SearchSpeed;
        _enemy_AI.Agent.angularSpeed = 99999;
        _enemy_AI.Agent.stoppingDistance = 0f;
        currentTarget = GetNewLocations();
        SetAnimation(_enemy_AI.ClipsArray[0], 0.5f);
    }

    public override void SetAnimation(AnimationClip clip, float speed = 1f)
    {
        base.SetAnimation(clip);
        _enemy_AI._Animator.clip = clip;
        _enemy_AI._Animator.AddClip(clip, "clip");
        _enemy_AI._Animator["clip"].speed = speed;
        _enemy_AI._Animator.Play("clip");
    }

    private bool SearchTimer()
    {
        if (currentTime < _searchTime)
            currentTime += Time.deltaTime;
        else if (currentTime >= _searchTime)
        {
            return true;
        }

        return false;
    }

    private bool IsStuckTimer()
    {
        if (currentStuckTimer < stuckTimer)
            currentStuckTimer += Time.deltaTime;
        else if (currentStuckTimer >= stuckTimer)
        {
            return true;
        }

        return false;
    }

    private Vector3 GetNewLocations()
    {
        Vector3 dir = -Vector3.forward;
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
                dir = _enemy_AI.transform.forward;
            else if (i == 1)
                dir = _enemy_AI.transform.right;
            else if (i == 2)
                dir = -_enemy_AI.transform.right;

            if (Physics.Raycast(_enemy_AI.transform.position, dir, 10f, 1))
                continue;
            else
                return _enemy_AI.transform.position + (dir * 8f);
        }

        return _enemy_AI.transform.position;
    }

    private void SeachArea()
    {
        if (IsStuckTimer())
        {
            currentTarget = GetNewLocations();
            currentStuckTimer = 0f;
        }


        float dist = _enemy_AI.DistanceBetweenObjets(_enemy_AI.transform.position, currentTarget);
        if (dist > 0.1f)
        {
            if (_enemy_AI.Agent.isStopped)
                _enemy_AI.Agent.isStopped = false;

            _enemy_AI.Agent.SetDestination(currentTarget);

            if (_enemy_AI._Animator.clip != _enemy_AI.ClipsArray[1])
                SetAnimation(_enemy_AI.ClipsArray[1], 1f);
        }
        else if (dist < 0.2f)
        {
            if(pointsSearched >= 5)
                _enemy_AI.SetState(_enemy_AI.PatrolState);

            pointsSearched++;
            currentTime = 0f;
            currentTarget = GetNewLocations();
            SetAnimation(_enemy_AI.ClipsArray[0], 0.5f);

            //Set the position so the agent doesnt slide forward
            _enemy_AI.Agent.isStopped = true;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Debug.Log("Searching state");

        if (_enemy_AI.CanSeePlayer)
        {
            _enemy_AI.SetState(_enemy_AI.ChaseState);
            _enemy_AI.TimesPlayerHasBeenSeen++;
        }

        if (SearchTimer())
            SeachArea();
    }

}
