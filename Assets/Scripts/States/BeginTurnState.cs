using UnityEngine;
using System.Collections;

public class BeginTurnState : StateMachineBehaviour
{
	public bool IsPlayer;
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameContext.Instance.Command.gameObject.SetActive(true);
		GameContext.Instance.Command.Text = IsPlayer ? Constants.PlayersTurn : Constants.OpponentsTurn;
	}
}
