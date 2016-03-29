using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Planning,
        MoveCountdown,
        Moving
    }

    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }

    public delegate void OnStateChange(GameState old_state, GameState new_state);
    public event OnStateChange NotifyStateChange;
        

    //Awake is always called before any Start functions
    void Awake()
    {
        //Enforce Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        State = GameState.Planning;
    }

    // Use this for initialization
    void Start()
    {
        //Watch the player so that we can end the scene if they die.
        GameObject.Find("Player").GetComponent<Destructible>().OnDeath += onPlayerDeath;
        GameObject.FindObjectOfType<ExitController>().OnExit += onPlayerVictory;
    }

    void OnDestroy()
    {
        Instance = null;
    }

    private void onPlayerDeath()
    {
        SceneManager.LoadScene("Menu");
    }

    private void onPlayerVictory()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SetGameState (GameState gameState)
	{
		if (NotifyStateChange != null) {
			NotifyStateChange (this.State, gameState);
		}
		this.State = gameState;
	}
}