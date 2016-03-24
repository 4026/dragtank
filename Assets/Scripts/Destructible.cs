using UnityEngine;

public delegate void DeathNotifier();

public class Destructible : MonoBehaviour {

    /// <summary>
    /// The explosion to instantiate upon death.
    /// </summary>
    public GameObject Explosion;

    /// <summary>
    /// The amount of health the enemy starts with.
    /// </summary>
    public int StartingHealth;

    /// <summary>
    /// The enemy's current health.
    /// </summary>
    public int Health { get; private set; }

    /// <summary>
    /// Multiplier applied to all damage taken on the front of the collider.
    /// </summary>
    public float FrontDamageMultiplier = 1f;

    /// <summary>
    /// Multiplier applied to all damage taken from the back of the collider.
    /// </summary>
    public float RearDamageMultiplier = 1f;

    /// <summary>
    /// Event that can be subscribed to to receive notification of this GameObject's death.
    /// </summary>
    public static event DeathNotifier OnDeath;

    void Start ()
    {
        Health = StartingHealth;
	}

    public void TakeDamage(int damage, Vector3 damage_origin, string except_tag = null)
    {
        if (except_tag != null && except_tag == gameObject.tag)
        {
            return;
        }

        Vector3 damage_direction = damage_origin - transform.position;
        if (Vector3.Dot(damage_direction, transform.forward) > 0)
        {
            damage = Mathf.RoundToInt(damage * FrontDamageMultiplier);
        }
        else
        {
            damage = Mathf.RoundToInt(damage * RearDamageMultiplier);
        }   

        Health -= damage;

        if (Health <= 0)
        {
            die();
        }
    }

    private void die()
    {
        if (OnDeath != null)
        {
            OnDeath();
        }

        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        iTween.PunchPosition(Camera.main.gameObject, Vector3.left, 1.0f);
    }
}
