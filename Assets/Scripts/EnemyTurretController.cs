using UnityEngine;
using System.Collections;

public class EnemyTurretController : Turret
{

	public float Range;
	public float FieldOfView;

	public bool IsDormant { get; set; }

	private GameManager gameManager;
	private GameObject player;                  //The player object that is our target.

	void Awake ()
	{
		gameManager = GameManager.Instance;
		IsDormant = true;
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

		//If the turret is active and isn't still shaking from the last shot, turn it.
		if (!IsDormant && !animating) {
			//If the player is in vision, rotate to face them.
			if (IsPlayerInVision ()) {
				if (turnToward (player.transform.position)) {
					fireAt (player);
				}
			} else {
				//If no target, turn to face front of tank.
				turnToward (transform.parent.position + transform.parent.forward);
			}
		}
	}

	public bool IsPlayerInVision ()
	{
		Vector3 direction_to_player = player.transform.position - transform.position;
		if (direction_to_player.magnitude > Range) {
			return false; //Player is too far away to cause aggro.
		}
		
		float angle_to_player = Vector3.Angle (direction_to_player, transform.up);
		if (angle_to_player > FieldOfView / 2) {
			return false; //Player is outside our FoV.
		}
		
		if (Physics.Raycast (transform.position, direction_to_player, direction_to_player.magnitude, LayerMask.GetMask ("Walls"))) {
			return false; //Player is behind a wall.
		}

		return true;
	}
}
