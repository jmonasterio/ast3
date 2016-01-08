using UnityEngine;
using System.Collections;
using Toolbox;

public class Asteroid : Base2DBehaviour
{

    public AudioClip ExplosionSound;

    public enum Sizes
    {
        Small,
        Medium,
        Large
    }

    public Sizes Size = Sizes.Small;

}
