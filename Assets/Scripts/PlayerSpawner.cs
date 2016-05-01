using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public float SpawnDelay;

    private float m_birthTime;
    private ParticleSystem m_particleSystem;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(SpawnPlayerAfter(SpawnDelay));
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    IEnumerator SpawnPlayerAfter(float delay)
    {
        GameObject player_prefab = Resources.Load<GameObject>("Prefabs/Player");
        yield return new WaitForSeconds(delay);
        
        Instantiate(player_prefab, transform.position, player_prefab.transform.rotation);
        GameManager.Instance.State = GameManager.GameState.PlanCountdown;
    }

	
	void Update ()
    {
        if (m_particleSystem.isStopped)
        {
            Destroy(gameObject);
        }
	}
}
