using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Toolbox;

public class Player : Wrapped2D
{
    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public float MaxSpeed = 50.0f;
    public int PlayerIndex = 0; // Or 1, for 2 players.
    [Obsolete]
    public ParticleSystem ExhaustParticlePrefab;
    [Obsolete]
    public ParticleSystem ExplosionParticlePrefab;
    [Obsolete]
    public Transform GhostPrefab;
    //private Transform[] Ghosts = new Transform[4];
    private ParticleSystem _exhaust;
    private ParticleSystem _explosion;

    private enum State
    {
        Alive = 0,
        Killed = 1
    }

    private State _state;


    // Use this for initialization
    public void Start()
    {
        _exhaust = Instantiate(ExhaustParticlePrefab);
        _exhaust.transform.parent = this.transform;
        _exhaust.transform.position = this.transform.FindChild("ExhaustExit").transform.position;
        _exhaust.transform.rotation = this.transform.rotation;
        //_exhaust.enableEmission = true;
        _exhaust.Stop();

        _explosion = Instantiate(ExplosionParticlePrefab);
        _explosion.transform.parent = this.transform;
        _explosion.transform.position = this.transform.position; //new Vector3(0.015f, -0.15f, 0.0f);
        _explosion.transform.rotation = this.transform.rotation;
        _explosion.loop = false;
        _explosion.Stop();

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
        _state = State.Killed;
        Show(false);
        _exhaust.Stop();
        _explosion.Play();
        GetComponent<Rigidbody2D>().velocity *= 0.5f; // Slow down when killed.
        GameManager.Instance.PlayerKilled(this);
        Destroy(this.gameObject, _explosion.duration + 0.5f); 
    }

    void OnDestroy()
    {
        // Cleanup
        if (_exhaust != null)
        {
            _exhaust.Stop();
        }
        if (_explosion != null)
        {
            _explosion.Stop();
        }
        SafeDestroy( ref _exhaust);
        SafeDestroy( ref _explosion);
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
                if (_exhaust.isStopped)
                {
                    _exhaust.Play();
                }
            }
            else
            {
                if (_exhaust.isPlaying)
                {
                    _exhaust.Stop();
                }
            }
        }

        // Always run this, so even explosions wrap.
        WrapScreen();
    }

    public void Show(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
    }
}
