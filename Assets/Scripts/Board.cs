using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject tilePrefab;
    private GameObject[,] allTiles;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GameObject[width, height];
        InitBoard();
    }

    private void InitBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newContainer = Instantiate(tilePrefab, transform);
                newContainer.GetComponent<TileContainer>().Initialize();
            }
        }
    }
}
