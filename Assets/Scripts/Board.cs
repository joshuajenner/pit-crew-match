using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int boardWidth;
    public int boardHeight;
    public float tileScale;
    public float displayHeight;

    public GameObject tilesParent;
    public GameObject backgroundParent;

    public GameObject backgroundPrefab;

    private GameObject[,] allTiles;


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
                Vector3 gridPosition = new Vector3(y * tileScale, displayHeight, x * tileScale);
                gridPosition += transform.position;
                GameObject newBackground = Instantiate(backgroundPrefab, backgroundParent.transform);
                newBackground.transform.position = gridPosition;
            }
        }
    }
}
