using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class Player : Wrapped2D
{

    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public float MaxSpeed = 50.0f;
    public int PlayerIndex = 0; // Or 1, for 2 players.
    public ParticleSystem ExhaustParticlePrefab;
    public ParticleSystem ExplosionParticlePrefab;
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
        _exhaust.transform.position = ExhaustParticlePrefab.transform.position; //new Vector3(0.015f, -0.15f, 0.0f);
        _exhaust.transform.rotation = ExhaustParticlePrefab.transform.rotation;
        //_exhaust.enableEmission = true;
        _exhaust.Stop();

        _explosion = Instantiate(ExplosionParticlePrefab);
        _explosion.transform.parent = this.transform;
        _explosion.transform.position = ExhaustParticlePrefab.transform.position; //new Vector3(0.015f, -0.15f, 0.0f);
        _explosion.transform.rotation = ExhaustParticlePrefab.transform.rotation;
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
        _state = State.Killed;
        Show(false);
        _explosion.Play();
        StartCoroutine(DestroyPlayerLater());
        GameManager.Instance.PlayerKilled(this);
    }

    private IEnumerator DestroyPlayerLater()
    {
        yield return new WaitForSeconds(1.0f);

        Destroy(this);
    }

    // Update is called once per frame
    void Update ()
    {
        if (_state == State.Killed)
        {
            return;
        }

        //base.DebugForceSinusoidalFrameRate();

        float horz = Input.GetAxisRaw("Horizontal");

        base.InstantAngleChange(horz, AngleIncrement, RotateSpeed);


        float vert = Input.GetAxisRaw("Vertical");
        if (vert > 0.0f )
        {
            var rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.AddRelativeForce(Vector2.up * Thrust * Time.deltaTime);
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

        WrapScreen();
    }

    public void Show(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
    }
}
