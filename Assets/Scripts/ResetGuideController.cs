using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetGuideController : MonoBehaviour, IPointerClickHandler
{
    private GameObject m_player;
    private PathPlanner m_pathPlanner;
    private GameManager m_gameManager;

    void Start ()
    {
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnStateChange;
        m_gameManager.NotifyPlayerSpawn += OnPlayerSpawn;

        m_pathPlanner = transform.parent.GetComponent<PathPlanner>();

        m_player = FindObjectOfType<PlayerController>().gameObject;
        transform.position = m_player.transform.position;
    }

    void OnDestroy()
    {
        m_gameManager.NotifyStateChange -= OnStateChange;
        m_gameManager.NotifyPlayerSpawn -= OnPlayerSpawn;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        m_pathPlanner.ClearWaypoints();
    }

    void OnPlayerSpawn(GameObject player)
    {
        m_player = player;
        transform.position = m_player.transform.position;
    }

    void OnStateChange(GameManager.GameState old_state, GameManager.GameState new_state)
    {
        switch (new_state)
        {
            case GameManager.GameState.Moving:
                GetComponent<SpriteRenderer>().enabled = false;
                break;


            case GameManager.GameState.Planning:
                transform.position = m_player.transform.position;
                GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
    }
}
