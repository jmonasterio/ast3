using System;
using UnityEngine;
using System.Collections.Generic;
using Toolbox;
using Random = UnityEngine.Random;

public class LevelManager : Base2DBehaviour {

    public int Level { get; set; }
    public Asteroid[] AsteroidPrefabs;
    public Alien AlienPrefab; // TBD
    public Player PlayerPrefab;
    public GameOver GameOverPrefab;
    public Instructions InstructionsPrefab;
    public ParticleSystem AsteroidExplosionParticlePrefab;
    public AudioClip Jaws1;
    public AudioClip Jaws2;
    public AudioClip FreeLifeSound;
    public AudioClip AlienSoundBig;
    public AudioClip AlienSoundSmall;
    public int FREE_USER_AT = 10000;

    private int _nextFreeLifeScore;

    private GameOver _gameOver;
    private Instructions _instructions;
    private Player _player1;
    private List<Asteroid> _asteroids = new List<Asteroid>();
    private Alien _alien;

    private float _nextJawsSoundTime;
    private float _jawsIntervalSeconds;
    private bool _jawsAlternate;
    private Rect _camRect;
    private double _disableStartButtonUntilTime; // TBD: Clunky

    // Use this for initialization
    void Start ()
    {

        _camRect = GetCameraWorldRect();
        _disableStartButtonUntilTime = Time.time;

        _gameOver = Instantiate(GameOverPrefab);
        //_gameOver.transform.position = Vector3.zero; // TBD SPAWN
        _gameOver.transform.rotation = Quaternion.identity;
        _gameOver.transform.parent = GameManager.Instance.SceneRoot;
        _gameOver.gameObject.SetActive(true);

        _instructions = Instantiate(InstructionsPrefab);
        //_gameOver.transform.position = Vector3.zero; // TBD SPAWN
        _instructions.transform.rotation = Quaternion.identity;
        _instructions.transform.parent = GameManager.Instance.SceneRoot;
        _instructions.gameObject.SetActive(true);

        ShowGameOver(true);
        ShowInstructions(true);
    }

    // Update is called once per frame
    void Update ()
	{
        if (GameManager.Instance.Score > _nextFreeLifeScore)
        {
            GameManager.Instance.Lives++;
            _nextFreeLifeScore += FREE_USER_AT;
            GameManager.Instance.PlayClip(FreeLifeSound);
        }

        if (Time.time > _nextJawsSoundTime)
        {
            if (_player1 != null) // Means we're in level
            {
                if (_jawsIntervalSeconds > 180.0f)
                {
                    _jawsIntervalSeconds -= 5.0f;
                }
                _nextJawsSoundTime = Time.time + _jawsIntervalSeconds;
                if (_jawsAlternate)
                {
                    GameManager.Instance.PlayClip(Jaws1);
                }
                else
                {
                    GameManager.Instance.PlayClip(Jaws2);
                }
                _jawsAlternate = !_jawsAlternate;
            }
        }

        if (IsGamePlaying())
        {
            if (_alien == null)
            {
                if (Random.Range(0, 100) > 97)
                {
                    _alien = Instantiate(AlienPrefab);
                    _alien.Size = (Random.Range(0, 3) == 0) ? Alien.Sizes.Small : Alien.Sizes.Big;
                    _alien.SetPath(MakeRandomPath());
                    _alien.transform.localScale = _alien.transform.localScale*(_alien.Size == Alien.Sizes.Small ? 1 : 2);

                    if (IsGamePlaying())
                    {
                        _alien.PlaySound();
                    }
                }
            }
        }
    }

    private bool IsGamePlaying()
    {
        return Level > 0;
    }

    private List<Vector3> MakeRandomPath()
    {
        var camRect = GetCameraWorldRect();
        var path = new List<Vector3>();

        path.Add( new Vector3(_camRect.xMin -0.5f, Random.Range(_camRect.yMin*0.9f, _camRect.yMax*0.9f)));

//        for (var segment = 0; segment <= 4; segment++)
  //      {
    //        path.Add( )
     //   }
        path.Add( new Vector3(_camRect.xMax + 0.5f, Random.Range(_camRect.yMin * 0.9f, _camRect.yMax * 0.9f)));

        if (Random.Range(0, 2) == 0)
        {
            path.Reverse();
        }

        return path;
    }

    public void OnDestroy()
    {
        if (_player1 != null)
        {
            Destroy(_player1.gameObject);
            _player1 = null;
        }
        if (_instructions != null)
        {
            Destroy(_instructions);
        }
        if (_gameOver != null)
        {
            Destroy(_gameOver);
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

    public bool CanStartGame()
    {
        if (Time.time < _disableStartButtonUntilTime)
        {
            return false;
        }
        return true;
    }

    public void StartGame()
    {
        
        Level = 0;
        _nextFreeLifeScore = FREE_USER_AT;

        ShowGameOver(false);
        ShowInstructions(false);
        
        ShowInstructions(false);
        ClearAsteroids();
        StartLevel();
        Respawn(_player1, 0.5f);
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

    public void HyperSpace()
    {
        _player1.transform.position = MakeRandomPos(); // Not safe on purpose
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
        return new Vector3(Random.Range(_camRect.xMin, _camRect.xMax),
            Random.Range(_camRect.yMin, _camRect.yMax), 0.0f);
    }

    private Vector3 MakeRandomCentralPos()
    {
        return new Vector3(Random.Range(_camRect.xMin/2, _camRect.xMax/2),
            Random.Range(_camRect.yMin/2, _camRect.yMax/2), 0.0f);
    }

    // Best effort.
    private Vector3 MakeSafeRandomPos()
    {
        for(int ii=0; ii<1000; ii++)
        {

            var pos = MakeRandomCentralPos();
            if (ii == 0)
            {
                // Try the center, because it's the best place.
                pos = new Vector3(0,0,0);
            }
            

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
        if (_player1 != null)
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
        }
        return MakeRandomPos();

    }

    public void StartLevel()
    {
        Level++;
        _jawsIntervalSeconds = 0.9f;
        _jawsAlternate = true;
        _nextJawsSoundTime += _jawsIntervalSeconds;
        AddAsteroids((int) (2 + Level)); // 3.0 + Mathf.Log( (float) Level)));
    }

    private void ShowGameOver(bool b)
    {
        _disableStartButtonUntilTime = Time.time + 1.5;
        _gameOver.GetComponent<MeshRenderer>().enabled = b;
        if (b)
        {
            _gameOver.BlinkText(2.0f, 0.1f, Color.gray);
        }
    }

    private void ShowInstructions(bool b)
    {
        _instructions.GetComponent<MeshRenderer>().enabled = b;
    }

    public void GameOver(Player player)
    {
        _player1 = null;

        ShowGameOver(true);
        ShowInstructions(true);
    }


    public void Respawn(Player player, float delay )
    {
        _player1 = null;

        // TBD: Would be cleaner in a base class.
        StartCoroutine(CoroutineUtils.DelaySeconds(() =>
        {
            MakeNewPlayer();
            _player1.BlinkSprite(1.0f, 0.05f);

            // Change the count AFTER the respawn occurs. It looks better.
            GameManager.Instance.Lives--;

        }, delay));

    }

    public void DestroyAlien(Alien alien, bool explode)
    {
        Debug.Assert( alien.GetInstanceID() == _alien.GetInstanceID());
        _alien = null;
        if (explode)
        {
            CreateAsteroidOrAlienExplosion(alien.transform.position);
        }
        Destroy(alien.gameObject); 
    }

    public void ReplaceAsteroidWith(Asteroid ast, int p1, Asteroid.Sizes astSize, Bullet bullet)
    {
        bool removed = _asteroids.Remove(ast);
        System.Diagnostics.Debug.Assert(removed);

        CreateAsteroidOrAlienExplosion(ast.transform.position); // TBD: Maybe put sound here, too.

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

    private void CreateAsteroidOrAlienExplosion(Vector3 position)
    {
        var explosion = Instantiate(AsteroidExplosionParticlePrefab);
        explosion.transform.parent = this.transform.parent.transform.FindChild("AsteroidField");
        explosion.transform.position = position;
        explosion.transform.rotation = this.transform.rotation;
        //_exhaust.enableEmission = true;
        explosion.Play();
        DestroyObject( explosion, explosion.duration + 0.25f);

    }


}
