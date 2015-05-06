using UnityEngine;
using System.Collections;

public class ActionState : StateMachineBehaviour
{
	public bool IsPlayer;
	public bool PlayerControlled;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameContext.Instance.Player.GridVisible = IsPlayer;
		GameContext.Instance.Opponent.GridVisible = !IsPlayer;
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameContext.Instance.Player.GridVisible = IsPlayer;
		GameContext.Instance.Opponent.GridVisible = !IsPlayer;
	}
}
