using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Model
{
    /// <summary>
    /// クリック箇所の画素表示部分の状態を管理する
    /// </summary>
    /// <remarks>Background="Turquoise"のグリッド部分</remarks>
    internal class PixelDataViewManager
    {
        /// <summary>
        /// 文言"X座標"
        /// </summary>
        private readonly string XPOS = "X座標";

        /// <summary>
        /// 文言"X座標"
        /// </summary>
        public string XPosition
        {
            get
            {
                return XPOS;
            }
        }

        /// <summary>
        /// 文言"Y座標"
        /// </summary>
        private readonly string YPOS = "Y座標";

        /// <summary>
        /// 文言"Y座標"
        /// </summary>
        public string YPosition
        {
            get
            {
                return YPOS;
            }
        }

        /// <summary>
        /// 文言"現在の画素値"
        /// </summary>
        private readonly string OLD_PIXEL = "現在の画素値";

        /// <summary>
        /// 文言"現在の画素値"
        /// </summary>
        public string OldPixel
        {
            get
            {
                return OLD_PIXEL;
            }
        }


        /// <summary>
        /// 文言"新しい画素値"
        /// </summary>
        private readonly string NEW_PIXEL = "新しい画素値";

        /// <summary>
        /// 文言"新しい画素値"
        /// </summary>
        public string NewPixel
        {
            get
            {
                return NEW_PIXEL;
            }
        }

        /// <summary>
        /// 文言"R"
        /// </summary>
        private readonly string RED = "R";

        /// <summary>
        /// 文言"R"
        /// </summary>
        public string Red
        {
            get
            {
                return RED;
            }
        }

        /// <summary>
        /// 文言"G"
        /// </summary>
        private readonly string GREEN = "G";

        /// <summary>
        /// 文言"G"
        /// </summary>
        public string Green
        {
            get
            {
                return GREEN;
            }
        }

        /// <summary>
        /// 文言"B"
        /// </summary>
        private readonly string BLUE = "B";

        /// <summary>
        /// 文言"B"
        /// </summary>
        public string Blue
        {
            get
            {
                return BLUE;
            }
        }

        /// <summary>
        /// 文言"A"
        /// </summary>
        private readonly string ALPHA = "A";

        /// <summary>
        /// 文言"A"
        /// </summary>
        public string Alpha
        {
            get
            {
                return ALPHA;
            }
        }

        /// <summary>
        /// 文言"更新"
        /// </summary>
        private readonly string UPDATE = "更新";

        /// <summary>
        /// 文言"更新"
        /// </summary>
        public string Update
        {
            get
            {
                return UPDATE;
            }
        }


    }
}
