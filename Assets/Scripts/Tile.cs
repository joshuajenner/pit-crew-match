using Pixelplacement;
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
    public const float baseMoveTime = 1f;

    
    public void MoveTo(Vector2Int target, float moveTime)
    {
        isMoving = true;
        coordTarget = coordCurrent;
        Vector3 targetPosition = new Vector3(target.x * board.scale, board.tileHeight, target.y * board.scale);
        Tween.LocalPosition(transform, targetPosition, moveTime, 0, null, Tween.LoopType.None, null, OnMoveToFinished);
    }
    public void MoveTo(Vector2Int target)
    {
        MoveTo(target, baseMoveTime);
    }


    private void OnMoveToFinished()
    {
        isMoving = false;
        coordCurrent = coordTarget;
        board.tiles[coordCurrent.x, coordCurrent.y] = this;
        board.RequestMatchCheck(coordCurrent.y);
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

