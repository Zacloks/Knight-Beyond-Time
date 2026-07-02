# Tienda: items según la escena de origen

## Objetivo

Que los items que ofrece la tienda dependan de la escena desde la que el jugador
entró. Se reutiliza el mismo sistema de tablas por nivel que ya usan los enemigos
para soltar drops (`TablaDropsNivel` + `DropsDelNivel`): la tienda venderá los
mismos `ItemData` que aparecen en la tabla de drops de esa escena.

## Enfoque elegido (A): capturar la tabla al entrar

Cada escena ya declara su tabla de drops mediante el objeto `DropsDelNivel`
(singleton `DropsDelNivel.Instancia.tabla`). No se mantiene ningún mapa
escena→tabla en la tienda: la tabla se captura en el momento de entrar.

Flujo:

1. El jugador cruza la zona de entrada a la tienda. Esa zona ya tiene
   `recordarEscenaActual = true` y guarda `gm.escenaRetorno`.
2. En ese mismo momento, `ZonaCambioEscena` captura la tabla de drops activa
   (`DropsDelNivel.Instancia?.tabla`) y la guarda en el `GameManager`
   (campo nuevo `tablaTienda`).
3. Al cargar la escena `Tienda`, el `Manager` lee `gm.tablaTienda`. Si hay tabla,
   arma su catálogo a partir de `tabla.drops`; si es null, cae al comportamiento
   actual (catálogo completo de `Resources/Items`).

## Componentes a modificar

### `GameManager`
- Nuevo campo `[NonSerialized] public TablaDropsNivel tablaTienda;`
- Se limpia en `ResetState()` y `NuevaPartida()` (junto a `escenaRetorno`).

### `ZonaCambioEscena`
- En `OnTriggerEnter2D`, dentro del bloque `if (recordarEscenaActual)`, además de
  guardar `escenaRetorno`, capturar la tabla:
  `gm.tablaTienda = DropsDelNivel.Instancia != null ? DropsDelNivel.Instancia.tabla : null;`

### `Manager` (tienda)
- `cargarCatalogo()` pasa a construir el catálogo según el origen:
  - Si `GameManager.Instance?.tablaTienda != null`: catálogo = items válidos de
    `tablaTienda.drops` (cada `PosibleDrop.item`), sin duplicados.
  - Si no: comportamiento actual (`Resources.LoadAll<ItemData>("Items")` filtrado).
- `generarTienda()` no cambia: sigue eligiendo hasta 3 items al azar sin repetir.

## Reglas de datos

- Se **ignoran** las probabilidades de `PosibleDrop` (son para la caída de
  enemigos). La tienda solo usa la lista de `ItemData`.
- Se filtran items nulos o inválidos (`ItemData.EsValido()`) y duplicados.
- La tienda sigue mostrando hasta 3 items (`Mathf.Min(3, puntosDeAparicion.Length)`),
  sin repetir.

## Casos borde

- **Escena de origen sin `DropsDelNivel`** (p. ej. una aldea sin enemigos):
  `tablaTienda` queda null → fallback al catálogo completo.
- **Tabla con menos de 3 items válidos**: la tienda muestra los que haya
  (el bucle corta cuando se agota la lista).
- **Entrar a la tienda sin pasar por una zona con `recordarEscenaActual`**:
  `tablaTienda` conserva su último valor o null → fallback razonable.
  (Se limpia en reset/nueva partida para no arrastrar tablas viejas entre partidas.)

## No incluido (YAGNI)

- No se crean tablas nuevas ni assets de tienda: se reutilizan los
  `TablaDropsNivel` existentes tal cual.
- No se cambian precios ni la UI de la tienda.
- No se ponderan los items por probabilidad en la tienda.
