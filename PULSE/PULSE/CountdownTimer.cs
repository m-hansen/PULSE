using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace PulseGame
{
    class CountdownTimer
    {
        private Timer timer;
        private int timeInMsRemaining;
        private double deltaT;
        private bool isAudioEnabled = false;
        private bool countdownFinished = false;

        public CountdownTimer(int countdownTimeInMs, bool isAudio)
        {
            timer = new Timer(1000); // 1 second in ms
            isAudioEnabled = isAudio;
            timeInMsRemaining = countdownTimeInMs;
            timer.Start();
        }

        public void Update()
        {

            // single countdown tick
            if ((isAudioEnabled) && (deltaT > 1))
            {
                //countdownSound.Play();
                deltaT = 0;
                //countdown -= deltaT;
            }

            if (timeInMsRemaining <= 0)
            {
                countdownFinished = true;
                timer.Stop();
                //gameState = (int)Enums.GameState.Gameplay;
                //timer = 0;
                //deltaT = 0;
                //sw.Stop();
                //// begin playing background music
                //if (!songStart)
                //{
                //    MediaPlayer.Play(backgroundMusic);
                //    songStart = true;
                //}
            }
        }
    }
}
