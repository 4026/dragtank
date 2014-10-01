using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour
{
    public PathPlanner path_planner;

    private GameManager gameManager;
    
    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void OnGUI()
    {
        switch (gameManager.gameState)
        {
            case GameState.Planning:
                if (path_planner.dragged_path.Count > 0)
                {
                    int button_width = (int)Mathf.Min(Screen.width, Screen.height) / 10;
                    if (GUI.Button(new Rect(20, 20, button_width, button_width), "Go"))
                    {
                        gameManager.SetGameState(GameState.Moving);
                    }
                }
                break;
                
            case GameState.Moving:
                break;
        }
    }

}
