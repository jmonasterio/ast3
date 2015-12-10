﻿using UnityEngine;
using System.Collections;
using Toolbox;

public class Wrapped2D : Base2DBehaviour
{

    protected Rect? _camRect = null;

    // Update is called once per frame
    void Update()
    {
        WrapScreen();
    }

    protected void WrapScreen()
    {
        if (!_camRect.HasValue)
        {
            // Cache
            _camRect = GetCameraWorldRect();
        }
        var camRect = _camRect.Value;

        // If this fails, you did not call base.Start();
        if (camRect.Contains(this.PosTo2D()))
        {
            return;
        }
        var t = PosTo2D();
        if (t.x > camRect.xMax)
        {
            t.x = camRect.xMin;
        }
        else if (t.x < camRect.xMin)
        {
            t.x = camRect.xMax;
        }
        if (t.y > camRect.yMax)
        {
            t.y = camRect.yMin;
        }
        else if (t.y < camRect.yMin)
        {
            t.y = camRect.yMax;
        }
        PosFrom2D(t);
    }
}
