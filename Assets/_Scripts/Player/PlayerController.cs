using Enemies;
using Managers;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private ThirdPersonCharacter _character;

        private LevelManager _levelManager;
        private int _currentHP = 10;

        private void Start()
        {
            _levelManager = LevelManager.Get;
            
            // Making player transform accessible to enemies 
            _levelManager.PlayerTransform = transform;
            
            // Third person character controller rotate enemy
            _agent.updateRotation = false;
        }

        private void Update() => Move();

        private void Move()
        {
            if (Input.GetMouseButton(0))
            {
                // Get the input coordinates set up new destination
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit))
                {
                    _agent.SetDestination(hit.point);
                }
            }

            // if there is destination, move to destination, otherwise stay still.
            _character.Move(_agent.remainingDistance > _agent.stoppingDistance ? _agent.desiredVelocity : Vector3.zero,
                false, false);
        }

        public void TakeDamage(int amount)
        {
            _currentHP -= amount;

            if (_currentHP <= 0)
            {
                _levelManager.LevelLost();
                
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            var enemy =  other.transform.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                Destroy(enemy.gameObject);
                
                _levelManager.RegisterEnemyDeath();
            }
        }
    }
}
