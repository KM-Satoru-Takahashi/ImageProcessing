using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Markup;

namespace BMPOpen
{
    internal class BitMapImage
    {
        /// <summary>
        /// テスト用画像ファイルパス
        /// </summary>
        /// <remarks>このパスから読み込む</remarks>
        private string _testPath = @"C:\ImageProcessing\ImageProcessing\test.bmp";

        /// <summary>
        /// テストコード
        /// テスト画像用ファイルパスを読み込んで、WriteableBitmapを使用する
        /// </summary>
        internal void GetWriteableBitmapSample()
        {
            // BMPファイルを取得する
            Bitmap bmp = null;

            // ファイルが存在しているか
            if (File.Exists(_testPath))
            {
                try
                {
                    // bmpファイルの生成
                    bmp = new Bitmap(_testPath);
                }
                catch (Exception e)
                {
                    // error
                }
            }

            // 読み書き用mms
            System.IO.MemoryStream mms = new System.IO.MemoryStream();

            // bmpをmmsと関連付けする
            bmp.Save(mms, System.Drawing.Imaging.ImageFormat.Bmp);

            // bmpをバイナリにしてByte配列に
            Byte[] bmpArr = mms.GetBuffer();

            // 変換処理
            mms.Seek(0, System.IO.SeekOrigin.Begin);

            // BitmapSourceの生成
            BitmapSource bmpSource = BitmapFrame.Create
                (
                mms,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad
                );

            WriteableBitmap wbmp = new WriteableBitmap(bmpSource);

            // 終了処理
            mms.Close();
            mms.Dispose();

            // バイナリ処理テスト
            CheckBinaryFile(bmpArr);

        }

        /// <summary>
        /// bmpファイルのバイナリ配列を読み取る
        /// </summary>
        /// <param name="dataArr"></param>
        private void CheckBinaryFile(byte[] dataArr)
        {
            // 引数チェック
            if (dataArr == null || dataArr.Any() == false)
            {
                return;
            }

            // 配列の使える要素数を保持しておく
            int dataCount = dataArr.Length - 1;

            // bmpは0始まりで少なくとも54Byte(0~53)のヘッダ部+データ部1以上ある
            // dataArr[54]にアクセスできなければだめ
            if (dataCount < 54)
            {
                return;
            }

            // [0]~[1]はデータフォーマット
            // byte[] dataFormat = (new ArraySegment<byte>(dataArr, 0, 2)).Array;
            byte[] dataFormat = new byte[2];
            Array.Copy(dataArr, 0, dataFormat, 0, 2);

            // 16進数を小文字の文字列化            
            string first = System.Text.Encoding.ASCII.GetString(dataFormat).ToLower();


            if (first == "bm")
            {
                // .bmpファイル
                ReadBitmapBinary(dataArr);
            }
        }

        /// <summary>
        /// bmpファイルのバイナリ配列を読み取る
        /// </summary>
        /// <param name="bitmapArr"></param>
        private void ReadBitmapBinary(byte[] bitmapArr)
        {
            List<byte[]> bitmapArrDatas = SplitBitmapBinary(bitmapArr);
        }

        /// <summary>
        /// bmpArrを必要なものに切る
        /// </summary>
        /// <param name="bitmapArr"></param>
        /// <returns></returns>
        private List<byte[]> SplitBitmapBinary(byte[] bitmapArr)
        {
            List<byte[]> splitBitmapArr = new List<byte[]>();

            #region ヘッダ部 54byte [0]~[53]

            #region BITMAP FILEHEADER

            // [0]~[1]はデータフォーマット
            // この部分はリトルエンディアンではないので注意する
            // BMPなら0x42, 0x4DでASCIIコードBM
            byte[] dataFormat = new byte[2];
            Array.Copy(bitmapArr, 0, dataFormat, 0, 2);
            splitBitmapArr.Add(dataFormat);

            // [2]~[5]はファイルサイズ byte
            byte[] fileSize = new byte[4];
            Array.Copy(bitmapArr, 2, fileSize, 0, 4);
            splitBitmapArr.Add(fileSize);

            // [6]~[7], [8]~[9]は予約領域1, 2→常に0
            byte[] reserved1 = new byte[2];
            Array.Copy(bitmapArr, 6, reserved1, 0, 2);
            splitBitmapArr.Add(reserved1);
            byte[] reserver2 = new byte[2];
            Array.Copy(bitmapArr, 8, reserver2, 0, 2);
            splitBitmapArr.Add(reserver2);

            // [10]~[13]はヘッダサイズ→データ部の先頭位置
            byte[] headerSize = (new ArraySegment<byte>(bitmapArr, 10, 4)).Array;
            splitBitmapArr.Add(headerSize);

            #endregion

            #region BITMAP INFOHEADER

            // [14]~[17]は情報ヘッダのサイズ→40 byte
            byte[] infoHeaderSize = (new ArraySegment<byte>(bitmapArr, 14, 4)).Array;
            splitBitmapArr.Add(infoHeaderSize);

            // [18]~[21]は画像の幅 pixel(px)
            byte[] imageWidth = (new ArraySegment<byte>(bitmapArr, 18, 4)).Array;
            splitBitmapArr.Add(imageWidth);

            // [22]~[25]は画像の高さ pixel(px)
            byte[] imageHeight = (new ArraySegment<byte>(bitmapArr, 22, 4)).Array;
            splitBitmapArr.Add(imageHeight);

            // [26]~[27]はプレーン数(常に1→01 00)
            byte[] plane = (new ArraySegment<byte>(bitmapArr, 26, 2)).Array;
            splitBitmapArr.Add(plane);

            // [28]~[29]は1画素の色数
            // RGBなら1byte*3で24bit, グレースケールなら1byteで8bit
            byte[] pixel = (new ArraySegment<byte>(bitmapArr, 28, 2)).Array;
            splitBitmapArr.Add(pixel);

            // [30]~[33]は圧縮形式
            byte[] compressionStyle = (new ArraySegment<byte>(bitmapArr, 30, 4)).Array;
            splitBitmapArr.Add(compressionStyle);

            // [34]~[37]は圧縮サイズ byte
            byte[] compressionSize = (new ArraySegment<byte>(bitmapArr, 34, 4)).Array;
            splitBitmapArr.Add(compressionSize);

            // [38]~[41]は水平解像度 ppm (px/m)
            byte[] horizontalResolution = (new ArraySegment<byte>(bitmapArr, 38, 4)).Array;
            splitBitmapArr.Add(horizontalResolution);

            // [42]~[45]は垂直解像度 ppm (px/m)
            byte[] verticalResolution = (new ArraySegment<byte>(bitmapArr, 42, 4)).Array;
            splitBitmapArr.Add(verticalResolution);

            // [46]~[49]は色数
            // 0なら全色を使用
            byte[] color = (new ArraySegment<byte>(bitmapArr, 46, 4)).Array;
            splitBitmapArr.Add(color);

            // [50]~[53]は重要色数
            // 0なら全色を使用
            byte[] importantColor = (new ArraySegment<byte>(bitmapArr, 50, 4)).Array;
            splitBitmapArr.Add(importantColor);

            #endregion

            #endregion

            #region データ部

            // 読み込んだbitmapArrのうち、[54]~最後までがデータ部となる
            int dataCount = bitmapArr.Count() - 54;
            byte[] bitmapDataArr = (new ArraySegment<byte>(bitmapArr, 54, dataCount)).Array;

            #endregion

            return splitBitmapArr;
        }
    }
}
