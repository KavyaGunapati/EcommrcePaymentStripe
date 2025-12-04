namespace Models.DTOs
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message=string.Empty;
        public objeck? Data {get;set;}
    }
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public  T? Data { get; set; }

    }

}
