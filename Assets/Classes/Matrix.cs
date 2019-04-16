using System;
using System.Collections.Generic;
namespace MachineLearning.Classes
{
    public class Matrix
    {
        #region Variables
        private int rows, cols;
        private List<List<double>> values;
		Random random;
		#endregion

		#region Constructors
		public Matrix(int numRows, int numCols, bool isRandom, Random rand)
        {
			random = rand;
            values = new List<List<double>>();
            rows = numRows;
            cols = numCols;
            for (int r = 0; r < numRows; r++)
            {
                List<double> row = new List<double>();
                for (int c = 0; c < numCols; c++)
                {
                    double v = 0;
                    if (isRandom)
                    {
                        v = random.Next(0, 10000) / 10000.0;
                    }
                    row.Add(v);
                }
                values.Add(row);
            }
        }
        #endregion

        #region Getters
        public double getValue(int r, int c)
        {
            return values[r][c];
        }

        public int getRows() 
        {
            return rows;
        }

        public int getCols()
        {
            return cols;
        }
        #endregion

        #region Setters
        public void setValue(int r, int c, double v)
        {
            values[r][c] = v;
        }
        #endregion

        #region Functions
        public Matrix transpose()
        {
            Matrix newMatrix = new Matrix(cols, rows, false, random);

            for (int r = 0; r < rows;r++) {
                for (int c = 0; c < cols;c++) {
                    newMatrix.setValue(c, r, values[r][c]);
                }
            }

            return newMatrix;
        }

        public void dumpMatrix()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(values[i][j] + " ");
                }
                Console.WriteLine();
            }
        }
        #endregion
    }
}
