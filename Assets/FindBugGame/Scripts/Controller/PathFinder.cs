using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private Transform m_hintHolder;
    [SerializeField] private MazeMaker m_mazeMaker;
    [SerializeField] private Button m_findButton;
    [SerializeField] private Button m_autoMoveButton;
    [SerializeField] private GameObject m_verticalHintPrefab;
    [SerializeField] private GameObject m_horizontalHintPrefab;
    [SerializeField] private float m_moveSpeed = 5f;
    private GameObject m_player;
    private Stack<GameObject> m_hints;
    private Stack<CellData> m_result;
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
        foreach (var hint in m_hints)
        {
            hint.SetActive(true);
            SpriteRenderer hintSprite = hint.GetComponent<SpriteRenderer>();
            Color tmp = hintSprite.color;
            tmp.a = 0f;
            hintSprite.color = tmp;
            hintSprite.DOFade(1, 0.3f);
        }
    }

    private void MovePlayerToGoal()
    {
        if (Vector2.Distance(m_player.transform.position, m_goalCell.cellComp.transform.position) < 0.1f) return;
        StartCoroutine(MoveToGoalRoutine());
    }

    private IEnumerator MoveToGoalRoutine()
    {
        while (m_result.Count > 0)
        {
            if (m_hints.Count > 0)
            {
                SpriteRenderer hint = m_hints.Pop().GetComponent<SpriteRenderer>();
                hint.DOFade(0, 0.3f).OnComplete(() => hint.gameObject.SetActive(false));
            }

            Vector3 targetPosition = m_result.Pop().cellComp.transform.position + new Vector3(0, 0, m_player.transform.position.z);
            float time = 0;
            Vector3 startPosition = m_player.transform.position;
            LookAtDirection(startPosition, targetPosition);
            float duration = m_mazeMaker.cellSize / (2  * m_moveSpeed);
            while (time < duration)
            {
                m_player.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            m_player.transform.position = targetPosition;
        }
        
    }

    private void LookAtDirection(Vector3 startPosition, Vector3 targetPosition)
    {
        if (targetPosition.x > startPosition.x)
        {
            m_player.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if(targetPosition.x < startPosition.x)
        {
            m_player.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (targetPosition.y < startPosition.y) 
        {
            m_player.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
            m_player.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    internal void Init(GameObject player, Vector3 startPos, Vector3 endPos)
    {
        m_player = player;
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
                    break;
            }
            else
            {
                m_currentCell = m_stack.Count > 0 ? m_stack.Pop() : null;
            }
        }
        SpawnInactiveHints();
    }

    private void SpawnInactiveHints()
    {
        m_hints = new Stack<GameObject>();
        m_result = new Stack<CellData>();
        m_tempCell = m_goalCell.prevCell;
        m_result.Push(m_goalCell);
        while (m_tempCell != null)
        {
            GameObject hint = null;
            if (m_tempCell.pos.x < m_currentCell.pos.x)
            {
                hint = Instantiate(m_horizontalHintPrefab,
                    new Vector3(m_currentCell.cellComp.transform.position.x - m_mazeMaker.cellSize / 2 - m_mazeMaker.wallSize / 2,
                    m_currentCell.cellComp.transform.position.y, -0.15f),
                    Quaternion.identity, m_hintHolder);

            }
            else if (m_tempCell.pos.x > m_currentCell.pos.x)
            {
                hint = Instantiate(m_horizontalHintPrefab,
                       new Vector3(m_currentCell.cellComp.transform.position.x + m_mazeMaker.cellSize / 2 + m_mazeMaker.wallSize / 2,
                       m_currentCell.cellComp.transform.position.y, -0.15f),
                       Quaternion.identity, m_hintHolder);
            }
            else if (m_tempCell.pos.y < m_currentCell.pos.y)
            {
                hint = Instantiate(m_verticalHintPrefab,
                       new Vector3(m_currentCell.cellComp.transform.position.x,
                       m_currentCell.cellComp.transform.position.y + m_mazeMaker.cellSize / 2 + m_mazeMaker.wallSize / 2, -0.15f),
                       Quaternion.identity, m_hintHolder);
            }
            else
            {
                hint = Instantiate(m_verticalHintPrefab,
                       new Vector3(m_currentCell.cellComp.transform.position.x,
                       m_currentCell.cellComp.transform.position.y - m_mazeMaker.cellSize / 2 - m_mazeMaker.wallSize / 2, -0.15f),
                       Quaternion.identity, m_hintHolder);
            }
            hint.SetActive(false);
            m_hints.Push(hint);
            m_result.Push(m_tempCell);
            m_currentCell = m_tempCell;
            m_tempCell = m_tempCell.prevCell;
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
}
