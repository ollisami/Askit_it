using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopMenuScript : MonoBehaviour {
	public GameObject menu;
	private bool menuActive = false;

	public void ShowMenu() {
		if (menu != null) {
			menuActive = !menuActive;
			menu.SetActive(menuActive);
		}
	}
}
