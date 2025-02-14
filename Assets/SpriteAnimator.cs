using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public List<Sprite> sprites; // List of sprites for the animation
    public float frameRate = 0.1f; // Time per frame in seconds
    public bool loop = true; // Should the animation loop?

    private Image imageComponent;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        // Get the Image component
        imageComponent = GetComponent<Image>();

        if (imageComponent == null)
        {
            Debug.LogError("No Image component found on this GameObject!");
        }

        if (sprites == null || sprites.Count == 0)
        {
            Debug.LogError("No sprites assigned to the ImageAnimator!");
        }
    }

    void Update()
    {
        if (sprites != null && sprites.Count > 0 && imageComponent != null)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // If the timer exceeds the frame rate, move to the next frame
            if (timer >= frameRate)
            {
                timer -= frameRate;
                currentFrame++;

                // Check if we should loop or stop the animation
                if (currentFrame >= sprites.Count)
                {
                    if (loop)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        enabled = false; // Stop updating if not looping
                        return;
                    }
                }

                // Update the image's sprite
                imageComponent.sprite = sprites[currentFrame];
            }
        }
    }
}
