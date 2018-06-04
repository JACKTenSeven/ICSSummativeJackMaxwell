using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is inherited by all herbivores. It handles creating their NN, feeding the input and using the output to control the velocity. This class also controls the herbivores hunger and checks for death. This class also checks for collsion with food and then eats it.
/// </summary>
public class HerbivoreManager : MonoBehaviour {
	//refrences
	private NeuralNetwork net;
	private FoodManager foodManager;
	private EnvironmentManager envi;


	private float[] output; //the output from the NN
	private float[] input= new float[4]; //input []
		//input 0 food x to go
		//input 1 food y to go
		//input 2 enemy x to go 
		//input 3 enemy y to go

	private int[] layers = new int[] {4, 10, 10, 2 }; // //neurons: 4 input and 3 output: x vel y vel and angle

	private Vector2 startPos; //raycast start pos (need to translate Vector3 to Vector 2
	public float fitness; //the fitness rating, this value is purely visual, the important fitness value is stored in the NN and can be accessed with getNet().getFitness();
	public Rigidbody2D rb; //the rigidbody of this creature, this is the movement system

	public LayerMask herbMask; //layermask for raycast from herbivores

	/// <summary>
	/// called first when the program starts
	/// </summary>
	void Awake(){
		foodManager = GameObject.FindGameObjectWithTag ("FoodManager").GetComponent<FoodManager> (); //get foodManger refrence
		envi = GameObject.FindGameObjectWithTag ("EnvironmentManager").GetComponent<EnvironmentManager> (); //get environmentManger refrence

	}
	//create the NN with a fitness of 10, the fitness serves as health
	void Start(){
		getNet ().SetFitness (10F);

	}

	//NN normal init, creates with random neuron connections
	public void normalInit(){
		net = new NeuralNetwork (layers);
		net.Mutate ();
	}
	//NN copy init, creates neuron connections based on a parent NN (from breeding)
	public void copyInit(NeuralNetwork n){
			net = new NeuralNetwork (n);
			net.Mutate ();
	}
	//return the NN
	public NeuralNetwork getNet(){
		return net;
	}
	//mutate the NN
	public void mutateNet(){
		net.Mutate ();
	}

	/// <summary>
	/// Shoots a raycast up to act as a eye focing North
	/// </summary>
	private void lookUp(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.up, 10F,herbMask);
		if (hit.collider != null) { //if eye sees something
			if (hit.collider.gameObject.tag == "Food") { 
				//see food
				input [1] = 1; 
			} else if (hit.collider.gameObject.tag == "Carnivore") {
				//see predator
				input [3] = -1; //*-1 difference from food input to help NN make connection to travel in opposite direction
			} else {
				//see nothing
				input [1] = 0;
				input [3] = 0;
			}
		}
	}
	/// <summary>
	/// Shoots a raycast right to act as a eye focing East
	/// </summary>
	private void lookRight(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.right, 10F, herbMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Food") {
				//see food
				input [0] = 1;
			} else if (hit.collider.gameObject.tag == "Carnivore") {
				//see predator
				input [2] = -1; //*-1 difference from food input to help NN make connection to travel in opposite direction
			} else {
				input [0] = 0;
				input [2] = 0;
			}
		}
	}
	/// <summary>
	/// Shoots a raycast down to act as a eye focing South
	/// </summary>
	private void lookDown(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.down, 10F, herbMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Food") {
				//see food
				input [1] = -1;
			} else if (hit.collider.gameObject.tag == "Carnivore") {
				//see predator
				input [3] = 1; //*-1 difference from food input to help NN make connection to travel in opposite direction
			} else {
				input [1] = 0;
				input [3] = 0;
			}
		}
	}
	/// <summary>
	/// Shoots a raycast left to act as a eye focing East
	/// </summary>
	private void lookLeft(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.left, 10F, herbMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Food") {
				//see food
				input [0] = -1;
			} else if (hit.collider.gameObject.tag == "Carnivore") {
				//see predator
				input [2] = 1; //*-1 difference from food input to help NN make connection to travel in opposite direction
			} else {
				input [0] = 0;
				input [2] = 0;
			}
		}
	}

	/// <summary>
	/// Called every frame
	/// </summary>
	void Update(){
		fitness = getNet ().GetFitness ();
		startPos = new Vector2 (this.transform.position.x, this.transform.position.y);

		getNet ().AddFitness (-0.3f*Time.deltaTime); //hunger
		//death checker
		if (getNet ().GetFitness () <= 0) {
			//dead
			if (this.gameObject != envi.bestHerbivore) {
				envi.herbivores.Remove (this.gameObject);
				Destroy (this.gameObject);
			} else {
				//this is the best creature so dont let it starve because of 1 mistake
				this.getNet().SetFitness(10F);
			}
		}

		//call eye raycasts
		lookUp ();
		lookDown ();
		lookRight ();
		lookLeft ();


		output = net.FeedForward (input); //use feedforward to get output from input through NN
		rb.velocity = new Vector2 (output [0] * 10, output [1] * 10); //set Vel with output
	}


	/// <summary>
	/// checks for collsions
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerEnter2D(Collider2D col){
		//if the herbivore collides with food
		if (col.gameObject.tag == "Food") {
			//eat the food and delete the food object
			net.AddFitness (6f);
			foodManager.foods.Remove (col.gameObject);
			Destroy (col.gameObject);
		}
	}
}




