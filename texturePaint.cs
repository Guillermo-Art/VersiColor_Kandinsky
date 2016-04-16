using UnityEngine;
using System.Collections;

public class texturePaint : MonoBehaviour {

	public GameObject node1, node2; //objeto a isntanciar

	private int order;
	private Color colLine, colIni, colEnd;

	void Start() {
		//comienza neutral
		colLine = new Color(42 / 255.0f, 44 / 255.0f, 41 / 255.0f);
		colIni = new Color (199 / 255.0f, 30 / 255.0f, 49 / 255.0f);
		colEnd = new Color (132 / 255.0f, 71 / 255.0f, 87 / 255.0f);
		setColors ();
		order = 1;
	}
	
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos.z = 0;
			Debug.Log ("Input: X:" + pos.x + ", Y:" + pos.y);

			switch(order){
			case 1:
				Instantiate (node1, pos, Quaternion.identity);
				order = 2;
				break;
			case 2:
				Instantiate (node2, pos, Quaternion.identity);
				order = 1;
				break;
			}
		}
		if(Input.GetKey(KeyCode.Z)){ //neutral
			colLine = new Color(42 / 255.0f, 44 / 255.0f, 41 / 255.0f);
			colIni = new Color (199 / 255.0f, 30 / 255.0f, 49 / 255.0f);
			colEnd = new Color (132 / 255.0f, 71 / 255.0f, 87 / 255.0f);
			setColors ();
		}
		if(Input.GetKey(KeyCode.X)){ //feliz
			colLine = new Color(252 / 255.0f, 211 / 255.0f, 0 / 255.0f);
			colIni = new Color (250 / 255.0f, 100 / 255.0f, 0 / 255.0f);
			colEnd = new Color (157 / 255.0f, 226 / 255.0f, 221 / 255.0f);
			setColors ();
		}
	}

	void setColors() {
		node1.GetComponent<SpriteRenderer> ().color = colIni;
		node2.GetComponent<SpriteRenderer> ().color = colEnd;
		node1.GetComponent<LineRenderer> ().SetColors (colLine, colLine);
	}

}
