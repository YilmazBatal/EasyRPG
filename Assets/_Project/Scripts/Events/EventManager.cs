using System;

namespace TextBasedRPG.Events
{
    public static class EventManager
    {
        public static class CombatEvents
        {
            public static event Action OnRoundEnded;
            public static event Action<bool, int, bool> OnEntityGotHit;
            public static event Action<bool, int> OnPlayerGotHit;
            public static event Action OnPlayerLowHP;

            public static event Action OnFocusChanged;
            public static event Action OnGuardChanged;
            public static void TriggerOnFocusChanged() => OnFocusChanged?.Invoke();
            public static void TriggerOnGuardChanged() => OnGuardChanged?.Invoke();
            public static void TriggerOnEntityGotHit(bool isCrit, int damage, bool wasFocused) => OnEntityGotHit?.Invoke(isCrit, damage, wasFocused);
            public static void TriggerOnPlayerGotHit(bool isCrit, int damage) => OnPlayerGotHit?.Invoke(isCrit, damage);
            public static void TriggerOnPlayerLowHP() => OnPlayerLowHP?.Invoke();
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
