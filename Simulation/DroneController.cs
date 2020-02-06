using UnityEngine;

public class DroneController : MonoBehaviour
{
    private static int idGenerator = 0;

    private static int NextID
    {
        get { return idGenerator++; }
    }

    private const float MAX_CHECKPOINT_DELAY = 7;

    public Agent Agent
    {
        get;
        set;
    }

    public float CurrentCompletionReward
    {
        get { return Agent.Genotype.Evaluation; }
        set { Agent.Genotype.Evaluation = value; }
    }

    public bool UseUserInput = false;

    public DroneMovement Movement
    {
        get;
        private set;
    }

    public double[] CurrentControlInputs
    {
        get { return Movement.CurrentInputs; }
    }

    public SpriteRenderer SpriteRenderer
    {
        get;
        private set;
    }

    private Sensor[] sensors;
    private float timeSinceLastCheckpoint;

    void Awake()
    {

        Movement = GetComponent<DroneMovement>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        sensors = GetComponentsInChildren<Sensor>();
    }
    void Start()
    {
        Movement.HitWall += Die;

        this.name = "Drone (" + NextID + ")";
    }

    public void Restart()
    {
        Movement.enabled = true;
        timeSinceLastCheckpoint = 0;

        foreach (Sensor s in sensors)
            s.Show();

        Agent.Reset();
        this.enabled = true;
    }

    void Update()
    {
        timeSinceLastCheckpoint += Time.deltaTime;
    }

    void FixedUpdate()
    {

        if (!UseUserInput)
        {

            double[] sensorOutput = new double[sensors.Length];
            for (int i = 0; i < sensors.Length; i++)
                sensorOutput[i] = sensors[i].Output;

            double[] controlInputs = Agent.FNN.ProcessInputs(sensorOutput);
            Movement.SetInputs(controlInputs);
        }

        if (timeSinceLastCheckpoint > MAX_CHECKPOINT_DELAY)
        {
            Die();
        }
    }

    private void Die()
    {
        this.enabled = false;
        Movement.Stop();
        Movement.enabled = false;

        foreach (Sensor s in sensors)
            s.Hide();

        Agent.Kill();
    }

    public void CheckpointCaptured()
    {
        timeSinceLastCheckpoint = 0;
    }
}
