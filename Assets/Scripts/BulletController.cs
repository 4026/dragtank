using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
	public Vector3 Target;
	public float Speed;
    /// <summary>
    /// Damage caused by direct hit.
    /// </summary>
    public int Damage;
	public GameObject ExplosionPrefab;

    /// <summary>
    /// The player or NPC that fired this bullet
    /// </summary>
    [HideInInspector]
    public GameObject FiredBy;

	private GameManager gameManager;
	private float trail_fade_time;

    /// <summary>
    /// Whether this bullet is still able to move and kill things.
    /// </summary>
    private bool m_isAlive = true;
    
	void Start ()
	{
        gameManager = GameManager.Instance;
        gameManager.NotifyStateChange += OnStateChange;

        trail_fade_time = GetComponent<TrailRenderer> ().time;
	}

	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}

	void OnStateChange (GameManager.GameState old_state, GameManager.GameState new_state)
	{
		switch (new_state) {
		case GameManager.GameState.Moving:
			GetComponent<TrailRenderer> ().time = trail_fade_time; //Resume trail renderer fade-out
			break;
		case GameManager.GameState.PlanCountdown:
			GetComponent<TrailRenderer> ().time = Mathf.Infinity; //Pause trail renderer fade-out
			break;
		}
	}

	void FixedUpdate ()
	{
		if (gameManager.State != GameManager.GameState.Moving || !m_isAlive) {
			return;
		}

		//Calculate the bullet's new position.
		Vector3 to_target = Target - transform.position;
		Vector3 motion;
		bool hitting_target = false;
		if (to_target.magnitude < Speed * Time.deltaTime) {
			motion = to_target;
			hitting_target = true;
		} else {
			motion = to_target.normalized * Speed * Time.deltaTime;
		}

        //Move the bullet to the new position, calculating to see if we hit anything on the way.
        LayerMask layer_mask = LayerMask.GetMask("Enemies", "Walls", "Player");
        RaycastHit hit_info;
		if (Physics.Raycast (transform.position, motion, out hit_info, motion.magnitude, layer_mask)) {
			transform.position = hit_info.point;

            Collider other = hit_info.collider;
            if (other.GetComponent<Shield>() != null)
            {
                //Shield eats the bullet; no explosion.
                other.GetComponent<Shield>().OnHit(transform.position);
                m_isAlive = false;

                //This still counts as a hit, though...
                if (FiredBy.tag == "Player")
                {
                    StatsTracker.Instance.CurrentGame.Increment(GameStats.IntStatistic.ShotsHit);
                }
            }
            else if (other.GetComponent<Destructible>() != null)
            {
                //Direct hit! Target takes extra damage.
                other.GetComponent<Destructible>().TakeDamage(Damage, hit_info.point);
                detonate();

                if (FiredBy != null && FiredBy.tag == "Player")
                {
                    StatsTracker.Instance.CurrentGame.Increment(GameStats.IntStatistic.ShotsHit);
                }
            }
            else
            {
                //Just explode.
                detonate();
            }
			
		} else if (hitting_target) {
			transform.position = Target;
			detonate ();
		} else {
			transform.position += motion;
		}
	}

	private void detonate ()
	{
        if (ExplosionPrefab != null)
        {
            GameObject explosion = Instantiate(ExplosionPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f)) as GameObject;
            ExplosionController explosion_data = explosion.GetComponent<ExplosionController>();
            explosion_data.CausedBy = FiredBy;
        }

        m_isAlive = false;
	}
}
