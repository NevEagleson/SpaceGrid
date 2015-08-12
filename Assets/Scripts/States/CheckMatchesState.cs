using UnityEngine;
using System.Collections;

public class CheckMatchesState : StateMachineBehaviour
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//Check Matches
		if (GameContext.Instance.CurrentPlayer.CheckMatches())
		{
			GameContext.Instance.SetState(GameState.MoveShips);
		}
		else
		{
			if (GameContext.Instance.CurrentPlayer.TurnsRemaining > 0)
			{
				GameContext.Instance.SetState(GameState.Action);			
			}
			else
			{
				GameContext.Instance.SetState(GameState.EndTurn);
			}
		}
	}
}
