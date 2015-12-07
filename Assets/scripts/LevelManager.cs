using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public int Level { get; set; }
    public GameObject[] Asteroids;
    public GameObject Alien;
    public GameObject Player;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame()
    {
        Level = 1;

        var newPlayer = (GameObject) Instantiate(Player, Vector3.zero, Quaternion.identity);
        newPlayer.GetComponent<Rigidbody2D>().gravityScale = 0.0f; // Turn off gravity.
        newPlayer.SetActive(true);

        for (int ii = 0; ii < 10; ii++)
        {
            
            //Instantiate()
        }
    }

    public void StartLevel()
    {
        //throw new System.NotImplementedException();
    }
}
