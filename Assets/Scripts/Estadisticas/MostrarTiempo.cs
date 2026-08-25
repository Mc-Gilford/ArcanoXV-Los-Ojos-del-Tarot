using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador de la escena FinPartida:
/// muestra el tiempo jugado, permite escribir un nombre,
/// guarda en el ranking y lista los mejores tiempos.
/// Se auto-construye si se llama CrearUI() o espera los objetos de la escena.
/// </summary>
public class MostrarTiempo : MonoBehaviour
{
    private InputField _campoNombre;
    private Text _textoTiempo;
    private Text _textoRanking;
    private Text _textoEstado;
    private GameObject _panelGuardar;
    private GameObject _panelRanking;
    private bool _guardado;

    private void Start()
    {
        // Si los objetos vienen de la escena (creados por la herramienta), buscar por nombre
        if (_textoTiempo == null) BuscarObjetosEscena();

        float tiempo = TerminarPartida.UltimoTiempo;
        _textoTiempo.text = TiempoSesion.FormatearTiempo(tiempo);

        MostrarRanking();
    }

    private void BuscarObjetosEscena()
    {
        // La herramienta crea estos GameObjects con nombres específicos
        GameObject campo = GameObject.Find("CampoNombre");
        if (campo != null) _campoNombre = campo.GetComponent<InputField>();

        GameObject tiempo = GameObject.Find("TextoTiempoValor");
        if (tiempo != null) _textoTiempo = tiempo.GetComponent<Text>();

        GameObject ranking = GameObject.Find("TextoRanking");
        if (ranking != null) _textoRanking = ranking.GetComponent<Text>();

        GameObject estado = GameObject.Find("TextoEstado");
        if (estado != null) _textoEstado = estado.GetComponent<Text>();

        GameObject btnGuardar = GameObject.Find("BotonGuardar");
        if (btnGuardar != null) _panelGuardar = btnGuardar;

        _panelRanking = GameObject.Find("PanelRanking");
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
            // Resaltar el tiempo actual si coincide
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
