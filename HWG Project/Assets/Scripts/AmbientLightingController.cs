using UnityEngine;

public class AmbientLightingController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float indoorIntensity = 0.3f;

    public float transitionSpeed = 3f;

    private Color originalAmbient;
    private Color targetAmbient;

    void Start()
    {
        // Store the starting ambient color
        originalAmbient = RenderSettings.ambientLight;
        targetAmbient = originalAmbient;
    }

    void Update()
    {
        // Smoothly transition toward target ambient color
        RenderSettings.ambientLight = Color.Lerp(
            RenderSettings.ambientLight,
            targetAmbient,
            Time.deltaTime * transitionSpeed
        );
    }

    public void SetIndoor()
    {
        targetAmbient = originalAmbient * indoorIntensity;
    }

    public void SetOutdoor()
    {
        targetAmbient = originalAmbient;
    }
}