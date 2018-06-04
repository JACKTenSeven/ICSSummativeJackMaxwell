using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the environment in which the creatures are trying to survive in. This class controls the timer tick rate of the environment (the tick rate controls when new food spawns and when creatures breed). 
/// This class controls the natural selection in the environment which includes all creature spawning which includes initial spawning and breeding on every environment tick, natural selection also encomposses
/// finding the top creature to be used in breeding.
/// </summary>
public class EnvironmentManager : MonoBehaviour {

	//refrences
	public FoodManager foodManager;

	//simulation speed, can speed up evolution
	public float simSpeed = 1;

	//timer for compute environment tick rate
	private float environmentTimer;
	private float environmentTimerMax=10;
	private int tick;
	private int generation;

	public GameObject herbPrefab; 	//prefab refrence
	public GameObject carnPrefab; 	//prefab refrence

	public List<GameObject> herbivores; //array of food objects
	public List<GameObject> carnivores; //array of food objects

	public GameObject bestHerbivore, bestCarnivore;

	/// <summary>
	/// called first when the program first runs
	/// </summary>
	void Awake(){
		//create 25 herbivores initially
		for (int i = 0; i < 25; i++) {
			createFirstHerbivores ();
		}
		for (int i = 0; i < 25; i++) {
			createFirstCarnivores ();
		}
	}
	//called everyframe
	void Update(){
		doTimer (); //compute the timer
		UnityEngine.Time.timeScale = simSpeed;
	}
	/// <summary>
	/// Gets the simulation speed.
	/// </summary>
	/// <returns>The simulation speed.</returns>
	public float getSimulationSpeed(){
		return simSpeed;
	}
	/// <summary>
	/// Gets the generation number.
	/// </summary>
	/// <returns>The generation number.</returns>
	public int getGenerationNum(){
		return generation;
	}
	/// <summary>
	/// Gets the best herbivore.
	/// </summary>
	/// <returns>The best herbivore.</returns>
	public GameObject getBestHerbivore(){
		return bestHerbivore;
	}
	/// <summary>
	/// Gets the best carnivore.
	/// </summary>
	/// <returns>The best carnivore.</returns>
	public GameObject getBestCarnivore(){
		return bestCarnivore;
	}
	/// <summary>
	/// handle the timer, increment and check for tick
	/// </summary>
	private void doTimer(){
		//if timer is less than the max then count up
		if (environmentTimer < environmentTimerMax) {
			environmentTimer += 1 * Time.deltaTime * simSpeed; //increment timer 1 every second * simulation speed

			//if the timer is reached the max then do things
		} else if(environmentTimer>=environmentTimerMax) {
			foodManager.spawnFood (); //spawn food 
			environmentTick (); //call environment tick, this is called every 10 seconds / simspeed
			environmentTimer = 0; //reset timer
		}
	}

	/// <summary>
	/// do things ever environment tick, this serves as natural selection because by the time the environment gets to 5 ticks the weaker gene creatures will have died
	/// </summary>
	private void environmentTick(){
		//if the environment has counted to 50 / simspeed
		if (tick >= 5) {
			//kill all except best genome
			killLowerHerbivores ();
			killLowerCarnivores ();

			//breed new creatures with the one best genome
			breedHerbivores();
			breedCarnivores ();


			int amtParentHerb = herbivores.Count; //amount of parent creatures
			//if there are no herbivore spawn a new 25 creatures
			if (amtParentHerb == 0) {
				for (int i = 0; i < 25; i++) {
					createFirstHerbivores ();

				}
					
			}
			//if there are no herbivore spawn a new 25 creatures
			int amtParentCarn = carnivores.Count; //amount of parent creatures
			if(amtParentCarn==0){
				for (int i = 0; i < 25; i++) {
					createFirstCarnivores ();

				}
			}
			//teleport all to start
			newGenSpawn();

			tick = 0; //reset tick (if the tick reached 5)
			generation+=1;
		}
		tick += 1; //increment tick (if the tick is not 5 yet)
	}
	/// <summary>
	/// Called every new generation (5 ticks (50 seconds on speed x1)) to reset the position of all creatures so they can change their inputs therefore helping evolve them faster because their input will change more
	/// </summary>
	private void newGenSpawn(){
		//teleport all creatures to start positions

		//teleport all herbivores to start
		for (int i = 0; i < herbivores.Count; i++) {
			GameObject h = herbivores [i];
			h.transform.position = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-4f, 4f), 0); //give the creature a random position within the environment (So herbivores and carnivores dont spawn in same pos and herbivore is eaten instantly)
		}
		//teleport all carnivores to start
		for (int i = 0; i < carnivores.Count; i++) {
			GameObject h = carnivores [i];
			h.transform.position = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-4f, 4f), 0); //give the creature a random position within the environment (So herbivores and carnivores dont spawn in same pos and herbivore is eaten instantly)
		}
	}



	//----------------------------HERBIVORES----------------------------//


	/// <summary>
	/// Kills all herbivores except for the top fitness herbivore
	/// </summary>
	private void killLowerHerbivores(){
		//find the top fitness herbivore
		float bestFitness=0;
		GameObject bestCreature = null;
		for (int i = 0; i < herbivores.Count; i++) {
			if (bestFitness < herbivores [i].GetComponent<HerbivoreManager> ().getNet ().GetFitness ()) {
				bestFitness = herbivores [i].GetComponent<HerbivoreManager> ().getNet ().GetFitness ();
				bestCreature = herbivores [i];
			}
		}
		//set the variable
		bestHerbivore = bestCreature;

		//kill all herbivores except the top herbivore
		for (int i = 0; i < herbivores.Count; i++) {
			if (herbivores[i]!=bestHerbivore){
				GameObject h = herbivores [i];
				herbivores.Remove (h);
				Destroy (h);
			}
		}

	}
	/// <summary>
	/// The best creature spawns 25 new herbivores
	/// </summary>
	private void breedHerbivores(){
		//if there is a best carnivore than breed 25 new herbivores
		if (bestHerbivore != null) {
			Vector3 pos = Vector3.zero;
			for (int i = 0; i < 25; i++) {
				breedNewHerbivore (pos, bestHerbivore.GetComponent<HerbivoreManager> ()); //breed new herbivore given the best herbivore
			}
			//else create 25 random herbivores
		} else {
			for (int i = 0; i < 25; i++) {
				createFirstHerbivores ();

			}
		}
	}


	/// <summary>
	/// creates a initial herbivore
	/// </summary>
	private void createFirstHerbivores(){
		GameObject h = GameObject.Instantiate (herbPrefab);
		h.GetComponent<HerbivoreManager> ().normalInit ();
		h.transform.position = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-4f, 4f), 0);
		herbivores.Add (h);
	}

	/// <summary>
	/// creates a herbivore from a parent
	/// </summary>
	private void breedNewHerbivore(Vector3 pos, HerbivoreManager parent){
		GameObject h = GameObject.Instantiate (herbPrefab); //instantiate herbivore
		h.GetComponent<HerbivoreManager> ().copyInit(parent.getNet()); //create NN
		h.transform.position = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-4f, 4f), 0);
		herbivores.Add (h); //add herbivore to list
	}




	//----------------------------CARNIVORES----------------------------//


	/// <summary>
	/// Kill all the lower carnivores except the top fitness carnivore 
	/// </summary>
	private void killLowerCarnivores(){
		//find the best carnivore
		float bestFitness=0; 
		GameObject bestCreature=null;
		for (int i = 0; i < carnivores.Count; i++) {
			if (bestFitness < carnivores [i].GetComponent<CarnivoreManager> ().getNet ().GetFitness ()) {
				bestFitness = carnivores [i].GetComponent<CarnivoreManager> ().getNet ().GetFitness ();
				bestCreature = carnivores [i];
			}
		}
		//set the best carnivore
		bestCarnivore = bestCreature;
		//kill all carnivores that are not the best carnivore
		for (int i = 0; i < carnivores.Count; i++) {
			if (carnivores[i]!=bestCarnivore){
				GameObject h = carnivores [i];
				carnivores.Remove (h);
				Destroy (h);
			}
		}

	}
	/// <summary>
	/// The best creature spawns 25 new carnivores
	/// </summary>
	private void breedCarnivores(){
		//if there is a best carnivore than breed 25 new carnivores
		if (bestCarnivore != null) {
			Vector3 pos = Vector3.zero;
			for (int i = 0; i < 25; i++) {
				breedNewCarnivore (pos, bestCarnivore.GetComponent<CarnivoreManager> ()); //breed new creature given the best carnivores
			}
			//else create 25 random carnivores
		} else {
			for (int i = 0; i < 25; i++) {
				createFirstCarnivores ();
			}
		}

	}

	/// <summary>
	/// creates a initial carnivore
	/// </summary>
	private void createFirstCarnivores(){
		GameObject h = GameObject.Instantiate (carnPrefab); //instantiate carnivore
		h.GetComponent<CarnivoreManager> ().normalInit (); //create NN 
		h.transform.position = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-4f, 4f), 0);
		carnivores.Add (h);
	}

	/// <summary>
	/// creates a herbivore from a parent
	/// </summary>
	private void breedNewCarnivore(Vector3 pos, CarnivoreManager parent){
		GameObject h = GameObject.Instantiate (carnPrefab); //instantiate carnivore
		h.GetComponent<CarnivoreManager> ().copyInit(parent.getNet()); //create NN based on parent connections with mutation
		h.transform.position = new Vector3 (UnityEngine.Random.Range (-8f, 8f), UnityEngine.Random.Range (-4f, 4f), 0);
		carnivores.Add (h); //add herbivore to list
	}






	/// <summary>
	/// Loads the saved creatures into the environment by spawning breeded versions of the best carnivore
	/// </summary>
	/// <param name="bestCarn">The best carnivore</param>
	/// <param name="bestHerb">The best herbivore</param>
	public void loadBestCreatures(GameObject bestCarn, GameObject bestHerb){
		Vector3 pos = Vector3.zero;
		for (int i = 0; i < 25; i++) {
			breedNewHerbivore (pos, bestHerb.GetComponent<HerbivoreManager>());
			breedNewCarnivore (pos, bestCarn.GetComponent<CarnivoreManager>());
		}
	}
	
}
