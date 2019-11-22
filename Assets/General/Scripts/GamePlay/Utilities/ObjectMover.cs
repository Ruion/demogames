using UnityEngine;

/// <summary>
/// Move towars a target transform position
/// Tips: Attach to a gameObject and drag target transform into "target" field, set your desire speed
/// Notes: component depends on GameManager.isGameEnded state, it will stop moving and disable itself
/// if GameManager.isGameEnded is true
/// </summary>
public class ObjectMover : MonoBehaviour {

    // Adjust the speed for the application.
    public float speed = 1.5f;

    // The target (cylinder) position.
    public Transform target;

    [HideInInspector]
    public GameManager GM;

    void Update()
    {
        if(GM == null)
        {
            GM = FindObjectOfType<GameManager>();
        }

        if (GM.isGameEnded)
        {
            enabled = false;
            return;
        }

        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            // destroy unseen object 
            Destroy(transform.parent.gameObject);
        }

        
    }
}

