using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Model
{
    /// <summary>
    /// 画処理領域に表示する文言を管理する
    /// </summary>
    /// <remarks>Background="Aquamarine"のグリッド部分</remarks>
    internal class ImageProcessingRegionViewManager
    {
        /// <summary>
        /// 右回転ボタン表示文言
        /// </summary>
        public string RightRotate90 { get; } = "90度右回転";

        /// <summary>
        /// 回転ボタン使用可否
        /// </summary>
        public bool IsRotateButtonEnabled { get; set; } = true;

        /// <summary>
        /// 左回転ボタン表示文言
        /// </summary>
        public string LeftRotate90 { get; } = "90度左回転";

        /// <summary>
        /// 反転ボタン
        /// </summary>
        public string Flip { get; } = "反転";

        /// <summary>
        /// 反転ボタン押下可否
        /// </summary>
        public bool IsFlipButtonEnabled { get; set; } = true;

        /// <summary>
        /// 最近傍法での画像拡縮ボタン表示文言
        /// </summary>
        public string NearestNeighborButtonMessage { get; } = "最近傍法";

        /// <summary>
        /// 線形補間法での画像拡縮ボタン表示文言
        /// </summary>
        public string BilinearButtonMessage { get; } = "線形補間法";

        /// <summary>
        /// 幅入力欄表示文言
        /// </summary>
        public string WidthPercent { get; } = "幅(%)";

        /// <summary>
        /// 高さ入力欄表示文言
        /// </summary>
        public string HeightPercent { get; } = "高さ(%)";

        /// <summary>
        /// 拡縮ボタン押下可否
        /// </summary>
        public bool IsScalingButtonEnabled { get; set; } = true;
    }
}
