using System;
using UnityEngine;
using System.Collections;
using Toolbox;


[RequireComponent(typeof(AudioSource))] 
public class GameManager : Base2DBehaviour
{
    public struct Buttons
    {
        public const string HORIZ = "Horizontal";
        public const string VERT = "Vertical";
        public const string HYPERSPACE = "HyperSpace";
        public const string HYPERSPACE2 = "HyperSpace2";
        public const string JUMP = "Jump";
        public const string FIRE1 = "Fire1";
    }

    public int Score = 0;
    public int Lives = 0;
    public SceneController SceneControllerPrefab;

    public static GameManager Instance;

    private enum State
    {
        Playing = 0,
        Over = 1
    }
    private State _state = State.Over;

    [HideInInspector]
    public SceneController SceneController;
    [HideInInspector]
    public Transform SceneRoot;

    // Use this for initialization
    void Awake()
	{
        DontDestroyOnLoad(this); // Keep running across different scenes.
        Instance = this; // Simple singleton.
        SceneRoot = this.transform.parent;
	    this.transform.parent = SceneRoot;
	    SceneController = Instantiate(SceneControllerPrefab); 
	    SceneController.transform.parent = this.transform.parent;
	}

    // Safer to play sounds on the game object, since bullets or or asteroids may get destroyed while sound is playing???
    public void PlayClip(AudioClip clip, bool loop = false)
    {
        if (_state == State.Over)
        {
            // No sound when not playing.
            return;
        }

        this.GetComponent<AudioPoolController>().PlayClip(clip, loop);
    }

    public void PlayerKilled( Player player)
    {
        if (this.Lives < 1)
        {
            _state = State.Over;
            SceneController.GameOver(player);
        }
        else
        {
            SceneController.Respawn(player, 2.0f);

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_state == State.Over)
        {
            if (Input.GetButton(Buttons.FIRE1))
            {

                // Try to prevent game starting right after previous if you keep firing.
                if (SceneController.CanStartGame())
                {
                    Lives = 4;
                    Score = 0;
                    _state = State.Playing;
                    this.SceneController.StartGame();

                }

            }
        }

    }

}
