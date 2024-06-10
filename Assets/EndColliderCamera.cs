using UnityEngine;
using System.Collections;

public class DirectionalTransitionCollider : MonoBehaviour
{
    public float transitionAmountX = 39f; // Amount of transition in the specified X direction
    public float transitionAmountY = 18f; // Amount of transition in the specified Y direction
    public float transitionDuration = 7.0f; // Duration of the transition
    public GameObject toDuplicate; // The parent GameObject containing the background to duplicate
    public float cooldownTime = 2.0f; // Time to wait before the trigger can be activated again

    private Vector3 offset; // Offset to duplicate the background
    private static bool isOnCooldown = false; // Indicates if the trigger is on cooldown (shared between all instances)

    void Start()
    {
        // Calculate the offset based on the specified transition amount
        offset = new Vector3(transitionAmountX, transitionAmountY, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with the directional transition collider and the cooldown is not active
        if (other.CompareTag("Player") && !isOnCooldown)
        {
            // Start the cooldown
            StartCoroutine(CooldownCoroutine());

            // Log a message to indicate that the player collided with the collider
            Debug.Log($"Player collided with {gameObject.name}. Initiating camera transition.");

            // Duplicate the background
            DuplicateBackground();
            
            // Calculate the target position based on the specified direction
            Vector3 targetPosition = Camera.main.transform.position + new Vector3(transitionAmountX, transitionAmountY, 0f);

            // Call the camera control function to initiate the transition
            Camera.main.GetComponent<CameraController>().SmoothTransition(targetPosition, transitionDuration);
        }
    }

    void DuplicateBackground()
    {
        // Instantiate a new background at the new position
        GameObject newBackground = Instantiate(toDuplicate, toDuplicate.transform.position + offset, toDuplicate.transform.rotation);

        // Log a message to indicate that the background has been duplicated
        Debug.Log("Background duplicated.");

        // Remove the old background after the camera transition duration
        Destroy(toDuplicate, (transitionDuration-4));

        // Update the reference to the duplicated background
        toDuplicate = newBackground;
    }

    IEnumerator CooldownCoroutine()
    {
        // Set the cooldown as active
        isOnCooldown = true;

        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);

        // Set the cooldown as inactive
        isOnCooldown = false;
    }
}
