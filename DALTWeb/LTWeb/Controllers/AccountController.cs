using Microsoft.AspNetCore.Mvc;
using LTWeb.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LTWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var vm = new AccountViewModel();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel vm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Xóa bỏ các lỗi validation của phần Register khi đang thực hiện Login
            foreach (var key in ModelState.Keys)
            {
                if (key.StartsWith("Register"))
                {
                    ModelState[key].Errors.Clear();
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            }

            if (!TryValidateModel(vm.Login, nameof(vm.Login)))
            {
                return View(vm);
            }

            var login = vm.Login;
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountViewModel vm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Xóa bỏ các lỗi validation của phần Login khi đang thực hiện Register
            foreach (var key in ModelState.Keys)
            {
                if (key.StartsWith("Login"))
                {
                    ModelState[key].Errors.Clear();
                    ModelState[key].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }
            }

            if (!TryValidateModel(vm.Register, nameof(vm.Register)))
            {
                return View("Login", vm);
            }

            var reg = vm.Register;
            var user = new IdentityUser { UserName = reg.Email, Email = reg.Email };

            var result = await _userManager.CreateAsync(user, reg.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["RegisterSuccess"] = "Đăng ký thành công!";

                // Chuyển hướng về trang chủ hoặc trang MyAccount nếu bạn đã tạo
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            foreach (var error in result.Errors)
            {
                // Việt hóa một số lỗi phổ biến của Identity
                string desc = error.Description;
                if (error.Code == "DuplicateUserName") desc = "Email này đã được đăng ký.";

                ModelState.AddModelError(string.Empty, desc);
            }

            return View("Login", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}