using UnityEngine;
using System.Collections;

public class BattleState : StateMachineBehaviour
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//TEMP - just go to action state
		GameContext.Instance.SetState(GameState.MoveShips);
	}
}
