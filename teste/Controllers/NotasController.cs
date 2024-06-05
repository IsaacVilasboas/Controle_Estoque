using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using teste.Entities;
using teste.LojaContext;

namespace teste.Controllers
{
    [Authorize]
    public class NotasController : Controller
    {
        private readonly Context _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public NotasController(Context context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //[Authorize(Policy = "IsFuncionarioClaimAccess")]

        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado

            // Filtrar os produtos com base no ID do usuário
            var notas = await _context.Notas
            .Where(n => n.UsuarioID == user.Id)
            .Include(n => n.FkProdutosNavigation)
            .ToListAsync();

            return View(notas);
        } 

        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Notas
                .FirstOrDefaultAsync(m => m.Id == id);
            var relacao = await _context.Produtos.FirstOrDefaultAsync(m => m.Id == produto.FkProdutos);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        [Authorize(Roles = "User, Admin, Gerente")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string cod, Nota p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado
                    p.UsuarioID = user.Id; // Atribuir o ID do usuário ao produto

                    var produto = await _context.Produtos.Where(n => n.UsuarioID == p.UsuarioID).FirstOrDefaultAsync(x => x.CodBarra == cod);

                    if (produto != null && produto.Qtd >= p.Uni)
                    {
                        produto.Qtd -= p.Uni;

                        p.FkProdutos = produto.Id;
                        p.Cod = produto.NomeProduto;
                        _context.Notas.Add(p);
                        await _context.SaveChangesAsync();

                        Banco banco = new Banco()
                        {
                            FkNotas = p.Id,
                            UsuarioID = user.Id,
                            Valor = p.Valor,
                            Razao = p.Cod,
                            Pagamento = p.Pagamento,
                            Caixa = (p.Valor * p.Uni) - p.Gastos,
                            Dia = p.DataSaida,

                        };

                        _context.Bancos.Add(banco);

                        
                        await _context.SaveChangesAsync();
                        TempData["Sucesso"] = "Nota adicionada com sucesso";
                        return RedirectToAction("Index");
                    }

                    
                }
                
            
                TempData["Falha"] = "Quantidade insuficiente do produto em estoque ou produto não encontrado.";
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                // Capturando e exibindo a exceção interna
                TempData["Falha"] = "Ocorreu um erro ao salvar as alterações: " + ex.InnerException?.Message;
                return View(p); // Retornar a view com o objeto Nota p para que o usuário possa corrigir os dados
            }
            catch (Exception ex)
            {
                TempData["Falha"] = "Ocorreu um erro inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        [Authorize(Roles = "User,Admin, Gerente")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Notas.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado
            if (produto.UsuarioID != user.Id) // Verificar se o usuário logado é o criador do produto
            {
                return Forbid(); // Ou retorne uma página de erro
            }

            return View(produto);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Nota produto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado

                var nota = await _context.Notas.FirstOrDefaultAsync(p => p.Id == produto.Id);
                var produtoRelacionado = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == nota.FkProdutos);
                var banco = await _context.Bancos.FirstOrDefaultAsync(p => p.FkNotas == produto.Id);

                if (ModelState.IsValid)
                {
                    // Calcular a diferença na quantidade de produtos
                    int? diferencaEstoque = (int?)(produto?.Uni - nota?.Uni);

                    // Atualizar os dados da nota, exceto o código de barras
                    nota.Cpf = produto.Cpf;
                    nota.Valor = produto.Valor;
                    nota.Clientes = produto.Clientes;
                    nota.Uni = produto.Uni;
                    nota.DataSaida = produto.DataSaida;
                    nota.Pagamento = produto.Pagamento;
                    nota.UsuarioID = user.Id;
                    nota.Gastos = produto.Gastos;


                    if (produtoRelacionado != null)
                    {
                        // Atualizar o estoque do produto relacionado apenas se não for nulo
                        produtoRelacionado.Qtd -= diferencaEstoque;
                    }

                    // Atualizar a nota no banco de dados
                    _context.Notas.Update(nota);
                    await _context.SaveChangesAsync();

                    // Atualizar o valor e a retirada no banco
                    banco.Valor = nota.Valor;
                    banco.Caixa = (nota.Valor * nota.Uni) - nota.Gastos;
                    _context.Bancos.Update(banco);
                    await _context.SaveChangesAsync();


                    TempData["Sucesso"] = "Nota atualizada com sucesso";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Falha"] = $"Não foi possível atualizar a nota. Pois a quantidade tem que ser maior que 0";
                    return RedirectToAction(nameof(Edit), produto);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(produto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
    

        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Encontre a nota associada ao produto a ser excluído, incluindo os dados do produto
            var nota = await _context.Notas
                .Include(n => n.FkProdutosNavigation)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nota == null)
            {
                return NotFound();
            }

            return View(nota);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Encontrar a nota a ser excluída, incluindo os dados do produto associado
                var nota = await _context.Notas
                    .Include(n => n.FkProdutosNavigation)
                    .FirstOrDefaultAsync(n => n.Id == id);

                var user = await _userManager.GetUserAsync(User);

                if (nota.UsuarioID != user.Id)
                {
                    return Forbid(); // Ou redirecione para uma página de erro
                }

                // Verificar se a nota existe
                if (nota == null)
                {
                    return NotFound();
                }

                // Verificar se o usuário logado é o criador da nota

                var produto = await _context.Produtos.FirstOrDefaultAsync(x => x.Id == nota.FkProdutos);

                produto.Qtd += nota.Uni;
                _context.Produtos.Update(produto);


                var banco = await _context.Bancos.FirstOrDefaultAsync(x => x.FkNotas == nota.Id);
                if (banco != null)
                {
                    _context.Bancos.Remove(banco);
                }
                // Remover a nota do banco de dados
                _context.Notas.Remove(nota);

                // Salvar as alterações no banco de dados
                await _context.SaveChangesAsync();

                // Definir a mensagem de sucesso
                TempData["Sucesso"] = "Nota excluída com sucesso!";

                // Redirecionar para a página de listagem de notas
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Capturar e lidar com exceções inesperadas
                TempData["Falha"] = "Ocorreu um erro ao excluir a nota: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
