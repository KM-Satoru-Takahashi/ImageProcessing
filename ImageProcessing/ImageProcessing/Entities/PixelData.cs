using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Entities
{
    /// <summary>
    /// View上に表示されるPixelDataの保持クラス
    /// </summary>
    public class PixelData
    {

        /// <summary>
        /// マウスが指した画素のX座標
        /// </summary>
        public int XCoordinate
        {
            get;
            set;
        }

        /// <summary>
        /// マウスが指した画素のY座標
        /// </summary>
        public int YCoordinate
        {
            get;
            set;
        }

        /// <summary>
        /// 現在のR値
        /// </summary>
        public byte OldRed
        {
            get;
            set;
        }

        /// <summary>
        /// 現在のG値
        /// </summary>
        public byte OldGreen
        {
            get;
            set;
        }

        /// <summary>
        /// 現在のB値
        /// </summary>
        public byte OldBlue
        {
            get;
            set;
        }

        /// <summary>
        /// 現在のA値
        /// </summary>
        public byte OldAlpha
        {
            get;
            set;
        }

        /// <summary>
        /// 新しいR値
        /// </summary>
        public byte NewRed
        {
            get;
            set;
        }

        /// <summary>
        /// 新しいG値
        /// </summary>
        public byte NewGreen
        {
            get;
            set;
        }

        /// <summary>
        /// 新しいB値
        /// </summary>
        public byte NewBlue
        {
            get;
            set;
        }

        /// <summary>
        /// 新しいA値
        /// </summary>
        public byte NewAlpha
        {
            get;
            set;
        }

    }
}
