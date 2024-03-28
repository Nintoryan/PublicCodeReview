using System;
using DG.Tweening;
using MPUIKIT;
using UnityEngine;

namespace Util
{
    [Serializable]
    public class BoxStyle
    {
        public Color Background;
        public Color Outline;

        public void ApplyToMpImage(MPImage image, float duration = -1f)
        {
            if (Math.Abs(duration + 1f) < 0.1f)
            {
                image.color = Background;
                image.OutlineColor = Outline;
            }
            else
            {
                DOTween.To(()=> image.color, x=> image.color = x, Background, duration);
                DOTween.To(()=> image.OutlineColor, x=> image.OutlineColor = x, Outline, duration);
            }
            
        }
    }
}