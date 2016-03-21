using UnityEngine;
using System.Collections;

public class EnemySpawnController : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnSpecification
    {
        public GameObject EnemyType;
        public int MaxToSpawn;
    }

    public SpawnSpecification[] SpawnSpecifications;

    public float spawnPeriod;
	public float spawnDistance;

	private GameObject player;
	private GameManager gameManager;

	private class SpawnLocationException : UnityException
	{
	}

	void Awake ()
	{
		gameManager = GameManager.Instance;
		gameManager.NotifyStateChange += OnStateChange;
	}

	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");

	}
    
	void OnDestroy ()
	{
		gameManager.NotifyStateChange -= OnStateChange;
	}


	void OnStateChange (GameState old_state, GameState new_state)
	{
		if (new_state == GameState.Moving) {
			StartCoroutine (SpawnEnemies());
		} else if (old_state == GameState.Moving) {
			StopCoroutine (SpawnEnemies());
		}
	}

	IEnumerator SpawnEnemies ()
	{
		while (true) {
			foreach (SpawnSpecification spawn_specification in SpawnSpecifications)
            {
				GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag (spawn_specification.EnemyType.tag);
				if (existingEnemies.Length < spawn_specification.MaxToSpawn) {
					try {
						Vector3 spawn_pos = getSpawnLocation ();
						Instantiate (spawn_specification.EnemyType, spawn_pos, Quaternion.identity);
					} catch (SpawnLocationException) {
						Debug.LogWarning ("Failed to find enemy spawn location.");
					}
				}
			}

			yield return new WaitForSeconds (spawnPeriod);
		}
	}

	private Vector3 getSpawnLocation ()
	{
		Vector3 spawn_pos;
		LayerMask layer_mask = ~LayerMask.GetMask ("Ground");

		for (int attempt = 0; attempt < 10; ++attempt) {
			//Generate a new candidate spawn location
			Vector2 spawn_direction = Random.insideUnitCircle.normalized * spawnDistance;
			spawn_pos = new Vector3 (spawn_direction.x, 0, spawn_direction.y) + player.transform.position;

			if (!Physics.CheckSphere (spawn_pos, 1.0f, layer_mask)) {
				//If there's nothing else near the spawn location, great. Return this location.
				return spawn_pos;
			}

		}

		//If we've failed to find anywhere suitable to spawn, throw an exception.
		throw new SpawnLocationException ();
	}


}
