using UnityEngine;

public class DeactivateOnFinished : MonoBehaviour
{
    public void Finished()
    {
        if (Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
