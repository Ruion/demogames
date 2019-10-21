using UnityEngine;
using UnityEngine.Events;

public class SwipeDetector : MonoBehaviour
{
    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;

    [Header("Gameplay Config")]
    public float turnTime = .6f;
    public float horizontalThresholdSwipe = .06f;
    public float horizontalDistance;

    private Vector3 mouseDownPosition;
    private Vector3 mouseUpPosition;
    private bool finishTurn;

    private float xDistance;

    public bool allowControl = true;
    public bool executeOnce = true;


    void Start()
    {
        finishTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!allowControl) return;

        if (finishTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = Input.mousePosition; //Get mouse down position
            }

            if (Input.GetMouseButtonUp(0))
            {

                mouseUpPosition = Input.mousePosition; //Get mouse position

                xDistance = (mouseDownPosition.x - mouseUpPosition.x) / Screen.width; //Caculate the distance between them

                // Side swiping
                if (Mathf.Abs(xDistance) > horizontalThresholdSwipe && mouseDownPosition.x != 0)
                {
                    if (xDistance < 0) // Right
                    {
                        TurnRight();

                    }
                    else // Left
                    {
                        TurnLeft();
                    }
                }
            }
        }
    }

    void TurnRight()
    {

        finishTurn = false;

        OnSwipeRight.Invoke();

        finishTurn = true;

        if (executeOnce)
        {
            enabled = false;
        }
    }

    void TurnLeft()
    {
        finishTurn = false;

        OnSwipeLeft.Invoke();

        finishTurn = true;

        if (executeOnce)
        {
            enabled = false;
        }
    }


    public void DisableControl()
    {
        allowControl = false;
    }
}
