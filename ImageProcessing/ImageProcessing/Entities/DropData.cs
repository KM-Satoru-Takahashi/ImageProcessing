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
        internal DropData()
        { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>For Debugging</remarks>
        internal DropData(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath">ドロップされたファイルのパス</param>
        /// <param name="imageData">対象ファイル</param>
        internal DropData(string filePath, WriteableBitmap imageData)
        {
            FilePath = filePath;
            ImageData = imageData;
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
        /// <remarks>これは別クラスにしたほうがいいかも
        /// 最後までWriteableBitmap作るのに必要な値がわからないのから作れないので</remarks>
        public WriteableBitmap ImageData
        {
            get;
            private set;
        }
        
        /// <summary>
        /// BMPデータフォーマット部分
        /// </summary>
        /// <remarks>[0]~[1], ビッグエンディアン, 0x42-0x4d->BM</remarks>
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


        private byte[] _pixel = new byte[2];


        public byte[] Pixel
        {
            get
            {
                return _pixel;
            }
        }


        private byte[] _compressionStyle = new byte[4];


        public byte[] CompressionStyle
        {
            get
            {
                return _compressionStyle;
            }
        }


        private byte[] _compressionSize = new byte[4];


        public byte[] CompressionSize
        {
            get
            {
                return _compressionSize;
            }
        }


        private byte[] _horizontalResolution = new byte[4];


        public byte[] HorizontalResolution
        {
            get
            {
                return _horizontalResolution;
            }
        }


        private byte[] _verticalResolution = new byte[4];


        public byte[] VerticalResolution
        {
            get
            {
                return _verticalResolution;
            }
        }


        private byte[] _color = new byte[4];


        public byte[] Color
        {
            get
            {
                return _color;
            }
        }



        private byte[] _importantColor = new byte[4];


        public byte[] ImportantColor
        {
            get
            {
                return _importantColor;
            }
        }


        public byte[] Data
        {
            get;
            set;
        }
        
        

    }
}
