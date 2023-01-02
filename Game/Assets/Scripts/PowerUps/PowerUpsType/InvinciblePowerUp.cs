using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class InvinciblePowerUp : IPowerUp
    {
        public Player Player { get; set; }
        public string Label { get; set; }
        
        public InvinciblePowerUp(string label, float duration, Player player)
        {
            Player = player;
            Label = label;
        }

        public void Execute()
        {
            Player.Invincible();
        }
    }
}