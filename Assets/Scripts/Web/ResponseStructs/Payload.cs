using System;

namespace Web.ResponseStructs
{
    [Serializable]
    public class Payload<T>
    {
        public string field;
        public T value;
        public string signature;
    }

  

    
    [Serializable]
    public class AddIdempotentPayload : IdempotentPayload
    {
        public int type;
    }
    
    [Serializable]
    public class RemoveIdempotentPayload : IdempotentPayload
    {
        public int amount;
        public int country_code;
    }

    [Serializable]
    public class PromocodeIdempotentPayload : IdempotentPayload
    {
        public int type;
        public int country_code;
    }

    [Serializable]
    public class IdempotentPayload
    {
        public string idempotency_key;
        public string signature; 
    }
}