using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class TetrisBlock : MonoBehaviour
{
    private Vector3 firstTouchPosition;
    private Vector3 lastTouchPosition;
    private float dragDistance;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 30;
    public static int width = 15;
    private float[] timeTouchBegan;
    private float tapTime;
    private static  Transform[,] grid = new Transform[width,height];

    // Start is called before the first frame update
    void Start()
    {
        dragDistance = Screen.height * 5 / 100;
        timeTouchBegan = new float[10];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch tapTouch = Input.GetTouch(0);
            SwipeLeftRight(tapTouch);
        }

        if (Time.time - previousTime > 
            (Input.touchCount == 1 ? 
                (Input.GetTouch(0).phase == TouchPhase.Stationary ? fallTime / 10 : fallTime) : fallTime))
        {
            transform.position += new Vector3(0, -1, 0);

            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();
                this.enabled = false;
                FindObjectOfType<SpawnTetromino>().NewTetromino();
            }
            previousTime = Time.time;
        }
    }

    void CheckForLines()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }

        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y-1].transform.position -= new Vector3(0,1,0);
                }
            }
        }
    }

    void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }

    }

    public void Rotate()
    {
        this.transform.eulerAngles -= new Vector3(0, 0, 90);
        if (!ValidMove())
            this.transform.eulerAngles -= new Vector3(0, 0, -90);
    }

    bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3 ChildrenMove(Vector3 vector3)
    {
        transform.position += vector3;
        if (!ValidMove())
            transform.position -= vector3;
            
        return transform.position;
    }

    public void SwipeLeftRight(Touch swipeTouch)
    {
        int fingerIndex = swipeTouch.fingerId;

        if (swipeTouch.phase == TouchPhase.Began)
        {
            firstTouchPosition = swipeTouch.position;
            lastTouchPosition = swipeTouch.position;

            timeTouchBegan[fingerIndex] = Time.time;
        }
        else if (swipeTouch.phase == TouchPhase.Moved)
        {
            lastTouchPosition = swipeTouch.position;
        }
        else if (swipeTouch.phase == TouchPhase.Ended)
        {
            tapTime = Time.time - timeTouchBegan[fingerIndex];

            lastTouchPosition = swipeTouch.position;

            if (Math.Abs(lastTouchPosition.x - firstTouchPosition.x) > dragDistance
                || Math.Abs(lastTouchPosition.y - firstTouchPosition.y) > dragDistance)
            {
                if (Math.Abs(lastTouchPosition.x - firstTouchPosition.x) >
                    Math.Abs(lastTouchPosition.y - firstTouchPosition.y))
                {
                    if (lastTouchPosition.x > firstTouchPosition.x)
                    {
                        ChildrenMove(new Vector3(1, 0, 0));
                    }
                    else
                    {
                        ChildrenMove(new Vector3(-1, 0, 0));
                    }
                }
            }
            else
            {
                if (tapTime < 0.2f)
                {
                    Rotate();
                }
            }
        }
    }
}
