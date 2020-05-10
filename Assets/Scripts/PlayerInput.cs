using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;


public class PlayerInput : MonoBehaviour
{
    private float keyLastHit = 0.0f;
    private Tetris mTS;
    // Start is called before the first frame update
    void Start()
    {
        mTS = transform.GetComponent<Tetris>();
        /*
        this.UpdateAsObservable()
        .Subscribe(_
    */

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            mTS.Rotate("left");
           // Debug.Log("rotate left");
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            mTS.Rotate("right");
          //  Debug.Log("rotate right");
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            mTS.TryDropDown(true, true);
           // Debug.Log("Return key was pressed.");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (keyLastHit < Time.time - 0.15f)
            {
                keyLastHit = Time.time;
                mTS.MoveTet("left");
              //  Debug.Log("Return key was pressed.");
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (keyLastHit < Time.time - 0.15f)
            {
                keyLastHit = Time.time;
                mTS.MoveTet("right");
              //  Debug.Log("Return key was pressed.");
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (keyLastHit < Time.time - 0.05f)
            {
                keyLastHit = Time.time;
                mTS.TryDropDown(true);

              //  Debug.Log("Down arrow pressed");
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mTS.TryDropDown(true, true);
            Debug.Log("Space key was pressed.");
        }
    }
}
