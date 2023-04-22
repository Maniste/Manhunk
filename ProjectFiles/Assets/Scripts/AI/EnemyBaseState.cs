using UnityEngine;

public abstract class EnemyBaseState
{
    protected EnemyAI _enemy_AI = null;

    public EnemyBaseState(EnemyAI enemy_ai)
    {
        _enemy_AI = enemy_ai;
    }

    public virtual void StateStart() { }

    public virtual void SetAnimation(AnimationClip clip, float speed = 1f) { }

    // Update is called once per frame
    public virtual void StateUpdate(){ }
}
