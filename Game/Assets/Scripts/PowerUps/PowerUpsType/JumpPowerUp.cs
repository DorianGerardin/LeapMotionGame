using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class JumpPowerUp : IPowerUp
    {
        public Player Player { get; set; }
        
        public JumpPowerUp(Player player)
        {
            Player = player;
        }

        public void Execute()
        {
            Player.Jump();
        }
    }
}