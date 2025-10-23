using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerHealthBarDual : MonoBehaviour
{
    [Header("Health Bar References")]
    public Slider healthSlider;       // UI Slider for health
    public playerHealth player;       // Reference to player's health
    public float maxHealth = 25f;     // Max health value

    [Header("Result UI References")]
    public Canvas currentCanvas;      // Main in-game HUD canvas
    public Canvas resultCanvas;       // Win/Lose canvas
    public RawImage winImage;         // Win image
    public RawImage loseImage;        // Lose image
    public Button backButton;         // Back to menu
    public Button tryAgainButton;     // Restart level

    [Header("Hover Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.8f, 0.4f); // soft yellow glow
    public float hoverScale = 1.1f;
    public float transitionSpeed = 8f;

    private bool hasEnded = false;
    private Vector3 backBtnOriginalScale;
    private Vector3 tryBtnOriginalScale;
    private Image backBtnImage;
    private Image tryBtnImage;

    void Start()
    {
        if (!healthSlider || !player)
        {
            Debug.LogError("Missing references in PlayerHealthBarDual!");
            enabled = false;
            return;
        }

        // Setup slider
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;
        healthSlider.value = 1f;

        if (resultCanvas) resultCanvas.gameObject.SetActive(false);

        if (backButton)
        {
            backButton.onClick.AddListener(OnBackToMenu);
            backBtnOriginalScale = backButton.transform.localScale;
            backBtnImage = backButton.GetComponent<Image>();
            AddHoverEvents(backButton);
        }

        if (tryAgainButton)
        {
            tryAgainButton.onClick.AddListener(OnTryAgain);
            tryBtnOriginalScale = tryAgainButton.transform.localScale;
            tryBtnImage = tryAgainButton.GetComponent<Image>();
            AddHoverEvents(tryAgainButton);
        }

        // Hide cursor during gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (hasEnded) return;

        float normalized = Mathf.Clamp01(player.health / maxHealth);
        healthSlider.value = normalized;

        // Check for lose condition
        if (player.health <= 0)
        {
            ShowResult(false);
        }
    }

    public void ShowResult(bool won)
    {
        if (hasEnded) return;
        hasEnded = true;

        // Disable gameplay canvas
        if (currentCanvas) currentCanvas.gameObject.SetActive(false);

        // Enable result canvas
        if (resultCanvas) resultCanvas.gameObject.SetActive(true);

        // Set win/lose state
        if (winImage) winImage.gameObject.SetActive(won);
        if (loseImage) loseImage.gameObject.SetActive(!won);

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // "Pause" logic — disable gameplay scripts but keep frame rate stable
        PauseGameplay(true);
    }

    private void PauseGameplay(bool pause)
    {
        var playerObj = player.gameObject;
        var playerComponents = playerObj.GetComponents<MonoBehaviour>();
        foreach (var comp in playerComponents)
        {
            if (comp != this)
                comp.enabled = !pause;
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            foreach (var comp in enemy.GetComponents<MonoBehaviour>())
            {
                comp.enabled = !pause;
            }
        }
    }

    private void OnBackToMenu() => StartCoroutine(LoadSceneClean("mainMenu"));
    private void OnTryAgain() => StartCoroutine(LoadSceneClean(SceneManager.GetActiveScene().name));

    private IEnumerator LoadSceneClean(string sceneName)
    {
        Time.timeScale = 1f;
        hasEnded = false;
        yield return null;
        SceneManager.LoadScene(sceneName);
    }

    // -------------------------------
    // HOVER EFFECT HELPERS
    // -------------------------------
    private void AddHoverEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((_) => StartCoroutine(HoverButton(button, true)));
        trigger.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((_) => StartCoroutine(HoverButton(button, false)));
        trigger.triggers.Add(exit);
    }

    private IEnumerator HoverButton(Button button, bool isHover)
    {
        Image img = button.GetComponent<Image>();
        Vector3 targetScale = isHover ? button.transform.localScale * hoverScale :
                                        (button == backButton ? backBtnOriginalScale : tryBtnOriginalScale);
        Color targetColor = isHover ? hoverColor : normalColor;

        while (button && Vector3.Distance(button.transform.localScale, targetScale) > 0.01f)
        {
            if (img)
                img.color = Color.Lerp(img.color, targetColor, Time.unscaledDeltaTime * transitionSpeed);
            button.transform.localScale = Vector3.Lerp(button.transform.localScale, targetScale, Time.unscaledDeltaTime * transitionSpeed);
            yield return null;
        }

        if (img) img.color = targetColor;
        if (button) button.transform.localScale = targetScale;
    }
}
