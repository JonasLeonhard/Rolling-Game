using System.Collections;
using UnityEngine;

public class GroundSpikesController : MonoBehaviour {
    public GameObject spikes;
    public GameObject floor;

    public float triggersAfter = 1f;
    public float activeTime = 0.6f;

    Renderer floorRen;

    private void Start()
    {
        floorRen = floor.GetComponent<Renderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(TriggerSpikes());
    }

    private IEnumerator TriggerSpikes()
    {
        Debug.Log("Spikes Triggered");
        ShowTriggered();

        yield return new WaitForSeconds(triggersAfter);
        FindObjectOfType<AudioController>().Play("EnemyShot");
        spikes.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        ShowInactive();
        spikes.SetActive(false);

    }

    private void ShowTriggered()
    {
        //set floor red
        if(floor != null){
            floorRen.material.color = new Color(168, 0, 0, 1);
        }else
        {
            Debug.Log("floor not Set in: " + gameObject.name);
        }
    }

    private void ShowInactive()
    {
        if (floor != null)
        {
            floorRen.material.color = new Color(1, 1, 1, 1);
        }
        else
        {
            Debug.Log("floor not Set in: " + gameObject.name);
        }
    }
}
