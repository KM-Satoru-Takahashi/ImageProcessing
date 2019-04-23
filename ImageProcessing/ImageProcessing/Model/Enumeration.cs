using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Model
{
    /// <summary>
    /// 画像補間方法
    /// </summary>
    internal enum InterpolationStyle
    {
        /// <summary>
        /// 未選択、初期値
        /// </summary>
        None,

        /// <summary>
        /// 最近傍法
        /// </summary>
        NearestNeighbor,

        /// <summary>
        /// 線形補間法
        /// </summary>
        Bilinear
    }

    /// <summary>
    /// 回転方向
    /// </summary>
    internal enum RotateDirection
    {
        /// <summary>
        /// 未選択、初期値
        /// </summary>
        None,

        /// <summary>
        /// 右方向
        /// </summary>
        Right,

        /// <summary>
        /// 左方向
        /// </summary>
        Left
    }
}
