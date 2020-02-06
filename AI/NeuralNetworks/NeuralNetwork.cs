using System;

public class NeuralNetwork
{
    public NeuralLayer[] Layers
    {
        get;
        private set;
    }

    public uint[] Topology
    {
        get;
        private set;
    }

    public int WeightCount
    {
        get;
        private set;
    }

    public NeuralNetwork(params uint[] topology)
    {
        this.Topology = topology;

        WeightCount = 0;
        for (int i = 0; i < topology.Length - 1; i++)
            WeightCount += (int)((topology[i] + 1) * topology[i + 1]);

        Layers = new NeuralLayer[topology.Length - 1];
        for (int i = 0; i < Layers.Length; i++)
            Layers[i] = new NeuralLayer(topology[i], topology[i + 1]);
    }

    public double[] ProcessInputs(double[] inputs)
    {
        if (inputs.Length != Layers[0].NeuronCount)
            throw new ArgumentException("Given inputs do not match network input amount.");

        double[] outputs = inputs;
        foreach (NeuralLayer layer in Layers)
            outputs = layer.ProcessInputs(outputs);

        return outputs;

    }

    public void SetRandomWeights(double minValue, double maxValue)
    {
        if (Layers != null)
        {
            foreach (NeuralLayer layer in Layers)
                layer.SetRandomWeights(minValue, maxValue);
        }
    }

    public NeuralNetwork GetTopologyCopy()
    {
        NeuralNetwork copy = new NeuralNetwork(this.Topology);
        for (int i = 0; i < Layers.Length; i++)
            copy.Layers[i].NeuronActivationFunction = this.Layers[i].NeuronActivationFunction;

        return copy;
    }

    public NeuralNetwork DeepCopy()
    {
        NeuralNetwork newNet = new NeuralNetwork(this.Topology);
        for (int i = 0; i < this.Layers.Length; i++)
            newNet.Layers[i] = this.Layers[i].DeepCopy();

        return newNet;
    }

    public override string ToString()
    {
        string output = "";

        for (int i = 0; i < Layers.Length; i++)
            output += "Layer " + i + ":\n" + Layers[i].ToString();

        return output;
    }
}
