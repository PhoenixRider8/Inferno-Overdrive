using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSlider : MonoBehaviour
{
    [Header("References")]
    public Slider healthSlider;   // Assign in Inspector
    public playerHealth player;   // Reference to playerHealth script

    private float maxHealth;

    void Start()
    {
        if (player != null && healthSlider != null)
        {
            maxHealth = player.health;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = player.health;
        }
    }

    void Update()
    {
        if (player != null && healthSlider != null)
        {
            healthSlider.value = player.health;
        }
    }
}
