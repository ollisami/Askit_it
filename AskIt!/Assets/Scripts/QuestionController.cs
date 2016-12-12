using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionController : MonoBehaviour {

	public GetQuestionFromServer getQuestion;

	public GameObject bottomText;

	public Text yesButtonText;
	public Text noButtonText;

	public Text questionText;
	public Text countdownText;
	public Question currentQuestion;

	// SHOW QUESTION
	private bool showQuestion = false;
	public float Qcountdown;
	private float QcountdownStartTime = 10;

	// SHOW SCORE
	private bool showScore = false;
	private float scoreCountDown;
	private float scoreCountdownStart = 5;

	//SEND ANSWER
	private bool answerSent = false;

	void Awake() {
		DontDestroyOnLoad (this);
		GetQuestion ();
	}

	void FixedUpdate() {
		if (showQuestion) {
			if(Qcountdown > 0) {
				Qcountdown -= Time.deltaTime;
				countdownText.text = "" + Mathf.Round(Qcountdown);
			} else {
				ShowScore();
				showQuestion = false;
			}
		}

		if (showScore) {
			if(scoreCountDown > 0) {
				scoreCountDown -= Time.deltaTime;
				bottomText.GetComponent<SetBottomText>().setText( "Next question starts in: " + Mathf.Round(scoreCountDown));
			} else {
				GetQuestion();
				showScore = false;
			}
		}
	}


	public void GetQuestion() {
		Debug.Log("GetQuestion running");
			if (getQuestion != null) {
					getQuestion.GetQuestion();
			}

		}

	public void SetNewQueston(Question question) {
		Debug.Log("ReceiveQuestion running");
		if (questionText != null && question.question.Length > 3) {
			currentQuestion = question;
			showQuestion = true;
			answerSent = false;
			Qcountdown = QcountdownStartTime;
			questionText.text = question.question;
			Debug.Log ("current question: " + question.question);

			yesButtonText.text = "Yes";
			noButtonText.text = "No";

		} else {
			GetQuestion();
		}
	}

	public void ShowScore() {
		showScore = true;
		scoreCountDown = scoreCountdownStart;

		float total = currentQuestion.yes + currentQuestion.no;
		if (total <= 5) {
			total = FixScore();
		}
		yesButtonText.text = "" + Mathf.Round ((currentQuestion.yes / total) * 100) + "%";
		noButtonText.text = "" + Mathf.Round ((currentQuestion.no / total) * 100) + "%";
	}


	private float FixScore() {
		currentQuestion.yes = currentQuestion.yes + Random.Range (0, 100);
		currentQuestion.no = currentQuestion.no + Random.Range (0, 100);
		float total = currentQuestion.yes + currentQuestion.no;

		return total;
	}

	public void SendAnswer(string answer) {
		if (!answerSent && currentQuestion != null) {
				getQuestion.SendAnswer(currentQuestion.id, answer);
			if(answer.Equals("yes"))
				currentQuestion.yes++;
			if(answer.Equals("no"))
				currentQuestion.no++;

			bottomText.GetComponent<SetBottomText>().setText("Answered!");
			answerSent = true;
		}
	}
}
