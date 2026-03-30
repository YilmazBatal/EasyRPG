using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Events;

namespace TextBasedRPG.Managers
{
    public class DefenseBuff
    {
        private Hero _player;
        private int _duration;
        private int _addedDef;
        private int _percentage;

        public DefenseBuff(Hero player, int duration, int amount)
        {
            _player = player;
            _duration = duration;
            _addedDef = amount;
            _percentage = player.TotalDEF * _addedDef / 100;

            _player.BonusDef += _percentage;

            EventManager.CombatEvents.OnRoundEnded += Tick;
        }

        private void Tick()
        {
            _duration--;
            if (_duration <= 0)
            {
                // 3. Süre bitti: Bonusu geri al ve aboneliği iptal et
                _player.BonusDef -= _percentage;
                EventManager.CombatEvents.OnRoundEnded -= Tick;
                //Console.WriteLine("\n[SYSTEM] Guard Up effect has worn off!");
            }
        }
    }
    public class AttackBuff
    {
        private Hero _player;
        private int _duration;
        private int _addedAtk;
        private int _percentage;

        public AttackBuff(Hero player, int duration, int amount)
        {
            _player = player;
            _duration = duration;
            _addedAtk = amount;
            _percentage = player.TotalATK * _addedAtk / 100;
            _player.BonusATK += _percentage;

            EventManager.CombatEvents.OnRoundEnded += Tick;
        }

        private void Tick()
        {
            _duration--;
            if (_duration <= 0)
            {
                _player.BonusATK -= _percentage;
                EventManager.CombatEvents.OnRoundEnded -= Tick;
                //Console.WriteLine("\n[SYSTEM] Bonus Attack effect has worn off!");
                //Console.ReadKey(true);
            }
        }
    }
}
