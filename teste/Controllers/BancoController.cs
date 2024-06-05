using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using teste.Entities;
using teste.LojaContext;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace teste.Controllers
{
    public class BancoController : Controller
    {
        private readonly Context _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public BancoController(Context context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado

            // Filtrar os produtos com base no ID do usuário
            var banco = await _context.Bancos
            .Where(n => n.UsuarioID == user.Id)
            .Include(n => n.FkNotasNavigation)
            .ToListAsync();

            return View(banco);
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin, Gerente")]
        public IActionResult Retirada()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Retirada(Banco banco)
        {

            var user = await _userManager.GetUserAsync(User); // Obter o usuário atualmente logado
            banco.UsuarioID = user.Id;

            decimal? totalCaixa = _context.Bancos.Sum(t => t.Caixa) ?? 0;

            if (totalCaixa < 0)
            {
                TempData["Falha"] = "O valor do caixa é insuficiente.";
                return RedirectToAction("Index", "Banco");
            }

            // Verifica se o valor de retirada é maior que o total do caixa
            if (totalCaixa < banco.Saida)
            {
                TempData["Falha"] = "O valor do caixa é insuficiente.";
                return RedirectToAction("Index", "Banco");
            }

            // Verifica se o caixa ficará negativo após a retirada
            if (totalCaixa - banco.Saida < 0)
            {
                TempData["Falha"] = "A retirada resultará em caixa negativo.";
                return RedirectToAction("Index", "Banco");
            }

            // Realiza a retirada

            banco.Valor = banco.Saida;
            banco.Caixa = -banco.Saida;
            banco.Pagamento = "Retirada";

            _context.Bancos.Add(banco);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Valor retirado com sucesso";
            return RedirectToAction("Index", "Banco");
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin, Gerente")]
        public async Task<IActionResult> Analitica(DateTime? inicio, DateTime? final)
        {
            if (inicio == null || final == null)
            {
                // Trate o caso em que as datas não foram informadas
                return BadRequest("As datas de início e final devem ser informadas.");
            }
            var user = await _userManager.GetUserAsync(User);
            // Consultar o banco de dados para recuperar os registros de movimentações entre as datas especificadas
            var movimentacoes = await _context.Bancos
                                                .Where(m => m.Dia >= inicio && m.Dia <= final && m.UsuarioID == user.Id)
                                                .Include(n => n.FkNotasNavigation)
                                                .ToListAsync();

            // Retornar os resultados para a view
            return View(movimentacoes);
        }
    }
}

