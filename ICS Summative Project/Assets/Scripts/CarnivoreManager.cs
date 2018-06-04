using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is placed on every carnivore and acts as the logic system for the carnivores NN. This class gives the NN the input, by raycasting (which act as eyes), and also uses the output on the carnivore to set velocity. This class also handles 
/// giving the carnivore hunger and checking for the carnivores death. This class also checks for collsions with Herbivores, which result in herbivores losing health
/// </summary>
public class CarnivoreManager : MonoBehaviour {
	//refrences
	private NeuralNetwork net;
	private FoodManager foodManager;
	private EnvironmentManager envi;


	private float[] output; //the output from the NN
	private float[] input= new float[2]; //input []
	//input 0 prey x axis to go (1 is right and -1 is left)
	//input 1 prey y axis to go (1 is up and -1 is down)

	private int[] layers = new int[] {2, 10, 10, 2}; // //neurons: 2 input (described above) and 2 output (x vel y vel)

	private Vector2 startPos; //raycast start pos (need to translate Vector3 to Vector 2
	private float fitness; //the fitness rating, this value is purely visual, the important fitness value is stored in the NN and can be accessed with getNet().getFitness();
	public Rigidbody2D rb; //the rigidbody of this creature, this is the movement system

	public LayerMask carnMask; //layermask for raycast from herbivores

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
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.up, 10F,carnMask);
		if (hit.collider != null) { //if eye sees something
			if (hit.collider.gameObject.tag == "Herbivore") { 
				//see food
				input [1] = 1; 
			}
			 else {
				//see nothing
				input [1] = 0;

			}
		}
	}
	/// <summary>
	/// Shoots a raycast right to act as a eye focing East
	/// </summary>
	private void lookRight(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.right, 10F, carnMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Herbivore") {
				//see food
				input [0] = 1;
			} else {
				input [0] = 0;

			}
		}
	}
	/// <summary>
	/// Shoots a raycast left to act as a eye focing Wast
	/// </summary>
	private void lookLeft(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.left, 10F, carnMask);
		if (hit.collider != null) {
			if (hit.collider.gameObject.tag == "Herbivore") {
				//see food
				input [0] = -1;
			} else {
				input [0] = 0;

			}
		}
	}
	/// <summary>
	/// Shoots a raycast up to act as a eye focing North
	/// </summary>
	private void lookDown(){
		RaycastHit2D hit = Physics2D.Raycast (startPos, Vector2.down, 10F,carnMask);
		if (hit.collider != null) { //if eye sees something
			if (hit.collider.gameObject.tag == "Herbivore") { 
				//see food
				input [1] = -1; 
			}
			else {
				//see nothing
				input [1] = 0;

			}
		}
	}

	/// <summary>
	/// Called every frame
	/// </summary>
	void Update(){
		fitness = getNet ().GetFitness ();
		startPos = new Vector2 (this.transform.position.x, this.transform.position.y);

		getNet ().AddFitness (-0.35f*Time.deltaTime); //hunger
		//death checker
		if (getNet ().GetFitness () <= 0) {
			//dead
			if (this.gameObject != envi.bestCarnivore) {
				envi.carnivores.Remove (this.gameObject);
				Destroy (this.gameObject);
			} else {
				//this is the best creature
				this.getNet().SetFitness(10F);
			}
		}

		//call eye raycasts
		lookUp ();
		lookRight ();
		lookLeft ();
		lookDown ();

		output = net.FeedForward (input); //use feedforward to get output from input through NN
		rb.velocity = new Vector2 (output [0] * 10, output [1] * 10); //set Vel with output
	
	}
		
	/// <summary>
	/// checks for collsions
	/// </summary>
	/// <param name="col">Col.</param>
	void OnCollisionEnter2D(Collision2D col){
		//if the herbivore collides with herbivore
		if (col.gameObject.tag == "Herbivore") {
			//eat the food and delete the food object
			net.AddFitness (8f);
			col.gameObject.GetComponent<HerbivoreManager> ().getNet ().AddFitness (-4f);
		}
	}
}
