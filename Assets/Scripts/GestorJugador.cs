public static class GestorJugador
{
    public static float vidaGuardada;
    public static bool tieneVidaGuardada = false;
    public static int consumiblesGuardados = 0;
    public static bool bloquearGuardado = false;

    public static void GuardarVida(float vida)
    {
        vidaGuardada = vida;
        tieneVidaGuardada = true;
    }

    public static void Resetear()
    {
        vidaGuardada = 0f;
        tieneVidaGuardada = false;
        consumiblesGuardados = 0;
        bloquearGuardado = false;
    }
}
