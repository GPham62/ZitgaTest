using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Controller
{
    public class GameplayController : MonoBehaviour
    {
        [SerializeField] private MazeMaker m_mazeMaker;
        [SerializeField] private PathFinder m_pathFinder;
        [SerializeField] private GameObject m_playerPrefab;
        [SerializeField] private GameObject m_goalPrefab;
        [SerializeField] private Button m_returnButton;

        // Start is called before the first frame update
        void Start()
        {
            m_mazeMaker.Init();
            Vector3 startPos = Vector2.zero;
            Vector3 endPos = new Vector2(Random.Range(1, m_mazeMaker.columns), Random.Range(1, m_mazeMaker.rows));
            GameObject player = Instantiate(m_playerPrefab, m_mazeMaker.GetMazePositionTransform(startPos).position + m_playerPrefab.transform.position, m_playerPrefab.transform.rotation);
            Instantiate(m_goalPrefab, m_mazeMaker.GetMazePositionTransform(endPos).position + m_goalPrefab.transform.position, m_goalPrefab.transform.rotation);
            m_pathFinder.Init(player, startPos, endPos);

            m_returnButton.onClick.AddListener(ReturnToLevelSelect);
        }

        private void ReturnToLevelSelect()
        {
            SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            m_returnButton.onClick.RemoveAllListeners();
        }
    }

}