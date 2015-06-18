using UnityEngine;
using System.Collections;

public class EnemyTurretController : Turret {

    public float Range;

    private GameManager gameManager;
    private GameObject player;                  //The player object that is our target.


    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public void Start ()
    {
        player = GameObject.FindGameObjectWithTag ("Player");
    }
    
    void Update () 
    {
        if (gameManager.gameState != GameState.Moving) {
            return;
        }

        //If the turret isn't still shaking from the last shot, turn it.
        if (!animating)
        {
            //If the player is in range, rotate to face them.
            Debug.Log(Vector3.Distance(player.transform.position, transform.position));
            if (Vector3.Distance(player.transform.position, transform.position) <= Range) {
                if (turnToward(player.transform.position))
                {
                    fireAt(player);
                }
            } else {
                //If no target, turn to face front of tank.
                turnToward(transform.parent.position + transform.parent.forward);
            }
        }
    }
}
