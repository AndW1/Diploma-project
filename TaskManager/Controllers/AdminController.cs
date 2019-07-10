using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SMTPClient;
using TaskManager.Models;
using TaskManager.Models.Admin;
using TaskManager.Models.Encoder;

namespace TaskManager.Controllers
{
    public class AdminController : Controller
    {

        private readonly ApplicationContext _context;

        public AdminController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            string key = HttpContext.Session.GetString("key");
            if (key != null)
            {
                int role = (int)HttpContext.Session.GetInt32("role");

                if (role != 2)
                {
                    return NotFound();             
                }
            }
            else
            {
                return RedirectToAction("Signin", "Account");
            }

            try
            {
              List<UserApp> listTmp = _context.UserApps.Select(u => u).ToList();
                List<ViewUserModel> user_list = new List<ViewUserModel>();

                for (int i = 0; i < listTmp.Count(); i++)
                {
                    Cryptographer cryptographer = new Cryptographer().Create(listTmp[i].Upassword);

                    string firstName = cryptographer.Decode(listTmp[i].FirstName);
                    string lastName = cryptographer.Decode(listTmp[i].LastName);

                    long count_task = _context.BoardTasks.Where(b => b.IdOwner == listTmp[i].Id).Count();


                    ViewUserModel userView = new ViewUserModel()
                    {
                        Id = listTmp[i].Id,
                        Name = firstName.First().ToString().ToUpper() + String.Join("", firstName.Skip(1)),
                        LastName = lastName.First().ToString().ToUpper() + String.Join("", firstName.Skip(1)),
                        DateRegister = listTmp[i].DateApp,
                       CountTask = count_task,
                        Image = listTmp[i].ImagePath
                    };

                    user_list.Add(userView);
                }
                return View(user_list);
            }
            catch (Exception)
            {
                throw new Exception();
            }      
        }    

        [HttpPost]
        public async Task<JsonResult> SendEmailAsync(string textEmail, long? id)
        {
            try
            {
                var user =  await _context.UserApps.SingleOrDefaultAsync(u => u.Id == id);

                Cryptographer cryptographer = new Cryptographer().Create(user.Upassword);

                string email = cryptographer.Decode(user.Email);

                EmailService emailService = new EmailService();

                await emailService.SendEmailAsync(email, "Info", textEmail);

                var empty = new { };
                return Json(empty);
            }
            catch (Exception)
            {
                throw new Exception();
            }   
        }


        [HttpPost]
        public async Task<JsonResult>FilterSearchAsync(int number)
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false);
            }

            List<ViewUserModel> users_list;
            List<UserApp> listData;
            string json;

            try
            {
                switch (number)
                {
                    case 1:
                        listData = await _context.UserApps.OrderByDescending(u => u.DateApp).ToListAsync();
                        users_list = DecodeData(listData);
                        json = JsonConvert.SerializeObject(users_list);
                        return Json(json); // возвращаем json
                    case 2:                  
                         listData = await _context.UserApps.ToListAsync();
                         users_list = DecodeData(listData);
                         users_list =  users_list.OrderByDescending(u => u.CountTask).ToList();
                        json = JsonConvert.SerializeObject(users_list);
                        return Json(json); // возвращаем json

                    case 3:
                        listData = await _context.UserApps.ToListAsync();
                        users_list = DecodeData(listData);
                        json = JsonConvert.SerializeObject(users_list);
                        return Json(json); // возвращаем json
                    default:
                        var empty = new { };
                        return Json(empty);
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }   
        }


        private List<ViewUserModel> DecodeData(List<UserApp> list)
        {
            List<ViewUserModel> user_list = new List<ViewUserModel>();

            for (int i = 0; i < list.Count(); i++)
            {
                Cryptographer cryptographer = new Cryptographer().Create(list[i].Upassword);

                string firstName = cryptographer.Decode(list[i].FirstName);
                string lastName = cryptographer.Decode(list[i].LastName);

                long count_task = _context.BoardTasks.Where(b => b.IdOwner == list[i].Id).Count();


                ViewUserModel userView = new ViewUserModel()
                {
                    Id = list[i].Id,
                    Name = firstName.First().ToString().ToUpper() + String.Join("", firstName.Skip(1)),
                    LastName = lastName.First().ToString().ToUpper() + String.Join("", firstName.Skip(1)),
                    DateRegister = list[i].DateApp,
                    CountTask = count_task,
                    Image = list[i].ImagePath
                };

                user_list.Add(userView);
            }

            return user_list;
        }

        private bool CheckUser()
        {
            long iduser;
            try
            {
                iduser = GetUserId();
                return true;
            }
            catch
            {
                return false;
            }
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