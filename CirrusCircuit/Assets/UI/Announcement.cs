﻿using UnityEngine;
using System.Collections;

namespace Cirrus.Circuit.UI
{
    public class Announcement : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text _text;

        [SerializeField]
        private Game _game;

        private int _number = 0;

        public void OnValidate()
        {
            if (_game == null)
                _game = FindObjectOfType<Game>();
        }

        public int Number {
            get {
                return _number;
            }

            set
            {
                Enabled = true;
                _number = value;
                _text.text = "Round " + (_number+1).ToString();                
                _timer.Start();
            }

        }

        private bool _enabled = false;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                transform.GetChild(0).gameObject.SetActive(_enabled);
            }
        }

        private float _time = 2f;

        private Circuit.Timer _timer;


        private Circuit.Timer _timesUpTimer;

        [SerializeField]
        private float _timesUpTime = 2f;

        public void Awake()
        {
            _game.OnNewRoundHandler += OnNewRound;
            _timesUpTimer = new Circuit.Timer(_timesUpTime, start: false, repeat: false);
            _timesUpTimer.OnTimeLimitHandler += OnTimesUpTimeOut;

            _timer = new Circuit.Timer(_time, start: false, repeat: false);
            _timer.OnTimeLimitHandler += OnTimeOut;
        }

        public void OnNewRound(Round round)
        {
            round.OnRoundEndHandler += OnRoundEnd;
            round.OnIntermissionHandler += OnIntermission;
        }

        public void OnIntermission(int count)
        {
            Number = count;
        }

        public void OnRoundEnd()
        {
            Enabled = true;
            _timesUpTimer.Start();
        }

        public void OnTimeOut()
        {
            Enabled = false;
        }

        private void OnTimesUpTimeOut()
        {
            Enabled = false;
        }
    }
}