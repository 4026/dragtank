using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject Bwip;
    public float Duration;
    public float Cooldown;
    public Destructible ProtectedObject;

    private float m_remainingDuration;

    void Start()
    {
        ProtectedObject.Invulnerable = true;
        m_remainingDuration = Duration;
    }

    void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.Moving && m_remainingDuration > 0)
        {
            m_remainingDuration = Mathf.Max(m_remainingDuration - Time.deltaTime, 0);
            if (m_remainingDuration <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (ProtectedObject != null)
        {
            ProtectedObject.Invulnerable = false;
        }
    }
    
    /// <summary>
    /// Called when a projectile is blocked by the shield.
    /// </summary>
	public void OnHit (Vector3 position)
    {
        //When hit, spawn a particle effect to show that the bullet got eaten by the shield.
        Instantiate(Bwip, position, Quaternion.identity);
	}
}
