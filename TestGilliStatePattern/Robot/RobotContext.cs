using System;
using System.Collections.Generic;
using System.Text;

namespace TestGilliStatePattern.Robot
{
    class RobotContext
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();


        // A reference to the current state of the Context.
        private State _state = null;
        public CameraNesting.NestingContext Nesting;

        public RobotContext(State state, CameraNesting.NestingContext nesting)
        {
            this.TransitionTo(state);
            Nesting = nesting;
        }

        // The Context allows changing the State object at runtime.
        public void TransitionTo(State state)
        {
            _log.Trace($" Transition to {state.GetType().Name}.");
            this._state = state;
            this._state.SetContext(this);
        }

        // The Context delegates part of its behavior to the current State
        // object.
        public void Pick()
        {
            _state.Pick();
        }

        public void Dropped()
        {
            _state.GoIdle();
        }

        public void Stop()
        {
            this._state.GoIdle();
        }
    }

    // The base State class declares methods that all Concrete State should
    // implement and also provides a backreference to the Context object,
    // associated with the State. This backreference can be used by States to
    // transition the Context to another State.
    abstract class State
    {
        protected RobotContext _context;

        public void SetContext(RobotContext context)
        {
            this._context = context;
        }

        public abstract void GoIdle();

        public abstract void Pick();

        public abstract void Drop();
    }

    // Concrete States implement various behaviors, associated with a state of
    // the Context.
    class IdleState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void Pick()
        {
            _log.Trace($" handles analyse {nameof(Pick)}.");
            _log.Trace($" wants to change the state of the context.");
            var state = new PickingState();
            this._context.TransitionTo(state);
            state.Drop();
        }

        public override void Drop() 
        {

        }

        public override void GoIdle()
        {
            
        }
    }

    class PickingState: State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void Pick()
        {
            _log.Trace($" handles analyse {nameof(Pick)}.");
            _log.Trace($" wants to change the state of the context.");

            Drop();
        }

        public override void Drop() 
        {
            _log.Trace($" wants to change the state of the context {nameof(DroppingState)}.");
            var state = new DroppingState();
            _context.TransitionTo(state);
            state.Drop();
        }

        public override void GoIdle()
        {
            _log.Trace($" wants to change the state of the context {nameof(IdleState)}.");
            _context.TransitionTo(new IdleState());
        }

    }

    class DroppingState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void Drop()
        {
            _log.Trace($" handles analyse {nameof(Pick)}.");

            _context.Nesting.AnalyseRock();

            _log.Trace($" Nesting is finished, dropping the rock and going to idle");
            _context.TransitionTo(new IdleState());
        }

        public override void GoIdle()
        {
            _log.Trace($" wants to change the state of the context {nameof(IdleState)}.");
            _context.TransitionTo(new IdleState());
        }

        public override void Pick()
        {
            _log.Trace($" is in dropping state, cannot go to {nameof(PickingState)}.");
        }
    }
}
