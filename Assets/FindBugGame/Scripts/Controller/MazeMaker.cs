using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MazeMaker : MonoBehaviour
{
    public int rows;
    public int columns;

    [SerializeField] private GameObject m_cellPrefab;
    [SerializeField] private bool m_disableSprite;

    [SerializeField] private Transform m_mazeHolder;
    [SerializeField] private Transform m_background;
    private Dictionary<Vector2, CellData> m_cells;
    private List<CellData> m_unvisitedCells;
    private Queue<CellData> m_queue = new Queue<CellData>();

    private CellData[] m_cornerCells;
    private CellData m_currentCell;
    private CellData m_tempCell;

    private Vector2[] m_neighBourRelativePositions;

    private Camera m_cam;

    //square 64 pixel with 100 px per unit
    private float m_cellSize = 0.64f;
    private float m_wallSize = 0.064f;

    #region MazeGenerateLogic
    // Start is called before the first frame update
    public void Init()
    {
        InitValues();
        CreateMazeLayout();
        ResizeBackground();
        AutoFitCamera();
        GenerateMaze();
    }

    private void InitValues()
    {
        m_cells = new Dictionary<Vector2, CellData>();
        m_unvisitedCells = new List<CellData>();
        m_cornerCells = new CellData[4];
        m_queue = new Queue<CellData>();
        m_neighBourRelativePositions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };
    }

    private void CreateMazeLayout()
    {
        float distanceBetweenCells = m_cellSize / 2 + m_wallSize / 2;
        Vector2 startPos = new Vector2(distanceBetweenCells, -distanceBetweenCells);

        Vector2 spawnPos = startPos;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GenerateCell(spawnPos, new Vector2(x, y));
                spawnPos.y -= distanceBetweenCells * 2;
            }
            spawnPos.y = startPos.y;
            spawnPos.x += distanceBetweenCells * 2;
        }
        m_cornerCells[0] = m_cells[Vector2.zero];
        m_cornerCells[0].cellComp.HideWall(Direction.Right);
        m_cornerCells[0].cellComp.HideWall(Direction.Down);
        m_cornerCells[1] = m_cells[new Vector2(columns - 1, 0)];
        m_cornerCells[1].cellComp.HideWall(Direction.Left);
        m_cornerCells[1].cellComp.HideWall(Direction.Down);
        m_cornerCells[2] = m_cells[new Vector2(0, rows - 1)];
        m_cornerCells[2].cellComp.HideWall(Direction.Right);
        m_cornerCells[2].cellComp.HideWall(Direction.Up);
        m_cornerCells[3] = m_cells[new Vector2(columns - 1, rows - 1)];
        m_cornerCells[3].cellComp.HideWall(Direction.Left);
        m_cornerCells[3].cellComp.HideWall(Direction.Up);
    }

    private void GenerateCell(Vector2 spawnPos, Vector2 positionInGrid)
    {
        CellData newCell = new CellData();
        newCell.pos = positionInGrid;
        newCell.cellComp = Instantiate(m_cellPrefab, spawnPos, m_cellPrefab.transform.rotation, m_mazeHolder).GetComponent<Cell>();
        newCell.cellComp.gameObject.name = "Cell - X: " + positionInGrid.x + " Y: " + positionInGrid.y;
        if (m_disableSprite) newCell.cellComp.ToggleSprite(false);

        m_cells[positionInGrid] = newCell;
        m_unvisitedCells.Add(newCell);
    }

    private void ResizeBackground()
    {
        SpriteRenderer backgroundSprite = m_background.GetComponent<SpriteRenderer>();
        float backgroundWidth = backgroundSprite.bounds.size.x;
        float backgroundHeight = backgroundSprite.bounds.size.y;
        m_background.position = Vector3.zero;
        m_background.localScale = new Vector2(
            (columns * m_cellSize + columns * m_wallSize) / backgroundWidth,
            (rows * m_cellSize + rows * m_wallSize) / backgroundHeight);
    }

    private void AutoFitCamera()
    {
        m_cam = Camera.main;
    }


    private void GenerateMaze()
    {
        //BFS logic
        //1.Randomly select a node(or cell) N.
        m_currentCell = m_cells[new Vector2(UnityEngine.Random.Range(0, columns), UnityEngine.Random.Range(0, rows))];
        //2.Push the node N onto a queue Q.
        m_queue.Enqueue(m_currentCell);
        //3.Mark the cell N as visited.
        m_unvisitedCells.Remove(m_currentCell);
        //4.Randomly select an adjacent cell A of node N that has not been visited.If all the neighbors of N have been visited:
        //    Continue to pop items off the queue Q until a node is encountered with at least one non-visited neighbor - assign this node to N and go to step 4.
        //    If no nodes exist: stop.

        while (m_currentCell != null)
        {
            List<CellData> unvisitedNeighbours = GetUnvisitedNeighbours();
            if (unvisitedNeighbours.Count > 0)
            {
                m_tempCell = unvisitedNeighbours[UnityEngine.Random.Range(0, unvisitedNeighbours.Count)];
                //5.Break the wall between N and A.
                BreakWallBetween(m_tempCell, m_currentCell);
                //6.Assign the value A to N.
                m_currentCell = m_tempCell;
                //7.Go to step 2.
                m_queue.Enqueue(m_currentCell);
                m_unvisitedCells.Remove(m_currentCell);
            }
            else
            {
                m_currentCell = m_queue.Count > 0 ? m_queue.Dequeue() : null;
            }
        }
    }


    private void BreakWallBetween(CellData cell1, CellData cell2)
    {
        if (cell1.pos.x < cell2.pos.x)
        {
            cell1.cellComp.HideWall(Direction.Right);
            cell2.cellComp.HideWall(Direction.Left);
        }
        else if (cell1.pos.x > cell2.pos.x)
        {
            cell1.cellComp.HideWall(Direction.Left);
            cell2.cellComp.HideWall(Direction.Right);
        }
        else if (cell1.pos.y > cell2.pos.y)
        {
            cell1.cellComp.HideWall(Direction.Up);
            cell2.cellComp.HideWall(Direction.Down);
        }
        else if (cell1.pos.y < cell2.pos.y)
        {
            cell1.cellComp.HideWall(Direction.Down);
            cell2.cellComp.HideWall(Direction.Up);
        }
    }

    private List<CellData> GetUnvisitedNeighbours()
    {
        List<CellData> unvisitedNeighbours = new List<CellData>();
        foreach (Vector2 neighbourRelativePos in m_neighBourRelativePositions)
        {
            Vector2 neighbourPos = m_currentCell.pos + neighbourRelativePos;
            if (IsPositionWithinMaze(neighbourPos)) {
                CellData neighbourCell = m_cells[neighbourPos];
                if (m_unvisitedCells.Contains(neighbourCell)) unvisitedNeighbours.Add(neighbourCell);
            }
        }
        return unvisitedNeighbours;
    }

    #endregion

    public Transform GetMazePositionTransform(Vector2 pos)
    {
        return m_cells[pos].cellComp.gameObject.transform;
    }

    public CellData GetCellByPosition(Vector2 pos)
    {
        return m_cells[pos];
    }

    public bool IsPositionWithinMaze(Vector2 pos)
    {
        return m_cells.ContainsKey(pos);
    }
}
