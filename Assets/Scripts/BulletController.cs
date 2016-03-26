using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
	public Vector3 target;
	public float speed;
	public GameObject ExplosionPrefab;
	public float splashRadius;
	public int damage;
    public GameObject FiredBy;

	private GameManager gameManager;
	private float trail_fade_time;
    
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
		case GameManager.GameState.Planning:
			GetComponent<TrailRenderer> ().time = Mathf.Infinity; //Pause trail renderer fade-out
			break;
		}
	}

	void FixedUpdate ()
	{
		if (gameManager.State != GameManager.GameState.Moving) {
			return;
		}

		//Calculate the bullet's new position.
		Vector3 to_target = target - transform.position;
		Vector3 motion;
		bool hitting_target = false;
		if (to_target.magnitude < speed * Time.deltaTime) {
			motion = to_target;
			hitting_target = true;
		} else {
			motion = to_target.normalized * speed * Time.deltaTime;
		}

        //Move the bullet to the new position, calculating to see if we hit anything on the way.
        LayerMask layer_mask = LayerMask.GetMask("Enemies", "Walls", "Player");
        RaycastHit hit_info;
		if (Physics.Raycast (transform.position, motion, out hit_info, motion.magnitude, layer_mask)) {
			transform.position = hit_info.point;
			detonate ();
		} else if (hitting_target) {
			transform.position = target;
			detonate ();
		} else {
			transform.position += motion;
		}
	}

	private void detonate ()
	{
		GameObject explosion = Instantiate (ExplosionPrefab, transform.position, Quaternion.Euler (90f, 0f, 0f)) as GameObject;
        ExplosionController explosion_data = explosion.GetComponent<ExplosionController>();
        
        explosion_data.CausedBy = FiredBy;
        explosion_data.Damage = damage;
        explosion_data.SplashRadius = splashRadius;

        Destroy (this.gameObject);
	}
}
