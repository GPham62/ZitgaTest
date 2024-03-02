using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Cell : MonoBehaviour
{
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallUp;
    public GameObject wallDown;
    [SerializeField] SpriteRenderer m_sprite;

    public void ToggleSprite(bool enabled)
    {
        m_sprite.enabled = enabled;
    }

    public void HideWall(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                wallLeft.SetActive(false);
                break;
            case Direction.Right:
                wallRight.SetActive(false);
                break;
            case Direction.Up:
                wallUp.SetActive(false);
                break;
            case Direction.Down:
                wallDown.SetActive(false);
                break;
            default:
                Debug.LogWarning("Incorrect Direction");
                break;
        }
    }

    public bool IsWallActive(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return wallLeft.activeSelf;
            case Direction.Right:
                return wallRight.activeSelf;
            case Direction.Up:
                return wallUp.activeSelf;
            case Direction.Down:
                return wallDown.activeSelf;
            default:
                Debug.LogWarning("Wrong direction!");
                return false;
        }
    }
}
