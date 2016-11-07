using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour
{
    /// <summary>
    /// The player or NPC to blame for this explosion.
    /// </summary>
    [HideInInspector]
    public GameObject CausedBy;

    /// <summary>
    /// The damage this explosion will deal to a target caught in the center of the blast.
    /// </summary>
    public int MaxDamage;
    /// <summary>
    /// The damage this explosion will deal to a target caught at the edge of the blast.
    /// </summary>
    public int MinDamage;
    /// <summary>
    /// The radius of the effect of this explosion.
    /// </summary>
    public float SplashRadius;
    
    // Use this for initialization
    void Start ()
    {
        //Check for killable things in the radius
        LayerMask layer_mask = LayerMask.GetMask("Enemies", "Player", "Walls");
        Collider[] hit_colliders = Physics.OverlapSphere(transform.position, SplashRadius, layer_mask);
        for (int i = 0; i < hit_colliders.Length; ++i)
        {
            //Scale damage done by distance from centre of explosion.
            Vector3 hit_point = hit_colliders[i].ClosestPointOnBounds(transform.position);
            float distance_from_centre = Vector3.Distance(hit_point, transform.position);
            int damage = Mathf.RoundToInt(Mathf.Lerp(MaxDamage, MinDamage, Mathf.Clamp01(distance_from_centre / SplashRadius)));

            Destructible hit_object = hit_colliders[i].GetComponent<Destructible>();
            if (hit_object != null)
            {
                hit_object.TakeDamage(damage, hit_point, (CausedBy != null && CausedBy.tag == "Player") ? "Player" : null);
            }
        }

    }
}
