using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	public int width;
	public int height;

	public float scale;
	public float tileHeight;
	public float backgroundHeight;

	public Transform tilesParent;
	public Transform backgroundParent;

	public GameObject backgroundPrefab;
	public GameObject[] tilePrefabs;

	public Tile[,] tiles;

	Tile[] swappedTiles = new Tile[2];

	private bool isCheckQueued = false;

	void Start()
	{
		tiles = new Tile[width, height];
		InitBoard();
	}

    private void FixedUpdate()
    {
		if (isCheckQueued && IsBoardSettled())
		{
			isCheckQueued = false;
			HandleMatchCheck();
		}
	}


	private void HandleMatchCheck()
	{
        List<List<Tile>> matchedTiles = CheckMatches();

        if (matchedTiles.Count == 0)
        {
			Debug.Log("No Matches Found");

            if (swappedTiles[0] != null && swappedTiles[1] != null)
            {
                SwapBackTiles();
            }
        }
		else
		{
            DestoryMatches(matchedTiles);
			CollapseColumns();
			DropAllTiles();
			QueueMatchCheck();
        }
    }

	public void QueueMatchCheck()
	{
		isCheckQueued = true;
	}

    public void RequestMatchCheck2(int row)
	{
		// Swapped Tiles Still Moving
		if (swappedTiles[0] != null && swappedTiles[1] != null)
		{
            if (swappedTiles[0].isMoving || swappedTiles[1].isMoving)
            {
                return;
            }
        }
		//Debug.Log(row);

		//Row Hasn't Settled
		//for (int x = 0; x < width; x++)
		//{
		//	if (tiles[x, row] == null || tiles[x, row].isMoving)
		//	{
		//		Debug.Log("Row Moving Still");
		//		return;
		//	}
		//}

		List<List<Tile>> matchedTiles = CheckMatches();

        if (matchedTiles.Count == 0)
		{
			if (swappedTiles[0] != null && swappedTiles[1] != null)
			{
                SwapBackTiles();
            }
			return;
		}
		
		//foreach (List<Tile> tileSet in matchedTiles)
		//{
		//	Debug.Log($"{tileSet[0].tag} Match");
		//}

		
		CollapseColumns();
	}

    public void DestoryMatches(List<List<Tile>> matches)
    {
        foreach (List<Tile> matchSet in matches)
        {
            foreach (Tile tile in matchSet)
            {
                if (tiles[tile.coordCurrent.x, tile.coordCurrent.y] != null)
                {
                    tiles[tile.coordCurrent.x, tile.coordCurrent.y] = null;
                    Destroy(tile.gameObject);
                }
            }
        }
    }

    public void CollapseColumnsAndDropTiles()
    {
        for (int x = 0; x < width; x++)
        {
            bool isFirst = true;

            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    for (int t = y; t < height; t++)
                    {
                        if (tiles[x, t] != null)
                        {
                            float moveTime = Tile.baseMoveTime;
                            if (isFirst)
                            {
                                isFirst = false;
                                moveTime = Tile.firstMoveTime;
                            }
                            MoveTileTo(tiles[x, t], new Vector2Int(x, y), moveTime);
                            break;
                        }
                    }
                }
            }
        }
    }

	private void DropAllTiles()
	{
		for (int x = 0; x < width; x++)
		{
			int count = 1;
			for (int y = 0; y < height; y++)
			{
				if (tiles[x, y] == null)
				{
					DropTile(new Vector2Int(x, y), count);
					count++;
                }
			}
		}
	}

	private void DropTile(Vector2Int coord, int tileNum)
	{
        int tileIndex = Random.Range(0, tilePrefabs.Length);
        GameObject newObject = Instantiate(tilePrefabs[tileIndex], tilesParent);
		newObject.transform.localPosition = new Vector3(coord.x * scale, tileHeight, (height + tileNum) * scale);

        Tile newTile = newObject.GetComponent<Tile>();
        newTile.board = this;
		newTile.MoveTo(coord);

        tiles[coord.x, coord.y] = newTile;
    }

    public void CollapseColumns()
	{
		for (int x = 0; x < width; x++)
		{
			bool isFirst = true;
            for (int y = 0; y < height; y++)
			{
				if (tiles[x, y] == null)
				{
					for (int t = y; t < height; t++)
					{
						if (tiles[x, t] != null)
						{
							float moveTime = Tile.baseMoveTime;
							if (isFirst)
							{
								isFirst = false;
								moveTime = Tile.firstMoveTime;
							}
							MoveTileTo(tiles[x, t], new Vector2Int(x, y), moveTime);
                            break;
						}
					}
                }
			}
		}
	}


	private void MoveTileTo(Tile tile, Vector2Int newCoord, float inTime)
	{
        tiles[newCoord.x, newCoord.y] = tile;
        tiles[tile.coordCurrent.x, tile.coordCurrent.y] = null;
		tile.MoveTo(newCoord, inTime);
	}
    private void MoveTileTo(Tile tile, Vector2Int newCoord)
    {
		MoveTileTo(tile, newCoord, Tile.firstMoveTime);
    }

    private List<List<Tile>> CheckMatches()
	{
		//Debug.Log("Checking");

		List<List<Tile>> matchedTiles = new List<List<Tile>>();
		List<Tile> concurrentTiles = new List<Tile>();
        string previousTag;
		//int concurrentTags;

		// Check Vertically / Columns
		for (int x = 0; x < width; x++)
		{
            concurrentTiles.Clear();
			previousTag = "";

			for (int y = 0; y < height; y++)
			{
				Tile checkTile = tiles[x, y];
				if (checkTile != null && !checkTile.isMoving)
				{
					if (checkTile.tag == previousTag)
					{
                        concurrentTiles.Add(checkTile);

                        if (concurrentTiles.Count >= 5)
                        {
                            matchedTiles.Add(concurrentTiles);
                            concurrentTiles = new List<Tile>{};
                            previousTag = "";
                        }
                    }
					else
					{
						previousTag = checkTile.tag;
                        if (concurrentTiles.Count >= 3)
                        {
                            matchedTiles.Add(concurrentTiles);
                            previousTag = "";
                        }
                        concurrentTiles = new List<Tile>{checkTile};
					}
				}
				else
				{
                    concurrentTiles.Clear();
                    previousTag = "";
                }
			}
		}

		// Check Horizontally / Rows
		for (int y = 0; y < height; y++)
		{
            concurrentTiles.Clear();
            previousTag = "";

            for (int x = 0; x < width; x++)
			{
                Tile checkTile = tiles[x, y];
                if (checkTile != null && !checkTile.isMoving)
                {
                    if (checkTile.tag == previousTag)
                    {
                        concurrentTiles.Add(checkTile);

                        if (concurrentTiles.Count >= 5)
                        {
                            matchedTiles.Add(concurrentTiles);
                            concurrentTiles = new List<Tile>{};
                            previousTag = "";
                        }
                    }
                    else
                    {
                        previousTag = checkTile.tag;
                        if (concurrentTiles.Count >= 3)
                        {
                            matchedTiles.Add(concurrentTiles);
                            previousTag = "";
                        }
                        concurrentTiles = new List<Tile> {checkTile};
                    }
                }
                else
                {
                    concurrentTiles.Clear();
                    previousTag = "";
                }
            }
		}

		

		return matchedTiles;
	}

	public void DebugMatches()
	{
        Debug.Log($"Found {CheckMatches().Count} Matches");
    }

	private void SwapBackTiles()
	{
		Tile tile1 = swappedTiles[0];
		Tile tile2 = swappedTiles[1];

		tiles[tile1.coordCurrent.x, tile1.coordCurrent.y] = tile2;
		tiles[tile2.coordCurrent.x, tile2.coordCurrent.y] = tile1;

        tile1.MoveTo(swappedTiles[1].coordCurrent);
        tile2.MoveTo(swappedTiles[0].coordCurrent);

        swappedTiles[0] = null;
        swappedTiles[1] = null;
    }

	public void SwipeTiles(Vector2Int coords, float angle)
	{
		Vector2Int endCoords;

		if (angle > -45 && angle <= 45 && coords.x < width - 1) // Right
		{
			endCoords = new Vector2Int(coords.x + 1, coords.y);
		}
		else if (angle < -45 && angle >= -135 && coords.y > 0) // Down
		{
			endCoords = new Vector2Int(coords.x, coords.y - 1);
		}
		else if ((angle > 135 || angle <= -135) && coords.x > 0) // Left
		{
			endCoords = new Vector2Int(coords.x - 1, coords.y);
		}
		else if (angle > 45 && angle <= 135 && coords.y < height) // Up
		{
			endCoords = new Vector2Int(coords.x, coords.y + 1);
		}
		else
		{
			return;
		}

		QueueMatchCheck();

		Tile startTile = tiles[coords.x, coords.y];
		Tile endTile = tiles[endCoords.x, endCoords.y];

		swappedTiles[0] = startTile;
		swappedTiles[1] = endTile;

        tiles[endCoords.x, endCoords.y] = startTile;
        tiles[coords.x, coords.y] = endTile;

		startTile.MoveTo(endCoords);
        endTile.MoveTo(coords);
    }

	//public bool IsTileBelowNull(Vector2Int coord)
	//{
	//	int checkX = Mathf.Clamp(coord.x, 0, width - 1);
	//	int checkY = Mathf.Clamp(coord.y - 1, 0, height - 1);

	//	if (tiles[checkX, checkY] == null)
	//	{
	//		return true;
	//	}
	//	return false;
	//}


    private void InitBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileIndex = Random.Range(0, tilePrefabs.Length);
                int iterations = 0;
                while (HasMatchesAtSpawn(new Vector2Int(x, y), tilePrefabs[tileIndex]) && iterations < 100)
                {
                    tileIndex = Random.Range(0, tilePrefabs.Length);
                    iterations++;
                }

                SpawnTile(tilePrefabs[tileIndex], x, y);
                SpawnObjectInBoard(backgroundPrefab, backgroundParent, x, y, backgroundHeight);
            }
        }
    }

    private void SpawnTile(GameObject prefab, int x, int y)
	{
		GameObject newObject = SpawnObjectInBoard(prefab, tilesParent, x, y, tileHeight);
		Tile newTile = newObject.GetComponent<Tile>();
		newTile.board = this;
		newTile.coordCurrent = new Vector2Int(x, y);
		newTile.coordTarget = new Vector2Int(x, y);

		tiles[x, y] = newTile;
	}

	private GameObject SpawnObjectInBoard(GameObject prefab, Transform parent, int gridX, int gridY, float height)
	{
		Vector3 gridPosition = new Vector3(gridX * scale, height, gridY * scale);
		GameObject newObject = Instantiate(prefab, parent);
		newObject.transform.localPosition = gridPosition;
		//newObject.name = "(" + gridX.ToString() + ", " + gridY.ToString() + ")";
		return newObject;
	}

	private bool HasMatchesAtSpawn(Vector2Int coords, GameObject checkTile)
	{
		if (coords.x > 1 && coords.y > 1)
		{
			if (tiles[coords.x - 1, coords.y].tag == checkTile.tag && tiles[coords.x - 2, coords.y].tag == checkTile.tag)
			{
				return true;
			}

			if (tiles[coords.x, coords.y - 1].tag == checkTile.tag && tiles[coords.x, coords.y - 2].tag == checkTile.tag)
			{
				return true;
			}
		}
		else if (coords.x <= 1 || coords.y <= 1)
		{
			if (coords.y > 1)
			{
				if (tiles[coords.x, coords.y - 1].tag == checkTile.tag && tiles[coords.x, coords.y - 2].tag == checkTile.tag)
				{
					return true;
				}
			}
			if (coords.x > 1)
			{
				if (tiles[coords.x - 1, coords.y].tag == checkTile.tag && tiles[coords.x - 2, coords.y].tag == checkTile.tag)
				{
					return true;
				}
			}
		}
		return false;
	}

    private bool IsBoardSettled()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null || tiles[x, y].isMoving)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
