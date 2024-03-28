using System;
using VillageGame.Data.Types;

namespace VillageGame.Services
{
    [Serializable]
    public class NotificationStateData
    {
        public WindowType Type;
        public bool IsHasAttention;
    }
}