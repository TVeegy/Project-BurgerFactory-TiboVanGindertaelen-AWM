using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class does something.
/// </summary>
public class Manager : MonoBehaviour {

    public GameObject boomerPrefab;
    public GameObject hex;

    private bool flag_startTrainingRound = true;

    private int populationSize = 50;
    private int generationNumber = 0;
    private int[] layers = new int[] { 1, 10, 10, 1 }; //1 input and 1 output
    private List<Boomerang> coronasList = null;
    private List<NeuralNetwork> nets = new List<NeuralNetwork>();

    /// <summary>
    /// Adds two integers and returns the result.
    /// </summary>
    void Timer()
    {
        flag_startTrainingRound = true;
    }

    /// <summary>
    /// Adds two integers and returns the result.
    /// </summary>
	  void Update ()
    {
        // Start a new training round (possibly the first one)
        if (flag_startTrainingRound) HandleTrainingRound();

        // Handle a left mouse button click
        if (Input.GetMouseButtonUp(0)) HandleLeftMouseButtonDown();
    }

    /// <summary>
    /// Adds two integers and returns the result.
    /// </summary>
    private void HandleLeftMouseButtonDown()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hex.transform.position = mousePosition;
    }

    /// <summary>
    /// Adds two integers and returns the result.
    /// </summary>
    private void HandleTrainingRound()
    {
        // Initialize the coronas their neural networks or regenerate them
        if (generationNumber == 0) InitCoronasNeuralNetworks();
        else SortAndRegenNeuralNets();

        // Reset the variables to this operation
        flag_startTrainingRound = false;
        generationNumber++;

        // Invoke the operations to this operation
        CreateCoronaBodies();
        Invoke("Timer", 15f);        
    }

    /// <summary>
    /// Sorts the neural networks, keeping the better half and regenerating the lesser half.
    /// </summary>
    private void SortAndRegenNeuralNets()
    {
        nets.Sort();
        for (int i = 0; i < (populationSize / 2); i++)
        {
            // Creating an altered version. (Altered as in altered connection values)
            nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
            nets[i].Mutate();

            // Keeping half of the neural networks, the good half.
            nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);

            // Reset the fitness
            nets[i].SetFitness(0f);
        }
    }

    /// <summary>
    /// Creates the corona game objects that will be paired with their corresponding neural networks.
    /// </summary>
    private void CreateCoronaBodies()
    {
        // Destroy already existing bodies
        if (coronasList != null)
        {
            for (int i = 0; i < coronasList.Count; i++)
            {
                GameObject.Destroy(coronasList[i].gameObject);
            }

        }

        // Recreate the kist and it's contents
        coronasList = new List<Boomerang>();

        for (int i = 0; i < populationSize; i++)
        {
            Boomerang boomer = ((GameObject)Instantiate(boomerPrefab, new Vector3(UnityEngine.Random.Range(-10f,10f), UnityEngine.Random.Range(-10f, 10f), 0),boomerPrefab.transform.rotation)).GetComponent<Boomerang>();
            boomer.Init(nets[i],hex.transform);
            coronasList.Add(boomer);
        }

    }

    /// <summary>
    /// Initializes the set of neural networks belonging to the corona-bodies.
    /// This including checking whether the population is a correct input.
    /// </summary>
    private void InitCoronasNeuralNetworks()
    {
        // Making sure the population is even
        if (populationSize % 2 != 0)
        {
            populationSize++;
        }

        // Create a unique neural network per body and add it onto the nets collection
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}