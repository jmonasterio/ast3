using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Toolbox;
using Random = UnityEngine.Random;

public class LevelManager : Base2DBehaviour {

    public int Level { get; set; }
    [Obsolete]
    public Asteroid[] AsteroidPrefabs;
    [Obsolete]
    public GameObject AlienPrefab; // TBD
    [Obsolete]
    public Player PlayerPrefab;
    [Obsolete]
    public GameOver GameOverPrefab;

    private GameOver _gameOver;
    private Player _player1;
    private List<Asteroid> _asteroids = new List<Asteroid>();

    // Use this for initialization
    void Start () {
        _gameOver = Instantiate(GameOverPrefab);
        _gameOver.transform.position = Vector3.zero; // TBD SPAWN
        _gameOver.transform.rotation = Quaternion.identity;
        _gameOver.gameObject.SetActive(true);


    }

    // Update is called once per frame
    void Update ()
	{
    }

    public void OnDestroy()
    {
        if (_player1 != null)
        {
            Destroy(_player1.gameObject);
            _player1 = null;
        }
        ClearAsteroids();
    }

    private void ClearAsteroids()
    {
        if (_asteroids != null)
        {
            foreach (var ast in _asteroids)
            {
                Destroy(ast.gameObject);
            }
            _asteroids = new List<Asteroid>();
        }
    }

    public void StartGame()
    {
        Level = 0;
        ShowGameOver(false);
        MakeNewPlayer();
        ClearAsteroids();
        StartLevel();
    }

    private void MakeNewPlayer()
    {
        _player1 = Instantiate(PlayerPrefab); //, Vector3.zero, Quaternion.identity);
        _player1.PlayerIndex = 0;
        _player1.GetComponent<Rigidbody2D>().gravityScale = 0.0f; // Turn off gravity.
        _player1.transform.position = MakeSafeRandomPos();
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
            newAst.transform.localScale = newAst.transform.localScale*4;
            newAst.transform.position = pos;
            newAst.transform.rotation = Quaternion.identity;
            newAst.transform.parent = AsteroidPrefabs[sizeIdx].transform.parent;
            newAst.GetComponent<Rigidbody2D>().AddForce( force);
            newAst.GetComponent<Rigidbody2D>().AddTorque(spin);
            newAst.gameObject.SetActive(true);

            //newAst.GetComponent<SpriteRenderer>().sprite.bounds.size = newAst.GetComponent<Rigidbody2D>().transform.localScale = sz;
            _asteroids.Add(newAst);
        }
    }

    private Vector2 MakeRandomForce()
    {
        return new Vector2(Random.Range(-25.0f, 25.0f), Random.Range(-20.0f, 20.0f));
    }

    private Vector3 MakeRandomPos()
    {
        var camRect = GetCameraWorldRect();
        return new Vector3(Random.Range(camRect.xMin, camRect.xMax),
            Random.Range(camRect.yMin, camRect.yMax), 0.0f);
    }

    private Vector3 MakeRandomCentralPos()
    {
        var camRect = GetCameraWorldRect();
        return new Vector3(Random.Range(camRect.xMin/2, camRect.xMax/2),
            Random.Range(camRect.yMin/2, camRect.yMax/2), 0.0f);
    }

    // Best effort.
    private Vector3 MakeSafeRandomPos()
    {
        for(int ii=1; ii<1000; ii++)
        {
            var pos = MakeRandomCentralPos();
            if ((_asteroids==null) || (_asteroids.Count == 0))
            {
                return pos;
            }
            bool foundClose = false;
            foreach (var ast in _asteroids)
            {
                if (Vector3.Distance(ast.transform.position, pos) < 2.0)
                {
                    foundClose = true;
                    break;
                }
            }
            if (!foundClose)
            {
                return pos;
            }
        }

        return MakeRandomPos();

    }

    public void StartLevel()
    {
        Level++;
        AddAsteroids(5);
        ShowGameOver(false);
    }

    private void ShowGameOver(bool b)
    {
        _gameOver.GetComponent<MeshRenderer>().enabled = b;
    }

    public void GameOver(Player player)
    {
        ShowGameOver(true);
    }

    public void Respawn(Player player )
    {
        // TBD: Would be cleaner in a base class.
        StartCoroutine(CoroutineUtils.DelaySeconds(() => MakeNewPlayer(), 2.0f));

    }
}
