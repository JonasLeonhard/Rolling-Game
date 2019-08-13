using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeColissionController : MonoBehaviour
{
    Rigidbody player;
    GameController gameController;

    bool inv = false;
    float throwForce = 25f;
    private void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (player == null)
            Debug.Log("Prefab Spike couldn't .FindGameObjectWithTag(Player);");
        if (gameController == null)
            Debug.Log("Prefab Spike couldn't .FindGameObjectWithTag(GameController);");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CheckInv(); //before gameController changes Invincible
            gameController.SubLifePoint(1); //- changes player to invincible
            Hit();
        }
    }

    private void Hit()
    {
        if (!inv)
        {
            player.AddForce(Vector3.up * throwForce, ForceMode.Impulse);
        }

    }
    private void CheckInv()
    {
        inv = gameController.PlayerInvincible();
    }
}
