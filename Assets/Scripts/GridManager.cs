using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour 
{
	public GameObject GridPrefab;
	public int Columns = 8;
	public int Rows = 6;

    public GameObject ShipPrefab;
	public ShipDefinition ShipDefinition;
    public ColorLibrary ColorLib;
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

        //generate grid
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

        //link grid
        for (int y = 0; y < Rows; ++y)
        {
            for (int x = 0; x < Columns; ++x)
            {
                mGrid[x, y].ForwardSpace = y > 0 ? mGrid[x, y - 1] : null;
                mGrid[x, y].BackSpace = y < Rows - 1 ? mGrid[x, y + 1] : null;
                mGrid[x, y].LeftSpace = x > 0 ? mGrid[x - 1, y] : null;
                mGrid[x, y].RightSpace = x < Columns - 1 ? mGrid[x + 1, y] : null;                
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

            GameObject so = Instantiate<GameObject>(ShipPrefab);
            so.transform.SetParent(transform, false);
            so.GetComponent<Ship>().SetShip(ShipDefinition, ColorLib, Random.Range(0, 3), spawnSpace, SpawnOffset);

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
