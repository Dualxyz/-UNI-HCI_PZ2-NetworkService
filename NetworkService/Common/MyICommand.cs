using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkService.Common
{
    public class MyICommand : ICommand, ICommandUndo
    {
        readonly Action _TargetExecuteMethod;
        readonly Func<bool> _TargetCanExecuteMethod;

        public MyICommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod();
                if (_TargetUnExecuteMethod != null)
                {
                    StaticData.myICommands.Push(this);
                }
            }
        }

        readonly Action _TargetUnExecuteMethod;
        public MyICommand(Action executeMethod, Action unExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetUnExecuteMethod = unExecuteMethod;
        }

        public void UnExecute()
        {
            _TargetUnExecuteMethod?.Invoke();
        }
    }

    public class MyICommand<T> : ICommand, ICommandUndo
    {
        readonly Action<T> _TargetExecuteMethod;
        readonly Func<T, bool> _TargetCanExecuteMethod;

        public MyICommand(Action<T> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_TargetCanExecuteMethod != null)
            {
                T tparm = (T)parameter;
                return _TargetCanExecuteMethod(tparm);
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod((T)parameter);
                if (_TargetUnExecuteMethod != null)
                {
                    StaticData.myICommands.Push(this);
                }
            }
        }

        readonly Action _TargetUnExecuteMethod;
        public MyICommand(Action<T> executeMethod, Action unExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetUnExecuteMethod = unExecuteMethod;
        }

        public void UnExecute()
        {
            _TargetUnExecuteMethod?.Invoke();
        }
    }

    public class MyICommand<T1, T2> : ICommand, ICommandUndo
    {
        readonly Action<T1, T2> _TargetExecuteMethod;
        readonly Func<T1, T2, bool> _TargetCanExecuteMethod;

        public MyICommand(Action<T1, T2> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public MyICommand(Action<T1, T2> executeMethod, Func<T1, T2, bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (_TargetCanExecuteMethod != null)
            {
                T1 tparm1 = (T1)parameter;
                T2 tparm2 = (T2)parameter;
                return _TargetCanExecuteMethod(tparm1, tparm2);
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }
            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            if (_TargetExecuteMethod != null)
            {
                _TargetExecuteMethod((T1)parameter, (T2)parameter);
                if (_TargetUnExecuteMethod != null)
                {
                    StaticData.myICommands.Push(this);
                }
            }
        }

        readonly Action _TargetUnExecuteMethod;
        public MyICommand(Action<T1, T2> executeMethod, Action unExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetUnExecuteMethod = unExecuteMethod;
        }

        public void UnExecute()
        {
            _TargetUnExecuteMethod?.Invoke();
        }
    }
}
