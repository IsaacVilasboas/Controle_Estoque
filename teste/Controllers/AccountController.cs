using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using teste.Models;

namespace teste.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Usuario,
                    Email = model.Usuario
                };

                var result = await userManager.CreateAsync(user, model.Senha);
                
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    string errorMessage = error.Description switch
                    {
                        "Passwords must be at least 6 characters." => "As senhas devem ter pelo menos 6 caracteres.",
                        "Passwords must have at least one non alphanumeric character." => "As senhas devem conter pelo menos um caractere não alfanumérico.",
                        "Passwords must have at least one digit ('0'-'9')." => "As senhas devem conter pelo menos um dígito ('0'-'9').",
                        "Passwords must have at least one uppercase ('A'-'Z')." => "As senhas devem conter pelo menos uma letra maiúscula ('A'-'Z').",
                        "Passwords must have at least one lowercase ('a'-'z')." => "As senhas devem conter pelo menos uma letra minúscula ('a'-'z').",
                        "Email 'Usuario' is invalid." => "O Usuario é inválido.",
                       
                        _ => error.Description 
                    };

                    ModelState.AddModelError(string.Empty, errorMessage);
                }
            }
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Login()
        { 
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Usuario, model.Senha, model.RememberMe, false);
                
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Login ou Senha Inválido");
            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/Account/AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
