---
name: legado-level-design
description: Diseñar y auditar salas, plataformas, trampas, encuentros y rutas del juego 2D hack and slash El Legado del Héroe, respetando movimiento medido y estructura existente de escenas de Unity. Usar para greybox, pacing, puzzles o revisión de niveles.
---

# Level design — El Legado del Héroe

## Método
1. Identificar escena y recorrido actual sin modificarla.
2. Extraer valores reales de MovimientoPersonaje, prefabs y colliders: velocidad, salto, dash, escalada, dimensiones, distancia de ataques, resolución y cámara. Los valores serializados pueden sobrescribir defaults del script: revisar prefab/Inspector.
3. Dibujar flujo entrada -> enseñanza -> práctica -> riesgo -> recompensa -> salida, con rutas opcionales y descanso.
4. Definir reglas de trampas: telegraph suficiente, no daño inevitable al entrar, lectura visual, recuperación, checkpoints y ausencia de softlocks.
5. Hacer propuesta de greybox primero; conservar escena original mediante rama/copia revisable.
6. Validar in-game geometría, saltos, líneas de visión y combinaciones de enemigos; no afirmar que es alcanzable sólo por leer scripts.

## Entregables
- Objetivo de la sala y habilidad evaluada.
- Mapa textual de conexiones y riesgos.
- Tabla con distancias/alturas en unidades Unity medidas, nunca inventadas.
- Tres variantes de encounter/pacing y criterios objetivos para elegir.
- Checklist de playtest.
