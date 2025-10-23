using UnityEngine;

public class GlowLoopColorCycle : MonoBehaviour
{
    [Header("Glow Settings")]
    public float minIntensity = 0.2f;     // Minimum glow strength
    public float maxIntensity = 2f;       // Maximum glow strength
    public float glowSpeed = 2f;          // Speed of glow/dim cycle

    private Material mat;
    private float glowTimer = 0f;
    private int colorIndex = 0;

    private Color[] colors = { Color.red, Color.blue, Color.green };
    private Color currentColor;
    private Color nextColor;

    private bool glowingUp = true;

    void Start()
    {
        // Get material instance to avoid editing shared material
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");

        // Initialize first color
        currentColor = colors[colorIndex];
        nextColor = colors[(colorIndex + 1) % colors.Length];
    }

    void Update()
    {
        // Timer oscillates between 0 and 1
        glowTimer = Mathf.PingPong(Time.time * glowSpeed, 1f);

        // Determine intensity
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, glowTimer);

        // Determine glow direction (up or down)
        bool currentlyGlowingUp = glowTimer > 0.5f;
        if (currentlyGlowingUp != glowingUp)
        {
            glowingUp = currentlyGlowingUp;

            // When glow completes a full up–down cycle, switch to next color
            if (!glowingUp)
            {
                colorIndex = (colorIndex + 1) % colors.Length;
                currentColor = colors[colorIndex];
                nextColor = colors[(colorIndex + 1) % colors.Length];
            }
        }

        // Blend emission color toward the next color smoothly
        Color emissionColor = Color.Lerp(currentColor, nextColor, glowTimer);
        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }

    void OnDisable()
    {
        // Reset emission when disabled
        if (mat != null)
            mat.SetColor("_EmissionColor", Color.black);
    }
}
