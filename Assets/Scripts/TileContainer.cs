using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContainer : MonoBehaviour
{
    public GameObject[] tiles;

    public void Initialize()
    {
        int tileIndex = Random.Range(0, tiles.Length);
        GameObject newTile = Instantiate(tiles[tileIndex], this.transform);
    }
}
