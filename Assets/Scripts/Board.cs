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

}
