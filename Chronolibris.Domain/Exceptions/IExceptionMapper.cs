namespace Chronolibris.Domain.Exceptions
{
    public interface IExceptionMapper
    {
        (int StatusCode, string Title, string Detail) Map(Exception exception);
    }
}
