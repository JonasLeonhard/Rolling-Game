using UnityEngine;

public class PushAwayController : MonoBehaviour
{
    public Vector3 pushDirection = new Vector3(0, 1, 0);
    public float pushStrength = 25f;

    Vector3 appliedPushDirection;

    private void Start()
    {
        appliedPushDirection = pushDirection;
    }
    private void OnTriggerEnter(Collider other)
    {

        PushToPlayer(other);
        PushAway(other);
    }
    private void OnCollisionEnter(Collision collision)
    {
        PushToPlayer(collision);
        PushAway(collision);
    }

    private void PushToPlayer(Collider other)
    {
        appliedPushDirection = -(transform.position - other.transform.position).normalized;
    }
    private void PushToPlayer(Collision collision)
    {
        appliedPushDirection = -(transform.position - collision.transform.position).normalized;
    }

    private void PushAway(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * pushStrength, ForceMode.Impulse);
        rb.AddForce(appliedPushDirection * pushStrength, ForceMode.Impulse);

    }
    private void PushAway(Collision other)
    {
        other.rigidbody.AddForce(Vector3.up * pushStrength, ForceMode.Impulse);
        other.rigidbody.AddForce(appliedPushDirection * pushStrength, ForceMode.Impulse);
    }
}
