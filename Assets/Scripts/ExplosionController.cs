using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour
{
    public GameObject CausedBy;

    public int Damage;
    public float SplashRadius;
    
    // Use this for initialization
    void Start ()
    {
        //Check for killable things in the radius
        LayerMask layer_mask = LayerMask.GetMask("Enemies", "Player", "Walls");
        Collider[] hit_colliders = Physics.OverlapSphere(transform.position, SplashRadius, layer_mask);
        for (int i = 0; i < hit_colliders.Length; ++i)
        {
            Destructible hit_object = hit_colliders[i].gameObject.GetComponent<Destructible>();
            if (hit_object != null)
            {
                hit_object.TakeDamage(Damage, transform.position, (CausedBy != null && CausedBy.tag == "Player") ? "Player" : null);
            }
        }

    }
}
