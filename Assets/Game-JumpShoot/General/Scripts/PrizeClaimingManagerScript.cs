using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrizeClaimingManagerScript : MonoBehaviour {

	public TextMeshProUGUI prizeText;
	void Start () {
		prizeText.SetText(PlayerPrefs.GetString("prizeName"));
	}
}
