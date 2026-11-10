# Selección curada de skills — El Legado del Héroe

**Primero: no instalar todo un repositorio de 73 skills automáticamente.** Revisar cada `SKILL.md`, scripts, dependencies, hooks y red antes de habilitarlo.

## Skills incluidas en este paquete (propias, solo instrucciones)
- `legado-repo-audit`: primera auditoría y roadmap, SOLO lectura.
- `legado-level-design`: greybox, player metrics, encuentros.
- `legado-art-pipeline`: sprites, import, consistencia y licencias.

## Skills comunitarias para analizar e importar por separado
Origen: `gamedev-skills/awesome-gamedev-agent-skills`. Son genéricas y requieren adaptación.
1. `level-design`
2. `game-feel`
3. `camera-systems`
4. `create-game-assets` (si se ejecutan scripts opcionales, auditar antes)
5. `game-ai`
6. `dialogue-systems`
7. `audio-design`
8. `performance-optimization`
9. `platformer`
10. `physics-tuning`

No instalar todavía la serie `unity-*` del repositorio comunitario sin verificar su adecuación a Unity 2021: el README documenta su baseline como Unity 6.3 LTS.

## Skills oficiales de Unity
`Unity-Technologies/unity-agent-plugin` declara soporte Unity 6+. No usar en esta copia hasta verificar versión local del editor y planificar migración o compatibilidad.

## Procedimiento seguro
1. Commit/branch de respaldo.
2. Instalar estas skills locales bajo la raíz `.agents/skills/` (no hay scripts ejecutables en el paquete).
3. Pedir auditoría en modo solo lectura usando `Docs/CODEX_FIRST_AUDIT.md`.
4. Validar primera build o capturar logs actuales en el Editor efectivamente utilizado.
5. Seleccionar, auditar y copiar 3–5 skills genéricas de las 10 sugeridas.
6. Validar comportamiento antes de instalar otras.
