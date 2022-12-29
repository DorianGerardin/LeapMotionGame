using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class InvinciblePowerUp : IPowerUp
    {
        public Player Player { get; set; }
        
        public InvinciblePowerUp(float duration, Player player)
        {
            Player = player;
        }

        public void Execute()
        {
            Player.Invincible();
        }
    }
}