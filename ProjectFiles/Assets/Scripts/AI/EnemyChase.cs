using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    private float chaseTime = 4f;
    private float currentTime = 0f;


    public EnemyChaseState(EnemyAI enemy_ai) : base(enemy_ai)
    {
    }

    public override void StateStart() 
    {
        _enemy_AI.Agent.speed = _enemy_AI.RunSpeed;
        SetAnimation(_enemy_AI.ClipsArray[2], 1.2f);
        _enemy_AI.Agent.isStopped = false;
        _enemy_AI.Agent.stoppingDistance = 0.5f;
        _enemy_AI.Agent.angularSpeed = _enemy_AI.TurnSpeed;

        //Make sure it resets any time the action breaks
        //So they dont automatically lose the player
        //When attacking at a unfourtunate direction
        currentTime = 0f;
    }

    private bool ChaseTimer()
    {
        if (currentTime <= chaseTime)
        {
            currentTime += Time.deltaTime;
            return false;
        }
        else if (currentTime >= chaseTime)
            return true;
        else
            return false;
    }

    private void MovementFunc()
    {
        //If they lose line of sight, keep going to last location
        if (!_enemy_AI.CanSeePlayer && _enemy_AI.DoesntHaveLineOfSight())
        {
            _enemy_AI.MoveToPlayersLastKnownLocation();
            if (ChaseTimer())
                _enemy_AI.SetState(_enemy_AI.SearchState);
        }
        else if (!_enemy_AI.CanSeePlayer && !_enemy_AI.DoesntHaveLineOfSight())
        {
            _enemy_AI.PlayerLastLocation = _enemy_AI.Player.position;
            _enemy_AI.MoveToPlayer();
        }
        else
        {
            _enemy_AI.PlayerLastLocation = _enemy_AI.Player.position;
            _enemy_AI.MoveToPlayer();
        }
    }

    public override void SetAnimation(AnimationClip clip, float speed = 1f)
    {
        base.SetAnimation(clip);
        _enemy_AI._Animator.clip = clip;
        _enemy_AI._Animator.AddClip(clip, "clip");
        _enemy_AI._Animator["clip"].speed = speed;
        _enemy_AI._Animator.Play("clip");
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Debug.Log("Chasing");

        if (_enemy_AI.DistanceBetweenObjets(_enemy_AI.transform.position, _enemy_AI.Player.position) < 1.5f)
            _enemy_AI.SetState(_enemy_AI.AttackState);

        MovementFunc();

        //reset value if player is seen
        if (currentTime >= 0f && _enemy_AI.CanSeePlayer)
            currentTime = 0f;

    }
}
