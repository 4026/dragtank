using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Class that tracks playthrough stats from a single game.
/// </summary>
public class GameStats
{
    public enum Flag
    {
        ExitReached
    }

    public enum IntStatistic
    {
        CubesCollected,
        ShotsFired,
        ShotsHit,
        TotalKills,
        TankKills,
        DroneKills
    }

    public enum Timer
    {
        Total,
        Planning,
        Moving
    }

    private Dictionary<Flag, bool> m_flags = new Dictionary<Flag, bool>();
    private Dictionary<IntStatistic, int> m_intStats = new Dictionary<IntStatistic, int>();

    private Dictionary<Timer, float> m_timerValues = new Dictionary<Timer, float>();
    private Dictionary<Timer, float> m_timerStarts = new Dictionary<Timer, float>();

    /// <summary>
    /// Increase the value of the specified integer statistic by the specified amount.
    /// </summary>
    public void Increment(IntStatistic stat, int amount = 1)
    {
        if (!m_intStats.ContainsKey(stat))
        {
            m_intStats[stat] = amount;
        }
        else
        {
            m_intStats[stat] += amount;
        }
    }

    /// <summary>
    /// Set the value of the specified flag.
    /// </summary>
    public void SetFlag(Flag flag, bool value = true)
    {
        m_flags[flag] = value;
    }

    /// <summary>
    /// Start a timer running, or reset a running timer.
    /// </summary>
    public void StartTimer(Timer timer)
    {
        m_timerStarts[timer] = Time.time;
    }

    /// <summary>
    /// Stop a running timer, adding any elapsed time to the running total for that timer. 
    /// </summary>
    public void StopTimer(Timer timer)
    {
        if (!m_timerValues.ContainsKey(timer))
        {
            m_timerValues[timer] = Time.time - m_timerStarts[timer];
        }
        else
        {
            m_timerValues[timer] += Time.time - m_timerStarts[timer];
        }

        m_timerStarts.Remove(timer);
    }

    /// <summary>
    /// Get the value of the supplied statistic.
    /// </summary>
    public int Get(IntStatistic stat)
    {
        return m_intStats.ContainsKey(stat) ? m_intStats[stat] : 0;
    }

    /// <summary>
    /// Get the status of the supplied flag.
    /// </summary>
    public bool Get(Flag flag)
    {
        return m_flags.ContainsKey(flag) ? m_flags[flag] : false;
    }

    /// <summary>
    /// Get the number of seconds accumulated by the specified timer.
    /// </summary>
    public float Get(Timer timer)
    {
        return m_timerValues.ContainsKey(timer) ? m_timerValues[timer] : 0f;
    }
}