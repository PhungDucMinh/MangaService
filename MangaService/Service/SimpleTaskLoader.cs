using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MangaService.Service
{
    class SimpleTaskLoader<Parameter, TResult>
    {
        public async Task<List<TResult>> DoListTask(List<Parameter> parameters, Func<Parameter, Task<TResult>> function)
        {
            List<TResult> resultList = new List<TResult>();
            List<Task<TResult>> taskList = new List<Task<TResult>>();
            foreach (var parameter in parameters)
            {
                taskList.Add(function(parameter));
            }

            while (taskList.Count > 0)
            {

                var completedTask = await Task.WhenAny(taskList.ToArray());
                taskList.Remove(completedTask);
                resultList.Add(completedTask.Result);
            }
            return resultList;
        }
    }
}