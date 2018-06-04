using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages spawning food throughout the environment. The class spawns food exponentially based on the current amount of food
/// </summary>
public class FoodManager : MonoBehaviour {

	//refrences
	public EnvironmentManager environment;
	public GameObject foodObjectParent;

	public List<GameObject> foods; //array of food objects
	public GameObject food;	//food prefab refrence
	private int numFood, maxNumFood=100; //tracks number of food and the max number of food (for spawning)

	/// <summary>
	/// spawn food in the environment ever tick
	/// </summary>
	public void spawnFood(){
		if (foods.Count == 0) { //if there is no food right now
			//spawn a food and add it to the foods list
			GameObject spawnedFood = GameObject.Instantiate (food);
			spawnedFood.transform.parent = foodObjectParent.transform;
			foods.Add (spawnedFood);

		} else {
			int numOfFoodSpawning = (int)Mathf.Round ((float)foods.Count); //num of food spawning = Round to an int(numFood/Eulers number) + 1
			//if(the food is not at carrying capacity)
			if (foods.Count < maxNumFood) {
				//spawn food in for loop given the amt of food spawning (above)
				for (int i = 0; i < numOfFoodSpawning; i++) {
					//spawn food
					GameObject spawnedFood = GameObject.Instantiate (food);
					spawnedFood.transform.parent = foodObjectParent.transform;

					//find a random parent for the food spawning
					GameObject foodParent = foods [UnityEngine.Random.Range (0, foods.Count - 1)];

					//set position of new food to parent + some random displacment 
					Vector3 position = foodParent.gameObject.transform.position;
					float randomDisplacment1 = UnityEngine.Random.Range (-2.5f, 2.5f);
					float randomDisplacment2 = UnityEngine.Random.Range (-2.5f, 2.5f);
					position += new Vector3 (randomDisplacment1, randomDisplacment2, 0);
					spawnedFood.gameObject.transform.position = position;
					foods.Add (spawnedFood); //add the food to the list
				}
			} else {
				//environment is at its max amount of food so dont spawn anything 
			}
		}

		//for loop for each food to check if they spawned outside the barriers
		for (int i = 0; i < foods.Count; i++) {
			//if a food spawned outside the barrier then delete it and remove it from the list
			if (foods [i].gameObject.transform.position.x < -8f || foods [i].gameObject.transform.position.x > 8f || foods [i].gameObject.transform.position.y > 4 || foods [i].gameObject.transform.position.y < -4) {
				GameObject f = foods [i];
				foods.Remove (f);
				GameObject.Destroy (f);
			}
		}
	}



}
