using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endboss2_WinzoneMiddleController : MonoBehaviour
{
    bool endless = true;
    public float timer = 5;
    public GameObject zone;

   
    public void Start()
    {
        TurnOn();
    }

    public IEnumerator WindSwitch()
    {
        while (endless)
        {
            
                yield return new WaitForSeconds(timer);
                zone.SetActive(false);
                yield return new WaitForSeconds(timer);
                zone.SetActive(true);


        }
     
    }

    public void TurnOff()
    {
        endless = false;
    }

    public void TurnOn()
    {
        endless = true;
        StartCoroutine(WindSwitch());
       
    }
}
