using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurretController : MonoBehaviour
{

    public GameObject player;
    public float turnSpeed;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public GameObject targetPrefab;

    private List<GameObject> targets = new List<GameObject>();
    private GameObject dragged_target;
    private bool animating = false;
    private GameObject current_target;
    private bool is_dragging = false;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;

        InputController.NotifyStartDrag += StartDrag;
        InputController.NotifyDragUpdate += UpdateDrag;
        InputController.NotifyStopDrag += EndDrag;
    }

    void OnDestroy()
    {
        InputController.NotifyStartDrag -= StartDrag;
        InputController.NotifyDragUpdate -= UpdateDrag;
        InputController.NotifyStopDrag -= EndDrag;
    }

    void Update()
    {
        if (gameManager.gameState != GameState.Moving)
        {
            return;
        }

        //Follow tank around
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if (!animating)
        {
            //If the turret isn't still shaking from the last shot, turn it.

            if (targets.Count > 0 || dragged_target != null)
            {
                //If we have an active target, rotate to face it.
                GameObject target = (targets.Count > 0) ? targets.First() : dragged_target;
                
                if (turnToward(target.transform.position) && targets.Count > 0)
                {
                    fireAt(target);
                }
            }
            else
            {
                //If no target, turn to face front of tank.
                turnToward(player.transform.position + player.transform.up);
            }
        }
    }

    void StartDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Moving)
        {
            return;
        }

        is_dragging = true;

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target = Instantiate(targetPrefab, world_pos, Quaternion.AngleAxis(90, Vector3.right)) as GameObject;
        
        dragged_target = current_target;
    }
    
    void UpdateDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Moving || !is_dragging)
        {
            return;
        }

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target.transform.position = world_pos;
    }
    
    void EndDrag(Vector2 screen_pos)
    {
        if (gameManager.gameState != GameState.Moving || !is_dragging)
        {
            return;
        }

        is_dragging = false;

        Vector3 world_pos = Camera.main.ScreenToWorldPoint(new Vector3(screen_pos.x, screen_pos.y, Camera.main.transform.position.y));
        current_target.transform.position = world_pos;
        
        dragged_target = null;
        targets.Add(current_target);
    }


    /// <summary>
    /// Makes the turret rotate to point at the specified position.
    /// </summary>
    /// <returns><c>true</c>, if turret is pointing at the position after the rotation, <c>false</c> otherwise.</returns>
    /// <param name="position">Position.</param>
    private bool turnToward(Vector3 target_position)
    {
        Vector3 target_from_self = target_position - transform.position;
        float bearing = Vector3.Angle(transform.up, target_from_self);
        
        //Create the rotation we need to be in to look at the target
        Quaternion lookRotation = Quaternion.LookRotation(target_from_self.normalized, Vector3.up) * Quaternion.Euler(90, 0, 0);
        
        if (Mathf.Abs(bearing) < Time.deltaTime * turnSpeed)
        {
            transform.rotation = lookRotation;
            return true;
        }
        else
        {
            //Rotate over time according to speed until we are in the required rotation.
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (turnSpeed / bearing));
            return false;
        }
    }

    private void fireAt(GameObject target)
    {
        //Spawn bullet
        GameObject new_bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        new_bullet.GetComponent<BulletController>().target = target.transform.position;
        new_bullet.GetComponent<BulletController>().speed = bulletSpeed;

        //Animate recoil
        animating = true;
        Vector3 recoil_pos = Vector3.down;
        Hashtable itween_options = iTween.Hash(
            "amount", recoil_pos,
            "time", 0.5f,
            "oncomplete", "animationEnd"
        );
        iTween.PunchPosition(this.gameObject, itween_options);

        //Remove target indicator
        targets.Remove(target);
        GameObject.Destroy(target);
    }

    public void animationEnd()
    {
        animating = false;
    }
}
