﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Toolbox;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class LevelManager : Base2DBehaviour {

    public int Level { get; set; }
    public Asteroid[] AsteroidPrefabs;
    public GameObject AlienPrefab; // TBD
    public Player PlayerPrefab;
    public GameOver GameOverPrefab;
    public ParticleSystem AsteroidExplosionParticlePrefab;

    private GameOver _gameOver;
    private Player _player1;
    private List<Asteroid> _asteroids = new List<Asteroid>();

    // Use this for initialization
    void Start () {
        _gameOver = Instantiate(GameOverPrefab);
        //_gameOver.transform.position = Vector3.zero; // TBD SPAWN
        _gameOver.transform.rotation = Quaternion.identity;
        _gameOver.transform.parent = GameManager.Instance.SceneRoot;
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
        _player1.transform.parent = GameManager.Instance.SceneRoot;
        _player1.gameObject.SetActive(true);
    }

    private void AddAsteroids(int astCount)
    {
        for (int ii = 0; ii < astCount; ii++)
        {
            var pos = MakeSafeAsteroidPos();
            AddAsteroidWithSizeAt( Asteroid.Sizes.Large, pos);
        }
    }

    private Asteroid GetPrefabBySize(Asteroid.Sizes astSize)
    {
        return AsteroidPrefabs[(int) astSize];
    }

    private Vector2 MakeRandomForce()
    {
        var f = new Vector2(Random.Range(-25.0f, 25.0f), Random.Range(-20.0f, 20.0f));
        f = f * 2.0f;
        return f;
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

    private Vector3 MakeSafeAsteroidPos()
    {
        var playerPos = _player1.transform.position;
        for (int ii = 1; ii < 1000; ii++)
        {
            var astPos = MakeRandomPos();
            if (Vector3.Distance(astPos, playerPos) > 2.0)
            {
                return astPos;
            }
        }
        return MakeRandomPos();

    }

    public void StartLevel()
    {
        Level++;
        AddAsteroids( (int) (3.0f + Mathf.Log( (float) Level)));
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
        StartCoroutine(CoroutineUtils.DelaySeconds(() =>
        {
            MakeNewPlayer();

            // Change the count AFTER the respawn occurs. It looks better.
            GameManager.Instance.Lives--;

        }, 2.0f));

    }

    public void ReplaceAsteroidWith(Asteroid ast, int p1, Asteroid.Sizes astSize, Bullet bullet)
    {
        bool removed = _asteroids.Remove(ast);
        System.Diagnostics.Debug.Assert(removed);

        CreateAsteroidExplosion(ast); // TBD: Maybe put sound here, too.

        for (int ii = 0; ii < p1; ii++)
        {
            var newAst = AddAsteroidWithSizeAt(astSize, ast.transform.position);

            // Give some momentum from the bullet.
            var rigid = newAst.GetComponent<Rigidbody2D>();
            var bulletVelocity = bullet.GetComponent<Rigidbody2D>().velocity;
            var f = bulletVelocity.normalized*Random.Range(0.05f, 0.2f);
            rigid.AddRelativeForce( f, ForceMode2D.Impulse );

            // Speed up smaller ones
            //rigid.AddRelativeForce( rigid.velocity * 0.05f, ForceMode2D.Impulse);
        }

        Destroy(ast.gameObject); // TBD: ExplosionSound & Split asteroid & keep count.

        if (_asteroids.Count == 0)
        {
            StartLevel();
        }
    }

    private Asteroid AddAsteroidWithSizeAt(Asteroid.Sizes astSize, Vector3 pos)
    {
        //var astSize = (Asteroid.Sizes)Random.Range(0, AsteroidPrefabs.Length);
        var spin = Random.Range(10.0f, 50.0f);
        var force = MakeRandomForce();
        var prefab = GetPrefabBySize(astSize);
        var newAst = Instantiate(prefab);
        newAst.transform.localScale = newAst.transform.localScale * 3; // Original graphics were too tiny.
        newAst.transform.position = pos;
        newAst.transform.rotation = Quaternion.identity;
        newAst.transform.parent = this.transform.parent.transform.FindChild("AsteroidField");
        newAst.GetComponent<Rigidbody2D>().AddForce(force);
        newAst.GetComponent<Rigidbody2D>().AddTorque(spin);
        newAst.gameObject.SetActive(true);

        //newAst.GetComponent<SpriteRenderer>().sprite.bounds.size = newAst.GetComponent<Rigidbody2D>().transform.localScale = sz;
        _asteroids.Add(newAst);
        return newAst;
    }

    private void CreateAsteroidExplosion(Asteroid ast)
    {
        var explosion = Instantiate(AsteroidExplosionParticlePrefab);
        explosion.transform.parent = this.transform.parent.transform.FindChild("AsteroidField");
        explosion.transform.position = ast.transform.position;
        explosion.transform.rotation = this.transform.rotation;
        //_exhaust.enableEmission = true;
        explosion.Play();
        DestroyObject( explosion, explosion.duration + 0.25f);

    }

}
