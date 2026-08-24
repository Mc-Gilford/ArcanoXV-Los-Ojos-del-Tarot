# Contexto del proyecto — Arcano XV: Los Ojos del Tarot

Proyecto Unity 6 con URP 17.3.0. El usuario habla español; responder en español y de forma concisa.
Repositorio: `https://github.com/Mc-Gilford/ArcanoXV-Los-Ojos-del-Tarot.git`, rama `develop`. Varios compañeros suben cambios en paralelo: antes de pushear hacer `git fetch` + `git pull` para no pisar trabajo ajeno.

## Trabajo realizado (sesión agosto 2026)

### Escena `Assets/Scenes/Carro y salida.unity`
- Carro raíz = instancia del prefab `RMCar26_D` (guid `6ffff8e67ac89334b9b5ace4ae99ca65`), posición (-66.06, 1.7, 1.85), rotY=90, scale 3.
- **Pintura negra**: `Assets/External Assets/RealisticMobileCars - Pro3DModels/RMCar26/Materials/RMCar26_Paint.mat` cambiado a shader URP/Lit (guid `933532a4fcc9baf4fa0491de14d08ed7`), `_BaseColor` (0.02,0.02,0.02), `_Smoothness 0.75`.
- **Ambiente nocturno** (igual que PrincipalScene): skybox BOXOPHOBIC `Skybox Cubemap Extended Blend.mat` (guid `1cf6f4c6e6e0518419839d3f5448dd99`) + luz lunar color (0.4,0.45,0.6) intensidad 0.8, sombras suaves, bounceIntensity 0.3.
- Lluvia: RainPrefab (DigitalRuby.RainMaker). Sus scripts `BaseRainScript.cs`/`RainScript.cs` fueron parcheados con null-checks de cámara.

### Scripts clave
- `Assets/Scripts/Carro/CarroSalidaController.cs`: mueve el carro de `puntoInicio` a `puntoFin`; campo `duracionTrayecto = 14f` (Awake calcula velocidad = distancia/14); contador de teclas X (`teclasRequeridas = 10`) para salir antes; al terminar espera 2s y llama `CompletarSalida()`.
- `TransicionEscenaRunner` (misma clase archivo): objeto DontDestroyOnLoad que hace fundido a negro → carga asíncrona de `nombreEscenaSiguiente` ("PrincipalScene") → fundido de vuelta dentro de la nueva escena. ⚠️ PENDIENTE PROBAR EN PLAY MODE.
- `Assets/Scripts/Camera/CarCameraFollow.cs`: seguimiento tercera persona (NO adjuntar a la cámara interior).
- `Assets/Scripts/Editor/ConfigurarCarroSalida.cs`: menú `Tools > Arcano XV > Configurar Carro Salida (Primera Persona)`. Busca SOLO objetos raíz cuyo nombre empiece con "rmcar26", agrega Rigidbody cinemático + PlayerInput (asset `Assets/Scenes/PlayerActions.inputactions`, mapa "Jugador") + controller, crea cámara FP hija de la raíz (localPos (-0.35, 0.72, 0.20), nearClip 0.02, FOV 70), recrea HUD si falta y aplica el ambiente nocturno.
- `Assets/Scripts/Editor/CarroYSalidaGenerator.cs`: generador original de la escena (referencia).

## Pendientes / decisiones del usuario
- Probar en Play el fundido de transición hacia PrincipalScene (TransicionEscenaRunner).
- El usuario QUIERE cambiar el texto del HUD de "Presiona X: n/10" a "Frenar: n/10" y lo hará ÉL MISMO en `ActualizarUI()` de `CarroSalidaController.cs`. No hacerlo por él.

## Lecciones importantes
- ⚠️ NUNCA editar archivos .unity en disco cuando el usuario puede tener cambios sin guardar en el editor (Unity recarga y pierde su trabajo). Ya pasó una vez.
- Después de correr la herramienta de configuración, guardar la escena con Ctrl+S inmediatamente.
- Tras editar scripts, verificar errores de compilación con `$env:LOCALAPPDATA\Unity\Editor\Editor.log` (buscar "error CS").
- El proyecto usa Input System (no el Input Manager viejo): usar `Keyboard.current.xKey.wasPressedThisFrame` o PlayerInput.
