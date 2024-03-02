using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private MazeMaker m_mazeMaker;
    [SerializeField] private Button m_findButton;
    [SerializeField] private Button m_autoMoveButton;
    [SerializeField] private GameObject m_verticalHintPrefab;
    [SerializeField] private GameObject m_horizontalHintPrefab;
    private CellData m_startCell, m_goalCell, m_currentCell, m_tempCell;
    private Stack<CellData> m_stack = new Stack<CellData>();
    [SerializeField] private List<CellData> m_visitedCells;
    private Vector2[] m_neighBourRelativePositions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };
private void Start()
    {
        m_findButton.onClick.AddListener(ShowPath);
        m_autoMoveButton.onClick.AddListener(MovePlayerToGoal);
    }

    private void ShowPath()
    {
        CellData prevCell = m_goalCell.prevCell;
        while (prevCell != null)
        {
            //prevCell.cellComp.ShowHint(prevCell.pos.x < m_currentCell.pos.x || prevCell.pos.x > m_currentCell.pos.x);
            m_currentCell = prevCell;
            prevCell = prevCell.prevCell;
        }
    }

    private void MovePlayerToGoal()
    {
    }

    internal void Init(Vector3 startPos, Vector3 endPos)
    {
        m_visitedCells = new List<CellData>();
        m_startCell = m_mazeMaker.GetCellByPosition(startPos);
        m_goalCell = m_mazeMaker.GetCellByPosition(endPos);
        GeneratePath();
    }

    private void GeneratePath()
    {
        //DFS logic
        m_currentCell = m_startCell;
        m_currentCell.prevCell = null;
        m_stack.Push(m_currentCell);
        m_visitedCells.Add(m_currentCell);
        while (m_currentCell != null)
        {
            List<CellData> possibleCells = GetPossibleCellsToGo();
            if (possibleCells.Count > 0)
            {
                m_tempCell = possibleCells[UnityEngine.Random.Range(0, possibleCells.Count)];
                m_tempCell.prevCell = m_currentCell;
                m_currentCell = m_tempCell;
                m_stack.Push(m_currentCell);
                m_visitedCells.Add(m_currentCell);
                if (m_tempCell == m_goalCell)
                {
                    break;
                }
            }
            else
            {
                m_currentCell = m_stack.Count > 0 ? m_stack.Pop() : null;
            }
        }
    }

    private List<CellData> GetPossibleCellsToGo()
    {
        List<CellData> possibleCells = new List<CellData>();
        foreach (Vector2 neighbourRelativePos in m_neighBourRelativePositions)
        {
            Vector2 neighourPos = m_currentCell.pos + neighbourRelativePos;
            if (m_mazeMaker.IsPositionWithinMaze(neighourPos))
            {
                CellData neighbourCell = m_mazeMaker.GetCellByPosition(neighourPos);
                if (!m_visitedCells.Contains(neighbourCell) && !IsWallBetween(neighbourCell, m_currentCell))
                    possibleCells.Add(neighbourCell);
            }
        }
        return possibleCells;
    }

    private bool IsWallBetween(CellData cell1, CellData cell2)
    {
        if (cell1.pos.x < cell2.pos.x)
        {
            return cell1.cellComp.IsWallActive(Direction.Right) || cell2.cellComp.IsWallActive(Direction.Left);
        }
        else if (cell1.pos.x > cell2.pos.x)
        {
            return cell1.cellComp.IsWallActive(Direction.Left) || cell2.cellComp.IsWallActive(Direction.Right);
        }
        else if (cell1.pos.y > cell2.pos.y)
        {
            return cell1.cellComp.IsWallActive(Direction.Up) || cell2.cellComp.IsWallActive(Direction.Down);
        }
        else
        {
            return cell1.cellComp.IsWallActive(Direction.Down) || cell2.cellComp.IsWallActive(Direction.Up);
        }
    }

    //private List<CellData> GetUnvisitedNeighbours()
    //{
    //    List<CellData> unvisitedNeighbours = new List<CellData>();
    //    foreach (Vector2 neighbourRelativePos in m_neighBourRelativePositions)
    //    {
    //        Vector2 neighbourPos = m_currentCell.pos + neighbourRelativePos;
    //        if (m_mazeMaker.IsPositionWithinMaze(neighbourPos))
    //        {
    //            CellData neighbourCell = m_mazeMaker.GetCellByPosition(neighbourPos);
    //            if (!m_visitedCells.Contains(neighbourCell)) unvisitedNeighbours.Add(neighbourCell);
    //        }
    //    }
    //    return unvisitedNeighbours;
    //}
}
