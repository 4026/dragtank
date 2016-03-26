using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetGuideController : MonoBehaviour, IPointerClickHandler
{
    private GameObject m_player;
    private PathPlanner m_pathPlanner;
    private GameManager gameManager;

    void Start ()
    {
        gameManager = GameManager.Instance;
        gameManager.NotifyStateChange += OnStateChange;

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
