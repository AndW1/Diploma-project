using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.StageUtils
{
    public interface IStageQuery
    {
        IEnumerable<StageTask> GetStagesForTask(ApplicationContext applicationContext, long taskId);
        StageTask GetOneStage(ApplicationContext applicationContext, long taskId);
    }
}
