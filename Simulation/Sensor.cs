using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField]
    private LayerMask LayerToSense;

    [SerializeField]
    private SpriteRenderer Cross;

    private const float MAX_DIST = 10f;
    private const float MIN_DIST = 0.01f;

    public float Output
    {
        get;
        private set;
    }
    void Start ()
    {
        Cross.gameObject.SetActive(true);
	}
    void FixedUpdate ()
    {
        Vector2 direction = Cross.transform.position - this.transform.position;
        direction.Normalize();

        RaycastHit2D hit =  Physics2D.Raycast(this.transform.position, direction, MAX_DIST, LayerToSense);

        if (hit.collider == null)
            hit.distance = MAX_DIST;
        else if (hit.distance < MIN_DIST)
            hit.distance = MIN_DIST;

        this.Output = hit.distance;
        Cross.transform.position = (Vector2) this.transform.position + direction * hit.distance;
	}

    public void Hide()
    {
        Cross.gameObject.SetActive(false);
    }

    public void Show()
    {
        Cross.gameObject.SetActive(true);
    }
}
