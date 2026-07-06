using System;

public static class BossEvents
{
    public static event Action<string, int> OnBossSpawned;
    public static event Action<int> OnBossHealthChanged;
    public static event Action OnBossDefeated;

    public static void RaiseSpawned(string nombre, int maxLife) => OnBossSpawned?.Invoke(nombre, maxLife);
    public static void RaiseHealthChanged(int current) => OnBossHealthChanged?.Invoke(current);
    public static void RaiseDefeated() => OnBossDefeated?.Invoke();
}
