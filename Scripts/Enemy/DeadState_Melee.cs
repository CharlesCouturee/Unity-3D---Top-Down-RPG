using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState_Melee : EnemyState
{
    private Enemy_Melee enemy;
    private Enemy_Ragdoll ragdoll;

    public DeadState_Melee(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Melee;
        ragdoll = enemy.GetComponent<Enemy_Ragdoll>();
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = 1.5f;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        ragdoll.RagdollActive(true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            ragdoll.RagdollActive(false);
            ragdoll.CollidersActive(false);
        }
    }
}
