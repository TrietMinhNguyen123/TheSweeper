using UnityEngine;
using Platformer.Core;
using Platformer.Mechanics;
using System.Collections;

namespace Platformer.Gameplay
{
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public EnemyController enemy;

        public override void Execute()
        {
            Debug.Log($"EnemyDeath Execute called on {enemy.gameObject.name}");

            // Disable control and collision but NOT the GameObject yet
            enemy._collider.enabled = false;
            enemy.control.enabled = false;
            

            var rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // stop current motion
                rb.bodyType = RigidbodyType2D.Static; // or set constraints if needed
            }


            // Trigger the death animation
            if (enemy.animator != null)
            {
                enemy.animator.SetTrigger("death");
            }

            // Play sound
            if (enemy._audio && enemy.ouch)
            {
                enemy._audio.PlayOneShot(enemy.ouch);
            }

            // Start coroutine to destroy after animation ends
            enemy.StartCoroutine(DestroyAfterDeathAnim(enemy));
        }

        private IEnumerator DestroyAfterDeathAnim(EnemyController enemy)
        {
            // Wait for the length of the EnemyDead animation
            float delay = enemy.animator.GetCurrentAnimatorStateInfo(0).length;

            // You can also use AnimatorStateInfo.normalizedTime if needed
            yield return new WaitForSeconds(delay);

            // Now destroy the GameObject
            GameObject.Destroy(enemy.gameObject);
            Debug.Log($"Enemy {enemy.gameObject.name} destroyed");
        }
    }
}
