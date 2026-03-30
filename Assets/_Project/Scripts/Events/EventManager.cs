using System;

namespace TextBasedRPG.Events
{
    public static class EventManager
    {
        public static class CombatEvents
        {
            public static event Action? OnRoundEnded;
            public static void TriggerOnRoundEnded() => OnRoundEnded?.Invoke();
        }
        public static class HeroEvents
        {
            public static event Action<GameContext>? OnGoldChanged;
            public static event Action<int>? OnExpGained;
            public static void TriggerGoldChanged(GameContext context) => OnGoldChanged?.Invoke(context);
            public static void TriggerExpGained(int amount) => OnExpGained?.Invoke(amount);
        }
    }
}
