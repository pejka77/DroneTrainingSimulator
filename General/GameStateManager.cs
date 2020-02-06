using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    public string TrackName;

    public UIController UIController
    {
        get;
        set;
    }

    public static GameStateManager Instance
    {
        get;
        private set;
    }

    private DroneController prevBest, prevSecondBest;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameStateManagers in the Scene.");
            return;
        }
        Instance = this;

        SceneManager.LoadScene("GUI", LoadSceneMode.Additive);

        SceneManager.LoadScene(TrackName, LoadSceneMode.Additive);
    }

    void Start ()
    {
        TrackManager.Instance.BestDronesChanged += OnBestDroneChanged;
        EvolutionManager.Instance.StartEvolution();
	}

    private void OnBestDroneChanged(DroneController bestDrone)
    {
        var drones = GameObject.FindObjectsOfType<DroneController>();
        if (UIController != null)
            UIController.SetDisplayTarget(bestDrone);
        if (bestDrone == null || drones == null || CameraAir.useAirCam) return;
        foreach(var drone in drones)
        {
            var camera = drone.GetComponentInChildren<CameraFollow>( true);
            if (camera.gameObject.transform.parent.gameObject != bestDrone.gameObject)
            {
                camera.gameObject.SetActive(false);
            }
            else
            {
                camera.gameObject.SetActive(true);
            }
        }
    }
}
