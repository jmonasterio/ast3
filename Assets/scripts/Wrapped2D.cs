using UnityEngine;
using System.Collections;
using Toolbox;

public class Wrapped2D : Base2DBehaviour {

    protected Rect _camRect;

    public virtual void Start()
    {
        _camRect = GetCameraWorldRect();
    }


    protected void WrapScreen()
    {
        // If this fails, you did not call base.Start();
        if (_camRect.Contains(this.PosTo2D()))
        {
            return;
        }
        var t = PosTo2D();
        if (t.x > _camRect.xMax)
        {
            t.x = _camRect.xMin;
        }
        else if (t.x < _camRect.xMin)
        {
            t.x = _camRect.xMax;
        }
        if (t.y > _camRect.yMax)
        {
            t.y = _camRect.yMin;
        }
        else if (t.y < _camRect.yMin)
        {
            t.y = _camRect.yMax;
        }
        PosFrom2D(t);
    }


    protected void StopRigidBody()
    {
        var rigidBody = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(0, 0, 0);
        rigidBody.velocity = new Vector3(0, 0, 0);
    }

}
