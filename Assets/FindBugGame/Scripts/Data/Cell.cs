using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject wallUp;
    public GameObject wallDown;
    [SerializeField] SpriteRenderer m_sprite;

    public enum WallDirection
    {
        Left,
        Right,
        Up,
        Down
    }
    public void ToggleSprite(bool enabled)
    {
        m_sprite.enabled = enabled;
    }

    public void HideWall(WallDirection direction)
    {
        switch (direction)
        {
            case WallDirection.Left:
                wallLeft.SetActive(false);
                break;
            case WallDirection.Right:
                wallRight.SetActive(false);
                break;
            case WallDirection.Up:
                wallUp.SetActive(false);
                break;
            case WallDirection.Down:
                wallDown.SetActive(false);
                break;
            default:
                Debug.LogWarning("Incorrect Direction");
                break;
        }
    }
}
