using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdelState : EnemyBaseState
{
    public EnemyIdelState(EnemyAI enemy_ai) : base(enemy_ai)
    {
    }

    public override void StateStart()
    {
        base.StateStart();

        _enemy_AI.Agent.speed = _enemy_AI.WanderSpeed;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }
}
