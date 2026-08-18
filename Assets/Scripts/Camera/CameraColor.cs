using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraColor : MonoBehaviour
{
    private Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    private Color originalColor = Color.white;
    private Color redColor = Color.red;

    private Jugador playerScript;
    private GameObject player;

    private bool isRed = false;

    void Start()
    {
        player = GameObject.Find("Player");

        Debug.Log("Player encontrado: " + (player != null));

        if (player != null)
        {
            playerScript = player.GetComponent<Jugador>();

            Debug.Log(
                "Jugador script encontrado: " +
                (playerScript != null)
            );
        }

        globalVolume = GetComponent<Volume>();

        Debug.Log(
            "Volume encontrado: " +
            (globalVolume != null)
        );

        if (globalVolume != null)
        {
            Debug.Log(
                "Volume Profile: " +
                globalVolume.profile
            );

            bool colorFound =
                globalVolume.profile.TryGet(out colorAdjustments);

            Debug.Log(
                "ColorAdjustments encontrado: " +
                colorFound
            );

            Debug.Log(
                "ColorAdjustments null: " +
                (colorAdjustments == null)
            );

            if (colorAdjustments != null)
            {
                colorAdjustments.colorFilter.overrideState = true;

                originalColor =
                    colorAdjustments.colorFilter.value;

                Debug.Log(
                    "Color original: " +
                    originalColor
                );

                Debug.Log(
                    "Override activo: " +
                    colorAdjustments.colorFilter.overrideState
                );
            }
        }
    }

    void Update()
    {
        if (playerScript != null && colorAdjustments != null)
        {
            float corduraPlayer =
                playerScript.getCordura();

            Debug.Log(
                "Cordura: " +
                corduraPlayer +
                " | isRed: " +
                isRed
            );

            if (corduraPlayer <= 5)
            {
                ChangeCameraToRed();
            }
            else
            {
                RestoreCameraColor();
            }
        }
        else
        {
            Debug.LogWarning(
                "NO SE PUEDE CAMBIAR COLOR | playerScript: " +
                (playerScript != null) +
                " | colorAdjustments: " +
                (colorAdjustments != null)
            );
        }
    }

    private void ChangeCameraToRed()
    {
        colorAdjustments.colorFilter.overrideState = true;

        colorAdjustments.colorFilter.value = redColor;

        isRed = true;

        Debug.Log(
            "APLICANDO ROJO | Color actual: " +
            colorAdjustments.colorFilter.value
        );
    }

    private void RestoreCameraColor()
    {
        colorAdjustments.colorFilter.overrideState = true;

        colorAdjustments.colorFilter.value = originalColor;

        isRed = false;

        Debug.Log(
            "RESTAURANDO COLOR | Color actual: " +
            colorAdjustments.colorFilter.value
        );
    }
}