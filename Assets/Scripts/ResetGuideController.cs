using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetGuideController : MonoBehaviour, IPointerClickHandler
{
    private GameObject m_player;
    private PathPlanner m_pathPlanner;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.NotifyStateChange += OnStateChange;
    }

    void Start () {
        m_pathPlanner = transform.parent.GetComponent<PathPlanner>();
        m_player = GameObject.Find("Player");
    }

    void OnDestroy()
    {
        gameManager.NotifyStateChange -= OnStateChange;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_pathPlanner.ClearWaypoints();
    }

    void OnStateChange(GameState old_state, GameState new_state)
    {
        switch (new_state)
        {
            case GameState.Moving:
                GetComponent<SpriteRenderer>().enabled = false;
                break;


            case GameState.Planning:
                transform.position = m_player.transform.position;
                GetComponent<SpriteRenderer>().enabled = true;
                break;
        }
    }
}
