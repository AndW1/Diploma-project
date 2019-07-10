using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TaskManager.Models;
using TaskManager.Models.DataBaseConnect.GeneralDbActions;
using TaskManager.Models.DataBaseConnect.StageUtils;

namespace TaskManager.Controllers
{
    public class NodeStagesController : Controller
    {
        private readonly ApplicationContext _context;

        public NodeStagesController(ApplicationContext context)
        {
            _context = context;
        }  

        [Route("/NodeSrages/Index",
        Name = "nodeindex")]
        public async Task<IActionResult> Index(long? Id, long? IdOwner, string name )//Show collection Nodes for one StageTask
        {

          bool userExists =  CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }

            if (Id==null || IdOwner==null || name == null)
            {
                return NotFound();
            }

            StageTask stage = null;

            try
            {            
                await Task.Run(() =>
                {
                    stage = new StageUtils().GetOneStage(_context, (long)Id);
                });
            }
            catch
            {
                return NotFound();
            }

            if (stage == null)
            {
                return NotFound();
            }

            if(name.Length < 6)
            {
                return NotFound();
            }
          
            string firstFiveChar = new string(name.Take(5).ToArray());

            if ("Stage".CompareTo(firstFiveChar) != 0)
            {
                return NotFound();
            }
            

            string numbers = new string(name.Skip(5).ToArray());

            long number;
            try
            {
                number = Int64.Parse(numbers);

            }
            catch (FormatException)
            {
                return NotFound();
            }

            int count_stage;
            try
            {
                count_stage = _context.StageTasks.Where(s => s.IdOwner == IdOwner).Count();
            }
            catch (Exception)
            {
                return NotFound();
            }

          
            if (number > count_stage)
            {
                return NotFound();
            }


            int sizePage = 10;

            int count;
            try
            {
                count = _context.NodeStages.Where(n => n.IdOwner == Id).Count();
            }
            catch (Exception)
            {
                return NotFound();
            }
            int totalPages;
            int lastPageSize;
            try
            {
                 totalPages = (int)Math.Ceiling((count / (double)sizePage));

                 lastPageSize = count % sizePage;
            }
            catch (Exception)
            {
                return NotFound();
            }
         
            ViewBag.NameStage = name;
            ViewBag.IdOwner = IdOwner;
            ViewBag.IdStage = Id;

            string nameTask = null ;
            try
            {
                nameTask = await _context.BoardTasks.Where(t => t.Id == IdOwner).Select(t => t.NameTask).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            if (nameTask == null)
            {
                return NotFound();
            }
          

            ViewBag.NameTask = nameTask;

            ViewBag.TotalPages = totalPages;

            var list_node = await _context.NodeStages.Where(n=>n.IdOwner==Id).Skip((1 - 1) * sizePage).Take(sizePage).ToListAsync();

            return View(list_node);
        }

     
        public async Task<JsonResult> PaginationJsonAsync(int pagenumber, long Id) // Id -> Stage Id
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false); 
            }
            int sizePage = 10;
            try
            {
                int count = _context.NodeStages.Where(n => n.IdOwner == Id).Count();

                int totalPages = (int)Math.Ceiling(count / (double)sizePage);

                int lastPageSize = count % sizePage;

                if (pagenumber == totalPages && lastPageSize != 0)
                {

                    var items = await _context.NodeStages.Where(n => n.IdOwner == Id).Skip((pagenumber - 1) * sizePage).Take(lastPageSize).ToListAsync();
                    string json = JsonConvert.SerializeObject(items);
                    return Json(json); // возвращаем json

                }

                var itemsfull = await _context.NodeStages.Where(n => n.IdOwner == Id).Skip((pagenumber - 1) * sizePage).Take(sizePage).ToListAsync();

                string jsonf = JsonConvert.SerializeObject(itemsfull);
                return Json(jsonf); // возвращаем json
            }
            catch(Exception) {
                throw new Exception();
            }     
        }

        [HttpPost]
        public async Task<JsonResult> CreateNodAsync(long idOwner, string content)//Create Node
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false);
            }

            DateTime now = DateTime.Now;

            NodeStage node = new NodeStage { IdOwner = idOwner, ContentNode = content, DateCreated = now, DateFinished = now };

            try
            {
                await Task.Run(() =>
                {
                    new GeneralActions().AddNewObject(_context, node);

                });

                string json = JsonConvert.SerializeObject(node);
                return Json(json); // возвращаем json
            }
            catch {
                throw new Exception();
            }   
        }

        [HttpPost]
        public async Task<JsonResult> DeleteNodeAsync(long idNode)//Remove Node
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false);
            }

            try
            {
                var node = await _context.NodeStages.SingleOrDefaultAsync(n => n.Id == idNode);

                _context.NodeStages.Remove(node);
                await _context.SaveChangesAsync();

                var empty = new { };
                return Json(empty);
            }
            catch (Exception)
            {
                throw new Exception();
            }
            
        }


        [HttpPost]
        public async Task<JsonResult> SetLineAsync(long idNode)//Set Node.Created=true
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false);
            }

            try
            {
                var node = await _context.NodeStages.SingleOrDefaultAsync(n => n.Id == idNode);

                node.NodeCreated = true;

                _context.Update(node);

                await _context.SaveChangesAsync();

                var empty = new { };
                return Json(empty);
            }
            catch (Exception)
            {
                throw new Exception();
            }    
        }


        [HttpPost]
        public async Task<JsonResult> RemoveLineAsync(long idNode)//Set Node.Created=false
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false);
            }

            try
            {
                var node = await _context.NodeStages.SingleOrDefaultAsync(n => n.Id == idNode);

                node.NodeCreated = false;

                _context.Update(node);

                await _context.SaveChangesAsync();

                var aw = new { };
                return Json(aw);
            }
            catch (Exception)
            {
                throw new Exception();
            }      
        }

        //[HttpGet]
        public async Task<JsonResult> GetTotalPageAsync(long idOwner)//Get total pages
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return Json(false);
            }

            try
            {
                int sizePage = 10;

                var count = await _context.NodeStages.Where(n => n.IdOwner == idOwner).CountAsync();
                int totalPages = (int)Math.Ceiling(count / (double)sizePage);
                return Json(totalPages);
            }
            catch (Exception)
            {
                throw new Exception();
            }      
        }


        private bool NodeStageExists(long id)
        {
            return _context.NodeStages.Any(e => e.Id == id);
        }

        private  bool CheckUser()
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
