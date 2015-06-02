using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour 
{
    public enum ShipState
    {
        Normal,
        Attacking,
        Defending,
        Combining
    }

    [Header("Stats")]
	public int Attack;
	public int Defense;
	public int Color;
	public int Multiplier;
    public int AttackTurns;
    public float MoveSpeed;

	[Header("Scene Links")]
    [SerializeField]
    private SpriteRenderer ColorSprite;
    [SerializeField]
    private SpriteRenderer Sprite;
    [SerializeField]
    private Animator Animator;
	
    private ShipState mState;
    private ShipDefinition mDefinition;
    private ColorLibrary mColorLib;
    //this is the top left space the ship occupies
    private GridSpace mCurrentSpace;
    //animation
    private bool mMoving;
    private bool Moving
    {
        get { return mMoving; }
        set
        {
            if(mMoving == value) return;
            mMoving = value;
            Animator.SetBool("Moving", value);
        }
    }

    void OnEnable()
    {
        //because we spawn offscreen so need to move to space
        Moving = true;
    }

	void Update()
	{
        if (mDefinition == null || mCurrentSpace == null) return;

        if (transform.position != mCurrentSpace.transform.position)
        {
            Moving = true;
            transform.position = Vector3.MoveTowards(transform.position, mCurrentSpace.transform.position, MoveSpeed * Time.deltaTime);
        }
        else
        {
            Moving = false;

            if (mState == ShipState.Combining)
                Destroy(gameObject);
        }
	}

	public void SetShip(ShipDefinition def, ColorLibrary colorLib, int color, GridSpace space, Vector3 offset)
	{
		mDefinition = def;
        mColorLib = colorLib;
        UpdateLocation(space);
		Color = color;

		Attack = mDefinition.Attack;
		Defense = mDefinition.Defense;

		Sprite.sprite = mDefinition.NormalSprite;
		ColorSprite.sprite = mDefinition.NormalSpriteColor;
		ColorSprite.color = mColorLib.Colors[Color].NormalColor;

        GridSpace bottomRight = mCurrentSpace;
        for (int i = 1; i < mDefinition.Width; ++i)
            bottomRight = bottomRight.RightSpace;
        for (int i = 1; i < mDefinition.Height; ++i)
            bottomRight = bottomRight.BackSpace;

        Vector3 SpriteOffset = (bottomRight.transform.position - mCurrentSpace.transform.position) * 0.5f;
        Animator.transform.localPosition = SpriteOffset;

        transform.localPosition = offset;
	}

    public void UpdateLocation(GridSpace newSpace)
    {
        if (mCurrentSpace != null)
        {
            GridSpace xSpace = mCurrentSpace;
            for (int x = 0; x < mDefinition.Width; ++x)
            {
                xSpace.Ship = null;
                
                GridSpace ySpace = xSpace.BackSpace;
                for (int y = 1; y < mDefinition.Height; ++y)
                {
                    ySpace.Ship = this;
                    ySpace = ySpace.BackSpace;
                }

                xSpace = xSpace.RightSpace;
            }
        }
        mCurrentSpace = newSpace;
        transform.SetParent(mCurrentSpace.transform, true);
        if (mCurrentSpace != null)
        {
            GridSpace xSpace = mCurrentSpace;
            for (int x = 0; x < mDefinition.Width; ++x)
            {
                xSpace.Ship = this;

                GridSpace ySpace = xSpace.BackSpace;
                for (int y = 1; y < mDefinition.Height; ++y)
                {
                    ySpace.Ship = this;
                    ySpace = ySpace.BackSpace;
                }

                xSpace = xSpace.RightSpace;
            }
        }
    }

    public bool CanCombine(Ship other)
	{
		//color, multipler, ship type, and state must match
		bool sameType = other.mDefinition == mDefinition && other.Color == Color;
		bool sameState = other.mState == mState;
		if(!sameType || !sameState) return false;
		if(other.Multiplier == Multiplier)
		{
			return true;
		}
		else
		{
			//multiplers must be non zero - combine with the highest multiplier upto 3
			return Multiplier > 0 && other.Multiplier > 0 && Multiplier > other.Multiplier && Multiplier < 3;
		}
	}

	public void SetAttacking()
	{
		if (mState != ShipState.Attacking)
		{
			AttackTurns = mDefinition.AttackTurns;
		}

		++Multiplier;

		Attack = mDefinition.Attack * Multiplier;
		Defense = Mathf.CeilToInt(mDefinition.Defense * 0.5f) * Multiplier;

		mState = ShipState.Attacking;
		Sprite.sprite = mDefinition.AttackingSprite;
		ColorSprite.sprite = mDefinition.AttackingSpriteColor;
		ColorSprite.color = mColorLib.Colors[Color].AttackLevels[Multiplier];
		Animator.SetBool("Attacking", true);
	}

	public void SetDefending()
	{
		//all defending ships are same color
		Color = 0;

		++Multiplier;
		
		Attack = 0;
		Defense = mDefinition.Defense * Multiplier;

		mState = ShipState.Defending;
		Sprite.sprite = mDefinition.DefendingSprite;
		ColorSprite.sprite = mDefinition.DefendingSpriteColor;
		ColorSprite.color = mColorLib.Colors[Color].DefneseLevels[Multiplier];
		Animator.SetBool("Attacking", true);
	}

    public void Select()
    {
        int x, y; 
        mCurrentSpace.Grid.GetGridLocation(mCurrentSpace, out x, out y);

        GameContext.Instance.StatusText.text = string.Format("Grid X:{0} Y:{1}\nAttack:{2}\nDefense:{3}", x, y, Attack, Defense);
        if (mCurrentSpace.Grid.GridVisible)
        {
            GridSpace behindSpace = mCurrentSpace;
            for (int i = 0; i < mDefinition.Height && behindSpace != null; ++i)
                behindSpace = behindSpace.BackSpace;
            if (behindSpace == null || !behindSpace.Occupied)
                mCurrentSpace.Grid.HighlightBackRow(mCurrentSpace);
           
            
            if (mCurrentSpace.LeftSpace != null && mCurrentSpace.LeftSpace.Occupied) mCurrentSpace.LeftSpace.Highlight = true;
            if (mCurrentSpace.RightSpace != null && mCurrentSpace.RightSpace.Occupied) mCurrentSpace.RightSpace.Highlight = true;
        }
    }
}
