﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Cirrus.Circuit.UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField]
        private PlayerDisplay[] _playerDisplays;

        private List<PlayerDisplay> _availablePlayerDisplays;


        [SerializeField]
        private UnityEngine.UI.Text _levelName;

        [SerializeField]
        private UnityEngine.UI.Text _previous;

        [SerializeField]
        private UnityEngine.UI.Text _next;

        [SerializeField]
        private float _selectPunchScale = 0.5f;

        [SerializeField]
        private float _selectPunchScaleTime = 1f;

        [SerializeField]
        private GameObject _playerDisplay;

        [SerializeField]
        private GameObject _levelSelectDisplay;


        public void Awake()
        {
            _availablePlayerDisplays = new List<PlayerDisplay>();
        }


        public IEnumerator PunchScale(bool previous)
        {
            iTween.Stop(_previous.gameObject);
            iTween.Stop(_next.gameObject);

            _previous.transform.localScale = new Vector3(1, 1, 1);
            _next.transform.localScale = new Vector3(1, 1, 1);

            yield return new WaitForSeconds(0.01f);

            if (previous)
            {

                iTween.PunchScale(
                    _previous.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
            else
            {
                iTween.PunchScale(
                    _next.gameObject,
                    new Vector3(_selectPunchScale,
                    _selectPunchScale, 0),
                    _selectPunchScaleTime);
            }
        }


        public void OnWaiting()
        {
            _levelSelectDisplay.SetActive(false);
            _playerDisplay.SetActive(true);

            _availablePlayerDisplays.Clear();
            _availablePlayerDisplays.AddRange(_playerDisplays);

            for (int i = 0; i < Game.Instance.CurrentLevel.CharacterCount; i++)
            {
                _playerDisplays[i].TryChangeState(PlayerDisplay.State.Disconnected);
            }

            for (int i = 0; i < Game.Instance.Lobby.ControllerCount; i++)
            {
                _playerDisplays[i].TryChangeState(PlayerDisplay.State.Waiting);
            }
        }


        public void UpdateDisplay(int playerNumber, PlayerDisplay.State state)
        {
            if (_availablePlayerDisplays.Count != 0)
            {
                _availablePlayerDisplays[0].TryChangeState(PlayerDisplay.State.Ready, playerNumber)

                //_playerDisplays[index]?.TryChangeState(state);
            }

            
        }
        

        public void OnLevelSelect()
        {
            _levelSelectDisplay.SetActive(true);
            _playerDisplay.SetActive(false);
        }


        internal void OnLevelSelected(int step)
        {

            if (step < 0)
            {
                StartCoroutine(PunchScale(true));
            }
            else if(step > 0)
            {
                StartCoroutine(PunchScale(false));
            }

            _levelName.text = Game.Instance.CurrentLevel.Name;

            foreach (var display in _playerDisplays)
            {
                display.TryChangeState(PlayerDisplay.State.Disabled);
            }

            for (int i = 0; i < Game.Instance.CurrentLevel.CharacterCount; i++)
            {
                if (_playerDisplays[i] == null)
                    continue;

                _playerDisplays[i].Color = Game.Instance.Lobby.Colors[i];
            }
            
        }

        public void OnValidate()
        {
            OnLevelSelected(0);
        }
    }
}