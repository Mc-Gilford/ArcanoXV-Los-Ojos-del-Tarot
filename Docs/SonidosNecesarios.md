# Sonidos necesarios por objeto (Arcano XV)

> Lista de clips de audio que debe haber en `Assets/Audio/Objetos/<Categoría>/` para que
> cada objeto de la **Habitación Trofeo** suene (y después, el resto de habitaciones).

**Estado actual (2026-08-10):** COMPLETO. Las **18 carpetas** de objetos tienen clip
(incluida `Grilletes`) y `Pasos` tiene 3 crujidos (variedad de tono).
>
> Formato recomendado: **.wav o .ogg** (mejor calidad que .mp3). Cada carpeta debe tener
> su clip. Ponle el nombre que quieras al archivo; solo importa la **carpeta**.
>
> Para crearlas automáticamente: `Tools > Arcano XV > Habitación trofeo: Crear carpetas de audio`.
> Después de pegar los clips: `Habitación trofeo: Asignar sonidos a los objetos`.
>
> Sitios gratuitos para conseguir sonidos CC0/licencia libre:
> [freesound.org](https://freesound.org), [pixabay.com/sound-effects](https://pixabay.com/sound-effects),
> [mixkit.co/free-sound-effects](https://mixkit.co/free-sound-effects).

---

## Habitaciones y sus objetos

### 1 – El Olvido
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Televisor CRT viejo | `Televisor` | estática de televisor (ruido blanco / pantalla encendida) |
| Reloj de pared | `RelojPared` | tic-tac de reloj de pared |
| Espejo empañado | *(silencioso)* | — |
| Mesa de madera | *(silencioso)* | — |
| Vitrina polvorienta | *(silencioso)* | — |

### 2 – El Castigo
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Cadenas colgantes | `Cadenas` | cadenas de metal golpeándose |
| Grilletes en pared | `Grilletes` | metal que choca / grilletes |
| Yunque de fragua | *(silencioso)* | — |
| Mesa de trabajo | *(silencioso)* | — |

### 3 – El Generador
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Generador | `Generador` | zumbido de motor/generador eléctrico |
| Panel eléctrico | `Electricidad` | zumbido eléctrico (50/60 Hz) |
| Bombilla colgante | `Bombilla` | zumbido de bombilla encendida |
| Cables enredados | *(silencioso)* | — |

### 4 – El Ritual
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Altar de ritual | *(silencioso)* | — |
| Velas encendidas | `Velas` | crepitar de llama (candela) |
| Pentagrama en el suelo | *(silencioso)* | — |
| Copas ceremoniales | *(silencioso)* | — |

### 5 – La Reliquia
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Relicario maldito | `Reliquia` | zumbido ominoso bajo / pulso de "objeto maldito" |
| Vitrina de cristal | *(silencioso)* | — |
| Libros antiguos | *(silencioso)* | — |

### 6 – La Cacería
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Cuerno de caza | `CuernoCaza` | cuerno de caza sonando lejos |
| Muro de trofeos | *(silencioso)* | — |
| Escopeta de caza | *(silencioso)* | — |
| Trampa de oso | *(silencioso)* | — |

### 7 – La Investigación
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Máquina de escribir | `MaquinaEscribir` | tecleo de máquina de escribir |
| Teléfono de disco | `Telefono` | timbre de teléfono antiguo |
| Escritorio con notas | *(silencioso)* | — |
| Pizarra de casos | *(silencioso)* | — |

### 8 – El Almacén
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Nevera industrial | `Nevera` | zumbido de motor de nevera/refrigerador |
| Ventilador de piso | `Ventilador` | hélices de ventilador girando |
| Cajas de madera | *(silencioso)* | — |
| Barril oxidado | *(silencioso)* | — |
| Estantería metálica | *(silencioso)* | — |

### 9 – La Corrupción
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Carne colgando | `Moscas` | zumbido de moscas |
| Goteo de agua oscura | `Goteo` | gotas de agua cayendo (eco) |
| Mancha negra | *(silencioso)* | — |
| Huesos | *(silencioso)* | — |

### 10 – La Huida
| Objeto | Categoría (carpeta) | Qué buscar |
|---|---|---|
| Puerta de salida | `Viento` | viento entre puerta / chirrido |
| Linterna vieja | *(silencioso)* | — |
| Maleta abandonada | *(silencioso)* | — |
| Escalera de madera | `EscaleraMadera` | *(opcional)* crujidos de madera al caminar por ella |
| **Pasos del jugador** | `Assets/Audio/Pasos/` | 2–4 crujidos de **piso de madera vieja** |

---

## Resumen: carpetas que deben existir en `Assets/Audio/Objetos/`

```
Assets/Audio/Objetos/
├── Televisor/
├── RelojPared/
├── Cadenas/
├── Grilletes/
├── Generador/
├── Electricidad/
├── Bombilla/
├── Velas/
├── Reliquia/
├── CuernoCaza/
├── MaquinaEscribir/
├── Telefono/
├── Nevera/
├── Ventilador/
├── Moscas/
├── Goteo/
├── Viento/
└── EscaleraMadera/          (opcional)
Assets/Audio/Pasos/          (crujidos de madera para los pasos)
```

**Total: 17 carpetas de objetos + 1 de pasos.**

---

## Cómo funciona cada sonido

- **Objeto con sonido** → componente `ObjectAmbience` (audio 3D, `spatialBlend = 1`).
  Cuanto más cerca estés, más se escucha; se apaga si sales de la habitación (`sameRoomOnly`).
  Si la carpeta de su categoría no tiene clip, el objeto queda en silencio (no rompe nada).
- **Objeto silencioso** → sin `ObjectAmbience`; no emite audio (mesa, silla, caja...).
- **Pasos** → componente `PlayerFootsteps` sobre el jugador: cruje cada `stepDistance`
  metros caminados, con aleatoriedad de tono para que no parezca una grabación.
- El sistema de **sustos** (Near/Far) ya funciona aparte con `Assets/Audio/Sustos/`.

> Nota: los trofeos de la habitación se generan con `ObjectAmbience` armado por
> categoría. Al regenerar la escena se vuelven a leer los clips de las carpetas.