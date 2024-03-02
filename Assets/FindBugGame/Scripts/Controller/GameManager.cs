using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GameManager : SingletonMono<GameManager>
{
    public PlayerData player
    {
        get
        {
            if (m_player == null)
            {
                SaveHandler saveHandler = new SaveHandler();
                m_player = (PlayerData)saveHandler.LoadData("player");
                if (m_player == null)
                {
                    m_player = new PlayerData();
                }
            }
            return m_player;
        }
        set
        {
            m_player = value;
        }
    }

    private PlayerData m_player;

    public void Save()
    {
        SaveHandler saveHandler = new SaveHandler();
        saveHandler.SaveData(player, "player");
    }

    public override void Init()
    {
        base.Init();
        GameInit();
    }

    private void GameInit()
    {
        if (PlayerPrefs.GetInt("FIRST_TIME_PLAYING") == 1)
            return;
        else
        {
            PlayerPrefs.SetInt("FIRST_TIME_PLAYING", 1);
            RandomizeStageUnlocked();
        }
    }

    private void RandomizeStageUnlocked()
    {
        int levelUnlocked = UnityEngine.Random.Range(1, 1000);
        player.stageUnlocked = levelUnlocked;
        Save();
    }
}
