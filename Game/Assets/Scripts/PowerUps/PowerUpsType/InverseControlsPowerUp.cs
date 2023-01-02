using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class InverseControlsPowerUp : IPowerUp
    {
        public Player Player { get; set; }
        
        public string Label { get; set; }

        private float _delay;
        private float Duration { get; }
        
        public InverseControlsPowerUp(string label, float duration, Player player, float delay)
        {
            Duration = duration;
            Player = player;
            _delay = delay;
            Label = label;
        }

        public void Execute()
        {
            Player.InverseControls(Duration, _delay);
        }
    }
}