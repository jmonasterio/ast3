using UnityEngine;
using System.Collections;
using Toolbox;

public class Rotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private float? _instantTargetAngle;

    /// <summary>
    /// Rotate at smooth rotation rate. Stop at next increment after key lifted
    /// </summary>
    /// <param name="horz"></param>
    /// <param name="angleIncrement"></param>
    /// <param name="rotateSpeed"></param>
    public void InstantAngleChange(float horz, float angleIncrement, float rotateSpeed)
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
                var newAngle = Mathf.MoveTowardsAngle(curAngle, _instantTargetAngle.Value,
                    rotateSpeed * Time.deltaTime);
                transform.Rotate(0.0f, 0.0f, newAngle - curAngle);

                if (newAngle == curAngle)
                {
                    _instantTargetAngle = null;
                }
            }


        }

    }

    float _smoothTargetAngle;

    public void SmoothAngleChange(float horz, float angleIncrement, float rotateSpeed)
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
