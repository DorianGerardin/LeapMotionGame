using System;
using UnityEngine;

namespace PowerUps.PowerUpsType
{
    public class DestructObstaclesPowerUp : IPowerUp
    {
        public Player Player { get; set; }

        private readonly float _radius = 20f;
        
        public DestructObstaclesPowerUp(Player player)
        {
            Player = player;
        }

        public void Execute()
        {
            Player.DestructObstacles(_radius);
        }
    }
}