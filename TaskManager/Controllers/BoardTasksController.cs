using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models;
using TaskManager.Models.DataBaseConnect.BoardTaskUtils;
using TaskManager.Models.DataBaseConnect.GeneralDbActions;
using TaskManager.Models.DataBaseConnect.StageUtils;
using TaskManager.Models.DataBaseConnect.UserUtils;
using TaskManager.Models.Encoder;
using TaskManager.Models.ViewModels.ViewModelStage;
using TaskManager.Models.ViewModels.ViewModelTask;

namespace TaskManager.Controllers
{
    public class BoardTasksController : Controller
    {
        private readonly ApplicationContext _context;

        public BoardTasksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BoardTasks
        public async Task<IActionResult> Index()
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

                    userApp = new UserUtils().GetUserById(_context,iduser);

                });
            }
            catch (Exception ){
                return NotFound();
            }

            if(userApp!=null)
            {
                Cryptographer cryptographer = new Cryptographer().Create(userApp.Upassword);
                string name = cryptographer.Decode(userApp.FirstName);
                string email = cryptographer.Decode(userApp.Email);

                UserViewModel user = new UserViewModel();

                user.Id = userApp.Id;

                user.Name = name.First().ToString().ToUpper() + String.Join("", name.Skip(1));
                user.Email = email;
                user.ImagePath = userApp.ImagePath;

                IEnumerable<BoardTask> usertasks = null;

                try
                {
                    await Task.Run(() =>
                    {
                        usertasks = new BoardTaskUtils().GetUsersBoardTask(_context, userApp.Id);
                    });
                }
                catch (Exception ) {
                    return NotFound();
                }

                UserTaskViewModel userTaskView = new UserTaskViewModel { UserView = user, BoardTasks = usertasks };

                return View(userTaskView);

            }
            else
            {
                return NotFound();
            }        
        }

        // GET: BoardTasks/Details/5
        public async Task<IActionResult> Details(long? id) //Show one BoardTask and collection StageTasks
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            BoardTask boardTask = null;
            try
            {
                await Task.Run(() =>
                {
                    boardTask = new BoardTaskUtils().GetBoardTask(_context, (long)id);
                });
            }
            catch (Exception ){
                return NotFound();
            }

            if (boardTask != null)
            {
                BoardViewModel board = new BoardViewModel();
                board.Content = boardTask.ContentTask;
                board.Name = boardTask.NameTask;
                board.Id = boardTask.Id;
                board.DateCreated = boardTask.DateCreated;
                board.DateFinished = boardTask.DateFinished;


                IEnumerable<StageTask> taskstages = null;

                try
                {
                    await Task.Run(() =>
                    {
                        taskstages = new StageUtils().GetStagesForTask(_context, boardTask.Id);
                    });
                }
                catch (Exception ) {
                    return NotFound();
                }

                BoardStageViewModel boardStageView = new BoardStageViewModel { BoardView = board, StageTasks = taskstages };

                return View(boardStageView);
            }
            else
            {
                return NotFound();
            }

        }

        // GET: BoardTasks/Create
        public IActionResult Create()
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            return View();
        }

        // POST: BoardTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NameTask,ContentTask,DateFinished")] BoardTask boardTask, string date_create)//Create BoardTask
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            // boardTask.DateCreated = ConvertStringToDateTime(date_create);

            double localDate = Convert.ToDouble(date_create);          
            boardTask.DateCreated = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(localDate);


            if ((boardTask.DateCreated.CompareTo(boardTask.DateFinished))>0)
            {
                ModelState.AddModelError("DateFinished", "Дата окончния меньше текущей");
            }
            if (ModelState.IsValid)
            { 
                boardTask.IdOwner = GetUserId();

                BoardTask newBoard = new BoardTask
                {
                    IdOwner = boardTask.IdOwner,
                    ContentTask = boardTask.ContentTask,
                    NameTask = boardTask.NameTask,
                    DateCreated = boardTask.DateCreated,
                    DateFinished = boardTask.DateFinished
                };

                try
                {
                    await Task.Run(() =>
                    {
                        new GeneralActions().AddNewObject(_context, newBoard);
                    });

                }
                catch (Exception){
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
   
            return View(boardTask);
        }

        // GET: BoardTasks/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }

            BoardTask boardTask = null;

            try {
                await Task.Run(() =>
                {
                    boardTask = new BoardTaskUtils().GetBoardTask(_context, (long)id);
                });
            }
            catch (Exception)
            {
                return NotFound();
            }

            if (boardTask == null)
            {
                return NotFound();
            }
            ViewData["IdOwner"] = boardTask.IdOwner; 
            return View(boardTask);
        }

        // POST: BoardTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
     public async Task<IActionResult> Edit(long id, [Bind("Id,IdOwner,NameTask,ContentTask,DateCreated,DateFinished,BackgroundImage,TaskCreated")] BoardTask boardTask)//Edit BoardTask
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }

            if (id != boardTask.Id)
            {
                return NotFound();
            }

            if ((boardTask.DateCreated.CompareTo(boardTask.DateFinished)) > 0)
            {
                ModelState.AddModelError("DateFinished", "Дата окончния меньше текущей");
            }


            if (ModelState.IsValid)
            {
                try
                {
                   await Task.Run(() => {
                        new GeneralActions().UpdateObject(_context, boardTask);
                    });        
                }
                catch (Exception)
                {
                    if (!BoardTaskExists(boardTask.Id))
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
            ViewData["IdOwner"] = boardTask.IdOwner;
            return View(boardTask);
        }


        // POST: BoardTasks/EditBoardView/6
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBoardView( [FromForm] BoardViewModel boardViewModel)
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            if ((boardViewModel.DateCreated.CompareTo(boardViewModel.DateFinished)) > 0)
            {
                ModelState.AddModelError("DateFinished", "Дата окончния меньше текущей");           
            }

            if (ModelState.IsValid)
            {
                try
                {
                     var boardtask = new BoardTaskUtils().GetBoardTask(_context, boardViewModel.Id);

                      boardtask.ContentTask = boardViewModel.Content;
                      boardtask.DateFinished = boardViewModel.DateFinished;

                    await Task.Run(() => {
                        new GeneralActions().UpdateObject(_context, boardtask);
                    });
                }
                catch (Exception)
                {
                    if (!BoardTaskExists(boardViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "BoardTasks", new { id = boardViewModel.Id });
            }

            return RedirectToAction("Details", "BoardTasks", new { id = boardViewModel.Id });
        }

        // GET: BoardTasks/Delete/5
        public async Task<IActionResult> Delete(long? id)//View BoardTask delete page
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }

            BoardTask boardTask = null;
            try
            {
                await Task.Run(() => {

                    boardTask = new BoardTaskUtils().GetBoardTask(_context, (long)id);
                });
            }
            catch (Exception){
                return NotFound();
            }

            if (boardTask == null)
            {
                return NotFound();
            }

            return View(boardTask);
        }

        // POST: BoardTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id) // Remove BoardTask
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            try
            {
             var boardTask = await _context.BoardTasks.SingleOrDefaultAsync(m => m.Id == id);

                _context.BoardTasks.Remove(boardTask);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return NotFound();
            }     
        }

       


        private bool BoardTaskExists(long id)
        {
            return _context.BoardTasks.Any(e => e.Id == id);
        }



        #region ALL METHODS FOR STAGE MODEL

        // GET: StageTasks/Create
        public IActionResult CreateStage(long? id)//View create Stage page
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                int count_stage = _context.StageTasks.Where(s => s.IdOwner == id).Count();
                ViewData["Number"] = count_stage+1;
            }
            catch (Exception)
            {
                return NotFound();
            }

            ViewData["IdOwner"] = id; 
            return View();
        }

        // POST: BoardTasks/CreateStage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStage([Bind("IdOwner,ContentStage,DateFinished")] StageTask stageTask, string date_create) //CreateStage       
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }


            double localDate = Convert.ToDouble(date_create);
            stageTask.DateCreated = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(localDate);

            // stageTask.DateCreated = ConvertStringToDateTime(date_create);
            //AddSeconds(localDate);

            BoardTask boardTask = null;

            try
            {
                await Task.Run(() =>
                {
                    boardTask = new BoardTaskUtils().GetBoardTask(_context,stageTask.IdOwner);

                });
            }
            catch (Exception)
            {
                return NotFound();
            }
            if (boardTask == null)
            {
                return NotFound();
            }


            if ((boardTask.DateFinished.CompareTo(stageTask.DateFinished)) < 0)
            {
                ModelState.AddModelError("DateFinished", "Дата окончния не может быть позже даты окончния задачи");
            }


            if ((stageTask.DateCreated.CompareTo(stageTask.DateFinished)) > 0)
            {
                ModelState.AddModelError("DateFinished", "Дата окончния меньше текущей");
            }

            if (ModelState.IsValid)
            {  
                stageTask.NameStage = "Stage";

                try
                {
                    await Task.Run(() =>
                    {
                        new GeneralActions().AddNewObject(_context, stageTask);
                    });
                    return RedirectToAction("Details", "BoardTasks", new { id = stageTask.IdOwner });
                }
                catch (Exception){
                    return NotFound();
                }
            }
            ViewData["IdOwner"] = stageTask.IdOwner;
            return View(stageTask);
        }

       [HttpGet]
        public async Task<IActionResult> DeleteStage(long? Id, long? IdOwner)//Delete Stage
        {
            bool userExists = CheckUser();
            if (!userExists)
            {
                return RedirectToAction("Signin", "Account");
            }
            if (Id == null || IdOwner == null)
            {
                return NotFound();
            }
            try
            {
                StageTask stageTask;

                await Task.Run(() =>
                {
                    stageTask = new StageUtils().GetOneStage(_context, (long)Id);
                    _context.StageTasks.Remove(stageTask);
                });

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = IdOwner });
            }
            catch{
                return NotFound();
            }

        }


        #endregion


        private DateTime ConvertStringToDateTime(string date_create)
        {
            string formatString = "yyyy MMM ddd HH:mm:ss";

            DateTime now = new DateTime();

            var gmtIndex = date_create.IndexOf(" GMT");
            if (gmtIndex > -1)
            {
                date_create = date_create.Remove(gmtIndex);
                now = DateTime.ParseExact(date_create, formatString, null);
            }
            else
            {
                now = DateTime.Parse(date_create);
            }

            return now;
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
