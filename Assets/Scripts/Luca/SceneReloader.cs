using UnityEngine;

using UnityEngine.SceneManagement;
public class SceneReloader : MonoBehaviour
{

    [SerializeField] AudioClip buttonSFX;


    public void Quit()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSFX, transform, 1f);
        Application.Quit();
    }

    public void StartGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(buttonSFX, transform, 1f);
        SceneManager.LoadScene(0);

    }
}
