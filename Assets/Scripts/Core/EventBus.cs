using System;
using System.Collections.Generic;

namespace PastaDefence.Core
{
    public enum GameEvent
    {
        GameStarted,
        StateChanged,
        WaveStarted,
        WaveCompleted,
        Victory,
        Defeat,
        ServingsChanged,
        DoughChanged,
        EnemyKilled,
        EnemyReachedEnd,
        TowerPlaced,
        TowerSold,
        TowerUpgraded,
        ChefAbilityUsed,
        ChefDowned,
        BossSpawned,
        ComboTriggered
    }

    public static class EventBus
    {
        private static readonly Dictionary<GameEvent, List<Action<object>>> listeners = new();

        public static void Subscribe(GameEvent eventType, Action<object> callback)
        {
            if (!listeners.ContainsKey(eventType))
                listeners[eventType] = new List<Action<object>>();

            listeners[eventType].Add(callback);
        }

        public static void Unsubscribe(GameEvent eventType, Action<object> callback)
        {
            if (listeners.ContainsKey(eventType))
                listeners[eventType].Remove(callback);
        }

        public static void Trigger(GameEvent eventType, object data = null)
        {
            if (!listeners.ContainsKey(eventType)) return;

            // Iterate over a copy to avoid modification during iteration
            var callbacks = new List<Action<object>>(listeners[eventType]);
            foreach (var callback in callbacks)
            {
                callback?.Invoke(data);
            }
        }

        public static void Clear()
        {
            listeners.Clear();
        }
    }
}
