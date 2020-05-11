using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    private float keyLastHit = 0.0f;
    private Tetris mTS;
    void Start()
    {
        mTS = transform.GetComponent<Tetris>();
    }
    private string lP = "";
    void Update()
    {
        float delayMult = 1.0f;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lP = "";
            mTS.Rotate("left");
            // Debug.Log("rotate left");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            lP = "";
            mTS.Rotate("right");
            //  Debug.Log("rotate right");
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            lP = "";
            mTS.TryDropDown(true, true);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // reduce key hit delay if already pressed
            if (lP == "left")
            {
                delayMult = 0.3f;
            }
            if (keyLastHit < Time.time - 0.15f * delayMult)
            {
                keyLastHit = Time.time;
                mTS.MoveTet("left");
                lP = "left";
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            // reduce key hit delay if already pressed
            if (lP == "right")
            {
                delayMult = 0.3f;
            }
            if (keyLastHit < Time.time - 0.15f * delayMult)
            {
                keyLastHit = Time.time;
                mTS.MoveTet("right");
                lP = "right";
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            lP = "";
            if (keyLastHit < Time.time - 0.05f)
            {
                keyLastHit = Time.time;
                mTS.TryDropDown(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lP = "";
            mTS.TryDropDown(true, true);
            Debug.Log("Space key was pressed.");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
