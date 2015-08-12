using UnityEngine;
using UnityEngine.UI;
using System;

public enum GameState
{
	Intro,
	BeginTurn,
	Battle,
	Action,
	DoUltimate,
	CheckMatches,
	MoveShips,
	SpawnReinforcements,
	EndTurn,
	Reset,
	NumStates
}

public class GameContext : MonoBehaviour 
{
	private static GameContext sInstance;
	public static GameContext Instance
	{
		get
		{
			if (sInstance == null)
			{
				GameObject go = GameObject.FindGameObjectWithTag("GameController");
				sInstance = go.GetComponent<GameContext>();
			}
			return sInstance;
		}
	}

	public Action<GridSpace> OnCombo;
	public Action<GridSpace> OnChain;

	public Animator StateController;
	public GridManager Player;
	public GridManager Opponent;
	public Command Command;
    public Text StatusText;

	private bool mPlayersTurn;
	public bool PlayersTurn
	{
		get { return mPlayersTurn; }
		set { mPlayersTurn = value; StateController.SetBool("playersTurn", mPlayersTurn); }
	}

	public GridManager CurrentPlayer { get { return mPlayersTurn ? Player : Opponent; } }

	// Use this for initialization
	void Start () {
		if (sInstance == null)
			sInstance = this;
		else if (sInstance != this)
			Destroy(this);
	}

	public void SetState(GameState nextState)
	{
		switch (nextState)
		{
			case GameState.Action:
				StateController.SetTrigger("action");
				break;
			case GameState.Battle:
				StateController.SetTrigger("battle");
				break;
			case GameState.CheckMatches:
				StateController.SetTrigger("checkMatches");
				break;
			case GameState.MoveShips:
				StateController.SetTrigger("moveShips");
				break;
			case GameState.SpawnReinforcements:
				StateController.SetTrigger("spawnReinforcements");
				break;
			case GameState.DoUltimate:
				StateController.SetTrigger("doUltimate");
				break;
			case GameState.EndTurn:
				StateController.SetTrigger("endTurn");
				break;
			default:
				break;
		}
	}

	public void FireOnCombo(GridSpace location)
	{
		if (OnCombo != null)
			OnCombo.Invoke(location);
	}

	public void FireOnChain(GridSpace location)
	{
		if (OnChain != null)
			OnChain.Invoke(location);
	}
}
