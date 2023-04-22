using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInvestigateState : EnemyBaseState
{
    public EnemyInvestigateState(EnemyAI enemy_ai) : base(enemy_ai)
    {
    }

    private float stayTimer = 10f;
    private float currentTime = 0f;


    public override void StateStart()
    {
        base.StateStart();

        _enemy_AI.Agent.speed = _enemy_AI.WanderSpeed;
        _enemy_AI.Agent.isStopped = false;
        _enemy_AI.Agent.angularSpeed = 99999;

        if (_enemy_AI._Animator.clip != _enemy_AI.ClipsArray[1])
            SetAnimation(_enemy_AI.ClipsArray[1], 1f);
    }

    public override void SetAnimation(AnimationClip clip, float speed = 1f)
    {
        base.SetAnimation(clip);
        _enemy_AI._Animator.clip = clip;
        _enemy_AI._Animator.AddClip(clip, "clip");
        _enemy_AI._Animator["clip"].speed = speed;
        _enemy_AI._Animator.Play("clip");
    }

    private bool StayTimerFunc()
    {
        if (currentTime >= stayTimer)
        {
            return true;
        }
        else
        {
            currentTime += Time.deltaTime;
            return false;
        }
    }
    
    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_enemy_AI.CanSeePlayer)
        {
            _enemy_AI.SetState(_enemy_AI.ChaseState);
            _enemy_AI.TimesPlayerHasBeenSeen++;
        }

        if (_enemy_AI.IsEnemyAtPoint(_enemy_AI.transform.position, _enemy_AI.PlayerLastLocation))
        {
            if (_enemy_AI._Animator.clip != _enemy_AI.ClipsArray[0])
                SetAnimation(_enemy_AI.ClipsArray[0], 0.5f);

            if (StayTimerFunc())
                _enemy_AI.SetState(_enemy_AI.PatrolState);
        }
        else
            _enemy_AI.Agent.SetDestination(_enemy_AI.PlayerLastLocation);
    }
}
