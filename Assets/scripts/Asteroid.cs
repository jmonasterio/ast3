using UnityEngine;
using System.Collections;
using Toolbox;

public class Asteroid : Wrapped2D {

    public enum Sizes
    {
        Small,
        Medium,
        Large
    }

    public Sizes Size = Sizes.Small;

    // Update is called once per frame
	void Update () {
        WrapScreen();
    }
}
