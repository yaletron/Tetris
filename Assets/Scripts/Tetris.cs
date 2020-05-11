using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetris : MonoBehaviour
{
    // active tet anchor pos
    public static int aTAP = 0;
    // screen size 
    private Vector2 screenSize = new Vector2(0.0f, 0.0f);
    // scale modifier
    private float setScale = 1.0f;
    // my canvas
    private GameObject mC;
    [Header("Piece (2D Image Element)")]
    // piece
    public GameObject piece;
    // bottom of the screen buffer space
    private float bSBS = 0.1f;
    // board size
    protected static int rows = 40;
    protected static int cols = 10;
    // total pieces
    public static int tP = rows * cols;
    // start position of the tetris board
    private Vector2 sP;
    // all board spots
    List<GameObject> allB = new List<GameObject>();
    // all board spots for preview pieces
    List<GameObject> allPP = new List<GameObject>();
    // all piece states
    public static List<PieceState> aPS = new List<PieceState>();
    // - - - - - "draw over" piece locations
    // active Tetromino being dropped
    public List<int> aTet = new List<int>();
    // last Tetromino being dropped
    List<int> lTet = new List<int>();
    // ghost Tetromino
    List<int> gTet = new List<int>();
    // test Tetromino being dropped
    public List<int> tTet = new List<int>();
    // sprites
    public Sprite tetSprite;
    public Sprite emptySprite;
    // my active tetromino
    public Tetrimino mTet;

    [Header("Upcoming Shapes")]
    // next / upcoming tet shapes
    public List<TetShape> nextTets = new List<TetShape>();
    public TetState mTetState = TetState.Falling;

    private int mScore = 0;

    // add to score for mulitple lines cleared
    public List<int> scorePointsForLines = new List<int>();
    // start position for next tet previews
    public List<Vector2> nextTetPreviewSP = new List<Vector2>();

    // unity reactive code
    private MUniRx mUniRx;

    public enum PieceState
    {
        Empty,
        Cyan,
        Yellow,
        Purple,
        Green,
        Red,
        Blue,
        Orange,
        White,
        None
    }

    public enum TetShape
    {
        I,
        O,
        T,
        S,
        Z,
        J,
        L
    }
    public enum TetState
    {
        Falling,
        Grounded,
        Placed,
        None
    }
    public enum ScoreMult
    {
        None,
        Twist,
        TSpin
    }
    // on floor timer
    private float oFT = 0.0F;
    float tempTimer = 0.0F;
    // create preview next tet size
    private Vector2Int pNTXY = new Vector2Int(4, 4);

    private Transform mTransform;
    private void Awake()
    {
        mTransform = transform;
        mUniRx = mTransform.GetComponent<MUniRx>();
        mTet = mTransform.GetComponent<Tetrimino>();
    }


    void Update()
    {

        if (mC != null)
        {
            mC.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Score: " + mScore;
        }

        if (mTetState == TetState.Grounded)
        {
            oFT += Time.deltaTime;
            if (oFT >= 0.5f)
            {
                oFT = 0.0F;
                mTetState = TetState.Placed;
                LockTetInPlace();
            }
        }

        tempTimer += Time.deltaTime;
        // old gravity
        /*
        if (tempTimer > 1.0F)
        {
            tempTimer -= 1.0F;
            TryDropDown();

        }*/

    }
    private void LockTetInPlace()
    {
        ScoreMult mSM = ScoreMult.None;
        // reset game
        if (aTAP >= 190)
        {
            UnityEngine.Debug.Log("RESET GAME");
            SetUp(mC);
        }
        else
        {
            // is there active tetrino?
            if (aTet.Count > 0)
            {
                for (int i = 0; i < aTet.Count; i++)
                {
                    // piece location normal?
                    if (aTet[i] >= 0 && aTet[i] < allB.Count)
                    {
                        aPS[aTet[i]] = mTet.mPS;
                        allB[aTet[i]].GetComponent<Image>().sprite = tetSprite;
                        allB[aTet[i]].GetComponent<Image>().color = mTet.mColor;
                        //lTet.Add(aTet[i]);
                    }
                }
                // T SPIN - - - - - - - - - -
                if (mTet.lastMoveWasRotate)
                {
                    mSM = ScoreMult.Twist;
                    if (mTet.mS == TetShape.T)
                    {
                        bool lockedInTSpin = LockedInTSpin();
                        if (lockedInTSpin)
                        {
                            mSM = ScoreMult.TSpin;
                        }
                    }
                }
            }
            aTet.Clear();
            SpawnNewTet();
        }

        RemoveFullLinesForPoints(mSM);
    }
    // check if 3/4 diagonal blocks are occupied or off the game board
    private bool LockedInTSpin()
    {
        bool mB = false;
        List<int> checkDiagCornersLocs = new List<int>();
        int availDiagCorners = 0;
        checkDiagCornersLocs.Add(aTAP + cols + 1);
        checkDiagCornersLocs.Add(aTAP + cols - 1);
        checkDiagCornersLocs.Add(aTAP - cols + 1);
        checkDiagCornersLocs.Add(aTAP - cols - 1);

        for (int i = 0; i < checkDiagCornersLocs.Count; i++)
        {
            if (checkDiagCornersLocs[i] < allB.Count && checkDiagCornersLocs[i] >= 0)
            {
                if (aPS[i] == PieceState.Empty)
                {
                    availDiagCorners++;
                }
            }
        }
        if (availDiagCorners == 1)
        {
            mB = true;
        }
        return mB;
    }

    private void RemoveFullLinesForPoints(ScoreMult mSM)
    {
        List<int> killLines = new List<int>();
        for (int i = 0; i < 20; i++)
        {
            int inARow = 0;
            if (aPS[i * cols] != PieceState.Empty)
            {


                for (int i2 = 0; i2 < 10; i2++)
                {
                    if (aPS[i * cols + i2] != PieceState.Empty)
                    {
                        inARow++;
                    }
                }
                if (inARow == 10)
                {
                    UnityEngine.Debug.Log("KILL LINE: " + i);
                    killLines.Add(i);
                }
            }
        }
        if (killLines.Count > 0)
        {
            int thisToGo = killLines.Count - 1;
            if (thisToGo > scorePointsForLines.Count - 1)
            {
                thisToGo = scorePointsForLines.Count - 1;
            }
            int multForTSpin = 1;
            if (mSM == ScoreMult.Twist)
            {
                multForTSpin = 2;
            }
            else if (mSM == ScoreMult.TSpin)
            {
                multForTSpin = 3;
            }
            mScore += scorePointsForLines[thisToGo] * multForTSpin;
            Debug.Log("Score Mult X " + multForTSpin + " From: " + mSM);


            for (int i3 = 0; i3 < killLines.Count; i3++)
            {
                for (int i4 = 0; i4 < 10; i4++)
                {
                    if (killLines[i3] * cols + i4 >= 0 && killLines[i3] * cols + i4 < aPS.Count)
                    {
                        aPS[killLines[i3] * cols + i4] = PieceState.Empty;
                        allB[killLines[i3] * cols + i4].GetComponent<Image>().sprite = emptySprite;
                        allB[killLines[i3] * cols + i4].GetComponent<Image>().color = Color.white;
                    }
                }
            }
            for (int i5 = 0; i5 < killLines.Count; i5++)
            {
                for (int i7 = (killLines[i5] - i5) * cols; i7 < aPS.Count; i7++)
                {
                    if (i7 < aPS.Count && i7 >= 0)
                    {
                        if (i7 + cols < aPS.Count && i7 + cols >= 0)
                        {
                            aPS[i7] = aPS[i7 + cols];
                            allB[i7].GetComponent<Image>().sprite = allB[i7 + cols].GetComponent<Image>().sprite;
                            allB[i7].GetComponent<Image>().color = allB[i7 + cols].GetComponent<Image>().color;
                        }
                    }
                }
            }
        }
    }
    public void TryDropDown(bool fromKey = false, bool hardDropDown = false)
    {
        if (mTetState == TetState.Falling)
        {
            if (mTet != null)
            {
                if (mTet.imSet)
                {
                    DropDown(fromKey, hardDropDown);
                    DrawGameState();
                }
            }
        }
    }
    public int GETATAP()
    {
        return aTAP;
    }
    public void DropDown(bool fromKey = false, bool hardDropDown = false)
    {
        if (!iSetUp)
        {
            return;
        }
        if (mTetState == TetState.Falling)
        {
            if (aTAP - cols >= 0)
            {
                bool blocked = IsAnyBlockBlockedBelow();
                if (!blocked)
                {
                    mTet.lastMoveWasRotate = false;
                    aTAP -= cols;
                    oFT = 0.0f;
                    if (fromKey)
                    {
                        mScore++;
                    }
                    if (hardDropDown)
                    {
                        DrawGameState();
                        DropDown(fromKey, hardDropDown);
                    }
                }
                else
                {
                    if (mTetState == TetState.Falling)
                    {
                        mTetState = TetState.Grounded;
                    }
                }
            }
            else
            {
                if (mTetState == TetState.Falling)
                {
                    mTetState = TetState.Grounded;
                }
            }
        }
    }
    private bool IsAnyBlockBlockedBelow()
    {
        bool isAny = false;
        // is there active tetrino?
        if (aTet.Count > 0)
        {
            for (int i = 0; i < aTet.Count; i++)
            {
                int checkHere = aTet[i] - cols;
                // piece location normal?
                if (checkHere >= 0 && checkHere < allB.Count)
                {
                    if (aPS[checkHere] != PieceState.Empty)
                    {
                        isAny = true;
                        return isAny;
                    }
                }
                else
                {
                    isAny = true;
                    return isAny;
                }
            }
        }
        return isAny;
    }
    void SizeFitScreen()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        // scale so 20.1 pieces will show incl. empty buffer at bottom of screen
        setScale = screenSize.y * (1.0f - bSBS) / 20.1f;
        // set start position of the tetris board
        float yPosStart = bSBS * screenSize.y + setScale * 0.5F - screenSize.y * 0.5f;
        sP = new Vector2(setScale * cols * -0.5f + setScale * 0.5F, yPosStart);
        nextTetPreviewSP.Clear();
        nextTetPreviewSP.Add(new Vector2(setScale * cols * -1.15f + setScale * 0.5F, yPosStart + 17.0f * setScale));
        nextTetPreviewSP.Add(new Vector2(setScale * cols * -1.15f + setScale * 0.5F, yPosStart + 12.0f * setScale));
        nextTetPreviewSP.Add(new Vector2(setScale * cols * -1.15f + setScale * 0.5F, yPosStart + 7.0f * setScale));
        //Debug.Log("Screen Size: " + screenSize + " set scale: " + setScale + " yPosStart " + yPosStart);
        mC.transform.GetChild(0).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(250.0f, -60.0f);
        int setFontSize = Mathf.RoundToInt(Screen.height * 0.06f);
        mC.transform.GetChild(0).gameObject.GetComponent<Text>().fontSize = setFontSize;
    }
    // only need to spawn pieces once
    private bool piecesSpawned = false;
    // piece total
    private int pTotal = 0;
    public bool iSetUp = false;
    // parameters: dev build? canvas GO
    public void SetUp(GameObject can)
    {
        iSetUp = false;
        mScore = 0;
        if (aPS.Count > 0)
        {
            for (int i = 0; i < aPS.Count; i++)
            {
                aPS[i] = PieceState.Empty;
                allB[i].GetComponent<Image>().sprite = emptySprite;
                allB[i].GetComponent<Image>().color = Color.white;
            }
            for (int i2 = 0; i2 < allPP.Count; i2++)
            {
                allPP[i2].GetComponent<Image>().sprite = emptySprite;
                allPP[i2].GetComponent<Image>().color = Color.white;
                allPP[i2].GetComponent<Image>().enabled = false;
            }
        }
        pTotal = 0;
        mC = can;
        SizeFitScreen();
        if (!piecesSpawned)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int i2 = 0; i2 < cols; i2++)
                {
                    CreatePiece(new Vector2((float)i2, (float)i), Vector2.zero);
                }
            }
            for (int z = 0; z < nextTetPreviewSP.Count; z++)
            {
                for (int o = 0; o < pNTXY.x; o++)
                {
                    for (int o2 = 0; o2 < pNTXY.y; o2++)
                    {
                        CreatePiece(new Vector2((float)o2, (float)o), nextTetPreviewSP[z]);
                    }
                }
            }
            piecesSpawned = true;
        }
        iSetUp = true;
        SpawnNewTet();
    }
    // position + overwrite start position?
    public void CreatePiece(Vector2 rPos, Vector2 ovrSP)
    {
        GameObject mP = Instantiate(piece, Vector3.zero, Quaternion.identity);
        if (ovrSP != Vector2.zero)
        {
            mP.transform.name = "PreviewPiece " + allPP.Count;
        }
        else
        {
            mP.transform.name = "Piece " + pTotal;
        }
        mP.transform.SetParent(mC.transform);

        mP.transform.localScale = Vector3.one;
        mP.GetComponent<RectTransform>().sizeDelta = new Vector2(setScale, setScale);
        // overwrite start pos?
        Vector2 useAsStart = sP;

        if (ovrSP != Vector2.zero)
        {
            useAsStart = ovrSP;
        }

        mP.GetComponent<RectTransform>().localPosition = new Vector3(useAsStart.x + rPos.x * setScale, useAsStart.y + rPos.y * setScale, 0.0F);

        if (ovrSP != Vector2.zero)
        {
            mP.GetComponent<Image>().enabled = false;
            allPP.Add(mP);
        }
        else
        {
            allB.Add(mP);
            aPS.Add(PieceState.Empty);
            pTotal++;
        }
    }
    public void DrawGameState(bool forceAllRedraw = false)
    {
        if (mTet.mPL != null)
        {
            if (mTet.mPL.Count == 4)
            {
                aTet.Clear();
                aTet.Add(mTet.mPL[0] + aTAP);
                aTet.Add(mTet.mPL[1] + aTAP);
                aTet.Add(mTet.mPL[2] + aTAP);
                aTet.Add(mTet.mPL[3] + aTAP);
            }
        }
        // clear last drawn tetrino (check for if this spot is now occupied)
        if (lTet.Count > 0)
        {
            for (int i2 = 0; i2 < lTet.Count; i2++)
            {
                if (lTet[i2] >= 0 && lTet[i2] < allB.Count)
                {
                    if (aPS[lTet[i2]] == PieceState.Empty)
                    {
                        allB[lTet[i2]].GetComponent<Image>().sprite = emptySprite;
                        allB[lTet[i2]].GetComponent<Image>().color = Color.white;
                    }
                    //allB[lTet[i2]].GetComponent<Image>().color = Color.white;
                }
            }
            lTet.Clear();
        }
        // is there active tetrino?
        if (aTet.Count > 0)
        {
            for (int i = 0; i < aTet.Count; i++)
            {
                // piece location normal?
                if (aTet[i] >= 0 && aTet[i] < allB.Count)
                {
                    allB[aTet[i]].GetComponent<Image>().sprite = tetSprite;
                    allB[aTet[i]].GetComponent<Image>().color = mTet.mColor;
                    lTet.Add(aTet[i]);
                }
            }
        }
    }
    // spawn new tet
    private void SpawnNewTet()
    {
        mTetState = TetState.Falling;
        // start position of the new Tet
        aTAP = 204;
        if (nextTets.Count < System.Enum.GetValues(typeof(TetShape)).Length)
        {
            PopulateNextTets();
        }
        mTet.rotS = 0;
        mTet.lSRotS = 0;
        mTet.SetUpTet(nextTets[0], aTAP);
        nextTets.RemoveAt(0);
        mUniRx.DoTetPreview();
    }
    private void PopulateNextTets()
    {
        // get one of each shape to add to queue
        List<TetShape> addToNextTets = new List<TetShape>();
        foreach (TetShape x in Enum.GetValues(typeof(TetShape)))
        {
            addToNextTets.Add(x);
        }
        // shuffle
        for (int i = 0; i < addToNextTets.Count; i++)
        {
            TetShape temp = addToNextTets[i];
            int randomIndex = UnityEngine.Random.Range(i, addToNextTets.Count);
            addToNextTets[i] = addToNextTets[randomIndex];
            addToNextTets[randomIndex] = temp;
        }
        for (int i2 = 0; i2 < addToNextTets.Count; i2++)
        {
            nextTets.Add(addToNextTets[i2]);
        }
        mUniRx.DoTetPreview();
    }
    public void MoveTet(string thisDir)
    {
        bool blocked = IsAnyBlockBlockedSide(thisDir);
        if (!blocked)
        {
            mTet.lastMoveWasRotate = false;
            if (thisDir == "left")
            {
                aTAP--;
                if (mTetState == TetState.Grounded)
                {
                    oFT += 0.35f;
                    mTetState = TetState.Falling;
                }
            }
            else
            {
                aTAP++;
                if (mTetState == TetState.Grounded)
                {
                    oFT += 0.35f;
                    mTetState = TetState.Falling;
                }
            }
            DrawGameState();
        }
    }
    private bool IsAnyBlockBlockedSide(string thisDir)
    {
        bool isAny = false;
        // is there active tetrino?
        if (aTet.Count > 0)
        {
            for (int i = 0; i < aTet.Count; i++)
            {
                int checkHere = aTet[i];
                if (thisDir == "left")
                {
                    // on left edge already
                    if (checkHere == 0 || checkHere % cols == 0)
                    {
                        isAny = true;
                        return isAny;
                    }
                    checkHere--;
                }
                else
                {
                    // on right edge already
                    if ((checkHere + 1) % cols == 0)
                    {
                        isAny = true;
                        return isAny;
                    }
                    checkHere++;
                }
                // piece location normal?
                if (checkHere >= 0 && checkHere < allB.Count)
                {
                    if (aPS[checkHere] != PieceState.Empty)
                    {
                        isAny = true;
                        return isAny;
                    }
                }
                else
                {
                    isAny = true;
                    return isAny;
                }
            }
        }
        return isAny;
    }
    public void Rotate(string thisDir)
    {
        mTet.RotateTet(thisDir);
        DrawGameState();
    }

    public bool CheckPieceSpaceBlocked(int checkHere)
    {
        bool blocked = false;
        if (checkHere >= 0 && checkHere < allB.Count)
        {
            if (aPS[checkHere] == PieceState.Empty)
            {
                blocked = true;
            }
        }
        return blocked;
    }

    public void SetUpTetPreview(int thisP, List<Vector2Int> tI, Color tCP)
    {
        // startLoc
        int oneThird = Mathf.FloorToInt((float)allPP.Count / 3.0f);
        Vector2Int betweenHere = new Vector2Int(0, oneThird);
        if (thisP == 1)
        {
            betweenHere = new Vector2Int(oneThird, oneThird * 2);
        }
        else if (thisP == 2)
        {
            betweenHere = new Vector2Int(oneThird * 2, oneThird * 3);
        }
        // give loc instead of x , y 
        List<int> mLocs = new List<int>();
        for (int e = 0; e < tI.Count; e++)
        {
            int addThis = 0;
            addThis += tI[e].x;
            addThis += tI[e].y * pNTXY.x + 1;

            addThis += betweenHere.x;

            mLocs.Add(addThis);
        }
        for (int i = betweenHere.x; i < betweenHere.y; i++)
        {
            // yes tet loc
            bool yesTetLoc = false;
            for (int e2 = 0; e2 < mLocs.Count; e2++)
            {
                if (mLocs[e2] == i)
                {
                    yesTetLoc = true;
                }
            }
            if (!yesTetLoc)
            {
                allPP[i].GetComponent<Image>().sprite = emptySprite;
                allPP[i].GetComponent<Image>().color = Color.white;
                allPP[i].GetComponent<Image>().enabled = false;
            }
            else
            {
                allPP[i].GetComponent<Image>().enabled = true;
                allPP[i].GetComponent<Image>().sprite = tetSprite;
                allPP[i].GetComponent<Image>().color = tCP;
            }
        }
    }
}

