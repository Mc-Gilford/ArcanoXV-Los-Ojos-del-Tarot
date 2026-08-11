# 🧩 Plan de Integración con el equipo y publicación en itch.io — Arcano XV

Documento de referencia para el momento de unir el trabajo de todos (Bryam = casa + sonido)
con el de los compañeros y subir el juego a itch.io **sin que salgan errores**.

---

## ✅ Estado actual: ¿qué falta para que TU parte esté completa?

| Pieza | Estado |
|---|---|
| Modelos de la casa (43 assets) | ✅ Organizados en `Assets/Models/` |
| Importarlos a Unity | ✅ **Blender 5.2.0 instalado** (`D:\blender.exe`) + glTFast 6.19.0 en `manifest.json` |
| Clips de sonido (objetos) | ✅ 18 categorías con audio en `Assets/Audio/Objetos/` |
| Clips de pasos | ✅ 3 en `Assets/Audio/Pasos/` |
| Clips de sustos | ✅ 4 cerca + 2 lejos en `Assets/Audio/Sustos/` |
| Sistema de susto aleatorio | ✅ `HauntedObject` (suena solo a intervalos, sin importar la distancia) |
| Sonido por contacto | ✅ `ObjectAmbience` |
| 10 habitaciones del GDD | ⏳ Se generan al correr `Tools > Arcano XV > Generar habitaciones` |

**Respuesta corta a tu duda:** para *tu* tarea, **Blender era lo único que faltaba instalar** y ya está.
Todo lo demás (audio, código, paquetes) ya está en el proyecto.

---

## ⚠️ Lo que sí puede dar "errores" al integrar con el equipo

### 1. ✓ Los `.blend` ya NO son necesarios (resuelto: convertidos a `.glb`)
Se convirtieron los 36 `.blend` a **`.glb` con texturas incluidas** (en `Assets/Models/GLB`) y los
originales se movieron a **`BlendOriginal/`** (fuera de `Assets`). **Ningún compañero necesita
instalar Blender** para abrir/importar/jugar: solo glTFast, que ya está en `manifest.json` y baja
solo. El catálogo (`RoomsLayouts.cs`) ya apunta a los `.glb`. Este punto dejó de ser un riesgo.

### 2. Lo reutilizable = prefabs y scripts, no las escenas generadas
Mis 10 escenas en `Assets/Scenes/Habitaciones/` son para **probar la mecánica de sonido**.
Para el juego real, el equipo arma su propio flujo de niveles. Lo que se debe llevar a esas
escenas es lo reutilizable:

- **`Assets/Prefabs/Objetos/*`** — muebles con `HauntedObject` + `ObjectAmbience` ya configurados.
- **`Assets/Prefabs/Player/Jugador_Pasos.prefab`** — jugador con pasos (quitar `DebugPlayerMover`
  y poner el controlador real del equipo; **conservar `PlayerFootsteps`**).
- **Scripts** en `Assets/Scripts/Ambience/`: `HauntedObject`, `ObjectAmbience`, `RoomTracker`,
  `RoomTriggerZone`, `PlayerFootsteps` (todos auto-contenidos, sin dependencias externas).

> Así evitás dos personas editando la misma escena (conflictos de merge).

### 3. Control de versiones: meta y commit
- **NUNCA editar `.meta` a mano** (los genera Unity). Asegurar que al commiteAR vayan
  `archivo + .meta` juntos para que no se rompan los GUID.
- **Los GUID de los scripts** no deben cambiar (mover un `.cs` junto con su `.cs.meta`).
- Nada está commiteado todavía (regla del proyecto): esto se commitea solo cuando Bryam
  pruebe en Unity y apruebe.

### 4. Materiales de Poly Haven en URP
Los modelos importan con shaders PBR estándar; en URP pueden verse **magenta** o con avisos.
- Fix: `Window > Rendering > Render Pipeline Converter` → convertir Built-in → URP.
- No es un error de compilación; es visual.

### 5. Compilar la parte nueva (paso 1 obligatorio antes de mergear)
Al abrir Unity por primera vez con estos scripts puede salir algún error de compilación.
- Correr `Tools > Arcano XV > Generar TODO` y **copiar cualquier error de la Consola aquí**
  para corregirlo. Recién cuando compile limpio se integra.

---

## 🕹️ Subir a itch.io sin errores

### Recomendación: build de Windows (no WebGL)
Es un survival horror con **muchos AudioSource simultáneos** y URP. WebGL (el modo
"jugar en el navegador") tiene límites de memoria (~2 GB) y el audio puede cortarse.
**Para itch.io: `File > Build Profiles` → Windows → Build → subir el `.zip`** con el botón
*Upload > .zip file*. itch.io pone el botón "Descargar" automáticamente.

Si además quieren la versión "jugar en el navegador" (WebGL):
- AudioClips: `Load Type = Compressed` + **desmarcar** `Preload Audio Data` (importador de cada clip).
- Añadir solo las escenas del juego real a Build Profiles (no las 10 de prueba).
- El primer clic del jugador desbloquea el audio (autoplay del navegador).

### Checklist antes de subir
- [ ] La escena inicial del juego está en **Build Profiles** (con sus escenas encadenadas).
- [ ] El audio se escucha en una build (no solo en el Editor).
- [ ] Los modelos no se ven magenta (conversión URP hecha).
- [ ] Los clips no pesan de más (conversión a `.ogg`/`.wav` comprimido si la build crece).
- [ ] Nadie con el proyecto en sus manos necesita algo que no esté documentado (Blender, paquetes).

---

## 🎯 Próximos pasos inmediatos
1. Abrir Unity (con Blender ya instalado) → `Tools > Arcano XV > Generar TODO`.
2. Si hay errores de compilación: pegarlos aquí y se corrigen.
3. Abrir `Assets/Scenes/Habitaciones/Habitacion_1_El_Olvido.unity`, Play, caminar con WASD
   y confirmar que los muebles suenan solos.
4. Recién después: commitear (rama Bryam) y seguir con la integración del equipo.
