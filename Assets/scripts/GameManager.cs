using System;
using UnityEngine;
using System.Collections;
using Toolbox;


// TODO: Leftover explosions?
// x Faster asteriods
// Bullet momemntum to new asteroids.
// x Bullets
// x Breaking up asteroids
// x Next level when all asteroids dead.
// x Safe create not very safe.
// Explosion lasts too long.
// Goofy shaped asteriod.
// x Gameover not shown at end.
// x Extra asteroids after each death.
// x Show lives and score.
// x Score goes to zero on new level.
// Limit bullets.
// Spawn on edges.
// Level levelling
// Alien
// Free player every 10000

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

    [Obsolete]
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
