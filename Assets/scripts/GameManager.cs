using UnityEngine;
using System.Collections;
using Toolbox;

public class GameManager : Singleton<GameManager>
{

    public static GameManager Current
    {
        get { return Instance as GameManager; }
    }

    public LevelManager LevelManagerPrefab;

    public class PlayerInfo 
    {
        public int Score;
        public int Lives;
    }

    public PlayerInfo[] PlayersInfo; // = new PlayerInfo[2];

	// Use this for initialization
	void Awake() 
	{
	    if ((PlayersInfo == null) || (PlayersInfo.Length == 0))
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

        this.LevelManagerPrefab.StartGame();
	    this.LevelManagerPrefab.StartLevel();
	}

    public void PlayerKilled( Player player)
    {
        int playerIdx = player.PlayerIndex;
        var playerInfo = PlayersInfo[playerIdx];
        this.LevelManagerPrefab.PlayerKilled(playerInfo);
        playerInfo.Lives--;
        if (playerInfo.Lives < 0)
        {
            LevelManagerPrefab.GameOver(playerInfo);
        }
        else
        {
            LevelManagerPrefab.Respawn(playerInfo);
        }

    }

    // Update is called once per frame
	void Update () {
        Instance.TickFrameRate();
    }
}
