using UnityEngine;
using System.Collections;

/// <summary>
/// From here: http://wiki.unity3d.com/index.php?title=FramesPerSecond
/// </summary>
public class FrameRate : MonoBehaviour
{
    /// <summary>
    /// Show framerate on screen.
    /// </summary>
    public bool DisplayFrameRate;

    /// <summary>
    /// Enables a debug mode where frame rate varies a lot so you can test smooth movement.
    /// </summary>
    public bool DebugSinusoidalFrameRate;
    private float _deltaTime = 0.0f;

#if UNITY_EDITOR
    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update () {

        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;

	    if (DebugSinusoidalFrameRate)
	    {
            // http://answers.unity3d.com/questions/300467/how-to-limit-frame-rate-in-unity-editor.html
#if UNITY_EDITOR
                QualitySettings.vSyncCount = 0;  // VSync must be disabled
                Application.targetFrameRate = (int)((Mathf.Sin(Time.time) + 1.05f) * 60);
#endif
        }


	}

    void OnGUI()
    {
        if (DisplayFrameRate)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.LowerRight;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.yellow;
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }

    }

    public float GetFrameRate()
    {
        //float msec = _deltaTime*1000.0f;
        float fps = 1.0f / _deltaTime;
        return fps;
    }
#endif

}
