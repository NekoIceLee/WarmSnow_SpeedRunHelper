﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WarmSnow_SpeedRunHelper
{
    public class TimeControl
    {
        public static TimeControl Instance { get; private set; } = new TimeControl();
        double timeSeconds;
        public int MiliSeconds => (int)(Math.Round(timeSeconds - (int)timeSeconds, 3) * 1000);
        public int Seconds => (int)(timeSeconds % 60);
        public int Minutes => (int)(timeSeconds / 60);
        public string StrMiliSeconds => $"{MiliSeconds}";
        public string StrSeconds => $"{Seconds}";
        public string StrMinutes => $"{Minutes}";
        public string StrTime => $"{Minutes}:{Seconds}.{MiliSeconds}";
        public static bool IsBossHenshin { get; set; } = false;
        public double TimeSeconds => timeSeconds; 
        public bool ContinueTimeGo
        {
            get
            {
                bool temp;
                temp = CameraControl.instance?.CanManipulate is true && UI_TimeCount.instance?.isStart is true;
                temp &= !IsBossHenshin;
                temp &= !(UsuallyCutScene.instance?.isOn is true);
                temp &= UI_PlayerUICanvas.instance?.isShow is true;
                return temp;
            }
        }
        public double stage1EndTime = 0;
        public double stage2EndTime = 0;
        public double stage3EndTime = 0;
        public double stage4EndTime = 0;
        public double stage5EndTime = 0;
        public double stage6EndTime = 0;
        private TimeControl()
        {

        }

        public void OnRunEnd(string message)
        {
            timeSeconds = 0;
        }

        public void Update()
        {
            if (ContinueTimeGo)
            {
                timeSeconds += Time.deltaTime;
            }
        }
        public override string ToString()
        {
            return StrTime;
        }
    }
}
