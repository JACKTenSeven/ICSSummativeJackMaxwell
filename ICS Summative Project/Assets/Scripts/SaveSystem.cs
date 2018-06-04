using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

/// <summary>
/// This class saves the top creatures so they can be loaded in after training the NN
/// </summary>
public class SaveSystem : MonoBehaviour {

	//refrence
	private EnvironmentManager envi;

	//set the refrence
	private void Awake(){
		envi = GameObject.FindGameObjectWithTag ("EnvironmentManager").GetComponent<EnvironmentManager>();
	}

	/// <summary>
	/// Binary serialization save file that saves the weights of the NN
	/// </summary>
	public void Save ()
	{
		if (envi.getBestHerbivore () != null && envi.getBestCarnivore () != null) {
			
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");
			PlayerData data = new PlayerData ();

			//get the weights of the best herbivore
			float[][][] bestHerbWeights = envi.getBestHerbivore ().GetComponent<HerbivoreManager> ().getNet ().getWeights ();
			int w = 0; //an int that is used to go through all of the float [] in data
			//for loop through a 3D array of the best Herb weight
			for (int i = 0; i < bestHerbWeights.Length; i++) {
				for (int j = 0; j < bestHerbWeights [i].Length; j++) {
					for (int k = 0; k < bestHerbWeights [i] [j].Length; k++) {
					
						data.herbivoreWeights [w] = bestHerbWeights [i] [j] [k]; //set the data herb weights to the bestHerbWeights
						w++; //increment
					}
				}
			}

			float[][][] bestCarnWeights = envi.getBestCarnivore ().GetComponent<CarnivoreManager> ().getNet ().getWeights ();
			int s = 0; //an int that is used to go through all of the float [] in data
			for (int i = 0; i < bestCarnWeights.Length; i++) {
				for (int j = 0; j < bestCarnWeights [i].Length; j++) {
					for (int k = 0; k < bestCarnWeights [i] [j].Length; k++) {

					
						data.carnivoreWeights [s] = bestCarnWeights [i] [j] [k]; //set the data carn weights to the bestCarnWeights
						s++; //increment
					}
				}
			}

			//serialize and close the file stream
			bf.Serialize (file, data);
			file.Close ();

		} else {
			//there is no best carn or herb so you cant save
		}
	}

	/// <summary>
	/// Load the best NN weights 
	/// </summary>
	public void Load()
	{
		//if a save file has already been created then load the data
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			//open file stream, desterialize the file then close the file stream
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			//load the weights back into the best Herb weights in 3D array with a for loop
			float [][][] bestHerbWeights = envi.getBestHerbivore().GetComponent<HerbivoreManager>().getNet().getWeights();
			int w = 0;
			for (int i = 0; i < bestHerbWeights.Length; i++)
			{
				for (int j = 0; j < bestHerbWeights[i].Length; j++)
				{
					for (int k = 0; k < bestHerbWeights[i][j].Length; k++)
					{
						//set the weights of the best herb to what is saved 
						envi.getBestHerbivore ().GetComponent<HerbivoreManager> ().getNet ().getWeights () [i] [j] [k] = data.herbivoreWeights [w];
					
						w++;
					}
				}
			}
			//load the weights back into the best carn weights in 3D array with a for loop
			float [][][] bestCarnWeights = envi.getBestCarnivore().GetComponent<CarnivoreManager>().getNet().getWeights();
			int s = 0;
			for (int i = 0; i < bestCarnWeights.Length; i++)
			{
				for (int j = 0; j < bestCarnWeights[i].Length; j++)
				{
					for (int k = 0; k < bestCarnWeights[i][j].Length; k++)
					{
						//set the weights of the best herb to what is saved 
						envi.getBestCarnivore ().GetComponent<CarnivoreManager> ().getNet ().getWeights () [i] [j] [k] = data.carnivoreWeights [s];
						s++;
					}
				}
			}



			envi.loadBestCreatures (envi.getBestCarnivore (), envi.getBestHerbivore ());
		}
	}

	[Serializable]
	class PlayerData
	{
		public float [] herbivoreWeights = new float[160]; //the weights of each neuron in a herbivore NN
		public float [] carnivoreWeights = new float[140]; //the weights of each neuron in a carnivore NN
	}
}
