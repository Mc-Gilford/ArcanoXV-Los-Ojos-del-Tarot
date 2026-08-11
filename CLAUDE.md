# CLAUDE.md — Contexto del proyecto

## ¿De qué se trata?
Juego **survival horror**: "Arcano XV: Los Ojos del Tarot" (el GDD lo llama *Holy Mother Monsters*).
El jugador es el cazador de demonios **Gretel**, que explora una casa paranormal buscando a su hermana **Hazel**, sobreviviendo a oleadas de enemigos.

## Dónde está cada cosa
- **Proyecto Unity** (este repo): carpeta `Arcano-XV Los Ojos del Tarot/` (Unity **6000.3.16f1**).
- **GDD (PDF)**: `D:\Claude\ProyectoFinal\GDD Juego de Hockey de mesa.pdf` (el nombre "Hockey" es un typo; es el GDD del juego).
- **Docs**: `Docs/SistemaAmbience.md` y `Docs/AssetsGratuitosMobiliario.md`.
- **Rama de trabajo**: `Bryam` (main = lo que ven/usan los compañeros; otras ramas: ChristianBranch, CisnerosBranch, develop).

## GDD en pocas líneas
- Sobrevivir oleadas de enemigos (temporizador en decremento, los rounds escalan variedad/cantidad).
- **Barra de cordura** que baja con el tiempo; a cierto % aparece un **acosador** invulnerable que te persigue.
- **Zona segura** para guardar, curar y recuperar cordura (tras guardar te obliga a salir; deseos con efecto bueno+negativo desde el 2º uso).
- Sonidos aleatorios para mantener al jugador en alerta (sustos).
- Al entrar a una habitación se bloquea la anterior + probabilidad de evento.
- 10 habitaciones temáticas: El Olvido, El Castigo, El Generador, El Ritual, La Reliquia, La Cacería, La Investigación, El Almacén, La Corrupción, La Huida.
- Interfaz: caminar, correr, disparar, interactuar con puertas y con la **mesa de adivinación** de la sala segura.

## Estado actual (agosto 2026)
Construido y commiteado en `Bryam`:
- **Sistema de ambience paranormal**: `RoomTracker`, `RoomTriggerZone`, `HauntedObject`.
- **Prefabs** en `Assets/Prefabs/Ambience` (RoomTracker, RoomZone, HauntedFurniture).
- **Generadores por menú**: `Tools > Arcano XV > Generar prefabs de ambience` y `Generar escena de prueba de ambience`.
- `DebugPlayerMover` temporal (WASD+ratón) solo para pruebas; `AmbienceTest.unity` se genera por menú, no está en el repo.
- La escena persistente es `Assets/Scenes/SampleScene.unity`.

## Pendiente / siguiente
- Sonidos de susto (`nearSounds`/`farSounds`) — no hay AudioClips aún.
- Barra de cordura, oleadas/enemigos, zona segura, mesa de adivinación, etc.

## Cómo trabajar conmigo
- **No puedo ver imágenes** (el proxy las bloquea): no me mandes capturas; verifica visualmente y yo reviso código/logs.
- Bryam autoriza actuar sin confirmar cada paso; informar al final.
- Al final de cada tarea: resumen claro de qué se hizo, con rutas de archivos.