using System;
using System.Collections.Generic;
using MachineLearning.Classes;
using UnityEngine;

public class GameScript : MonoBehaviour {
    
    public List<List<char>> boardState;
    public List<NeuralNetwork> networks;

    NeuralNetwork p1, p2;
    System.Random random = new System.Random();
    int popsize = 100;
    bool isP1Turn = true;
    bool newGame = true;
    int player1Index = 0;
    DateTime lastMoveMade;

    int totalMoves = 0;

    public GameObject redPeice;
    public GameObject blackPeice;

	// Use this for initialization
    void Start () {
        //Initialize Population
        networks = new List<NeuralNetwork>();
        boardState = new List<List<char>>();
        for (int i = 0; i < popsize; i++) {
            List<int> topology = new List<int>();
            topology.Add(42);
            int hLayeWidth = random.Next(0, 5);
            for (int j = 0; j < hLayeWidth; j++) {
                int hLayerNeurons = random.Next(1, 30);
                topology.Add(hLayerNeurons);
            }
            topology.Add(7);
            NeuralNetwork neuralNetwork = new NeuralNetwork(topology, random);
            networks.Add(neuralNetwork);
        }

        //Build Board State
        for (int r = 0; r < 6; r++) {
            Debug.Log("Init row" + r);
            List<char> row = new List<char>();
            for (int c = 0; c < 7; c++) {
                row.Add('E');
            }
            boardState.Add(row);
        }

        lastMoveMade = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (newGame) {
            player1Index += 2;
            p1 = networks[player1Index];
            p2 = networks[player1Index+1];
            newGame = false;
        }
        //Player 1 is black

        DateTime currentTime = DateTime.Now;
        TimeSpan t = currentTime - lastMoveMade;
        if (isP1Turn && t.TotalSeconds > 1)
        {
            enabled = false;
            List<double> inputs = new List<double>();
            for (int r = 0; r < 6;r++) {
                for (int c = 0; c < 7;c++) {
                    if (boardState[r][c] == 'B') {
                        inputs.Add(-1);
                    } else if (boardState[r][c] == 'R') {
                        inputs.Add(1);
                    } else {
                        inputs.Add(0);
                    }
                }
            }
            p1.setCurrentInput(inputs);
            p1.feedForward();
            List<double> outputs = p1.getOutputValues();

            double maxVal = outputs[0];
            int maxIndex = 0;
            bool maxOpenFound = false;
            while (!maxOpenFound)
            {
                for (int i = 1; i < outputs.Count; i++)
                {
                    if (outputs[i] > maxVal)
                    {
                        maxVal = outputs[i];
                        maxIndex = i;
                    }
                }
                if (boardState[0][maxIndex] == 'E')
                {
                    Debug.Log(boardState[0][maxIndex]);
                    maxOpenFound = true;
                }
                else
                {
                    outputs[maxIndex] = -1000;
                    maxVal = outputs[0];
                    maxIndex = 0;
                }
            }

            enabled = true;
            PlacePeice(blackPeice, maxIndex + 1);
            UpdateBoard('B', maxIndex);
            lastMoveMade = DateTime.Now;
            isP1Turn = false;
            totalMoves++;
        }
        else if (!isP1Turn && t.TotalSeconds > 1)
        {
            enabled = false;
            List<double> inputs = new List<double>();
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    if (boardState[r][c] == 'R')
                    {
                        inputs.Add(-1);
                    }
                    else if (boardState[r][c] == 'B')
                    {
                        inputs.Add(1);
                    }
                    else
                    {
                        inputs.Add(0);
                    }
                }
            }
            p2.setCurrentInput(inputs);
            p2.feedForward();
            List<double> outputs = p2.getOutputValues();

            double maxVal = outputs[0];
            int maxIndex = 0;
            bool maxOpenFound = false;
            while (!maxOpenFound)
            {
                for (int i = 1; i < outputs.Count; i++)
                {
                    if (outputs[i] > maxVal)
                    {
                        maxVal = outputs[i];
                        maxIndex = i;
                    }
                }
                if (boardState[0][maxIndex] == 'E') {
                    Debug.Log(boardState[0][maxIndex]);
                    maxOpenFound = true;
                } else {
                    outputs[maxIndex] = -1000;
                    maxVal = outputs[0];
                    maxIndex = 0;
                }
            }

            enabled = true;
            PlacePeice(redPeice, maxIndex + 1);
            UpdateBoard('R', maxIndex);
            lastMoveMade = DateTime.Now;
            isP1Turn = true;
            totalMoves++;
        }
    }

    void PlacePeice(GameObject peicePrefab, int columnIndex) {
        Vector3 placementVector;
        switch(columnIndex) {
            case 1:
                placementVector = new Vector3(-4.67f, 3.89f, -1);
                break;
            case 2:
                placementVector = new Vector3(-3.08f, 3.89f, -1);
                break;
            case 3:
                placementVector = new Vector3(-1.56f, 3.89f, -1);
                break;
            case 4:
                placementVector = new Vector3(0, 0, -1);
                break;
            case 5:
                placementVector = new Vector3(1.49f, 3.89f, -1);
                break;
            case 6:
                placementVector = new Vector3(3.08f, 3.89f, -1);
                break;
            default:
                placementVector = new Vector3(4.56f, 3.89f, -1);
                break;
        }
        Instantiate(peicePrefab, placementVector, Quaternion.identity);
    }

    void UpdateBoard(char ch, int placementIndex) {
        Debug.Log("Updating Board");
        int r = 0;
        while (r < 6 && boardState[r][placementIndex] == 'E') {
            r++;
        }
        boardState[r-1][placementIndex] = ch;
        /*
        for (r = 0; r < 6; r++) {
            string t = "";
            for (int c = 0; c < 7; c++) {
                t += boardState[r][c] + " ";
            }
            Debug.Log(t);
        }
        Debug.Log("---------------------------");
        */
    }
}
