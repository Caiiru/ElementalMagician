using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target; // Target to follow (e.g., player)
    private Vector3 targetPosition; // Target position for smooth transition
    private bool isTransitioning = false; // Flag to indicate if a transition is in progress
    private float transitionStartTime; // Time when the transition started
    private float transitionDuration; // Duration of the transition

    public void SmoothTransition(Vector3 position, float duration)
    {
        // Set target position and transition parameters
        targetPosition = position;
        transitionDuration = duration;
        transitionStartTime = Time.time;
        isTransitioning = true;
    }

    void Update()
    {
        // Check if a transition is in progress
        if (isTransitioning)
        {
            // Calculate the interpolation factor based on the elapsed time
            float t = Mathf.Clamp01((Time.time - transitionStartTime) / transitionDuration);

            // Interpolate between the current position and the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, t);

            // Check if the transition is complete
            if (t >= 1.0f)
            {
                // End the transition
                isTransitioning = false;
            }
        }
    }
}
