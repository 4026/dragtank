using UnityEngine;
using System.Collections;

/// <summary>
/// An environmental obstacle that can be destroyed, and updates the pathfinding grid on destruction.
/// </summary>
public class BreakableObstacle : MonoBehaviour
{
    /// <summary>
    /// The material to use for this object when it is below half health.
    /// </summary>
    public Material DamagedMaterial;

    /// <summary>
    /// The Destructible component on this object tracking its current health.
    /// </summary>
    private Destructible m_destructible;

	void Start ()
    {
        m_destructible = GetComponent<Destructible>();
        m_destructible.OnDamaged += OnDamaged;
        m_destructible.OnDeath += OnDeath;
    }

    void OnDamaged(int damage_taken, int remaining_health)
    {
        //If the obstacle has had its health dropped below 50%, update the render material
        int half_health = m_destructible.StartingHealth / 2;
        if (remaining_health <= half_health && remaining_health + damage_taken > half_health)
        {
            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            Material[] materials = renderer.materials;
            materials[0] = DamagedMaterial;
            renderer.materials = materials;
        }

        //Shake the obstacle slightly to show it's been hit.
        iTween.ShakeScale(transform.GetChild(0).gameObject, new Vector3(0.1f, 0, 0.1f), 0.2f);
    }

    void OnDeath()
    {
        //Rebuild pathfinding grid.
        GetComponent<Collider>().enabled = false;
        AstarPath.active.Scan();
    }
	
	
}
