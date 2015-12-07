using UnityEngine;
using Toolbox;

public class Player : Base2DBehaviour
{

    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public ParticleEmitter ParticleEmitter;
    private Rect _camRect;

    // Use this for initialization
    void Start ()
    {
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
#endif

        _camRect = GetCameraWorldRect();
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
            /*
            float thetaDegrees = transform.eulerAngles.z-90;
            float thetaRad = (float) Mathf.PI* thetaDegrees / 180.0f;
            float sinTheta = (float) Mathf.Sin(thetaRad);
            float cosTheta = (float) Mathf.Cos(thetaRad);
            float timeFactor = 100*Time.deltaTime;
            float forceX = cosTheta*timeFactor;
            float forceY = sinTheta*timeFactor;
            var force = new Vector2( transform.rotation.z, transform.rotation.w);
            print(transform.rotation +","+ force);
            */
            //rigidBody.AddRelativeForce(force);
            rigidBody.AddRelativeForce(Vector2.up * Thrust * Time.deltaTime);
        }


        if ( !_camRect.Contains(this.TransformTo2D()))
	    {
            StopRigidBody();
	    }


    }


    private void StopRigidBody()
    {
        var rigidBody = GetComponent<Rigidbody2D>();
        transform.position = new Vector3(0,0,0);
        rigidBody.velocity = new Vector3(0,0,0);
    }
}
