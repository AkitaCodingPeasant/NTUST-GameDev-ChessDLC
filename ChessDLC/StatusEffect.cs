using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDLC {
    public class StatusEffect {
        int[] currentDuration;
        public StatusEffect() {
            currentDuration = Enumerable.Repeat(0, Enum.GetValues(typeof(EffectType)).Length).ToArray();
        }
        public void AddStatusEffect(EffectType effectType, int duration) {
            currentDuration[(int)effectType] = duration > currentDuration[(int)effectType] ? duration : currentDuration[(int)effectType];
        }
        public void AddStatusEffect(EffectType effectType) {
            AddStatusEffect(effectType, 1);
        }

        public void RemoveStatusEffect(EffectType effectType) {
            currentDuration[(int)effectType] = 0;
        }

        public void DurationReduce() {
            for (int i = 0; i < currentDuration.Length; i++) {
                if (currentDuration[i] <= 0) {
                    currentDuration[i] = 0;
                    continue;
                }
                currentDuration[i]--;
            }
        }

        public string GetDisplayString() {
            string[] iconArr = new string[7] { "🥾", "🛡", "🗿", "⚡", "⏳", "🕸", "⚠️" };
            string result = "";
            for (int i = 0; i < 7; i++) {
                if (currentDuration[i] > 0) {
                    result += iconArr[i] + ":" + currentDuration[i] + " ";
                }
            }
            return result;
        }
        public List<(EffectType effectType, int duration)> GetStatusEffect() {
            List<(EffectType effectType, int duration)> result = new List<(EffectType, int)>();
            for (int i = 0; i < currentDuration.Length; i++) {
                if (currentDuration[i] == 0) {
                    continue;
                }
                result.Add(((EffectType)i, currentDuration[i]));
            }
            return result;
        }

        public bool HasStatusEffect(EffectType effectType) {
            return currentDuration[(int)effectType] >= 1;
        }
    }
}
