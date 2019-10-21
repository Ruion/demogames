using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrizeTextManager : MonoBehaviour {

	public TextMeshProUGUI prizeText;
	private string prize;
	
	void Start () {
		prize = PlayerPrefs.GetString("prizeName");
		prizeText.text = prize;
	}
}
