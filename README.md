# Knight Beyond Time

Beat 'em up 2.5D ambientado en un mundo medieval de fantasía oscura, con estética en pixel art. Desarrollado en Unity por **Glitch Studios**.

> Jack Lake, la mayor eminencia científica de su época, es succionado por su propia máquina del tiempo y despierta en **Kokin-Boo**, un reino medieval del siglo XIII. Acusado de invasión por el Rey Paul Eenie y desterrado sin armas ni armadura, deberá abrirse paso de vuelta al castillo, donde quedó su máquina, para regresar a su línea temporal.

## Características

- **Género:** Beat 'em up · **Estilo:** 2.5D pixel art · **Modo:** un jugador
- Movimiento en 8 direcciones y sistema de combate rápido con *dash* de invulnerabilidad
- Tres tipos de armas: **cuerpo a cuerpo**, **a distancia** y **mágicas** (con ataque especial)
- Inventario de 5 espacios para armas y consumibles
- **12 pociones** con distintos efectos (vida, fuerza, velocidad, resistencia, suerte, absoluta, misteriosa, etc.)
- Enemigos con IA por estados (perseguir / atacar / muerto), enemigos especiales y **jefes (Boss)**
- Sistema de **mercader / tienda** con objetos aleatorios
- 4 niveles: **Bosque → Aldea → Exterior del castillo → Interior del castillo**

## Requisitos previos

- [Unity Hub](https://unity.com/download) instalado
- Unity Editor **6000.4.0f1** (Unity 6). La versión exacta está en `ProjectSettings/ProjectVersion.txt`.
- [Git](https://git-scm.com/downloads) instalado

## Cómo clonar el proyecto

```bash
git clone https://github.com/Zacloks/Knight-Beyond-Time.git
```

## Cómo abrir el proyecto en Unity Hub

1. Abre **Unity Hub** → pestaña **Projects**.
2. Haz clic en **Add** → **"Add project from disk"**.
3. Selecciona la carpeta raíz del proyecto (`Knight-Beyond-Time`, la que contiene `Assets`, `Packages`, `ProjectSettings`, etc.).
4. Unity Hub detecta la versión necesaria. Si no la tienes, ofrecerá instalarla, hazlo antes de abrir.
5. Haz clic sobre el proyecto para abrirlo en el Editor.

## Controles

| Acción | Tecla |
|---|---|
| Mover (8 direcciones) | W / A / S / D |
| Atacar / usar consumible equipado | J |
| Ataque mágico | K |
| Esquivar (dash) | Espacio |
| Recoger objeto | P |
| Soltar objeto | L |
| Cambiar objeto de inventario | Q / E |
| Pausa | Esc |

## Estructura del proyecto

```
Knight-Beyond-Time/
├── Assets/            # Contenido del juego (ver detalle abajo)
├── Packages/          # Dependencias y paquetes de Unity
├── ProjectSettings/   # Configuración del proyecto (versión, input, render…)
├── .gitignore
└── README.md
```

### Dentro de `Assets/`

```
Assets/
├── Scripts/        # Todo el código C# (POO), organizado por sistema
│   ├── Player/         # PlayerScript coordina Movement, Combat, Stats,
│   │                   #   Animator, Interaction, Inventory
│   ├── Enemies/        # IA por estados, Boss y habilidades, oleadas (waves),
│   │                   #   combate, drops y tipos de enemigo
│   ├── Weapons/        # Armas melee, a distancia, mágicas y ataques especiales
│   ├── Items/          # Ítems, consumibles, cofres y efectos (patrón Strategy)
│   ├── Manager/        # GameManager, LevelLoader, SceneSetup, AudioManager…
│   ├── Menu/           # Menú principal, pausa, opciones, game over
│   ├── UI/             # HUD, barras de vida/energía, monedas e inventario
│   ├── Tienda/         # Lógica de la tienda del mercader
│   ├── Flow/           # Transiciones y cambios de escena
│   ├── Camera/         # CameraController estilo beat 'em up
│   └── SPUMManagement/ # Integración con el sistema de personajes SPUM
│
├── Scenes/         # Escenas jugables (Inicio, MainMenu, Aldea, CaminoCastillo,
│                   #   InteriorCastillo, Tienda, OptionsMenu, EscenaCarga…)
├── Animations/     # Animator Controllers y clips (Player, Enemies, Merchant, Coin)
├── Prefabs/        # Prefabs (Player, Enemies, Weapons, Items, Cofre, UI, Menus)
├── Sprites/        # Sprites (Enemies, Merchant, Chests, Potions, Weapons, UI)
├── Art/            # Tiles, decoraciones, fondos y arte de menús
├── Resources/      # Assets cargados en runtime: ataques especiales, efectos, ítems
├── Settings/       # Render URP 2D y perfiles de build
├── Music/          # Audio del juego (Ambiente y Efectos)
├── SPUM/           # Paquete SPUM (Soonsoon Pixel Unit Maker) para personajes
└── InputSystem_Actions.inputactions  # Acciones del nuevo Input System
```

## Arquitectura (resumen)

- **Programación Orientada a Objetos en C#**, con una escena por nivel.
- Armas y consumibles heredan de una clase base `Item` para integrarse con el inventario.
- Efectos de pociones modelados con el patrón **Strategy**; el jugador usa **Singleton** para persistir entre escenas.
- Inventario separado en lógica (`Inventory.cs`) y visualización (`InventoryUI.cs`, patrón **Observer**).
- Los niveles solo contienen geometría de colisión y enemigos; los *managers* globales (`GameManager`, `LevelLoader`, `SceneSetup`) instancian jugador, HUD, menús y spawn.

## Equipo, Glitch Studios

| Integrante | Rol |
|---|---|
| Francisco Romero | Scrum Master |
| Martín Droguett | Product Owner |
| Catalina Galleguillos | Developer, arte, estética e integración de sprites |
| Miguel Valenzuela | Developer, combate y armas del jugador |
| Roger Villarroel | Developer, sistema de enemigos y coordinación |

## Flujo de trabajo

Metodología **Scrum** con *sprints* semanales y control de versiones en GitHub siguiendo **GitHub Flow**: rama principal `main` fija, ramas cortas por funcionalidad y *Pull Requests* revisados antes de integrar.