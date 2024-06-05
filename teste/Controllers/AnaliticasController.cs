using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teste.Entities;
using teste.LojaContext;

namespace teste.Controllers
{
    public class AnaliticasController : Controller
    {
        private readonly Context _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AnaliticasController(Context context, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Buscar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Buscar(Analitica analitica)
        {
            var user = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                analitica.UsuarioID = user.Id;

                // Redirecionar para a outra action passando as datas informadas pelo usuário
                return RedirectToAction("Analitica", "Banco", new { inicio = analitica.Inicio, final = analitica.Final });
            }
            return View(analitica);
        }

    }
}
