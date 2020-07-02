using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Move towars a target transform position
/// Tips: Attach to a gameObject and drag target transform into "target" field, set your desire speed
/// Notes: component depends on GameManager.isGameEnded state, it will stop moving and disable itself
/// if GameManager.isGameEnded is true
/// </summary>
public class ObjectMover : MonoBehaviour
{
    // Adjust the speed for the application.
    public float speed = 5f;

    public float speedMultipier = 1f;

    public Vector3 direction;

    public Transform target;
    public float minDistance = 0.01f;

    private void Update()
    {
        float step = speed * Time.deltaTime * speedMultipier;

        if (target == null)
        {
            // Move our position a step closer to the target.
            // calculate distance to move
            transform.Translate(direction * step);
        }
        else
        {
            if ((transform.position - target.position).sqrMagnitude > minDistance * minDistance)
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * step);
        }
    }
}