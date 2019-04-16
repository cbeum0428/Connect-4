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
	public int epoch = 1;
    bool isP1Turn = true;
    bool newGame = true;
    int player1Index = 0;
    DateTime lastMoveMade;
	double mr = 3;

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
            //Debug.Log("Init row" + r);
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
        if (newGame || totalMoves == 48) {
			totalMoves = 0;
			isP1Turn = true;
			enabled = false;
			if (player1Index >= popsize)
			{
				epoch++;
				Breed();
				player1Index = 0;
			} else
			{
				//Debug.Log("P1: " + player1Index);
				Debug.Log(player1Index);
				p1 = networks[player1Index];
				p2 = networks[player1Index + 1];
				player1Index += 2;
				newGame = false;
			}
			enabled = true;
		}
        //Player 1 is black

        DateTime currentTime = DateTime.Now;
        TimeSpan t = currentTime - lastMoveMade;

		if (CheckForBlackWins())
		{
			Debug.Log("Black wins in" + (totalMoves/2)+1 + " moves!");
			double fs = p1.getFitnessScore() + (49 - totalMoves/2);
			p1.setFitnessScore(fs);
			ResetBoardState();
		}
		else if (CheckForRedWins())
		{
			Debug.Log("Red wins in" + totalMoves/2 + " moves!");
			double fs = p2.getFitnessScore() + (49 - totalMoves / 2);
			p2.setFitnessScore(fs);
			ResetBoardState();
		}

		if (isP1Turn && t.TotalSeconds > 1 && totalMoves < 49)
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
                    //Debug.Log(boardState[0][maxIndex]);
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
        else if (!isP1Turn && t.TotalSeconds > 1 && totalMoves < 49)
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
                    //Debug.Log(boardState[0][maxIndex]);
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
                placementVector = new Vector3(-4.672f, 3.93f, -1);
                break;
            case 2:
                placementVector = new Vector3(-3.07f, 3.93f, -1);
                break;
            case 3:
                placementVector = new Vector3(-1.55f, 3.93f, -1);
                break;
            case 4:
                placementVector = new Vector3(-0.019f, 3.93f, -1);
                break;
            case 5:
                placementVector = new Vector3(1.54f, 3.93f, -1);
                break;
            case 6:
                placementVector = new Vector3(3.05f, 3.93f, -1);
                break;
            default:
                placementVector = new Vector3(4.62f, 3.93f, -1);
                break;
        }
        Instantiate(peicePrefab, placementVector, Quaternion.identity);
    }

    void UpdateBoard(char ch, int placementIndex) {
        //Debug.Log("Updating Board");
        int r = 0;
        while (r < 6 && boardState[r][placementIndex] == 'E') {
            r++;
        }
        boardState[r-1][placementIndex] = ch;
    }

	void ResetBoardState()
	{
		for (int r = 0; r < 6; r++)
		{
			for (int c = 0; c < 7; c++)
			{
				boardState[r][c] = 'E';
			}
		}

		List<GameObject> pieces = new List<GameObject>(GameObject.FindGameObjectsWithTag("piece"));
		foreach(GameObject g in pieces)
		{
			Destroy(g);
		}
		totalMoves = 0;
		newGame = true;
	}

	bool CheckForBlackWins()
	{
		//Check for Horizontal Wins
		int colSeq = 0;
		for (int r = 0; r < 6;r++)
		{
			for (int c = 0; c < 7;c++)
			{
				if (boardState[r][c] == 'B')
				{
					colSeq++;
				} else if (boardState[r][c] == 'R' || boardState[r][c] == 'E')
				{
					colSeq = 0;
				}

				if (colSeq == 4)
				{
					return true;
				}
			}
		}

		//Check for Vertical Wins
		int rowSeq = 0;
		for (int c = 0; c < 7; c++)
		{
			for (int r = 0; r < 6; r++)
			{
				if (boardState[r][c] == 'B')
				{
					rowSeq++;
				}
				else if (boardState[r][c] == 'R' || boardState[r][c] == 'E')
				{
					rowSeq = 0;
				}

				if (rowSeq == 4)
				{
					return true;
				}
			}
		}

		//Check For Diagonal Wins

		return false;
	}

	bool CheckForRedWins()
	{
		//Check for Horizontal Wins
		int colSeq = 0;
		for (int r = 0; r < 6; r++)
		{
			for (int c = 0; c < 7; c++)
			{
				if (boardState[r][c] == 'R')
				{
					colSeq++;
				}
				else if (boardState[r][c] == 'B' || boardState[r][c] == 'E')
				{
					colSeq = 0;
				}

				if (colSeq == 4)
				{
					return true;
				}
			}
		}

		//Check for Vertical Wins
		int rowSeq = 0;
		for (int c = 0; c < 7; c++)
		{
			for (int r = 0; r < 6; r++)
			{
				if (boardState[r][c] == 'R')
				{
					rowSeq++;
				}
				else if (boardState[r][c] == 'B' || boardState[r][c] == 'E')
				{
					rowSeq = 0;
				}

				if (rowSeq == 4)
				{
					return true;
				}
			}
		}

		return false;
	}
	
	void Breed()
	{
		BubbleSort();
		//Debug.Log("Finished sorting");
		int ind = (int)(popsize * 0.25);
		int count = (int)(popsize * 0.75);
		networks.RemoveRange(ind, count);
		for (int i = 0; i < ind; i++)
		{
			networks.Add(networks[i].mutate(mr));
			networks.Add(networks[i].mutate(mr));
			networks.Add(networks[i].mutate(mr));
			mr *= 0.1;
		}

		for (int i = 0; i < networks.Count; i++)
		{
			networks[i].setFitnessScore(0);
		}
		//Debug.Log("New networks count: " + networks.Count);
	}

	void BubbleSort()
	{
		int n = popsize - 1;
		for (int i = 0;i < n-1;i++)
		{
			Debug.Log(i);
			for (int j = 0;j < n-1-i-1;j++)
			{
				if (networks[j].getFitnessScore() > networks[j+1].getFitnessScore())
				{
					NeuralNetwork t = networks[j+1];
					networks[j + 1] = networks[j];
					networks[j] = t;
				}
			}
		}
	}
}
