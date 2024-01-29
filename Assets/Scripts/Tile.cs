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

    public Vector2Int coordPrevious;
    public Vector2Int coordCurrent;
    public Vector2Int coordTarget;
    public bool isMoving = false;
    private float moveSpeed = 2f;
    private float positionSnapMinimum = 0.1f;


    private void Update()
    {
        if (coordCurrent != coordTarget)
        {
            if (!isMoving)
                isMoving = true;

            Vector3 targetPosition = new Vector3(coordTarget.x * board.scale, board.tileHeight, coordTarget.y * board.scale);

            if (Mathf.Abs((targetPosition - transform.localPosition).magnitude) > positionSnapMinimum)
            {
                // Tile Moving Towards Target
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                // Tile Arrived At Target
                transform.localPosition = targetPosition;
                coordCurrent = coordTarget;
                isMoving = false;
                HandleArrival();                
            }
        }
    }



    private void HandleArrival()
    {
        if (justSwiped)
        {
            Debug.Log($"Checking Swipe on {coordTarget.x}, {coordTarget.y}");
            if (!board.TileHasMatches(coordCurrent) && !board.TileHasMatches(coordPrevious)) {
                Debug.Log("No Matches");
                board.SwapBackTiles(coordCurrent, coordPrevious);
            }
            else
            {
                Debug.Log("Has Matches");
                justSwiped = false;
            }
        }
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

