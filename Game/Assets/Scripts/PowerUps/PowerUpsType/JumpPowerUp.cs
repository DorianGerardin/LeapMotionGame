using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class JumpPowerUp : IPowerUp
    {
        public Player Player { get; set; }
        public string Label { get; set; }
        
        public JumpPowerUp(string label, Player player)
        {
            Player = player;
            Label = label;
        }

        public void Execute()
        {
            Player.Jump();
        }
    }
}