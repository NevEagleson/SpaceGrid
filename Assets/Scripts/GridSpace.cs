using UnityEngine;
using System.Collections;

public class GridSpace : MonoBehaviour 
{
	public GridManager Grid { get; set; }
	public Ship Ship;
	public GameObject Box;
	public GameObject Selection;
    public bool Occupied { get { return Ship != null; } }

    public GridSpace ForwardSpace { get; set; }
    public GridSpace BackSpace { get; set; }
    public GridSpace LeftSpace { get; set; }
    public GridSpace RightSpace { get; set; }

    private bool mHighlighted;

	void OnEnable()
	{
		GridVisible = false;
	}

	public bool GridVisible
	{
		get { return Box.activeSelf; }
        set { Box.SetActive(value); if (!value) Selection.SetActive(false); }
	}

    public bool Highlight
    {
        get { return mHighlighted; }
        set { mHighlighted = value; GetComponent<Animator>().SetBool("Flashing", mHighlighted); }
    }

	public void Select()
	{
		if (!Occupied) return;
		Selection.SetActive(true);
		GameContext.Instance.Player.OnSelect(this);
		GameContext.Instance.Opponent.OnSelect(this);
        Ship.Select();
	}

	public void Deselect()
	{
		Selection.SetActive(false);
        Highlight = false;
	}
}
