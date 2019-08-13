using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, 0);



    bool xUp = true;
    bool yUp = true;
    bool zUp = true;

    Vector3 currentPosition;
    Vector3 startPosition;

    public float elevatorSpeed = 0.1f;

    void Start()
    {
        startPosition = transform.position;

        if (offset.x < 0)
        {
            offset.x *= -1;
        }
        if (offset.y < 0)
        {
            offset.y *= -1;
        }
        if (offset.z < 0)
        {
            offset.y *= -1;
        }
    }

    void Update()
    {
        if (offset.x >= 0.1 || offset.x <= -0.1)
        {
            xUpDown();
        }
        if (offset.y >= 0.1 || offset.y <= -0.1)
        {
            yUpDown();
        }
        if (offset.z >= 0.1 || offset.z <= -0.1)
        {
            zUpDown();
        }

    }

    void xUpDown()
    {
        if (xUp)
        {
            //go up
            currentPosition = new Vector3(transform.position.x + elevatorSpeed, transform.position.y, transform.position.z);
            transform.position = currentPosition;

            //end check
            if (startPosition.x + offset.x <= currentPosition.x)
            {
                xUp = false;
            }
        }
        else if (!xUp)
        {
            //go down
            currentPosition = new Vector3(transform.position.x - elevatorSpeed, transform.position.y, transform.position.z);
            transform.position = currentPosition;

            //end check
            if (currentPosition.x <= startPosition.x)
            {
                xUp = true;
            }
        }
    }

    void yUpDown()
    {
        if (yUp)
        {
            //go up
            currentPosition = new Vector3(transform.position.x, transform.position.y + elevatorSpeed, transform.position.z);
            transform.position = currentPosition;

            //end check
            if (startPosition.y + offset.y <= currentPosition.y)
            {
                yUp = false;
            }

        }
        else if (!yUp)
        {
            //go down
            currentPosition = new Vector3(transform.position.x, transform.position.y - elevatorSpeed, transform.position.z);
            transform.position = currentPosition;

            //end check
            if (currentPosition.y <= startPosition.y)
            {
                yUp = true;
            }

        }
    }

    void zUpDown()
    {
        if (zUp)
        {
            //go up
            currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + elevatorSpeed);
            transform.position = currentPosition;

            //end check
            if (startPosition.z + offset.z <= currentPosition.z)
            {
                zUp = false;
            }
        }
        else if (!zUp)
        {
            //go down
            currentPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - elevatorSpeed);
            transform.position = currentPosition;

            //end check
            if (currentPosition.z <= startPosition.z)
            {
                zUp = true;
            }
        }
    }
}
