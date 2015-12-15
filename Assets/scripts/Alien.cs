using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Toolbox;

[RequireComponent(typeof(AudioSource))]
public class Alien : Base2DBehaviour
{
    public enum Sizes
    {
        Big,
        Small
    }

    public enum States
    {
        Live,
        Killed
    }

    public struct GoNames
    {
        public const string ASTEROID_BULLET_CONTAINER = "AlienBulletContainer";
    }

    public Sizes Size;
    public AudioClip ExplosionSound;
    public ParticleSystem ExplosionParticlePrefab;
    public int MAX_BULLETS = 1;
    public AudioClip ShootSound;
    public Bullet BulletPrefab;
    public Muzzle MuzzleChild;

    private ParticleSystem _explosionParticleSystem;


    private States _state;
    private List<Vector3> _path = new List<Vector3>();
    private int _curPoint = 0;
    private float travelSpeed = 25.0f;
    private Bullet _bullet;
    private GameObject _bulletsContainer;

    public void SetPath(List<Vector3> newPath)
    {
        _path = newPath;
        this.transform.position = _path[0];
    }

    // Use this for initialization
    void Start()
    {
        _bulletsContainer = GameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.ASTEROID_BULLET_CONTAINER);

        _explosionParticleSystem = ExplosionParticlePrefab.InstantiateAtTransform( this.transform);
        _explosionParticleSystem.loop = false;
        _explosionParticleSystem.Stop();
    }


    public static void ClearBullets()
    {
        var abc = GameManager.Instance.SceneRoot.FindOrCreateTempContainer(GoNames.ASTEROID_BULLET_CONTAINER);
        while (abc.transform.childCount > 0)
        {
            Destroy(abc.transform.GetChild(0).gameObject);
        }

    }


    // Update is called once per frame
    void Update ()
    {
        var rigidBody = GetComponent<Rigidbody2D>();

        var target = _path[_curPoint];
        var curPos = this.transform.position;

        if (Vector2.Distance(target, curPos)<= 0.25)
        {
            GoToNextPoint();
        }
        else
        {
            // Go towards
            var dir = target - curPos;
            rigidBody.velocity = dir.normalized * travelSpeed*Time.deltaTime;

        }

        var hit = Physics2D.Raycast(transform.position, rigidBody.velocity, distance:10.0f, layerMask:9 /* Asteroid */);
        if (hit.collider != null)
        {
            float distance = Vector2.Distance(hit.point, transform.position); //n + Vector2.Up From2D(rigidBody.velocity.normalized*transform.localScale.magnitude)); // Extra math to not hit self.
            if (distance > 0 && distance < 4.0f)
            {
                print("distance: " + distance);
                distance = 0;
            }
            
        }

        if (_bullet == null)
        {
            FireBullet();
        }

    }

    private void FireBullet()
    {

        if (_bulletsContainer.transform.childCount < MAX_BULLETS)
        {
            var newBullet = Instantiate(BulletPrefab);
            newBullet.Source = Bullet.Sources.AlienShooter;
            newBullet.transform.parent = _bulletsContainer.transform;
            newBullet.transform.position = MuzzleChild.transform.position;
            newBullet.transform.rotation = this.transform.rotation;
            //newBullet.transform.localScale = new Vector3(2.0f, 2.0f, 0);

            var target = GameManager.Instance.SceneController.GetAlienTargetOrNull();
            Vector2 dir;
            if (target.HasValue)
            {
                dir = target.Value - MuzzleChild.transform.position;
            }
            else
            {
                dir = MathfExt.MakeRandom2D();
            }

            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(dir *0.5f, ForceMode2D.Impulse);
            newBullet.gameObject.SetActive(true);

            GameManager.Instance.PlayClip(ShootSound);
            Destroy(newBullet.gameObject, 3.0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetInstanceID() == this.GetInstanceID())
        {
            // Avoid collision with self.
            return;
        }
        if (other.gameObject.GetComponent<Bullet>())
        {
            var bullet = other.gameObject.GetComponent<Bullet>();
            if (bullet.Source == Bullet.Sources.AlienShooter)
            {
                // We don't allow shooting ourself
                return; // 
            }
        }
        AlienKilled();
    }

    private void AlienKilled()
    {
        _state = States.Killed;
        this.gameObject.Show(false);
        _explosionParticleSystem.Play();
        GetComponent<Rigidbody2D>().velocity *= 0.5f; // Slow down when killed.
        GameManager.Instance.SceneController.DestroyAlien(this, explode: true);
        GameManager.Instance.PlayClip(ExplosionSound);
        GameManager.Instance.Score += ((this.Size == Alien.Sizes.Small) ? 1000 : 500);
        Destroy(this.gameObject, _explosionParticleSystem.duration + 0.5f);
    }

    private void GoToNextPoint()
    {
        _curPoint++;
        if (_curPoint >= _path.Count)
        {
            //If end of path, we're done.
            GameManager.Instance.SceneController.DestroyAlien(this, explode: false);
        }

    }


    public void PlaySound( bool play)
    {
        var audioSource = GetComponent<AudioSource>();
        if (play)
        {
            audioSource.loop = true;
            audioSource.clip = (this.Size == Alien.Sizes.Small)
                ? GameManager.Instance.SceneController.AlienSoundSmall
                : GameManager.Instance.SceneController.AlienSoundBig;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
