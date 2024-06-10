using UnityEngine;

public class DirectionalTransitionCollider : MonoBehaviour
{
    public float transitionAmountX = 39f; // Amount of transition in the specified X direction
    public float transitionAmountY = 18f; // Amount of transition in the specified Y direction

    public float transitionDuration = 1.0f; // Duration of the transition

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with the directional transition collider
        if (other.CompareTag("Player"))
        {
            // Log a message to indicate that the player collided with the collider
            Debug.Log($"Player collided with {gameObject.name}. Initiating camera transition.");

            // Calculate the target position based on the specified direction
            Vector3 targetPosition = Camera.main.transform.position + new Vector3(transitionAmountX, transitionAmountY, 0f);
            Debug.Log(targetPosition);

            // Call the camera control function to initiate the transition
            //Camera.main.GetComponent<CameraController>().SmoothTransition(targetPosition, transitionDuration);
        }
    }
}
