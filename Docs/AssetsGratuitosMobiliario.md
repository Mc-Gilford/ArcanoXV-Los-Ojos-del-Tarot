# 🛋️ Assets Gratuitos para Amoblar la Casa — Arcano XV

Documento con recomendaciones de assets **gratuitos** para armar el interior de la casa paranormal.
Los muebles son la base de la ambientación: no solo decoran, sino que cada uno puede recibir el
componente `HauntedObject` (sonidos y animaciones aleatorias) del sistema de ambience.

---

## ⭐ Fuente principal recomendada: Poly Haven (CC0, 100% gratis)

**Sitio:** https://polyhaven.com/models

- Licencia **CC0**: puedes usar los modelos en cualquier proyecto, comercial incluido, sin atribución.
- Formato: **.glTF / .blend / .usd**. En Unity se importa el `.gltf` (arrastrar a la escena) o `.fbx` exportado.
- Tienen categorías de **Furniture** (muebles), **Lighting** (lámparas), **Decor & Art**, **Architecture** y más.
- Ejemplos verificados que ya están en el catálogo:
  - `mid_century_lounge_chair` — sillón (Furniture)
  - `Chandelier_01` — candelabro/iluminación (Lighting)
  - `marble_bust_01` — busto de decoración (Decor)
  - `pocket_watch` — reloj de bolsillo (Decor)
- **Cómo buscar:** entra a https://polyhaven.com/models/furniture

> Nota: el catálogo crece todo el tiempo, así que revisa la categoría Furniture/Lighting periódicamente.

---

## 🧰 Otras fuentes CC0 confiables

### Kenney — https://kenney.nl/assets
- Pack gratuitos (mayormente 2D/UI, pero también kits 3D modulares). Licencia CC0.
- No siempre tienen mobiliario 3D, pero sí kits modulares útiles para ambientar (cajas, estantes, props).
- Útil sobre todo para el **Almacén** (cajas destructibles) y props sueltos.

### Quaternius — https://quaternius.com
- Packs **low-poly** gratuitos (CC0), varios de props y mobiliario.
- Ideal si el equipo decide un estilo **stylized/low-poly** para la casa.

---

## 🛒 Dentro de la Asset Store de Unity (filtrar por "Gratis")

No puedo entrar a la Asset Store por código, así que la regla de oro:

1. Abre **Window > Asset Store** en Unity y busca `furniture` o `house interior`.
2. Filtra el resultado con **Price = Free**.
3. Revisa la **categoría 3D > Props / Interior**.
4. Busca también por autor: **"Quality Assets"** (autor que publica packs de props "Unique ..." de alta calidad de forma gratuita) y **"nexassets"**.
5. **Verifica la licencia** de cada pack antes de importarlo a un proyecto que irá a Steam (algunos "free" solo son para proyectos no comerciales).

---

## ✅ Mobiliario mínimo según las 10 habitaciones del GDD

Conseguir el mobiliario correspondiente a cada sala hace que cada una se "lea" distinta y apoye su mecánica:

| Habitación | Mobiliario/props a buscar |
|---|---|
| El Olvido | Estanterías, relojes, mesas con llaves encima |
| El Castigo | Mobiliario pesado (taquillas, muebles altos), puertas |
| El Generador | Cajas, engranajes, paneles, cables (más industrial que doméstico) |
| El Ritual | **Velas**, mesas de altar, cuencos, telas |
| La Reliquia | Pedestales, vitrinas, bustos, objetos malditos |
| La Cacería | Trofeos, cuadros, cabezas decorativas |
| La Investigación | **Escritorio, papeles, carteles, archivadores, lámpara de mesa** |
| El Almacén | **Cajas/cajones** (varios, repetibles), estantes industriales |
| La Corrupción | Muebles "degradados/retorcidos", sillas flotantes |
| La Huida | Corredores, bancos, puertas dobles |

Base común para toda la casa: **sillón, sofá, cama, mesas de centro, sillas, armario, lámparas, alfombras, cocina**.

---

## 🔧 Buenas prácticas al importar

- **URP:** el proyecto usa Universal Render Pipeline. Prefiere modelos con **PBR/metálico** y reasigna los materiales a shaders URP si vienen en Built-in (o usa `Render Pipeline Converter` de Unity: *Window > Rendering > Render Pipeline Converter*).
- **Escala:** al importar, comprueba que el mueble está en escala humana (1 unidad ≈ 1 metro). Ajusta con un cubo de 1 m de referencia.
- **Colliders:** añade `Mesh Collider` (o `Box Collider` aproximado) a los muebles con los que el jugador collide.
- **Como prefab:** una vez importado, hazlo un **Prefab** e instáncialo en las habitaciones. Así puedes añadirle el `HauntedObject` una sola vez al prefab base y se propaga a todas las copias.

---

## 📦 Inventario descargado (2026-08-11)

Todo extraído y organizado en **`Assets/Models/<Categoría>/<nombre>/`** (cada carpeta con
su modelo + `textures/`). Categorías: `Base`, `Cocina`, `Olvido`, `Castigo`, `Generador`,
`Ritual`, `Reliquia`, `Caceria`, `Investigacion`, `Almacen`.

**📦 FORMATOS — ya convertidos a `.glb` (nadie necesita Blender):**
- Los **37 `.blend`** originales se convirtieron a **`.glb` con texturas incluidas** con Blender
  headless, en `Assets/Models/GLB/<Categoría>/<modelo>.glb` con texturas a **máx. 2048px**
  (el proyecto bajó de ~3 GB a ~850 MB). Los `.blend` originales se resguardaron en
  `BlendOriginal/` (fuera de `Assets`, por si hay que re-exportar).
- Los **5 `.gltf`** (Alfombra, Almacen, Cadenas, Escritorio, Altar_cabeza) se importan directo
  con glTFast (ya registrado en `Packages/manifest.json`).
- Para que los modelos se importen en Unity solo hace falta glTFast (baja solo). **Blender
  solo es necesario si quieren volver a tocar los `.blend`** (Edit > Preferences > External Tools).
- **La huida no tenía modelo:** `corredores blancos` era una **textura JPG**; quedó en
  `Assets/Textures/Huida/corredor_blanco.jpg` como textura de pared.

**⚠️ `Reliquia_bustos` está mal descargado:** el zip contiene `grass_medium` (césped), no bustos.
   Re-descargar de Poly Haven en su lugar: buscar **"bust"** (ej. `marble_bust_01`).

**Notas de montaje (los marqué al organizar):**
- Algunos son **escenas/props grandes** que hay que ubicar: `Generador/engranajesytuberias`
  (modular_pipes: sistema de tuberías), `Ritual/Altar_velas` (candelabros de latón).
- Varios son **objetos sueltos que necesitan una superficie** donde apoyarse (tazas, comida, velas,
  relojes → van sobre mesas/estanterías).
- `Reliquia/Pedestal` trae una `gothic_statue` (estatua), no un pedestal: igual sirve para la sala.
- `Reliquia/Vitrina` trae una `worn_metal_rack` (estante metálico gastado): útil como vitrina/estante.

---

## 🏠 Habitaciones reales del GDD (generador)

`Tools > Arcano XV > Generar habitaciones` (también sale con **Generar TODO**) arma las
**10 habitaciones** del GDD como escenas en `Assets/Scenes/Habitaciones/`:

- Estructura: piso, paredes con vano de puerta, techo y luz tenue de terror.
- **RoomTracker + zona de sonido** con el nombre de la sala (sistema de ambience).
- Mobiliario con los **modelos reales de `Assets/Models`** (los que no tienen modelo usan
  la primitiva de respaldo con su sonido: TV, máquina de escribir, teléfono, nevera...).
- **Sonido por contacto** (`ObjectAmbience`) y **susto aleatorio** (`HauntedObject`) en cada mueble.
- Jugador temporal con pasos de madera para probar.

**La Huida:** no tiene modelo 3D (el zip era una textura), así que su sala usa
`Assets/Textures/Huida/corredor_blanco.jpg` en las paredes + muebles base + puerta de salida.

> Los planos de muebles (posición, rotación, escala y qué modelo usan) se editan en
> `Assets/Scripts/Ambience/RoomsLayouts.cs`. Si un modelo aún no importa (falta Blender/glTFast)
> verás avisos "Modelo no importado aún" y se usa la primitiva; al repetir el comando
> cuando Blender esté listo se reemplazan solos.

**📏 Medición (2026-08-11) — polígonos y tamaño por modelo (ya con texturas 2k):**
- La mayoría está entre **1k–20k triángulos** (correcto para un juego).
- Los más pesados: tubos del generador **94k**, archivadores/libros **68k**, trofeo león **47k**,
  candelabros **42k**, set de té **40k**. Si algún FPS/espacio los exige, se pueden decimar con
  Blender o usar LODs — es un punto de optimización opcional, no urgente.
- Tamaño en disco: total **~850 MB** en `Assets/Models/GLB/` (los gordos son `modular_pipes` y
  `brass_candleholders`, ~95 MB c/u).