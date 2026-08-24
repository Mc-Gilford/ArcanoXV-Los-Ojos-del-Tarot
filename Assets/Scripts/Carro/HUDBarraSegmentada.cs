using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra segmentada de 10 pips bajo el contador "Presiona X".
/// SOLO visual: escucha el evento estático OnProgresoX de CarroSalidaController,
/// no duplica ninguna lógica de gameplay.
/// </summary>
public class HUDBarraSegmentada : MonoBehaviour
{
    private Image[] _pips;
    private static readonly Color Lleno = new Color(0.957f, 0.788f, 0.365f); // #F4C95D
    private static readonly Color Vacio = new Color(0.16f, 0.18f, 0.22f, 0.85f);

    private void OnEnable()
    {
        CarroSalidaController.OnProgresoX += Actualizar;
        Construir();
    }

    private void OnDisable()
    {
        CarroSalidaController.OnProgresoX -= Actualizar;
    }

    private void Start()
    {
        // Estado inicial 0/10 por si el evento ya ocurrió antes de suscribirse
        Actualizar(0, _pips != null ? _pips.Length : 10);
    }

    private void Construir()
    {
        if (transform.Find("VISUAL_BarraX") != null)
        {
            int n = transform.Find("VISUAL_BarraX").childCount;
            _pips = new Image[n];
            for (int i = 0; i < n; i++)
                _pips[i] = transform.Find("VISUAL_BarraX").GetChild(i).GetComponent<Image>();
            return;
        }

        GameObject fila = new GameObject("VISUAL_BarraX", typeof(RectTransform));
        fila.transform.SetParent(transform, false);

        const int total = 10;
        const float anchoPip = 26f;
        const float altoPip = 8f;
        const float separacion = 6f;
        float anchoTotal = total * anchoPip + (total - 1) * separacion;

        RectTransform filaRect = fila.GetComponent<RectTransform>();
        // Anclado al borde inferior del panel del contador
        filaRect.anchorMin = new Vector2(0.5f, 0f);
        filaRect.anchorMax = new Vector2(0.5f, 0f);
        filaRect.pivot = new Vector2(0.5f, 1f);
        filaRect.anchoredPosition = new Vector2(0f, -4f);
        filaRect.sizeDelta = new Vector2(anchoTotal, altoPip);

        _pips = new Image[total];
        Sprite cuadrado = SpriteRedondeado();
        for (int i = 0; i < total; i++)
        {
            GameObject pip = new GameObject("Pip_" + i, typeof(RectTransform), typeof(Image));
            pip.transform.SetParent(fila.transform, false);
            RectTransform r = pip.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0f, 0.5f);
            r.anchorMax = new Vector2(0f, 0.5f);
            r.pivot = new Vector2(0f, 0.5f);
            r.anchoredPosition = new Vector2(i * (anchoPip + separacion), 0f);
            r.sizeDelta = new Vector2(anchoPip, altoPip);

            Image img = pip.GetComponent<Image>();
            img.sprite = cuadrado;
            img.type = cuadrado != null ? Image.Type.Sliced : Image.Type.Simple;
            img.color = Vacio;
            img.raycastTarget = false;
            _pips[i] = img;
        }
    }

    private void Actualizar(int presionadas, int requeridas)
    {
        if (_pips == null) Construir();
        if (_pips == null || _pips.Length == 0) return;
        for (int i = 0; i < _pips.Length; i++)
        {
            if (_pips[i] == null) continue;
            bool activo = requeridas > 0 && i < Mathf.Min(presionadas, requeridas);
            _pips[i].color = activo ? Lleno : Vacio;
        }
    }

    private static Sprite _spritePip;

    /// <summary>Sprite redondeado generado por código (evita dependencias externas).</summary>
    private static Sprite SpriteRedondeado()
    {
        if (_spritePip != null) return _spritePip;
        const int tam = 24;
        Texture2D tex = new Texture2D(tam, tam, TextureFormat.RGBA32, false);
        float centro = (tam - 1) / 2f;
        float radio = tam / 2f - 1.5f;
        Color[] px = new Color[tam * tam];
        for (int y = 0; y < tam; y++)
        {
            for (int x = 0; x < tam; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(centro, centro));
                float a = Mathf.Clamp01(radio - d + 1f); // borde suave de 1px
                px[y * tam + x] = new Color(1f, 1f, 1f, a);
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        _spritePip = Sprite.Create(tex, new Rect(0, 0, tam, tam), new Vector2(0.5f, 0.5f), 100f);
        return _spritePip;
    }
}
