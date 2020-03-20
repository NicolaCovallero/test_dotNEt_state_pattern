using System;
using System.Collections.Generic;
using System.Text;

namespace TestGilliStatePattern.CameraNesting
{
    class NestingContext
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        // A reference to the current state of the Context.
        private State _state = null;

        public NestingContext(State state)
        {
            this.TransitionTo(state);
        }

        // The Context allows changing the State object at runtime.
        public void TransitionTo(State state)
        {
            _log.Trace($"Transition to {state.GetType().Name}");
            this._state = state;
            this._state.SetContext(this);
        }

        // The Context delegates part of its behavior to the current State
        // object.
        public void AnalyseRock()
        {
            _state.AnalyseRock();
        }

        public void Stop()
        {
            this._state.Stop();
        }
    }

    // The base State class declares methods that all Concrete State should
    // implement and also provides a backreference to the Context object,
    // associated with the State. This backreference can be used by States to
    // transition the Context to another State.
    abstract class State
    {
        protected NestingContext _context;

        public void SetContext(NestingContext context)
        {
            this._context = context;
        }

        public abstract void AnalyseRock();

        public abstract void Stop();
    }

    // Concrete States implement various behaviors, associated with a state of
    // the Context.
    class IdleState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void AnalyseRock()
        {
            _log.Trace($" handles analyse {nameof(AnalyseRock)}.");
            _log.Trace($" wants to change the state of the context.");
            var state = new AnalyseState();
            this._context.TransitionTo(state);
            state.AnalyseRock();
        }

        public override void Stop() { }
    }

    class AnalyseState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void AnalyseRock() 
        {
            _log.Trace($" handles analyse {nameof(AnalyseRock)}.");
            _log.Trace($" wants to change the state of the context.");
            this._context.TransitionTo(new IdleState());
        }

        public override void Stop() => _context.TransitionTo(new IdleState());

    }

}
