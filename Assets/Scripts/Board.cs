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








    public bool TileHasMatches(Vector2Int coord)
    {
        Vector2Int up1 = new Vector2Int(coord.x, coord.y + 1);
        Vector2Int up2 = new Vector2Int(coord.x, coord.y + 2);
        Vector2Int down1 = new Vector2Int(coord.x, coord.y - 1);
        Vector2Int down2 = new Vector2Int(coord.x, coord.y - 2);
        Vector2Int left1 = new Vector2Int(coord.x - 1, coord.y);
        Vector2Int left2 = new Vector2Int(coord.x - 2, coord.y);
        Vector2Int right1 = new Vector2Int(coord.x + 1, coord.y);
        Vector2Int right2 = new Vector2Int(coord.x + 2, coord.y);

        if (coord.x > 0 && coord.x < (width - 1) && HasSameTags(left1, coord, right1))
        {
            return true;
        }
        if (coord.x > 1 && HasSameTags(left2, left1, coord))
        {
            return true;
        }
        if (coord.x < (width - 2) && HasSameTags(coord, right1, right2))
        {
            return true;
        }

        if (coord.y > 0 && coord.y < (height - 1) && HasSameTags(up1, coord, down1))
        {
            return true;
        }
        if (coord.y > 1 && HasSameTags(coord, down1, down2))
        {
            return true;
        }
        if (coord.y < (height - 2) && HasSameTags(up2, up1, coord))
        {
            return true;
        }
        return false;
    }


    private bool HasSameTags(Vector2Int coord1, Vector2Int coord2, Vector2Int coord3)
    {
        Tile tile1 = tiles[coord1.x, coord1.y];
        Tile tile2 = tiles[coord2.x, coord2.y];
        Tile tile3 = tiles[coord3.x, coord3.y];

        if (!tile1.isMoving && !tile2.isMoving && !tile3.isMoving)
        {
            if (tile1.tag == tile2.tag && tile2.tag == tile3.tag)
            {
                return true;
            }
        }
        return false;
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

        Debug.Log(coords);

        Tile startTile = tiles[coords.x, coords.y];
        Tile endTile = tiles[endCoords.x, endCoords.y];

        startTile.coordTarget = endCoords;
        endTile.coordTarget = coords;
        startTile.justSwiped = true;
        endTile.justSwiped= true;

        tiles[coords.x, coords.y] = endTile;
        tiles[endCoords.x, endCoords.y] = startTile;

        Debug.Log("Swapped");
    }

    public void SwapBackTiles(Vector2Int start, Vector2Int end)
    {
        Tile startTile = tiles[start.x, start.y];
        Tile endTile = tiles[end.x, end.y];

        startTile.coordTarget = end;
        endTile.coordCurrent = end;
        endTile.coordTarget = start;

        startTile.justSwiped = false;
        endTile.justSwiped = false;

        tiles[start.x, start.y] = endTile;
        tiles[end.x, end.y] = startTile;
    }


    private void SpawnTile(GameObject prefab, int x, int y)
    {
        GameObject newObject = SpawnObjectInBoard(prefab, tilesParent, x, y, tileHeight);
        Tile newTile = newObject.GetComponent<Tile>();
        newTile.board = this;
        newTile.coordPrevious = new Vector2Int(x, y);
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
