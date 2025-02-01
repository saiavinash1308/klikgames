using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class swipe_menu : MonoBehaviour
{
    public GameObject scrollbar;  // Reference to the scrollbar object
    private float scroll_pos = 0; // Current scrollbar position
    private float[] pos;         // Positions of the menu items
    public float delayTime = 2f; // Delay time in seconds before auto-swiping
    private float timer = 0f;    // Timer to track elapsed time
    private int currentIndex = 0; // The current index of the displayed menu item

    void Start()
    {
        // Initialize positions of the menu items based on the number of child elements
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
    }

    void Update()
    {
        // Check if the user is clicking and interacting with the scrollbar
        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
            timer = 0f; // Reset the timer if the user manually interacts
        }
        else
        {
            // Increment the timer
            timer += Time.deltaTime;

            // If the timer exceeds the delay time, automatically swipe to the next item
            if (timer >= delayTime)
            {
                timer = 0f; // Reset the timer
                currentIndex = (currentIndex + 1) % pos.Length; // Move to the next item (looping back to 0 after the last item)
                scrollbar.GetComponent<Scrollbar>().value = pos[currentIndex]; // Set the scrollbar position to the next item
            }
        }

        // Get the current scroll position directly from the scrollbar
        scroll_pos = scrollbar.GetComponent<Scrollbar>().value;

        // Auto-scaling of menu items based on the current scroll position
        for (int i = 0; i < pos.Length; i++)
        {
            // Check if the scroll position is within range for the current item
            if (scroll_pos < pos[i] + (1f / (pos.Length - 1f)) / 2 && scroll_pos > pos[i] - (1f / (pos.Length - 1f)) / 2)
            {
                // Scale the current item to its full size
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                for (int a = 0; a < pos.Length; a++)
                {
                    if (a != i)
                    {
                        // Scale down the other items
                        transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                    }
                }
            }
        }
    }
}
