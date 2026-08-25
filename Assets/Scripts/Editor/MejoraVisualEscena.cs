using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Mejora visual cinematográfica de la escena "Carro y salida" (thriller nocturno):
/// - Luz de luna fría lateral
/// - Luces ambiente internas del auto (azul tablero + rojo sutil trasero)
/// - Niebla gris azulado oscura
/// - Global Volume con Color Adjustments + Bloom sutil + Vignette
/// - FOV cinematográfico en las cámaras
/// - Panel del HUD totalmente transparente (solo letras)
/// NO modifica lógica de gameplay ni elimina objetos existentes.
/// </summary>
public static class MejoraVisualEscenaTool
{
    // Paleta (spec): DARK BLUE #101827, COLD BLUE #3B526E, ACCENT YELLOW #F4C95D, SUBTLE RED #7F2525
    private static readonly Color ColorLuna = new Color(0.58f, 0.68f, 0.85f);
    private static readonly Color ColorNiebla = new Color(0.070f, 0.090f, 0.125f);
    private static readonly Color ColorAmbiente = new Color(0.100f, 0.120f, 0.170f);
    private static readonly Color AzulTablero = new Color(0.45f, 0.62f, 1.00f);
    private static readonly Color RojoSutil = new Color(0.50f, 0.15f, 0.15f);

    private const float FOVCine = 62f;

    public static void Aplicar()
    {
        ConfigurarLuna();
        CrearLucesAmbienteCarro();
        ConfigurarNieblaYAmbiente();
        ConfigurarVolume();
        AjustarCameras();
        TransparentarPanelHUD();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("[MejoraVisual] Aplicada. Prueba con Play y ajusta a gusto desde el Inspector.");
    }

    // ---------- 1. LUNA ----------
    private static void ConfigurarLuna()
    {
        Light luna = null;
        foreach (Light l in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (l.type == LightType.Directional) { luna = l; break; }

        if (luna == null)
        {
            GameObject go = new GameObject("VISUAL_MoonLight");
            Undo.RegisterCreatedObjectUndo(go, "Crear luz de luna");
            luna = go.AddComponent<Light>();
            luna.type = LightType.Directional;
        }
        else Undo.RecordObject(luna.transform, "Ajustar luna");

        Undo.RecordObject(luna, "Configurar luna");
        luna.intensity = 0.28f;
        luna.color = ColorLuna;
        luna.shadows = LightShadows.Soft;
        luna.shadowStrength = 0.65f;
        // Entra lateral (izquierda-atrás), no uniforme: deja zonas oscuras en el interior
        luna.transform.rotation = Quaternion.Euler(32f, 118f, 0f);
    }

    // ---------- 2. LUCES AMBIENTE DEL AUTO ----------
    private static void CrearLucesAmbienteCarro()
    {
        GameObject carro = BuscarCarro();
        if (carro == null)
        {
            Debug.LogWarning("[MejoraVisual] No encontré el carro RMCar26*; luces internas omitidas.");
            return;
        }

        CrearLuzInterna(carro.transform, "VISUAL_LuzTableroAzul", new Vector3(-0.15f, 0.55f, 0.55f),
                        AzulTablero, 0.40f, 0.50f);
        CrearLuzInterna(carro.transform, "VISUAL_LuzTraseraRoja", new Vector3(0.20f, 0.45f, -0.70f),
                        RojoSutil, 0.30f, 0.45f);
        // El carro escala 3 => el range se multiplica; valores pequeños dan radios correctos (~1.5 m)
    }

    private static void CrearLuzInterna(Transform padre, string nombre, Vector3 posLocal,
                                        Color color, float intensidad, float rango)
    {
        Transform previo = padre.Find(nombre);
        Light luz;
        if (previo != null)
        {
            luz = previo.GetComponent<Light>();
            Undo.RecordObject(luz, "Actualizar " + nombre);
        }
        else
        {
            GameObject go = new GameObject(nombre);
            Undo.RegisterCreatedObjectUndo(go, "Crear " + nombre);
            go.transform.SetParent(padre, false);
            luz = go.AddComponent<Light>();
        }
        luz.type = LightType.Point;
        luz.color = color;
        luz.intensity = intensidad;
        luz.range = rango;
        luz.shadows = LightShadows.None;
        luz.transform.localPosition = posLocal;
    }

    // ---------- 4. NIEBLA ----------
    private static void ConfigurarNieblaYAmbiente()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.018f;
        RenderSettings.fogColor = ColorNiebla;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ColorAmbiente;
    }

    // ---------- 5-7. VOLUME: COLOR ADJ + BLOOM + VIGNETTE ----------
    private static void ConfigurarVolume()
    {
        const string rutaPerfil = "Assets/Settings/VISUAL_PerfilCine.asset";
        if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            AssetDatabase.CreateFolder("Assets", "Settings");

        VolumeProfile perfil = AssetDatabase.LoadAssetAtPath<VolumeProfile>(rutaPerfil);
        if (perfil == null)
        {
            perfil = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(perfil, rutaPerfil);

            var colorAdj = perfil.Add<ColorAdjustments>();
            colorAdj.postExposure.Override(-0.3f);
            colorAdj.contrast.Override(18f);
            colorAdj.saturation.Override(-12f);
            colorAdj.colorFilter.Override(new Color(0.85f, 0.92f, 1.00f));
            GuardarSubAsset(perfil, colorAdj);

            var bloom = perfil.Add<Bloom>();
            bloom.intensity.Override(0.25f);
            bloom.threshold.Override(1.10f);
            bloom.scatter.Override(0.65f);
            GuardarSubAsset(perfil, bloom);

            var vignette = perfil.Add<Vignette>();
            vignette.intensity.Override(0.22f);
            vignette.smoothness.Override(0.70f);
            GuardarSubAsset(perfil, vignette);

            EditorUtility.SetDirty(perfil);
        }

        // Reutilizar un Global Volume existente antes de crear otro
        Volume volumen = null;
        foreach (Volume v in Object.FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (v.isGlobal) { volumen = v; break; }

        if (volumen == null)
        {
            GameObject go = new GameObject("VISUAL_GlobalVolume");
            Undo.RegisterCreatedObjectUndo(go, "Crear Global Volume");
            volumen = go.AddComponent<Volume>();
        }
        else Undo.RecordObject(volumen, "Asignar perfil visual");

        volumen.isGlobal = true;
        volumen.weight = 1f;
        volumen.sharedProfile = perfil;
    }

    private static void GuardarSubAsset(VolumeProfile perfil, VolumeComponent componente)
    {
        AssetDatabase.AddObjectToAsset(componente, perfil);
        EditorUtility.SetDirty(componente);
    }

    // ---------- 11. CÁMARAS ----------
    private static void AjustarCameras()
    {
        foreach (Camera cam in Object.FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            Undo.RecordObject(cam, "FOV cine");
            cam.fieldOfView = FOVCine;
        }
    }

    // ---------- 13. HUD TRANSPARENTE ----------
    private static void TransparentarPanelHUD()
    {
        GameObject panel = GameObject.Find("Panel_Contador");
        if (panel == null) return;
        Image fondo = panel.GetComponent<Image>();
        if (fondo != null)
        {
            Undo.RecordObject(fondo, "HUD transparente");
            fondo.color = new Color(fondo.color.r, fondo.color.g, fondo.color.b, 0f);
            fondo.raycastTarget = false;
        }
    }

    // ---------- Utilidad ----------
    private static GameObject BuscarCarro()
    {
        Transform[] todos = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform t in todos)
        {
            if (t.root != t || t.gameObject.scene.isLoaded == false) continue;
            string n = t.name.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");
            if (n.StartsWith("rmcar26")) return t.gameObject;
        }
        return null;
    }
}
