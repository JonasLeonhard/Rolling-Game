using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZoneController : MonoBehaviour
{
    //This Script Handles a Wind Zone- if a Player enters the Collider of the Wind Zone he gets pushed towards the given angle
    public enum State { On, Off }
    State currentState = State.On;

    public bool windX;
    public bool windY;
    public bool windZ;

    public bool negX;
    public bool negY;
    public bool negZ;

    public Rigidbody rbPlayer;
    public Transform particleSystem;

    public float windStrength = 1.7f;
    Vector3 windDirectionAngle = new Vector3(0, 0, 0);

    private void Start()
    {
        if (rbPlayer != null)
        {
            rbPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        }
        else
        {
            Debug.Log("WindController: rbPlayer not Set in - " + gameObject.name);
        }

        if (particleSystem == null)
        {
            Debug.Log("WindController: ParticleSystem not set in - " + gameObject.name);
        }

        SetWindDirection();
        AlignParticleSystem();
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentState == State.On && other.gameObject.tag == "Player")
        {
            ApplyWindForce(WindAngleToDirection(windDirectionAngle), windStrength);
        }
    }

    private void SetWindDirection()
    {
        if (windX)
        {
            windDirectionAngle += new Vector3(90, 0, 0);
        }

        if (windY)
        {
            windDirectionAngle += new Vector3(0, 90, 0);
        }

        if (windZ)
        {
            windDirectionAngle += new Vector3(0, 0, 90);
        }

        if (negX)
        {
            windDirectionAngle.x -= 180;
        }

        if (negY)
        {
            windDirectionAngle.y -= 180;
        }

        if (negZ)
        {
            windDirectionAngle.z -= 180;
        }
    }
    private void ApplyWindForce(Vector3 direction, float strength)
    {
        rbPlayer.AddForce(direction * strength, ForceMode.Impulse);
    }
    private Vector3 WindAngleToDirection(Vector3 angle)
    {
        return angle.normalized;
    }

    private void AlignParticleSystem()
    {
        if (windX || windY || windZ)
        {
            Vector3 systemRotation = new Vector3(0, 0, 0);

                if (windX && !windY && !windZ)//X
                {
                    systemRotation = new Vector3(0, 90, 0);
                }
                else if (windY && !windX && !windZ)//Y
                {
                    systemRotation = new Vector3(-90, 0, 0);
                }
                else if (windZ && !windX && !windY)//Z
                {
                    systemRotation = new Vector3(0, 0, 0);
                }
                else if (windX && windY && !windZ)//XY
                {
                    systemRotation = new Vector3(-125, -90, 0);
                }
                else if (windX && windZ && !windY)//XZ
                {
                    systemRotation = new Vector3(0, 45, 0);
                }
                else if (windY && windZ && !windX)//YZ
                {
                    systemRotation = new Vector3(-45, 0, 0);
                }
                else if (windX && windY && windZ)
                {
                    systemRotation = new Vector3(-45, 45, 0);
                }

            if(windStrength < 0 || negX || negY || negZ)
            {
                Debug.Log("TURN WIND");
                if((windX && windStrength <0)|| (windX && negX))
                {
                    Debug.Log("TURN X");
                    systemRotation += new Vector3(0, -180, 0);
                }
                if((windY && windStrength < 0)|| (windY && negY))
                {
                    Debug.Log("TURN Y");
                    systemRotation += new Vector3(180, 0, 0);
                }
                if((windZ && windStrength < 0)|| (windZ && negZ))
                {
                    Debug.Log("TURN Z");
                    systemRotation += new Vector3(0, 180, 0);
                }
            }

            particleSystem.transform.rotation = Quaternion.Euler(systemRotation);
        }
    }
    public void TurnOff()
    {
        currentState = State.Off;
    }

    public void TurnOn()
    {
        currentState = State.On;
    }
}
