﻿using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Toolbox
{

    public static class MathfExt
    {
        public static float RoundToNearestMultiple(float f, float multiple)
        {
            return Mathf.Round(f/multiple)*multiple;
        }
    }

    public static class GameObjectExt
    {
        /// <summary>
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static GameObject FindOrCreateTempContainer(this Transform root, string name)
        {

            var trans = root.FindChild(name);
            if (trans == null)
            {
                var ret = new GameObject(name);
                ret.transform.parent = root;
                return ret;
            }
            else
            {
                return trans.gameObject;
            }
        }

        public static T InstantiateAtTransform<T>(this T prefab, Transform tr) where T:Component
        {
            var instance = Object.Instantiate(prefab);
            instance.transform.parent = tr;
            instance.transform.position = tr.position;
            instance.transform.rotation = tr.rotation;
            return instance;
        }



    }

    public static class CoroutineUtils
    {

        /**
         * Usage: StartCoroutine(CoroutineUtils.Chain(...))
         * For example:
         *     StartCoroutine(CoroutineUtils.Chain(
         *         CoroutineUtils.Do(() => Debug.Log("A")),
         *         CoroutineUtils.WaitForSeconds(2),
         *         CoroutineUtils.Do(() => Debug.Log("B"))));
         */

        public static IEnumerator Chain(params IEnumerator[] actions)
        {
            foreach (IEnumerator action in actions)
            {
                yield return GameManager.Instance.StartCoroutine(action); 
            }
        }

        /**
         * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
         * For example:
         *     StartCoroutine(CoroutineUtils.DelaySeconds(
         *         () => DebugUtils.Log("2 seconds past"),
         *         2);
         */

        public static IEnumerator DelaySeconds(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static IEnumerator WaitForSeconds(float time)
        {
            yield return new WaitForSeconds(time);
        }

        public static IEnumerator Do(Action action)
        {
            action();
            yield return 0;
        }
    }

    public class Base2DBehaviour : MonoBehaviour
    {


        protected Vector2 MakeRandom2D()
        {
            return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        }


        public void Show(bool b)
        {
            GetComponent<SpriteRenderer>().enabled = b;
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

        public Vector2 To2D(Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public Vector3 From2D(Vector2 v2)
        {
            return new Vector3(v2.x, v2.y, 0.0f);
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
                    float angleToRotate = dir*rotateSpeed*Time.deltaTime;
                    var targetAngle = curAngle + angleToRotate;
                    transform.Rotate(0.0f, 0.0f, targetAngle - curAngle);

                    // In case we have to stop.
                    _instantTargetAngle = MathfExt.RoundToNearestMultiple(targetAngle + dir*angleIncrement,
                        angleIncrement);
                }

            }
            else
            {
                if (_instantTargetAngle.HasValue)
                {
                    var newAngle = Mathf.MoveTowardsAngle(curAngle, _instantTargetAngle.Value,
                        rotateSpeed*Time.deltaTime);
                    transform.Rotate(0.0f, 0.0f, newAngle - curAngle);

                    if (newAngle == curAngle)
                    {
                        _instantTargetAngle = null;
                    }
                }


            }

        }

        float _smoothTargetAngle;

        protected void SmoothAngleChange(float horz, float angleIncrement, float rotateSpeed)
        {
            var curAngle = transform.eulerAngles.z;
            if (horz != 0.0f)
            {
                // Time factored in later.
                float angleToRotate = -Mathf.Sign(horz)*angleIncrement;
                _smoothTargetAngle = MathfExt.RoundToNearestMultiple(curAngle + angleToRotate, angleIncrement);
            }
            if (0.0f != Mathf.DeltaAngle(_smoothTargetAngle, curAngle))
            {
                var newAngle = Mathf.MoveTowardsAngle(curAngle, _smoothTargetAngle, rotateSpeed*Time.deltaTime);
                transform.Rotate(0.0f, 0.0f, newAngle - curAngle);
            }
        }

        protected void SafeDestroy(ref Component obj)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
            obj = null;
        }

        protected void SafeDestroy(ref GameObject obj)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
            obj = null;
        }

        protected void SafeDestroy(ref ParticleSystem obj)
        {
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
            obj = null;
        }

    }
}

