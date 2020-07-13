using Enemies;
using Helpers.Settings;
using UnityEngine;

namespace Helpers.StateMachine.States
{
    public class Attack : IState
    {
        private readonly Enemy _enemy;
        
        private readonly float _spawnRate;

        /// <summary>
        /// Mimics Enemy reaction time.
        /// </summary>
        private float firstTimeDelay = 0.5f;
        private float timer = 0.3f;

        public Attack(Enemy enemy)
        {
            _enemy = enemy;

            _spawnRate = GameSettings.Get.EnemyConfig.SpawnRate;
        }

        public void OnEnter(){ }

        public void OnExit() { }
        
        /// <summary>
        /// Shoots at player
        /// </summary>
        public void Tick()
        {
            _enemy.LookAt();

            if (timer < _spawnRate + firstTimeDelay)
            {
                timer += Time.deltaTime;
                return;
            }

            firstTimeDelay = 0;
            timer = 0;
            
            _enemy.SpawnProjectile();
        }
    }
}