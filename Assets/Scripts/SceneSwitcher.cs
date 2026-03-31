using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] string dragonSceneName = "DragonScene";
    [SerializeField] string infinianSceneName = "InfinianScene";

    // Wired to the toggle Button.onClick in the Inspector.
    public void SwitchScene()
    {
        string current = SceneManager.GetActiveScene().name;
        string target = current == dragonSceneName ? infinianSceneName : dragonSceneName;
        SceneManager.LoadScene(target, LoadSceneMode.Single);
    }
}
