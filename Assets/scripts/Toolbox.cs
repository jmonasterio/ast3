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

        float _deltaTime = 0.0f;

        public float GetFrameRate()
        {
            //float msec = _deltaTime*1000.0f;
            float fps = 1.0f/_deltaTime;
            return fps;
        }

        public Rect GetCameraWorldRect()
        {
            var dist = (transform.position - Camera.main.transform.position).z;
            var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
            var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
            var topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
            var bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
            var camRect = new Rect(new Vector2(leftBorder, topBorder),
                new Vector2(rightBorder - leftBorder, bottomBorder - topBorder));
            return camRect;
        }

        public Vector2 PosTo2D()
        {
            return new Vector2(transform.position.x, transform.position.y);
        }

        public void PosFrom2D(Vector2 v2)
        {
            transform.position = new Vector3( v2.x, v2.y, 0.0f);
       }

        /// <summary>
        /// Call this in the Update() function.
        /// </summary>
        protected void TickFrameRate()
        {
            _deltaTime += (Time.deltaTime - _deltaTime)*0.1f;
        }

        protected void DebugForceSinusoidalFrameRate()
        {
            Application.targetFrameRate = (int) ((Mathf.Sin(Time.time) + 1.05f)*60);
        }

        private float? _instantTargetAngle;

        /// <summary>
        /// Rotate at smooth rotation rate. Stop at next increment after key lifted
        /// </summary>
        /// <param name="horz"></param>
        /// <param name="angleIncrement"></param>
        /// <param name="rotateSpeed"></param>
        protected void InstantAngleChange(float horz, float angleIncrement, float rotateSpeed)
        {
            var curAngle = transform.eulerAngles.z;
            if (horz != 0.0f)
            {
                var dir = -Mathf.Sign(horz);
                if (dir != 0.0f)
                {
                    float angleToRotate = dir * rotateSpeed * Time.deltaTime;
                    var targetAngle = curAngle + angleToRotate;
                    transform.Rotate(0.0f, 0.0f, targetAngle - curAngle);

                    // In case we have to stop.
                    _instantTargetAngle = MathfExt.RoundToNearestMultiple(targetAngle + dir * angleIncrement,
                        angleIncrement);
                }

            }
            else
            {
                if (_instantTargetAngle.HasValue)
                {
                    var newAngle = Mathf.MoveTowardsAngle(curAngle, _instantTargetAngle.Value, rotateSpeed*Time.deltaTime);
                    transform.Rotate(0.0f, 0.0f, newAngle - curAngle);

                    if (newAngle == curAngle)
                    {
                        _instantTargetAngle = null;
                    }
                }


            }

        }

        float _smoothTargetAngle;
        protected void SmoothAngleChange( float horz, float angleIncrement, float rotateSpeed)
        {
            var curAngle = transform.eulerAngles.z;
            if (horz != 0.0f)
            {
                // Time factored in later.
                float angleToRotate = -Mathf.Sign(horz) * angleIncrement;
                _smoothTargetAngle = MathfExt.RoundToNearestMultiple(curAngle + angleToRotate, angleIncrement);
            }
            if (0.0f != Mathf.DeltaAngle(_smoothTargetAngle, curAngle))
            {
                var newAngle = Mathf.MoveTowardsAngle(curAngle, _smoothTargetAngle, rotateSpeed * Time.deltaTime);
                transform.Rotate(0.0f, 0.0f, newAngle - curAngle);
            }
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
