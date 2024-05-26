namespace WebApplication1.Exceptions;

public class BadRequestException(string message) : Exception(message);