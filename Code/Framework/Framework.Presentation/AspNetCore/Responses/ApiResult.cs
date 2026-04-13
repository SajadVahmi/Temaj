namespace Framework.Presentation.AspNetCore.Responses;

public class ApiResult<T>(T data) : ApiResult
{
    public T Data { get; set; } = data;
}
public class ApiResult
{
    public ErrorResult? Errors { get; set; }
}
public class ErrorResult
{
    public string? Title { get; set; }
    public Dictionary<string, string[]>? Metadata { get; set; }
}
