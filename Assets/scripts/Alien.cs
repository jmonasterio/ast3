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

    public Sizes Size;
    public AudioClip ExplosionSound;
    public ParticleSystem ExplosionParticlePrefab;
    public int MAX_BULLETS = 1;
    public AudioClip ShootSound;

    private ParticleSystem _explosionParticleSystem;


    private States _state;
    private List<Vector3> _path = new List<Vector3>();
    private int _curPoint = 0;
    private float travelSpeed = 100.0f;
    public Bullet BulletPrefab;
    private Bullet _bullet;
    private GameObject _bulletsContainer;
    private GameObject _muzzle;

    public void SetPath(List<Vector3> newPath)
    {
        _path = newPath;
        this.transform.position = _path[0];
    }

    // Use this for initialization
    void Start ()
    {
        var camRect = GetCameraWorldRect();
        _muzzle = this.transform.FindChild("Muzzle").gameObject;

        _bulletsContainer = GameManager.Instance.SceneRoot.FindOrCreateTempContainer( "AlienBulletContainer");

        // TBD: DRY
        _explosionParticleSystem = Instantiate(ExplosionParticlePrefab);
        _explosionParticleSystem.transform.parent = this.transform;
        _explosionParticleSystem.transform.position = this.transform.position; //new Vector3(0.015f, -0.15f, 0.0f);
        _explosionParticleSystem.transform.rotation = this.transform.rotation;
        _explosionParticleSystem.loop = false;
        _explosionParticleSystem.Stop();
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
            newBullet.transform.position = _muzzle.transform.position;
            newBullet.transform.rotation = this.transform.rotation;
            newBullet.transform.localScale = new Vector3(2.0f, 2.0f, 0);

            var target = GameManager.Instance.LevelManager.GetAlienTargetOrNull();
            Vector2 dir;
            if (target.HasValue)
            {
                dir = target.Value - _muzzle.transform.position;
            }
            else
            {
                dir = MakeRandom2D();
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
        Show(false);
        _explosionParticleSystem.Play();
        GetComponent<Rigidbody2D>().velocity *= 0.5f; // Slow down when killed.
        GameManager.Instance.LevelManager.DestroyAlien(this, explode: true);
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
            GameManager.Instance.LevelManager.DestroyAlien(this, explode: false);
        }

    }


    public void PlaySound( bool play)
    {
        var audioSource = GetComponent<AudioSource>();
        if (play)
        {
            audioSource.loop = true;
            audioSource.clip = (this.Size == Alien.Sizes.Small)
                ? GameManager.Instance.LevelManager.AlienSoundSmall
                : GameManager.Instance.LevelManager.AlienSoundBig;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
