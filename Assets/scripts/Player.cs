using UnityEngine;
using Toolbox;


public class Player : Base2DBehaviour
{
    public class GoNames
    {
        public const string BULLET_CONTAINER_NAME = "PlayerBulletsContainer";
        public const string EXHAUST_EXIT = "ExhaustExit"; // TBD: handle like a muzzle.
    }

    public int MAX_BULLETS = 3;

    public float RotateSpeed = 150f;
    public float Thrust = 40.0f;
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
    public Muzzle MuzzleChild;


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
    private float _lastHyperSpaceTime;


    // Use this for initialization
    public void Start()
    {
        _thrustAudioSource = GetComponent<AudioSource>();

        _bulletsContainer = GameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.BULLET_CONTAINER_NAME);

        _exhaustParticleSystem = ExhaustParticlePrefab.InstantiateAtTransform( this.transform.FindChild(GoNames.EXHAUST_EXIT).transform);
        _exhaustParticleSystem.loop = false;
        _exhaustParticleSystem.Stop();
        _exhaustParticleSystem.transform.rotation = new Quaternion(0f, 0f, -180f, 0f);

        _explosionParticleSystem = ExplosionParticlePrefab.InstantiateAtTransform( this.transform);
        _explosionParticleSystem.loop = false;
        _explosionParticleSystem.Stop();

        _state = State.Alive;
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


    // Update is called once per frame
    void FixedUpdate()
    {
        if (_state != State.Killed)
        {

            //base.DebugForceSinusoidalFrameRate();

            float horz = Input.GetAxisRaw(GameManager.Buttons.HORIZ);

            // TBD: Move this into the component. Will need an ENABLED flag, tied to KILLED.
            GetComponent<Rotator>().InstantAngleChange(horz, AngleIncrement, RotateSpeed);


            float vert = Input.GetAxisRaw(GameManager.Buttons.VERT);

            // Maybe a thruster component? Or maybe Rotator+Thruster=PlayerMover component.
            if (vert > 0.0f)
            {
                var rigidBody = GetComponent<Rigidbody2D>();
                rigidBody.AddRelativeForce(Vector2.up*Thrust*Time.deltaTime);
                rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, MaxSpeed);
                if (_exhaustParticleSystem.isStopped)
                {
                    _exhaustParticleSystem.loop = true;
                    _exhaustParticleSystem.Play();
                }
                if (!_thrustAudioSource.isPlaying)
                {
                    _thrustAudioSource.loop = true;
                    _thrustAudioSource.Play();
                    Debug.Assert(_thrustAudioSource.isPlaying);
                }
            }
            else
            {
                if (_exhaustParticleSystem.isPlaying)
                {
                    _exhaustParticleSystem.Stop();
                }
                if (_thrustAudioSource.isPlaying)
                {
                    _thrustAudioSource.Stop();
                    Debug.Assert(!_thrustAudioSource.isPlaying);
                }
            }

            bool firePressed = Input.GetButtonDown(GameManager.Buttons.FIRE1) || Input.GetButtonDown(GameManager.Buttons.JUMP);
            if(firePressed)
            {
                FireBullet();
            }

            bool hyperPressed = Input.GetButtonDown(GameManager.Buttons.HYPERSPACE) || Input.GetButtonDown(GameManager.Buttons.HYPERSPACE2);
            if( hyperPressed && (Time.time - _lastHyperSpaceTime > 1.0))
            {
                GameManager.Instance.SceneController.HyperSpace();
                _lastHyperSpaceTime = Time.time;
            }
        }

    }

    // TBD: Shooter component.
    private void FireBullet()
    {

        if (_bulletsContainer.transform.childCount < MAX_BULLETS)
        {
            var newBullet = BulletPrefab.InstantiateInTransform(_bulletsContainer.transform);
            newBullet.Source = Bullet.Sources.PlayerShooter;
            newBullet.transform.position = MuzzleChild.transform.position;
            newBullet.transform.rotation = this.transform.rotation;
            //newBullet.transform.localScale = new Vector3(0.5f, 0.5f, 0);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up*1.4f, ForceMode2D.Impulse);
            newBullet.gameObject.SetActive(true);

            GameManager.Instance.PlayClip(ShootSound);
            Destroy(newBullet.gameObject, 1.4f);
        }
    }

    public void Show(bool b)
    {
        GetComponent<SpriteRenderer>().enabled = b;
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
        GameObjectExt.SafeDestroy(ref _exhaustParticleSystem);
        GameObjectExt.SafeDestroy(ref _explosionParticleSystem);
    }

    public static void ClearBullets()
    {
        var abc = GameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.BULLET_CONTAINER_NAME);
        GameObjectExt.DestroyChildren(abc);

    }
}
