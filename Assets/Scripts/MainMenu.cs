using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName;


    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("I'm Quitting");
    }

    // Hook this up to a button in the main menu scene to reset saved data (gold, purchased upgrades, high score)
    public void ResetData()
    {
        SaveSystem.ResetData();
        Debug.Log("Save data reset");
    }
}
