using InvestimentosJwt.Domain.Entities;
using InvestimentosJwt.Infra.Data.Sql.ProdutoRepository;

namespace InvestimentosJwt.Application.ProdutoService;

/// <summary>
/// Serviço responsável pela lógica de negócios relacionada a produtos.
/// Realiza a comunicação entre a camada de aplicação e o repositório de dados.
/// </summary>
public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="ProdutoService"/>.
    /// </summary>
    /// <param name="repository">Repositório responsável pelo acesso aos dados de produtos.</param>
    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista todos os produtos cadastrados.
    /// </summary>
    /// <returns>
    /// Uma coleção enumerável contendo todos os produtos.
    /// </returns>
    public async Task<IEnumerable<Produto>> ListarProdutos()
    {
        return await _repository.ObterTodos();
    }

    /// <summary>
    /// Obtém um produto específico pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador único do produto.</param>
    /// <returns>
    /// O produto encontrado ou <c>null</c> caso não exista.
    /// </returns>
    public async Task<Produto?> ObterProduto(int id)
    {
        return await _repository.ObterPorId(id);
    }

    /// <summary>
    /// Obtém um produto filtrando pelo tipo informado.
    /// </summary>
    /// <param name="tipo">Tipo do produto (ex: RendaFixa, CDB, LCI, etc.).</param>
    /// <returns>
    /// O produto encontrado ou <c>null</c> caso não exista.
    /// </returns>
    public async Task<Produto?> ObterPorTipo(string tipo)
    {
        return await _repository.ObterPorTipo(tipo);
    }
}
