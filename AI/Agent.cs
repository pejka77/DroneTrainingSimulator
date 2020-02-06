using System;
using System.Collections.Generic;

public class Agent : IComparable<Agent>
{
    public Genotype Genotype
    {
        get;
        private set;
    }

    public NeuralNetwork FNN
    {
        get;
        private set;
    }

    private bool isAlive = false;

    public bool IsAlive
    {
        get { return isAlive; }
        private set
        {
            if (isAlive != value)
            {
                isAlive = value;

                if (!isAlive && AgentDied != null)
                    AgentDied(this);
            }
        }
    }

    public event Action<Agent> AgentDied;

    public Agent(Genotype genotype, NeuralLayer.ActivationFunction defaultActivation, params uint[] topology)
    {
        IsAlive = false;
        this.Genotype = genotype;
        FNN = new NeuralNetwork(topology);
        foreach (NeuralLayer layer in FNN.Layers)
            layer.NeuronActivationFunction = defaultActivation;

        if (FNN.WeightCount != genotype.ParameterCount)
            throw new ArgumentException("The given genotype's parameter count must match the neural network topology's weight count.");

        IEnumerator<float> parameters = genotype.GetEnumerator();
        foreach (NeuralLayer layer in FNN.Layers)
        {
            for (int i = 0; i < layer.Weights.GetLength(0); i++)
            {
                for (int j = 0; j < layer.Weights.GetLength(1); j++) 
                {
                    layer.Weights[i,j] = parameters.Current;
                    parameters.MoveNext();
                }
            }
        }
    }

    public void Reset()
    {
        Genotype.Evaluation = 0;
        Genotype.Fitness = 0;
        IsAlive = true;
    }

    public void Kill()
    {
        IsAlive = false;
    }

    public int CompareTo(Agent other)
    {
        return this.Genotype.CompareTo(other.Genotype);
    }
}

