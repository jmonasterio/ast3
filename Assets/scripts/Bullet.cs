using System;
using UnityEngine;
using System.Collections;
using Toolbox;

public class Bullet : Wrapped2D {

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        WrapScreen();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Asteroid")) // TBD: Improve.
        {
            Asteroid ast = other.gameObject.GetComponent<Asteroid>(); // This is great. I can get associated script object for asteroid.

            GameManager.Instance.PlayClip(ast.ExplosionSound);


            if (ast.Size == Asteroid.Sizes.Large)
            {
                // Create 2 new mediumes
                GameManager.Instance.LevelManager.ReplaceAsteroidWith(ast, 2, Asteroid.Sizes.Medium, this);
                GameManager.Instance.Score += 10;
            }
            else if (ast.Size == Asteroid.Sizes.Medium)
            {
                // Create 2 new smalls.
                GameManager.Instance.LevelManager.ReplaceAsteroidWith(ast, 3, Asteroid.Sizes.Small, this);
                GameManager.Instance.Score += 20;
            }
            else if (ast.Size == Asteroid.Sizes.Small)
            {
                GameManager.Instance.LevelManager.ReplaceAsteroidWith(ast, 0, Asteroid.Sizes.Small, this); // Size does not matter.
                GameManager.Instance.Score += 30;
            }
            else
            {
                throw new NotImplementedException();

            }
            // Destroy the bullet.
            Destroy(this.gameObject);


        }
    }
}
