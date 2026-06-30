using System;

/// <summary>
/// Canal de eventos estáticos del jefe. Desacopla al jefe de su barra de vida:
/// el jefe avisa (spawn / daño / muerte) y la UI se suscribe, así funciona igual
/// si el jefe está colocado a mano en la escena o se instancia como prefab.
/// </summary>
public static class BossEvents
{
    /// <summary>(nombre del jefe, vida máxima)</summary>
    public static event Action<string, int> OnBossSpawned;
    /// <summary>(vida actual)</summary>
    public static event Action<int> OnBossHealthChanged;
    public static event Action OnBossDefeated;

    public static void RaiseSpawned(string nombre, int maxLife) => OnBossSpawned?.Invoke(nombre, maxLife);
    public static void RaiseHealthChanged(int current) => OnBossHealthChanged?.Invoke(current);
    public static void RaiseDefeated() => OnBossDefeated?.Invoke();
}
