using System;

namespace TextBasedRPG.Events
{
    public static class EventManager
    {
        public static class Combat
        {
            public static event Action? OnRoundEnded;
            public static void TriggerOnRoundEnded() => OnRoundEnded?.Invoke();
        }
        public static class HeroEvents
        {
            public static event Action<int>? OnGoldChanged;
            public static event Action<int>? OnExpGained;
            public static void TriggerGoldChanged(int amount) => OnGoldChanged?.Invoke(amount);
            public static void TriggerExpGained(int amount) => OnExpGained?.Invoke(amount);
        }
    }
}
