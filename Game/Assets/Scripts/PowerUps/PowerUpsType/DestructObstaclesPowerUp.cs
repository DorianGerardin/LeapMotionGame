using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class DestructObstaclesPowerUp : IPowerUp
    {
        public Player Player { get; set; }
        public string Label { get; set; }

        private readonly float _radius = 20f;
        
        public DestructObstaclesPowerUp(string label, Player player)
        {
            Player = player;
            Label = label;
        }

        public void Execute()
        {
            Player.DestructObstacles(_radius);
        }
    }
}