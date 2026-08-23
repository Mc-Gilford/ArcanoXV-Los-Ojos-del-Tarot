# 👁️ Eyes of Tarot

**Terror / Supervivencia / Misterio** — Juego Web (Unity, WebGL)

> *"Aquel que desee cambiar su destino, puede entrar. Pero recuerda: aquello que los ojos contemplan, el destino no olvida."*

---

## 📑 Tabla de contenidos

- [Sobre el juego](#-sobre-el-juego)
- [Controles](#-controles)
- [Mecánicas principales](#-mecánicas-principales)
- [Enemigos](#enemigos)
- [Estilo de arte](#-estilo-de-arte)
- [Audio](#-audio)
- [Stack técnico](#-stack-técnico)
- [Requisitos e instalación](#-requisitos-e-instalación)
- [Estructura del proyecto](#-estructura-del-proyecto)
- [Cómo contribuir](#-cómo-contribuir)
- [Roadmap](#-roadmap)
- [Equipo de desarrollo](#-equipo-de-desarrollo)
- [Estado del proyecto](#-estado-del-proyecto)
- [Créditos y licencia](#-créditos-y-licencia)

---

## 📖 Sobre el juego

Red Zvra, un luchador de Lucha Libre, viaja junto a su hermana rumbo a uno de sus eventos cuando una tormenta los obliga a refugiarse en una antigua y misteriosa mansión. Al entrar, su hermana desaparece y la puerta por la que ingresaron se desvanece.

A partir de ese momento, el jugador debe explorar libremente la casa, sobrevivir a oleadas de enemigos y descubrir la oscura historia de **Rosario Valdés**, una vidente que décadas atrás hizo un pacto con una entidad para intentar salvar a su familia... y terminó perdiéndolo todo.

## 🎮 Controles

| Acción | Tecla | Descripción |
|---|---|---|
| 🚶 Movimiento | `W` `A` `S` `D` | Mueve al personaje en las cuatro direcciones |
| 💨 Correr | `Shift` | Activa el sprint (consume stamina) |
| 👁️ Mirar | Mouse | Controla la dirección de la cámara |
| 🐸 Saltar | `Barra espaciadora` | Salta para esquivar o superar obstáculos |
| 🔫 Disparar | `Q` | Ataque principal / disparo del arma equipada |
| ⚡ Dash | Doble tap `W`/`A`/`S`/`D` | Impulso rápido en la dirección deseada (consume stamina) |
| 🃏 Interactuar / leer carta | `X` | Interactúa con objetos o lee una carta de historia |
| 🃏 Voltear carta | `E` | Voltea la carta seleccionada |
| 🃏 Cerrar carta | `V` | Cierra la carta abierta |
| 🃏 Selección de tarot | `Tab` | Abre el menú de selección de cartas del tarot |
| 🃏 Elegir carta | `J` / `K` / `L` | Navega entre las cartas del tarot |

## 🧩 Mecánicas principales

- **Cordura**: una barra que decrece con el tiempo. Al bajar de cierto umbral, aparece el **Acechador**, una entidad indañable que persigue al jugador. Por debajo de la mitad, se aplica una desventaja que solo se limpia al llegar a cero.
- **Zona segura**: punto de guardado y curación. No genera enemigos, pero obliga al jugador a salir tras un tiempo, y solo vuelve a estar disponible después de un periodo de espera.
- **Cartas del tarot**: en la zona segura, otorgan beneficios con una consecuencia negativa permanente (a partir del segundo uso).
- **Habitaciones únicas**: al entrar a una nueva habitación, la puerta de acceso se bloquea. Existen 10 tipos de habitaciones temáticas con objetivos y eventos propios:

  | # | Habitación | Objetivo |
  |---|---|---|
  | 1 | El Olvido | Encontrar la llave antes de que desaparezca (aparición/desaparición aleatoria de objetos) |
  | 2 | El Castigo | Eliminar a todos los enemigos para desbloquear las puertas |
  | 3 | El Generador | Activar 3 interruptores para restaurar la energía y revelar la llave |
  | 4 | El Ritual | Encender 4 velas mientras se sobrevive a los enemigos |
  | 5 | La Reliquia | Localizar un objeto maldito que revela la posición del jugador a los enemigos |
  | 6 | La Cacería | Eliminar una cantidad determinada de enemigos para que aparezca la llave |
  | 7 | La Investigación | Encontrar notas ocultas que revelan la ubicación de la llave |
  | 8 | El Almacén | Destruir cajas mientras se evita a los enemigos para hallar la llave |
  | 9 | La Corrupción | Completar el objetivo antes de que aumente la dificultad progresivamente |
  | 10 | La Huida | Llegar a la salida visible evitando (no necesariamente eliminando) a los enemigos |

- **Coleccionables**: cartas de tarot (progreso de la búsqueda), pistas (ayudan a resolver habitaciones) y notas (cuentan la historia de Rosario).
- **Recursos**: munición y objetos curativos aparecen por el mapa y desaparecen si no se recogen a tiempo.

### Enemigos

- **Ojos Arañas**: aparecen periódicamente por paredes y techos, atacan saltando sobre el jugador.
- **Acechador (Rosario Valdés)**: persigue al jugador cuando la cordura es ≤ 50%. Una vez recolectadas suficientes llaves, persigue de forma constante y puede teletransportarse cerca del jugador.

## 🎨 Estilo de arte

Estilo **low poly estilizado**, con formas simplificadas, siluetas claras y una estética ligeramente oscura para generar atmósfera de misterio y terror sin recurrir al realismo.

**Referencias visuales:**
- Ojos Arañas → *Glowing Eyeball* de *The Legend of Zelda: Breath of the Wild*
- Acechador → Dementores de *Harry Potter*
- Mansión → Mansión Everglot de *El cadáver de la novia* y la mansión de *Los locos Addams*
- Red Zvra y hermana → Estética de *Spooky's Jump Scare Mansion*

## 🔊 Audio

**Estado:** Implementado (parcial)

- Assets de audio de terror orientados a tensión y atmósfera
- Sonido individual por objeto interactivo/ambiental, ligado al Sistema de Objetos Interactivos Ambientales
- Pendiente: tema musical de persecución del Acechador, música de la zona segura y mezcla general de audio (Audio Mixer)

## 🛠️ Stack técnico

- **Motor:** Unity
- **Plataforma de distribución:** Web (WebGL)
- **Render/Arte:** Low poly estilizado

> Completa aquí la versión exacta de Unity (`ProjectSettings/ProjectVersion.txt`), render pipeline (Built-in / URP / HDRP) y paquetes/plugins relevantes (Input System, Cinemachine, etc.) una vez confirmados en el proyecto.

## 💻 Requisitos e instalación

1. Instala [Unity Hub](https://unity.com/download) y la versión de Unity usada en el proyecto.
2. Clona el repositorio:
   ```bash
   git clone https://github.com/<usuario>/eyes-of-tarot.git
   ```
3. Abre el proyecto desde Unity Hub → **Add** → selecciona la carpeta clonada.
4. Abre la escena principal (`Assets/Scenes/...`) y presiona **Play** para probar en el editor.
5. Para generar el build web: `File > Build Settings > WebGL > Build`.

## 📂 Estructura del proyecto

```
Assets/
├── Animations/        # Animaciones de personajes y enemigos
├── Audio/              # Música y efectos de sonido
├── Materials/          # Materiales y shaders
├── Models/             # Modelos 3D low poly
├── Prefabs/            # Prefabs de habitaciones, enemigos, objetos interactivos
├── Scenes/             # Escenas del juego (menú, casa, zona segura, etc.)
├── Scripts/
│   ├── Player/          # Movimiento, disparo, dash, stamina
│   ├── Enemies/         # Ojos Arañas, Acechador
│   ├── Rooms/           # Lógica de las 10 habitaciones temáticas
│   ├── Tarot/            # Sistema de cartas del tarot y efectos
│   ├── GameManager/      # Control de escenas, timer, score, guardado
│   └── UI/               # Interfaz de usuario
└── Resources/           # Recursos cargados en tiempo de ejecución
```

> Ajusta esta estructura a la organización real del repositorio.

## 🤝 Cómo contribuir

1. Crea una rama a partir de `main`: `git checkout -b feature/nombre-de-la-feature`
2. Sigue las convenciones de nombres del equipo para scripts y assets.
3. Haz commits descriptivos y en español (según el idioma del GDD y la documentación).
4. Abre un Pull Request describiendo los cambios y, si aplica, adjunta capturas/gifs.
5. Al menos un miembro del equipo debe revisar y aprobar antes del merge.

## 🗺️ Roadmap

- [x] Documento de diseño de juego (GDD)
- [x] Sistema de audio por objeto (parcial)
- [ ] Implementación completa de las 10 habitaciones temáticas
- [ ] IA del Acechador (persecución + teletransporte)
- [ ] Sistema completo de cartas del tarot con efectos y consecuencias
- [ ] Audio Mixer y música exclusiva (persecución / zona segura)
- [ ] Sistema de guardado y score
- [ ] Build WebGL optimizado

## 🛠️ Equipo de desarrollo

| Integrante | Rol |
|---|---|
| Aragon Zabala Bryan Andres | Game Developer y Diseñador de UI |
| Cisneros Rosales Christian Ivan | Game Developer y Diseñador de UI |
| Ramirez Andres Daniela | Artista 3D y Creadora de assets |
| Rodriguez Jose de Jesus | Game Developer y Diseñador de audio |

## 📊 Estado del proyecto

- **Plataforma:** Juego Web (Unity WebGL)
- **Audiencia:** Fans del terror y shooters, +12 años
- **Progreso actual:** 20%
- **Fecha de lanzamiento estimada:** 28/08/2026
- **Modelo de monetización:** Pago único de $10 USD por copia

## 📄 Créditos y licencia

Agradecimientos especiales a Josehzz. Hecho con cariño por [Game Dev Underground](http://gdu.io).

© 2017 Game Dev Underground. Libre para usar, modificar y distribuir bajo licencia **CC 4.0**.
