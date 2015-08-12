using UnityEngine;
using System.Collections;

public class ShipDefinition : ScriptableObject
{
	public int Width;
	public int Height;
	public int Attack;
	public int Defense;
	public Sprite NormalSprite;
	public Sprite NormalSpriteColor;
	public Sprite AttackingSprite;
	public Sprite AttackingSpriteColor;
	public Sprite DefendingSprite;
	public Sprite DefendingSpriteColor;
	public Sprite AttackingSpritePower;
	public Sprite DefendingSpritePower;
	public int AttackTurns;
	public bool CanAttack;
	public bool CanDefend;
	public bool CanRepawn;
}
