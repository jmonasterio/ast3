using UnityEngine;
using Toolbox;

public class Player : Wrapped2D
{

    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public float MaxSpeed = 50.0f;
    public ParticleSystem ParticleEmitter;
    public Transform Ghost;
    //private Transform[] Ghosts = new Transform[4];
    private ParticleSystem _emitter;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
#endif
        _emitter = (ParticleSystem) Instantiate(ParticleEmitter, new Vector3(0.015f, -0.15f, 0.0f), ParticleEmitter.transform.rotation);
        _emitter.transform.parent = this.transform;
        //_emitter.enableEmission = true;
        _emitter.Stop();

        //var newGhost = (Transform)Instantiate(Ghost, new Vector3(0, _camRect.height, 0), Quaternion.identity);
        //newGhost.parent = transform;
        //newGhost = (Transform)Instantiate(Ghost, new Vector3(0, -_camRect.height, 0), Quaternion.identity);
        //newGhost.parent = transform;
        //newGhost = (Transform)Instantiate(Ghost, new Vector3(_camRect.width, 0, 0), Quaternion.identity);
        //newGhost.parent = transform;
        //newGhost = (Transform)Instantiate(Ghost, new Vector3(-_camRect.width, 0, 0), Quaternion.identity);
        //newGhost.parent = transform;

    }

    void OnGUI()
    {
        //print(GetFrameRate());
    }

    // Update is called once per frame
    void Update ()
    {
        base.TickFrameRate();
        //base.DebugForceSinusoidalFrameRate();

        float horz = Input.GetAxisRaw("Horizontal");

        base.InstantAngleChange(horz, AngleIncrement, RotateSpeed);


        float vert = Input.GetAxisRaw("Vertical");
        if (vert > 0.0f )
        {
            var rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.AddRelativeForce(Vector2.up * Thrust * Time.deltaTime);
            rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, MaxSpeed);
            if (_emitter.isStopped)
            {
                _emitter.Play();
            }
            //if (!ParticleEmitter.isPlaying)
            //{
            //    ParticleEmitter.Play();
            //}
        }
        else
        {
            if (_emitter.isPlaying)
            {
                _emitter.Stop();
            }
            //if (!ParticleEmitter.isStopped)
            //{
            //    ParticleEmitter.Stop();
            //}
        }

        WrapScreen();
    }

}
