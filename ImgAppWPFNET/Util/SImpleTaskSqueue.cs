using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImgAppWPFNET.Util
{
    class SimpleTasksQueue
    {
        private List<Task> tasks = new List<Task>();
        private Task _generalTask;
        public  Task GeneralTask => _generalTask;
        private bool _continueWork = true;
        public bool ContinueWork { get => _continueWork; set  => _continueWork = value; }

        private Task _currentTask;
        public Task CurentTask => _currentTask;

        private void mainThread()
        {
            Console.WriteLine("Started GT with id " + Thread.CurrentThread.ManagedThreadId);
            while (ContinueWork)
            {
                if (tasks.Count > 0)
                {
                    var t = tasks.Last();
                    var i = tasks.IndexOf(t);

                    _currentTask = t;
                    t.RunSynchronously();
                   
                    _currentTask = null;

                    tasks.RemoveRange(0, i + 1);
                }

                if (tasks.Count == 0)
                    _generalTask.Wait(1);

            }
        }

        public void StartQueue()
        {
            _generalTask = new Task(mainThread);
            _generalTask.Start();
            
        }

        public void WaitGeneral()
        {
            _generalTask.Wait();
        }

        public void Wait()
        {
            //return;
            
            
            while (tasks.Count != 0)
            {
                Thread.Sleep(1);
                Console.WriteLine("Wait for " + Thread.CurrentThread.ManagedThreadId + "" + GeneralTask.Status);
            }
        }

        public void AddTask(Task t)
        {
            tasks.Add(t);
        }

    }
}
