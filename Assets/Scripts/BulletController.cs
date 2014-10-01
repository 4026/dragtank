using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{
    public Vector3 target;
    public float speed;

    private GameManager gameManager;
    private float trail_fade_time;

    void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.NotifyStateChange += OnStateChange;
    }

    void Start()
    {
        trail_fade_time = GetComponent<TrailRenderer>().time;
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
                GetComponent<TrailRenderer>().time = trail_fade_time; //Resume trail renderer fade-out
                break;
            case GameState.Planning:
                GetComponent<TrailRenderer>().time = Mathf.Infinity; //Pause trail renderer fade-out
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 to_target = target - transform.position;

        if (GameManager.Instance.gameState != GameState.Moving)
        {
            return;
        }

        if (to_target.magnitude < speed * Time.deltaTime)
        {
            transform.position = target;
            Destroy(this.gameObject);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, target, (speed / to_target.magnitude) * Time.deltaTime);
        }

    }
}
