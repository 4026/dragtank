using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurretController : MonoBehaviour {

    public GameObject player;
    public float turnSpeed;

    private List<GameObject> targets = new List<GameObject>();
    private bool animating = false;

	void Update ()
    {

        //Follow tank around
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        if (targets.Count > 0 && !animating)
        {
            GameObject target = targets.First();

            Vector3 target_from_self = target.transform.position - transform.position;

            float bearing = Vector3.Angle(transform.up, target_from_self);

            //Create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(target_from_self.normalized, Vector3.up) * Quaternion.Euler(90, 0, 0);


            if (Mathf.Abs(bearing) < Time.deltaTime * turnSpeed)
            {
                transform.rotation = lookRotation;
                fireAt(target);
            }
            else
            {
                //rotate us over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (turnSpeed / bearing));
            }

            

            

        }

	}

    public void addTarget(GameObject target)
    {
        targets.Add(target);
    }

    private void fireAt(GameObject target)
    {
        animating = false;
        targets.Remove(target);
        GameObject.Destroy(target);
    }
}
