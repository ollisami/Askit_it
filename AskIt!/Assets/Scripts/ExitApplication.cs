using UnityEngine;
using System.Collections;

public class ExitApplication : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) 
			Application.Quit(); 
	}
}
