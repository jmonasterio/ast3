using UnityEngine;

namespace Toolbox
{

    public static class MathfExt
    {
        public static float RoundToNearestMultiple(float f, float multiple)
        {
            return Mathf.Round(f / multiple) * multiple;
        }
    }

    public class Base2DBehaviour : MonoBehaviour
    {
        public Rect GetCameraWorldRect()
        {
            var dist = (transform.position - Camera.main.transform.position).z;
            var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
            var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
            var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
            var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
            var camRect = new Rect(new Vector2(leftBorder, topBorder), new Vector2(rightBorder - leftBorder, bottomBorder - topBorder));
            return camRect;
        }

        public Vector2 TransformTo2D()
        {
            return new Vector2(transform.position.x, transform.position.y);
        }
    }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        /**
           Returns the instance of this singleton.
        */
        public static T Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                           " is needed in the scene, but there is none.");
                    }
                }

                return instance;
            }
        }
    }


}
