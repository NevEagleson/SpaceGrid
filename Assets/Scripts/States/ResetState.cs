using UnityEngine;
using System.Collections;

public class ResetState : StateMachineBehaviour 
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		GameContext.Instance.Player.Reset();
		GameContext.Instance.Opponent.Reset();
		animator.SetBool("playersTurn", Random.value > 0.5f);
	}

	/*
	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		GameContext.Instance.Player.Reset();
		GameContext.Instance.Opponent.Reset();
		animator.SetBool("playersTurn", Random.value > 0.5f);
	}
	*/

}
