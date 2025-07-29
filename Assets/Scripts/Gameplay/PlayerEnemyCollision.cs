using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;


namespace Platformer.Gameplay
{


    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        public EnemyController enemy;
        public PlayerController player;


        PlatformerModel model = Simulation.GetModel<PlatformerModel>();


        public override void Execute()
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;


            if (willHurtEnemy)
            {
            var enemyHealth = enemy.GetComponent<Health>();
                if (!enemyHealth.IsAlive)
                {


                    player.Bounce(2);
                }
            }
			else
			{
				var sliderObj = GameObject.Find("Health Bar")?.GetComponent<UnityEngine.UI.Slider>();
				if (sliderObj != null)
				{
					sliderObj.value -= sliderObj.maxValue * 0.2f;


					if (sliderObj.value <= .2)
					{
						sliderObj.value = 0;
						Schedule<PlayerDeath>();
					}
				}
			}


        }
    }
}
