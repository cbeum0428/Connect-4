using System;
using System.Collections.Generic;

namespace MachineLearning.Classes
{
    public class NeuralNetwork
    {
        #region Variables
        private int topologySize;
        private List<int> topology;
        private List<Layer> layers;
        private List<Matrix> weightMatricies;
        private List<Matrix> gradientMatricies;
        private List<double> currentInput;
        private List<double> currentTarget;
        private double error;
        private List<double> errors;
        private List<double> historicalErrors;
		private Random random;
		private double fitnessScore;
        #endregion

        #region Constructors
        public NeuralNetwork(List<int> top, Random r)
        {
            //initialize all Lists
            topology = top;
            layers = new List<Layer>();
            weightMatricies = new List<Matrix>();
            gradientMatricies = new List<Matrix>();
            currentInput = new List<double>();
            currentTarget = new List<double>();
            historicalErrors = new List<double>();
			random = r;

            topologySize = top.Count;
            for (int i = 0; i < topologySize;i++) {
                Layer l = new Layer(top[i], random);
                layers.Add(l);
            }

            for (int i = 0; i < topologySize - 1; i++)
            {
                /*
                 * Weight matrix where rows = number of neurons 
                 * on the left and cols = number of neurons on right.
                 */
                Matrix m = new Matrix(top[i], top[i+1], true, random);
                weightMatricies.Add(m);

            }

			fitnessScore = 0;
        }
        #endregion

        #region Getters
        public List<double> getOutputValues()
        {
            List<double> output = new List<double>();
            int outputIndexLayer = layers.Count - 1;
            Matrix outputValues = layers[outputIndexLayer].matrixifyDerivedValues();
            for (int c = 0; c < outputValues.getCols(); c++)
            {
                output.Add(outputValues.getValue(0, c));
            }

            return output;
        }

        public Matrix getNeuronMatrix(int i)
        {
            return layers[i].matrixifyValues();
        }

        public Matrix getActivatedNeuronMatrix(int i)
        {
            return layers[i].matrixifyActivatedValues();
        }

        public Matrix getDerivedNeuronMatrix(int i)
        {
            return layers[i].matrixifyDerivedValues();
        }

        public Matrix getWeightMatrix(int i)
        {
            return weightMatricies[i];
        }

        public double getTotalError()
        {
            return error;
        }

        public List<double> getErrors()
        {
            return errors;
        }

		public double getFitnessScore()
		{
			return fitnessScore;
		}
        #endregion

        #region Setters
        public void setCurrentInput(List<double> input)
        {
            currentInput = input;
            for (int i = 0; i < input.Count; i++)
            {
                //Console.WriteLine("Coming from here");
                layers[0].setValue(i, input[i]);
            }
        }

        public void setNeuronValue(int indexLayer, int indexNeuron, double value)
        {
            //Console.WriteLine("Coming from here");
            layers[indexLayer].setValue(indexNeuron, value);
        }

        public void setCurrentTarget(List<double> t)
        {
            currentTarget = t;
        }

        public void setErrors()
        {
            if (currentTarget.Count == 0)
            {
                Console.WriteLine("No target can't set errors");
            }

            int outputLayerIndex = layers.Count - 1;
            if (currentTarget.Count != layers[outputLayerIndex].getNeurons().Count)
            {
                Console.WriteLine("Target not the same size as output");
            }

            error = 0;
            errors = new List<double>();
            List<Neuron> outputNeurons = layers[outputLayerIndex].getNeurons();
            for (int i = 0; i < currentTarget.Count;i++)
            {
                double tempErr = outputNeurons[i].getActivatedValue() - currentTarget[i];
                errors.Add(tempErr);
                error += tempErr;
            }

            historicalErrors.Add(error);
        }

		public void setFitnessScore(double v)
		{
			fitnessScore = v;
		}

		public void setWeightMatricies(List<Matrix> newWeightMatricies)
		{
			weightMatricies = newWeightMatricies;
		}
		#endregion

		#region Functions
		public void printToConsole() 
        {
            for (int i = 0; i < layers.Count;i++)
            {
                if (i == 0)
                {
                    Matrix m = layers[i].matrixifyValues();
                    m.dumpMatrix();
                }
                else
                {
                    Matrix m = layers[i].matrixifyActivatedValues();
                    m.dumpMatrix();
                }
            }
        }

        public void printInputToConsole()
        {
            for (int i = 0; i < currentInput.Count;i++) {
                Console.Write(currentInput[i] + "\t");
            }
            Console.WriteLine();
        }

        public void printTargetToConsole()
        {
            for (int i = 0; i < currentTarget.Count; i++)
            {
                Console.Write(currentTarget[i] + "\t");
            }
            Console.WriteLine();
        }

        public void printOutputToConsole()
        {
            int outputIndexLayer = layers.Count - 1;
            Matrix outputValues = layers[outputIndexLayer].matrixifyDerivedValues();
            for (int c = 0; c < outputValues.getCols();c++)
            {
                Console.Write(outputValues.getValue(0, c) + "\t");
            }
            Console.WriteLine();
        }

        public void feedForward()
        {
            for (int i = 0; i < (layers.Count - 1);i++)
            {
                Matrix a = getNeuronMatrix(i);

                if (i != 0)
                {
                    a = getActivatedNeuronMatrix(i);
                }

                Matrix b = getWeightMatrix(i);
                MultiplyMatrix mm = new MultiplyMatrix(a, b, random);
                Matrix c = mm.execute();

                for (int c_index = 0; c_index < c.getCols(); c_index++)
                {
                    //Console.WriteLine("Setting neuron at " + c_index);
                    setNeuronValue(i + 1, c_index, (c.getValue(0, c_index)));
                }
            }
        }

        public void backPropogation()
        {
            List<Matrix> newWeights = new List<Matrix>();
            Matrix gradient;
            int outputLayerIndex = layers.Count - 1;
            Matrix derivedValuesYtoZ = layers[outputLayerIndex].matrixifyDerivedValues();
            Matrix gradientsYtoZ = new Matrix(1, layers[outputLayerIndex].getNeurons().Count, false, random);
            for (int i = 0; i < errors.Count;i++)
            {
                double d = derivedValuesYtoZ.getValue(0, i);
                double e = errors[i];
                double g = d * e;
                gradientsYtoZ.setValue(0, i, g);
            }

            int lastHiddenLayerIndex = outputLayerIndex - 1;
            Layer lastHiddenLayer = layers[lastHiddenLayerIndex];
            Matrix weightsOutputToHidden = weightMatricies[lastHiddenLayerIndex];
            Matrix deltaOutputToHidden = (new MultiplyMatrix(gradientsYtoZ.transpose(), lastHiddenLayer.matrixifyActivatedValues(), random)).execute().transpose();
            Matrix newWeightsOutputToHidden = new Matrix(deltaOutputToHidden.getRows(), deltaOutputToHidden.getCols(), false, random);

            for (int r = 0; r < deltaOutputToHidden.getRows();r++)
            {
                for (int c = 0; c < deltaOutputToHidden.getCols(); c++)
                {
                    
                    double originalWeight = weightsOutputToHidden.getValue(r, c);
                    double deltaWeight = deltaOutputToHidden.getValue(r, c);
                    newWeightsOutputToHidden.setValue(r, c, originalWeight - deltaWeight);
                }
            }

            newWeights.Add(newWeightsOutputToHidden);

            gradient = new Matrix(gradientsYtoZ.getRows(), gradientsYtoZ.getCols(), false, random);
            for (int r = 0; r < gradientsYtoZ.getRows(); r++)
            {
                for (int c = 0; c < gradientsYtoZ.getCols(); c++)
                {
                    gradient.setValue(r, c, gradientsYtoZ.getValue(r, c));
                }
            }

            //Loop backwards and adjust weights
            for (int i = lastHiddenLayerIndex;i > 0;i--)
            {
                Layer l = layers[i];
                Matrix derivedHidden = l.matrixifyDerivedValues();
                Matrix activatedHidden = l.matrixifyActivatedValues();
                Matrix derivedGradients = new Matrix(1, l.getNeurons().Count, false, random);
                Matrix weightMatrix = weightMatricies[i];
                Matrix originalWeight = weightMatricies[i - 1];

                for (int r = 0; r < weightMatrix.getRows();r++)
                {
                    double sum = 0;
                    for (int c = 0; c < weightMatrix.getCols(); c++)
                    {
                        double p = gradient.getValue(0, r) * weightMatrix.getValue(r, c);
                        sum += p;
                    }
                    double g = sum * activatedHidden.getValue(0, r);
                    derivedGradients.setValue(0, r, g);
                }

                Matrix leftNeurons = (i - 1) == 0 ? layers[0].matrixifyValues() : layers[i - 1].matrixifyActivatedValues();
                Matrix deltaWeights = (new MultiplyMatrix(derivedGradients.transpose(), leftNeurons, random)).execute().transpose();
                Matrix newWeightsHidden= new Matrix(deltaWeights.getRows(), deltaWeights.getCols(), false, random);

                for (int r = 0; r < newWeightsHidden.getRows();r++)
                {
                    for (int c = 0; c < newWeightsHidden.getCols(); c++)
                    {
                        double w = originalWeight.getValue(r, c);
                        double d = deltaWeights.getValue(r, c);
                        double n = w - d;
                        newWeightsHidden.setValue(r, c, n);
                    }
                }

                gradient = new Matrix(derivedGradients.getRows(), derivedGradients.getCols(), false, random);
                for (int r = 0; r < derivedGradients.getRows(); r++)
                {
                    for (int c = 0; c < derivedGradients.getCols(); c++)
                    {
                        gradient.setValue(r, c, derivedGradients.getValue(r, c));
                    }
                }

                newWeights.Add(newWeightsHidden);
            }
            newWeights.Reverse();
            weightMatricies = newWeights;
        }

		public void randomizeWeights()
		{
			weightMatricies = new List<Matrix>();
			for (int i = 0; i < topologySize - 1; i++)
			{
				/*
                 * Weight matrix where rows = number of neurons 
                 * on the left and cols = number of neurons on right.
                 */
				Matrix m = new Matrix(topology[i], topology[i + 1], true, random);
				weightMatricies.Add(m);
			}
		}

		public NeuralNetwork mutate(double mr)
		{
			NeuralNetwork newnn = new NeuralNetwork(topology, random);
			List<Matrix> newWeights = new List<Matrix>();
			for (int i = 0;i < weightMatricies.Count;i++)
			{
				Matrix parentMatrix = weightMatricies[i];
				Matrix newMatrix = new Matrix(parentMatrix.getRows(), parentMatrix.getCols(), false, random);

				for (int r = 0; r < parentMatrix.getRows(); r++)
				{
					for (int c = 0; c < parentMatrix.getCols(); c++)
					{
						double rand = (double)random.Next(-1, 1) * ((double)random.Next(0, 100) / 100.0);
						double newW = parentMatrix.getValue(r, c) * rand * mr;
						newMatrix.setValue(r, c, newW);
					}
				}

				newWeights.Add(newMatrix);
			}

			newnn.setWeightMatricies(newWeights);

			return newnn;
		}
        #endregion
    }
}
