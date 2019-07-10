using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Models.Crypto;
using TaskManager.Models.DataBaseConnect.UserUtils;
using TaskManager.Models.Encoder;
using TaskManager.Models.ViewModels.ViewModelTask;
using static System.Net.Mime.MediaTypeNames;


namespace TaskManager.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationContext _context;
        IHostingEnvironment _appEnvironment;


        public ProfileController(ApplicationContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }


        public async Task<IActionResult> ProfileEdit()
        {
            long iduser;
            try
            {
                iduser = GetUserId();
            }
            catch
            {
                return RedirectToAction("Signin", "Account");
            }

            UserApp userApp = null;

            try
            {
                await Task.Run(() => {

                    userApp = new UserUtils().GetUserById(_context, iduser);

                });
            }
            catch (Exception)
            {
                return NotFound();
            }

            if (userApp != null)
            {
                Cryptographer cryptographer = new Cryptographer().Create(userApp.Upassword);
                string name = cryptographer.Decode(userApp.FirstName);
                string lastname = cryptographer.Decode(userApp.LastName);
                string email = cryptographer.Decode(userApp.Email);
                DateTime dateTime = userApp.DateApp;
                string imagePath;
                if (userApp.ImagePath == null)
                {
                    imagePath = "";
                }
                else
                {
                    imagePath = userApp.ImagePath;
                }



                UserViewModel user = new UserViewModel
                {
                    Id = userApp.Id,
                    Name = name.First().ToString().ToUpper() + String.Join("", name.Skip(1)),
                    LastName = lastname.First().ToString().ToUpper() + String.Join("", lastname.Skip(1)),
                    Email = email,
                    DateRegister = dateTime,
                    ImagePath = imagePath
                };
                return View(user);
            }
            return View();
        }


        public IActionResult Error(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        [HttpPost("Profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(long? id , IFormFile uploadedFile)
        {
            if (id == null)
            {    
                return NotFound();            
            }

            UserApp user = GetCurrentUserAsync();

            if (user == null)
            {            
               return NotFound();
            }
            if (user.Id == id)
            {
                if (uploadedFile != null)
                {
                    try
                    {
                        var files = HttpContext.Request.Form.Files;
                        foreach (var Image in files)
                        {
                            if (Image != null && Image.Length > 0)
                            {
                                var file = Image;

                                var uploads = Path.Combine(_appEnvironment.WebRootPath, "images");

                                if (file.Length > 0)
                                {
                                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                                    {
                                        await file.CopyToAsync(fileStream);
                                        user.ImagePath = fileName;
                                    }
                                }
                            }

                            _context.Update(user);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(ProfileEdit));
                        }
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("Error", "Profile", new { error = e.Message });
                    }
                }
            }
         
            
            return RedirectToAction(nameof(ProfileEdit));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> ProfileEdit([Bind("Id,Name, LastName, Email, DateRegister")] UserViewModel userViewModel)
        {

            long iduser;
            try
            {
                iduser = GetUserId();
            }
            catch
            {
                return RedirectToAction("Signin", "Account");
            }
            if (ModelState.IsValid)
            {
                UserApp userApp = null;
                UserApp temp = null;
                try
                {
                    await Task.Run(() => {
                        userApp = new UserUtils().GetUserById(_context, iduser);

                    });
                              
                }
                catch (Exception)
                {
                    return NotFound();
                }
                if (userApp != null)
                {
                    Cryptographer cryptographer = new Cryptographer().Create(userApp.Upassword);
                    string Email = cryptographer.Encode(userViewModel.Email.Trim().ToLower());

                    try
                    {
                         temp = await _context.UserApps.SingleOrDefaultAsync(u => u.Email == Email);
                    }
                    catch (Exception)
                    {
                        return NotFound();
                    }

                    if(userApp!=null && temp !=null)
                    {
                        if (userApp.Email == temp.Email)
                        {
                            userApp.FirstName = cryptographer.Encode(userViewModel.Name.Trim().ToLower());
                            userApp.LastName = cryptographer.Encode(userViewModel.LastName.Trim().ToLower());
                           
                            try
                            {
                                _context.Update(userApp);
                                await _context.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!UserAppExists(userApp.Id))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            return RedirectToAction(nameof(ProfileEdit));
                        }
                    }

                }
            }
            return View(userViewModel);
        }



        private UserApp GetCurrentUserAsync()
        {
            UserApp userApp = null;

            long iduser;
            try
            {
                iduser = GetUserId();
            }
            catch
            {
                throw;
            }
            try
            {
                userApp = _context.UserApps.SingleOrDefault(u => u.Id == iduser);
                
            }
            catch (Exception)
            {
                throw;
            }

            return userApp;
        }


        private bool UserAppExists(long id)
        {
            return _context.UserApps.Any(e => e.Id == id);
        }

        private long GetUserId()
        {
            string key = HttpContext.Session.GetString("key");

            string output = Regex.Match(key, @"\d+").Value;

            long iduser = Int64.Parse(output);

            return iduser;
        }

    }
}
