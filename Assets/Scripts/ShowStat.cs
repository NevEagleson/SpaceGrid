using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowStat : MonoBehaviour
{
	public enum Stat
	{
		Health,
		Reinforcements,
		Turns
	}

	public Stat StatToShow;
	public bool IsPlayer;

	private Text text;

	// Use this for initialization
	void OnEnable()
	{
		GridManager grid = IsPlayer ? GameContext.Instance.Player : GameContext.Instance.Opponent;
		text = GetComponent<Text>();

		switch (StatToShow)
		{
			case Stat.Health:
				{
					grid.OnHealthChanged += OnValueChanged;
					OnValueChanged(grid.Health);
				}
				break;
			case Stat.Reinforcements:
				{
					grid.OnReinforcementsChanged += OnValueChanged;
					OnValueChanged(grid.Reinforcements);
				}
				break;
			case Stat.Turns:
				{
					grid.OnTurnsChanged += OnValueChanged;
					OnValueChanged(grid.TurnsRemaining);
				}
				break;
		}
	}

	// Update is called once per frame
	void OnDisable()
	{
		GridManager grid = IsPlayer ? GameContext.Instance.Player : GameContext.Instance.Opponent;
		switch (StatToShow)
		{
			case Stat.Health:
				{
					grid.OnHealthChanged -= OnValueChanged;
				}
				break;
			case Stat.Reinforcements:
				{
					grid.OnReinforcementsChanged -= OnValueChanged;
				}
				break;
			case Stat.Turns:
				{
					grid.OnTurnsChanged -= OnValueChanged;
				}
				break;
		}
	}

	void OnValueChanged(int value)
	{
		text.text = value.ToString("N0");
	}
}
