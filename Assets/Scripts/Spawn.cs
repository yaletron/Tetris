using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Spawn : MonoBehaviour
{
	// spawn object to instantiate game elements
	// parent all
	private GameObject mPA;
	public GameObject camP;
	// camera parent
	private GameObject mCP;
	// game creator
	public GameObject tetris;
    private GameObject mGC;
	// canvas
	public GameObject canvas;
	private GameObject mC;



	private Spawn mS;
	private Transform mT;

	

	void Start()
	{
		mS = this;
		mT = transform;

		// parent all
		mPA = new GameObject();
		mPA.transform.name = "ParentAll";

		// camera parent
		mCP = Instantiate(camP, Vector3.zero, Quaternion.identity);
		mCP.transform.name = "CamP";
		mCP.transform.SetParent(mPA.transform);
		// canvas
		mC = Instantiate(canvas, Vector3.zero, Quaternion.identity);
		mC.transform.name = "Canvas";
		mC.transform.SetParent(mPA.transform);
		
		mC.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
		mC.GetComponent<Canvas>().worldCamera = mCP.transform.GetChild(0).GetComponent<Camera>();
		// game creator
		mGC = Instantiate(tetris, Vector3.zero, Quaternion.identity);
		mGC.transform.name = "Tetris";
		mGC.transform.parent = mPA.transform;
		mGC.GetComponent<Tetris>().SetUp(mC);
	}
}
