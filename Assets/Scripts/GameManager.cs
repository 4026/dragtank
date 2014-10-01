using UnityEngine;

public enum GameState
{
    Planning,
    Moving
}
public delegate void OnStateChange(GameState old_state,GameState new_state);

public class GameManager
{
    
    private static GameManager _instance = null;    
    public event OnStateChange NotifyStateChange;
    public GameState gameState { get; private set; }

    protected GameManager()
    {
        //Protected constructor
        gameState = GameState.Planning;
    }
    
    // Singleton pattern implementation
    public static GameManager Instance
    { 
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager(); 
            }  
            return _instance;
        } 
    }
    
    public void SetGameState(GameState gameState)
    {
        if (NotifyStateChange != null)
        {
            NotifyStateChange(this.gameState, gameState);
        }
        this.gameState = gameState;
    }
}