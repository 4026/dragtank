using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//Events triggered for dragging
public delegate void OnStartDrag(Vector2 screen_pos);
public delegate void OnDragUpdate(Vector2 screen_pos);
public delegate void OnStopDrag(Vector2 screen_pos);


public class InputController : MonoBehaviour
{
    public static event OnStartDrag NotifyStartDrag;
    public static event OnDragUpdate NotifyDragUpdate;
    public static event OnStopDrag NotifyStopDrag;
	
    void Update()
    {
        //Process touch / mouse input for dragging interactions. 

        #if UNITY_EDITOR
        Vector2 screen_pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
        if (Input.GetMouseButtonDown(0) && NotifyStartDrag != null)
        {
            NotifyStartDrag(screen_pos);
        }
        else if (Input.GetMouseButton(0) && NotifyDragUpdate != null)
        {
            NotifyDragUpdate(screen_pos);
        }
        else if (Input.GetMouseButtonUp(0) && NotifyStopDrag != null)
        {
            NotifyStopDrag(screen_pos);
        }
        #endif
		
		
        #if UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && NotifyStartDrag != null)
            {
                NotifyStartDrag(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && NotifyDragUpdate != null)
            {
                NotifyDragUpdate(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended && NotifyStopDrag != null)
            {
                NotifyStopDrag(touch.position);
            }
        }
        #endif
    }


}
