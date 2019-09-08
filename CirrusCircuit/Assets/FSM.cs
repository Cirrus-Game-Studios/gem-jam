﻿using UnityEngine;
using System.Collections;
using Cirrus.Circuit.Controls;
using System;

namespace Cirrus.Circuit
{
    [System.Serializable]
    public class FSM : MonoBehaviour
    {
        [System.Serializable]
        public enum State
        {
            LevelSelection,
            Round,
            Score,
            WaitingNextRound,
        }

        [SerializeField]
        public State _state = State.LevelSelection;

        public void Awake()
        {
            TryChangeState(State.LevelSelection);
        }

        public void FixedUpdate()
        {
            switch (_state)
            {
                case State.LevelSelection:
                case State.Round:
                case State.Score:

                    Game.Instance._camera.orthographicSize =
                        Mathf.Lerp(
                            Game.Instance._camera.orthographicSize,
                            Game.Instance._targetSizeCamera,
                            Game.Instance._cameraSizeSpeed);

                    break;
            }
        }

        public void Update()
        {
            switch (_state)
            {
                case State.LevelSelection:
                case State.Round:
                case State.Score:
                    break;
            }
        }

        public bool TryChangeState(State transition, params object[] args)
        {
            if (TryTransition(transition, out State destination))
            {
                return TryFinishChangeState(destination, args);
            }

            return false;
        }

        protected bool TryTransition(State transition, out State destination, params object[] args)
        {
            switch (_state)
            {
                case State.Round:

                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.LevelSelection:
                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Score:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.Score:
                    switch (transition)
                    {
                        case State.WaitingNextRound:
                        case State.LevelSelection:
                        case State.Round:
                            destination = transition;
                            return true;
                    }
                    break;

                case State.WaitingNextRound:
                    switch (transition)
                    {
                        case State.LevelSelection:
                        case State.Round:
                            destination = transition;
                            return true;
                    }
                    break;
            }

            destination = State.Round;
            return false;
        }

        internal void Join(Controller ctrl)
        {

            switch (_state)
            {
                case State.LevelSelection:


                    break;
            }
        }

        protected bool TryFinishChangeState(State target, params object[] args)
        {

            switch (target)
            {
                case State.LevelSelection:

                    foreach(Levels.Level lv in Game.Instance._levels)
                    {
                        lv.gameObject.SetActive(true);
                    }

                    Game.Instance.OnLevelSelected(0);

                    return true;

                case State.Round:
                    return true;

                case State.Score:

                    return true;

                case State.WaitingNextRound:

                    return true;
                default:
                    return false;
            }

        }

        private bool _wasMovingVertical = false;

        // TODO: Simulate LeftStick continuous axis with WASD
        public void HandleAxesLeft(Controller controller, Vector2 axis)
        {
            bool isMovingHorizontal = Mathf.Abs(axis.x) > 0.5f;
            bool isMovingVertical = Mathf.Abs(axis.y) > 0.5f;

            Vector3 stepHorizontal = new Vector3(Mathf.Sign(axis.x), 0, 0);
            Vector3 stepVertical = new Vector3(0, 0, Mathf.Sign(axis.y));
            Vector3 step = Vector3.zero;

            if (isMovingVertical && isMovingHorizontal)
            {
                //moving in both directions, prioritize later
                if (_wasMovingVertical)
                {
                    step = stepHorizontal;
                }
                else
                {
                    step = stepVertical;
                }
            }
            else if (isMovingHorizontal)
            {
                step = stepHorizontal;
                _wasMovingVertical = false;
               
                
            }
            else if (isMovingVertical)
            {
                step = stepVertical;
                _wasMovingVertical = true;
            }


            switch (_state)
            {

                case State.LevelSelection:

                    if (Mathf.Abs(step.x) > 0)
                    {

                        int prev = Game.Instance._currentLevelIndex;

                        Game.Instance._currentLevelIndex =
                            Mathf.Clamp(Game.Instance._currentLevelIndex + (int)Mathf.Sign(step.x), 0, Game.Instance._levels.Length);

                        if (prev != Game.Instance._currentLevelIndex)
                        { 
                            Game.Instance.OnLevelSelected((int)Mathf.Sign(step.x));
                        }
                    }

                    break;

                case State.WaitingNextRound:

                    break;

                case State.Round:
                    //character?.TryMove(axis);

                    break;

                case State.Score:
                    break;
            }
        }

        public void HandleAction0(Controller controller)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    break;

                case State.WaitingNextRound:

                    //Game.Instance.Lobby.Join()
                    TryChangeState(State.LevelSelection);

                    break;

                case State.Round:
                    controller.Character?.TryAction0();
                    break;

                case State.Score:
                    break;
            }

        }


        public void HandleAction1(Controller controller)
        {
            switch (_state)
            {
                case State.LevelSelection:
                    TryChangeState(State.WaitingNextRound);
                    break;

                case State.WaitingNextRound:

                    if (controller.Character == null)
                    {
                        controller.Character = 
                            Game.Instance.CurrentLevel.Characters[Game.Instance.Lobby.ControllerCount - 1];
                    }
                    else if(
                        Game.Instance.Lobby.ControllerCount >= 
                        Game.Instance.CurrentLevel.CharacterCount)
                    {
                        // Start
                    }

                    break;

                case State.Round:
                    controller.Character?.TryAction0();
                    break;

                case State.Score:
                    break;
            }

        }

    }
}