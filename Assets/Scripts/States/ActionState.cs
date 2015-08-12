using UnityEngine;
using System.Collections;

public class ActionState : StateMachineBehaviour
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//activate players grid
		GameContext.Instance.CurrentPlayer.GridVisible = true;
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//disable players grid
		GameContext.Instance.CurrentPlayer.GridVisible = false;
	}
}
