using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Genotype : IComparable<Genotype>, IEnumerable<float>
{
    private static System.Random randomizer = new System.Random();

    public float Evaluation
    {
        get;
        set;
    }

    public float Fitness
    {
        get;
        set;
    }

    private float[] parameters;

    public int ParameterCount
    {
        get
        {
            if (parameters == null) return 0;
            return parameters.Length;
        }
    }

    public float this[int index]
    {
        get { return parameters[index]; }
        set { parameters[index] = value; }
    }

    public Genotype(float[] parameters)
    {
        this.parameters = parameters;
        Fitness = 0;
    }

    public int CompareTo(Genotype other)
    {
        return other.Fitness.CompareTo(this.Fitness);
    }

    public IEnumerator<float> GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }

    public void SetRandomParameters(float minValue, float maxValue)
    {
        if (minValue > maxValue) throw new ArgumentException("Minimum value may not exceed maximum value.");

        float range = maxValue - minValue;
        for (int i = 0; i < parameters.Length; i++)
            parameters[i] = (float)((randomizer.NextDouble() * range) + minValue);
    }

    public float[] GetParameterCopy()
    {
        float[] copy = new float[ParameterCount];
        for (int i = 0; i < ParameterCount; i++)
            copy[i] = parameters[i];

        return copy;
    }

    public void SaveToFile(string filePath)
    {
        StringBuilder builder = new StringBuilder();
        foreach (float param in parameters)
            builder.Append(param.ToString()).Append(";");

        builder.Remove(builder.Length - 1, 1);

        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log(filePath);
            File.WriteAllText(filePath, builder.ToString());
        }
    }

    public static Genotype GenerateRandom(uint parameterCount, float minValue, float maxValue)
    {
        if (parameterCount == 0) return new Genotype(new float[0]);

        Genotype randomGenotype = new Genotype(new float[parameterCount]);
        randomGenotype.SetRandomParameters(minValue, maxValue);

        return randomGenotype;
    }

    public static Genotype LoadFromFile(string filePath)
    {
        string data = File.ReadAllText(filePath);

        List<float> parameters = new List<float>();
        string[] paramStrings = data.Split(';');

        foreach (string parameter in paramStrings)
        {
            float parsed;
            if (!float.TryParse(parameter, out parsed)) throw new ArgumentException("The file at given file path does not contain a valid genotype serialisation.");
            parameters.Add(parsed);
        }

        return new Genotype(parameters.ToArray());
    }
}
