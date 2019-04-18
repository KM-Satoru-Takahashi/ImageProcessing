using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;


namespace ImageProcessing.ViewModel
{
    /// <summary>
    /// VMの基底クラス
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo, IDisposable
    {
        #region INotifyPropertyChanged

        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChangedEventの発生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IDataErrorInfo

        /// <summary>
        /// エラーメッセージ保存用辞書
        /// </summary>
        private Dictionary<string, string> _errorMessages = new Dictionary<string, string>();

        /// <summary>
        /// エラー発生時表示文言
        /// </summary>
        private const string ERROR_MESSAGE = "HasError";

        /// <summary>
        /// IDataErrorInfo.Errorの実装
        /// </summary>
        string IDataErrorInfo.Error
        {
            get
            {
                if (_errorMessages.Count > 0)
                {
                    return ERROR_MESSAGE;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// IDataErrorInfo.Itemの実装
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (_errorMessages.ContainsKey(columnName))
                {
                    return _errorMessages[columnName];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// エラーメッセージをセットする
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        protected void SetError(string propertyName, string errorMessage)
        {
            _errorMessages[propertyName] = errorMessage;
        }

        /// <summary>
        /// エラーメッセージをクリアする
        /// </summary>
        /// <param name="propertyName">クリア対象のプロパティ</param>
        protected void ClearError(string propertyName)
        {
            if (_errorMessages.ContainsKey(propertyName))
            {
                _errorMessages.Remove(propertyName);
            }
        }

        #endregion

        #region ICommand Helper

        #region DelegateCommand

        /// <summary>
        /// ICommandを実装する内部クラス
        /// </summary>
        private class DelegateCommand : ICommand
        {
            /// <summary>
            /// コマンド本体
            /// </summary>
            private Action<object> _command;

            /// <summary>
            /// 実行可否
            /// </summary>
            private Func<object, bool> _canExecute;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="command">コマンド</param>
            /// <param name="canExecute">実行可否</param>
            public DelegateCommand(Action<object> command, Func<object, bool> canExecute)
            {
                if (command == null)
                {
                    throw new ArgumentNullException();
                }

                _command = command;
                _canExecute = canExecute;
            }

            /// <summary>
            /// ICommand.Execute
            /// </summary>
            /// <param name="parameter"></param>
            void ICommand.Execute(object parameter)
            {
                _command(parameter);
            }

            /// <summary>
            /// ICommand.CanExecute
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            bool ICommand.CanExecute(object parameter)
            {
                if (_canExecute != null)
                {
                    return _canExecute(parameter);
                }
                else
                {
                    return true;
                }
            }

            /// <summary>
            /// ICommand.CanExecuteChanged
            /// </summary>
            event EventHandler ICommand.CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

        }

        #endregion

        /// <summary>
        /// コマンドの生成
        /// </summary>
        /// <param name="command">コマンド</param>
        /// <param name="canExecute">実行可否</param>
        /// <returns></returns>
        protected ICommand CreateCommand(Action<object> command, Func<object, bool> canExecute)
        {
            return new DelegateCommand(command, canExecute);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 開放処理フラグ
        /// </summary>
        protected bool isDisposed = false;

        /// <summary>
        /// SafeHandle
        /// </summary>
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// 開放処理
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 開放処理
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed == true)
            {
                return;
            }

            if (disposing == true)
            {
                handle.Dispose();

                // 他マネージドの開放処理をここで行う
            }

        }


        #endregion
    }
}
