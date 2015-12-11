using System;
using UnityEngine;
using System.Collections;
using Toolbox;


[RequireComponent(typeof(LevelManager))] // Is this right, really?
public class GameManager : Singleton<GameManager>
{
    public int Score = 0;
    public int Lives = 0;

    private enum State
    {
        Playing = 0,
        Over = 1
    }

    private State _state = State.Over;

    public static GameManager Current
    {
        get { return Instance; }
    }

    internal Transform SceneRoot;

    public LevelManager LevelManagerPrefab;

    [HideInInspector]
    public LevelManager LevelManager;


	// Use this for initialization
	void Awake()
	{
	    SceneRoot = this.transform.parent;
	    LevelManager = Instantiate(LevelManagerPrefab); 
	    LevelManager.transform.parent = this.transform.parent;
	}

    // Safer to play sounds on the game object, since bullets or or asteroids may get destroyed while sound is playing???
    public void PlayClip(AudioClip clip, bool loop = false)
    {
        if (_state == State.Over)
        {
            // No sound when not playing.
            return;
        }
        var src = GetComponent<AudioSource>();
        src.loop = loop;
        src.PlayOneShot(clip, 1.0f);
    }

    public void PlayerKilled( Player player)
    {
        if (this.Lives < 1)
        {
            _state = State.Over;
            LevelManager.GameOver(player);
        }
        else
        {
            LevelManager.Respawn(player, 2.0f);

        }

    }

    // Update is called once per frame
	void Update () {

	    if (Input.GetButton("Fire1"))
	    {
	        if (_state == State.Over)
	        {
                // Try to prevent game starting right after previous if you keep firing.
	            if (LevelManager.CanStartGame())
	            {
	                Lives = 4;
	                Score = 0;
                    _state = State.Playing;
                    this.LevelManager.StartGame();

                }

            }
        }

    }

}
