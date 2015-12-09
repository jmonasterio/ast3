using System;
using UnityEngine;
using System.Collections;
using Toolbox;

namespace Assets.Scripts
{

    public class Lives : Base2DBehaviour
    {

        private int _lives = -1;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var lives = GameManager.Instance.Lives;
            if (_lives != lives)
            {
                if (lives > 0)
                {
                    GetComponent<TextMesh>().text = new String('^', lives);
                }
                else
                {
                    GetComponent<TextMesh>().text = String.Empty;
                }
                _lives = lives;
            }
        }
    }
}
