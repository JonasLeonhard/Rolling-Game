using UnityEngine;

public class Windzone_SchalterController : MonoBehaviour
{
    public GameObject getsTriggered;
    public Transform lookAt;

    CameraController cam;

    bool hasLooked = false;

    public bool setActive = true;
    public bool permanent = false;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        if (cam == null || getsTriggered == null)
        {
            Debug.Log("trigger or camera not set in: " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Switch" || other.tag == "Player")
        {
            LookAt();
            this.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
            FindObjectOfType<AudioController>().Play("Event");
        }
    }

    private void LookAt()
    {
        if (!hasLooked)
        {
            cam.CameraTargetMode(lookAt.position, 2);
            hasLooked = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log(setActive);
        if (setActive && (other.gameObject.tag == "Switch" || other.gameObject.tag == "Player"))
        {
            getsTriggered.SetActive(true);
        }
        else if (!setActive && (other.gameObject.tag == "Switch" || other.gameObject.tag == "Player"))
        {
            getsTriggered.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (setActive && (other.gameObject.tag == "Switch" || other.gameObject.tag == "Player") && !permanent)
        {
            getsTriggered.SetActive(false);
            this.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
        }
        else if (!setActive && (other.gameObject.tag == "Switch" || other.gameObject.tag == "Player") && !permanent)
        {
            getsTriggered.SetActive(true);
        }
    }
}
