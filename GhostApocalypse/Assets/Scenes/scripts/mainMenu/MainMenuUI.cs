using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Buttons")]
    public GameObject startButton;
    public GameObject quitButton;

    [Header("Mode Buttons")]
    public GameObject trainingButton;
    public GameObject bossButton;
    public GameObject survivalButton;
    public GameObject backButton;

    void Start()
    {
        ShowMainButtons(true);
        ShowModeButtons(false);
    }

    // Called when Start is pressed
    public void OnStartButton()
    {
        ShowMainButtons(false);
        ShowModeButtons(true);
    }

    // Called when Back is pressed
    public void OnBackButton()
    {
        ShowModeButtons(false);
        ShowMainButtons(true);
    }

    // Called by Quit button
    public void OnQuitButton()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void ShowMainButtons(bool show)
    {
        startButton.SetActive(show);
        quitButton.SetActive(show);
    }

    private void ShowModeButtons(bool show)
    {
        trainingButton.SetActive(show);
        bossButton.SetActive(show);
        survivalButton.SetActive(show);
        backButton.SetActive(show);
    }

    // Example: Load game mode scene
    public void OnTrainingButton()
    {
        SceneManager.LoadScene("main");
    }

    public void OnBossButton()
    {
        SceneManager.LoadScene("main3");
    }

    public void OnSurvivalButton()
    {
        SceneManager.LoadScene("main2");
    }
}
