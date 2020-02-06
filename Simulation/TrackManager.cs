using System;
using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    public static TrackManager Instance
    {
        get;
        private set;
    }

    private Checkpoint[] checkpoints;

    public DroneController PrototypeDrone;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private class MarsDrone
    {
        public MarsDrone(DroneController drone = null, uint checkpointIndex = 1)
        {
            this.Drone = drone;
            this.CheckpointIndex = checkpointIndex;
        }
        public DroneController Drone;
        public uint CheckpointIndex;
    }
    private List<MarsDrone> drones = new List<MarsDrone>();

    public int DroneCount
    {
        get { return drones.Count; }
    }

    private DroneController bestDrone = null;

    public DroneController BestDrone
    {
        get { return bestDrone; }
        private set
        {
            if (bestDrone != value)
            {
                DroneController previousBest = bestDrone;
                bestDrone = value;
                if (BestDronesChanged != null)
                    BestDronesChanged(bestDrone);

                SecondBestDrone = previousBest;
            }
        }
    }

    public event System.Action<DroneController> BestDronesChanged;

    private DroneController secondBestDrone = null;

    public DroneController SecondBestDrone
    {
        get { return secondBestDrone; }
        private set
        {
            if (SecondBestDrone != value)
            {
                secondBestDrone = value;
                if (SecondBestDroneChanged != null)
                    SecondBestDroneChanged(SecondBestDrone);
            }
        }
    }

    public event System.Action<DroneController> SecondBestDroneChanged;

    public float TrackLength
    {
        get;
        private set;
    }

    void Awake()
    {
        Instance = this;

        checkpoints = GetComponentsInChildren<Checkpoint>();

        startPosition = PrototypeDrone.transform.position;
        startRotation = PrototypeDrone.transform.rotation;
        PrototypeDrone.gameObject.SetActive(false);

        CalculateCheckpointPercentages();
    }

    void Start()
    {
        foreach (Checkpoint check in checkpoints)
            check.IsVisible = false;
    }

    void Update()
    {
        for (int i = 0; i < drones.Count; i++)
        {
            MarsDrone drone = drones[i];
            if (drone.Drone.enabled)
            {
                drone.Drone.CurrentCompletionReward = GetCompletePerc(drone.Drone, ref drone.CheckpointIndex);
                if (BestDrone == null || drone.Drone.CurrentCompletionReward >= BestDrone.CurrentCompletionReward)
                    BestDrone = drone.Drone;
                else if (SecondBestDrone == null || drone.Drone.CurrentCompletionReward >= SecondBestDrone.CurrentCompletionReward)
                    SecondBestDrone = drone.Drone;
            }
        }
    }

    public void SetDroneAmount(int amount)
    {
        if (amount < 0) throw new ArgumentException("Amount may not be less than zero.");

        if (amount == DroneCount) return;

        if (amount > drones.Count)
        {
            for (int toBeAdded = amount - drones.Count; toBeAdded > 0; toBeAdded--)
            {
                GameObject droneCopy = Instantiate(PrototypeDrone.gameObject);
                droneCopy.transform.position = startPosition;
                droneCopy.transform.rotation = startRotation;
                DroneController controllerCopy = droneCopy.GetComponent<DroneController>();
                drones.Add(new MarsDrone(controllerCopy, 1));
                droneCopy.SetActive(true);
            }
        }
        else if (amount < drones.Count)
        {

            for (int toBeRemoved = drones.Count - amount; toBeRemoved > 0; toBeRemoved--)
            {
                MarsDrone last = drones[drones.Count - 1];
                drones.RemoveAt(drones.Count - 1);

                Destroy(last.Drone.gameObject);
            }
        }
    }

    public void Restart()
    {
        foreach (MarsDrone drone in drones)
        {
            drone.Drone.transform.position = startPosition;
            drone.Drone.transform.rotation = startRotation;
            drone.Drone.Restart();
            drone.CheckpointIndex = 1;
        }

        BestDrone = null;
        SecondBestDrone = null;
    }

    public IEnumerator<DroneController> GetDroneEnumerator()
    {
        for (int i = 0; i < drones.Count; i++)
            yield return drones[i].Drone;
    }

    private void CalculateCheckpointPercentages()
    {
        checkpoints[0].AccumulatedDistance = 0;

        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].DistanceToPrevious = Vector2.Distance(checkpoints[i].transform.position, checkpoints[i - 1].transform.position);
            checkpoints[i].AccumulatedDistance = checkpoints[i - 1].AccumulatedDistance + checkpoints[i].DistanceToPrevious;
        }

        TrackLength = checkpoints[checkpoints.Length - 1].AccumulatedDistance;

        for (int i = 1; i < checkpoints.Length; i++)
        {
            checkpoints[i].RewardValue = (checkpoints[i].AccumulatedDistance / TrackLength) - checkpoints[i-1].AccumulatedReward;
            checkpoints[i].AccumulatedReward = checkpoints[i - 1].AccumulatedReward + checkpoints[i].RewardValue;
        }
    }

    private float GetCompletePerc(DroneController drone, ref uint curCheckpointIndex)
    {
        if (curCheckpointIndex >= checkpoints.Length)
            return 1;

        float checkPointDistance = Vector2.Distance(drone.transform.position, checkpoints[curCheckpointIndex].transform.position);

        if (checkPointDistance <= checkpoints[curCheckpointIndex].CaptureRadius)
        {
            curCheckpointIndex++;
            drone.CheckpointCaptured();
            return GetCompletePerc(drone, ref curCheckpointIndex);
        }
        else
        {
            return checkpoints[curCheckpointIndex - 1].AccumulatedReward + checkpoints[curCheckpointIndex].GetRewardValue(checkPointDistance);
        }
    }

}
