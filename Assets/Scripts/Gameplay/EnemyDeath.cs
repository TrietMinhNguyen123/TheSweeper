using UnityEngine;
using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of 0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            Debug.Log($"EnemyDeath Execute called on {enemy.gameObject.name}");

            // Trigger the death animation
            var animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("death");
            }

            // Disable logic and collision so they can't interact further
            enemy._collider.enabled = false;
            enemy.control.enabled = false;

            // Optional: play death sound
            if (enemy._audio && enemy.ouch)
                enemy._audio.PlayOneShot(enemy.ouch);

            // Schedule destruction after a short delay (e.g., 1 second)
            Simulation.Schedule<EnemyDestroy>(1f).enemy = enemy;
        }
    }

    public class EnemyDestroy : Simulation.Event<EnemyDestroy>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            GameObject.Destroy(enemy.gameObject);
            Debug.Log($"Enemy {enemy.gameObject.name} destroyed");
        }
    }
}
