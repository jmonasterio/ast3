using UnityEngine;
using System.Collections;

public class Blinker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BlinkText(float duration, float interval, Color offColor)
    {
        if (GetComponent<TextMesh>())
        {
            StartCoroutine(BlinkTextCoroutine(duration, interval, offColor));
        }
    }

    //function to blink the text 
    private IEnumerator BlinkTextCoroutine(float duration, float interval, Color offColor)
    {
        var textMesh = GetComponent<TextMesh>();
        var originalColor = textMesh.color;
        var endTime = Time.time + duration;

        // Blink until duration is over.
        while (Time.time < endTime)
        {
            textMesh.color = offColor;
            yield return new WaitForSeconds(interval);
            textMesh.color = originalColor;
            yield return new WaitForSeconds(interval);

        }
    }


    public void BlinkSprite(float duration, float interval)
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            StartCoroutine(BlinkSpriteCoroutine(duration, interval));
        }
    }


    //function to blink the text 
    private IEnumerator BlinkSpriteCoroutine(float duration, float interval)
    {
        var sprite = GetComponent<SpriteRenderer>();
        var originalEnabled = sprite.enabled;
        var endTime = Time.time + duration;

        // Blink until duration is over.
        while (Time.time < endTime)
        {
            sprite.enabled = !originalEnabled;
            yield return new WaitForSeconds(interval);
            sprite.enabled = originalEnabled;
            yield return new WaitForSeconds(interval);

        }
    }

}
