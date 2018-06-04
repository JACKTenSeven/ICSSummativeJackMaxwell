using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles the UI display
/// </summary>
public class UIManager : MonoBehaviour {

	//refrence
	public EnvironmentManager enviManager;

	//UI text
	public Text generationText;
	private Text topHerbivoreFitness, topCarnivoreFitness;

	//update the text every frame
	void Update(){
		generationText.text = "Generation: " + enviManager.getGenerationNum();

	}
}
