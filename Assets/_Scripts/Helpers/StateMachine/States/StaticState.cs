namespace Helpers.StateMachine.States
{
    /// <summary>
    /// Enemy cannot move during the state, used in first level (tutorial) 
    /// </summary>
    public class StaticState : IState
    {
        public StaticState() { }

        public void OnEnter() { }

        public void OnExit() { }
        
        public void Tick() { }
    }
}