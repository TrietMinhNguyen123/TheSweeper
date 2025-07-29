using System;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        public int maxHP = 3;
        public int currentHP;

        public bool IsAlive => currentHP > 0;

        void Awake()
        {
            currentHP = maxHP;
        }

        public void Decrement()
{
        currentHP = Mathf.Clamp(currentHP - 1, 0, maxHP);
        if (currentHP == 0)
        {
            var ev = Schedule<HealthIsZero>();
            ev.health = this;
        }
    }

        public void Increment()
        {
            currentHP = Mathf.Min(currentHP + 1, maxHP);
            Debug.Log($"{gameObject.name} healed. HP: {currentHP}");
        }

        public void Die()
        {
            currentHP = 0;
            Debug.Log($"{gameObject.name} died.");
        }
    }
}
