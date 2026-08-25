using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cronómetro persistente: comienza a contar cuando PrincipalScene carga y se detiene
/// cuando se llama a TerminarPartida.IrAFinPartida(). El tiempo queda registrado en
/// TerminarPartida.UltimoTiempo para que la escena de fin lo muestre.
/// </summary>
public class TiempoSesion : MonoBehaviour
{
    public static TiempoSesion Instancia;
    public static float TiempoJugado;

    private bool _contando;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Inicializar()
    {
        var go = new GameObject("TiempoSesion");
        DontDestroyOnLoad(go);
        Instancia = go.AddComponent<TiempoSesion>();
        TiempoJugado = 0;
        SceneManager.sceneLoaded += Instancia.OnEscenaCargada;
    }

    private void OnEscenaCargada(Scene escena, LoadSceneMode modo)
    {
        _contando = (escena.name == "PrincipalScene");
    }

    private void Update()
    {
        if (_contando)
            TiempoJugado += Time.deltaTime;

        // SOLO EN EDITOR: F1 en PrincipalScene para probar sin jugar toda la partida
        #if UNITY_EDITOR
        if (_contando && Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("[TiempoSesion] F1 detectado -> TerminarPartida (prueba)");
            TerminarPartida.IrAFinPartida();
        }
        #endif
    }

    public void Detener()
    {
        _contando = false;
    }

    public void Reiniciar()
    {
        TiempoJugado = 0;
        _contando = true;
    }

    /// <summary>Formatea el tiempo como MM:SS.ss</summary>
    public static string FormatearTiempo(float segundos)
    {
        int min = (int)(segundos / 60);
        int seg = (int)(segundos % 60);
        int cs  = (int)((segundos * 100) % 100);
        return $"{min:00}:{seg:00}.{cs:00}";
    }
}
