using UnityEngine;

public class Destructible : MonoBehaviour {

    //The explosion to instantiate upon death.
    public GameObject Explosion;

    //The amount of health the enemy starts with.
    public int StartingHealth;

    //The enemy's current health.
    public int Health { get; private set; }

    // Multiplier applied to all damage taken on the front of the collider.
    public float FrontDamageMultiplier = 1f;

    // Multiplier applied to all damage taken from the back of the collider.
    public float RearDamageMultiplier = 1f;

    void Start ()
    {
        Health = StartingHealth;
	}

    public void TakeDamage(int damage, Vector3 damage_origin)
    {
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
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
        iTween.PunchPosition(Camera.main.gameObject, Vector3.left, 1.0f);
    }
}
