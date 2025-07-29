using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        // Static flag to block multiple death triggers
        public static bool isProcessingDeath = false;

        public override void Execute()
        {
            if (isProcessingDeath) return; // Prevent duplicate death events

            isProcessingDeath = true;

            var player = model.player;

            model.virtualCamera.Follow = null;
            model.virtualCamera.LookAt = null;
            player.controlEnabled = false;

            if (player.audioSource && player.ouchAudio)
                player.audioSource.PlayOneShot(player.ouchAudio);
            player.animator.SetTrigger("hurt");
            player.animator.SetBool("dead", true);

            Simulation.Schedule<PlayerSpawn>(2f); // Delay respawn
        }
    }
}
