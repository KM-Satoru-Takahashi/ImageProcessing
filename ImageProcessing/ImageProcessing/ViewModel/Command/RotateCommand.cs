﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

namespace ImageProcessing.ViewModel.Command
{
    /// <summary>
    /// 右回転ボタン押下時に生じるコマンド
    /// </summary>
    public class RotateCommand : ICommand
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">ボタン押下時関数</param>
        /// <param name="canExecute">ボタン押下可否判定関数</param>
        public RotateCommand(Action<object> execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// ボタン押下時実行関数
        /// </summary>
        private Action<object> execute;

        /// <summary>
        /// ボタン使用可否判定
        /// </summary>
        private Func<bool> canExecute;

        /// <summary>
        /// イベントハンドラの登録と解除
        /// </summary>
        public event EventHandler CanExecuteChanged
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

        /// <summary>
        /// ボタン使用可否判定
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <remarks>VM側で実体を定義</remarks>
        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        /// <summary>
        /// ボタン押下時実行関数
        /// </summary>
        /// <param name="parameter"></param>
        /// <remarks>VM側で実体を定義</remarks>
        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
