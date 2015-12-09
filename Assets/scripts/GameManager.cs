using System;
using UnityEngine;
using System.Collections;
using Toolbox;



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
	    LevelManager = Instantiate(LevelManagerPrefab); // TBD: Cleanup.
	    LevelManager.transform.parent = this.transform.parent;
	}

    // Safer to play sounds on the game object, since bullets or or asteroids may get destroyed while sound is playing???
    public void PlayClip(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip, 1.0f);
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
            LevelManager.Respawn(player);

        }

    }

    public void LastAsteroidKilled(Player p)
    {
        LevelManager.StartLevel();
    }

    // Update is called once per frame
	void Update () {

	    if (Input.GetButton("Fire1"))
	    {
	        if (_state == State.Over)
	        {
	            _state = State.Playing;
	            Lives = 2;
	            Score = 0;
	            this.LevelManager.StartGame();
	        }
	    }


	    Instance.TickFrameRate();
    }
}
