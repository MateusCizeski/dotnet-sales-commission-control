namespace Application.Interfaces
{
    public interface IComissaoApplication
    {
        Task MarcarComoPagaAsync(Guid id);
    }
}
