using System;

namespace MachineLearning.Classes
{
    public class MultiplyMatrix
    {
        #region Variables
        private Matrix a;
        private Matrix b;
        private Matrix c;
		Random random;
        #endregion

        #region Constructors
        public MultiplyMatrix(Matrix aa, Matrix bb, Random r)
        {
			random = r;
            a = aa;
            b = bb;
            c = new Matrix(a.getRows(), b.getCols(), false, random);
            if (a.getCols() != b.getRows())
            {
                Console.WriteLine("Matrix a cols don't match Matrix b rows.");
                //Exit?
            }
        }
        #endregion

        #region Getters
        #endregion

        #region Setters
        #endregion

        #region Functions
        public Matrix execute()
        {
            for (int i = 0; i < a.getRows(); i++)
            {
                for (int j = 0; j < b.getCols(); j++)
                {
                    for (int k = 0; k < b.getRows();k++)
                    {
                        double p = a.getValue(i, k) * b.getValue(k, j);
                        double newValue = c.getValue(i, j) + p;
                        c.setValue(i, j, newValue);
                    }
                }
            }

            return c;
        }
        #endregion
    }
}
