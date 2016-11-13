using com.kleberswf.lib.core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton to which other classes report events so that they can be recorded in the stats for this session.
/// </summary>
[Prefab("Prefabs/Singletons/Stats Tracker")]
public class StatsTracker : Singleton<StatsTracker>
{
    public GameStats CurrentGame { get { return m_currentGameStats; } }
    private GameStats m_currentGameStats = new GameStats();
	
    /// <summary>
    /// Clear the stats from the last game, and prepare to record stats for a new game.
    /// </summary>
    public void Clear()
    {
        m_currentGameStats = new GameStats();
    }
}
