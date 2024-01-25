using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;
    public float tileScale;
    public float displayHeight;
    public float tileHeight;

    public GameObject tilesParent;
    public GameObject backgroundParent;

    public GameObject backgroundPrefab;
    public GameObject[] tilePrefabs;



    public GameObject[,] allTiles;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GameObject[boardWidth, boardHeight];
        InitBoard();
    }

    private void InitBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                int tileIndex = Random.Range(0, tilePrefabs.Length);

                int iterations = 0;
                while (HasMatchesAt(new Vector2Int(x, y), tilePrefabs[tileIndex]) && iterations < 100) {
                    tileIndex = Random.Range(0, tilePrefabs.Length);
                    iterations++;
                }
                iterations = 0;

                SpawnTileInBoard(tilePrefabs[tileIndex], tilesParent.transform, x, y, tileHeight);
                SpawnObjectInBoard(backgroundPrefab, backgroundParent.transform, x, y, displayHeight);
            }
        }
    }


    private void SpawnTileInBoard(GameObject prefab, Transform parent, int gridX, int gridY, float height)
    {
        GameObject newTile = SpawnObjectInBoard(prefab, parent, gridX, gridY, height);
        allTiles[gridX, gridY] = newTile;
        Tile tile = newTile.GetComponent<Tile>();
        tile.board = this;
        tile.coordinatesCurrent = new Vector2Int(gridX, gridY);
        tile.coordinatesTarget = new Vector2Int(gridX, gridY);
        tile.coordinatesPrevious = new Vector2Int(gridX, gridY);
    }

    private GameObject SpawnObjectInBoard(GameObject prefab, Transform parent, int gridX, int gridY, float height)
    {
        Vector3 gridPosition = new Vector3(gridY * tileScale, height, gridX * tileScale);
        gridPosition += transform.position;

        GameObject newObject = Instantiate(prefab, parent);
        newObject.name = "(" + gridX.ToString() + ", " + gridY.ToString() + ")";
        newObject.transform.position = gridPosition;

        return newObject;
    }


    private bool HasMatchesAt(Vector2Int coords, GameObject checkTile) 
    {
        if (coords.x > 1 && coords.y > 1)
        {
            if (allTiles[coords.x - 1, coords.y].tag == checkTile.tag && allTiles[coords.x - 2, coords.y].tag == checkTile.tag)
            {
                return true;
            }

            if (allTiles[coords.x, coords.y - 1].tag == checkTile.tag && allTiles[coords.x, coords.y - 2].tag == checkTile.tag)
            {
                return true;
            }
        }
        else if (coords.x <= 1 || coords.y <= 1)
        {
            if (coords.y > 1) { 
                if (allTiles[coords.x, coords.y - 1].tag == checkTile.tag && allTiles[coords.x, coords.y - 2].tag == checkTile.tag)
                {
                    return true;
                }
            }
            if (coords.x > 1)
            {
                if (allTiles[coords.x - 1, coords.y].tag == checkTile.tag && allTiles[coords.x - 2, coords.y].tag == checkTile.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(Vector2Int coords)
    {
        Tile checkTile = allTiles[coords.x, coords.y].GetComponent<Tile>();

        if (checkTile.isMatched)
        {
            Destroy(checkTile.gameObject);
            allTiles[coords.x, coords.y] = null;
        }
    }


    public void DestroyMatches()
    {
        for (int w = 0; w < boardWidth; w++)
        {
            for (int h = 0; h < boardHeight; h++)
            {
                if (allTiles[w, h] != null)
                {
                    DestroyMatchesAt(new Vector2Int(w, h));
                }
            }
        }
        StartCoroutine(DescreaseRowCo());
    }

    private IEnumerator DescreaseRowCo()
    {
        int nullCount = 0;

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = boardHeight - 1; y >= 0; y--)
            {
                if (allTiles[x, y] == null)
                {
                    nullCount++;
                } 
                else if (nullCount > 0)
                {
                    allTiles[x, y].GetComponent<Tile>().coordinatesTarget.y += nullCount;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(0.6f);
    }

    //private int CycleIndex(int index, int maxIndex)
    //{
    //    int newIndex = index + 1;

    //    if (newIndex == maxIndex)
    //    {
    //        return 0;
    //    }
    //    else
    //    {
    //        return newIndex;
    //    }
    //}
}
