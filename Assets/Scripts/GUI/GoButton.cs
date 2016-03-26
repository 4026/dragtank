using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GoButton : MonoBehaviour
{
    private GameObject m_button;
    private PathPlanner m_pathPlanner;
    private GameManager m_gameManager;

    private void Awake()
    {
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyStateChange += OnGameStateChange;
    }

    private void Start()
    {
        m_button = transform.FindChild("GoButton").gameObject;

        m_pathPlanner = FindObjectOfType<PathPlanner>();
        m_pathPlanner.OnPathChange += OnPathChange;
    }

    private void OnDestroy()
    {
        m_gameManager.NotifyStateChange -= OnGameStateChange;
        m_pathPlanner.OnPathChange -= OnPathChange;
    }

    public void OnClick()
    {
        m_gameManager.SetGameState(GameState.MoveCountdown);
    }

    private void OnGameStateChange(GameState old_state, GameState new_state)
    {
        switch (new_state)
        {
            case GameState.Planning:
                m_button.SetActive(true);
                m_button.GetComponent<Button>().interactable = false;
                break;

            case GameState.MoveCountdown:
                m_button.SetActive(false);
                m_button.GetComponent<Button>().interactable = false;
                break;
        }
    }

    private void OnPathChange()
    {
        if (m_pathPlanner.GetNumWaypoints() > 0) {
            m_button.GetComponent<Button>().interactable = true;
        } else {
            m_button.GetComponent<Button>().interactable = false;
        }
    }
}
