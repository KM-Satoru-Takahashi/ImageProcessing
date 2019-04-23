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
    internal class BackgroundChangeRegionViewManager
    {
        /// <summary>
        /// Info(Label)表示文言
        /// </summary>
        public string BackgroundChangeMessage { get; } = "クリックで表示領域の背景色を変更できます";

        /// <summary>
        /// カラーコード入力領域表示メッセージ
        /// </summary>
        public string InputColorcodeMessage { get; } = "カラーコードの入力も可能です";

        /// <summary>
        /// 入力されたカラーコード
        /// </summary>
        public string InputColorcode
        {
            get;
            set;
        }

        /// <summary>
        /// 入力されたカラーコードに対応する背景色
        /// </summary>
        /// <remarks>初期値は白としておく</remarks>
        public Brush InputColorcodeColor
        {
            get;
            set;
        } = new SolidColorBrush(Colors.White);

        /// <summary>
        /// 背景色変更ボタン表示文言
        /// </summary>
        public string BackgroundChangeButtonMessage { get; } = "変更";

        /// <summary>
        /// 背景色変更ボタン押下可否
        /// </summary>
        public bool IsBackChangeEnabled { get; set; } = true;

        /// <summary>
        /// 黒色ボタンに表示する文言(カラーコード)
        /// </summary>
        public string BlackColorcode { get; } = "000000";

        /// <summary>
        /// 黒色ボタンの背景色
        /// </summary>
        public Brush BlackColor { get; } = new SolidColorBrush(Colors.Black);

        /// <summary>
        /// 灰色ボタンに表示する文言
        /// </summary>
        public string GrayColorcode { get; } = "808080";

        /// <summary>
        /// 灰色ボタンの背景色
        /// </summary>
        public Brush GrayColor { get; } = new SolidColorBrush(Colors.Gray);

        /// <summary>
        /// 薄灰ボタンに表示する文言
        /// </summary>
        public string LightgrayColorcode { get; } = "D3D3D3";

        /// <summary>
        /// 薄灰ボタンの背景色
        /// </summary>
        public Brush LightgrayColor { get; } = new SolidColorBrush(Colors.LightGray);

        /// <summary>
        /// 白ボタンに表示する文言
        /// </summary>
        public string WhiteColorcode { get; } = "FFFFFF";

        /// <summary>
        /// 白ボタンの背景色
        /// </summary>
        public Brush WhiteColor { get; } = new SolidColorBrush(Colors.White);
    }
}
