using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelController : MonoBehaviour
{
    private GameObject m_statisticPrefab;

	void Start ()
    {
        m_statisticPrefab = Resources.Load<GameObject>("Prefabs/GUI/Statistic");

        GameStats stats = StatsTracker.Instance.CurrentGame;

        AddStatistic(string.Format("Cubes collected: {0}", stats.Get(GameStats.IntStatistic.CubesCollected)));

        TimeSpan total_time = TimeSpan.FromSeconds(stats.Get(GameStats.Timer.Total));
        AddStatistic(string.Format("Time taken: {0:D2}m:{1:D2}s", total_time.Minutes, total_time.Seconds));

        float accuracy = stats.Get(GameStats.IntStatistic.ShotsHit) / (float) stats.Get(GameStats.IntStatistic.ShotsFired);
        AddStatistic(string.Format("Accuracy: {0:P1}", accuracy));


        AddStatistic(string.Format("Kills: {0}", stats.Get(GameStats.IntStatistic.TotalKills)));
        AddStatistic(string.Format(" - Tanks : {0}", stats.Get(GameStats.IntStatistic.TankKills)));
        AddStatistic(string.Format(" - Drones: {0}", stats.Get(GameStats.IntStatistic.DroneKills)));
    }
	

    void AddStatistic(string text)
    {
        GameObject stat = Instantiate(m_statisticPrefab);
        stat.GetComponent<Text>().text = text;
        stat.transform.SetParent(transform, false);
    }
}
