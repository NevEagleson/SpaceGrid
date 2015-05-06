using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour 
{
	public GameObject GridPrefab;
	public int Columns = 8;
	public int Rows = 6;

	public ShipDefinition ShipDefinition;
	public Vector3 SpawnOffset;

	private GridSpace[,] mGrid;
	private bool mGridVisible;
	private List<Ship> mSpawnedShips;

	[SerializeField]
	private int StartingHealth;

	[SerializeField]
	private int TotalReinforcements;

	public int Health { get; private set; }

	public int Reinforcements { get; private set; }

	public void Reset()
	{
		Health = StartingHealth;
		Reinforcements = TotalReinforcements;

		for (int i = 0; i < transform.childCount; ++i)
			DestroyObject(transform.GetChild(i).gameObject);

		mGrid = new GridSpace[Columns, Rows];

		mSpawnedShips = new List<Ship>();

		for (int y = 0; y < Rows; ++y)
		{
			for (int x = 0; x < Columns; ++x)
			{

				GameObject go = Instantiate(GridPrefab) as GameObject;
				go.transform.SetParent(transform, false);
				GridSpace space = go.GetComponentInChildren<GridSpace>();
				mGrid[x, y] = space;
				space.Grid = this;
			}
		}

		_SpawnReinforcements();
	}

	private void _SpawnReinforcements()
	{
		while (Reinforcements > 0)
		{
			//first check if we can spawn anything at all
			bool canSpawn = false;
			for (int x = 0; x < Columns; ++x)
				canSpawn |= !mGrid[x, 0].Occupied;
			if (!canSpawn) break;

			//pick random unoccupied column
			int spawnCol = Random.Range(0, Columns);
			while (mGrid[spawnCol, 0].Occupied)
				spawnCol = Random.Range(0, Columns);

			//now pick last unoccupied row
			int spawnRow = Rows - 1;
			while (spawnRow > 0 && mGrid[spawnCol, spawnRow].Occupied)
				--spawnRow;

			GridSpace spawnSpace = mGrid[spawnCol, spawnRow];
			spawnSpace.Occupied = true;
			spawnSpace.Ship.SpritePos = Vector3.zero;
			spawnSpace.Ship.Graphic.transform.localPosition = SpawnOffset;
			spawnSpace.Ship.SetShip(ShipDefinition, Random.Range(0, 3));

			mSpawnedShips.Add(spawnSpace.Ship);

			--Reinforcements;
		}
	}

	public void SpawnReinforcements()
	{
		_SpawnReinforcements();

		GameContext.Instance.GameState.SetTrigger("spawningDone");
	}

	public bool GridVisible
	{
		get { return mGridVisible; }
		set
		{
			mGridVisible = value;
			for (int x = 0; x < Columns; ++x)
			{
				for (int y = 0; y < Rows; ++y)
				{
					mGrid[x, y].GridVisible = value;
				}
			}
		}
	}

	public void OnSelect(GridSpace space)
	{
		for (int x = 0; x < Columns; ++x)
		{
			for (int y = 0; y < Rows; ++y)
			{
				if (space != mGrid[x, y])
					mGrid[x, y].Deselect();
			}
		}
	}
}
