using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RankingManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Text textoTiempoValor;
    [SerializeField] private InputField campoNombre;
    [SerializeField] private Button botonGuardar;
    [SerializeField] private Text textoEstado;
    [SerializeField] private Text textoRanking;

    public GameObject panelFinPartida;

    private float tiempoFinal;

    void Start()
    {
        botonGuardar.onClick.AddListener(GuardarRecord);

        textoEstado.text = "";

        MostrarRanking();

        panelFinPartida.SetActive(false);
    }

    // GameManager manda el tiempo aqui
    public void SetTiempoFinal(float tiempo)
    {
        tiempoFinal = tiempo;

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // NUEVA FEATURE: Usamos tu tiempo ya formateado para mostrarlo
        textoTiempoValor.text = gameManager.timeWithFormat;
    }

    private void GuardarRecord()
    {
        string nombre = campoNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            textoEstado.text = "Escribe tu nombre";
            return;
        }

        List<RecordData> records = CargarRecords();

        RecordData nuevoRecord = new RecordData();
        nuevoRecord.nombre = nombre;
        nuevoRecord.tiempo = tiempoFinal;

        records.Add(nuevoRecord);

        // Menor tiempo = mejor posicion
        records.Sort((a, b) => a.tiempo.CompareTo(b.tiempo));

        // Solo guardamos los mejores 5
        if (records.Count > 5)
        {
            records.RemoveRange(5, records.Count - 5);
        }

        GuardarRecords(records);

        textoEstado.text = "¡Record guardado!";

        // Evita guardar varias veces la misma partida
        botonGuardar.interactable = false;

        MostrarRanking();
    }

    public void ShowRankingPanel()
    {
        // Activa el objeto donde está este RankingManager
        gameObject.SetActive(true);

        textoEstado.text = "";
        botonGuardar.interactable = true;

        panelFinPartida.SetActive(true);

        MostrarRanking();
    }

    private void MostrarRanking()
    {
        List<RecordData> records = CargarRecords();

        if (records.Count == 0)
        {
            textoRanking.text = "Aun no hay records";
            return;
        }

        textoRanking.text = "";

        for (int i = 0; i < records.Count; i++)
        {
            int horas = Mathf.FloorToInt(records[i].tiempo / 3600f);
            int minutos = Mathf.FloorToInt((records[i].tiempo % 3600f) / 60f);
            int segundos = Mathf.FloorToInt(records[i].tiempo % 60f);

            textoRanking.text += (i + 1) + ". " +
                                 records[i].nombre + "     " +
                                 horas.ToString("00") + ":" +
                                 minutos.ToString("00") + ":" +
                                 segundos.ToString("00");

            if (i < records.Count - 1)
            {
                textoRanking.text += "\n";
            }
        }
    }

    private void GuardarRecords(List<RecordData> records)
    {
        PlayerPrefs.SetInt("CantidadRecords", records.Count);

        for (int i = 0; i < records.Count; i++)
        {
            PlayerPrefs.SetString("RecordNombre_" + i, records[i].nombre);
            PlayerPrefs.SetFloat("RecordTiempo_" + i, records[i].tiempo);
        }

        PlayerPrefs.Save();
    }

    private List<RecordData> CargarRecords()
    {
        List<RecordData> records = new List<RecordData>();

        int cantidad = PlayerPrefs.GetInt("CantidadRecords", 0);

        for (int i = 0; i < cantidad; i++)
        {
            RecordData record = new RecordData();

            record.nombre = PlayerPrefs.GetString("RecordNombre_" + i, "---");
            record.tiempo = PlayerPrefs.GetFloat("RecordTiempo_" + i, 0f);

            records.Add(record);
        }

        records.Sort((a, b) => a.tiempo.CompareTo(b.tiempo));

        return records;
    }

    public void ReturnToMenu()
    {
        panelFinPartida.SetActive(false);
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]
public class RecordData
{
    public string nombre;
    public float tiempo;
}