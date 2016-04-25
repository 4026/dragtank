using UnityEngine;
using System.Collections;

public class EnemySpawnController : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnSpecification
    {
        public GameObject EnemyType;
        public int MaxToSpawn;
        public int GroupSize;
    }

    public SpawnSpecification[] SpawnSpecifications;

    public float spawnPeriod;
	public float spawnDistance;

	private GameObject m_player;
	private GameManager m_gameManager;
    private EnvironmentController m_environment;
    private Coroutine m_coroutine;

    private class SpawnLocationException : UnityException
	{
	}

	void Start ()
	{
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;

        m_player = GameObject.FindGameObjectWithTag ("Player");
        m_environment = FindObjectOfType<EnvironmentController>();
	}
    
	void OnDestroy ()
	{
		m_gameManager.NotifyStateChange -= OnStateChange;
	}


	void OnStateChange (GameManager.GameState old_state, GameManager.GameState new_state)
	{
		if (new_state == GameManager.GameState.Moving) {
			m_coroutine = StartCoroutine (SpawnEnemies());
		} else if (old_state == GameManager.GameState.Moving) {
			StopCoroutine (m_coroutine);
		}
	}

	IEnumerator SpawnEnemies ()
	{
		while (m_player != null) {
			foreach (SpawnSpecification spawn_specification in SpawnSpecifications)
            {
				GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag (spawn_specification.EnemyType.tag);
				if (existingEnemies.Length < spawn_specification.MaxToSpawn) {
					try {
						Vector3 spawn_pos = getSpawnLocation ();
                        for (int i = 0; i < spawn_specification.GroupSize; ++i)
                        {
                            Vector3 deviation;
                            if (i == 0)
                            {
                                deviation = Vector3.zero;
                            }
                            else
                            {
                                float deviation_degrees = (i - 1) * (360 / (spawn_specification.GroupSize - 1));
                                deviation = Quaternion.AngleAxis(deviation_degrees, Vector3.up) * Vector3.forward;
                            }
                            
                            Instantiate(spawn_specification.EnemyType, spawn_pos + deviation, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                        }
						
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
			spawn_pos = new Vector3 (spawn_direction.x, 0, spawn_direction.y) + m_player.transform.position;

			if (m_environment.PointIsInBounds(spawn_pos) && !Physics.CheckSphere (spawn_pos, 1.0f, layer_mask)) {
				//If there's nothing else near the spawn location, great. Return this location.
				return spawn_pos;
			}

		}

		//If we've failed to find anywhere suitable to spawn, throw an exception.
		throw new SpawnLocationException ();
	}


}
