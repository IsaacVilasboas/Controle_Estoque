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
    public class ProdutosController : Controller
    {
        private readonly Context _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public ProdutosController(Context context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
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
            var produtos = await _context.Produtos
                .Where(p => p.UsuarioID == user.Id)
                .ToListAsync();

            return View(produtos);
        }

        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.Id == id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {

            if (ModelState.IsValid) // Verifica se o modelo é válido
            {
                var user = await _userManager.GetUserAsync(User); // Obtém o usuário atualmente logado
                produto.UsuarioID = user.Id; // Atribui o ID do usuário ao produto

                // Verifica se já existe um produto com o mesmo código de barras para o usuário atual
                if (_context.Produtos.Any(p => p.CodBarra == produto.CodBarra && p.UsuarioID == user.Id))
                {
                    ModelState.AddModelError("CodBarra", "Já existe um produto com este código de barras.");
                    return View(produto); // Retorna a view com erro se o produto já existir
                }

                // Busca todos os produtos existentes com as mesmas informações de categoria, marca e tipo
                var produtosComMesmasInformacoes = await _context.Produtos
                    .Where(p => p.Categoria == produto.Categoria && p.Marca == produto.Marca && p.Tipo == produto.Tipo)
                    .ToListAsync();

                if (produtosComMesmasInformacoes.Count > 0) // Se existirem produtos com as mesmas informações
                {
                    // Calcula o valor total somando os valores dos produtos existentes com o valor do novo produto
                    decimal? valorTotal = produtosComMesmasInformacoes.Sum(p => p.Preco) + produto.Preco;
                    // Calcula a nova média dos valores considerando todos os produtos existentes e o novo produto
                    decimal? media = valorTotal / (produtosComMesmasInformacoes.Count + 1);

                    produto.Media = media;
                    _context.Produtos.Add(produto); // Adiciona o novo produto ao contexto
                }
                else // Se não existirem produtos com as mesmas informações
                {
                    produto.Media = produto.Preco; // Define a média como o valor do novo produto
                    _context.Produtos.Add(produto); // Adiciona o novo produto ao contexto
                }

                TempData["Sucesso"] = "Produto adicionado com sucesso"; // Define uma mensagem de sucesso para exibição posterior
                await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados
                return RedirectToAction(nameof(Index)); // Redireciona para a página Index
            }

            return View(produto); // Retorna a view com o produto em caso de modelo inválido
        }

            [Authorize(Roles = "User,Admin, Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
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

        // POST: Produtos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado


                    var pd = await _context.Produtos.FirstOrDefaultAsync(p=> p.Id == produto.Id);
                    if (pd != null)
                    {
                        pd.Preco = produto.Preco;
                        pd.CodBarra = produto.CodBarra;
                        pd.Marca = produto.Marca;
                        pd.Cadastro = produto.Cadastro;
                        pd.Validade = produto.Validade;
                        pd.NomeProduto = produto.NomeProduto;
                        pd.Qtd = produto.Qtd;
                        pd.Categoria = produto.Categoria;
                        pd.Sabor = produto.Sabor;
                        pd.Recipiente = produto.Recipiente;
                        pd.Tipo = produto.Tipo;
                        pd.Peso = produto.Peso;
                        pd.Validade = produto.Validade;
                        pd.UsuarioID = user.Id;
                        _context.Update(pd);
                        await _context.SaveChangesAsync();

                    }

                    var pi = await _context.Produtos
                    .Where(p => p.Categoria == produto.Categoria && p.Marca == produto.Marca && p.Tipo == produto.Tipo)
                    .ToListAsync();

                    if (pi.Count > 0) // Se existirem produtos com as mesmas informações
                    {
                        // Calcula o valor total somando os valores dos produtos existentes com o valor do novo produto
                        decimal? valorTotal = pi.Sum(p => p.Preco);
                        // Calcula a nova média dos valores considerando todos os produtos existentes e o novo produto
                        decimal? media = valorTotal / (pi.Count);

                        pd.Media = media;
                        _context.Update(pd);

                    }
                    else // Se não existirem produtos com as mesmas informações
                    {
                        pd.Media = produto.Preco; // Define a média como o valor do novo produto
                        _context.Update(pd);

                    }

                    
                    TempData["Sucesso"] = "Produto alterado com sucesso";
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.Id == id);

            
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado
            var nota = await _context.Notas.FirstOrDefaultAsync(x => x.FkProdutos == produto.Id);

            if (nota != null)
            {
                TempData["Falha"] = "Não é possivel excluir um produto sem ter excluido a sua nota de venda ";
                return RedirectToAction(nameof(Index));
            }
            if (produto.UsuarioID != user.Id) // Verificar se o usuário logado é o criador do produto
            {
                return Forbid(); // Ou retorne uma página de erro
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Produto removido com sucesso";
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
