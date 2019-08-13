
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    //scrip to controll the Camera relative to the Player.
    public enum State { Iso, Follow, Target, FirstPerson }
    State currentState = State.Follow;

    public GameObject player;

    //setRotation():
    public float rotationsmoothness = 5.0f;
    public Vector3 rotationAngle = new Vector3(24, 0, 0);

    //setPosition():
    public float positionsmoothness = 10f;
    public Vector3 offset = new Vector3(10, 10, 10); //used to follow position and change height
    public Vector3 fpoffset = new Vector3((4), 1, 1); //mincamheight + 1 !!BOTH OFFSETS CANT BE 0 IN XYZ!!
    //rotateAround():
    public float orbitalSpeed = 10f;

    //TurnCamera
    private float qeRotation = 0;

    //change height
    public float maxCamHeight = 18;
    public float minCamHeight = 2;
    public float fpmaxCamHeight = 9;
    public float fpminCamHeight = 1;

    private void Start()
    {
        if (currentState == State.Follow || currentState == State.Iso || currentState == State.FirstPerson)
        {
            SetCamRotationAngle(rotationAngle); //set to given angle then-
            transform.position = GetCamPositionfromAngle(); //returns camPosition from angle
        }
    }

    private void LateUpdate()
    {
        if (currentState == State.Follow || currentState == State.Iso)
        {
            RotateAround(); //allows to rotate around player
            TurnCamera(); //q/e turn
            ChangeHeight();
        }
        else if (currentState == State.FirstPerson)
        {
            RotateAround();
            TurnCamera();
            ChangeHeight();
        }

        if (Input.GetKeyDown("#") && currentState != State.Target)//starts TargetMode Coroutine
        {
            CameraTargetMode(player.transform.position + Vector3.up * 5, 2);
        }

        ChangeMode();

    }
    private void FixedUpdate()
    {
        if (currentState == State.Follow || currentState == State.Iso)
        {
            setPosition();
        }
        else if (currentState == State.FirstPerson)
        {
            setFirstPersonPosition();
        }
    }

    public void setPosition()
    {
        //get desired position, and interpolate towards it with smoothnesfactor*timepassed
        Vector3 desiredPosition = player.transform.position - offset.magnitude * transform.forward; //cam.forwardVector
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionsmoothness * Time.deltaTime);

        //if distance is too far, increase smoothness:
        //if too small, decrease sm.
        //dont go under threshhold of 2
        if (Vector3.Distance(desiredPosition, smoothedPosition) > 4)
        {
            positionsmoothness += 0.2f;
        }
        else if (Vector3.Distance(desiredPosition, smoothedPosition) < 4)
        {
            if (positionsmoothness > 5)
            {
                positionsmoothness -= 0.2f;
            }

        }
        //set camera position:
        transform.position = (smoothedPosition);
    }

    public void setPosition(Vector3 targetPosition)
    {
        //get desired position, and interpolate towards it with smoothnesfactor*timepassed
        Vector3 desiredPosition = targetPosition - offset.magnitude * transform.forward; //cam.forwardVector
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionsmoothness * Time.deltaTime);

        //if distance is too far, increase smoothness:
        //if too small, decrease sm.
        //dont go under threshhold of 2
        if (Vector3.Distance(desiredPosition, smoothedPosition) > 4)
        {
            positionsmoothness += 0.2f;
        }
        else if (Vector3.Distance(desiredPosition, smoothedPosition) < 4)
        {
            if (positionsmoothness > 5)
            {
                positionsmoothness -= 0.2f;
            }

        }
        //set camera position:
        transform.position = (smoothedPosition);
    }

    public void setFirstPersonPosition()
    {
        Vector3 desiredPosition = player.transform.position - (fpoffset.magnitude * transform.forward);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionsmoothness * Time.deltaTime);

        if (Vector3.Distance(desiredPosition, smoothedPosition) > 4)
        {
            positionsmoothness += 0.2f;
        }
        else if (Vector3.Distance(desiredPosition, smoothedPosition) < 4)
        {
            if (positionsmoothness > 5)
            {
                positionsmoothness -= 0.2f;
            }

        }

        transform.position = smoothedPosition;
    }

    private void SetCamRotationAngle(Vector3 rotationAngle)
    {
        //sets the camera to the given rotation
        Quaternion targetr = Quaternion.Euler(rotationAngle);
        transform.rotation = targetr; //set given rotation
    }
    private Vector3 GetCamPositionfromAngle()
    {
        //returns Cameraposition from given Rotation -
        //it gets the forward vector of the camera and goes backwards offset.mag off the player 
        //returns cam position from player.pos + offsetlength
        float positionoffset = offset.magnitude;

        if (currentState == State.FirstPerson)
        {
            positionoffset = fpoffset.magnitude;
        }

        return player.transform.position - positionoffset * transform.forward;
    }

    public void LookAt(Vector3 desiredPosition)
    {
        //turn camera to look at given position
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(desiredPosition), Time.deltaTime);

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(desiredPosition, Vector3.up), Time.time * positionsmoothness);
        //transform.position = getCampositionfromRotation();
        transform.LookAt(desiredPosition);
    }

    private void TurnCamera()
    {
        //allows camera to turn around player while pressing q / e

        if (Input.GetKeyUp(KeyCode.Q))
        {
            qeRotation += 90;
            QERotationChanger();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            qeRotation -= 90;
            QERotationChanger();
        }

        if (qeRotation >= 360 || qeRotation < -360)
        {
            qeRotation = 0;
        }


    }

    private void QERotationChanger()
    {

        rotationAngle = new Vector3(rotationAngle.x, qeRotation, rotationAngle.z);
        SetCamRotationAngle(rotationAngle);
        transform.position = GetCamPositionfromAngle();
    }


    private void RotateAround()
    {
        // line 48-49: https://forum.unity.com/threads/simple-rotation-of-the-camera-with-the-mouse-around-the-player.470278/

        //rotate around given position with you mouse input
        //rotates around up vector offset by camera position
        //only rotate while left mouse button is pressed


        if (Input.GetMouseButton(0))// key<--
        {
            //interpolate towards player position with camera:
            if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            {
                offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * orbitalSpeed, Vector3.up) * offset;
                //Debug.Log(Input.GetAxis("Mouse X"));
                //smooth turns
                transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, positionsmoothness * Time.deltaTime / 0.5f); //f-smoothness of rotation
                LookAt(player.transform.position);
            }
            else if (currentState == State.FirstPerson)
            {
                fpoffset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * orbitalSpeed, Vector3.up) * fpoffset;
                transform.position = Vector3.Lerp(transform.position, player.transform.position + fpoffset, positionsmoothness * Time.deltaTime / 0.5f); //f-smoothness of rotation
                LookAt(player.transform.position);
            }


            //while centered view:
            //lookAt(player.transform.position);
        }
    }
    private bool BetweenMaxandMin()
    {
        bool maxmin = false;
        //between maxHeight and minheight?
        if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            maxmin = player.transform.position.y + offset.y <= player.transform.position.y + maxCamHeight && player.transform.position.y + offset.y >= player.transform.position.y + minCamHeight;
        else if (currentState == State.FirstPerson)
            maxmin = player.transform.position.y + fpoffset.y <= player.transform.position.y + fpmaxCamHeight && player.transform.position.y + fpoffset.y >= player.transform.position.y + fpminCamHeight;

        return maxmin;
    }

    private bool LowerMin()
    {
        //lower then minHeihgt?
        bool lowmin = false;
        if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            lowmin = player.transform.position.y + offset.y < player.transform.position.y + minCamHeight;
        else if (currentState == State.FirstPerson)
            lowmin = player.transform.position.y + fpoffset.y < player.transform.position.y + fpminCamHeight;

        return lowmin;
    }

    private bool HigherMax()
    {
        bool highmax = false;

        if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            highmax = player.transform.position.y + offset.y > player.transform.position.y + maxCamHeight;
        else if (currentState == State.FirstPerson)
            highmax = player.transform.position.y + fpoffset.y > player.transform.position.y + fpmaxCamHeight;
        //highter maxHeight?
        return highmax;
    }

    private void ChangeOffset(float yInput)
    {
        if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            offset.y += yInput;
        if (currentState == State.FirstPerson)
            fpoffset.y += yInput;
    }

    private void LowerClampOffset()
    {
        if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            offset.y = minCamHeight + 1; //reset if smaller minheight
        if (currentState == State.FirstPerson)
            fpoffset.y = fpminCamHeight + 1;
    }

    private void HigherClampOffset()
    {
        if (currentState == State.Follow || currentState == State.Iso || currentState == State.Target)
            offset.y = maxCamHeight - 1; //reset if taller maxheight
        if (currentState == State.FirstPerson)
            fpoffset.y = fpmaxCamHeight - 1;
    }
    private void ChangeHeight()
    {
        // canges the offset.y 
        if (Input.GetKey("<"))// key<--
        {
            float yInput = Input.GetAxis("Mouse Y");

            //checks if the cam offset height is between max and min height relative to the player
            //prevents cam from going out of bounds
            if (BetweenMaxandMin())
            {
                ChangeOffset(yInput);
            }

            if (LowerMin())
            {
                LowerClampOffset();
            }

            if (HigherMax())
            {
                HigherClampOffset();
            }

        }


    }

    private IEnumerator TargetMode(Vector3 target, float time)
    {
        Debug.Log("Coroutine TargetMode-");
        //Coroutine for Looking at given position
        State lastState = currentState;
        currentState = State.Target;

        //save last cam state to reset later
        Vector3 lastforward = transform.forward;
        Vector3 lastPosition = transform.position;

        float progress = 0;
        while (progress <= 1)
        {
            progress += Time.deltaTime * 2;
            setPosition(target);
            LookAt(target);
            yield return null;
        }
        yield return new WaitForSeconds(time);

        //reset to last State
        transform.position = lastPosition;
        transform.forward = lastforward;
        currentState = lastState;
        Debug.Log("TargetMode Over- Reset to CameraState: " + currentState.ToString());
    }

    public void CameraTargetMode(Vector3 target, float time)
    {
        Debug.Log("CameraController: CameraTargetMode: TargetMode ");
        {
            StartCoroutine(TargetMode(target, time));
        }
    }

    public void ChangeMode()
    {
        if (Input.GetKeyDown("."))
        {
            //rotationAngle = new Vector3(0, rotationAngle.y, rotationAngle.z);
            currentState = State.FirstPerson;
        }

        else if (Input.GetKeyDown(","))
        {
            //rotationAngle = new Vector3(0, rotationAngle.y, rotationAngle.z);
            currentState = State.Follow;
        }
        else if (Input.GetKeyDown("-"))
        {
            currentState = State.Iso;
        }
    }
}