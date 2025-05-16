namespace APBD_example_test1_2025.Exceptions;

public class ConflictException : Exception
{

    public ConflictException(string? message) : base(message)
    {
    }
    public ConflictException()
    {
    }
    public ConflictException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}