using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Board board;
    public int tileIndex;

    public Vector2Int coordinatesCurrent;
    public Vector2Int coordinatesTarget;

    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    public float swipeAngle = 0;

    private GameObject otherTile;

    public void SetTarget()
    {
        
    }


    private void OnMouseDown()
    {
        firstTouchPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        lastTouchPosition = Input.mousePosition;
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
        Debug.Log(swipeAngle);

    }

    private void MoveTile()
    {
        if (swipeAngle > -45 && swipeAngle <= 45)
        {
            // right swipe
            otherTile = board.allTiles[coordinatesCurrent.x + 1, coordinatesCurrent.y];
            otherTile.GetComponent<Tile>().coordinatesCurrent.x -= 1;
            coordinatesCurrent.x += 1;
        }
    }
}

