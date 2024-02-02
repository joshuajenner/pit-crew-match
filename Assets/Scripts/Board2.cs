using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board2 : MonoBehaviour
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
                while (HasMatchesAt(new Vector2Int(x, y), tilePrefabs[tileIndex]) && iterations < 100)
                {
                    tileIndex = Random.Range(0, tilePrefabs.Length);
                    iterations++;
                }

                SpawnTileInBoard(tilePrefabs[tileIndex], tilesParent.transform, x, y, tileHeight);
                SpawnObjectInBoard(backgroundPrefab, backgroundParent.transform, x, y, displayHeight);
            }
        }
    }

    private void SpawnTileInBoard(GameObject prefab, Transform parent, int gridX, int gridY, float height)
    {
        GameObject newTile = SpawnObjectInBoard(prefab, parent, gridX, gridY, height);
        allTiles[gridX, gridY] = newTile;
        Tile2 tile = newTile.GetComponent<Tile2>();
        tile.board = this;
        tile.coordinatesCurrent = new Vector2Int(gridX, gridY);
        tile.coordinatesTarget = new Vector2Int(gridX, gridY);
        tile.coordinatesPrevious = new Vector2Int(gridX, gridY);
    }

    private GameObject SpawnObjectInBoard(GameObject prefab, Transform parent, int gridX, int gridY, float height)
    {
        Vector3 gridPosition = new Vector3(gridX * tileScale, height, gridY * tileScale);

        GameObject newObject = Instantiate(prefab, parent);
        newObject.name = "(" + gridX.ToString() + ", " + gridY.ToString() + ")";
        newObject.transform.localPosition = gridPosition;

        return newObject;
    }



    public void SwipeTile(Vector2Int coords, float angle)
    {
        Vector2Int endCoords;

        if (angle > -45 && angle <= 45 && coords.x < boardWidth - 1) // Right
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
        else if (angle > 45 && angle <= 135 && coords.y < boardHeight) // Up
        {
            endCoords = new Vector2Int(coords.x, coords.y + 1);
        }
        else
        {
            return;
        }

        Tile2 startTile = allTiles[coords.x, coords.y].GetComponent<Tile2 >();
        Tile2 endTile = allTiles[endCoords.x, endCoords.y].GetComponent<Tile2 >();

        startTile.coordinatesTarget = endCoords;
        endTile.coordinatesTarget = coords;
        startTile.justSwiped = true;
        endTile.justSwiped = true;

        SetTileAtCoord(endCoords, startTile.gameObject);
        SetTileAtCoord(coords, endTile.gameObject);
    }

    public void SetTileAtCoord(Vector2Int coords, GameObject tile)
    {
        allTiles[coords.x, coords.y] = tile;
    }


    public IEnumerator DestroyMatchesAt(Vector2Int coords)
    {
        yield return new WaitForSeconds(0.1f);

        List<Vector2Int> matchedTiles = new List<Vector2Int>();

        Vector2Int up1 = new Vector2Int(coords.x, coords.y + 1);
        Vector2Int up2 = new Vector2Int(coords.x, coords.y + 2);
        Vector2Int down1 = new Vector2Int(coords.x, coords.y - 1);
        Vector2Int down2 = new Vector2Int(coords.x, coords.y - 2);
        Vector2Int left1 = new Vector2Int(coords.x - 1, coords.y);
        Vector2Int left2 = new Vector2Int(coords.x - 2, coords.y);
        Vector2Int right1 = new Vector2Int(coords.x + 1, coords.y);
        Vector2Int right2 = new Vector2Int(coords.x + 2, coords.y);

        if (coords.x > 0 && coords.x < (boardWidth - 1) && HasSameTags(left1, coords, right1))
        {
            matchedTiles.Add(left1);// OXO
            matchedTiles.Add(coords);
            matchedTiles.Add(right1);
        }
        if (coords.x > 1 && HasSameTags(left2, left1, coords))
        {
            matchedTiles.Add(left2);// OOX
            matchedTiles.Add(left1);
            matchedTiles.Add(coords);
        }
        if (coords.x < (boardWidth - 2) && HasSameTags(coords, right1, right2))
        {
            matchedTiles.Add(coords);// XOO
            matchedTiles.Add(right1);
            matchedTiles.Add(right2);
        }

        if (coords.y > 0 && coords.y < (boardHeight - 1) && HasSameTags(up1, coords, down1))
        {
            matchedTiles.Add(up1);//    O
            matchedTiles.Add(coords);// X
            matchedTiles.Add(down1);//  O
        }
        if (coords.y > 1 && HasSameTags(coords, down1, down2))
        {
            matchedTiles.Add(coords);// X
            matchedTiles.Add(down1);//  O
            matchedTiles.Add(down2);//  O
        }
        if (coords.y < (boardHeight - 2) && HasSameTags(up2, up1, coords))
        {
            matchedTiles.Add(up2);//    O
            matchedTiles.Add(up1);//    O
            matchedTiles.Add(coords);// X
        }

        if (matchedTiles.Count > 0)
        {
            DestoryMatchedTiles(matchedTiles);
        }
    }

    private bool HasSameTags(Vector2Int coord1, Vector2Int coord2, Vector2Int coord3)
    {
        Tile2 tile1 = allTiles[coord1.x, coord1.y].GetComponent<Tile2 >();
        Tile2 tile2 = allTiles[coord2.x, coord2.y].GetComponent<Tile2 >();
        Tile2 tile3 = allTiles[coord3.x, coord3.y].GetComponent<Tile2 >();

        if (!tile1.isMoving && !tile2.isMoving && !tile3.isMoving)
        {
            if (tile1.tag == tile2.tag)
            {
                if (tile2.tag == tile3.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private void DestoryMatchedTiles(List<Vector2Int> coordsList)
    {
        Debug.Log("Found " + coordsList.Count + " Tiles");

        List<Vector2Int> columnsToCollapse = new List<Vector2Int>();

        foreach (Vector2Int coords in coordsList)
        {
            GameObject tileObject = allTiles[coords.x, coords.y];

            if (tileObject != null)
            {
                allTiles[coords.x, coords.y] = null;
                Destroy(tileObject);
                columnsToCollapse.Add(coords);
            }
        }

        CollapseColumns(columnsToCollapse);

        Debug.Log("Destroyed " + columnsToCollapse.Count + " Tiles");
    }

    private void DropInTile(int column)
    {
        int tileIndex = Random.Range(0, tilePrefabs.Length);
        GameObject tileObject = Instantiate(tilePrefabs[tileIndex], tilesParent.transform);
        Tile2 newTile = tileObject.GetComponent<Tile2>();
        newTile.board = this;

        Vector3 gridPosition = new Vector3(column * tileScale, tileHeight, boardHeight * tileScale);
        tileObject.transform.localPosition = gridPosition;
        allTiles[column, boardHeight - 1] = tileObject;
        newTile.coordinatesCurrent = new Vector2Int(column, boardHeight);
        newTile.coordinatesPrevious = newTile.coordinatesCurrent;
        newTile.coordinatesTarget = new Vector2Int(column, boardHeight - 1);
    }

    private void CollapseColumns(List<Vector2Int> coords)
    {
        int dbg = 0;

        //for (int i = 0; i < tilesParent.transform.childCount; i++)
        //{
        //    Transform tileObject = tilesParent.transform.GetChild(i);
        //    Tile2 tile = tileObject.GetComponent<Tile2 >();

        //    if (!tile.justSwiped)
        //    {
        //        if (tile.coordinatesCurrent.x == coords.x && tile.coordinatesCurrent.y > coords.y)
        //        {
        //            tile.coordinatesTarget.y -= 1;

        //            if (tile.coordinatesCurrent.y < boardHeight)
        //            {
        //                allTiles[tile.coordinatesCurrent.x, tile.coordinatesCurrent.y] = null;
        //            }

        //            dbg++;
        //        }
        //    }

        //}

        foreach (Vector2Int coord in coords)
        {
            for (int row = coord.y + 1; row < boardHeight; row++)
            {
                GameObject tileObject = allTiles[coord.x, row];

                if (tileObject != null)
                {
                    allTiles[coord.x, row - 1] = tileObject;
                    tileObject.GetComponent<Tile2 >().coordinatesTarget.y -= 1;
                    dbg++;
                }
            }
            DropInTile(coord.x);
        }



        //Debug.Log("Collapsed: " + dbg + " tiles on Column " + coord.x);
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
            if (coords.y > 1)
            {
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

    private void DestroyMatchAt(Vector2Int coords)
    {
        Tile2 checkTile = allTiles[coords.x, coords.y].GetComponent<Tile2 >();

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
                    DestroyMatchAt(new Vector2Int(w, h));
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
            for (int y = 0; y < boardHeight; y++)
            {
                if (allTiles[x, y] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allTiles[x, y].GetComponent<Tile2 >().coordinatesTarget.y -= nullCount;
                    allTiles[x, y] = null;
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
