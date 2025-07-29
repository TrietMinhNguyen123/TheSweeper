using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine.InputSystem;
using UnityEngine;


namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        private InputAction m_MoveAction;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        Vector2 move;

        public override void Execute()
        {
            m_MoveAction = InputSystem.actions.FindAction("Player/Move");
            var player = model.player;
            player.collider2d.enabled = true;
            player.controlEnabled = false;
            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);
                move.y = 0;
            player.health.Increment();
            player.Teleport(model.spawnPoint.transform.position);
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);
            model.virtualCamera.Follow = player.transform;
            model.virtualCamera.LookAt = player.transform;
            Simulation.Schedule<EnablePlayerInput>(2f);
        }
    }
}