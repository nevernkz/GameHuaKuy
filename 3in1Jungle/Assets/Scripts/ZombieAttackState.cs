using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Movement aiMovement = animator.GetComponent<Movement>();

        if (aiMovement)
        {
            aiMovement.ChangeState(Movement.AIState.Idle);
        }
    }
 
}
