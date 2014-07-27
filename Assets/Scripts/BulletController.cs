using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour
{

    public Vector3 target;
    public float speed;
	
    // Update is called once per frame
    void Update()
    {
        Vector3 to_target = target - transform.position;

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
