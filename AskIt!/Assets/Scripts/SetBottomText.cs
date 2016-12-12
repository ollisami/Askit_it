using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetBottomText : MonoBehaviour {
	public Text UItext;

	private float minimum = 0.0F;
	private float maximum = 1.0F;
	private float duration = 2.0F;
	private float startTime;

	private bool isFading = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (isFading) {
			float t = (Time.time - startTime) / duration;
			UItext.color = new Color(1f,1f,1f,Mathf.SmoothStep(maximum, minimum, t)); 
			if(t >= 1) {
				isFading = false;
			}
		}
	}

	public void setText(string text) {
		UItext.text = text;

		UItext.color = new Color(1f,1f,1f,0F); 
		startTime = Time.time;
		isFading = true;;
	}
}
