using Enemies;
using Helpers.Settings;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Helpers.StateMachine.States
{
    public class Wander : IState
    {
        // References 
        private readonly Enemy _enemy;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        private readonly ThirdPersonCharacter _character;
        private readonly Enemy.Config _enemyConfig;
        
        // Local variables
        private Vector3 _destination;
        private Vector3 _lastPosition = Vector3.zero;
        private float TimeStuck;
        

        // Pass dependencies.
        public Wander(Enemy enemy, NavMeshAgent navMeshAgent, ThirdPersonCharacter character, Animator animator)
        {
            _enemy = enemy;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            _character = character;
            
            _enemyConfig = GameSettings.Get.EnemyConfig;
        }
        
        public void OnEnter()
        {
            // Enter running animation.
            _animator.SetFloat("Strafe", 0.5f);
            _animator.SetFloat("Forward", 0.5f);
        }

        public void OnExit()
        {
            // Exit running animation.
            _animator.SetFloat("Strafe", 0);
            _animator.SetFloat("Forward", 0);

            // Stop moving.
            _navMeshAgent.ResetPath();
            _character.Move(_enemy.transform.position, false, false);
        }

        public void Tick()
        {
            var enemyTransform = _enemy.transform;
            
            // Checks if enemy did not move in last frame.
            if (Vector3.Distance(enemyTransform.position, _lastPosition) <= 0.00001f)
                TimeStuck += Time.deltaTime;

            _lastPosition = enemyTransform.position;

            // If enemy did not move for some time, he will get new destination.
            if (TimeStuck > 0.5f)
            {
                SetNewDestination();
                TimeStuck = 0;
            }

            // If enemy arrived to desired designation.
            if (Vector3.Distance(enemyTransform.position, _destination) <= _enemyConfig.StopDistance)
            {
                SetNewDestination();
            }
            
            // Checks if enemy can move forward.
            if (IsPathBLocked())
            {
                Debug.DrawRay(enemyTransform.position, enemyTransform.forward * 10f, Color.blue);
                _enemy.transform.Rotate(0, 0.1f, 0);

                return;
            }
            
            // Slightly moves enemy to desired location. 
            _character.Move(_navMeshAgent.desiredVelocity, false, false);
        }

        private void SetNewDestination()
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.SetDestination(FindRandomDestination());
        }

        /// <summary>
        /// Checks if Enemy can go forward.
        /// </summary>
        /// <returns></returns>
        private bool IsPathBLocked()
        {
            var ray = new Ray(_enemy.transform.position, _enemy.transform.forward);
            
            return Physics.SphereCast(ray, 1f, _enemyConfig.RayDistance,  _enemyConfig.LayerMask);
        }

        /// <summary>
        /// Get random spot around enemy to move.
        /// </summary>
        /// <returns></returns>
        private Vector3 FindRandomDestination() =>
            _destination += new Vector3(Random.Range(-1, 1f), 0f, Random.Range(-1, 1f));
    }
}