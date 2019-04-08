using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;

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
            // 読み書き用mms
            System.IO.MemoryStream mms =
                new System.IO.MemoryStream();

            // bmpファイルの生成
            Bitmap bmp = new Bitmap(_testPath);

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
            mms.Close();
            mms.Dispose();

        }

    }
}
