using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageProcessing.Model
{
    /// <summary>
    /// 背景色変更部分の管理クラス
    /// </summary>
    internal class BackgroundChangeViewManager
    {
        /// <summary>
        /// Info表示文言
        /// </summary>
        private readonly string BACKGROUND_CHANGE_MESSAGE = "クリックで表示領域の背景色を変更できます";

        /// <summary>
        /// Info(Label)表示文言
        /// </summary>
        public string BackgroundChangeMessage
        {
            get
            {
                return BACKGROUND_CHANGE_MESSAGE;
            }
        }


        private readonly string INPUT_COLORCODE_MESSAGE = "カラーコードの入力も可能です";

        public string InputColorcodeMessage
        {
            get
            {
                return INPUT_COLORCODE_MESSAGE;
            }
        }

        /// <summary>
        /// 背景色変更ボタン表示文言
        /// </summary>
        private readonly string BACKGROUND_CHANGE_BUTTON_MESSAGE = "変更";

        /// <summary>
        /// 背景色変更ボタン表示文言
        /// </summary>
        public string BackgroundChangeButtonMessage
        {
            get
            {
                return BACKGROUND_CHANGE_BUTTON_MESSAGE;
            }
        }

        /// <summary>
        /// 背景色変更ボタン押下可否
        /// </summary>
        private bool _isBackChangeEnabled = true;

        /// <summary>
        /// 背景色変更ボタン押下可否
        /// </summary>
        public bool IsBackChangeEnabled
        {
            get
            {
                return _isBackChangeEnabled;
            }
            set
            {
                _isBackChangeEnabled = value;
            }
        }

        #region 色ボタン

        #region 黒

        /// <summary>
        /// 黒色ボタンに表示する文言(カラーコード)
        /// </summary>
        private readonly string BLACK_COLORCODE = "000000";

        /// <summary>
        /// 黒色ボタンに表示する文言(カラーコード)
        /// </summary>
        public string BlackColorcode
        {
            get
            {
                return BLACK_COLORCODE;
            }
        }

        /// <summary>
        /// 黒色ボタンの背景色(黒)
        /// </summary>
        private readonly Brush BLACK_COLOR = new SolidColorBrush(Colors.Black);

        /// <summary>
        /// 黒色ボタンの背景色
        /// </summary>
        public Brush BlackColor
        {
            get
            {
                return BLACK_COLOR;
            }
        }

        #endregion

        #region 灰

        private readonly string GRAY_COLORCODE = "808080";

        public string GrayColorcode
        {
            get
            {
                return GRAY_COLORCODE;
            }
        }

        private readonly Brush GRAY_COLOR = new SolidColorBrush(Colors.Gray);

        public Brush GrayColor
        {
            get
            {
                return GRAY_COLOR;
            }
        }

        #endregion

        #region 薄灰

        private readonly string LIGHTGRAY_COLORCODE = "D3D3D3";

        public string LightgrayColorcode
        {
            get
            {
                return LIGHTGRAY_COLORCODE;
            }
        }

        private readonly Brush LIGHTGRAY_COLOR = new SolidColorBrush(Colors.LightGray);

        public Brush LightgrayColor
        {
            get
            {
                return LIGHTGRAY_COLOR;
            }
        }

        #endregion

        #region 白

        private readonly string WHITE_COLORCODE = "FFFFFF";

        public string WhiteColorcode
        {
            get
            {
                return WHITE_COLORCODE;
            }
        }

        private readonly Brush WHITE_COLOR = new SolidColorBrush(Colors.White);

        public Brush WhiteColor
        {
            get
            {
                return WHITE_COLOR;
            }
        }

        #endregion

        #endregion
    }
}
