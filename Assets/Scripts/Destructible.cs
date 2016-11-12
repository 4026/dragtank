using UnityEngine;

/// <summary>
/// An object that takes damage.
/// </summary>
public class Destructible : MonoBehaviour
{
    /// <summary>
    /// The explosion to instantiate upon death.
    /// </summary>
    public GameObject Explosion;

    /// <summary>
    /// The amount of health the object starts with.
    /// </summary>
    public int StartingHealth;

    /// <summary>
    /// The object's current health.
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
    /// Whether the object is currently invulnerable or not.
    /// </summary>
    public bool Invulnerable { get; set; }

    public delegate void DamageNotifier(int damage_taken, int remaining_health);
    /// <summary>
    /// Event that can be subscribed to to receive notification of this GameObject taking damage.
    /// </summary>
    public event DamageNotifier OnDamaged;

    public delegate void DeathNotifier();
    /// <summary>
    /// Event that can be subscribed to to receive notification of this GameObject's death.
    /// </summary>
    public event DeathNotifier OnDeath;
    

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

        if (OnDamaged != null)
        {
            OnDamaged(damage, Health);
        }

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
