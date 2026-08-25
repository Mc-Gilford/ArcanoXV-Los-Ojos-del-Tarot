using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Ranking de tiempos guardado en PlayerPrefs (JSON).
/// Top 10 entradas ordenadas por tiempo (más rápido = mejor puesto).
/// No necesita base de datos ni servidor: funciona offline y en builds.
/// </summary>
[Serializable]
public class EntradaRanking
{
    public string nombre;
    public float tiempoSegundos;
}

[Serializable]
public class ListaRanking
{
    public List<EntradaRanking> entradas = new List<EntradaRanking>();
}

public static class Ranking
{
    private const string Clave = "RankingPartidas";
    private const int MaxEntradas = 10;

    public static ListaRanking Cargar()
    {
        string json = PlayerPrefs.GetString(Clave, "");
        if (string.IsNullOrEmpty(json))
            return new ListaRanking();
        return JsonUtility.FromJson<ListaRanking>(json);
    }

    public static void Guardar(string nombre, float tiempoSegundos)
    {
        ListaRanking lista = Cargar();

        lista.entradas.Add(new EntradaRanking
        {
            nombre = string.IsNullOrWhiteSpace(nombre) ? "Anónimo" : nombre.Trim(),
            tiempoSegundos = tiempoSegundos
        });

        // Ordenar: más rápido = mejor puesto
        lista.entradas = lista.entradas
            .OrderBy(e => e.tiempoSegundos)
            .Take(MaxEntradas)
            .ToList();

        PlayerPrefs.SetString(Clave, JsonUtility.ToJson(lista));
        PlayerPrefs.Save();
    }

    /// <summary>Formatea una entrada para mostrar en el ranking.</summary>
    public static string FormatearEntrada(int posicion, EntradaRanking entrada)
    {
        string tiempo = TiempoSesion.FormatearTiempo(entrada.tiempoSegundos);
        return $"{posicion}. {entrada.nombre} — {tiempo}";
    }
}
