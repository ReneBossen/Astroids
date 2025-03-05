using UnityEngine;

namespace Assets.Scripts.GameCriticals
{
    public class ScreenWrap : MonoBehaviour
    {
        private Camera _camera;
        private Vector2 _screenBounds;

        private void Awake()
        {
            _camera = Camera.main;
            CalculateScreenBounds();
        }

        private void LateUpdate()
        {
            HandleScreenWrap();
        }

        private void CalculateScreenBounds()
        {
            Vector3 screenBottomLeft = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            Vector3 screenTopRight = _camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
            _screenBounds = new Vector2(
                screenTopRight.x - screenBottomLeft.x,
                screenTopRight.y - screenBottomLeft.y
            );
        }

        private void HandleScreenWrap()
        {
            Vector3 position = transform.position;

            if (position.x > _screenBounds.x / 2)
            {
                position.x = -_screenBounds.x / 2;
            }
            else if (position.x < -_screenBounds.x / 2)
            {
                position.x = _screenBounds.x / 2;
            }

            if (position.y > _screenBounds.y / 2)
            {
                position.y = -_screenBounds.y / 2;
            }
            else if (position.y < -_screenBounds.y / 2)
            {
                position.y = _screenBounds.y / 2;
            }

            transform.position = position;
        }
    }
}
