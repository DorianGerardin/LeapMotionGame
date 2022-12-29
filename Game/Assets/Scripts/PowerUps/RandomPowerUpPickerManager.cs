using System.Collections.Generic;
using PowerUps.PowerUpsType;
using UnityEngine;

namespace PowerUps
{
    public class RandomPowerUpPickerManager
    {
        private List<IPowerUp> _powerUps;
        
        public RandomPowerUpPickerManager(Player player)
        {
            _powerUps = new List<IPowerUp>
            {
                //new JumpPowerUp(player),
                new InvinciblePowerUp(10f, player),
                //new ScaleDownPowerUp(10f, player),
                //new InverseControlsPowerUp(10f, player),
                //new DestructObstaclesPowerUp(player)
            };
        }

        public IPowerUp Pick()
        {
            int index = Random.Range(0, _powerUps.Count);
            return _powerUps[index];
        }
    }
}