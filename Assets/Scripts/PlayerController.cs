using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public PathPlanner pathPlanner;

    private GameManager gameManager;
    private float trail_fade_time;
    
    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.NotifyStateChange += OnStateChange;
    }

    void Start()
    {
        trail_fade_time = GetComponentInChildren<TrailRenderer>().time;
    }

    void OnDestroy()
    {
        gameManager.NotifyStateChange -= OnStateChange;
    }

    void OnStateChange(GameState old_state, GameState new_state)
    {
        switch (new_state)
        {
            case GameState.Moving:
                GetComponentInChildren<TrailRenderer>().time = trail_fade_time; //Resume trail renderer fade-out

                //Start moving along the path
                Hashtable itween_options = iTween.Hash(
                    "path", pathPlanner.dragged_path.ToArray(),
                    "orienttopath", true,
                    "axis", "y",
                    "speed", 1,
                    "easetype", iTween.EaseType.linear, 
                    "oncompletetarget", this.gameObject,
                    "oncomplete", "moveComplete"
                );
                iTween.MoveTo(gameObject, itween_options);
                break;


            case GameState.Planning:
                GetComponentInChildren<TrailRenderer>().time = Mathf.Infinity; //Pause trail renderer fade-out
                break;
        }
    }

    //Called when the tank reaches the end of its path.
    public void moveComplete()
    {
        pathPlanner.dragged_path.Clear();
        gameManager.SetGameState(GameState.Planning);
    }
}
