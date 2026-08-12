# 🎴 Cartas del Tarot — Poderes, Selección y Coleccionables

Mecánica nueva para Arcano XV (tema del GDD: "deseos con efecto bueno+negativo").
Son dos sistemas independientes:

1. **Selección de carta** (habilidad activa con cooldown): un poder + una maldición de movimiento.
2. **Cartas coleccionables** (jefe final): recoger 5 cartas con **E** abre la puerta del jefe final.

---

## 1. Selección de carta (habilidad activa)

- **Activar:** `Tab` (solo si el cooldown terminó). Abre el panel con 3 cartas.
- **Mientras eliges:** el movimiento WASD se congela, pero **la cámara/ratón queda libre**
  (no interfiere con la cámara ni con el movimiento).
- **Elegir:** `J` / `K` / `L` → animación de toma + se anuncia la **maldición** de la carta.
- **Salir sin elegir:** `Espacio` (o `Tab` otra vez) → **penalización** (movimiento).
- **Cooldown:** 45 s (campo `cooldownHabilidad`; bajarlo a ~5 s para probar rápido).

### Las 3 cartas (solo efectos de movimiento)

| Carta | Panel | Poder | Maldición |
|---|---|---|---|
| **El Carro** | Verde | +60 % velocidad, 12 s | Sin sprint, 12 s |
| **La Estrella** | Azul | +25 % velocidad, 12 s | −30 % velocidad, 15 s |
| **El Ahorcado** | Blanco | +100 % velocidad, 8 s | Sin sprint + −50 % velocidad, 15 s |

La maldición se aplica al agotarse el poder (la carta se "cobra" su precio después);
la maldición de sprint (El Carro / El Ahorcado) arranca desde el primer instante.

### Scripts

- `Assets/Scripts/Cartas/CardDef.cs` — datos de cada carta + `CardDef.Defaults()` (las 3).
- `Assets/Scripts/Cartas/CardSelectionSystem.cs` — UI por código (Canvas + cuadros
  verde/azul/blanco, sin assets ni EventSystem), estados, cooldown y efectos.
- `Assets/Scripts/Player/DebugPlayerMover.cs` — ganchos que usa el sistema:
  `lockMovement`, `speedMultiplier`, `canSprint`.

### Si se integra el controlador real (Jugador.cs)

El sistema lee `Keyboard.current` directo y modifica `DebugPlayerMover`. Cuando el equipo
conecte `Jugador.cs` (PlayerInput), replicar los tres campos y usar
`CardSelectionSystem.SeleccionAbierta` para gatear acciones (ej. Saltar = Espacio) mientras
se elige.

---

## 2. Cartas coleccionables → puerta del jefe final

- Hay **5 cartas** repartidas en la sala (anillo de radio 25 m en `HabitacionTrofeo`).
- Al acercarse aparece una **"E"** flotando; pulsa **E** para recogerla.
- El contador **"Cartas: X/5"** se ve abajo a la derecha.
- Al reunir las **5**, la **PuertaJefeFinal** (antes bloqueada, roja) se eleva y se pone verde.

### Scripts

- `Assets/Scripts/Cartas/CardCollector.cs` — contador en el jugador + HUD; evento
  `OnTodasRecogidas` para abrir la puerta.
- `Assets/Scripts/Cartas/CardPickup.cs` — carta que flota/gira; recoger con E (distancia).
- `Assets/Scripts/Cartas/BossDoor.cs` — puerta que se abre al tener todas las cartas.

### Modelo de la carta

`Assets/Models/Cartas/scene.gltf` (importado con glTFast, ya incluido en `Packages/manifest.json`).

---

## Crédito del modelo (CC-BY-4.0)

> This work is based on "KawaiChan Card Item" (https://sketchfab.com/3d-models/kawaichan-card-item-ba2349252d9d4ff991024b2d94c17ee0) by RavenBlox (https://sketchfab.com/RavenBlox) licensed under CC-BY-4.0 (http://creativecommons.org/licenses/by/4.0/).
>
> Se debe incluir este crédito donde se comparta el juego (itch.io, build, créditos del menú).

---

## Cómo regenerar y probar

1. Abrir Unity sobre la **RAÍZ** del repo (`D:\unity\ArcanoXV-Los-Ojos-del-Tarot`).
2. `Tools > Arcano XV > Generar TODO` (regenera escenas y prefab del jugador con los
   componentes nuevos; **imprescindible** o no aparecerá la mecánica).
3. Abrir `Assets/Scenes/HabitacionTrofeo.unity` → Play:
   - `Tab` → elige con J/K/L (o Espacio para la penalización).
   - Acércate a las 5 cartas → `E` → la puerta del jefe se abre.
4. Consola sin errores.
