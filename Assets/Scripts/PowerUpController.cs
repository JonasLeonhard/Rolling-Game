using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public GameController gameController;

    public bool isCoin = false;
    public bool isHearth = false;
    public bool isOrb = false;

    private bool isRotating = false;
    private bool isFloating = false;

    //rotate()
    private float currentY;
    private Vector3 currentPos;

    void Start()
    {
        currentPos = transform.position;
        currentY = transform.position.y;

        if (isCoin)
        {
            setCoin();
        }

        if(isHearth)
        {
            setHearth();
        }

        if(isOrb)
        {
            SetOrb();
        }
    }

    void Update()
    {
        if (isRotating)
        {
            Rotate();
        }
    }

    private void FixedUpdate()
    {
        if (isFloating)
        {
            Floating();
        }
    }

    public void setCoin()
    {
        isRotating = true;
        isFloating = true;
    }

    public void setHearth()
    {
        isRotating = true;
        isFloating = true;
    }

    public void SetOrb()
    {
        isRotating = true;
        isFloating = true;
    }

    public void Floating()
    {
        float height = 0.1f;

        currentPos.y = currentY + height * Mathf.Sin(Time.time);
        transform.position = currentPos;
    }
    public void Rotate()
    {

        transform.Rotate(0, 0, Time.deltaTime * 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCoin && other.gameObject.tag == "Player")
        {
            gameController.CoinTrigger();
            Destroy(transform.gameObject);
        }

        if(isHearth && other.gameObject.tag == "Player")
        {
            gameController.HearthTrigger();
            Destroy(transform.gameObject);
        }

        if(isOrb && other.gameObject.tag == "Player")
        {
            gameController.OrbTrigger();
            Destroy(transform.gameObject);
        }
    }
}
