using UnityEngine;

public class PointController : MonoBehaviour
{
    public PlayerController player;
    public GameController gameController;
    public GameObject Point;

    public bool startingPoint;
    public bool endLevelPoint;
    public bool checkPoint;

    public bool cameraEvent;
    public Transform pointTo;
    public CameraController cameraController;
    public float lookTime = 2f;
    bool hasLooked = false;

    //endLevelPoint Variables:

    public float detectionRadius = 3f;
    private SphereCollider pointCollider; //used in checkpoint aswell

    //checkPointVariables:
    private GameObject checkPointShape;

    public void Start()
    {
        SetStartingPoint(); //init. player position
        SetEndLevelPoint(); //init collider, ..
        SetCheckPoint(); //init collider, 
        SetCameraEvent(); //init collider;
    }

    public Vector3 getCurrentPosition()
    {
        //return the position of the object with this script attached to,
        //used for setting spawningpoints etc.
        return transform.position;
    }

    public void SetStartingPoint()
    {
        //controls startingPoint functionality:
        if (startingPoint)
        {
            CreateSphereCollider();

            player.transform.position = this.transform.position;
            player.lastCheckPoint = transform.position;
        }

    }

    public void SetCameraEvent()
    {
        if (cameraEvent)
        {
            Debug.Log("COLLIDER");
            CreateSphereCollider();
        }
    }
    public void SetEndLevelPoint()
    {
        //initialize colliders
        if (endLevelPoint)
        {
            CreateSphereCollider();
        }
    }

    public void SetCheckPoint()
    {
        //initialize CheckPoint:
        if (checkPoint)
        {
            //create collider for checkpoint
            CreateSphereCollider();

            //create checkpoint shape:
            checkPointShape = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            checkPointShape.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + checkPointShape.transform.localScale.y + 1, this.transform.position.z);
            checkPointShape.transform.localScale = new Vector3(0.3f, 1, 0.3f);
            checkPointShape.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1); //change color to red

            //make collider istrigger:
            Collider cylcol = checkPointShape.GetComponent<CapsuleCollider>();
            cylcol.isTrigger = true;

        }
    }

    private void CreateSphereCollider()
    {
        //creates a sphere collider for player detection
        // used in levelendpoint / checkpoint

        pointCollider = Point.gameObject.AddComponent<SphereCollider>();
        pointCollider.radius = detectionRadius;
        pointCollider.isTrigger = true; //ignore physics
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (endLevelPoint)
            {
                EndLevelPointTrigger();
            }
            else if (checkPoint)
            {
                CheckPointTrigger();
            }
            else if (startingPoint)
            {
                StartPointTrigger();
            }
            else if (cameraEvent)
            {
                CameraEventTrigger();
            }
        }
    }

    private void EndLevelPointTrigger()
    {
        //point detected level ended, because player is in endzone collision radius:
        print("ZIEL ERREICHT!");
        gameController.LevelEndPointTrigger();
    }

    private void CheckPointTrigger()
    {
        //checkpoint Triggered, save playerposition, state this point as last checkpoint:
        //if the player dies he gets reset to this point
        print("checkpoint entered");
        FindObjectOfType<AudioController>().Play("Event");
        player.lastCheckPoint = transform.position;
        checkPointShape.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
    }

    private void StartPointTrigger()
    {
        player.lastCheckPoint = transform.position;
        player.startPoint = transform.position;
        print("StartPoint set");
    }

    private void CameraEventTrigger()
    {
        if(!hasLooked)
        {
            hasLooked = true;
            FindObjectOfType<AudioController>().Play("Event");
            cameraController.CameraTargetMode(pointTo.position, lookTime);
        }
    }


}
