using UnityEngine;
using System.Collections;

public class EnvironmentController : MonoBehaviour
{
    public Rect Bounds;
    	
	public bool PointIsInBounds (Vector3 point)
    {
        return Bounds.Contains(new Vector2(point.x, point.z));
	}
}
