using System;
using UnityEngine;
using System.Collections;
using Toolbox;


// TODO: Leftover explosions?
// Bullets
// Breaking up asteroids
// Next level when all asteroids dead.
// x Safe create not very safe.
// Explosion lasts too long.
// Goofy shaped asteriod.
// x Gameover not shown at end.
// x Extra asteroids after each death.

public class GameManager : Singleton<GameManager>
{
    public int Score = 0;
    public int Lives = 4;

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

    [Obsolete]
    public LevelManager LevelManagerPrefab;

    private LevelManager _levelManager;

	// Use this for initialization
	void Awake() 
	{
	    _levelManager = Instantiate(LevelManagerPrefab); // TBD: Cleanup.
	}

    public void PlayerKilled( Player player)
    {
        this.Lives--;
        if (this.Lives < 0)
        {
            _state = State.Over;
            _levelManager.GameOver(player);
        }
        else
        {
            _levelManager.Respawn(player);
        }

    }

    public void LastAsteroidKilled(Player p)
    {
        _levelManager.StartLevel();
    }

    // Update is called once per frame
	void Update () {

	    if (Input.GetButton("Fire1"))
	    {
	        if (_state == State.Over)
	        {
	            _state = State.Playing;
	            Lives = 4;
	            Score = 0;
	            this._levelManager.StartGame();
	        }
	    }


	    Instance.TickFrameRate();
    }
}
