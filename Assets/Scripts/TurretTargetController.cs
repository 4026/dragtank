using System;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Component that forwards drag events from the backing plane to the player's turret controller, for setting targets while the game is 
/// in the movement phase.
/// </summary>
public class TurretTargetController : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameManager m_gameManager;
    private TurretController m_controlledTurret;

    void Start()
    {
        m_gameManager = GameManager.Instance;
        m_gameManager.NotifyPlayerSpawn += OnPlayerSpawn;
    }

    void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.NotifyPlayerSpawn -= OnPlayerSpawn;
        }
    }
    
    private void OnPlayerSpawn(GameObject player)
    {
        m_controlledTurret = player.GetComponentInChildren<TurretController>();
        player.GetComponent<Destructible>().OnDeath += OnPlayerDeath;
    }

    private void OnPlayerDeath()
    {
        m_controlledTurret = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_controlledTurret != null)
        {
            m_controlledTurret.StartTargetDrag(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_controlledTurret != null)
        {
            m_controlledTurret.UpdateTargetDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_controlledTurret != null)
        {
            m_controlledTurret.EndTargetDrag(eventData.position);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_controlledTurret != null)
        {
            m_controlledTurret.PlaceTarget(eventData.position);
        }
    }
}
