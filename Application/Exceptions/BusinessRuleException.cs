namespace Application.Exceptions
{
    /// <summary>
    /// A rule that needs data outside one aggregate (e.g. "CPF already registered") was violated.
    /// Mapped to HTTP 400 by the exception handler; the message is shown to the client.
    /// "Not found" uses ApiBase.Domain.Exceptions.NotFoundException (HTTP 404).
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
