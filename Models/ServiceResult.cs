using System.Text.Json.Serialization;

namespace EliteSocials.Models
{
    public class ServiceResult<T>
    {
        // Field declaration
        public bool isSuccess { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ApiResponseData<T> data { get; set; } = default;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string message { get; set; } = default;

        // Constructors
        public ServiceResult()
        {

        }
        public ServiceResult(bool isSuccess, ApiResponseData<T> responseData)
        {
            this.isSuccess = isSuccess;
            data = responseData;
        }
        public ServiceResult(bool isSuccess, string message)
        {
            this.isSuccess = isSuccess;
            this.message = message;
        }

        // Success responses
        public static ServiceResult<T> AsSuccess(ApiResponseData<T> responseData)
        {
            return new ServiceResult<T>(true, responseData);
        }
        public static ServiceResult<T> AsSuccess(string message)
        {
            return new ServiceResult<T>(true, message);
        }

        // Failure responses
        public static ServiceResult<T> AsFailure(string message)
        {
            return new ServiceResult<T>(false, message);
        }
    }
}
