# Primer prompt recomendado para Codex / Astra

Antes de empezar, revisá `AGENTS.md` y activá la skill `legado-repo-audit`.

Analizá **sin modificar archivos** la situación actual de El Legado del Héroe.

Primero verificá y explicá por qué `ProjectSettings/ProjectVersion.txt` declara Unity 2021.3.14f1, pero `UnityCompile.log` registra la ejecución de Unity 6000.4.3f1. No supongas que la migración fue exitosa ni que el exit code 1 equivale necesariamente a incompatibilidad.

Quiero un inventario de sistemas reales, con rutas y evidencias: combate, movimiento, enemigos, salud, UI, puzzles, niveles, cámaras, audio, state/save, assets y paquetes. Separá escenas del juego y demos de terceros; examiná `EditorBuildSettings.asset`.

Priorizá riesgos confirmados y verificables. Prestá especial atención a:
- `using UnityEditor` dentro de `Assets/Scripts/MovimientoPersonaje.cs` y consecuencias para player builds.
- Input legacy en `MovimientoPersonaje` y `ControladorPersonaje` (no migrar todavía).
- Persistencia de estado en `GestorJugador` y si realmente existe save/load persistente.
- Posibles referencias nulas y dependencias del Inspector.
- Diferencias entre scripts de enemigos (`Enemigo`, `Enemigo3`, `LancerEnemy`, `SpiderController`).
- Escenas y prefabs con referencias rotas, si el Editor está disponible.
- Tests presentes o ausentes y pruebas necesarias.
- Licencias de sprites/assets de terceros.

Emití informe por severidad (CRITICAL, HIGH, MEDIUM, LOW), citando archivo y líneas. Distingí observaciones estáticas de fallas reproducidas. Creá un roadmap priorizado para un desarrollador individual. No instales Unity 6, plugins, MCPs ni skills nuevas sin mi autorización.
