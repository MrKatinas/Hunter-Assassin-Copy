using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemies;
using Helpers.Settings;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using Random = UnityEngine.Random;

namespace Helpers.StateMachine.States
{
    public class Wander : IState
    {
        // References 
        private readonly Enemy _enemy;
        private readonly CustomMovementController _movementController;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;
        private readonly Enemy.Config _enemyConfig;
        
        // Local variables
        private Vector3 _destination;
        private Vector3 _lastPosition = Vector3.zero;
        private float TimeStuck;

        private float _turningTimer;

        private Coroutine _scoutingCoroutine;
        

        // Pass dependencies.
        public Wander(Enemy enemy, CustomMovementController movementController, NavMeshAgent navMeshAgent, Animator animator)
        {
            _enemy = enemy;
            _movementController = movementController;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
            
            _enemyConfig = GameSettings.Get.EnemyConfig;
        }
        
        public void OnEnter()
        {
            // Enter running animation.
            _animator.SetFloat("Strafe", 0.5f);
            _animator.SetFloat("Forward", 0.5f);

            // To check if you facing a obstacle.
            _turningTimer = int.MaxValue;
            
            _movementController.Move(_destination);
            
            _scoutingCoroutine = _enemy.StartCoroutine(StartChasing());
        }

        public void OnExit()
        {
            // Exit running animation.
            _animator.SetFloat("Strafe", 0);
            _animator.SetFloat("Forward", 0);

            // Stop moving.
            _navMeshAgent.ResetPath();
            
            _movementController.Move(_enemy.transform.position);
            
            _enemy.StopCoroutine(_scoutingCoroutine);
        }

        public void Tick()
        {
            var enemyTransform = _enemy.transform;
            
            _movementController.Move(_navMeshAgent.desiredVelocity);
            _turningTimer += Time.deltaTime;
            
            // Checks if enemy can move forward.
            if (IsPathBLocked() && _turningTimer > 0.2f)
            {
                _turningTimer = 0;
                SetNewDestination();
                return;
            }
            
            // Checks if enemy did not move in last frame.
            if (Vector3.Distance(enemyTransform.position, _lastPosition) <= 0.001f)
            {
                TimeStuck += Time.deltaTime;
                
                if (TimeStuck > 0.5f)
                {
                    // If enemy did not move for some time, he will get new destination.
                    TimeStuck = 0;
                    SetNewDestination();
                    return;
                }
            }

            _lastPosition = enemyTransform.position;

            // If enemy arrived to desired designation.
            if (Vector3.Distance(enemyTransform.position, _destination) <= _enemyConfig.StopDistance)
            {
                SetNewDestination();
            }
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
        private bool IsPathBLocked(float maxDistance = -1f, Vector3 direction = new Vector3())
        {
            // Check if params was sent with method.
            if (maxDistance <= 0)
                maxDistance = _enemyConfig.RayDistance;

            if (direction == new Vector3())
                direction = _enemy.transform.forward;
            
            var ray = new Ray(_enemy.transform.position, direction);
            
            return Physics.Raycast(ray, maxDistance);
        }

        private Vector3 FindRandomDestination()
        {
            var destinations = GetDestinationsInAllDirections();

            if (destinations.Count == 0)
            {
                Debug.LogError("Bug - Enemy can't move to any direction");

                return _destination = _enemy.transform.position;
            }

            // Only can go back where he come from
            if (destinations.Count == 1)
            {
                return _destination = destinations[0];
            }

            // -1 to remove backwards direction
            var maxIndex = destinations.Count - 2;

            return _destination = destinations[Random.Range(0, maxIndex)];
        }

        private List<Vector3> GetDestinationsInAllDirections()
        {
            var possibleDestinations = new List<Vector3>();

            // Cashed references.
            var enemyTransform = _enemy.transform;
            var enemyPosition = enemyTransform.position;

            // Anonymous delegate for callback
            void AddDestination(Vector3 destination) => possibleDestinations.Add(destination);

            // Try to get available destinations to all directions 
            GetDestination(enemyPosition, AddDestination, enemyTransform.forward);
            GetDestination(enemyPosition, AddDestination, enemyTransform.right);
            GetDestination(enemyPosition, AddDestination, enemyTransform.right * -1); // Left
            GetDestination(enemyPosition, AddDestination, enemyTransform.forward * -1); // Back

            return possibleDestinations;
        }

        private void GetDestination(Vector3 originPosition, Action<Vector3> AddDestination, Vector3 direction)
        {
            if (IsPathBLocked(2f, direction))
                return;

            AddDestination(originPosition + GetOneAxisUnitVector(direction) * Random.Range(1, 5));
        }

        private IEnumerator StartChasing()
        {
            yield return new WaitForSeconds(
                Random.Range(_enemyConfig.MinTimeUntilScouting, _enemyConfig.MaxTimeUntilScouting));

            _enemy.IsScouting = true;
        }
        
        private static Vector3 GetOneAxisUnitVector(Vector3 direction)
        {
            // Anonymous Type to join data together.
            var vectorAndDistance = new[]
            {
                new {Vector = Vector3.forward, Distance = Vector3.Distance(direction, Vector3.forward)},
                new {Vector = Vector3.right, Distance = Vector3.Distance(direction, Vector3.right)},
                new {Vector = Vector3.left, Distance = Vector3.Distance(direction, Vector3.left)},
                new {Vector = Vector3.back, Distance = Vector3.Distance(direction, Vector3.back)},
            };

            // Get destination closest to the vector.
            var closestDistance = vectorAndDistance.Min(vectorDistance => vectorDistance.Distance);

            // Closest vector to given direction
            var closestVector = vectorAndDistance.First(vectorDistance =>
                Math.Abs(vectorDistance.Distance - closestDistance) < 0.01f).Vector;

            return closestVector;
        }
    }
}