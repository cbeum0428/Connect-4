namespace MachineLearning.Classes
{
    public class Neuron
    {
        #region Variables
        private double value;
        private double activatedValue;
        private double derivedValue;
        #endregion

        #region Constructors
        public Neuron(double v)
        {
            value = v;
            activate();
            derive();
        }
        #endregion

        #region Getters
        public double getValue() 
        {
            return value;
        }

        public double getActivatedValue() 
        {
            return activatedValue;
        }

        public double getDerivedValue() 
        {
            return derivedValue;
        }
        #endregion

        #region Setters
        public void setValue(double v) 
        {
            value = v;
            activate();
            derive();
        }
        #endregion

        #region Functions
        public void activate() 
        {
            double absValue = value;
            if (absValue < 0) {
                absValue *= -1;
            }
            activatedValue = value / (1 + absValue);
        }

        public void derive() 
        {
            derivedValue = activatedValue * (1 - activatedValue);
        }
        #endregion
    }
}
