using System;
using System.Collections;
using Helpers;
using Helpers.Settings;
using Helpers.StateMachine;
using Helpers.StateMachine.States;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [HideInInspector] public Transform Target;
        [HideInInspector] public Transform Player;
        [HideInInspector] public bool IsChasing;
        [HideInInspector] public bool IsScouting;
        
        [SerializeField] private bool _isStatic;
        
        [Header("For Projectile spawn")]
        [SerializeField] private Transform _bulletSpawnPoint;
        [SerializeField] private AudioSource _spawnAudioSource;
        [SerializeField] private ParticleSystem _bulletSpawnParticles;

        private StateMachine _stateMachine;
        private Config _enemyConfig;
        private FieldOfView _fieldOfView;
        private LevelManager _levelManager;

        
        private void Awake()
        {
            _enemyConfig = GameSettings.Get.EnemyConfig;
            _stateMachine = new StateMachine();

            _fieldOfView = Instantiate(_enemyConfig.FieldOfViewPrefab, Vector3.zero, Quaternion.identity)
                .GetComponent<FieldOfView>();
            
            // Store component references.
            var navMeshAgent = GetComponent<NavMeshAgent>();
            var character = GetComponent<ThirdPersonCharacter>();
            var animator = GetComponent<Animator>();
            var movementController = GetComponent<CustomMovementController>();

            // Create wanted states.
            var staticState = new StaticState();
            var wander = new Wander(this, movementController, navMeshAgent, animator);
            var chase = new Chase(this, movementController, navMeshAgent, animator);
            var attack = new Attack(this);
            var scouting = new Scouting(this, animator);
            
            // Assign state transitions
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
            
            // TODO Move attack state to any state 

            At(staticState, wander, NotStatic());
            
            At(wander, attack, FoundPlayer());
            At(wander, scouting, Scouting());
            At(wander, chase, ChasingPlayer());
            
            At(scouting, attack, FoundPlayer());
            At(scouting, wander, ScoutingFinished());
            At(scouting, chase, ChasingPlayer());
            
            At(attack, chase, ChasingPlayer());
            
            At(chase, attack, FoundPlayer());
            At(chase, wander, NotChasingPlayer());
            
            // Set start state.
            _stateMachine.SetState(staticState);
            
            // Transition logic between states.
            Func<bool> NotStatic() => () => !_isStatic;
            
            Func<bool> FoundPlayer() => () => Target != null;
            
            Func<bool> ChasingPlayer() => () => IsChasing;
            Func<bool> NotChasingPlayer() => () => !IsChasing;
            
            Func<bool> Scouting() => () => IsScouting;
            Func<bool> ScoutingFinished() => () => !IsScouting;
        }
        
        private void Start()
        {
            _levelManager = LevelManager.Get;
            
            // Register Enemy to LevelManager
            _levelManager.RegisterNewEnemy(this);
            
            _stateMachine.Tick();
        }

        private void Update()
        {
            FindTargetPlayer();
            _stateMachine.Tick();
            
            _fieldOfView.SetEnemyPosition(transform.position);
            _fieldOfView.SetEnemyDirection(transform.forward);
        }
        
        /// <summary>
        /// Checks if Player is in enemy sight.
        /// </summary>
        public void FindTargetPlayer()
        {
            if (Player == null)
            {
                Player = LevelManager.Get.PlayerTransform;
            }
            
            if (Vector3.Distance(transform.position, Player.position) < _enemyConfig.ViewDistance)
            {
                // Player inside viewDistance 
                var directionToPlayer = (Player.position - transform.position).normalized;
                
                if (Vector3.Angle(transform.forward, directionToPlayer) < _enemyConfig.Fov / 2f)
                {
                    // Player inside of view field
                    if (Physics.Raycast(transform.position, directionToPlayer, out var hit, _enemyConfig.ViewDistance))
                    {
                        var player = hit.collider.GetComponent<PlayerController>();

                        if (player != null)
                        {
                            // PLayer is visible to enemy
                            Target = Player;
                            IsChasing = false;
                            return;
                        }
                    }
                }
            }

            // If lost sight of target, start chasing him.
            if (Target == true)
            {
                IsChasing = true;
            }

            Target = null;
        }
        
        public void SpawnProjectile()
        {
            Instantiate(_enemyConfig.ProjectilePrefab, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
            
            _levelManager.NotifyEnemies();

            if(_bulletSpawnParticles)
                _bulletSpawnParticles.Play();

            if(_spawnAudioSource)
                _spawnAudioSource.Play();
        }

        /// <summary>
        /// Looks at Target (Player).
        /// </summary>
        public void LookAt()
        {
            if (Target == null)
                return;
            
            var targetPosition = Target.position;
            targetPosition.y = _bulletSpawnPoint.position.y;
            
            var direction = targetPosition - _bulletSpawnPoint.position;
            var rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 6f * Time.deltaTime);
        }

        public void CustomStartCoroutine(IEnumerator coroutine)
        {
            StartCoroutine(coroutine);
        }

        private void OnDestroy()
        {
            if (_fieldOfView != null)
            {
                Destroy(_fieldOfView.gameObject);
            }
        }

        [Serializable]
        public class Config
        {
            public GameObject FieldOfViewPrefab;
            public GameObject ProjectilePrefab;
            
            public float AggroRadius = 4f;
            public float AttackRange = 3f;
            
            [Header("FieldOfView")]
             public float Fov = 90f; 
             public float ViewDistance = 5f;
             public int RayCount = 100;
            
            [Header("Used by multiple states")]
            public float BasicMovementSpeed = 1f;
            public float AwareMovementSpeed = 2.5f;
            public float ChasingMovementSpeed = 5f;
            
            public LayerMask LayerMask;

            [Header("Wander State")]
            public float StopDistance = 1f;
            public float RayDistance = 1.5f;
            
            // Min and Max times until enemy enters scouting state
            public float MinTimeUntilScouting = 10f;
            public float MaxTimeUntilScouting = 30f;

            [Header("Chase State")] 
            public float ChaseTime = 4f;
            
            [Header("Attack State")] 
            public float SpawnRate = 0.1f;
            
            [Header("Scouting State")] 
            public float TurnTime = 1f;
            public int RotateAngle = 180;
        }
    }
}