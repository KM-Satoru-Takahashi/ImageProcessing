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
        

        private byte[] _dataFormat = new Byte[2];


        public byte[] DataFormat
        {
            get
            {
                return _dataFormat;
            }
        }


        private byte[] _fileSize = new byte[4];


        public byte[] FileSize
        {
            get
            {
                return _fileSize;
            }
        }


        private byte[] _reservedRegion1 = new byte[2];


        public byte[] ReserevedRegion1
        {
            get
            {
                return _reservedRegion1;
            }
        }


        private byte[] _reservedRegion2 = new byte[2];


        public byte[] ReservedRegion2
        {
            get
            {
                return _reservedRegion2;
            }
        }


        private byte[] _headerSize = new byte[4];


        public byte[] HeaderSize
        {
            get
            {
                return _headerSize;
            }
        }


        private byte[] _infoHeaderSize = new byte[4];


        public byte[] InfoHeaderSize
        {
            get
            {
                return _infoHeaderSize;
            }
        }


        private byte[] _imageWidth = new byte[4];


        public byte[] ImageWidth
        {
            get
            {
                return _imageWidth;
            }
        }


        private byte[] _imageHeight = new byte[4];


        public byte[] ImageHeight
        {
            get
            {
                return _imageHeight;
            }
        }


        private byte[] _plane = new byte[2];


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
                return _verticalResoluton;
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


        public byte[] Color
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
