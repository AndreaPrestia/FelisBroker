namespace FelisBroker.Common.Helpers;

public class ValidationResponse
{
    public bool Success { get; private set; }
    public string[] Errors { get; private set; }

    public static ValidationResponse Ok() => new() { Success = true, Errors = [] };

    public static ValidationResponse Ko(params string[] errors) =>
        new() { Success = false, Errors = errors };

    public override string ToString() => Success ? "Valid" : string.Join("; ", Errors);
}