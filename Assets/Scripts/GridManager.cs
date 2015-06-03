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
	[SerializeField]
	private int ReinforcementsPerTurn;

	public int Health { get; private set; }

	private Queue<ShipDefinition> mReinforcementShips;
	public int Reinforcements { get { return mReinforcementShips.Count; } }

	public void Reset()
	{
		Health = StartingHealth;

		mReinforcementShips = new Queue<ShipDefinition>();
		for (int i = 0; i < TotalReinforcements; ++i)
		{
			mReinforcementShips.Enqueue(ShipDefinition);
		}

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
                mGrid[x, y].BackSpace = y > 0 ? mGrid[x, y - 1] : null;
                mGrid[x, y].ForwardSpace = y < Rows - 1 ? mGrid[x, y + 1] : null;
                mGrid[x, y].LeftSpace = x > 0 ? mGrid[x - 1, y] : null;
                mGrid[x, y].RightSpace = x < Columns - 1 ? mGrid[x + 1, y] : null;                
            }
        }

		_SpawnReinforcements();
	}

	private void _SpawnReinforcements()
	{
		int ships2Spawn = ReinforcementsPerTurn;
		while (ships2Spawn > 0 && mReinforcementShips.Count > 0)
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
            so.GetComponent<Ship>().SetShip(mReinforcementShips.Dequeue(), ColorLib, Random.Range(0, 3), spawnSpace, SpawnOffset);

			mSpawnedShips.Add(spawnSpace.Ship);

			--ships2Spawn;
		}
	}

	public void ReturnShip(ShipDefinition ship)
	{
		mReinforcementShips.Enqueue(ship);
	}

	public void SpawnReinforcements()
	{
		_SpawnReinforcements();

		GameContext.Instance.GameState.SetTrigger("spawningDone");
	}

    public void GetGridLocation(GridSpace space, out int x, out int y)
    {
        x = 0;
        y = 0;
        for (int i = 0; i < Columns; ++i)
        {
            for (int j = 0; j < Rows; ++j)
            {
                if (mGrid[i, j] == space)
                {
                    x = i;
                    y = j;
                    return;
                }
            }
        }
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

    public void HighlightBackRow(GridSpace space)
    {
        for (int x = 0; x < Columns; ++x)
        {
            for (int y = 0; y < Rows; ++y)
            {
                if (mGrid[x, y].Occupied)
                {
                    if (mGrid[x, y].Ship != space.Ship && y > 0)
                    {
                        mGrid[x, y - 1].Highlight = true;
                    }
                    break;
                }
                if (y == Rows - 1)
                {
                    mGrid[x, y].Highlight = true;
                }
            }
        }
    }
}
