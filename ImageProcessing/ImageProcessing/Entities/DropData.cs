using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Entities
{
    /// <summary>
    /// ドロップされたファイルを内部に保持するエンティティ
    /// </summary>
    public class DropData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DropData(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// ドロップされたファイルパス
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// 画像ファイル情報
        /// </summary>
        public WriteableBitmap ImageData
        {
            get;
            set;
        }

        /// <summary>
        /// BMPデータフォーマット部分
        /// </summary>
        public byte[] DataFormat { get; } = new byte[2];

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public byte[] FileSize { get; } = new byte[4];

        /// <summary>
        /// 予約領域1
        /// </summary>
        public byte[] ReservedRegion1 { get; } = new byte[2];

        /// <summary>
        /// 予約領域2
        /// </summary>
        public byte[] ReservedRegion2 { get; } = new byte[2];

        /// <summary>
        /// ヘッダサイズ
        /// </summary>
        public byte[] HeaderSize { get; } = new byte[4];

        /// <summary>
        /// 情報ヘッダサイズ
        /// </summary>
        public byte[] InfoHeaderSize { get; } = new byte[4];

        /// <summary>
        /// 画像幅
        /// </summary>
        public byte[] ImageWidth { get; } = new byte[4];

        /// <summary>
        /// 画像高さ
        /// </summary>
        public byte[] ImageHeight { get; } = new byte[4];

        /// <summary>
        /// プレーン数
        /// </summary>
        public byte[] Plane { get; } = new byte[2];

        /// <summary>
        /// 1画素の色数
        /// </summary>
        public byte[] Pixel { get; } = new byte[2];

        /// <summary>
        /// 圧縮形式
        /// </summary>
        public byte[] CompressionStyle { get; } = new byte[4];

        /// <summary>
        /// 圧縮サイズ(byte)
        /// </summary>
        public byte[] CompressionSize { get; } = new byte[4];

        /// <summary>
        /// 水平解像度(dot/m)
        /// </summary>
        public byte[] HorizontalResolutionDPM { get; } = new byte[4];

        /// <summary>
        /// 垂直解像度(dot/m)
        /// </summary>
        public byte[] VerticalResolutionDPM { get; } = new byte[4];

        /// <summary>
        /// 色数
        /// </summary>
        public byte[] Color { get; } = new byte[4];

        /// <summary>
        /// 重要色数
        /// </summary>
        public byte[] ImportantColor { get; } = new byte[4];

        /// <summary>
        /// データ部
        /// </summary>
        /// <remarks>[55]~, 作成時には長さが不明なので使用時にnewする</remarks>
        public byte[] Data
        {
            get;
            set;
        }

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
