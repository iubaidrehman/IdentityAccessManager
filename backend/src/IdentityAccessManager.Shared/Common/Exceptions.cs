namespace IdentityAccessManager.Shared.Common;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class ValidationException : DomainException
{
    public List<string> Errors { get; }

    public ValidationException(string message, List<string> errors) : base(message)
    {
        Errors = errors;
    }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message) { }
}

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message) : base(message) { }
} 