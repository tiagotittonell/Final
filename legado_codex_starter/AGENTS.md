# El Legado del Héroe — Instrucciones para Codex / Astra

## Contexto del proyecto
- Juego 2D de acción y plataformas con énfasis en hack & slash, pixel art y mazmorras.
- Repositorio: `tiagotittonell/Final`.
- Proyecto Unity versionado. `ProjectSettings/ProjectVersion.txt` declara `2021.3.14f1` en `main` al commit `96cfbb12643174f7d5f2cc593087eec34261fa72` (2026-08-03).
- `UnityCompile.log` contiene una ejecución de Unity `6000.4.3f1` (2026-07-26) que termina con código 1. No inferir de ahí que una migración fue exitosa ni cuál fue el error; verificar el editor real antes de ejecutar o actualizar.
- El paquete del proyecto incluye Cinemachine 2.8.9, TextMesh Pro 3.0.6, Unity 2D y sistema de Input legacy (`Input.Get...`).

## Dónde encontrar los sistemas existentes
- Jugador y combo: `Assets/Scripts/ControladorPersonaje.cs`
- Movimiento, dash, saltos, escalada: `Assets/Scripts/MovimientoPersonaje.cs`
- Salud del jugador: `Assets/Scripts/VidaPersonaje.cs`
- Audio: `Assets/Scripts/AudioManager.cs`
- Cámara/Cinemachine: `Assets/Scripts/ControladorCamara.cs`
- Daño/enemigos: `Assets/Scripts/IDamageable.cs`, `Assets/Scripts/EnemigoBase.cs`, `Assets/Scripts/SpiderController.cs`, `Assets/Scripts/LancerEnemy.cs`
- Escenas jugables: `Assets/Scenes/`
- Prefabs jugables: `Assets/Prefabs/`
- Recursos visuales: `Assets/Sprites/`, `Assets/Animaciones/`, y paquetes de terceros bajo `Assets/`.

## Antes de modificar
1. Examinar el código, escenas, prefabs y versiones relacionadas con la tarea; nunca suponer que faltan sistemas que ya existen.
2. Consultar primero `ProjectSettings/ProjectVersion.txt`, `Packages/manifest.json` y `Packages/packages-lock.json`. No utilizar APIs exclusivas de Unity 6 en Unity 2021.
3. Planificar cambios limitados, concretos y reversibles. Para cambios amplios, crear rama y pedir aprobación explícita antes de modificar escenas, paquetes, render pipeline o migrar Unity.
4. Proteger GUIDs, referencias y archivos `.meta`. No regenerar o borrar archivos `.meta` de assets existentes.
5. No instalar dependencias, descargar y ejecutar scripts, activar MCPs externos ni acceder a secretos sin autorización explícita.
6. No cambiar código de terceros ni carpetas demo de paquetes comprados sin necesidad justificada.
7. No afirmar que un cambio compila o funciona sin ejecutar una prueba real. Si el Editor no está disponible, reportar que la validación quedó pendiente.

## Criterios de calidad
- Conservar comportamiento existente salvo que se solicite cambiarlo.
- Corregir errores en pequeñas etapas y revisar las referencias de Inspector.
- Evitar `UnityEditor` en assemblies/scripts de runtime que deban incluirse en el build.
- Considerar animaciones, hurtboxes/hitboxes, golpes múltiples, invulnerabilidad, colliders y transiciones de escena al tocar combate.
- Testear con teclado/ratón existentes antes de proponer cambiar el Input System.
- En level design, medir primero velocidades, salto, dash, tamaño de colisionadores y encuadre de cámara. Construir greybox y probar antes de un pase de arte.
- En arte, mantener coherencia de silueta, paleta, escala, pivotes, PPU y filtrado; validar en Unity a escala de juego.
- Rastrear licencias de assets ajenos y no redistribuir archivos cuando sus términos lo prohíban.

## Formato de respuesta para tareas técnicas
1. Diagnóstico y archivos inspeccionados.
2. Propuesta y riesgos.
3. Cambios efectuados (si fueron autorizados).
4. Pruebas ejecutadas y evidencia observable.
5. Lo que falta comprobar manualmente.

Para la primera auditoría usar `.agents/skills/legado-repo-audit/SKILL.md`.
