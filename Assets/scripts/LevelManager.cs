using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Toolbox;

public class LevelManager : Base2DBehaviour {

    public int Level { get; set; }
    public Asteroid[] AsteroidPrefabs;
    public GameObject AlienPrefab; // TBD
    public Player PlayerPrefab;


    private Player _player1;
    private List<Asteroid> _asteroids = new List<Asteroid>();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {
        Level = 1;

        MakeNewPlayer();
        StartLevel();
    }

    private void MakeNewPlayer()
    {
        _player1 = Instantiate(PlayerPrefab); //, Vector3.zero, Quaternion.identity);
        (_player1).PlayerIndex = 0;
        _player1.GetComponent<Rigidbody2D>().gravityScale = 0.0f; // Turn off gravity.
        _player1.transform.position = Vector3.zero; // TBD SPAWN
        _player1.transform.rotation = Quaternion.identity;
        _player1.gameObject.SetActive(true);
    }

    private void AddAsteroids(int astCount)
    {
        for (int ii = 0; ii < astCount; ii++)
        {
            var sizeIdx = Random.Range(0, AsteroidPrefabs.Length);
            var pos = MakeRandomPos();
            var spin = Random.Range(10.0f, 50.0f);
            var force = MakeRandomForce();
            var newAst = Instantiate(AsteroidPrefabs[sizeIdx]);
            newAst.transform.position = pos;
            newAst.transform.rotation = Quaternion.identity;
            newAst.transform.parent = AsteroidPrefabs[sizeIdx].transform.parent;
            newAst.GetComponent<Rigidbody2D>().AddForce( force);
            newAst.GetComponent<Rigidbody2D>().AddTorque(spin);
            newAst.gameObject.SetActive(true);

            var sz = newAst.GetComponent<SpriteRenderer>().sprite.bounds.size;
            newAst.GetComponent<Rigidbody2D>().transform.localScale = sz;
            _asteroids.Add(newAst);
        }
    }

    private Vector2 MakeRandomForce()
    {
        return new Vector2(Random.Range(-15.0f, 15.0f), Random.Range(-10.0f, 10.0f));
    }

    private Vector3 MakeRandomPos()
    {
        var camRect = GetCameraWorldRect();
        return new Vector3(Random.Range(camRect.xMin, camRect.xMax),
            Random.Range(camRect.yMin, camRect.yMax), 0.0f);
    }

    public void StartLevel()
    {
        AddAsteroids(5);
    }

    public void PlayerKilled(GameManager.PlayerInfo playerInfo)
    {
        //throw new System.NotImplementedException();
    }

    public void GameOver(GameManager.PlayerInfo playerInfo)
    {
        //throw new System.NotImplementedException();
    }

    public void Respawn(GameManager.PlayerInfo playerInfo)
    {
       MakeNewPlayer();

        //throw new System.NotImplementedException();
    }
}
