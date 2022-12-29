using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class InverseControlsPowerUp : IPowerUp
    {
        public float Duration { get; }
        public Player Player { get; set; }
        
        public InverseControlsPowerUp(float duration, Player player)
        {
            Duration = duration;
            Player = player;
        }

        public void Execute()
        {
            Player.InverseControls(Duration);
        }
    }
}