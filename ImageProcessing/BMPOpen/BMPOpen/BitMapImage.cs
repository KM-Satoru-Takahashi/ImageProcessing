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
            bmp.Save(mms, System.Drawing.Imaging.ImageFormat.Jpeg);

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
            byte[] dataFormat = (new ArraySegment<byte>(dataArr, 0, 2)).Array;

            // 16進数を小文字の文字列化
            string first = BitConverter.ToString(dataFormat).ToLower();

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

            // [0]~[1]はデータフォーマット
            byte[] dataFormat = (new ArraySegment<byte>(bitmapArr, 0, 2)).Array;
            splitBitmapArr.Add(dataFormat);

            // [2]~[5]はファイルサイズ
            byte[] fileSize = (new ArraySegment<byte>(bitmapArr, 2, 4)).Array;
            splitBitmapArr.Add(fileSize);

            // [6]~[7], [8]~[9]は予約領域1, 2
            byte[] reserved1 = (new ArraySegment<byte>(bitmapArr, 6, 2)).Array;
            splitBitmapArr.Add(reserved1);
            byte[] reserver2 = (new ArraySegment<byte>(bitmapArr, 8, 2)).Array;
            splitBitmapArr.Add(reserver2);

            // [10]~[13]はヘッダサイズ→データ部の先頭位置
            byte[] headerSize = (new ArraySegment<byte>(bitmapArr, 10, 4)).Array;
            splitBitmapArr.Add(headerSize);

            // [14]~[17]は情報ヘッダのサイズ
            byte[] infoHeaderSize = (new ArraySegment<byte>(bitmapArr, 14, 4)).Array;
            splitBitmapArr.Add(infoHeaderSize);

            // [18]~[21]は画像の幅
            byte[] imageWidth = (new ArraySegment<byte>(bitmapArr, 18, 4)).Array;
            splitBitmapArr.Add(imageWidth);

            #endregion

            #region データ部

            #endregion

            return splitBitmapArr;
        }
    }
}
