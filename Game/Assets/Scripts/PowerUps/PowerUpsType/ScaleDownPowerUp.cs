using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class ScaleDownPowerUp : IPowerUp
    {
        public Player Player { get; set; }
        public string Label { get; set; }
        
        public ScaleDownPowerUp(string label, float duration, Player player)
        {
            Player = player;
            Label = label;
        }

        public void Execute()
        {
            Player.ScaleDown();
        }
    }
}