# 🔊 Sistema de Ambience Paranormal — Guía de uso

Sistema que hace "viva" la casa: detecta cuándo el jugador está cerca de un objeto o en una habitación
y dispara **sonidos y animaciones aleatorias** para generar sustos y tensión (GDD: características de
cordura y "sonidos aleatorios que mantendrán al usuario en alta alerta").

## Componentes

| Script | Rol |
|---|---|
| `RoomTracker` | Singleton que sabe en qué habitación (`RoomTriggerZone`) está el jugador. Emite eventos `PlayerEnteredRoom` / `PlayerExitedRoom`. |
| `RoomTriggerZone` | Volumen invisible (collider `isTrigger`) que delata a cada habitación. Se coloca uno por sala y reporta al `RoomTracker`. |
| `HauntedObject` | Se añade a cualquier mueble/prop. Reacciona con sonidos y animaciones aleatorias **cerca** del jugador y con sonidos inesperados **lejos pero en la misma habitación**. |

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
- Arrastra los **AudioClips** de susto a `nearSounds` (cerca) y/o `farSounds` (lejos, misma habitación).
- Opcional: si el objeto tiene `Animator`, asigna `animationTriggers` (nombres de los triggers). Si no,
  deja `proceduralShake = true` y el objeto hará una sacudida de susto automática.
- En el editor, al seleccionarlo verás un **círculo naranja** = radio de "cerca".

## Cómo funciona cada tarea del GDD

| Tarea | Componente | Comportamiento |
|---|---|---|
| Detección del jugador → sonidos/animaciones aleatorias | `HauntedObject` | Cuando el jugador entra al círculo de `proximityDistance`, el objeto reacciona cada `min/maxIntervalNear` con un clip aleatorio y una animación (trigger o sacudida). |
| Sonidos inesperados cerca o lejos en la misma habitación | `HauntedObject` + zonas | Si el jugador está en la misma `RoomTriggerZone` pero fuera del círculo, suenan los `farSounds` a intervalos largos y con menor volumen. Si está cerca, suenan los `nearSounds`. |

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

Los sonidos de susto (`nearSounds`/`farSounds`) aún no existen como AudioClips. Puedes usar efectos
gratuitos tipo *scary ambience*, y más adelante este sistema se integra con la **barra de cordura**
(por ejemplo: más sustos a menor cordura).

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