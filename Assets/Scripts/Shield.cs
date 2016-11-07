using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject Bwip;
	
    
    /// <summary>
    /// Called when a projectile is blocked by the shield.
    /// </summary>
	public void OnHit (Vector3 position)
    {
        //When hit, spawn a particle effect to show that the bullet got eaten by the shield.
        Instantiate(Bwip, position, Quaternion.identity);
	}
}
