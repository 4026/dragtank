using com.kleberswf.lib.core;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Prefab("Prefabs/Singletons/Director")]
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Enum of possible game states.
    /// </summary>
    public enum GameState
    {
        SceneStarting,
        PlanCountdown,
        Planning,
        MoveCountdown,
        Moving,
        SceneEnding
    }

    public float MoveCountdownDuration;
    public float PlanningCountdownDuration;
    public float SceneEndDuration;

    /// <summary>
    /// The current state of the game.
    /// </summary>
    public GameState State {
        get { return m_state;  }
        set
        {
            if (NotifyStateChange != null)
            {
                try
                {
                    NotifyStateChange(this.State, value);
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            }
            m_state = value;
        }
    }
    private GameState m_state = GameState.SceneStarting;

    /// <summary>
    /// An event that notifies subscribers of a change in game state.
    /// </summary>
    public event OnStateChange NotifyStateChange;
    public delegate void OnStateChange(GameState old_state, GameState new_state);

    /// <summary>
    /// An event that notifies subscribers of the player object spawning.
    /// </summary>
    public event PlayerSpawnEvent NotifyPlayerSpawn;
    public delegate void PlayerSpawnEvent(GameObject player);

    public void OnPlayerSpawn(GameObject player)
    {
        //Watch the player so that we can end the scene if they die.
        player.GetComponent<Destructible>().OnDeath += OnPlayerDeath;

        if (NotifyPlayerSpawn != null)
        {
            NotifyPlayerSpawn(player);
        }
    }

    public void OnPlayerDeath()
    {
        State = GameState.SceneEnding;
    }

    public void OnSceneEndComplete()
    {
        SceneManager.LoadScene("Menu");
    }
}