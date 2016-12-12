using UnityEngine;
using System.Collections;

public class Question {

	public int id;
	public string question;
	public int yes;
	public int no;

	public Question (int id, string question, int yes, int no) {
		this.id = id;
		this.question = question;
		this.yes = yes;
		this.no = no;
	}
}
