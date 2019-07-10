using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Encoder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using SMTPClient;
using TaskManager.Models;
using TaskManager.Models.AccountValidation;
using TaskManager.Models.Crypto;
using TaskManager.Models.DataBaseConnect.GeneralDbActions;
using TaskManager.Models.DataBaseConnect.UserUtils;
using TaskManager.Models.Encoder;

namespace TaskManager.Controllers
{
    public class AccountController : Controller
    {
       
        private readonly ApplicationContext _context;

        private const string ERROR = "error";

        public AccountController(ApplicationContext context)
        {
            _context = context;
        }


        public IActionResult Signin() // View Signin form
        {
            string key = HttpContext.Session.GetString("key");

            if (key != null)
            {
                return RedirectToAction("Index", "BoardTasks");
            }
     
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(LoginForm loginForm) // Sign in
        {
            HttpContext.Session.Remove(ERROR);

            if (ModelState.IsValid)
            {
                //TO DO
                // Send email and login to Java server
                // if exists in Java server  => select from MS SQL DATABASE
                // if not exists in MS SQL => save new UserApp
                //else = > redirect to Register form

                SomeData someData = null;

                try
                {
                    string someEmail = new HashConvertor().GetHash(loginForm.Email.Trim().ToLower());
                    someData = await _context.SomeDatas.LastOrDefaultAsync(sd => sd.Data1 == someEmail);
                }
                catch (Exception)
                {
                    return NotFound();
                }

                if (someData == null)
                {
                    return RedirectToAction("Register", "Account");
                }

                Cryptographer cryptographer = new Cryptographer().Create(someData.Data2);

                UserApp userApp = null;

                try
                {                
                    await Task.Run(() =>
                    {
                        string email = cryptographer.Encode(loginForm.Email.Trim().ToLower());
                        string password = new HashConvertor().GetHash(loginForm.Password.Trim().ToLower());

                        userApp = new UserUtils().GetUser(_context, email, password);
                    });
                }
                catch (Exception)
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("Error", "Account");
                }

                if (userApp != null)
                {
                    if (userApp.EmailConfirmed)
                    {
                        int role;

                        try
                        {
                            role = _context.UserRoles.Where(r => r.Id == userApp.IdRole).Select(r => r.IdRole).First();
                        }
                        catch (Exception)
                        {
                            HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                            return RedirectToAction("Error", "Account");
                        }
                        SetUserSession(userApp, role, cryptographer);

                        if (role == 2)
                        {
                            return RedirectToAction("Index", "Admin");
                        }


                        return RedirectToAction("Index", "BoardTasks");
                    }
                    else
                    {
                        return RedirectToAction("Confirm", "Account", new { id = userApp.Id });
                    }
                }
                         
                return RedirectToAction("Register", "Account");            
            }

            return View();
        }

        
        public IActionResult Register() // View Register form
        {
            string key = HttpContext.Session.GetString("key");

            if (key != null)
            {
                return RedirectToAction("Index", "BoardTasks");
            }
        
            return View();
        }

        [HttpPost]    
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,LastName,Email,Password,Confirm")] RegisterForm registerForm, string date_register) // Register
        {
            HttpContext.Session.Remove(ERROR);
 
            if (ModelState.IsValid)
            {
               
                string passwordHash = new HashConvertor().GetHash(registerForm.Password.Trim().ToLower());
                string emailHash= new HashConvertor().GetHash(registerForm.Email.Trim().ToLower());

                SomeData someData = null;

                try
                {
                    someData = await _context.SomeDatas.LastOrDefaultAsync(sd => sd.Data1 == emailHash);
                   
                }
                catch (Exception)
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("Error", "Account");
                }

                if (someData!=null)
                {
                    return RedirectToAction("Signin", "Account");
                }

                Cryptographer cryptographer = new Cryptographer().Create(passwordHash);

                string emailEncode = cryptographer.Encode(registerForm.Email.Trim().ToLower());

                UserApp userApp = null;

                try
                {
                    await Task.Run(() =>
                    {                     
                        userApp = new UserUtils().GetUser(_context, emailEncode, passwordHash);
                    });
                }
                catch (Exception )
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("RegisterError", "Account");
                }

                if (userApp != null)
                    {
                        return RedirectToAction("Signin", "Account");
                    }
                    else
                    {

                    EmailResponce goodEmail = await new EmailService().ChekEmaileService(registerForm.Email.Trim().ToLower());

                    switch (goodEmail.Success)
                    {
                        case 1:

                            string name = cryptographer.Encode( registerForm.Name.Trim().ToLower());
                            string lastname = cryptographer.Encode(registerForm.LastName.Trim().ToLower());
                            string email = emailEncode;
                            string password = passwordHash;


                          

                            double localDate = Convert.ToDouble(date_register);
                            registerForm.Time = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(localDate);


                            try
                            {
                               _context.Add(new SomeData {
                                   Data1 = emailHash,
                                   Data2 = passwordHash
                               });
                                await _context.SaveChangesAsync();

                                await Task.Run(() =>
                                {
                                    userApp = new UserUtils().RegisterNewUser(_context, name, lastname, email, password, registerForm.Time);
                                });
                            }
                            catch (Exception)
                            {
                                HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                                return RedirectToAction("RegisterError", "Account");
                            }

                            return RedirectToAction("Confirm", "Account", new { id = userApp.Id });
                        case -1:
                            HttpContext.Session.SetString(ERROR, "Mail is not correct.");
                            return RedirectToAction("RegisterError", "Account");

                    }
                }             
            }
            return View();
        }



        [AllowAnonymous]
        public async Task<IActionResult> Confirm(long? id) // View Confirm page
        {      
            if(id == null)
            {
                return NotFound(); 
            }

            UserApp userApp = null;
            try
                {
                    await Task.Run(() =>
                    {
                        userApp = new UserUtils().GetUserById(_context, (long)id);
                    });
                }
                catch (Exception)
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("RegisterError", "Account");
                }


            if (userApp != null)
            {
                Cryptographer cryptographer = new Cryptographer().Create(userApp.Upassword);

                string decodeEmail = cryptographer.Decode(userApp.Email);
              

                EmailResponce goodEmail = await new EmailService().ChekEmaileService(decodeEmail);

                  switch (goodEmail.Success)
                            {
                                case 1:

                                       EmailService emailService = new EmailService();

                                    var callbackUrl = Url.Action(
                                                "ConfirmEmail",
                                                "Account",
                                               new { Token = userApp.Id, Email = userApp.Email },
                                                protocol: HttpContext.Request.Scheme);


                                      await emailService.SendEmailAsync(decodeEmail, "Confirm email", $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>Confirm email</a>");


                        ViewBag.Name = goodEmail.EmailData.Item1;
                        ViewBag.Href = goodEmail.EmailData.Item2;

                        return View();
                     
                                case -1:
                                    HttpContext.Session.SetString(ERROR, "Mail is not correct.");
                                    return RedirectToAction("RegisterError", "Account");

                            }

            }
          
                return NotFound();        
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string Token, string Email) // Confirm Email
        {
            if (Token == null || Email == null)
            {
                return NotFound();
            }

            long idUser;
            try
            {
                idUser = Int64.Parse(Token);
            }
            catch
            {
                return NotFound();
            }

            UserApp userApp = null;
            try
            {
                await Task.Run(() =>
                {
                    userApp = new UserUtils().GetUserByEmail(_context, idUser, Email);
                });
            }
            catch (Exception)
            {
                HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                return RedirectToAction("RegisterError", "Account");
            }


            if (userApp != null)
            {
                userApp.EmailConfirmed = true;

                try
                {
                    await Task.Run(() =>
                    {
                        new GeneralActions().UpdateObject(_context, userApp);
                    });
                }
                catch (Exception )
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("RegisterError", "Account");
                }

                int role;

                try
                {
                    role = _context.UserRoles.Where(r => r.Id == userApp.IdRole).Select(r => r.IdRole).First();
                }
                catch (Exception )
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("RegisterError", "Account");
                }

                Cryptographer cryptographer = new Cryptographer().Create(userApp.Upassword);
                SetUserSession(userApp, role, cryptographer);

                return RedirectToAction("Index", "BoardTasks");
            }
            else
            {
                return NotFound();
            }

        }
     

        public IActionResult ChangePassword() // View Change password form
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword) // Change password
        {
            if (ModelState.IsValid)
            { 
                SomeData someData = null;

                try
                {
                    string someEmail = new HashConvertor().GetHash(changePassword.Email.Trim().ToLower());
                    someData =  await _context.SomeDatas.LastOrDefaultAsync(sd => sd.Data1 == someEmail);
                }
                catch (Exception)
                {
                    return NotFound();
                }

                if (someData == null)
                {
                    return RedirectToAction("Register", "Account");
                }

                Cryptographer cryptographer = new Cryptographer().Create(someData.Data2);
                
                string email = cryptographer.Encode(changePassword.Email.Trim().ToLower());
       
                string passwordHash = new HashConvertor().GetHash(changePassword.Password.Trim().ToLower());

                bool responce = false;

                try
                {
                    await Task.Run(() =>
                    {
                        responce = new UserUtils().CheckEmailExists(_context, email);
                    });
                }
                catch (Exception)
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("Error", "Account");
                }

                if (!responce)
                {
                    return RedirectToAction("Register", "Account");
                }

                UserApp userApp = null;
                try
                {
                    userApp = await _context.UserApps.SingleOrDefaultAsync(u => u.Email == email);

                    if (userApp != null)
                    {
                        userApp = cryptographer.ConvertUserData(userApp, passwordHash);

                        try
                        {
                            _context.Update(userApp);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            return NotFound();
                        }
                       
                        await Task.Run(() =>
                        {
                            userApp = new UserUtils().UpdatePassword(_context, userApp.Email, passwordHash);
                        });

                        string emailHash = new HashConvertor().GetHash(changePassword.Email.Trim().ToLower());

                        _context.Add(new SomeData
                        {
                            Data1 = emailHash,
                            Data2 = passwordHash
                        });
                        await _context.SaveChangesAsync();
                    }           
                }
                catch (Exception )
                {
                    HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later.");
                    return RedirectToAction("Error", "Account");
                }
               
                if (userApp != null)
                {
                    int role;

                    try
                    {
                        role = _context.UserRoles.Where(r => r.Id == userApp.IdRole).Select(r => r.IdRole).First();
                    }
                    catch (Exception)
                    {
                        HttpContext.Session.SetString(ERROR, "The server was not found or was not accessible. Try later. I am ");
                        return RedirectToAction("Error", "Account");
                    }

                    SetUserSession(userApp, role, cryptographer);

                    return RedirectToAction("Index", "BoardTasks");
                }
            }

            return View();
        }


        public IActionResult Error()
        {
            ViewBag.Exception = HttpContext.Session.GetString(ERROR);
            return View();
        }

        public IActionResult RegisterError()
        {
            ViewBag.Exception = HttpContext.Session.GetString(ERROR);
            return View();
        }

        public IActionResult Logout() // Logout
        {
            try
            {
                string userId = HttpContext.Session.GetString("key");
                HttpContext.Session.Remove(userId);
                HttpContext.Session.Remove("key");
                HttpContext.Session.Remove("role");


                foreach (var cookieKey in HttpContext.Request.Cookies.Keys)
                {
                    //var option = new CookieOptions();
                    //option.Expires = DateTime.Now.AddMinutes(10);
                    //Response.Cookies.Append("Emailoption", "true", option);
                   HttpContext.Response.Cookies.Delete(cookieKey);
                }

                HttpContext.Session.Clear();

                 return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }


        private  void SetUserSession(UserApp userApp, int role, Cryptographer cryptographer) // Session settings
        {
            string userId = "userId" + userApp.Id.ToString();
            HttpContext.Session.SetString("key", userId);
            HttpContext.Session.SetInt32(userId, (int)userApp.Id);
            HttpContext.Session.SetInt32("role", role);

            string user_name = cryptographer.Decode(userApp.FirstName);
            HttpContext.Session.SetString("name", user_name);
        }
    }
}