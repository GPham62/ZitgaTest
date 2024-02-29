using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Controller
{
    public class ScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
    {
        private SmallList<ScrollerLevelData> m_data;

        public EnhancedScroller scroller;
        public LevelRowCellView cellviewPrefab;
        public int numberOfCellsPerRow = 4;
        [SerializeField] private int m_lastIndex = 100;

        private void Start()
        {
            m_data = new SmallList<ScrollerLevelData>();
            int tempIndex = m_lastIndex;
            bool isReverse = false;
            while (tempIndex > 0)
            {
                AddData(tempIndex, isReverse, GameManager.instance.player.stageUnlocked);
                isReverse = !isReverse;
                tempIndex = tempIndex - 4;
            }
            
            scroller.Delegate = this;
            ReloadData();
        }

        public void ReloadData()
        {
            scroller.ReloadData();
            scroller.JumpToDataIndex(m_lastIndex);
        }

        private void AddData(int tempIndex, bool isReverse, int levelUnlocked)
        {
            if (isReverse)
            {
                for (int j = tempIndex; j > tempIndex - 4; j--)
                {
                    int reverseIndex = 2 * tempIndex - 3 - j;
                    m_data.Add(new ScrollerLevelData()
                    {
                        level = reverseIndex,
                        isCurrentPlaying = levelUnlocked == reverseIndex ? true : false,
                        stars = Random.Range(1, 4),
                        isLineDown = reverseIndex == tempIndex && reverseIndex != m_lastIndex ? true : false
                    });
                }
            }
            else
            {
                for (int i = tempIndex; i > tempIndex - 4; i--)
                {
                    m_data.Add(new ScrollerLevelData()
                    {
                        level = i,
                        isCurrentPlaying = levelUnlocked == i ? true : false,
                        stars = Random.Range(1, 4),
                        isLineDown = i == tempIndex && i != m_lastIndex ? true : false
                    });
                }
            }
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            LevelRowCellView cellView = scroller.GetCellView(cellviewPrefab) as LevelRowCellView;
            var di = dataIndex * numberOfCellsPerRow;
            cellView.name = "Cell " + (di).ToString() + " to " + ((di) + numberOfCellsPerRow - 1).ToString();
            cellView.SetData(ref m_data, di);
            return cellView;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 700f;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt((float)m_data.Count / (float)numberOfCellsPerRow);
        }
    }
}

