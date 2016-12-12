using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetQuestionFromServer : MonoBehaviour {

	private string secretKey = "194329"; // Edit this value and make sure it's the same as the one stored on the server
	public string addScoreURL = "http://starsrent.fi/AskIt/addscore.php?"; //be sure to add a ? to your url
	public string answerYesURL = "http://starsrent.fi/AskIt/yes.php?";
	public string answerNoURL = "http://starsrent.fi/AskIt/no.php?";
	public string getLastQuestionURL = "http://starsrent.fi/AskIt/getLastQuestion.php";
	public string getQuestionWithIDURL = "http://starsrent.fi/AskIt/getIDQuestion.php?";
	
	public GameObject bottomText;

	private List<int> usedQuestions = new List<int>();

	void Start() {
		loadQuestionList ();
		PlayerPrefs.DeleteAll();
	}

	public void GetQuestion() {
		StartCoroutine(GetData());
	}

	public void GetQuestionWithID(int id) {
		StartCoroutine(GetDataWithID(id));
	}

	public void SendAnswer(int id, string answer) {
		if(answer.Equals("yes"))
			StartCoroutine(PostAnswerYes(id));
		if(answer.Equals("no"))
			StartCoroutine(PostAnswerNo(id));
	}



	IEnumerator PostAnswerYes(int id) 
	{
		//Debug.Log ("trying to post answer with values" + id);
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5Sum(id + secretKey);
		
		string post_url = answerYesURL + "&id=" + id + "&hash=" + hash;
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done
		if (hs_post.error != null)
		{
			bottomText.GetComponent<SetBottomText>().setText("There was an error, try again later: " + hs_post.error);
		}
	}
	IEnumerator PostAnswerNo(int id) 
	{
		//Debug.Log ("trying to post answer with values" + id);
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5Sum(id + secretKey);
		
		string post_url = answerNoURL + "&id=" + id + "&hash=" + hash;
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done
		if (hs_post.error != null)
		{
			bottomText.GetComponent<SetBottomText>().setText("There was an error, try again later: " + hs_post.error);
		}
	}

	IEnumerator GetData()
	{
		WWW hs_get = new WWW(getLastQuestionURL);
		yield return hs_get;
		
		if (hs_get.error != null) {
			bottomText.GetComponent<SetBottomText>().setText("There was an error, try again later: " + hs_get.error);
		} else {
			string serverText = hs_get.text;
			string[] strings = ExplodeMainString (serverText, '\n');
			
			foreach (string itemString in strings) {
				string[] specs = ExplodeMainString (itemString, '\t');
				
				if (specs.Length == 4) {
					int questid = IntParseFast (specs [0]);
					//string name [1]

					int yes = IntParseFast (specs [2]);
					int no = IntParseFast (specs [3]);

					if (isNewQuestion(questid)) {
						createQuestion (questid, specs[1],yes,no);
						//Debug.Log("New question downloaded!");
					} else {
						Debug.Log(isNewQuestion(questid));
						GetQuestionWithID(questid);
					}
					yield break;
				}
			}
		}
	}

	IEnumerator GetDataWithID(int id)
	{
		string hash = Md5Sum(id + secretKey);
		string post_url = getQuestionWithIDURL + "&id=" + id + "&hash=" + hash;
		WWW hs_get = new WWW(post_url);

		yield return hs_get;
		
		if (hs_get.error != null) {
			bottomText.GetComponent<SetBottomText>().setText("There was an error, try again later: " + hs_get.error);
		} else {
			string serverText = hs_get.text;
			string[] strings = ExplodeMainString (serverText, '\n');
			
			foreach (string itemString in strings) {
				string[] specs = ExplodeMainString (itemString, '\t');

				if (specs.Length == 4) {

					int questid = IntParseFast (specs [0]);
					//string name [1]
					
					int yes = IntParseFast (specs [2]);
					int no = IntParseFast (specs [3]);

					if(isNewQuestion(questid)) {
						createQuestion (questid, specs[1],yes,no);
						//Debug.Log("New question downloaded!");

					} else {
						id--;
						if(id < 0) {
							usedQuestions.Sort();
							id = Random.Range(0,usedQuestions.Count -1);
						}
						GetQuestionWithID(id);
					}
				}
			}
			yield break;
		  }
	}
	
	public  string Md5Sum(string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
	
	
	
	
	private string[] ExplodeMainString(string main, char splitChar)
	{
		return main.Split (splitChar);
	}
	
	
	
	public static int IntParseFast(string value)
	{
		int result = 0;
		for (int i = 0; i < value.Length; i++)
		{
			char letter = value[i];
			result = 10 * result + (letter - 48);
		}
		return result;
	}

	private bool isNewQuestion(int id) {

			foreach (int i in usedQuestions) {
				if (i == id) {
					return false;
				}
			}
			return true;
	}

	public void createQuestion(int id, string question, int yes, int no) {
		Question quest = new Question (id, question, yes, no);
		if (usedQuestions.Count >= 20) {
			usedQuestions.Insert (Random.Range (0, usedQuestions.Count - 1), quest.id);
		} else {
			usedQuestions.Add (quest.id);
		}
		this.gameObject.GetComponent<QuestionController> ().SetNewQueston (quest);
		saveQuestionList ();
		StopCoroutine("GetData");
		StopCoroutine("GetDataWithID");
	}

	private void loadQuestionList() {
		int count = PlayerPrefs.GetInt("Count" , 0);
		
		for (int i = 0; i <= count; i++) {
			usedQuestions.Add(PlayerPrefs.GetInt("Question" + i, 0));
		}
		
	}
	
	private void saveQuestionList() {
		int count = 0;
		foreach (int i in usedQuestions) {
			PlayerPrefs.SetInt("Question" + count, i);
			count++;
		}
		PlayerPrefs.SetInt("Count", count);
		
	}

}
