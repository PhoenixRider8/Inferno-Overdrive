using UnityEngine;

public class MultiMaterialFrameAnimator : MonoBehaviour
{
    [System.Serializable]
    public class MaterialAnimation
    {
        [Header("Assign Material Slot Index (0 = first material)")]
        public int materialIndex;

        [Header("Frames (drag all PNGs for this material)")]
        public Texture2D[] frames;

        [Header("Playback Settings")]
        public float framesPerSecond = 24f;
        public bool loop = true;

        [Header("Glow (use texture as emission)")]
        public bool useTextureAsEmission = true;
        [Range(0f, 5f)] public float emissionStrength = 1f;

        [HideInInspector] public int currentFrame;
        [HideInInspector] public float timer;
    }

    [Header("Material Animations")]
    public MaterialAnimation[] materialsToAnimate;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (!rend)
        {
            Debug.LogError("❌ No Renderer found on this object!");
            return;
        }

        var mats = rend.materials;

        foreach (var anim in materialsToAnimate)
        {
            if (anim.materialIndex < mats.Length && anim.frames.Length > 0)
            {
                var mat = mats[anim.materialIndex];
                mat.mainTexture = anim.frames[0];

                if (anim.useTextureAsEmission)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetTexture("_EmissionMap", anim.frames[0]);
                    mat.SetColor("_EmissionColor", Color.white * anim.emissionStrength);
                }
            }
        }

        rend.materials = mats;
    }

    void Update()
    {
        if (!rend) return;

        var mats = rend.materials;

        foreach (var anim in materialsToAnimate)
        {
            if (anim.frames.Length == 0) continue;

            anim.timer += Time.deltaTime;
            int frameIndex = (int)(anim.timer * anim.framesPerSecond);

            if (anim.loop)
                frameIndex %= anim.frames.Length;
            else if (frameIndex >= anim.frames.Length)
                frameIndex = anim.frames.Length - 1;

            if (frameIndex != anim.currentFrame)
            {
                if (anim.materialIndex < mats.Length)
                {
                    var mat = mats[anim.materialIndex];
                    mat.mainTexture = anim.frames[frameIndex];

                    if (anim.useTextureAsEmission)
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetTexture("_EmissionMap", anim.frames[frameIndex]);
                        mat.SetColor("_EmissionColor", Color.white * anim.emissionStrength);
                    }
                }

                anim.currentFrame = frameIndex;
            }
        }

        rend.materials = mats;
    }
}
