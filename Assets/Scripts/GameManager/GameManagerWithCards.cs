using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    private CardSelectionSystem cardSelectionSystem;

    void Start()
    {
        cardSelectionSystem =
            FindFirstObjectByType<CardSelectionSystem>();
    }

    private void ApplicarPowerUp(int indexPowerUp)
    {
        switch (indexPowerUp)
        {
            case 0:
                // Efecto carta J
                break;

            case 1:
                // Efecto carta K
                break;

            case 2:
                // Efecto carta L
                break;
        }
    }

    private void Update()
    {
        if (cardSelectionSystem != null)
        {
            if (cardSelectionSystem.isPowerUpSeleccionado)
            {
                int indexPowerUp = cardSelectionSystem.IndexCartasSeleccionada;
                ApplicarPowerUp(indexPowerUp);
                cardSelectionSystem.isPowerUpSeleccionado = false;
            }
        }
    }
}