using System;
using Web.RequestStructs;

namespace Web.ResponseStructs
{
    [Serializable]
    public class UpdateResponesData<T> 
    {
        public string request = "update";
        public Payload<T> payload = new Payload<T>();
        
        
        public void SetData(string field, T value, string signature)
        {
            payload.field = field;
            payload.value = value;
            payload.signature = signature;
        }
    }

    [Serializable]
    public class GetTimeResponesData 
    {
        public string request = "get_time";
        //public string payload = "";
        
    }

    
    [Serializable]
    public class TrackingOrderPriceRequest
    {
        public string request = "set_tracking_order_price";
        public TrackingOrderPricePayload payload;
    }

    [Serializable]
    public class TrackingOrderPricePayload
    {
        public string task_id; 
        public int price;
        public string signature;
    }

    [Serializable]
    public class TrackingProductRequest
    {
        public string request = "set_tracking_product";
        public TrackingProductRequestPayload payload;
    }

    [Serializable]
    public class TrackingProductRequestPayload
    {
        public string task_id; 
        public string guid;
        public string signature;
    }

    [Serializable]
    public class DodoCoinResponseData<T> where T : IdempotentPayload
    {
        public string request = "add_coins";
        public T payload;
    }
    
    [Serializable]
    public struct SignatureData
    {
        public string uid;
        public string field;
        public string value;

        public string GetSignature()
        {
            return $"{uid};{field};{value}";
        }
    }
    
    [Serializable]
    public class LoadResponesData
    {
        public string request = "load_user";
        public LoadData response;

        public void SetData(LoadData value)
        {
            response = value;
        }
    }

    [Serializable]
    public class StandartResponse
    {
        public string request;
        public string error;
        public string message;
        public string response;
    }

    [Serializable]
    public class TaskDoneResponse
    {
        public string request = "task_done";
        public string error;
        public string message;
        public TaskDoneData response;
    }

    [Serializable]
    public class TaskDoneData
    {
        public string task_id;
    }

    [Serializable]
    public class PromocodeResponse
    {
        public string request = "issue_promocode";
        public string error;
        public string message;
        public PromocodeData response;
    }

    [Serializable]
    public class PromocodeData
    {
        public string promocode;
    }
}