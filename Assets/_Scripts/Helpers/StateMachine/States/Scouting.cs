using System.Collections;
using Enemies;
using Helpers.Settings;
using UnityEngine;

namespace Helpers.StateMachine.States
{
    public class Scouting : IState
    {
        private readonly Enemy _enemy;
        private readonly Animator _animator;
        private readonly Enemy.Config _enemyConfig;
        
        private Coroutine _turningCoroutine;
        private bool _isRotating; 

        public Scouting(Enemy enemy, Animator animator)
        {
            _enemy = enemy;
            _animator = animator;
            
            _enemyConfig = GameSettings.Get.EnemyConfig;
        }
        
        public void OnEnter()
        {
            _animator.SetFloat("Strafe", 0);
            _animator.SetFloat("Forward", 0);

            _isRotating = false;
        }

        public void OnExit()
        {
            _enemy.StopCoroutine(_turningCoroutine);
        }

        public void Tick()
        {
            if (!_isRotating)
            {
                //_turningCoroutine = _enemy.StartCoroutine(RotateEnemy());
                _turningCoroutine = _enemy.StartCoroutine(RotateAround());

                _isRotating = true;
            }
        }
        
        private IEnumerator RotateEnemy() 
        {    
            var angleVector = new Vector3(0, _enemyConfig.RotateAngle, 0);
            
            var fromAngle = _enemy.transform.rotation;
            var toAngle = Quaternion.Euler(_enemy.transform.eulerAngles + angleVector);
            
            for(var t = 0f; t < 1; t += Time.deltaTime/_enemyConfig.TurnTime) 
            {
                _enemy.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
                yield return null;
            }

            _enemy.IsScouting = false;
        }
        
        /// <summary>
        /// Temporary Rotation, to continue on the same path.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateAround()
        {
            // TODO Switch to RotateEnemy()
            
            var startRotation = _enemy.transform.eulerAngles.y;
            var endRotation = startRotation + 360.0f;
            var t = 0.0f;
            
            while ( t  < _enemyConfig.TurnTime )
            {
                t += Time.deltaTime;
                
                float yRotation = Mathf.Lerp(startRotation, endRotation, t / _enemyConfig.TurnTime) % 360.0f;
                _enemy.transform.eulerAngles = 
                    new Vector3(_enemy.transform.eulerAngles.x, yRotation, _enemy.transform.eulerAngles.z);
                
                yield return null;
            }
            
            _enemy.IsScouting = false;
        }
    }
}