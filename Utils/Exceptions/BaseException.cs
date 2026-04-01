public class BaseException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    public BaseException(string message, int statusCode, string errorCode = "ERROR_GENÉRICO") 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

// Ejemplo de excepciones específicas
public class NotFoundException : BaseException
{
    public NotFoundException(string message) 
        : base(message, 404, "NOT_FOUND") { }
}

public class BusinessException : BaseException
{
    public BusinessException(string message) 
        : base(message, 400, "ERROR_LÓGICA_DE_NEGOCIO") { }
}