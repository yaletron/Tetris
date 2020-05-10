using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class Tetrimino : Tetris
{

    // my piece directions from anchor
    public List<Vector2Int> mPDir = new List<Vector2Int>();
    // my piece locations
    public List<int> mPL = new List<int>();
    // my test piece locations
    public List<int> mTPL = new List<int>();

    // my shape
    public TetShape mS;
    // rotation state
    public int rotS = 0;
    // last good rotation state
    public int lSRotS = 0;

    // the 'anchor position' of the tetriminio for the I in a certain rotation it is empty space
    public int anchorPos = 0;

    public Color mColor = Color.white;
    public PieceState mPS;

    // make sure fully set up before falling
    public bool imSet = false;

    public bool lastMoveWasRotate = false;


    // Start is called before the first frame update

    // set up tet for preview
    public void SetUpPreviewTet(int thisP, TetShape tS)
    {
        //UnityEngine.Debug.Log(" SetUpPreviewTet " + thisP + " shape " + tS);
        List <Vector2Int> tI = new List<Vector2Int>();
        Color tCP = Color.white;
        switch (tS)
        {
            case TetShape.I:
                tCP = Color.cyan;
                tI.Add(new Vector2Int(-1, 0));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(1, 0));
                tI.Add(new Vector2Int(2, 0));
                break;
            case TetShape.J:
                tCP = Color.blue;
                tI.Add(new Vector2Int(-1, 1));
                tI.Add(new Vector2Int(-1, 0));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(1, 0));
                break;
            case TetShape.L:
                tCP = new Color(1.0f, 0.45f, 0.0f, 1.0f);// orange
                tI.Add(new Vector2Int(1, 1));
                tI.Add(new Vector2Int(-1, 0));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(1, 0));
                break;
            case TetShape.O:
                tCP = Color.yellow;
                tI.Add(new Vector2Int(0, 1));
                tI.Add(new Vector2Int(1, 1));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(1, 0));
                break;
            case TetShape.S:
                tCP = Color.green;
                tI.Add(new Vector2Int(-1, 0));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(0, 1));
                tI.Add(new Vector2Int(1, 1));
                break;
            case TetShape.T:
                tCP = new Color(0.5f, 0.0f, 0.5f, 1.0f);// purple
                //mColor = Color.magenta;
                tI.Add(new Vector2Int(0, 1));
                tI.Add(new Vector2Int(-1, 0));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(1, 0));
                break;
            case TetShape.Z:
                tCP = Color.red;
                tI.Add(new Vector2Int(-1, 1));
                tI.Add(new Vector2Int(0, 1));
                tI.Add(new Vector2Int(0, 0));
                tI.Add(new Vector2Int(1, 0));
                break;
        }
        Tetris mTetris = transform.GetComponent<Tetris>();
        mTetris.SetUpTetPreview(thisP, tI, tCP);
    }
    // set up tet to drop
    public void SetUpTet(TetShape tS, int sP)
    {

        anchorPos = sP;
        mS = tS;
        mPL.Clear();
        mPDir.Clear();

        switch (mS)
        {
            case TetShape.I:
                mPS = PieceState.Cyan;
                mColor = Color.cyan;
                mPDir.Add(new Vector2Int(-1, 0));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(1, 0));
                mPDir.Add(new Vector2Int(2, 0));
                break;
            case TetShape.J:
                mPS = PieceState.Blue;
                mColor = Color.blue;
                mPDir.Add(new Vector2Int(-1, 1));
                mPDir.Add(new Vector2Int(-1, 0));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(1, 0));
                break;
            case TetShape.L:
                mPS = PieceState.Orange;
                mColor = new Color(1.0f, 0.45f, 0.0f, 1.0f);// orange
                mPDir.Add(new Vector2Int(1, 1));
                mPDir.Add(new Vector2Int(-1, 0));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(1, 0));
                break;
            case TetShape.O:
                mPS = PieceState.Yellow;
                mColor = Color.yellow;
                mPDir.Add(new Vector2Int(0, 1));
                mPDir.Add(new Vector2Int(1, 1));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(1, 0));
                break;
            case TetShape.S:
                mPS = PieceState.Green;
                mColor = Color.green;
                mPDir.Add(new Vector2Int(-1, 0));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(0, 1));
                mPDir.Add(new Vector2Int(1, 1));
                break;
            case TetShape.T:
                mPS = PieceState.Purple;
                mColor = new Color(0.5f, 0.0f, 0.5f, 1.0f);// purple
                //mColor = Color.magenta;
                mPDir.Add(new Vector2Int(0, 1));
                mPDir.Add(new Vector2Int(-1, 0));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(1, 0));
                break;
            case TetShape.Z:
                mPS = PieceState.Red;
                mColor = Color.red;
                mPDir.Add(new Vector2Int(-1, 1));
                mPDir.Add(new Vector2Int(0, 1));
                mPDir.Add(new Vector2Int(0, 0));
                mPDir.Add(new Vector2Int(1, 0));
                break;
        }

        foreach (Vector2Int dInfo in mPDir)
        {
            Vector2Int useAsDInfo = new Vector2Int(0, 0);
            if (rotS == 0)
            {
                useAsDInfo = new Vector2Int(dInfo.x, dInfo.y);
            }
            else if (rotS == 1)
            {
                useAsDInfo = new Vector2Int(dInfo.y, dInfo.x * -1);
            }
            else if (rotS == 2)
            {
                useAsDInfo = new Vector2Int(dInfo.x * -1, dInfo.y * -1);
            }
            else if (rotS == 3)
            {
                useAsDInfo = new Vector2Int(dInfo.y * -1, dInfo.x);
            }

            int addThis = 0;
            addThis += useAsDInfo.x;
            addThis += useAsDInfo.y * cols;

            mPL.Add(addThis);
        }



        lSRotS = rotS;

        mTetState = TetState.Falling;
        // immediate fall happens here -- 

        DropDown();


       // Debug.Log("Set up new tet: " + mS.ToString());
        imSet = true;


        // DrawGameState();


    }
    public void RotateTet(string thisDir)
    {
        int setRot = rotS;

        if (mS == TetShape.O)
        {
            return;
        }
        if (thisDir == "right")
        {
            setRot++;
        }
        else
        {
            setRot--;
        }
        if (setRot < 0)
        {
            setRot = 3;
        }
        if (setRot > 3)
        {
            setRot = 0;
        }

        bool blocked = TestNewRot(setRot);
        if (!blocked)
        {
            lastMoveWasRotate = true;
            rotS = setRot;
            //UnityEngine.Debug.Log("rotate tet: " + thisDir);
            SetUpTet(mS, anchorPos);
        }
    }

    public bool TestNewRot(int setRot)
    {
        mTPL.Clear();
        bool blocked = false;

        foreach (Vector2Int dInfo in mPDir)
        {
            Vector2Int useAsDInfo = new Vector2Int(0, 0);
            if (setRot == 0)
            {
                useAsDInfo = new Vector2Int(dInfo.x, dInfo.y);
            }
            else if (setRot == 1)
            {
                useAsDInfo = new Vector2Int(dInfo.y, dInfo.x * -1);
            }
            else if (setRot == 2)
            {
                useAsDInfo = new Vector2Int(dInfo.x * -1, dInfo.y * -1);
            }
            else if (setRot == 3)
            {
                useAsDInfo = new Vector2Int(dInfo.y * -1, dInfo.x);
            }
            int addThis = 0;
            addThis += useAsDInfo.x;
            addThis += useAsDInfo.y * cols;
            mTPL.Add(addThis);
        }



        Vector2Int onBothEdges = new Vector2Int(0, 0);
        for (int i = 0; i < mTPL.Count; i++)
        {
            int mATAP = GETATAP();
            int checkHere = mTPL[i] + mATAP;

            if (checkHere < 0 || checkHere >= tP)
            {
                blocked = true;
                return blocked;
            }
            // left edge
            if (checkHere == 0 || checkHere % cols == 0)
            {
                onBothEdges.x = 1;
            }
            // right edge
            if ((checkHere + 1) % cols == 0)
            {
                onBothEdges.y = 1;
            }
            // check if it hits a piece or tet ends up on both sides of the board
            if (onBothEdges.x == 1 && onBothEdges.y == 1)
            {
                blocked = true;
                return blocked;
            }
            // check if rotates into non empty space
            bool seeBlocked = true;
            if (checkHere >= 0 && checkHere < aPS.Count)
            {
                //Debug.Log(" aPS[checkHere]: " + aPS[checkHere] + " checkHere: " + checkHere);
                if (aPS[checkHere] != PieceState.Empty)
                {
                    blocked = true;
                    return blocked;
                }
            }
            if (seeBlocked = CheckPieceSpaceBlocked(checkHere))
            {
                blocked = true;
                return blocked;
            }

        }


        return blocked;
    }




}
