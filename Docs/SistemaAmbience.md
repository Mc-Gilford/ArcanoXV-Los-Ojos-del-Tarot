# 🔊 Sistema de Ambience Paranormal — Guía de uso

Sistema que hace "viva" la casa: detecta cuándo el jugador está cerca de un objeto o en una habitación
y dispara **sonidos y animaciones aleatorias** para generar sustos y tensión (GDD: características de
cordura y "sonidos aleatorios que mantendrán al usuario en alta alerta").

## Componentes

| Script | Rol |
|---|---|
| `RoomTracker` | Singleton que sabe en qué habitación (`RoomTriggerZone`) está el jugador. Emite eventos `PlayerEnteredRoom` / `PlayerExitedRoom`. |
| `RoomTriggerZone` | Volumen invisible (collider `isTrigger`) que delata a cada habitación. Se coloca uno por sala y reporta al `RoomTracker`. |
| `HauntedObject` | Se añade a cualquier mueble/prop. **Por defecto** suena a intervalos aleatorios **sin importar la distancia** (cada objeto "habla" solo para asustar). Modo clásico opcional: reacciona con sonidos y animaciones aleatorias **cerca** del jugador y con sonidos inesperados **lejos pero en la misma habitación**. |

## Configuración paso a paso

### 1. El jugador
- Añade la tag **`Player`** al objeto raíz del personaje (Inspector → Tag).
- Si el personaje usa `CharacterController` o un `CapsuleCollider`, el sistema lo detecta solo.
- Alternativa: asigna el `Transform` del jugador en `playerOverride` (de `RoomTracker` o de cada `HauntedObject`).

### 2. El RoomTracker
- Crea un GameObject vacío (o usa tu manager global) y añade `RoomTracker`.
- Solo debe haber **uno** en la escena.

### 3. Zonas por habitación (una por sala del GDD)
- Crea un cubo/volumen que **cubra toda la habitación** (no las paredes).
- Ponle un `Box Collider` con **`Is Trigger = true`**.
- Añade el componente `RoomTriggerZone` y nómbralo (`roomName: "El Ritual"`, etc.).
- En el editor, al seleccionarlo verás el volumen verde.

### 4. Objetos embrujados (muebles/props)
- Selecciona un mueble (sillón, lámpara, mesa...) y añade `HauntedObject`.
- Arrastra los **AudioClips** de susto a `nearSounds` y/o `farSounds`. En el modo por defecto
  (`scareAnywhere = true`) se usan **todos juntos** como pool de sustos: el objeto suena uno al azar
  a intervalos aleatorios, no importa dónde esté el jugador.
- Ajusta `minIntervalScare`/`maxIntervalScare` (cuánto tarda en volver a sonar) y `scareChance`
  (probabilidad de sonar en cada intervalo). Si hay **muchos objetos** y suenan seguido, baja `scareChance`.
- Opcional: si el objeto tiene `Animator`, asigna `animationTriggers` (nombres de los triggers). Si no,
  deja `proceduralShake = true` y el objeto hará una sacudida de susto automática.
- En el editor, al seleccionarlo verás un **círculo naranja** = radio de "cerca" (solo modo clásico).

### 4b. Modo "susto en cualquier sitio" (nueva mecánica, por defecto)
- `scareAnywhere = true` → el objeto **no depende de la distancia**: suena solo, a intervalos aleatorios,
  mezclando `nearSounds` + `farSounds`. Ideal para que la casa entera "se despierta" sin importar dónde estés.
- `scareAnywhere = false` → vuelve al comportamiento clásico de la tabla de abajo (cerca/lejos por habitación).

## Cómo funciona cada tarea del GDD

| Tarea | Componente | Comportamiento |
|---|---|---|
| Sonidos aleatorios para asustar (sin importar distancia) | `HauntedObject` (`scareAnywhere = true`) | Cada objeto suena a intervalos aleatorios (`min/maxIntervalScare`, con `scareChance`) mezclando `nearSounds`+`farSounds`. No depende de dónde esté el jugador. |
| Detección del jugador → sonidos/animaciones aleatorias (clásico) | `HauntedObject` (`scareAnywhere = false`) | Cuando el jugador entra al círculo de `proximityDistance`, el objeto reacciona cada `min/maxIntervalNear` con un clip aleatorio y una animación (trigger o sacudida). |
| Sonidos inesperados cerca o lejos en la misma habitación (clásico) | `HauntedObject` + zonas | Si el jugador está en la misma `RoomTriggerZone` pero fuera del círculo, suenan los `farSounds` a intervalos largos y con menor volumen. Si está cerca, suenan los `nearSounds`. |

## Notas importantes

- **Audio 3D:** cada `HauntedObject` crea su propio `AudioSource` con `spatialBlend = 1`, así el sonido
  "llega desde el objeto". Ajusta `farVolume` más bajo que `nearVolume` para que lo lejano suene lejos.
- **Sin zonas configuradas (modo simple):** si no pones `RoomTriggerZone`, `HauntedObject` usa
  `roomRadiusFallback` (30 m por defecto) para aproximar "misma habitación". Útil para prototipar.
- **El jugador necesita collider:** sin collider en el personaje, los triggers no lo detectan.
- **Los volúmenes trigger pueden atravesar pisos/techos:** asegúrate de que cada zona sea exclusiva
  de su habitación; puedes usar varios colliders por zona si una sala es irregular.
- **Rendimiento:** `HauntedObject` hace 1 comparación de distancia por frame por objeto. A escala de
  decenas de muebles es despreciable.

## Siguiente paso sugerido

Los sonidos de susto ya existen como AudioClips en `Assets/Audio/Sustos/Near` y `Assets/Audio/Sustos/Far`
(y el menú **Tools > Arcano XV > Asignar sonidos de susto al prefab** los coloca en el `HauntedFurniture`).
Siguiente paso natural: conectar el susto aleatorio con la **barra de cordura**
(por ejemplo: más sustos y más seguido a menor cordura).

## 🧩 Prefabs listos (Assets/Prefabs/Ambience)

Ya hay **prefabs listos para instanciar**, para que el armado sea "arrastrar y usar":

| Prefab | Qué incluye | Cómo usarlo |
|---|---|---|
| `RoomTracker.prefab` | GameObject con `RoomTracker` | Arrastrar a la escena (uno solo). Opcionalmente asignar `playerOverride`. |
| `RoomZone.prefab` | `BoxCollider` (isTrigger, 10×3×10) + `RoomTriggerZone` | Por cada habitación: instanciar, escalar/posicionar para que cubra la sala, y cambiar `roomName`. |
| `HauntedFurniture.prefab` | `HauntedObject` con valores por defecto | Instanciar; **arrastrar el modelo 3D como hijo** del prefab y asignar `nearSounds`/`farSounds`. El `AudioSource` se agrega solo en runtime. |

> **Todos los prefabs traen la configuración por defecto del sistema** (volúmenes, intervalos,
> `proceduralShake` activado, etc.). Basta con arrastrar el modelo 3D dentro y asignar los clips.

### Regenerar los prefabs (opcional)
Los prefabs fueron escritos a mano y referencian a los scripts por GUID fijo. Si algo se desincroniza
o prefieres generarlos desde Unity, hay un generador automático:

1. Abre el proyecto en Unity.
2. Menú **Tools > Arcano XV > Generar prefabs de ambience**.
3. Se reescriben los 3 prefabs con la configuración por defecto y referencias limpias.

## 🎮 Probar el sistema en 1 clic (escena de prueba)

Para probar sin armar nada, en Unity:

1. Menú **Tools > Arcano XV > Generar escena de prueba de ambience**.
2. Se crea y abre `Assets/Scenes/AmbienceTest.unity` con:
   - `RoomTracker` (uno en la escena)
   - `ZonaPrueba` (trigger de 30×6×30 con `RoomTriggerZone`)
   - `MuebleEmbrujado` (cubo con `HauntedObject`, `scareAnywhere = true`, con los sustos asignados)
   - `JugadorPrueba` (cápsula con tag `Player` + `DebugPlayerMover`: WASD + ratón)
   - Piso y luz direccional
3. Presiona **Play**: el mueble **solo** empieza a sonar a intervalos aleatorios (15-40 s,
   con `scareChance`), **sin importar si estás cerca o lejos**. Espera un rato o acércate/aléjate.
4. Para probar el modo clásico, desmarca `scareAnywhere` en el `HauntedObject` y repite:
   - **Acércate** al mueble → reacciones `near` (sacudida procedural).
   - **Aléjate** quedándote dentro de la zona → cada 8-20 s la reacción `far`.

> `DebugPlayerMover` es **temporal** (solo para probar). Cuando exista el controlador real del juego,
> reemplázalo y elimina esta escena o este script.