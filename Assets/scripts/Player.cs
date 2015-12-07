using UnityEngine;
using Toolbox;

public class Player : Wrapped2D
{

    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public float MaxSpeed = 50.0f;
    public ParticleEmitter ParticleEmitter;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
#endif
    }

    void OnGUI()
    {
        print(GetFrameRate());
    }

    // Update is called once per frame
    void Update ()
    {
        base.TickFrameRate();
        //base.DebugForceSinusoidalFrameRate();

        float horz = Input.GetAxisRaw("Horizontal");

        base.InstantAngleChange(horz, AngleIncrement, RotateSpeed);


        bool vert = Input.GetButton("Vertical");
        if (vert )
        {
            var rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.AddRelativeForce(Vector2.up * Thrust * Time.deltaTime);
            rigidBody.velocity = Vector2.ClampMagnitude(rigidBody.velocity, MaxSpeed);
        }

        WrapScreen();
    }

}
