using UnityEngine;

public class TeleporterController : MonoBehaviour
{
    public PlayerController player;

    public Transform teleportPosition;
    Vector3 tPosition = new Vector3(0, 1, 0);
    private void OnTriggerEnter(Collider other)
    {
        Teleport(teleportPosition);
    }

    void Teleport(Transform teleposition)
    {
        FindObjectOfType<AudioController>().Play("Event");
        if (teleportPosition != null)
        {
           tPosition = new Vector3(teleposition.position.x, teleposition.position.y + 1, teleposition.position.z);
        }

        Debug.Log("teleporter Trigger enter: teleport to " + tPosition);
        player.SetPosition(tPosition);
    }


}
