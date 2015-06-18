using UnityEngine;
using System.Collections;

public class EnemyTankController : MonoBehaviour {

    private GameManager gameManager;
    
    void Awake()
    {
        gameManager = GameManager.Instance;
    }

	
	void Update () 
    {
        if (gameManager.gameState != GameState.Moving) {
            return;
        }


	}
}
