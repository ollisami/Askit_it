using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SendQuestionToServer : MonoBehaviour {

	private string secretKey = "194329"; // Edit this value and make sure it's the same as the one stored on the server
	public string sendQuestionURL = "http://starsrent.fi/AskIt/sendQuestion.php?";

	public void SendQuestion (string question) {
		StartCoroutine(SendData(question));
	}

	IEnumerator SendData(string question) 
	{
		Debug.Log ("trying to post question with value: " + question);
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5Sum(question + secretKey);
		
		string post_url = sendQuestionURL + "&question=" + WWW.EscapeURL(question) + "&hash=" + hash;
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		yield return hs_post; // Wait until the download is done
		if (hs_post.error != null)
		{
			print("There was an error posting the high score: " + hs_post.error);
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
}
