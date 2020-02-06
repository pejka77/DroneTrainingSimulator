using System;
using System.Collections.Generic;

public class GeneticAlgorithm
{

    public const float DefInitParamMin = -1.0f;
    public const float DefInitParamMax = 1.0f;
    public const float DefCrossSwapProb = 0.6f;
    public const float DefMutationProb = 0.3f;
    public const float DefMutationAmount = 2.0f;
    public const float DefMutationPerc = 1.0f;

    public delegate void InitialisationOperator(IEnumerable<Genotype> initialPopulation);
    public delegate void EvaluationOperator(IEnumerable<Genotype> currentPopulation);
    public delegate void FitnessCalculation(IEnumerable<Genotype> currentPopulation);
    public delegate List<Genotype> SelectionOperator(List<Genotype> currentPopulation);
    public delegate List<Genotype> RecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize);
    public delegate void MutationOperator(List<Genotype> newPopulation);
    public delegate bool CheckTerminationCriterion(IEnumerable<Genotype> currentPopulation);

    public InitialisationOperator InitialisePopulation = DefaultPopulationInitialisation;
    public EvaluationOperator Evaluation = AsyncEvaluation;
    public FitnessCalculation FitnessCalculationMethod = DefaultFitnessCalculation;
    public SelectionOperator Selection = DefaultSelectionOperator;
    public RecombinationOperator Recombination = DefaultRecombinationOperator;
    public MutationOperator Mutation = DefaultMutationOperator;
    public CheckTerminationCriterion TerminationCriterion = null;

    private static Random randomizer = new Random();

    private List<Genotype> currentPopulation;

    public uint PopulationSize
    {
        get;
        private set;
    }

    public uint GenerationCount
    {
        get;
        private set;
    }

    public bool SortPopulation
    {
        get;
        private set;
    }

    public bool Running
    {
        get;
        private set;
    }

    public event System.Action<GeneticAlgorithm> AlgorithmTerminated;

    public event System.Action<IEnumerable<Genotype>> FitnessCalculationFinished;

    public GeneticAlgorithm(uint genotypeParamCount, uint populationSize)
    {
        this.PopulationSize = populationSize;

        currentPopulation = new List<Genotype>((int)populationSize);
        for (int i = 0; i < populationSize; i++)
            currentPopulation.Add(new Genotype(new float[genotypeParamCount]));

        GenerationCount = 1;
        SortPopulation = true;
        Running = false;
    }

    public void Start()
    {
        Running = true;

        InitialisePopulation(currentPopulation);

        Evaluation(currentPopulation);
    }

    public void EvaluationFinished()
    {

        FitnessCalculationMethod(currentPopulation);

        if (SortPopulation)
            currentPopulation.Sort();

        if (FitnessCalculationFinished != null)
            FitnessCalculationFinished(currentPopulation);

        if (TerminationCriterion != null && TerminationCriterion(currentPopulation))
        {
            Terminate();
            return;
        }

        List<Genotype> intermediatePopulation = Selection(currentPopulation);

        List<Genotype> newPopulation = Recombination(intermediatePopulation, PopulationSize);

        Mutation(newPopulation);

        currentPopulation = newPopulation;
        GenerationCount++;

        Evaluation(currentPopulation);
    }

    private void Terminate()
    {
        Running = false;
        if (AlgorithmTerminated != null)
            AlgorithmTerminated(this);
    }

    public static void DefaultPopulationInitialisation(IEnumerable<Genotype> population)
    {
        foreach (Genotype genotype in population)
            genotype.SetRandomParameters(DefInitParamMin, DefInitParamMax);
    }

    public static void AsyncEvaluation(IEnumerable<Genotype> currentPopulation)
    {
    }

    public static void DefaultFitnessCalculation(IEnumerable<Genotype> currentPopulation)
    {
        uint populationSize = 0;
        float overallEvaluation = 0;
        foreach (Genotype genotype in currentPopulation)
        {
            overallEvaluation += genotype.Evaluation;
            populationSize++;
        }

        float averageEvaluation = overallEvaluation / populationSize;

        foreach (Genotype genotype in currentPopulation)
            genotype.Fitness = genotype.Evaluation / averageEvaluation;
    }

    public static List<Genotype> DefaultSelectionOperator(List<Genotype> currentPopulation)
    {
        List<Genotype> intermediatePopulation = new List<Genotype>();
        intermediatePopulation.Add(currentPopulation[0]);
        intermediatePopulation.Add(currentPopulation[1]);
        intermediatePopulation.Add(currentPopulation[2]);

        return intermediatePopulation;
    }

    public static List<Genotype> DefaultRecombinationOperator(List<Genotype> intermediatePopulation, uint newPopulationSize)
    {
        if (intermediatePopulation.Count < 2) throw new ArgumentException("Intermediate population size must be greater than 2 for this operator.");

        List<Genotype> newPopulation = new List<Genotype>();
        while (newPopulation.Count < newPopulationSize)
        {
            Genotype offspring1, offspring2;
            CompleteCrossover(intermediatePopulation[0], intermediatePopulation[1], DefCrossSwapProb, out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }

        return newPopulation;
    }

    public static void DefaultMutationOperator(List<Genotype> newPopulation)
    {
        foreach (Genotype genotype in newPopulation)
        {
            if (randomizer.NextDouble() < DefMutationPerc)
                MutateGenotype(genotype, DefMutationProb, DefMutationAmount);
        }
    }

    public static void CompleteCrossover(Genotype parent1, Genotype parent2, float swapChance, out Genotype offspring1, out Genotype offspring2)
    {
        int parameterCount = parent1.ParameterCount;
        float[] off1Parameters = new float[parameterCount], off2Parameters = new float[parameterCount];

        for (int i = 0; i < parameterCount; i++)
        {
            if (randomizer.Next() < swapChance)
            {
                off1Parameters[i] = parent2[i];
                off2Parameters[i] = parent1[i];
            }
            else
            {
                off1Parameters[i] = parent1[i];
                off2Parameters[i] = parent2[i];
            }
        }

        offspring1 = new Genotype(off1Parameters);
        offspring2 = new Genotype(off2Parameters);
    }

    public static void MutateGenotype(Genotype genotype, float mutationProb, float mutationAmount)
    {
        for (int i = 0; i < genotype.ParameterCount; i++)
        {
            if (randomizer.NextDouble() < mutationProb)
            {
                genotype[i] += (float)(randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount);
            }
        }
    }

}
