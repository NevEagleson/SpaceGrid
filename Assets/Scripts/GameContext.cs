using UnityEngine;
using UnityEngine.UI;

public class GameContext : MonoBehaviour 
{
	public static GameContext Instance { get; private set; }

	public Animator GameState;
	public GridManager Player;
	public GridManager Opponent;
	public Command Command;
    public Text StatusText;


	// Use this for initialization
	void Start () {
		if(Instance == null)
			Instance = this;
		else
			Destroy(this);
	}
}
