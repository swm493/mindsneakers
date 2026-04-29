using UnityEngine;

public class EnterTrigger : MonoBehaviour
{
    Collider2D col = null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        col = other;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == col)
        {
            col = null;
        }
    }

    public Collider2D GetCollider()
    {
        return col;
    }
}
