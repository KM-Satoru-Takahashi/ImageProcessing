using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Model
{
    /// <summary>
    /// クリック箇所の画素表示部分表示文言を管理する
    /// </summary>
    /// <remarks>Background="Turquoise"のグリッド部分</remarks>
    internal class PixelDataDisplayRegionViewManager
    {
        /// <summary>
        /// 更新ボタン文言"更新"
        /// </summary>
        public string UpdateButtonMessage { get; } = "更新";

        /// <summary>
        /// 文言"X座標"
        /// </summary>
        public string XPosition { get; } = "X座標";

        /// <summary>
        /// 文言"Y座標"
        /// </summary>
        public string YPosition { get; } = "Y座標";

        /// <summary>
        /// 文言"現在の画素値"
        /// </summary>
        public string OldPixel { get; } = "現在の画素値";

        /// <summary>
        /// 文言"新しい画素値"
        /// </summary>
        public string NewPixel { get; } = "新しい画素値";

        /// <summary>
        /// 文言"R"
        /// </summary>
        public string RedLabel { get; } = "R";

        /// <summary>
        /// 文言"G"
        /// </summary>
        public string GreenLabel { get; } = "G";

        /// <summary>
        /// 文言"B"
        /// </summary>
        public string BlueLabel { get; } = "B";

        /// <summary>
        /// 文言"A"
        /// </summary>
        public string AlphaLabel { get; } = "A";
    }
}
