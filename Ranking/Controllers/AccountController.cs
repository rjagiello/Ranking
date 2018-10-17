using Ranking.DAL;
using Ranking.Infrastructure;
using Ranking.Models;
using Ranking.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ranking.Controllers
{
    public class AccountController : Controller
    {
        private RankContext db;
        private MatchManager matchManager;
        private SessionManager sessionManager { get; set; }
        private AccountManager accountManager;

        public AccountController()
        {
            db = new RankContext();
            sessionManager = new SessionManager();
            accountManager = new AccountManager(db, sessionManager);
            matchManager = new MatchManager(db);
        }

        public ActionResult RegisterIndex()
        {
            return View();
        }

        public ActionResult LoginIndex()
        {
            return View();
        }

        public ActionResult Login()
        {
            ViewBag.UserList = GetUserList();
            accountManager.SetUserType(0);

            return View(new LoginViewModel() { userType = UserType.Team });
        }

        public ActionResult LoginFan()
        {
            accountManager.SetUserType(UserType.Fan);

            return View(new LoginViewModel() { userType = UserType.Fan });
        }

        public ActionResult LoginAdmin()
        {
            ViewBag.UserList = GetUserList(true);
            accountManager.SetUserType(UserType.Admin);

            return View(new LoginViewModel() { userType = UserType.Admin });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, UserType type)
        {
            accountManager.SetUserType(type);
            if (ModelState.IsValid)
            {
                var sha256 = Crypto.SHA256(model.Password);
                var user = new LoginViewModel() { Name = model.Name, Password = model.Password };
                if (accountManager.Login(user))
                {
                    accountManager.SetLoginState(true);
                    accountManager.Authentication(accountManager.GetUser(user));
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("loginerror", "Błędne hasło lub nazwa");
            }
            if (accountManager.GetUserType() == UserType.Admin)
            {
                ViewBag.UserList = GetUserList(true);
                return View("LoginAdmin", model);
            }
            else if (accountManager.GetUserType() == UserType.Fan)
            {
                return View("LoginFan", model);
            }
            else if (accountManager.GetUserType() == UserType.Team)
            {
                ViewBag.UserList = GetUserList();
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sha256 = Crypto.SHA256(model.Password);

                var user = new Users() { Password = sha256, Name = model.Name, Captain = model.Captain, Email = model.Email };

                bool notValid = false;

                if (accountManager.IsDuplicateEmail(model.Email))
                {
                    notValid = true;
                    ModelState.AddModelError("", "Użytkownik z podanym adresem e-mail istnieje");
                }
                if (accountManager.IsDuplicateName(model.Name))
                {
                    notValid = true;
                    ModelState.AddModelError("", "Drużyna o podanej nazwie istnieje");
                }
                if (!notValid)
                {
                    accountManager.AddUser(user);
                    accountManager.AddCaptainToMembers(user.Name, user.Captain);
                    accountManager.SetLoginState(true);
                    accountManager.Authentication(user);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult RegisterFan()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterFan(RegisterFanViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sha256 = Crypto.SHA256(model.Password);

                string ipAddress = Request.UserHostAddress;

                var user = new Fans() { Password = sha256, Name = model.Name, IpAddress = ipAddress, Email = model.Email };

                bool notValid = false;

                if (accountManager.IsDuplicateEmail(model.Email, true))
                {
                    notValid = true;
                    ModelState.AddModelError("", "Użytkownik z podanym adresem e-mail istnieje");
                }
                if (accountManager.IsDuplicateName(model.Name, false, true))
                {
                    notValid = true;
                    ModelState.AddModelError("", "Użytkownik o podanej nazwie istnieje");
                }
                if (accountManager.IsDuplicateIp(ipAddress))
                {
                    notValid = true;
                    ModelState.AddModelError("", "Posiadasz już konto kibica");
                }
                if (!notValid)
                {
                    accountManager.AddUser(user);
                    accountManager.SetLoginState(true);
                    accountManager.Authentication(user);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            accountManager.SetLoginState(false);
            accountManager.Authentication(null);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword(string token)
        {
            var user = db.Users.Where(u => u.ResetPasswordToken == token).SingleOrDefault();

            if(user !=null)
                return RedirectToAction("Index", "Home");

            accountManager.DeleteResetPasswordToken(user.UserId);
            accountManager.SetLoginState(true);
            accountManager.Authentication(user);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var sha256 = Crypto.SHA256(model.Password);

                var name = Helpers.UserName();

                var user = db.Users.Where(u => u.Name == name).Single();

                user.Password = sha256;
                user.ForgotPassword = false;
                db.SaveChanges();

                accountManager.DeleteResetPasswordToken(user.UserId);
                
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        public ActionResult ForgotPasswordFlag(bool flag = false)
        {
            ViewBag.Flag = flag;
            ViewBag.UserList = GetUserList();

            return View();
        }

        [HttpPost]
        public ActionResult ForgotPasswordFlag(Users model)
        {
            var user = db.Users.Where(u => u.Name == model.Name).SingleOrDefault();
            bool notValid = false;

            if(user.Email != model.Email)
            {
                notValid = true;
                ModelState.AddModelError("error", "Niepoprawny adres E-mail");
            }
            if(!notValid)
            {
                user.ForgotPassword = true;
                db.SaveChanges();
                return RedirectToAction("ForgotPasswordFlag", new { flag = true });
            }
            ViewBag.Flag = false;
            ViewBag.UserList = GetUserList();

            return View();
        }

        private SelectList GetUserList(bool admin = false)
        {
            List<Users> users = db.Users.Where(u => u.IsAdmin == admin).ToList();
            users.Reverse();

            return new SelectList(users, "Name", "Name");
        }
    }
}