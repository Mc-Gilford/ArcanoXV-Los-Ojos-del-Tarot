# Guía de integración — Arcano XV: sistemas de sonido

> Cómo usar lo que ya está hecho para implementarlo con las habitaciones del equipo.
> Todo se regenera con un solo clic: **`Tools > Arcano XV > Generar TODO`**.

---

## 1. ¿Qué incluye este sistema?

| Sistema | Script | Cómo se comporta |
|---|---|---|
| **Sonido de objeto por contacto** | `ObjectAmbience` | Al **tocar** el objeto suena su clip **3 s** y se apaga. Silencio total hasta tocarlo. |
| **Pasos de madera** | `PlayerFootsteps` | Crujidos de piso viejo al caminar; un crujido acaba antes de sonar el siguiente; se corta al detenerte. |
| **Habitaciones** | `RoomTracker` + `RoomTriggerZone` | Saben en qué habitación está el jugador (para sustos y eventos por sala). |
| **Sustos Near/Far** | `HauntedObject` | Sonido tenue de fondo + susto fuerte al acercarse. Clips en `Assets/Audio/Sustos/`. |
| **Controlador de prueba** | `DebugPlayerMover` | WASD para moverte, **Shift para correr**, ratón para mirar. **No es el controlador final.** |

---

## 2. Prefabs listos (solo arrastrar)

- **`Assets/Prefabs/Objetos/*.prefab`** → los **18 objetos CON sonido** (nevera, televisor, reloj, generador, etc.). Arrastra a tu habitación y listo: suenan 3 s al tocarlos. **No requieren zona.**
- **`Assets/Prefabs/Player/Jugador_Pasos.prefab`** → jugador de prueba con pasos de madera y sprint (Shift).
- **`Assets/Prefabs/Ambience/*.prefab`**:
  - `RoomTracker` → **uno por escena**.
  - `RoomZone` → un trigger por habitación (pónle el nombre en `roomName`).
  - `HauntedFurniture` → mueble embrujado (sustos).

---

## 3. Reglas rápidas

1. **1 `RoomTracker` por escena.**
2. **1 `RoomZone` por habitación** (BoxCollider trigger sobre la sala, tag `Player`).
3. Los **prefabs de objetos** no dependen de zonas: suenan por contacto.
4. Si el equipo usa **su propio jugador**: agreguen el componente `PlayerFootsteps` y ponganle los clips de `Assets/Audio/Pasos/`. Para correr con Shift, multipliquen la velocidad (ver `DebugPlayerMover.sprintMultiplier`).

---

## 4. Ajustes útiles

- **`ObjectAmbience`**: `playDuration` (3 s), `triggerRadius` (1.5 m = casi al tocar), `baseVolume`.
- **`PlayerFootsteps`**: `stepDistance` (0.75 m entre pasos), `volume`, `pitchJitter`.
- **`DebugPlayerMover`**: `moveSpeed` (5), `sprintMultiplier` (2.2).

---

## 5. Audio

```
Assets/Audio/
├── Objetos/<Categoría>/   → 18 clips (uno por prefab de objeto)
├── Pasos/                 → 3 crujidos de madera
└── Sustos/{Near, Far}     → sustos del sistema HauntedObject
```

Los clips se asignan **solos** al regenerar (se buscan por nombre de carpeta).

---

## 6. Menú único

**`Tools > Arcano XV > Generar TODO`** regenera todo de una vez:
prefabs de sustos → escena de prueba → prefabs de objetos (solo los con sonido) → prefab del jugador → **`HabitacionTrofeo.unity`** (sala circular con los 18 objetos para probar cada sonido).

---

## 7. Buenas prácticas (para la evaluación)

- **No escribir `.meta` a mano** — Unity los genera (mover un script con su `.meta` preserva las referencias).
- Scripts ordenados: `Scripts/Ambience` (sonidos y ambiente), `Scripts/Player` (controlador y pasos), `Scripts/Editor` (generadores).
- Prefabs agrupados: `Prefabs/Objetos` (sonoros), `Prefabs/Player`, `Prefabs/Ambience`.
- Documentación en `Docs/` (`SistemaAmbience.md`, `SonidosNecesarios.md`, esta guía).
