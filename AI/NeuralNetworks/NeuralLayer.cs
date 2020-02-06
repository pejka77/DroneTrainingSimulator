using System;

public class NeuralLayer
{
    private static Random randomizer = new Random();

    public delegate double ActivationFunction(double xValue);

    public ActivationFunction NeuronActivationFunction = MathHelper.SigmoidFunction;

    public uint NeuronCount
    {
        get;
        private set;
    }

    public uint OutputCount
    {
        get;
        private set;
    }

    public double[,] Weights
    {
        get;
        private set;
    }

    public NeuralLayer(uint nodeCount, uint outputCount)
    {
        this.NeuronCount = nodeCount;
        this.OutputCount = outputCount;

        Weights = new double[nodeCount + 1, outputCount];
    }

    public void SetWeights(double[] weights)
    {

        if (weights.Length != this.Weights.Length)
            throw new ArgumentException("Input weights do not match layer weight count.");

        int k = 0;
        for (int i = 0; i < this.Weights.GetLength(0); i++)
            for (int j = 0; j < this.Weights.GetLength(1); j++)
                this.Weights[i, j] = weights[k++];
    }

    public double[] ProcessInputs(double[] inputs)
    {
        if (inputs.Length != NeuronCount)
            throw new ArgumentException("Given xValues do not match layer input count.");

        double[] sums = new double[OutputCount];
        double[] biasedInputs = new double[NeuronCount + 1];
        inputs.CopyTo(biasedInputs, 0);
        biasedInputs[inputs.Length] = 1.0;

        for (int j = 0; j < this.Weights.GetLength(1); j++)
            for (int i = 0; i < this.Weights.GetLength(0); i++)
                sums[j] += biasedInputs[i] * Weights[i, j];

        if (NeuronActivationFunction != null)
        {
            for (int i = 0; i < sums.Length; i++)
                sums[i] = NeuronActivationFunction(sums[i]);
        }

        return sums;
    }

    public NeuralLayer DeepCopy()
    {
        double[,] copiedWeights = new double[this.Weights.GetLength(0), this.Weights.GetLength(1)];

        for (int x = 0; x < this.Weights.GetLength(0); x++)
            for (int y = 0; y < this.Weights.GetLength(1); y++)
                copiedWeights[x, y] = this.Weights[x, y];

        NeuralLayer newLayer = new NeuralLayer(this.NeuronCount, this.OutputCount);
        newLayer.Weights = copiedWeights;
        newLayer.NeuronActivationFunction = this.NeuronActivationFunction;

        return newLayer;
    }

    public void SetRandomWeights(double minValue, double maxValue)
    {
        double range = Math.Abs(minValue - maxValue);
        for (int i = 0; i < Weights.GetLength(0); i++)
            for (int j = 0; j < Weights.GetLength(1); j++)
                Weights[i, j] = minValue + (randomizer.NextDouble() * range);
    }

    public override string ToString()
    {
        string output = "";

        for (int x = 0; x < Weights.GetLength(0); x++)
        {
            for (int y = 0; y < Weights.GetLength(1); y++)
                output += "[" + x + "," + y + "]: " + Weights[x, y];

            output += "\n";
        }

        return output;
    }
}
