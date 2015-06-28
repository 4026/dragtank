using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
	public Vector3 target;
	public float speed;
	public GameObject explosion;
	public float splashRadius;
	public int damage;

	private GameManager gameManager;
	private float trail_fade_time;

	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
	}

	void Start ()
	{
		trail_fade_time = GetComponent<TrailRenderer> ().time;
	}

	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}

	void OnStateChange (GameState old_state, GameState new_state)
	{
		switch (new_state) {
		case GameState.Moving:
			GetComponent<TrailRenderer> ().time = trail_fade_time; //Resume trail renderer fade-out
			break;
		case GameState.Planning:
			GetComponent<TrailRenderer> ().time = Mathf.Infinity; //Pause trail renderer fade-out
			break;
		}
	}

	void FixedUpdate ()
	{
		if (GameManager.Instance.gameState != GameState.Moving) {
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
		RaycastHit hit_info;
		if (Physics.Raycast (transform.position, motion, out hit_info, motion.magnitude)) {
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
		//Check for killable things in the radius
		LayerMask layer_mask = LayerMask.GetMask ("Enemies");
		Collider[] hit_colliders = Physics.OverlapSphere (transform.position, splashRadius, layer_mask);
		for (int i = 0; i < hit_colliders.Length; ++i) {
			GameObject hit_object = hit_colliders [i].gameObject;
			hit_object.SendMessage ("TakeDamage", damage);

		}

		Instantiate (explosion, transform.position, Quaternion.Euler (90f, 0f, 0f));
		Destroy (this.gameObject);
	}
}
