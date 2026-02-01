using Application.DTOs.Invoice;
using Application.DTOs.Shared;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Applications
{
    public class InvoiceApplication : IInvoiceApplication
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IComissaoRepository _comissaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceApplication(
            IInvoiceRepository invoiceRepository,
            IVendedorRepository vendedorRepository,
            IComissaoRepository comissaoRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _vendedorRepository = vendedorRepository;
            _comissaoRepository = comissaoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Criar(CriarInvoiceDto dto)
        {
            var vendedor = await _vendedorRepository.ListarPorId(dto.VendedorId);

            if (vendedor == null)
            {
                throw new Exception("Vendedor não encontrado");
            }

            var numero = await _invoiceRepository.BuscarProximoNumeroInvoice();

            var invoice = new Invoice(vendedor, numero, dto.DataEmissao, dto.Cliente, dto.CnpjCpfCliente, dto.ValorTotal, dto.Observacoes);

            await _invoiceRepository.Criar(invoice);
            await _unitOfWork.CommitAsync();


            var comissao = new Comissao(invoice.Id, invoice.ValorTotal, vendedor.PercentualComissao);

            await _comissaoRepository.Criar(comissao);
            await _unitOfWork.CommitAsync();

            return invoice.Id;
        }

        public async Task Aprovar(Guid id)
        {
            var invoice = await _invoiceRepository.ListarPorId(id);
            
            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            invoice.Aprovar();

            await _invoiceRepository.Editar(invoice);
            await _unitOfWork.CommitAsync();
        }

        public async Task Cancelar(Guid id)
        {
            var invoice = await _invoiceRepository.ListarPorId(id);
            
            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            invoice.Cancelar();
            invoice.Comissao.Cancelar();

            await _invoiceRepository.Editar(invoice);
            await _unitOfWork.CommitAsync();
        }

        public async Task Editar(Guid id, EditarInvoiceDtoEnxuto dto)
        {
            var invoice = await _invoiceRepository.ListarPorId(id);
            
            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            if (invoice.Status == StatusInvoice.Aprovada)
            {
                throw new Exception("Não é permitido alterar invoice aprovada.");
            }

            var vendedor = await _vendedorRepository.ListarPorId(dto.VendedorId);

            invoice.AlterarVendedor(vendedor);
            invoice.AlterarValorTotal(dto.ValorTotal);
            await EditarComissao(invoice, vendedor);

            await _unitOfWork.CommitAsync();
        }

        private async Task EditarComissao(Invoice invoice, Vendedor vendedor)
        {
            var comissao = await _comissaoRepository.ListarPorInvoice(invoice.Id);

            if (comissao == null)
            {
                throw new Exception("Comissão não encontrada para esta invoice.");
            }

            var valorComissao = Math.Round(invoice.ValorTotal * (vendedor.PercentualComissao / 100), 2);

            comissao.AtualizarValores(invoice.ValorTotal, vendedor.PercentualComissao, valorComissao);

            await _unitOfWork.CommitAsync();
        }

        public async Task<EditarInvoiceDto> ListarPorId(Guid id)
        {
            var invoice = await _invoiceRepository.ListarPorId(id);

            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            return new EditarInvoiceDto
            {
                Id = invoice.Id,
                Numero = invoice.NumeroInvoice,
                Cliente = invoice.Cliente,
                DataEmissao = invoice.DataEmissao,
                Status = invoice.Status.ToString(),
                VendedorId = invoice.VendedorId,
                ValorTotal = invoice.ValorTotal
            };
        }

        public async Task<IReadOnlyList<ListarInvoiceDto>> Listar(Guid vendedorId)
        {
            var invoices = await _invoiceRepository.ListarPorVendedor(vendedorId);
            return invoices.Select(i => new ListarInvoiceDto
            {
                Id = i.Id,
                NumeroInvoice = i.NumeroInvoice,
                Cliente = i.Cliente,
                DataEmissao = i.DataEmissao,
                Status = i.Status,
                ValorTotal = i.ValorTotal,
                Vendedor = new VendedorDto
                {
                    Id = i.Vendedor.Id,
                    NomeCompleto = i.Vendedor.NomeCompleto
                }
            }).ToList();
        }

        public async Task<PagedResult<InvoiceDto>> ListarPaginado(Guid? vendedorId, int page, int pageSize)
        {
            var (items, total) = await _invoiceRepository.ListarPaginado(vendedorId, page, pageSize);

            return new PagedResult<InvoiceDto>
            {
                Items = items.Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    VendedorId = i.VendedorId,
                    ValorTotal = i.ValorTotal,
                    Status = i.Status,
                    DataEmissao = i.DataEmissao,
                    Cliente = i.Cliente,
                    NumeroInvoice = i.NumeroInvoice,
                    Vendedor = new VendedorDto()
                    {
                        Id = i.VendedorId,
                        NomeCompleto = i.Vendedor.NomeCompleto
                    }
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
