using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseStatusIndicator : MonoBehaviour
{
    private DefenseActivator m_defenseActivator;

	// Use this for initialization
	void Start ()
    {
        GameManager.Instance.NotifyPlayerSpawn += OnPlayerSpawn;
    }

    void OnPlayerSpawn(GameObject player)
    {
        m_defenseActivator = player.GetComponentInChildren<DefenseActivator>();
        player.GetComponent<Destructible>().OnDeath += OnPlayerDeath;

        GameManager.Instance.NotifyPlayerSpawn -= OnPlayerSpawn;
    }

    private void OnPlayerDeath()
    {
        m_defenseActivator = null;
    }

    void Update ()
    {
	    if (m_defenseActivator == null)
        {
            return;
        }

        GetComponent<Text>().text = m_defenseActivator.CooldownRemaining > 0 ? m_defenseActivator.CooldownRemaining.ToString("F1") : "";
    }
}
