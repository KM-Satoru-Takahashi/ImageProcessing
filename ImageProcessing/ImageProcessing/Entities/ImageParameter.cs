using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Entities
{
    /// <summary>
    /// 画像処理に使用する各データを保存するエンティティ
    /// </summary>
    internal class ImageParameter
    {
        /// <summary>
        /// 現在表示されている画像高さ
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// 現在表示されている画像の幅
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// 1画素あたりのピクセル数
        /// </summary>
        public int Pixel { get; set; }

        /// <summary>
        /// 現在表示されている画像が反転されているか
        /// </summary>
        public bool IsFlipped { get; set; } = false;

        /// <summary>
        /// 現在表示されている画像のバイナリデータ
        /// </summary>
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// 水平解像度(dot/inch)
        /// </summary>
        public float HorizontalResolutionDPI { get; set; } = 0;

        /// <summary>
        /// BMP画像の垂直解像度(dot/inch)
        /// </summary>
        public float VerticalResolutionDPI { get; set; } = 0;
    }
}
