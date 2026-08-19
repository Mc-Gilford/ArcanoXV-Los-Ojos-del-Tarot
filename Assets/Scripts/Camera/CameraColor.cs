using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraColor : MonoBehaviour
{
    private Volume globalVolume;
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;

    private Color originalColor = Color.white;
    private Color redColor = new Color(1f, 0.55f, 0.55f);

    private float originalVignetteIntensity;
    private float originalVignetteSmoothness;
    private Color originalVignetteColor;

    private Jugador playerScript;
    private GameObject player;

    private bool isRed = false;

    void Start()
    {
        player = GameObject.Find("Player");

        //Debug.Log("Player encontrado: " + (player != null));

        if (player != null)
        {
            playerScript = player.GetComponent<Jugador>();
            //Debug.Log("Jugador script encontrado: " + (playerScript != null));
        }

        globalVolume = GetComponent<Volume>();

        //Debug.Log("Volume encontrado: " + (globalVolume != null));

        if (globalVolume != null)
        {
            //Debug.Log("Volume Profile: " + globalVolume.profile);

            // Buscar Color Adjustments
            bool colorFound = globalVolume.profile.TryGet(out colorAdjustments);

            //Debug.Log("ColorAdjustments encontrado: " + colorFound);

            // Buscar Vignette
            bool vignetteFound = globalVolume.profile.TryGet(out vignette);

            //Debug.Log("Vignette encontrado: " + vignetteFound);

            if (colorAdjustments != null)
            {
                colorAdjustments.colorFilter.overrideState = true;

                originalColor = colorAdjustments.colorFilter.value;

                //Debug.Log("Color original: " + originalColor);
            }

            if (vignette != null)
            {
                vignette.intensity.overrideState = true;
                vignette.smoothness.overrideState = true;
                vignette.color.overrideState = true;

                originalVignetteIntensity = vignette.intensity.value;
                originalVignetteSmoothness = vignette.smoothness.value;
                originalVignetteColor = vignette.color.value;

                //Debug.Log("Vignette original intensity: " + originalVignetteIntensity);
            }
        }
    }

    void Update()
    {
        if (playerScript != null && colorAdjustments != null && vignette != null)
        {
            float corduraPlayer = playerScript.getCordura();

            //Debug.Log("Cordura: " + corduraPlayer + " | isRed: " + isRed);

            if (corduraPlayer <= 3 && !isRed)
            {
                ChangeCameraToRed();
            }
            else if (corduraPlayer > 3 && isRed)
            {
                RestoreCameraColor();
            }
        }
        else
        {
            //Debug.LogWarning("NO SE PUEDE CAMBIAR EFECTO | Player: " + (playerScript != null) + " | ColorAdjustments: " + (colorAdjustments != null) + " | Vignette: " + (vignette != null));
        }
    }

    private void ChangeCameraToRed()
    {
        // Color rojo
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = redColor;

        // Vignette
        vignette.intensity.overrideState = true;
        vignette.smoothness.overrideState = true;
        vignette.color.overrideState = true;

        vignette.intensity.value = 0.45f;
        vignette.smoothness.value = 0.7f;
        vignette.color.value = new Color(0.25f, 0f, 0f);

        isRed = true;

        //Debug.Log("APLICANDO EFECTO DE CORDURA | Color: " + colorAdjustments.colorFilter.value + " | Vignette: " + vignette.intensity.value);
    }

    private void RestoreCameraColor()
    {
        // Restaurar Color
        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = originalColor;

        // Restaurar Vignette
        vignette.intensity.overrideState = true;
        vignette.smoothness.overrideState = true;
        vignette.color.overrideState = true;

        vignette.intensity.value = originalVignetteIntensity;
        vignette.smoothness.value = originalVignetteSmoothness;
        vignette.color.value = originalVignetteColor;

        isRed = false;

        //Debug.Log("RESTAURANDO CAMARA | Color: " + colorAdjustments.colorFilter.value + " | Vignette: " + vignette.intensity.value);
    }
}