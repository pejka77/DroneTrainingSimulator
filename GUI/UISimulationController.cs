using UnityEngine.UI;
using UnityEngine;
using System;
public class UISimulationController : MonoBehaviour
{
    private DroneController target;

    public DroneController Target
    {
        get { return target; }
        set
        {
            if (target != value)
            {
                target = value;

                if (target != null)
                    NeuralNetPanel.Display(target.Agent.FNN);
            }
        }
    }

    [SerializeField]
    private Text[] InputTexts;
    [SerializeField]
    private Text Evaluation;
    [SerializeField]
    private Text GenerationCount;
    [SerializeField]
    private UINeuralNetworkPanel NeuralNetPanel;

    void Update()
    {
        if (Target != null)
        {
            if (Target.CurrentControlInputs != null)
            {
                for (int i = 0; i < InputTexts.Length; i++)
                    InputTexts[i].text = Target.CurrentControlInputs[i].ToString();
            }

            Evaluation.text = Target.Agent.Genotype.Evaluation.ToString();
            GenerationCount.text = EvolutionManager.Instance.GenerationCount.ToString();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
