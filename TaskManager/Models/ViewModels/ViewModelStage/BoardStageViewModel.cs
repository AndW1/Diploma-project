using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.ViewModels.ViewModelStage
{
    public class BoardStageViewModel
    {
        public BoardViewModel BoardView { get; set; }
        public IEnumerable<StageTask> StageTasks { get; set; }
    }
}
