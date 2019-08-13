using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneController : MonoBehaviour
{

    public PlayerController player;
    public GameController gameController;

    public bool instantDeath;

    private void OnTriggerEnter(Collider other)
    {
        //player Collides with DeathZone:
        if (instantDeath && other.gameObject.tag == "Player")
        {
            player.instantDeathZoneEnter();
            gameController.instantDeathZoneTrigger();
        }

        if(!instantDeath && other.gameObject.tag == "Player" && gameController.GetLifePoints() >= 2)
        {
            player.ResetToStartOrLastCheckpoint();
            gameController.SubLifePoint(1);
        }
        else if(!instantDeath && other.gameObject.tag == "Player" && gameController.GetLifePoints() <= 1)
        {
            player.instantDeathZoneEnter();
            gameController.instantDeathZoneTrigger();
        }

        if(other.gameObject.tag == "Switch")
        {
            Debug.Log("Switch hit Deathzone, reset gameobject" + other.gameObject.name);
            ObjectRespawnController oCtrl = other.gameObject.GetComponent<ObjectRespawnController>();
            other.transform.position = oCtrl.GetStartPosition();
        }

    }
}
