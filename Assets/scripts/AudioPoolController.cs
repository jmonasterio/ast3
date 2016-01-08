using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Adapted from: http://answers.unity3d.com/questions/932118/how-can-i-locally-play-audio-over-a-network-withou.html
/// </summary>
public class AudioPoolController : MonoBehaviour
{

    // Safer to play sounds on the game object, since bullets or or asteroids may get destroyed while sound is playing???
    public void PlayClip(AudioClip clip, bool loop = false)
    {
        var src = GetPooledAudioSource();
        src.loop = loop;
        src.PlayOneShot(clip, clip.length);
        StartCoroutine(WaitForAudioClipToFinish(src, clip.length));

    }

    private IEnumerator WaitForAudioClipToFinish(AudioSource src, float audioClipDuration)
    {
        yield return new WaitForSeconds(audioClipDuration);
        this.ReturnAudioSource(src);
    }

    // The amount of AudioSource we will initialize the _pool with
    public int poolSize = 10;

    // We use a queue since it will remove the instance at the same time that we are asking for one
    private Queue<AudioSource> _pool;

    void Awake()
    {
        _pool = new Queue<AudioSource>();

        // Here we create initialize our _pool with the specified amount of instance,
        for (int ii = 0; ii < poolSize; ii++)
        {
            _pool.Enqueue(CreateNewInstance());
        }
    }

    private AudioSource CreateNewInstance()
    {
        GameObject go = new GameObject("AudioSourceInstance");
        // Let's group in our instance under the _pool manager
        go.transform.parent = this.transform;
        return go.AddComponent<AudioSource>();
    }

    // When we are asking for an AudioSource, we will first check if we still have one in our
    // _pool, if not create a new instance
    private AudioSource GetPooledAudioSource()
    {
        System.Diagnostics.Debug.Assert(_pool.Count <= this.poolSize);
        if (_pool.Count < 1)
        {
            // We ran out! Make some more.
            return CreateNewInstance();
        }
        else
        {
            return _pool.Dequeue();
        }
    }

    // Always return the AudioSource instance once you are done with it
    private void ReturnAudioSource(AudioSource instance)
    {
        _pool.Enqueue(instance);
    }
}

