using UnityEngine;
using System.Collections;

public class ReinforcementsButton : MonoBehaviour
{
	public void OnPressed()
	{
		GameContext.Instance.SetState(GameState.SpawnReinforcements);
	}
}
