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

	void Start()
	{
		tiles = new Tile[width, height];
		InitBoard();
	}

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

	public void RequestMatchCheck(int row)
	{
		// Swapped Tiles Stil Moving
		if (swappedTiles[0] != null && swappedTiles[1] != null)
		{
            if (swappedTiles[0].isMoving || swappedTiles[1].isMoving)
            {
                return;
            }
        }

		// Row Hasn't Settled
		for (int x = 0; x < width; x++)
		{
			if (tiles[x, row] == null)
			{
				return;
			}
		}

		if (CheckMatches() == 0 && swappedTiles[0] != null)
		{
			SwapBackTiles();   
		}
	}

	public int CheckMatches()
	{
		int matches = 0;
		string previousTag;
		int concurrentTags;

		// Check Vertically / Columns
		for (int x = 0; x < width; x++)
		{
			concurrentTags = 0;
			previousTag = "";

			for (int y = 0; y < height; y++)
			{
				Tile checkTile = tiles[x, y];
				if (checkTile != null)
				{
					if (checkTile.tag == previousTag)
					{
						concurrentTags++;

                        if (concurrentTags >= 5)
                        {
                            concurrentTags = 0;
                            matches++;
                            previousTag = "";
                        }
                    }
					else
					{
						previousTag = checkTile.tag;
                        if (concurrentTags >= 3)
                        {
                            matches++;
                            previousTag = "";
                        }
                        concurrentTags = 1;
					}
				}
				else
				{
                    concurrentTags = 0;
                    previousTag = "";
                }
			}
		}

		// Check Horizontally / Rows
		for (int y = 0; y < height; y++)
		{
            concurrentTags = 0;
            previousTag = "";

            for (int x = 0; x < width; x++)
			{
                Tile checkTile = tiles[x, y];
                if (checkTile != null)
                {
                    if (checkTile.tag == previousTag)
                    {
                        concurrentTags++;

                        if (concurrentTags >= 5)
                        {
                            concurrentTags = 0;
                            matches++;
                            previousTag = "";
                        }
                    }
                    else
                    {
                        previousTag = checkTile.tag;
                        if (concurrentTags >= 3)
                        {
                            matches++;
                            previousTag = "";
                        }
                        concurrentTags = 1;
                    }
                }
                else
                {
                    concurrentTags = 0;
                    previousTag = "";
                }
            }
		}

		Debug.Log($"Found {matches}");

		return matches;
	}


	private void SwapBackTiles()
	{
		swappedTiles[0].MoveTo(swappedTiles[1].coordCurrent);
        swappedTiles[1].MoveTo(swappedTiles[0].coordCurrent);

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

		Tile startTile = tiles[coords.x, coords.y];
		Tile endTile = tiles[endCoords.x, endCoords.y];

		swappedTiles[0] = startTile;
		swappedTiles[1] = endTile;

		startTile.MoveTo(endCoords);
		endTile.MoveTo(coords);

		tiles[coords.x, coords.y] = null;
		tiles[endCoords.x, endCoords.y] = null;
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
}
