using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject Target;
	public float TargetFocusOffset;

	private GameManager gameManager;
	
	void Awake ()
	{
		gameManager = GameManager.Instance;
	}
		
	void Update ()
	{
		switch (gameManager.gameState) {
		case GameState.Planning:
			transform.position = new Vector3 (Target.transform.position.x, transform.position.y, Target.transform.position.z);
			break;
		case GameState.Moving:
			Vector3 focusPos = Target.transform.position + (Target.transform.up * TargetFocusOffset);
			focusPos.y = transform.position.y;

			iTween.MoveUpdate (gameObject, focusPos, 1.0f);
			iTween.RotateUpdate (gameObject, Target.transform.rotation.eulerAngles, 1.0f);
                //transform.position = new Vector3(focusPos.x, transform.position.y, focusPos.z);
                //transform.rotation = Target.transform.rotation;
			break;
		}
	}
}
