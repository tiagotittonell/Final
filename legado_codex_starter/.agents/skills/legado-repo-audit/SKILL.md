---
name: legado-repo-audit
description: Auditar de forma segura el repositorio Unity El Legado del Héroe y elaborar un plan priorizado de mantenimiento, compilación, combate, escenas, assets, testing y migración, sin modificar archivos. Usar antes de refactors, upgrades o grandes integraciones.
---

# Auditoría del repositorio — El Legado del Héroe

## Protocolo
- Esta skill es de solo lectura. No ejecutar instaladores, upgrades, `git clean/reset`, herramientas de terceros ni comandos que modifiquen archivos.
- Detectar la versión real desde ProjectVersion, packages, commit, y la instalada localmente si es observable. Señalar discrepancias, no adivinar.
- Clasificar escenas incluidas en Build Settings frente a demos de terceros y prototipos.
- Inventariar player (movimiento, combo, daños), enemigos, bosses, puzzles, checkpoint/save, UI, cámara, audio, performance.
- Buscar riesgos: compilación/editor namespaces, input legacy, referencias nulas, links inspector, componentes duplicados, eventos sin unsubscription, `Find` frecuentes, Instantiate/Destroy repetidos, GC, escenas huérfanas, scripts muertos, asset provenance.
- Identificar dependencias de terceros y posibles incompatibilidades con Unity 6, pero no migrar.
- Separar hallazgos confirmados en código de hipótesis que sólo pueden validarse con Unity Editor o una build.

## Salida
- Resumen ejecutivo de una página.
- Inventario de sistemas por ruta.
- Tabla CRITICAL/HIGH/MEDIUM/LOW: archivo, evidencia, impacto, arreglo sugerido y cómo probarlo.
- Checklist de versión/compilación, build, QA y assets/licencias.
- Roadmap de 4 semanas realista para un developer indie.
- 5 acciones iniciales pequeñas, con riesgos y criterios de aceptación.
