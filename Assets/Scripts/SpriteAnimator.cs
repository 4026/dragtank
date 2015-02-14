using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour
{

    public Sprite[] sprites;
    public float framesPerSecond;
    public bool loop;

    private SpriteRenderer spriteRenderer;
    private float start_time;

    void Start()
    {
        spriteRenderer = renderer as SpriteRenderer;
        start_time = Time.timeSinceLevelLoad;
    }

    void Update()
    {
        float running_time = (Time.timeSinceLevelLoad - start_time);
        if (!loop && running_time > sprites.Length / framesPerSecond)
        {
            //If we've been running long enough to play the entire animation once, and we're not set to loop, destroy the game object.
            Destroy(gameObject);
        }

        int index = (int)(running_time * framesPerSecond);
        index = index % sprites.Length;
        spriteRenderer.sprite = sprites [index];
    }
}
