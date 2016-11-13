using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebriefTitleController : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
		if (StatsTracker.Instance.CurrentGame.Get(GameStats.Flag.ExitReached))
        {
            GetComponent<Text>().text = "Mission Complete";
        }
        else
        {
            GetComponent<Text>().text = "Mission Failed";
        }
	}
}
