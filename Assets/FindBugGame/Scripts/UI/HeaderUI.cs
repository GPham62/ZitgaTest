using Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelect
{
    public class HeaderUI : MonoBehaviour
    {
        [SerializeField] private Button m_resetButton;
        [SerializeField] private ScrollerController scrollerController;

        private void Start()
        {
            m_resetButton.onClick.AddListener(ResetUnlockStage);
        }

        private void OnDestroy()
        {
            m_resetButton.onClick.RemoveListener(ResetUnlockStage);
        }

        private void ResetUnlockStage()
        {
            Debug.Log("RESET");
            GameManager.instance.player.stageUnlocked = 0;
            GameManager.instance.Save();
            scrollerController.ReloadData();
        }
    }
}