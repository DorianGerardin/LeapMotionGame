using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class ScaleDownPowerUp : IPowerUp
    {
        public Player Player { get; set; }
        
        public ScaleDownPowerUp(float duration, Player player)
        {
            Player = player;
        }

        public void Execute()
        {
            Player.ScaleDown();
        }
    }
}