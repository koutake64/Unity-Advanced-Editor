using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField, SceneSelector] string sceneName;

    public void NextSceneChange()
    {
        SceneManager.LoadScene(sceneName);
    }
}
