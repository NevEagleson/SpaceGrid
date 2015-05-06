using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour 
{
	public int Attack;
	public int Defense;
	public int Color;
	public int Multiplier;
	public ShipState State;
	public int AttackTurns;
	public GameObject Graphic;
	[SerializeField]
	public ColorLibrary ColorLib;
	[SerializeField]
	private SpriteRenderer ColorSprite;
	[SerializeField]
	private SpriteRenderer Sprite;
	public Animator Animator;

	public Vector3 SpritePos;

	public ShipDefinition Definition;

	public float MoveSpeed;

	private bool mMoving;

	public void OnEnable()
	{
		Reset();
	}

	public void Update()
	{
		Vector3 deltaPos = SpritePos - Graphic.transform.localPosition;
		deltaPos.z = 0;
		float sqrMoveDist = MoveSpeed * Time.deltaTime;
		sqrMoveDist *= sqrMoveDist;
		if (deltaPos.sqrMagnitude > sqrMoveDist)
		{
			if (!mMoving)
			{
				Animator.SetBool("Moving", true);
				mMoving = true;
			}
			deltaPos.Normalize();
			deltaPos *= MoveSpeed * Time.deltaTime;
			Graphic.transform.Translate(deltaPos);
		}
		else if(mMoving)
		{
			Graphic.transform.localPosition = SpritePos;
			Animator.SetBool("Moving", false);
			mMoving = false;
		}
	}

	public void Reset()
	{
		State = ShipState.Empty;
		Multiplier = 0;
		Attack = 0;
		Defense = 0;
		Definition = null;
		Sprite.sprite = null;
		ColorSprite.sprite = null;
		Animator.SetBool("Attacking", false);
		Animator.SetBool("Defending", false);
		Animator.SetBool("Moving", false);
		SpritePos = Vector3.zero;
		Graphic.transform.localPosition = Vector3.zero;
	}

	public void SetShip(ShipDefinition def, int color)
	{
		Definition = def;
		Color = color;

		Attack = Definition.Attack;
		Defense = Definition.Defense;

		Sprite.sprite = Definition.NormalSprite;
		ColorSprite.sprite = Definition.NormalSpriteColor;
		ColorSprite.color = ColorLib.Colors[Color].NormalColor;
	}

	public bool CanCombine(Ship other)
	{
		if (State == ShipState.Empty) return false;
		//color, multipler, ship type, and state must match
		bool sameType = other.Definition == Definition && other.Color == Color;
		bool sameState = other.State == State;
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
		if (State != ShipState.Attacking)
		{
			AttackTurns = Definition.AttackTurns;
		}

		++Multiplier;

		Attack = Definition.Attack * Multiplier;
		Defense = Mathf.CeilToInt(Definition.Defense * 0.5f) * Multiplier;

		State = ShipState.Attacking;
		Sprite.sprite = Definition.AttackingSprite;
		ColorSprite.sprite = Definition.AttackingSpriteColor;
		ColorSprite.color = ColorLib.Colors[Color].AttackLevels[Multiplier];
		Animator.SetBool("Attacking", true);
	}

	public void SetDefending()
	{
		//all defending ships are same color
		Color = 0;

		++Multiplier;
		
		Attack = 0;
		Defense = Definition.Defense * Multiplier;

		State = ShipState.Defending;
		Sprite.sprite = Definition.DefendingSprite;
		ColorSprite.sprite = Definition.DefendingSpriteColor;
		ColorSprite.color = ColorLib.Colors[Color].DefneseLevels[Multiplier];
		Animator.SetBool("Attacking", true);
	}
}
