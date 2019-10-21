using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using TMPro;


[RequireComponent(typeof(Button))]
public class FormSubmissionHandler : MonoBehaviour {
	/*
	 * This script you may need to modify according to your purpose
	*/
	[Header("This script you may need to modify according to your purpose")]
	[Header("Database Setting")]
	public string database;
	public string host;
	public string user;
	public string password;
	public string urlPath;

	[Header("Form Validation Setting")]
	public string nameRegex = "^[A-Za-z ]+$";	
	public string dobRegex = "^([0-9]){1,2}([0-9]){1,2}([0-9]){4}$";
	public string contactRegex = "^(\\+?6?01)[0-46-9]*[0-9]{7,8}$";
	public string emailRegex = "^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$";

	[Header("Form Setting")]
	public float messageBoxDelay = 2f;
	public string nextSceneString = "GameMenu";
	public InputField playerName;
	public InputField playerDOB;
	public InputField playerContact;
	public InputField playerEmail;
	public GameObject MessageBox;

	private Button startButton;
	void Awake(){
		startButton = GetComponent<Button> ();
		if (startButton) {
            startButton.onClick.AddListener (LoadScene);
        }
	}
	void LoadScene(){
		StartCoroutine(LoadSceneCoroutine());
	}

	IEnumerator LoadSceneCoroutine(){
		// Do form validation
		bool invalidInput = false;
		if(CheckInput(playerName.text, nameRegex, "Name is invalid") == false){
			invalidInput = true;
		}
		else if(CheckInput(playerDOB.text, dobRegex, "Birthday is invalid") == false){
			invalidInput = true;
		}
		else if(CheckInput(playerContact.text, contactRegex, "Contact is invalid") == false){
			invalidInput = true;
		}
		else if(CheckInput(playerEmail.text, emailRegex, "Email is invalid") == false){
			invalidInput = true;
		}

		if(invalidInput == false){
			yield return StartCoroutine(SaveDatabase());
			OpenMessageBox("Success!");
			yield return new WaitForSeconds(messageBoxDelay);
			// OpenMessageBox("Game Starting in 3...");
			// yield return new WaitForSeconds(1f);
			// OpenMessageBox("Game Starting in 2...");
			// yield return new WaitForSeconds(1f);
			// OpenMessageBox("Game Starting in 1...");
			// yield return new WaitForSeconds(1f);
			SceneManager.LoadScene(nextSceneString);
		}
		// startButton.interactable = true;
	}

	bool CheckInput(string input,string regex, string errorMessage){
		var match = Regex.Match(input, regex, RegexOptions.IgnoreCase);
		if (!match.Success){
			OpenMessageBox(errorMessage);
			return false;
		}
		return true;
	}

	void OpenMessageBox(string msg){
		MessageBox.SetActive(true);
		MessageBox.transform.Find("Panel").Find("Text").GetComponent<TextMeshProUGUI>().SetText(msg);
	}

	IEnumerator SaveDatabase(){
		WWWForm registerForm = new WWWForm();
		registerForm.AddField("database",database);
		registerForm.AddField("host", host);
		registerForm.AddField("user", user);
		registerForm.AddField("password", password);

		registerForm.AddField("name",playerName.text);
		registerForm.AddField("dob", playerDOB.text);
		registerForm.AddField("contact", playerContact.text);
		registerForm.AddField("email", playerEmail.text);
		
		WWW Reg = new WWW(urlPath, registerForm);
		yield return Reg;
		// Debug.Log(Reg.text);
	}
}
