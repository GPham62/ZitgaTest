using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelCellView : EnhancedScrollerCellView
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Sprite stageNormal;
        [SerializeField] private Sprite stageDark;
        public Image stage;
        public Image stageBlock;
        public TextMeshProUGUI levelText;
        public Image tutorialText;
        public Image lineDown;
        public List<Image> stars;

        public int DataIndex { get; private set; }
        private ScrollerLevelData m_data;

        private void OnDestroy()
        {
            
        }
        public void SetData(int dataIndex, ScrollerLevelData data)
        {
            DataIndex = dataIndex;
            m_data = data;
            if (data == null)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(true);
                HandleStageSprite();
                HandleStars();
                HandleText();
                HandleLineDown();
            }
        }

        private void HandleLineDown()
        {
            lineDown.gameObject.SetActive(m_data.isLineDown);
        }

        private void HandleStageSprite()
        {
            m_data.isLocked = GameManager.instance.player.stageUnlocked < m_data.level;
            if (m_data.isLocked)
            {
                stageBlock.gameObject.SetActive(true);
                stage.sprite = stageDark;
                for (int i = 0; i < stars.Count; i++)
                    stars[i].gameObject.SetActive(false);
            }
            else
            {
                stageBlock.gameObject.SetActive(false);
                stage.sprite = stageNormal;
            }
        }

        private void HandleStars()
        {
            if (!m_data.isLocked && !m_data.isCurrentPlaying)
            {
                for (int i = 1; i < m_data.stars; i++)
                {
                    stars[i - 1].gameObject.SetActive(true);
                }
                for (int j = m_data.stars; j < stars.Count; j++)
                {
                    stars[j].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < stars.Count; i++)
                    stars[i].gameObject.SetActive(false);
            }
        }

        private void HandleText()
        {
            if (m_data.level < 1)
            {
                tutorialText.gameObject.SetActive(true);
                levelText.gameObject.SetActive(false);
            }
            else
            {
                tutorialText.gameObject.SetActive(false);
                levelText.gameObject.SetActive(true);
                levelText.text = m_data.level.ToString();
            }
        }

        public void LoadLevel()
        {
            if (!m_data.isLocked)
            {
                if (GameManager.instance.player.stageUnlocked == m_data.level)
                {
                    GameManager.instance.player.stageUnlocked++;
                    GameManager.instance.Save();
                    Debug.Log("UNLOCK NEW LEVEL");
                }
            }
        }
    }
}

