using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuBehaviour : MonoBehaviour
{
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
