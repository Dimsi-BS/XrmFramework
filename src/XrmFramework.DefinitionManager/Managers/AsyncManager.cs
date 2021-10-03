// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinitionManager
{
    abstract class AsyncManager : IDisposable
    {

        public delegate void StepChangedEventHandler(object sender, StepChangedEventArgs e);
        public event StepChangedEventHandler StepChanged;

        public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
        public event ProgressChangedEventHandler ProgressChanged;

        private Queue<AsyncAction> _queue = new();

        protected BackgroundWorker _worker;

        public AsyncManager()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.ProgressChanged += _worker_ProgressChanged;
        }

        void _worker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
            {
                var progress = (Progress)e.UserState;
                ProgressChanged(this, new ProgressChangedEventArgs
                {
                    Current = progress.Current,
                    Max = progress.Max
                });
            }
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var action = (AsyncAction)e.Result;

            if (_queue.Count > 0)
            {
                _worker.RunWorkerAsync(_queue.Dequeue());
            }

            action.Callback(action.Result);
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var action = (AsyncAction)e.Argument;

            action.Result = action.Action(action.Argument);

            e.Result = action;
        }

        protected void Run(Func<object, object> action, Action<object> callback, object argument)
        {
            var asyncAction = new AsyncAction
            {
                Action = action,
                Callback = callback,
                Argument = argument
            };

            if (_worker.IsBusy || _queue.Count > 0)
            {
                _queue.Enqueue(asyncAction);
            }
            else
            {
                _worker.RunWorkerAsync(asyncAction);
            }
        }

        protected void Run()
        {
            _worker.RunWorkerAsync();
        }

        protected void SendStepChange(string stepName, int current = 0, int max = 0)
        {
            if (StepChanged != null)
            {
                StepChanged(this, new StepChangedEventArgs
                {
                    StepName = stepName,
                    Current = current,
                    Maximum = max
                });
            }
        }

        protected class Progress
        {
            public int Current { get; set; }
            public int Max { get; set; }
        }

        public class ProgressChangedEventArgs
        {
            public int Current { get; set; }
            public int Max { get; set; }
        }

        protected class AsyncAction
        {
            public Func<object, object> Action { get; set; }

            public Action<object> Callback { get; set; }

            public object Argument { get; set; }

            public object Result { get; set; }
        }

        public void Dispose()
        {
            if (_worker != null)
            {
                _worker.Dispose();
            }
        }
    }
}
