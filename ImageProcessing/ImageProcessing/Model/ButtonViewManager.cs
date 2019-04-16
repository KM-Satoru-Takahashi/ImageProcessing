﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Model
{
    /// <summary>
    /// MainWidonwViewに表示するボタンの状態を管理する
    /// </summary>
    internal class ButtonViewManager
    {
        /// <summary>
        /// 右回転ボタン表示文言
        /// </summary>
        private readonly string _rightRotate90 = "90度右回転";

        /// <summary>
        /// 右回転ボタン表示文言
        /// </summary>
        public string RightRotate90
        {
            get
            {
                return _rightRotate90;
            }
        }

        /// <summary>
        /// 右回転ボタン使用可否
        /// </summary>
        private bool _isRightButtonEnabled = true;

        /// <summary>
        /// 右回転ボタン使用可否
        /// </summary>
        public bool IsRightRotateButtonEnabled
        {
            get
            {
                return _isRightButtonEnabled;
            }
            set
            {
                _isRightButtonEnabled = value;
            }
        }

        /// <summary>
        /// 左回転ボタン
        /// </summary>
        private readonly string _leftRotate90 = "90度左回転";

        /// <summary>
        /// 左回転ボタン
        /// </summary>
        public string LeftRotate90
        {
            get
            {
                return _leftRotate90;
            }
        }

        /// <summary>
        /// 左回転ボタン使用可否
        /// </summary>
        private bool _isLeftButtonEnabled = true;

        /// <summary>
        /// 左回転ボタン使用可否
        /// </summary>
        public bool IsLeftRotateButtonEnabled
        {
            get
            {
                return _isLeftButtonEnabled;
            }
            set
            {
                _isLeftButtonEnabled = value;
            }
        }
    }
}
