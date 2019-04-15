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
        /// <remarks>for debugging</remarks>
        //internal DropData()
        //{ }

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
        /// <remarks>[0]~[1], ビッグエンディアン, 0x42_0x4d->BM</remarks>
        private byte[] _dataFormat = new Byte[2];

        /// <summary>
        /// BMPデータフォーマット部分
        /// </summary>
        public byte[] DataFormat
        {
            get
            {
                return _dataFormat;
            }
        }

        /// <summary>
        /// ファイルサイズ(byte)
        /// </summary>
        /// <remarks>[2]~[5]</remarks>
        private byte[] _fileSize = new byte[4];

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public byte[] FileSize
        {
            get
            {
                return _fileSize;
            }
        }

        /// <summary>
        /// 予約領域1
        /// </summary>
        /// <remarks>[6]~[7], 常に0</remarks>
        private byte[] _reservedRegion1 = new byte[2];

        /// <summary>
        /// 予約領域1
        /// </summary>
        public byte[] ReservedRegion1
        {
            get
            {
                return _reservedRegion1;
            }
        }

        /// <summary>
        /// 予約領域2
        /// </summary>
        /// <remarks>[8]~[9], 常に0</remarks>
        private byte[] _reservedRegion2 = new byte[2];

        /// <summary>
        /// 予約領域2
        /// </summary>
        public byte[] ReservedRegion2
        {
            get
            {
                return _reservedRegion2;
            }
        }

        /// <summary>
        /// ヘッダサイズ
        /// </summary>
        /// <remarks>[10]~[13], データ部の先頭位置の目印になる</remarks>
        private byte[] _headerSize = new byte[4];

        /// <summary>
        /// ヘッダサイズ
        /// </summary>
        public byte[] HeaderSize
        {
            get
            {
                return _headerSize;
            }
        }

        /// <summary>
        /// 情報ヘッダ(BITMAP_INFOHEADER)サイズ
        /// </summary>
        /// <remarks>[14]~[17], 常に40byte</remarks>
        private byte[] _infoHeaderSize = new byte[4];

        /// <summary>
        /// 情報ヘッダサイズ
        /// </summary>
        public byte[] InfoHeaderSize
        {
            get
            {
                return _infoHeaderSize;
            }
        }

        /// <summary>
        /// 画像幅
        /// </summary>
        /// <remarks>[18]~[21], pixel(px)</remarks>
        private byte[] _imageWidth = new byte[4];

        /// <summary>
        /// 画像幅
        /// </summary>
        public byte[] ImageWidth
        {
            get
            {
                return _imageWidth;
            }
        }

        /// <summary>
        /// 画像高さ
        /// </summary>
        /// <remarks>[22]~[25], pixel(px)</remarks>
        private byte[] _imageHeight = new byte[4];

        /// <summary>
        /// 画像高さ
        /// </summary>
        public byte[] ImageHeight
        {
            get
            {
                return _imageHeight;
            }
        }

        /// <summary>
        /// プレーン数
        /// </summary>
        /// <remarks>[26]~[27], 常に0(01_00)</remarks>
        private byte[] _plane = new byte[2];

        /// <summary>
        /// プレーン数
        /// </summary>
        public byte[] Plane
        {
            get
            {
                return _plane;
            }
        }

        /// <summary>
        /// 1画素の色数
        /// </summary>
        /// <remarks>[28]~[29], RGBなら3色*1byte=24bit, グレースケールなら1色*1byte</remarks>
        private byte[] _pixel = new byte[2];

        /// <summary>
        /// 1画素の色数
        /// </summary>
        public byte[] Pixel
        {
            get
            {
                return _pixel;
            }
        }

        /// <summary>
        /// 圧縮形式
        /// </summary>
        /// <remarks>[30]~[33]</remarks>
        private byte[] _compressionStyle = new byte[4];

        /// <summary>
        /// 圧縮形式
        /// </summary>
        public byte[] CompressionStyle
        {
            get
            {
                return _compressionStyle;
            }
        }

        /// <summary>
        /// 圧縮サイズ(byte)
        /// </summary>
        /// <remarks>[30]~[33]</remarks>
        private byte[] _compressionSize = new byte[4];

        /// <summary>
        /// 圧縮サイズ(byte)
        /// </summary>
        public byte[] CompressionSize
        {
            get
            {
                return _compressionSize;
            }
        }

        // (DPI*1000)/25.4=DPM
        // 1インチ=25.4mmのため

        /// <summary>
        /// 水平解像度(dot/m)
        /// </summary>
        /// <remarks>[38]~[41]</remarks>
        private byte[] _horizontalResolutionDPM = new byte[4];

        /// <summary>
        /// 水平解像度(dot/m)
        /// </summary>
        public byte[] HorizontalResolutionDPM
        {
            get
            {
                return _horizontalResolutionDPM;
            }
        }

        /// <summary>
        /// 垂直解像度(dot/m)
        /// </summary>
        /// <remarks>[42]~[45]</remarks>
        private byte[] _verticalResolutionDPM = new byte[4];

        /// <summary>
        /// 垂直解像度(dot/m)
        /// </summary>
        public byte[] VerticalResolutionDPM
        {
            get
            {
                return _verticalResolutionDPM;
            }
        }

        /// <summary>
        /// 色数
        /// </summary>
        /// <remarks>[46]~[49], 0の場合は全色を使用</remarks>
        private byte[] _color = new byte[4];

        /// <summary>
        /// 色数
        /// </summary>
        public byte[] Color
        {
            get
            {
                return _color;
            }
        }

        /// <summary>
        /// 重要色数
        /// </summary>
        /// <remarks>[50]~[53], 0の場合は全色を使用</remarks>
        private byte[] _importantColor = new byte[4];

        /// <summary>
        /// 重要色数
        /// </summary>
        public byte[] ImportantColor
        {
            get
            {
                return _importantColor;
            }
        }

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
        /// BMP画像から取得した水平解像度(dot/inch)
        /// </summary>
        /// <remarks>BMPファイルのバイナリデータにある解像度はDPM</remarks>
        private float _horizontalResolutionDPI = 0;

        /// <summary>
        /// 水平解像度(dot/inch)
        /// </summary>
        public float HorizontalResolutionDPI
        {
            get
            {
                return _horizontalResolutionDPI;
            }
            set
            {
                _horizontalResolutionDPI = value;
            }
        }


        /// <summary>
        /// BMP画像から取得した垂直解像度(dot/inch)
        /// </summary>
        /// <remarks>BMPファイルのバイナリデータにある解像度はDPM</remarks>
        private float _verticalResolutionDPI = 0;

        /// <summary>
        /// BMP画像の垂直解像度(dot/inch)
        /// </summary>
        public float VerticalResolutionDPI
        {
            get
            {
                return _verticalResolutionDPI;
            }
            set
            {
                _verticalResolutionDPI = value;
            }
        }

    }
}
