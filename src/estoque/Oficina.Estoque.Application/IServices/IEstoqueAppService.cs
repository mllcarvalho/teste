namespace Oficina.Estoque.Application.IServices
{
    public interface IEstoqueAppService
    {
        Task AdicionarPecaAoEstoque(Guid id, int quantidade);
        Task RemoverPecaDoEstoque(Guid id, int quantidade);
        Task<int> ConsultarQuantidadeEmEstoque(Guid pecaId);
    }
}
