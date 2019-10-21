using UnityEngine;

public class ClickFeedback : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            animator.PlayInFixedTime("1");
        }
    }
}
