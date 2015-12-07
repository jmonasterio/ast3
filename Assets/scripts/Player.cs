using UnityEngine;
using Toolbox;

public class Player : Base2DBehaviour
{
    
    public float RotateSpeed = 100.0f;
    public float Thrust = 100.0f;
    public float AngleIncrement = 5.0f;
    public ParticleEmitter ParticleEmitter;
    private Rect _camRect;
    private float _targetAngle { get; set; }
    //ivate bool _lastAngleChangeWasIncrement;

    // Use this for initialization
    void Start ()
    {
        _camRect = GetCameraWorldRect();
    }

    // Update is called once per frame
    void Update () {

	    float horz = Input.GetAxisRaw("Horizontal");
        
        //RotateTransformByIncrement( horz, AngleIncrement, RotateSpeed, ref _targetAngle, )
        

        // The bug here is that we may be stopping if we're really close to 5 degree increment.
        // So our speed is not consistent.
        if (horz != 0.0f)
	    {
	        float angleToRotate = -horz*AngleIncrement;
            var curAngle = transform.eulerAngles.z;
            _targetAngle = MathfExt.RoundToNearestMultiple( curAngle + angleToRotate, AngleIncrement);
	        var newAngle = Mathf.MoveTowardsAngle(curAngle, _targetAngle, RotateSpeed*Time.deltaTime);
	        transform.Rotate( 0.0f, 0.0f, newAngle-curAngle );
	    }
        else if (0.0f != Mathf.DeltaAngle(_targetAngle, transform.rotation.eulerAngles.z))
        {
            var curAngle = transform.eulerAngles.z;
            var newAngle = Mathf.MoveTowardsAngle(curAngle, _targetAngle, RotateSpeed * Time.deltaTime);
            transform.Rotate(0.0f, 0.0f, newAngle-curAngle);
        }

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
