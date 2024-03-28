using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class TimeResponse
    {
        public string request = "get_time";
        public TimeResponseSmall response;
    }

    [Serializable]
    public class TimeResponseSmall
    {
        public string time;
    }
}