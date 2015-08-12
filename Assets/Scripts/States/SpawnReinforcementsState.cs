using UnityEngine;
using System.Collections;

public class SpawnReinforcementsState : StateMachineBehaviour
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		GameContext.Instance.Command.gameObject.SetActive(false);
		GridManager grid = GameContext.Instance.PlayersTurn ? GameContext.Instance.Player : GameContext.Instance.Opponent;
		grid.SpawnReinforcements();
	}

}
