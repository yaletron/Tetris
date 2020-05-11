using System.Collections;
using UnityEngine;
using UniRx;

public class MUniRx : MonoBehaviour
{
    private Tetris mTetris;
    public void DoGravity()
    {
        var gravity = Observable.FromCoroutine(DropGravity)
            .Subscribe();
    }
    IEnumerator DropGravity()
    {
        mTetris.TryDropDown(false);
        yield return new WaitForSeconds(1);
        DoGravity();
    }

    public void DoTetPreview()
    {
        var tetPreview = Observable.FromCoroutine(DisplayTetPreview)
            .Subscribe();
    }

    private IEnumerator DisplayTetPreview()
    {

        while (mTetris.nextTets.Count < 3 || !mTetris.iSetUp)
        {
            yield return 0;
        }

        mTetris.mTet.SetUpPreviewTet(0, mTetris.nextTets[0]);
        mTetris.mTet.SetUpPreviewTet(1, mTetris.nextTets[1]);
        mTetris.mTet.SetUpPreviewTet(2, mTetris.nextTets[2]);
    }
    void Awake()
    {
        mTetris = transform.GetComponent<Tetris>();
    }
    void Start()
    {
        DoGravity();
    }
}
