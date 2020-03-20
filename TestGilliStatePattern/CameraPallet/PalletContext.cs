using System;
using System.Collections.Generic;
using System.Text;

namespace TestGilliStatePattern.CameraPallet
{
    // The Context defines the interface of interest to clients. It also
    // maintains a reference to an instance of a State subclass, which
    // represents the current state of the Context.
    class PalletContext
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();


        // A reference to the current state of the Context.
        private State _state = null;
        public Robot.RobotContext Robot;

        public PalletContext(State state, Robot.RobotContext robot)
        {
            this.TransitionTo(state);
            Robot = robot;
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
        public void StartCycle()
        {
            _state.AnalysePallet();
        }

        public void AnalyseNextRock()
        {
            _state.AnalysePallet();
        }

        public void Stop()
        {
            this._state.AnalysePallet();
        }
    }

    // The base State class declares methods that all Concrete State should
    // implement and also provides a backreference to the Context object,
    // associated with the State. This backreference can be used by States to
    // transition the Context to another State.
    abstract class State
    {
        protected PalletContext _context;

        public void SetContext(PalletContext context)
        {
            this._context = context;
        }

        public abstract void MoveTo(int encoderPOs);

        public abstract void AnalysePallet();

        public abstract void Stop();
    }

    // Concrete States implement various behaviors, associated with a state of
    // the Context.
    class IdleState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void MoveTo(int encoderPOs)
        {
            _log.Trace($" handles move to.");
        }

        public override void AnalysePallet()
        {
            _log.Trace($" handles analyse request.");
            _log.Trace($" wants to change the state of the context.");
            var state = new AnalyseState();
            this._context.TransitionTo(state);
            state.AnalysePallet();
        }

        public override void Stop() { }
    }

    class AnalyseState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void MoveTo(int encoderPOs)
        {
            _log.Trace($" handles request1.");
        }

        public override void AnalysePallet()
        {
            _log.Trace($" handles {nameof(AnalysePallet)}.");
            _log.Trace($" wants to change the state of the context.");

            this._context.TransitionTo(new IdleState());

            _log.Trace($" Telling robot to pick the rock");
            _context.Robot.Pick();
        }
        public override void Stop() => _context.TransitionTo(new IdleState());

    }

    class MovingState : State
    {
        NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        public override void AnalysePallet()
        {
            this._context.TransitionTo(new AnalyseState());
        }

        public override void MoveTo(int encoderPOs)
        {
            
        }

        public override void Stop()
        {
            _context.TransitionTo(new IdleState());
        }
    }

}
