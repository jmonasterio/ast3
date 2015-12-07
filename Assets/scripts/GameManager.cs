using UnityEngine;
using System.Collections;
using Toolbox;

public class GameManager : Singleton<Base2DBehaviour>
{

    public LevelManager LevelManager;

    public class PlayerInfo
    {
        public int Score;
        public int Lives;
    }

    public PlayerInfo[] PlayersInfo; // = new PlayerInfo[2];

	// Use this for initialization
	void Awake ()
	{
	    if (PlayersInfo == null)
	    {
	        PlayersInfo = new PlayerInfo[2];
            PlayersInfo[0] = new PlayerInfo();
	        PlayersInfo[1] = new PlayerInfo();
	    }
	    foreach (var playerInfo in PlayersInfo)
	    {
	        playerInfo.Score = 0;
	        playerInfo.Lives = 4;
	    }

        this.LevelManager.StartGame();
	    this.LevelManager.StartLevel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
