using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager.Models.DataBaseConnect.StageUtils
{
    public class StageUtils : IStageQuery
    {
        public StageTask GetOneStage(ApplicationContext applicationContext, long stageId)
        {
            try
            {
              return applicationContext.StageTasks.SingleOrDefault(s => s.Id == stageId);
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }

        }

        public IEnumerable<StageTask> GetStagesForTask(ApplicationContext applicationContext, long taskId)
        {
            try
            {
                return applicationContext.StageTasks.Include(d => d.IdOwnerNavigation).Where(s => s.IdOwner == taskId).Select(s => s).ToList();
            }
            catch (DbUpdateException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
