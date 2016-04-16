using UnityEngine;
using System.Collections;

public class nodeBehaviour : MonoBehaviour {

	LineRenderer myLineR;
	bool listener;
	public int order;

    private string handState;
    private GameObject pincel;
    public GameObject manager;
 
	void Start () {
        manager = GameObject.Find("BodyView");
		myLineR = gameObject.GetComponent<LineRenderer> ();
		myLineR.SetPosition (0, transform.position);
		myLineR.SetPosition (1, transform.position);
		listener = true;
	}
	
	void Update () {
        switch (order){
            case 1:
                if (listener)
                {
                    if(manager.GetComponent<BodySourceView>().flag == "Open")
                    {
                        pincel = GameObject.Find("stencil");
                        Vector3 pos = pincel.transform.position;
                        myLineR.SetPosition(1, pos);
                        listener = false;
                    }
                }
                break;
            case 2:
                break;
        }
	}
}
