using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la escena FinPartida:
/// muestra el tiempo jugado, permite escribir un nombre,
/// guarda en el ranking y lista los mejores tiempos.
/// Conecta los botones automáticamente al iniciar.
/// </summary>
public class MostrarTiempo : MonoBehaviour
{
    private InputField _campoNombre;
    private Text _textoTiempo;
    private Text _textoRanking;
    private Text _textoEstado;
    private bool _guardado;

    private void Start()
    {
        BuscarObjetos();
        ConectarBotones();

        float tiempo = TerminarPartida.UltimoTiempo;
        if (_textoTiempo != null)
            _textoTiempo.text = TiempoSesion.FormatearTiempo(tiempo);

        MostrarRanking();
    }

    private void BuscarObjetos()
    {
        if (_campoNombre == null)
        {
            GameObject campo = GameObject.Find("CampoNombre");
            if (campo != null) _campoNombre = campo.GetComponent<InputField>();
        }

        if (_textoTiempo == null)
        {
            GameObject t = GameObject.Find("TextoTiempoValor");
            if (t != null) _textoTiempo = t.GetComponent<Text>();
        }

        if (_textoRanking == null)
        {
            GameObject r = GameObject.Find("TextoRanking");
            if (r != null) _textoRanking = r.GetComponent<Text>();
        }

        if (_textoEstado == null)
        {
            GameObject e = GameObject.Find("TextoEstado");
            if (e != null) _textoEstado = e.GetComponent<Text>();
        }
    }

    private void ConectarBotones()
    {
        // Botón Guardar
        GameObject btnGuardar = GameObject.Find("BotonGuardar");
        if (btnGuardar != null)
        {
            Button btn = btnGuardar.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(GuardarNombre);
        }

        // Botón Volver al Menú
        GameObject btnVolver = GameObject.Find("BotonVolver");
        if (btnVolver != null)
        {
            Button btn = btnVolver.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(VolverAlMenu);
        }
    }

    public void GuardarNombre()
    {
        if (_guardado) return;

        string nombre = _campoNombre != null ? _campoNombre.text : "Anónimo";
        float tiempo = TerminarPartida.UltimoTiempo;

        Ranking.Guardar(nombre, tiempo);
        _guardado = true;

        if (_textoEstado != null)
            _textoEstado.text = "¡Guardado!";

        MostrarRanking();
    }

    private void MostrarRanking()
    {
        ListaRanking lista = Ranking.Cargar();
        if (_textoRanking == null || lista == null) return;

        if (lista.entradas.Count == 0)
        {
            _textoRanking.text = "Aún no hay registros.\n¡Sé el primero!";
            return;
        }

        string bloque = "";
        float miTiempo = TerminarPartida.UltimoTiempo;
        for (int i = 0; i < lista.entradas.Count; i++)
        {
            string linea = Ranking.FormatearEntrada(i + 1, lista.entradas[i]);
            if (Mathf.Approximately(lista.entradas[i].tiempoSegundos, miTiempo) && !_guardado)
                linea += "  <-- ¡TÚ!";
            bloque += linea + "\n";
        }
        _textoRanking.text = bloque.TrimEnd('\n');
    }

    public void VolverAlMenu()
    {
        TerminarPartida.HayTiempoPendiente = false;
        TiempoSesion.TiempoJugado = 0;
        SceneManager.LoadScene("StartGame");
    }
}
