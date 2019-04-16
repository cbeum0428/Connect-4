using System;
using System.Collections.Generic;
namespace MachineLearning.Classes
{
    public class Layer
    {
        #region Variables
        private int size;
        private List<Neuron> neurons;
		Random random;
        #endregion

        #region Constructors
        public Layer(int s, Random r)
        {
			random = r;
            neurons = new List<Neuron>();
            size = s;
            for (int i = 0; i < size; i++)
            {
                //Make a blank Neuron and add to list.
                Neuron n = new Neuron(0.00);
                neurons.Add(n);
            }
        }
        #endregion

        #region Getters
        public List<Neuron> getNeurons() 
        {
            return neurons;
        }
        #endregion

        #region Setters
        public void setValue(int i, double v) 
        {
            //Console.WriteLine(v);
            neurons[i].setValue(v);
        }
        #endregion

        #region Functions
        public Matrix matrixifyValues()
        {
            Matrix m = new Matrix(1, neurons.Count, false, random);
            for (int i = 0; i < neurons.Count;i++) 
            {
                m.setValue(0, i, neurons[i].getValue());
            }

            return m;
        }

        public Matrix matrixifyActivatedValues()
        {
            Matrix m = new Matrix(1, neurons.Count, false, random);
            for (int i = 0; i < neurons.Count; i++)
            {
                m.setValue(0, i, neurons[i].getActivatedValue());
            }

            return m;
        }

        public Matrix matrixifyDerivedValues()
        {
            Matrix m = new Matrix(1, neurons.Count, false, random);
            for (int i = 0; i < neurons.Count; i++)
            {
                m.setValue(0, i, neurons[i].getDerivedValue());
            }

            return m;
        }
        #endregion
    }
}
