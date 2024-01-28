using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Tile2 : MonoBehaviour
{
    public Board2 board;

    public int tileIndex;
    public bool isMatched = false;

    public Vector2Int coordinatesCurrent;
    public Vector2Int coordinatesTarget;
    public Vector2Int coordinatesPrevious;

    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    public float swipeAngle = 0;
    public float swipeMinimum = 1f;

    public bool justSwiped = false;
    public bool isMoving = false;

    //private GameObject otherTile;



    private void Update()
    {
        //FindMatches();

        if (coordinatesCurrent != coordinatesTarget)
        {
            Vector3 targetPosition = new Vector3(coordinatesTarget.x * board.tileScale, board.tileHeight, coordinatesTarget.y * board.tileScale);

            if (Mathf.Abs((targetPosition - transform.localPosition).magnitude) > 0.1f)
            {
                // Tile Moving Towards Target
                //board.allTiles[coordinatesCurrent.x, coordinatesCurrent.y] = null;
                isMoving = true;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 4f * Time.deltaTime);
            }
            else
            {
                // Tile Arrived At Target
                transform.localPosition = targetPosition;
                coordinatesCurrent = coordinatesTarget;
                coordinatesPrevious = coordinatesTarget;

                justSwiped = false;
                isMoving = false;
                //board.SetTileAtCoord(coordinatesCurrent, this.gameObject);
                StartCoroutine(board.DestroyMatchesAt(coordinatesCurrent));
            }
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        lastTouchPosition = Input.mousePosition;
        SendSwipe();
    }

    private void SendSwipe()
    {
        if (Mathf.Abs((firstTouchPosition - lastTouchPosition).magnitude) > swipeMinimum)
        {
            swipeAngle = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            board.SwipeTile(coordinatesCurrent, swipeAngle);
        }
    }

    //public IEnumerator CheckMoveCo()
    //{
    //    yield return new WaitForSeconds(0.5f);

    //    if (otherTile != null)
    //    {
    //        if (!isMatched && !otherTile.GetComponent<Tile>().isMatched)
    //        {
    //            otherTile.GetComponent<Tile>().coordinatesTarget = coordinatesCurrent;
    //            coordinatesTarget = otherTile.GetComponent<Tile>().coordinatesCurrent;
    //        }
    //        else
    //        {
    //            board.DestroyMatches();
    //        }
    //        otherTile = null;
    //    }
    //}




    //private void MoveTile()
    //{
    //    if (swipeAngle > -45 && swipeAngle <= 45 && coordinatesCurrent.x < board.boardWidth - 1)
    //    {
    //        // right swipe
    //        Debug.Log("Right");
    //        otherTile = board.allTiles[coordinatesTarget.x + 1, coordinatesTarget.y];
    //        otherTile.GetComponent<Tile>().coordinatesTarget.x -= 1;
    //        coordinatesTarget.x += 1;
    //    } 
    //    else if (swipeAngle < -45 && swipeAngle >= -135 && coordinatesCurrent.y < board.boardHeight - 1)
    //    {
    //        // up swipe
    //        Debug.Log("Up");
    //        otherTile = board.allTiles[coordinatesTarget.x, coordinatesTarget.y - 1];
    //        otherTile.GetComponent<Tile>().coordinatesTarget.y += 1;
    //        coordinatesTarget.y -= 1;
    //    }
    //    else if ((swipeAngle > 135 || swipeAngle <= -135) && coordinatesCurrent.x > 0)
    //    {
    //        // left swipe
    //        Debug.Log("Left");
    //        otherTile = board.allTiles[coordinatesTarget.x - 1, coordinatesTarget.y];
    //        otherTile.GetComponent<Tile>().coordinatesTarget.x += 1;
    //        coordinatesTarget.x -= 1;
    //    }
    //    else if (swipeAngle > 45 && swipeAngle <= 135 && coordinatesCurrent.y > 0)
    //    {
    //        // down swipe
    //        Debug.Log("Down");
    //        otherTile = board.allTiles[coordinatesTarget.x, coordinatesTarget.y + 1];
    //        otherTile.GetComponent<Tile>().coordinatesTarget.y -= 1;
    //        coordinatesTarget.y += 1;
    //    }
    //    StartCoroutine(CheckMoveCo());
    //}


    private void FindMatches()
    {
        if (coordinatesCurrent.x > 0 && coordinatesCurrent.x < board.boardWidth - 1)
        {
            GameObject leftTile1 = board.allTiles[coordinatesCurrent.x - 1, coordinatesCurrent.y];
            GameObject rightTile1 = board.allTiles[coordinatesCurrent.x + 1, coordinatesCurrent.y];

            if (leftTile1 != null && rightTile1 != null)
            {
                if (leftTile1.tag == gameObject.tag && gameObject.tag == rightTile1.tag)
                {
                    leftTile1.GetComponent<Tile2>().SetMatched();
                    SetMatched();
                    rightTile1.GetComponent<Tile2>().SetMatched();
                }
            }
        }

        if (coordinatesCurrent.y > 0 && coordinatesCurrent.y < board.boardHeight - 1)
        {
            GameObject upTile1 = board.allTiles[coordinatesCurrent.x, coordinatesCurrent.y - 1];
            GameObject downTile1 = board.allTiles[coordinatesCurrent.x, coordinatesCurrent.y + 1];

            if (upTile1 != null && downTile1 != null)
            {
                if (upTile1.tag == gameObject.tag && gameObject.tag == downTile1.tag)
                {
                    upTile1.GetComponent<Tile2>().SetMatched();
                    SetMatched();
                    downTile1.GetComponent<Tile2>().SetMatched();
                }

            }
        }
    }


    public void SetMatched()
    {
        Debug.Log($"Set Matched ({coordinatesCurrent.x}, {coordinatesCurrent.y})");

        isMatched = true;

        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = new Color(1f, 1f, 1f, 0.2f);
    }
}
