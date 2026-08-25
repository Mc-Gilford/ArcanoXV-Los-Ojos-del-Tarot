using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Punto de entrada para terminar una partida desde CUALQUIER escena.
/// Tus compañeros solo deben agregar UNA línea en su punto de salida:
///
///     TerminarPartida.IrAFinPartida();
///
/// Esto detiene el cronómetro, guarda el tiempo y carga la pantalla de fin + ranking.
/// </summary>
public static class TerminarPartida
{
    private const string EscenaFin = "FinPartida";

    /// <summary>Último tiempo registrado (lo lee la escena de fin para mostrarlo).</summary>
    public static float UltimoTiempo;

    /// <summary>True si hay un tiempo pendiente de mostrar (la escena de fin lo consume).</summary>
    public static bool HayTiempoPendiente;

    /// <summary>
    /// Llamar desde cualquier script para terminar la partida.
    /// Detiene el cronómetro y carga la escena de fin con ranking.
    /// Ejemplo:  TerminarPartida.IrAFinPartida();
    /// </summary>
    public static void IrAFinPartida()
    {
        if (TiempoSesion.Instancia != null)
            TiempoSesion.Instancia.Detener();

        UltimoTiempo = TiempoSesion.TiempoJugado;
        HayTiempoPendiente = true;

        SceneManager.LoadScene(EscenaFin);
    }
}
