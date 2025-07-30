using Platformer.Core;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;
using UnityEngine;


namespace Platformer.Gameplay
{
   /// <summary>
   /// Fired when the player health reaches 0. This usually would result in a
   /// PlayerDeath event.
   /// </summary>
   /// <typeparam name="HealthIsZero"></typeparam>
   public class HealthIsZero : Simulation.Event<HealthIsZero>
   {  
       public Health health;
    
      public override void Execute()
       {
           if (health.gameObject.CompareTag("Player"))
           {
               Schedule<PlayerDeath>();
           }
           else if (health.gameObject.CompareTag("enemy"))
           {
               var enemyController = health.GetComponent<EnemyController>();
               if (enemyController != null)
                   Schedule<EnemyDeath>().enemy = enemyController;
           }
       }


   }
}
