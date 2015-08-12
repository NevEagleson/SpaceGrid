using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridSpace : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IBeginDragHandler, IEndDragHandler
{
	public GridManager Grid { get; set; }
	
	public GameObject BoxGraphic;
	public GameObject SelectionGraphic;
	public Graphic HighlightGraphic;

	public Color DefaultColor;
	public Color ValidColor;
	public Color InvalidColor;

    public bool Occupied { get { return Ship != null; } }

    public GridSpace ForwardSpace { get; set; }
    public GridSpace BackSpace { get; set; }
    public GridSpace LeftSpace { get; set; }
    public GridSpace RightSpace { get; set; }

	private bool mVisible;
    private bool mHighlighted;
	private bool mSelected;
	private Ship mShip;
	private static GridSpace mDragging;

	void OnEnable()
	{
		mVisible = false;
		mHighlighted = false;
		mSelected = false;
		HighlightGraphic.color = DefaultColor;
		UpdateState();
	}

	public Ship Ship
	{
		get { return mShip; }
		set
		{
			mShip = value;
			UpdateState();
		}

	}

	public bool GridVisible
	{
		get { return mVisible; }
        set 
		{
			mVisible = value;
			UpdateState();
		}
	}

    public bool Highlight
    {
        get { return mHighlighted; }
        set 
		{ 
			mHighlighted = value;
			HighlightGraphic.color = DefaultColor;
			UpdateState();	
		}
    }

	public void Select()
	{
		if (!Occupied) return;
		mSelected = true;
		mHighlighted = false;
		GameContext.Instance.Player.OnSelect(this);
		GameContext.Instance.Opponent.OnSelect(this);
        Ship.Select();
		UpdateState();
	}

	public void Deselect()
	{
		mSelected = false;
		mHighlighted = false;
		UpdateState();
	}

	private void UpdateState()
	{
		BoxGraphic.SetActive(Occupied && mVisible);
		if (!Occupied || !mVisible) mSelected = false;
		if (!mVisible) mHighlighted = false;
		SelectionGraphic.SetActive(mSelected);
		HighlightGraphic.gameObject.SetActive(mHighlighted);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!mVisible) return;
		if (mDragging != null && mDragging != this)
		{
			HighlightGraphic.gameObject.SetActive(true);
			HighlightGraphic.color = mHighlighted ? ValidColor : InvalidColor;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!mVisible) return;
		if (mDragging != null && mDragging != this)
		{
			Highlight = mHighlighted;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!mVisible) return;
		Select();
		mDragging = this;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!mVisible) return;
		mDragging = null;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (!mVisible) return;
		if (mDragging != null && mDragging != this)
		{
			if (mHighlighted)
			{
				Ship dropShip = mDragging.Ship;
				Ship previousShip = Ship;
				if (previousShip != null) previousShip.GridSpace = null;
				GridSpace previousSpace = dropShip.GridSpace;

				dropShip.GridSpace = this;
				if (previousShip != null) previousShip.GridSpace = previousSpace;

				Select();

				Grid.UseTurn();

				GameContext.Instance.SetState(GameState.CheckMatches);
			}

			Highlight = false;
		}
	}
}
