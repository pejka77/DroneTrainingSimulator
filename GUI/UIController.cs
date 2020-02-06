using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Canvas Canvas
    {
        get;
        private set;
    }

    private UISimulationController simulationUI;

    void Awake()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.UIController = this;

        Canvas = GetComponent<Canvas>();
        simulationUI = GetComponentInChildren<UISimulationController>(true);

        simulationUI.Show();
    }

    public void SetDisplayTarget(DroneController target)
    {
        simulationUI.Target = target;
    }
}
