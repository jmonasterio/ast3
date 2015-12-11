using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Toolbox;

public class Player : Base2DBehaviour
{
    public int MAX_BULLETS = 3;

    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public float MaxSpeed = 50.0f;
    public int PlayerIndex = 0; // Or 1, for 2 players.

    public AudioClip ExplosionSound;
    public AudioClip ThrustSound;
    public AudioClip ShootSound;

    public ParticleSystem ExhaustParticlePrefab;
    public ParticleSystem ExplosionParticlePrefab;
    public Transform GhostPrefab;

    public Bullet BulletPrefab;

    //private Transform[] Ghosts = new Transform[4];
    private ParticleSystem _exhaustParticleSystem;
    private ParticleSystem _explosionParticleSystem;

    private AudioSource _thrustAudioSource;

    private enum State
    {
        Alive = 0,
        Killed = 1
    }


    private State _state;
    private GameObject _bulletsContainer;


    // Use this for initialization
    public void Start()
    {
        _thrustAudioSource = GetComponent<AudioSource>();

        _bulletsContainer = GameManager.Instance.SceneRoot.FindOrCreateTempContainer("PlayerBulletsContainer");
        
        System.Diagnostics.Debug.Assert(_bulletsContainer != null);

        _exhaustParticleSystem = Instantiate(ExhaustParticlePrefab);
        _exhaustParticleSystem.transform.parent = this.transform;
        _exhaustParticleSystem.transform.position = this.transform.FindChild("ExhaustExit").transform.position;
        //_exhaustParticleSystem.transform.rotation = this.transform.rotation;
        //_exhaustParticleSystem.transform.RotateAround( transform.position, transform.up, 180.0f);
        //_exhaustParticleSystem.enableEmission = true;
        _exhaustParticleSystem.Stop();

        _explosionParticleSystem = Instantiate(ExplosionParticlePrefab);
        _explosionParticleSystem.transform.parent = this.transform;
        _explosionParticleSystem.transform.position = this.transform.position; //new Vector3(0.015f, -0.15f, 0.0f);
        _explosionParticleSystem.transform.rotation = this.transform.rotation;
        _explosionParticleSystem.loop = false;
        _explosionParticleSystem.Stop();

        //var newGhost = (Transform)Instantiate(GhostPrefab, new Vector3(0, _camRect.height, 0), Quaternion.identity);
        //newGhost.parent = transform;
        //newGhost = (Transform)Instantiate(GhostPrefab, new Vector3(0, -_camRect.height, 0), Quaternion.identity);
        //newGhost.parent = transform;
        //newGhost = (Transform)Instantiate(GhostPrefab, new Vector3(_camRect.width, 0, 0), Quaternion.identity);
        //newGhost.parent = transform;
        //newGhost = (Transform)Instantiate(GhostPrefab, new Vector3(-_camRect.width, 0, 0), Quaternion.identity);
        //newGhost.parent = transform;

        _state = State.Alive;
    }

    void OnGUI()
    {
        //print(GetFrameRate());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_state == State.Killed)
        {
            return;
        }
        if (other.GetInstanceID() == this.GetInstanceID())
        {
            // Avoid collision with self.
            return;
        }
        if (other.gameObject.GetComponent<Bullet>())
        {
            var bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.Source == Bullet.Sources.PlayerShooter)
            {
                // We don't allow shooting ourself
                return; // 
            }
        }
        PlayerKilled();
    }

    /// <summary>
    /// Could also be called by alien bullet.
    /// </summary>
    private void PlayerKilled()
    {
        _state = State.Killed;
        Show(false);
        _exhaustParticleSystem.Stop();
        _explosionParticleSystem.Play();
        GetComponent<Rigidbody2D>().velocity *= 0.5f; // Slow down when killed.
        GameManager.Instance.PlayerKilled(this);
        GameManager.Instance.PlayClip(ExplosionSound);
        Destroy(this.gameObject, _explosionParticleSystem.duration + 0.5f);
    }

    void OnDestroy()
    {
        // Cleanup
        if (_exhaustParticleSystem != null)
        {
            _exhaustParticleSystem.Stop();
        }
        if (_explosionParticleSystem != null)
        {
            _explosionParticleSystem.Stop();
        }
        SafeDestroy( ref _exhaustParticleSystem);
        SafeDestroy( ref _explosionParticleSystem);
    }

    // Update is called once per frame
    void Update ()
    {
        if (_state != State.Killed)
        {

            //base.DebugForceSinusoidalFrameRate();

            float horz = Input.GetAxisRaw("Horizontal");

            base.InstantAngleChange(horz, AngleIncrement, RotateSpeed);


            float vert = Input.GetAxisRaw("Vertical");
            if (vert > 0.0f)
            {
                var rigidBody = GetComponent<Rigidbody2D>();
                rigidBody.AddRelativeForce(Vector2.up*Thrust*Time.deltaTime);
                rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, MaxSpeed);
                if (_exhaustParticleSystem.isStopped)
                {
                    _thrustAudioSource.loop = true;
                    _thrustAudioSource.Play();
                    _exhaustParticleSystem.Play();
                }
            }
            else
            {
                if (_exhaustParticleSystem.isPlaying)
                {
                    _thrustAudioSource.Stop();
                    _exhaustParticleSystem.Stop();
                }
            }

            bool firePressed = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump");
            if(firePressed)
            {
                FireBullet();
            }

            bool hyperPressed = Input.GetButtonDown("HyperSpace") || Input.GetButtonDown("HyperSpace2");
            if( hyperPressed)
            {
                GameManager.Instance.LevelManager.HyperSpace();
            }
        }

    }


    private void FireBullet()
    {

        if (_bulletsContainer.transform.childCount < MAX_BULLETS)
        {
            var newBullet = Instantiate(BulletPrefab);
            newBullet.Source = Bullet.Sources.PlayerShooter;
            newBullet.transform.parent = _bulletsContainer.transform;
            newBullet.transform.position = this.transform.FindChild("Muzzle").transform.position;
            newBullet.transform.rotation = this.transform.rotation;
            newBullet.transform.localScale = new Vector3(1.0f, 1.0f, 0);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up*4.0f, ForceMode2D.Impulse);
            newBullet.gameObject.SetActive(true);

            GameManager.Instance.PlayClip(ShootSound);
            Destroy(newBullet.gameObject, 1.5f);
        }
    }

    public void Show(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
    }
}
