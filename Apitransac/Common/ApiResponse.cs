namespace Apitransac.Common
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public int StatusCode { get; set; }

        public T? Data { get; set; }

        public List<ApiError> Errors { get; set; } = new();
    }
}
