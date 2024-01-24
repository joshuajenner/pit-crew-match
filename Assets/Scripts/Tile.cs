using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Board board;

    public int tileIndex;
    public bool isMatched = false;

    public Vector2Int coordinatesCurrent;
    public Vector2Int coordinatesTarget;
    public Vector2Int coordinatesPrevious;
    
    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    public float swipeAngle = 0;

    private GameObject otherTile;



    private void Update()
    {
        FindMatches();

        if (coordinatesCurrent != coordinatesTarget)
        {
            Vector3 targetPosition = new Vector3(coordinatesTarget.y * board.tileScale, board.tileHeight, coordinatesTarget.x * board.tileScale);

            if (Mathf.Abs((targetPosition - transform.localPosition).magnitude) > 0.1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.2f);
            }
            else
            {
                transform.localPosition = targetPosition;
                coordinatesCurrent = coordinatesTarget;
                coordinatesPrevious = coordinatesTarget;
                board.allTiles[coordinatesCurrent.x, coordinatesCurrent.y] = this.gameObject;
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
        CalculateAngle();
    }


    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.5f);

        if (otherTile != null)
        {
            if (!isMatched && !otherTile.GetComponent<Tile>().isMatched)
            {
                otherTile.GetComponent<Tile>().coordinatesTarget = coordinatesCurrent;
                coordinatesTarget = otherTile.GetComponent<Tile>().coordinatesCurrent;
            }
            otherTile = null;
        }
    }


    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
        Debug.Log(swipeAngle);
        MoveTile();

    }

    private void MoveTile()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && coordinatesCurrent.x < board.boardWidth - 1)
        {
            // right swipe
            Debug.Log("Right");
            otherTile = board.allTiles[coordinatesTarget.x + 1, coordinatesTarget.y];
            otherTile.GetComponent<Tile>().coordinatesTarget.x -= 1;
            coordinatesTarget.x += 1;
        } 
        else if (swipeAngle > 45 && swipeAngle <= 135 && coordinatesCurrent.y > 0)
        {
            // up swipe
            Debug.Log("Up");
            otherTile = board.allTiles[coordinatesTarget.x, coordinatesTarget.y - 1];
            otherTile.GetComponent<Tile>().coordinatesTarget.y += 1;
            coordinatesTarget.y -= 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && coordinatesCurrent.x > 0)
        {
            // left swipe
            Debug.Log("Left");
            otherTile = board.allTiles[coordinatesTarget.x - 1, coordinatesTarget.y];
            otherTile.GetComponent<Tile>().coordinatesTarget.x += 1;
            coordinatesTarget.x -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && coordinatesCurrent.y < board.boardHeight - 1)
        {
            // down swipe
            Debug.Log("Down");
            otherTile = board.allTiles[coordinatesTarget.x, coordinatesTarget.y + 1];
            otherTile.GetComponent<Tile>().coordinatesTarget.y -= 1;
            coordinatesTarget.y += 1;
        }
        StartCoroutine(CheckMoveCo());
    }


    private void FindMatches()
    {
        if (coordinatesCurrent.x > 0 && coordinatesCurrent.x < board.boardWidth - 1)
        {
            GameObject leftTile1 = board.allTiles[coordinatesCurrent.x - 1, coordinatesCurrent.y];
            GameObject rightTile1 = board.allTiles[coordinatesCurrent.x + 1, coordinatesCurrent.y];

            if (leftTile1.tag == gameObject.tag && gameObject.tag == rightTile1.tag)
            {
                leftTile1.GetComponent<Tile>().SetMatched();
                SetMatched();
                rightTile1.GetComponent<Tile>().SetMatched();
            }
        }

        if (coordinatesCurrent.y > 0 && coordinatesCurrent.y < board.boardHeight - 1)
        {
            GameObject upTile1 = board.allTiles[coordinatesCurrent.x, coordinatesCurrent.y - 1];
            GameObject downTile1 = board.allTiles[coordinatesCurrent.x, coordinatesCurrent.y + 1];

            if (upTile1.tag == gameObject.tag && gameObject.tag == downTile1.tag)
            {
                upTile1.GetComponent<Tile>().SetMatched();
                SetMatched();
                downTile1.GetComponent<Tile>().SetMatched();
            }
        }

    }


    public void SetMatched()
    {
        isMatched = true;

        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = new Color(1f, 1f, 1f, 0.2f);
    }
}

