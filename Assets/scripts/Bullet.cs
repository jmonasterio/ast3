using UnityEngine;
using System.Collections;

public class Bullet : Wrapped2D {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    WrapScreen();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Asteroid")) // TBD: Improve.
        {
            Destroy(other.gameObject); // TBD: Explosion & Split asteroid & keep count.
            Destroy(this.gameObject);
        }
    }
}
