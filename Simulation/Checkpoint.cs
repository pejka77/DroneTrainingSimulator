using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public float CaptureRadius = 3;
    private SpriteRenderer spriteRenderer;

    public float RewardValue
    {
        get;
        set;
    }

    public float DistanceToPrevious
    {
        get;
        set;
    }

    public float AccumulatedDistance
    {
        get;
        set;
    }

    public float AccumulatedReward
    {
        get;
        set;
    }

    public bool IsVisible
    {
        get { return spriteRenderer.enabled; }
        set { spriteRenderer.enabled = value; }
    }
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float GetRewardValue(float currentDistance)
    {
        float completePerc = (DistanceToPrevious - currentDistance) / DistanceToPrevious; 

        if (completePerc < 0)
            return 0;
        else return completePerc * RewardValue;
    }
}
