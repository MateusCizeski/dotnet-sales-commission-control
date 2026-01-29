namespace Application.Interfaces
{
    public interface IComissaoApplication
    {
        Task MarcarComoPagaAsync(Guid id);
        Task MarcarComoCanceladaAsync(Guid id);
    }
}
