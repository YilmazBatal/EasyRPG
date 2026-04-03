using System;

namespace TextBasedRPG.Events
{
    public static class EventManager
    {
        public static class CombatEvents
        {
            public static event Action OnRoundEnded;
            public static event Action<bool, int> OnEntityGotHit;
            public static event Action<bool, int> OnPlayerGotHit;
            public static void TriggerOnRoundEnded() => OnRoundEnded?.Invoke();
            public static void TriggerOnEntityGotHit(bool isCrit, int damage) => OnEntityGotHit?.Invoke(isCrit, damage);
            public static void TriggerOnPlayerGotHit(bool isCrit, int damage) => OnPlayerGotHit?.Invoke(isCrit, damage);
        }
        public static class HeroEvents
        {
            public static event Action<GameContext> OnGoldChanged;
            public static event Action<GameContext> OnExpChanged;
            public static event Action<GameContext> OnHPValueChanged;
            public static void TriggerGoldChanged(GameContext context) => OnGoldChanged?.Invoke(context);
            public static void TriggerExpChanged(GameContext context) => OnExpChanged?.Invoke(context);
            public static void TriggerHPValueChanged(GameContext context) => OnHPValueChanged?.Invoke(context);
        }
    }
}
