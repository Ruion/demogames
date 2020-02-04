using UnityEngine;

public class PuzzleInteraction : MonoBehaviour
{
    public SoundManager sm;
    public ParticleSystem ps;

    public void Interact(Transform puzzleTransform)
    {
        sm.AddScore();
        ps.transform.position = puzzleTransform.position;
        ps.Play();
    }
}
