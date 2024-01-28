using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public float tileScale;
    public float displayHeight;
    public float tileHeight;

    public Transform tilesParent;
    public Transform backgroundParent;

    public GameObject backgroundPrefab;
    public GameObject[] tilePrefabs;

    private Tile[,] tiles;


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
                while (HasMatchesAt(new Vector2Int(x, y), tilePrefabs[tileIndex]) && iterations < 100)
                {
                    tileIndex = Random.Range(0, tilePrefabs.Length);
                    iterations++;
                }

                SpawnTile(tilePrefabs[tileIndex], x, y);
                SpawnObjectInBoard(backgroundPrefab, backgroundParent, x, y, displayHeight);
            }
        }
    }










    private void SpawnTile(GameObject prefab, int x, int y)
    {
        GameObject newObject = SpawnObjectInBoard(prefab, tilesParent, x, y, tileHeight);
        Tile newTile = newObject.GetComponent<Tile>();
        newTile.board = this;
        newTile.positionTarget = new Vector2Int(x, y);

        Debug.Log(x + " / " + y);

        tiles[x, y] = newTile;
    }

    private GameObject SpawnObjectInBoard(GameObject prefab, Transform parent, int gridX, int gridY, float height)
    {
        Vector3 gridPosition = new Vector3(gridX * tileScale, height, gridY * tileScale);
        GameObject newObject = Instantiate(prefab, parent);
        newObject.transform.localPosition = gridPosition;
        //newObject.name = "(" + gridX.ToString() + ", " + gridY.ToString() + ")";
        return newObject;
    }

    private bool HasMatchesAt(Vector2Int coords, GameObject checkTile)
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
