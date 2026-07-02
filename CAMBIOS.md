# Registro de cambios — tuning de combate y movimiento

Este archivo lista cada ajuste hecho al código del juego para que puedas
revertir individualmente si algo se siente mal.

Formato: archivo · qué cambió · cómo revertir.

---

## 2026-06-28 — Globo no flota con el item soltado

### `Assets/Scripts/Items/DroppedItem.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| BB1 | `Start()` captura referencia al globo via `Item.globoInfo` | — | guarda `globoTransform` y `globoBaseLocalY` | Borrar las líneas. |
| BB2 | `Update()` contra-compensa el bobbing en el globo | El globo flotaba con el item al ser hijo | El globo se mantiene fijo, solo el sprite del item flota | Borrar el bloque del `if (globoTransform != null)`. |

**Efecto esperado:** los items siguen flotando como antes con sombra elipse, PERO el globo de info se queda quieto encima del item, no flota con él. Más prolijo visualmente.

---

## 2026-06-28 — Re-fix del globo de info de items (texto default + auto-size)

### `Assets/Scripts/Items/Item.cs` (reescritura)

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| AA1 | `SetActive(false)` del globo movido a `Awake()` | Estaba en `Start()` — no se ejecutaba para items de tienda (Consumible.Start lo oculta) | En Awake → corre siempre | Volver a poner la línea en `Start()`. |
| AA2 | `textoInfo.text = ""` en Awake | El TMP del prefab traía `"aaaaaaaaaa..."` y se mostraba | Lo limpia | Borrar la línea. |
| AA3 | Re-añadido `AjustarFondoAlTexto()` | No existía en la versión revertida | Auto-redimensiona el fondo según el alto del texto | Borrar el método y la llamada en `MostrarInfo()`. |
| AA4 | Re-añadida descripción en `MostrarInfo()` | Solo mostraba nombre + precio | Ahora `<b>nombre</b> + descripción + precio` | Quitar la línea `string desc = …`. |
| AA5 | Nuevos campos en Inspector: `fondoGlobo`, `anchoGlobo`, `altoMinimo`, `altoMaximo`, `fondoPadding`, `margenInferior` | — | Defaults: `2.2`, `0.4`, `2.5`, `(0.1, 0.15)`, `0.1` | Borrar las declaraciones. |

**Por qué pasaba:**
- El globo aparecía con el texto `"Sample text"` (o `"aaaaaaa..."`) porque el prefab tiene `Globo.m_IsActive: 1` y `Item.Start()` (que lo apagaba) está sombreado por `Consumible.Start()`.
- El texto crecía del recuadro porque no había auto-sizing — el `fondoGlobo` se quedaba en su size de prefab `0.8 × 0.4` sin importar el largo del texto.

**Cómo revertir todo este bloque rápido:** restaurar la versión simplificada de `Item.cs` (la que solo tiene `Start()` + `MostrarInfo()` + `OcultarInfo()` sin `AjustarFondoAlTexto`).

---

## 2026-06-28 — IA: surround táctico + cofre re-conectado

### `Assets/Scripts/Player/PlayerInteraction.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| Y1 | Nuevo campo `cofreCercano` | — | `private Cofre cofreCercano;` | Borrar la línea. |
| Y2 | `Update()` ahora abre cofre si no hay item alcanzable | solo `TryPickUp()` | `else if (cofreCercano != null) cofreCercano.Abrir();` | Borrar la línea del `else if`. |
| Y3 | `OnTriggerEnter2D` detecta tag `Cofre` | sin detección | setea `cofreCercano` | Borrar el bloque del if `"Cofre"`. |
| Y4 | `OnTriggerExit2D` discrimina por tag | nullificaba `alcanzable` con cualquier exit | solo lo nullifica si sale `"Consumible"`; nullifica `cofreCercano` si sale `"Cofre"` | Restaurar el cuerpo original (siempre `alcanzable = null`). |

### `Assets/Scripts/Enemies/Components/MovementEnemy.cs` (reemplaza al sistema de `anguloPersonal` aleatorio fijo)

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| Z1 | `anguloVariacion` default | `15f` | `8f` (ahora solo "jitter" entre enemigos que eligen el mismo lado) | Volver a `15f`. |
| Z2 | Nuevo campo `intervaloElegirAngulo` | — | `0.4f` | Subir a `999f` → el ángulo nunca se recalcula (se queda con el inicial random). |
| Z3 | Nuevo campo `candidatosAngulo` | — | `8` | Sin efecto si `intervaloElegirAngulo` es muy alto. |
| Z4 | Nuevo campo `pesoContinuidad` | — | `0.6f` | `0` = siempre toma el ángulo óptimo (jittery). `2+` = pega al ángulo actual. |
| Z5 | Nuevo campo `radioEscaneoSurround` | — | `5f` | Rango en el que escanea otros enemigos. |
| Z6 | Nuevos campos privados `anguloPreferido`, `ultimoRecalculo` | — | — | — |
| Z7 | `Start()` inicializa `anguloPreferido` aleatorio | — | random `[0, 2π]` | Borrar la línea. |
| Z8 | `Chase()` reemplaza la rotación de `fromPlayer` por `ElegirAnguloAtaque` | aplicaba `anguloPersonal` como offset al `fromPlayer` original | recalcula el ángulo periódicamente y usa el lado menos cubierto | Restaurar el código original de `Chase()`. |
| Z9 | Nuevo método `ElegirAnguloAtaque()` | — | Evalúa N candidatos angulares, devuelve el con mayor gap a otros enemigos | Borrar el método. |

**Efecto esperado:**
- Los enemigos **se distribuyen alrededor del jugador** automáticamente: cada uno ocupa el lado con menos competencia.
- Cuando hay 4 enemigos, naturalmente se reparten ~90° entre sí (N/S/E/O).
- Si matas a uno, los demás recalculan en ~0.4s y se reposicionan para cubrir el hueco.
- El jugador recibe daño desde varios lados, no solo del frente.

**Cómo revertir todo este bloque rápido (sin tocar código):**
- `MovementEnemy → Intervalo Elegir Angulo = 999`
- `MovementEnemy → Peso Continuidad = 2`

Eso congela el ángulo en uno random inicial, comportamiento similar al de antes.

---

## 2026-06-28 — IA: evasión + spread + stagger de ataques

### `Assets/Scripts/Enemies/Components/MovementEnemy.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| L | Nuevo campo `lookAhead` | — | `0.7f` | Setear a `0` en el Inspector → desactiva la evasión sin tocar código. |
| M | Nuevo campo `lookAheadRadius` | — | `0.25f` | Sin efecto si `lookAhead = 0`. |
| N | Nuevo campo `avoidanceStrength` | — | `0.65f` | Setear a `0` en el Inspector → no esquiva. |
| O | Nuevo campo `anguloVariacion` | — | `15f` | Setear a `0` → enemigos vuelven a aproximarse desde la línea recta a su posición actual. |
| P | `Start()`: inicializa `anguloPersonal` | — | `Random.Range(-anguloVariacion, anguloVariacion)` | Borrar la línea. |
| Q | `Chase()`: aplica `anguloPersonal` al punto del anillo | sin rotación | `Quaternion.Euler(0,0,anguloPersonal) * fromPlayer` | Borrar la línea de la rotación. |
| R | `Chase()`: llama a `AplicarEvasion()` antes de asignar `moveDirection` | asignaba directamente | pasa por evasión | Reemplazar por `moveDirection = desiredDir;` |
| S | Nuevo método `AplicarEvasion()` | — | `CircleCastAll` + steering perpendicular | Borrar el método completo. |

### `Assets/Scripts/Enemies/Combat/AttackEnemy.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| T | Nuevo campo `staggerAtaque` | — | `0.25f` | Setear a `0` → ataques no se desincronizan. |
| U | Nuevo campo `currentAttackCooldown` | — | `timeBetweenAttacks ± staggerAtaque` | — |
| V | `Update()` usa `currentAttackCooldown` en vez de `timeBetweenAttacks` directo | `lastAttackTime + timeBetweenAttacks` | `lastAttackTime + currentAttackCooldown` | Devolver el chequeo original. |

### `Assets/Scripts/Enemies/Combat/AttackEnemyRanged.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| W | Nuevo campo `staggerAtaque` | — | `0.35f` | Setear a `0`. |
| X | Mismo cambio que V pero en versión ranged | — | usa `currentAttackCooldown` | Devolver el chequeo original. |

**Efecto esperado:**
- Los enemigos **se reparten alrededor del jugador** en lugar de pegarse todos al mismo lado (ángulo variación).
- **Esquivan a otros enemigos** que les bloquean el paso en lugar de chocar (look-ahead steering).
- **No atacan todos al mismo tiempo** — puedes reaccionar uno por uno (stagger).

**Para deshacer todo este bloque rápido sin tocar código:** en los prefabs:
- `MovementEnemy → Look Ahead = 0`
- `MovementEnemy → Angulo Variacion = 0`
- `AttackEnemy / AttackEnemyRanged → Stagger Ataque = 0`

---

## 2026-06-28 — Enemigos quietos al atacar/recibirse + knockback más corto

### `Assets/Scripts/Enemies/Components/MovementEnemy.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| A | `knockbackDamping` default | `0.85` | `0.7` | Volver a poner `0.85` en el campo del Inspector de cada prefab (o cambiar el default en el script). |
| B | Nuevo campo `velocidadMaxKnockback` | — | `4f` | Borrar la declaración del campo y el bloque de clamp en `HurtBehavior()`. |
| C | Nuevo campo `knockbackMultiplicador` | — | `0.55f` | Subir el valor a `1f` en el Inspector (deshace el efecto sin tocar código). |
| D | `AttackBehavior()` zerea velocidad cada frame | velocidad heredada de Chase | `rb.linearVelocity = Vector2.zero` | Borrar la línea `rb.linearVelocity = Vector2.zero;` dentro de `AttackBehavior()`. |
| E | `HurtBehavior()` clamp de velocidad max | sin clamp | clamp a `velocidadMaxKnockback` | Borrar el bloque del `if (rb.linearVelocity.sqrMagnitude > ...)` |
| F | `ChangeToStateHurt()` escala la fuerza | `AddForce(knockbackForce, …)` | `AddForce(knockbackForce * knockbackMultiplicador, …)` | Quitar el `* knockbackMultiplicador`. |

**Efecto esperado:** enemigos quietos durante su ventana de ataque (más fáciles de acertar), velocidad de retroceso topada a `4` u/s, y la fuerza global del knockback recibido reducida al 55%.

**Para deshacer todo este bloque rápido:** en el Inspector de cada prefab enemigo, poner:
- `Knockback Multiplicador = 1`
- `Knockback Damping = 0.85`
- `Velocidad Max Knockback = 100`

Eso anula los tres efectos sin tocar el script (excepto el cambio D que es código duro).

---

## 2026-06-28 — Velocidad bajada y daño enemigo aumentado

### `Assets/Scripts/Player/PlayerMovement.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| G | `velocidadMov` default | `7f` | `5.5f` | Volver a `7f` en el script o setear `7` en el Inspector del Player. |
| H | `dashSpeed` default | `12f` | `10f` | Volver a `12f`. |

### `Assets/Scripts/Enemies/Components/MovementEnemy.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| I | Multiplicador global de velocidad enemiga (línea de `FixedUpdate`) | `* 1.6f` | `* 1.3f` | Cambiar `1.3f` por `1.6f` en la línea `Vector2 desired = moveDirection * movementSpeedBase * 1.3f;` |

### `Assets/Scripts/Enemies/Core/Enemy.cs`

| # | Cambio | Antes | Ahora | Cómo revertir |
|---|---|---|---|---|
| J | Nuevo campo `multiplicadorDaño` | — | `1.5f` | Bajar el campo a `1f` en el Inspector de cada prefab enemigo (deshace el efecto sin tocar código). |
| K | `CalcularDanoConCritico()` aplica el multiplicador | daño base directo | `Mathf.RoundToInt(danoBase * multiplicadorDaño)` | Si quieres dejarlo todo como antes, además quita las dos líneas que multiplican por `multiplicadorDaño` y vuelve al `return danoBase`. |

**Efecto esperado:** jugador ~21% más lento, todos los enemigos ~19% más lentos, daño enemigo +50%.

---

## Cambios previos en esta sesión (resumen)

Los siguientes archivos también fueron modificados pero no se trackean aquí
porque ya están integrados en la lógica del juego (globo de info, cofre, flash
rojo del jugador, etc.). Si necesitas revertir alguno específico, dímelo y lo
documentamos.

- `Assets/Scripts/Items/Item.cs` — campos del globo de info, MostrarInfo/OcultarInfo, AjustarFondoAlTexto.
- `Assets/Scripts/Items/ItemData.cs` — campo `descripcion`.
- `Assets/Scripts/Items/Consumible.cs` — llamada a `ConsumeEquippedItem`.
- `Assets/Scripts/Items/Cofre.cs` — nuevo, sistema de cofres.
- `Assets/Scripts/Player/PlayerInteraction.cs` — handling de globo + cofres.
- `Assets/Scripts/Player/PlayerInventory.cs` — nuevo método `ConsumeEquippedItem()`.
- `Assets/Scripts/Player/PlayerStats.cs` — flash rojo al recibir daño.
