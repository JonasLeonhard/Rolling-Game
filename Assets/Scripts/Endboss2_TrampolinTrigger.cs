using System.Collections;
using UnityEngine;

public class Endboss2_TrampolinTrigger : MonoBehaviour
{

    public Endboss2Controller endboss;
    public EnemyController enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (endboss != null)
            {
                endboss.SubLifePoints(100);
                endboss.TriggerPhase2();
                FindObjectOfType<AudioController>().Play("PlayerShot");
            }
            else if (enemy != null)
            {
                enemy.SubLifePoint(100);
                FindObjectOfType<AudioController>().Play("PlayerShot");
            }

        }
    }

    private IEnumerator invincibleTimer()
    {
        yield return null;
    }
}
