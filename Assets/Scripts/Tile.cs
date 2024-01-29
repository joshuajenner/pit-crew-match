using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    public float swipeMinimum = 1f;
    public float swipeAngle = 0;
    public bool justSwiped = false;

    public Vector2Int coordCurrent;
    public Vector2Int coordTarget;
    public bool isMoving = false;


    private void Update()
    {

    }



    private void HandleSwipe()
    {
        if (Mathf.Abs((firstTouchPosition - lastTouchPosition).magnitude) > swipeMinimum)
        {
            swipeAngle = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            board.SwipeTiles(coordCurrent, swipeAngle);
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        lastTouchPosition = Input.mousePosition;
        HandleSwipe();
    }
}

