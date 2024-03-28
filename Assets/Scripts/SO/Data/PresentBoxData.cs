using System;
using SO.Data.Presents;
using UnityEngine;
using Util;

namespace VillageGame.Data
{
    [CreateAssetMenu(fileName = "PresentBoxData", menuName = "SO/Data/Presenets/PresentBoxData")]
    public class PresentBoxData: ScriptableObject
    {
        public int ID;
        public string Title;
        public Sprite BoxSprite;
        public Sprite OpenBoxSprite;
        public DayData OpenDate;
        public PresentValueData PresentValue;
       
        public bool IsCanOpen(DateTime day)
        {
            DateTime openDateTime = OpenDate.AsDateTime();
            return OpenDate.IsEmpty || day >= openDateTime;
        }
        
        public bool IsCanOpen(DayData currentDay)
        {
            Debugging.Log($"PresentBoxData: Is can open " +
                          $"{currentDay.AsDateTime()} >= {OpenDate.AsDateTime()} " +
                          $"= {currentDay.AsDateTime() >= OpenDate.AsDateTime()}" +
                          $"", ColorType.Lime);
            return OpenDate.IsEmpty || currentDay.AsDateTime() >= OpenDate.AsDateTime();
        }


    }
}