using System.Collections.Generic;
using PowerUps.PowerUpsType;
using UnityEngine;

namespace PowerUps
{
    public class RandomPowerUpPickerManager
    {
        private List<IPowerUp> _powerUps;
        private Player _player;
        
        public RandomPowerUpPickerManager(Player player)
        {
            _player = player;
            _powerUps = new List<IPowerUp>
            {
                new JumpPowerUp("Jump", player),
                new InvinciblePowerUp("Invincible", 10f, player),
                new ScaleDownPowerUp("ScaleDown", 10f, player),
                new DestructObstaclesPowerUp("Destruct", player),
                new InverseControlsPowerUp("InverseControls", 8f, player, 3f)
            };
        }

        public IPowerUp Pick()
        {
            int index = Random.Range(0, _powerUps.Count);
            return _powerUps[index];
        }
        
        public IPowerUp PickOnlyPowerUp()
        {
            int index = Random.Range(0, _powerUps.Count-1);
            return _powerUps[index];
        }
    }
}