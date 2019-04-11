using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Entities;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Model
{
    /// <summary>
    /// 画像処理実行クラス
    /// </summary>
    internal class ImageManager
    {
        #region field

        /// <summary>
        /// MainWindowModel
        /// </summary>
        private MainWindowModel _parent = null;

        /// <summary>
        /// インチ(mm)
        /// </summary>
        private const float INCH = 25.4f;

        /// <summary>
        /// メートル(mm)
        /// </summary>
        private const float METER = 1000;

        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model"></param>
        internal ImageManager(MainWindowModel model)
        {
            _parent = model;
        }

        /// <summary>
        /// ドロップされたファイルのうち、BMP形式のものについてバイナリとしてデータを得る
        /// </summary>
        /// <param name="dropFilePathList"></param>
        /// <returns></returns>
        internal List<DropData> GetBitmapDropData(List<string> bitmapFileList)
        {
            if (bitmapFileList == null || bitmapFileList.Any() == false)
            {
                return null;
            }

            List<DropData> dropDatas = new List<DropData>();

            foreach (string path in bitmapFileList)
            {
                if (string.IsNullOrEmpty(path) == true)
                {
                    continue;
                }

                // BMPファイルをバイナリとして取得する
                byte[] bmpBinaryArr = GetBitmapBinary(path);

                // 正常に取得できていればリストに追加する
                if (bmpBinaryArr != null && bmpBinaryArr.Any() == true)
                {
                    DropData data = CreateDropData(bmpBinaryArr, path);

                    if (data != null)
                    {
                        dropDatas.Add(data);
                    }
                }

            }

            return dropDatas;
        }

        /// <summary>
        /// 与えられたファイルパスのBMPファイルをバイナリデータとして取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private byte[] GetBitmapBinary(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return null;
            }

            // BMPファイルを取得する
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(filePath);
            }
            catch (Exception e)
            {
                // 例外処理
                return null;
            }

            // 読み書き用MemoryStreamを生成
            MemoryStream ms = new MemoryStream();

            // msとbmpを紐付け
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

            // byte配列に
            byte[] bmpBinaryArray = ms.GetBuffer();

            // 開放処理
            ms.Close();
            ms.Dispose();

            return bmpBinaryArray;
        }

        /// <summary>
        /// 与えられたBMPバイナリデータからDropDataオブジェクトを作成する
        /// </summary>
        /// <param name="bmpBinaryData">BMPファイルのバイナリデータ</param>
        /// <returns>BMPファイルのバイナリデータを処理したDropDataオブジェクト</returns>
        private DropData CreateDropData(byte[] bmpBinaryData, string filePath)
        {
            // [0]~[53]はヘッダ情報なので[54]がなければbmpファイルではない
            // [0]~[54]までの要素数は55なのでLengthが55未満なら異常
            if (bmpBinaryData.Length < 55)
            {
                return null;
            }

            // オブジェクトを生成
            DropData dropData = SetBMPData(bmpBinaryData, filePath);

            dropData.ImageData = CreateWBMP(dropData);

            return dropData;
        }

        /// <summary>
        /// DropDataオブジェクトにBMP画像のバイナリデータを入れ込む
        /// </summary>
        /// <param name="bmpBinaryData">bmpファイルのバイナリデータ配列</param>
        /// <returns>バイナリデータを入れ込んだDropDataオブジェクト</returns>
        private DropData SetBMPData(byte[] bmpBinaryData, string filePath)
        {
            DropData dropData = new DropData(filePath);

            #region ヘッダ部

            // [0]~[53]は必ず存在しているのでチェック無しで配列から値を持ってこられる
            // [0]~[1]データフォーマット部分
            Array.Copy(bmpBinaryData, 0, dropData.DataFormat, 0, 2);

            // [2]~[5]ファイルサイズ
            Array.Copy(bmpBinaryData, 2, dropData.FileSize, 0, 4);

            // [6]~[7], [8]~[9]予約領域
            Array.Copy(bmpBinaryData, 6, dropData.ReservedRegion1, 0, 2);
            Array.Copy(bmpBinaryData, 8, dropData.ReservedRegion2, 0, 2);

            // [10]~[13]ヘッダサイズ
            Array.Copy(bmpBinaryData, 10, dropData.HeaderSize, 0, 4);

            // [14]~[17]情報ヘッダサイズ
            Array.Copy(bmpBinaryData, 14, dropData.InfoHeaderSize, 0, 4);

            // [18]~[21]画像幅
            Array.Copy(bmpBinaryData, 18, dropData.ImageWidth, 0, 4);

            // [22]~[25]画像高さ
            Array.Copy(bmpBinaryData, 22, dropData.ImageHeight, 0, 4);

            // [26]~[27]プレーン数
            Array.Copy(bmpBinaryData, 26, dropData.Plane, 0, 2);

            // [28]~[29]1画素の色数
            Array.Copy(bmpBinaryData, 28, dropData.Pixel, 0, 2);

            // [30]~[33]圧縮形式
            Array.Copy(bmpBinaryData, 30, dropData.CompressionStyle, 0, 4);

            // [34]~[37]圧縮サイズ
            Array.Copy(bmpBinaryData, 34, dropData.CompressionSize, 0, 4);

            // [38]~[41]水平解像度
            Array.Copy(bmpBinaryData, 38, dropData.VerticalResolutionDPM, 0, 4);

            // [42]~[45]垂直解像度
            Array.Copy(bmpBinaryData, 42, dropData.HorizontalResolutionDPM, 0, 4);

            // [46]~[49]色数
            Array.Copy(bmpBinaryData, 46, dropData.Color, 0, 4);

            // [50]~[53]重要色数
            Array.Copy(bmpBinaryData, 50, dropData.ImportantColor, 0, 4);

            #endregion

            // 解像度をDPMからDPIにする必要がある
            dropData.HorizontalResolutionDPI = ConversionDPMtoDPI(dropData.HorizontalResolutionDPM);
            dropData.VerticalResolutionDPI = ConversionDPMtoDPI(dropData.VerticalResolutionDPM);

            // データ部の配列の要素数を調べる必要がある
            int bmpLength = bmpBinaryData.Length;
            // ヘッダ情報部分を除いた配列の個数を得る
            int dataLength = bmpLength - 54;
            // 必要な配列を生成
            dropData.Data = new byte[dataLength];
            // 配列にコピー
            Array.Copy(bmpBinaryData, 54, dropData.Data, 0, dataLength);

            return dropData;
        }

        /// <summary>
        /// 解像度のDPM値をDPI値に変換する(バイナリデータ)
        /// </summary>
        /// <param name="binaryDPM">解像度のDPM(dot/meter)値</param>
        /// <returns>解像度のDPI(dot/inch)値</returns>
        private float ConversionDPMtoDPI(byte[] binaryDPM)
        {
            // バイト配列を数値に変換
            // リトルエンディアンはそのままConvertしてくれる
            int dpm = BitConverter.ToInt32(binaryDPM, 0);

            return ConversionDPMtoDPI(dpm);
        }

        /// <summary>
        /// 解像度のDPM値をDPI値に変換する(数値)
        /// </summary>
        /// <param name="dpm">解像度のDPM(dot/meter)値</param>
        /// <returns>解像度のDPI(dot/inch)値</returns>
        private float ConversionDPMtoDPI(float dpm)
        {
            return dpm * INCH / METER;
        }

        /// <summary>
        /// BMPのバイナリデータからWriteableBitmapデータを作成する
        /// </summary>
        /// <param name="bmpData">各データを入れ込んだエンティティ</param>
        /// <returns>WriteableBitampオブジェクト</returns>
        private WriteableBitmap CreateWBMP(DropData bmpData)
        {
            // 画像幅
            int width = BitConverter.ToInt32(bmpData.ImageWidth, 0);
            // 画像高さ
            int height = BitConverter.ToInt32(bmpData.ImageHeight, 0);

            WriteableBitmap wbmp = new WriteableBitmap
                (
                width,
                height,
                bmpData.HorizontalResolutionDPI,
                bmpData.VerticalResolutionDPI,
                // System.Windows.Media.PixelFormats.Gray8, // グレースケール用
                System.Windows.Media.PixelFormats.Bgr32, // RGB用
                null // indexつきのbmp以外はnullで良い
                );

            // バイナリ配列データを用意する
            // 1次元配列の中で行列が区別される

            // 試作→上からグラデーションをかける
            byte[] dataArr = new byte[width * height];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int index = row * width         // 行
                                            + col;  // 列

                    dataArr[index] = (byte)(0xFF * row / (double)height - 1);
                }
            }

            // 試作→そのまま入れ込む
            // WritePixelsで範囲外エラー発生
            // Int32Rect, wbmp作成時のwidth, heightも*3しないとだめかも
            // 
            //byte[] dataArr = new byte[width * height * 3];
            //for (int i = 0; i < bmpData.Data.Length && i < dataArr.Length; i++)
            //{
            //    dataArr[i] = bmpData.Data[i];
            //}


            // wbmpに書き込む
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, width, height),
            dataArr,
            width,
            0
            );
            return wbmp;
        }

    }
}
