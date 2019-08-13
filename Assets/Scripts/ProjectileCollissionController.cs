using UnityEngine;

public class ProjectileCollissionController : MonoBehaviour
{
    //script: projectiles cant go through walls
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Scenery" || other.gameObject.tag == "Switch")
        {
            Debug.Log(gameObject.name + " hit Scenery|Untagged");
            Destroy(gameObject, 0);
        }
        if(other.gameObject.tag == "Enemy" && this.gameObject.tag != "Projectile")
        {
            Destroy(gameObject, 0);
        }
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Scenery")
        {
            Debug.Log(gameObject.name + " hit Scenery|Untagged");
            Destroy(gameObject, 0);
        }
    }
}
