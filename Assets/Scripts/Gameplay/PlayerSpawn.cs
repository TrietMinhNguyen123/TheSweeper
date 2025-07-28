using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            var player = model.player;
            player.collider2d.enabled = true;
            player.controlEnabled = false;

            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);

            player.Teleport(model.spawnPoint.transform.position);
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);

            model.virtualCamera.Follow = player.transform;
            model.virtualCamera.LookAt = player.transform;

            // --- Fill health bar back to full ---
            var healthSlider = GameObject.Find("Health Bar")?.GetComponent<Slider>();
            if (healthSlider != null)
            {
                healthSlider.value = healthSlider.maxValue;
            }

            // --- Remove one heart (from rightmost active) ---
            GameObject[] hearts = new GameObject[]
            {
                GameObject.Find("Heart1"),
                GameObject.Find("Heart2"),
                GameObject.Find("Heart3")
            };

            for (int i = hearts.Length - 1; i >= 0; i--)
            {
                if (hearts[i] != null && hearts[i].activeSelf)
                {
                    hearts[i].SetActive(false);
                    break;
                }
            }

            // Re-enable player input after short delay
            Simulation.Schedule<EnablePlayerInput>(2f);
        }
    }
}
