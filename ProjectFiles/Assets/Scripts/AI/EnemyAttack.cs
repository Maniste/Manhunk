using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyAI enemy_ai) : base(enemy_ai)
    {
    }

    private float attackTime = 0.0f;
    private float currentTime = 0.0f;
    private bool hitPlayer = false;

    public override void StateStart()
    {
        base.StateStart();
        _enemy_AI.Agent.isStopped = true;
        _enemy_AI.Agent.stoppingDistance = 0.5f;
        StartAttack();
    }

    private void StartAttack()
    {
        SetAnimation(_enemy_AI.ClipsArray[3], 0.5f);
        attackTime = (_enemy_AI._Animator["clip"].speed * 1.5f);
        Debug.Log(attackTime);
        currentTime = 0f;
        hitPlayer = false;
    }

    public override void SetAnimation(AnimationClip clip, float speed = 1f)
    {
        base.SetAnimation(clip);
        _enemy_AI._Animator.clip = clip;
        _enemy_AI._Animator.AddClip(clip, "clip");
        _enemy_AI._Animator["clip"].speed = speed;
        _enemy_AI._Animator.Play("clip");
    }

    private bool AttackTimerFunc()
    {
        if (currentTime >= attackTime)
        {
            StartAttack();
            return true;
        }
        else
        {
            currentTime += Time.deltaTime;
            return false;
        }
    }

    private bool isPlayerInRange()
    {
        if (_enemy_AI.DistanceBetweenObjets(_enemy_AI.transform.position, _enemy_AI.Player.position) > 1f)
            return false;
        else
            return true;
    }


    private void AttackPlayer()
    {
        Collider[] hits = Physics.OverlapBox
            (_enemy_AI.AttackHitBox.transform.position, 
            _enemy_AI.AttackHitBox.size, 
            _enemy_AI.AttackHitBox.transform.rotation,
            _enemy_AI.PlayerLayer
            );

        if(hits.Length >= 1)
        {
            Debug.Log("attacked: " + hits[0]);
            PlayerHealth pHealth = hits[0].GetComponent<PlayerHealth>();
            if(pHealth == null)
            {
                Debug.Log("Could not find Player health scritp");
            }

            if (pHealth.isActiveAndEnabled)
                pHealth.ApplyDamage(_enemy_AI.AttackDamage);

            hitPlayer = true;
        }

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Debug.Log("Attack state");

        if (!hitPlayer)
            AttackPlayer();


        if (AttackTimerFunc() && !isPlayerInRange())
            _enemy_AI.SetState(_enemy_AI.ChaseState);

    }

}
