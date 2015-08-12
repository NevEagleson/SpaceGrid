using UnityEngine;
using System.Collections;

public class BeginTurnState : StateMachineBehaviour
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//turn callout
        GameContext.Instance.StatusText.text = "";
		GameContext.Instance.Command.gameObject.SetActive(true);
		GameContext.Instance.Command.Text = GameContext.Instance.PlayersTurn ? Constants.PlayersTurn : Constants.OpponentsTurn;

		GameContext.Instance.CurrentPlayer.StartTurn();
	}

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		//hide callout
		GameContext.Instance.Command.gameObject.SetActive(false);
	}
}
