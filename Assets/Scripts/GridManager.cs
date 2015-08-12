using UnityEngine;
using System.Collections;
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

	[SerializeField]
	private int StartingTurns;

	public int Health { get; private set; }

	public int TurnsRemaining { get; private set; }

	private Queue<ShipDefinition> mReinforcementShips = new Queue<ShipDefinition>();
	public int Reinforcements { get { return mReinforcementShips.Count; } }


	public System.Action<int> OnTurnsChanged;
	public System.Action<int> OnHealthChanged;
	public System.Action<int> OnReinforcementsChanged;

	public void Reset()
	{
		Health = StartingHealth;

		if (OnHealthChanged != null)
		{
			OnHealthChanged.Invoke(Health);
		}

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

		if (OnReinforcementsChanged != null)
			OnReinforcementsChanged.Invoke(Reinforcements);
	}

	public void StartTurn()
	{
		TurnsRemaining = StartingTurns;
		if (OnTurnsChanged != null)
			OnTurnsChanged.Invoke(TurnsRemaining);
	}

	public void UseTurn()
	{
		--TurnsRemaining;
		if (OnTurnsChanged != null)
			OnTurnsChanged.Invoke(TurnsRemaining);
	}

	public void ReturnShip(ShipDefinition ship)
	{
		mReinforcementShips.Enqueue(ship);
		if (OnReinforcementsChanged != null)
			OnReinforcementsChanged.Invoke(Reinforcements);
	}

	public void SpawnReinforcements()
	{
		_SpawnReinforcements();

		GameContext.Instance.SetState(GameState.Action);
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

	private void CreateDebugString(int x, int y, int count, bool horizontal)
	{
		string debug = horizontal ? "H Match: " : "V Match: ";

		for (int i = 0; i < count; ++i)
		{
			debug += string.Format("<{0},{1}> ", horizontal ? x - i : x, horizontal ? y : y + i);
		}
		Debug.Log(debug);
		GameContext.Instance.StatusText.text += debug += "\n";
	}

	/// <summary>
	/// Check grid for horizontal and vertical matches
	/// </summary>
	public bool CheckMatches()
	{
		List<GridSpace> allSpaces = new List<GridSpace>();
		int combos = 0;

		GameContext.Instance.StatusText.text = "";

		//First check horizontal matches - only need to check idle ships, ships already acting don't match horizontally
		List<List<GridSpace>> horizontal_matches = new List<List<GridSpace>>();
		for (int y = Rows - 1; y >= 0; --y)
		{
			List<GridSpace> match = new List<GridSpace>();
			Ship ship = null;
			for (int x = 0; x < Columns; ++x)
			{
				GridSpace space = mGrid[x, y];
				if (space.Occupied)
				{
					if (ship == null)
					{
						if (space.Ship.Idle)
						{
							ship = space.Ship;
							match.Add(space);
						}		
						
						continue;
					}
					else
					{
						if (space.Ship.Idle && ship.CanCombine(space.Ship))
						{
							match.Add(space);
						}
						else
						{
							if (match.Count >= Constants.MinMatchSize)
							{
								horizontal_matches.Add(match);
								CreateDebugString(x, y, match.Count, true);

								allSpaces.Add(space);

								match = new List<GridSpace>();
							}
							else
							{
								match.Clear();
							}
							
							ship = space.Ship;
						}
					}
				}
				else
				{
					if (match.Count >= Constants.MinMatchSize)
					{
						horizontal_matches.Add(match);
						CreateDebugString(x, y, match.Count, true);

						allSpaces.Add(space);

						match = new List<GridSpace>();
					}
					else
					{
						match.Clear();
					}
					ship = null;
				}
			}
		}

		//Now check vertical matches - now we do check acting ships because groups can merge
		//Check if any matches contain spaces that are also horizontal matches - combo!
		List<List<GridSpace>> vertical_matches = new List<List<GridSpace>>();
		for (int x = 0; x < Columns; ++x)
		{
			List<GridSpace> match = new List<GridSpace>();
			Ship ship = null;
			for (int y = Rows - 1; y >= 0; --y)
			{
				GridSpace space = mGrid[x, y];
				if (space.Occupied)
				{
					if (ship == null)
					{
						ship = space.Ship;
						match.Add(space);
						continue;
					}
					else
					{
						if (ship.CanCombine(space.Ship))
						{
							match.Add(space);
						}
						else
						{
							if (match.Count >= Constants.MinMatchSize || (!ship.Idle && match.Count > 1))
							{
								vertical_matches.Add(match);
								CreateDebugString(x, y, match.Count, false);

								if (allSpaces.Contains(space))
								{
									++combos;
									GameContext.Instance.FireOnCombo(space);
								}
								else
									allSpaces.Add(space);

								match = new List<GridSpace>();
							}
							else
							{
								match.Clear();
							}

							ship = space.Ship;
						}
					}
				}
				else
				{
					if (ship != null && (match.Count >= Constants.MinMatchSize || (!ship.Idle && match.Count > 1)))
					{
						vertical_matches.Add(match);
						CreateDebugString(x, y, match.Count, false);

						if (allSpaces.Contains(space))
						{
							++combos;
							GameContext.Instance.FireOnCombo(space);
						}
						else
							allSpaces.Add(space);

						match = new List<GridSpace>();
					}
					else
					{
						match.Clear();
					}
					ship = null;
				}
			}
		}

		//All horizontal matches are defending only
		foreach (List<GridSpace> match in horizontal_matches)
		{
			foreach (GridSpace space in match)
			{
				space.Ship.SetDefending();
			}
		}

		//Vertical matches - 1st ship acts, remaining ships combine. if not all ships are defending then top ship attacks.
		foreach (List<GridSpace> match in vertical_matches)
		{
			bool allDefending = true;
			foreach (GridSpace space in match)
			{
				if (!space.Ship.Defending)
				{
					//Attack Match - combine ships with 1st non defending ship
					space.Ship.CombineAttackShips(match);
					allDefending = false;
					break;
				}
			}

			if (allDefending)
			{
				//Defense Match - combine ships with 1st ship
				match[0].Ship.CombineDefenseShips(match);			
			}
		}

		TurnsRemaining += combos;

		if (horizontal_matches.Count + vertical_matches.Count > 0)
		{
			TurnsRemaining += horizontal_matches.Count + vertical_matches.Count - 1;
			return true;
		}

		if (OnTurnsChanged != null)
			OnTurnsChanged.Invoke(TurnsRemaining);

		return false;
	}

	/// <summary>
	/// Reorder ships from front to back, defense first, then attack by multiplier then color, then any idle, and empty spaces last.
	/// </summary>
	public void MoveShips()
	{
		StartCoroutine(_MoveShips());
	}

	IEnumerator _MoveShips()
	{
		bool shipMoved = false;
		do 
		{
			shipMoved = false;
			for (int x = 0; x < Columns; ++x)
			{
				for (int y = Rows - 1; y > 0; --y)
				{
					GridSpace space = mGrid[x, y];
					GridSpace nextSpace = mGrid[x, y - 1];

					//if no ship in next space - no action
					if (nextSpace.Occupied)
					{
						if (space.Occupied)
						{
							//defending ship in front - no change
							if (space.Ship.Defending)
								continue;

							//defending ship in back or idle ship in front - swap ships
							if (nextSpace.Ship.Defending || (space.Ship.Idle && !nextSpace.Ship.Idle))
							{
								Ship previousShip = space.Ship;
								previousShip.GridSpace = null;

								nextSpace.Ship.GridSpace = space;
								previousShip.GridSpace = nextSpace;

								shipMoved = true;
								continue;
							}

							//both ships are attacking
							if (space.Ship.Attacking && nextSpace.Ship.Attacking)
							{
								//back ship has higher multiplier - swap ships
								if (nextSpace.Ship.Multiplier > space.Ship.Multiplier)
								{
									Ship previousShip = space.Ship;
									previousShip.GridSpace = null;

									nextSpace.Ship.GridSpace = space;
									previousShip.GridSpace = nextSpace;

									shipMoved = true;
									continue;
								}

								//ships are same multiplier, order by color
								if (nextSpace.Ship.Multiplier == space.Ship.Multiplier
									&& nextSpace.Ship.Color > space.Ship.Color)
								{
									Ship previousShip = space.Ship;
									previousShip.GridSpace = null;

									nextSpace.Ship.GridSpace = space;
									previousShip.GridSpace = nextSpace;

									shipMoved = true;
									continue;
								}

								continue;
							}

							//both ships are already in order - no change
						}
						else
						{
							//move ship into empty space
							nextSpace.Ship.GridSpace = space;
							shipMoved = true;
						}
					}
				}
			}
			if(shipMoved)
				yield return new WaitForSeconds(Constants.MoveShipWaitTime);
		} 
		while (shipMoved);

		GameContext.Instance.SetState(GameState.CheckMatches);
	}

}
