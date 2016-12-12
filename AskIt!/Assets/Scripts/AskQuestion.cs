using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AskQuestion : MonoBehaviour {

	public SendQuestionToServer sendQuestion;
	private string questionText;

	public GameObject bottomText;

	public void SetQuestionText(Text text) {
		questionText = text.text;
	}

	public void SendQuestion() {
		if (questionText.Length < 5) {
			bottomText.GetComponent<SetBottomText>().setText("Question is too short! (min 5 letters)");
			return;
		}

		sendQuestion.SendQuestion (questionText);

	}
}
