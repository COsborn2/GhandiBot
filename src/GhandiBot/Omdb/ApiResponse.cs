namespace GhandiBot.Omdb
{
    public class ApiResponse<T>
    {
        public ApiResponse(T @object)
        {
            Object = @object;
            Success = true;
        }

        public ApiResponse()
        { }
        
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Object { get; set; }
    }
}