namespace HotelManagement.API.Common
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public BaseResponse(bool success, string message, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static BaseResponse Ok(object data = null, string message = "Success") =>
            new BaseResponse(true, message, data);

        public static BaseResponse Error(string message) =>
            new BaseResponse(false, message);
    }
}
