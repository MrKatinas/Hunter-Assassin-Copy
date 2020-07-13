﻿using Enemies;
using Helpers.Settings;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Helpers.StateMachine.States
{
    /// <summary>
    /// Enemy lost view of player and chasing him for some time 
    /// </summary>
    public class Chase : IState
    {
        // References
        private readonly Enemy _enemy;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly ThirdPersonCharacter _character;
        private readonly Enemy.Config _enemyConfig;
        private readonly Animator _animator;

        private readonly float _chaseTime;
        private float _timer = 0f;

        public Chase(Enemy enemy, NavMeshAgent navMeshAgent, ThirdPersonCharacter character, Animator animator)
        {
            _enemy = enemy;
            _navMeshAgent = navMeshAgent;
            _character = character;
            _animator = animator;
            
            _enemyConfig = GameSettings.Get.EnemyConfig;
            _chaseTime = _enemyConfig.ChaseTime;
        }
        
        public void OnEnter()
        {
            _navMeshAgent.speed = _enemyConfig.ChasingMovementSpeed;
            _timer = 0;
        }

        public void OnExit()
        {
            _navMeshAgent.speed = _enemyConfig.AwareMovementSpeed;
            
            _navMeshAgent.SetDestination(_enemy.transform.position);
            _character.Move(_navMeshAgent.desiredVelocity, false, false);
            
            _animator.SetFloat("Strafe", 0);
            _animator.SetFloat("Forward", 0);
        }
        
        public void Tick()
        {
            if (_timer >= _chaseTime)
            {
                _enemy.IsChasing = false;
                return;
            }

            _timer += Time.deltaTime;
            
            // Moves to enemy.
            _navMeshAgent.SetDestination(_enemy.Player.transform.position);
            _character.Move(_navMeshAgent.desiredVelocity, false, false);
        }
    }
}