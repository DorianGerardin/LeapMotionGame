using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class InverseControlsPowerUp : IPowerUp
    {
        public float Duration { get; }
        public Player Player { get; set; }

        private float _delay;
        
        public InverseControlsPowerUp(float duration, Player player, float delay)
        {
            Duration = duration;
            Player = player;
            _delay = delay;
        }

        public void Execute()
        {
            Player.InverseControls(Duration, _delay);
        }
    }
}