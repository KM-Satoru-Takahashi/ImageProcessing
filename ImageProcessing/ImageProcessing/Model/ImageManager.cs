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

        /// <summary>
        /// RGBA表現時の画素バイト数
        /// </summary>
        private const int COLORBYTE_RGBA = 4;

        /// <summary>
        /// BMP画像データで揃えるべき幅ピクセルの境界値
        /// </summary>
        private const int BMP_WIDTH_BOUNDARY = 4;

        /// <summary>
        /// WriteableBitmapに書き込む際のアルファ値のデフォルト(255)
        /// </summary>
        private readonly byte DEFAULT_ALPHA = 255;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model"></param>
        internal ImageManager(MainWindowModel model)
        {
            _parent = model;
        }

        #region BMPファイル読み込み

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

            dropData.ImageData = CreateWBMP(dropData.ImageParameter);

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
            Array.Copy(bmpBinaryData, 0, dropData.RowData.DataFormat, 0, 2);

            // [2]~[5]ファイルサイズ
            Array.Copy(bmpBinaryData, 2, dropData.RowData.FileSize, 0, 4);

            // [6]~[7], [8]~[9]予約領域
            Array.Copy(bmpBinaryData, 6, dropData.RowData.ReservedRegion1, 0, 2);
            Array.Copy(bmpBinaryData, 8, dropData.RowData.ReservedRegion2, 0, 2);

            // [10]~[13]ヘッダサイズ
            Array.Copy(bmpBinaryData, 10, dropData.RowData.HeaderSize, 0, 4);

            // [14]~[17]情報ヘッダサイズ
            Array.Copy(bmpBinaryData, 14, dropData.RowData.InfoHeaderSize, 0, 4);

            // [18]~[21]画像幅
            Array.Copy(bmpBinaryData, 18, dropData.RowData.ImageWidth, 0, 4);

            // [22]~[25]画像高さ
            Array.Copy(bmpBinaryData, 22, dropData.RowData.ImageHeight, 0, 4);

            // [26]~[27]プレーン数
            Array.Copy(bmpBinaryData, 26, dropData.RowData.Plane, 0, 2);

            // [28]~[29]1画素の色数
            Array.Copy(bmpBinaryData, 28, dropData.RowData.Pixel, 0, 2);

            // [30]~[33]圧縮形式
            Array.Copy(bmpBinaryData, 30, dropData.RowData.CompressionStyle, 0, 4);

            // [34]~[37]圧縮サイズ
            Array.Copy(bmpBinaryData, 34, dropData.RowData.CompressionSize, 0, 4);

            // [38]~[41]水平解像度
            Array.Copy(bmpBinaryData, 38, dropData.RowData.VerticalResolutionDPM, 0, 4);

            // [42]~[45]垂直解像度
            Array.Copy(bmpBinaryData, 42, dropData.RowData.HorizontalResolutionDPM, 0, 4);

            // [46]~[49]色数
            Array.Copy(bmpBinaryData, 46, dropData.RowData.Color, 0, 4);

            // [50]~[53]重要色数
            Array.Copy(bmpBinaryData, 50, dropData.RowData.ImportantColor, 0, 4);

            #endregion

            // データ部の配列の要素数を調べる必要がある
            int bmpLength = bmpBinaryData.Length;
            // ヘッダ情報部分を除いた配列の個数を得る
            int dataLength = bmpLength - 54;
            // 必要な配列を生成
            dropData.RowData.Data = new byte[dataLength];
            // 配列にコピー
            Array.Copy(bmpBinaryData, 54, dropData.RowData.Data, 0, dataLength);

            // ImageParameter内への代入

            // 解像度をDPMからDPIに
            dropData.ImageParameter.HorizontalResolutionDPI = ConversionDPMtoDPI(dropData.RowData.HorizontalResolutionDPM);
            dropData.ImageParameter.VerticalResolutionDPI = ConversionDPMtoDPI(dropData.RowData.VerticalResolutionDPM);

            // 画像情報
            dropData.ImageParameter.BinaryData = dropData.RowData.Data;

            // 幅と高さ
            dropData.ImageParameter.ImageWidth = BitConverter.ToInt32(dropData.RowData.ImageWidth, 0);
            dropData.ImageParameter.ImageHeight = BitConverter.ToInt32(dropData.RowData.ImageHeight, 0);

            // pixel数
            dropData.ImageParameter.Pixel = BitConverter.ToInt16(dropData.RowData.Pixel, 0);

            return dropData;
        }

        #endregion

        #region 回転

        /// <summary>
        /// バイナリデータの右回転処理
        /// </summary>
        /// <param name="imageParameter">回転対称となるデータ</param>
        /// <returns>右回転した画像データのWriteableBMP形式</returns>
        internal WriteableBitmap RightRotate(ImageParameter imageParameter)
        {
            if (imageParameter == null)
            {
                return null;
            }

            // 縦横が入れ替わる
            int newWidth = imageParameter.ImageHeight;
            int newHeight = imageParameter.ImageWidth;

            // 現在表示されているWBMP画像のバイナリを取得する
            byte[] oldWBMPBinary = imageParameter.BinaryData;

            if (oldWBMPBinary == null || oldWBMPBinary.Any() == false)
            {
                return null;
            }

            // 新データを入れる配列を用意しておく
            byte[] newImageData = new byte[imageParameter.BinaryData.Length];

            // データを入れるカウンタ
            int newArrayCount = 0;
            // 旧データのn列目を下側から読んだものが、新データのn行目と対応する
            // 旧データの列数(新データの行数)だけ繰り返す
            for (int oldCol = 0; oldCol < newHeight; oldCol++)
            {
                // oldCol列目を下側から読んだものが新しい配列のn行目となる
                // 旧配列における対応する最初の座標(一番下の行の当該列の最初の画素が入っているインデックス)を求める
                // 最下行に必ず最後の要素があるので、最下行の最後から巻き戻れば値を取得できる
                int readStartIndex = oldWBMPBinary.Length  // 全要素数を取得
                                        - (newHeight - oldCol) * COLORBYTE_RGBA; // (旧幅-ほしい列番)*ピクセル数で当該要素まで巻き戻る

                // その列の要素を上まで登っていく
                for (int element = readStartIndex; element >= 0; element -= newHeight * COLORBYTE_RGBA) // 行のストライドだけ上方に遷移すれば列を求められる
                {
                    // tempListへ順に保存していく
                    newImageData[newArrayCount] = oldWBMPBinary[element];           // B
                    newImageData[newArrayCount + 1] = oldWBMPBinary[element + 1];   // G
                    newImageData[newArrayCount + 2] = oldWBMPBinary[element + 2];   // R
                    newImageData[newArrayCount + 3] = oldWBMPBinary[element + 3];   // A
                    newArrayCount += 4;
                }
            }

            // imageParameterの更新
            imageParameter.ImageHeight = newHeight;
            imageParameter.ImageWidth = newWidth;
            imageParameter.BinaryData = newImageData;

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                newWidth,
                newHeight,
                imageParameter.HorizontalResolutionDPI,
                imageParameter.VerticalResolutionDPI,
                System.Windows.Media.PixelFormats.Pbgra32, // BGRA32用
                null // indexつきのbmp以外はnullで良い
                );

            // WBMPに書き込む            
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, newWidth, newHeight),
            newImageData,
            newWidth * COLORBYTE_RGBA, // ストライド：行の要素数*色pixel数
            0
            );

            return wbmp;
        }

        /// <summary>
        /// バイナリデータの左回転処理
        /// </summary>
        /// <param name="imageParameter">回転対称となるデータ</param>
        /// <returns>左回転した画像データのWriteableBMP形式</returns>
        internal WriteableBitmap LeftRotate(ImageParameter imageParameter)
        {
            if (imageParameter == null)
            {
                return null;
            }

            // 縦横が入れ替わる
            int newWidth = imageParameter.ImageHeight;
            int newHeight = imageParameter.ImageWidth;

            // 現在表示されているWBMP画像のバイナリを取得する
            byte[] oldWBMPBinary = imageParameter.BinaryData;
            if (oldWBMPBinary == null || oldWBMPBinary.Any() == false)
            {
                return null;
            }

            // 新データを入れる配列を用意
            byte[] newImageData = new byte[imageParameter.BinaryData.Length];

            // 新データに入れ込む際のカウンタ
            int newArrayCount = 0;

            // 旧データの(width-n)列目(最後からn列目)を上から読んだものが、新しい配列のn行目になる
            // 旧データの列数(新データの行数)だけ繰り返す
            for (int oldCol = newHeight; 0 < oldCol; oldCol--)
            {
                // 旧データの最後からoldCol列目を上から読んだものが新しい配列のn行目になる
                int readStartIndex = oldCol * 4  // 旧列の先頭から読み取る
                                                - COLORBYTE_RGBA;   // 当該要素の先頭からBGRAを読むため巻き戻しておく

                // 配列の要素範囲内にある間、旧データのストライドだけ下に遷移し続ける
                for (int element = readStartIndex; element < oldWBMPBinary.Length - 1; element += newHeight * COLORBYTE_RGBA)
                {
                    // tempListへ順に値を代入する
                    newImageData[newArrayCount] = oldWBMPBinary[element];            // B
                    newImageData[newArrayCount + 1] = oldWBMPBinary[element + 1];    // G
                    newImageData[newArrayCount + 2] = oldWBMPBinary[element + 2];    // R
                    newImageData[newArrayCount + 3] = oldWBMPBinary[element + 3];    // A
                    newArrayCount += 4;
                }
            }

            // imageParameterの更新
            imageParameter.ImageHeight = newHeight;
            imageParameter.ImageWidth = newWidth;
            imageParameter.BinaryData = newImageData;

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                newWidth,
                newHeight,
                imageParameter.HorizontalResolutionDPI,
                imageParameter.VerticalResolutionDPI,
                System.Windows.Media.PixelFormats.Pbgra32, // BGRA32用
                null // indexつきのbmp以外はnullで良い
                );

            // WBMPに書き込む            
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, newWidth, newHeight),
            newImageData,
            newWidth * COLORBYTE_RGBA, // ストライド：行の要素数*色pixel数
            0
            );

            return wbmp;
        }

        #endregion

        #region 画素取得と更新

        /// <summary>
        /// 表示されている画像データのバイナリ配列から要求された画素を返す
        /// </summary>
        /// <returns>画素情報オブジェクト</returns>
        /// <param name="imageParameter">対象画像</param>
        /// <param name="doublePixelPoint">マウス座標</param>
        /// <remarks>RGBAの[n]~[n+2]における[n]の位置を返す。異常時は0</remarks>
        internal PixelData GetPixelInfo(ImageParameter imageParameter, System.Windows.Point doublePixelPoint)
        {
            if (imageParameter == null)
            {
                return null;
            }

            // WBMPの画像データを取得する
            byte[] dataArr = imageParameter.BinaryData;

            if (dataArr == null || dataArr.Any() == false)
            {
                return null;
            }

            // マウス座標をintに変換する
            Point intPixelPoint = ConvertDblPtToIntPt(doublePixelPoint);

            // 画像のバイナリデータ内での位置を取得する
            int pixelPos = GetPixelPosition(intPixelPoint.X, intPixelPoint.Y, imageParameter.ImageWidth, COLORBYTE_RGBA);

            // 画像の領域を超えていれば異常
            // BGRAであれば、BGRAのBの位置を取得しているのでAの位置まであるかをチェックする必要がある
            if (dataArr.Length < pixelPos + COLORBYTE_RGBA - 1)
            {
                return null;
            }

            // オブジェクトを生成してwbmpから各値を入れ込んで返す
            PixelData pixelData = new PixelData();
            pixelData.XCoordinate = intPixelPoint.X;
            pixelData.YCoordinate = intPixelPoint.Y;
            // BGRAの順
            pixelData.OldBlue = dataArr[pixelPos];
            pixelData.OldGreen = dataArr[pixelPos + 1];
            pixelData.OldRed = dataArr[pixelPos + 2];
            pixelData.OldAlpha = dataArr[pixelPos + 3];

            return pixelData;
        }

        /// <summary>
        /// WBMPの指定された画素を渡された画素情報で更新する
        /// </summary>
        /// <param name="imageParameter">更新対象のWBMP</param>
        /// <param name="updateData">更新したい画素情報</param>
        /// <returns>更新したWBMP</returns>
        internal WriteableBitmap SetPixelInfo(ImageParameter imageParameter, PixelData updateData)
        {
            if (imageParameter == null || updateData == null)
            {
                return null;
            }

            // WBMPの画像データを取得する
            byte[] dataArr = imageParameter.BinaryData;

            if (dataArr == null || dataArr.Any() == false)
            {
                return null;
            }

            // 画像のバイナリデータ内での位置を取得する
            int pixelPos = GetPixelPosition(updateData.XCoordinate, updateData.YCoordinate, imageParameter.ImageWidth, COLORBYTE_RGBA);

            return UpdateWBMPPixel(imageParameter, pixelPos, updateData);
        }

        /// <summary>
        /// 与えられたBMPオブジェクトを指定された画素データで更新する
        /// </summary>
        /// <param name="targetWBMP">更新対象のWBMPデータ</param> 
        /// <param name="targetArr">更新対象のWBMPデータ配列</param>
        /// <param name="pixelPosition">更新開始バイナリ位置</param>
        /// <param name="pixelData">更新データ</param>
        /// <returns>更新したWBMPオブジェクト</returns>
        private WriteableBitmap UpdateWBMPPixel(ImageParameter imageParameter, int pixelPosition, PixelData pixelData)
        {
            // 引数オブジェクトのnullチェック
            if (imageParameter == null || pixelData == null)
            {
                return null;
            }
            // 引数配列のnullチェック
            else if (imageParameter.BinaryData == null || imageParameter.BinaryData.Any() == false)
            {
                return null;
            }
            // 画像の領域を超えていれば異常
            // BGRAであれば、BGRAのBの位置を取得しているのでAの位置まであるかをチェックする必要がある
            else if (imageParameter.BinaryData.Length < pixelPosition + COLORBYTE_RGBA - 1)
            {
                return null;
            }

            // 値の埋め込み
            imageParameter.BinaryData[pixelPosition] = pixelData.NewBlue;       // B
            imageParameter.BinaryData[pixelPosition + 1] = pixelData.NewGreen;  // G
            imageParameter.BinaryData[pixelPosition + 2] = pixelData.NewRed;    // R
            imageParameter.BinaryData[pixelPosition + 3] = pixelData.NewAlpha;  // A

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                imageParameter.ImageWidth,
                imageParameter.ImageHeight,
                imageParameter.HorizontalResolutionDPI,
                imageParameter.VerticalResolutionDPI,
                // System.Windows.Media.PixelFormats.Gray8, // グレースケール用
                // System.Windows.Media.PixelFormats.Bgr32, // RGB用
                System.Windows.Media.PixelFormats.Pbgra32, // BGRA32用
                null // indexつきのbmp以外はnullで良い
                );

            // wbmpに書き込む
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, imageParameter.ImageWidth, imageParameter.ImageHeight),
            imageParameter.BinaryData,
            imageParameter.ImageWidth * COLORBYTE_RGBA,
            0
            );

            return wbmp;
        }

        /// <summary>
        /// 座標情報と画像の表示幅から、その地点の画素情報が配列の何番目かを取得する
        /// </summary>
        /// <param name="xCoordinate">画像のX座標</param>
        /// <param name="yCoordinate">画像のY座標</param>
        /// <param name="width">画像の幅(pixel)</param>
        /// <param name="colorByte">1画素あたりの画素数(例:RGBなら3)</param>
        /// <returns>バイナリデータ上での対象画素情報開始位置(0~)</returns>
        private int GetPixelPosition(int xCoordinate, int yCoordinate, int width, int colorByte)
        {
            return ((yCoordinate * width) + xCoordinate) * colorByte;
        }

        #endregion

        #region 反転

        /// <summary>
        ///  画像反転処理(左右)
        /// </summary>
        /// <param name="imageParameter">反転対象のWBMPデータ</param>
        /// <returns></returns>
        internal WriteableBitmap Flip(ImageParameter imageParameter)
        {
            if (imageParameter == null)
            {
                return null;
            }

            // 現在表示されているWBMP画像のバイナリを取得する
            byte[] oldWBMPBinary = imageParameter.BinaryData;

            // 反転後のバイナリデータを入れ込む配列
            byte[] newBMPBinary = new byte[oldWBMPBinary.Length];

            int newBinaryCount = 0;
            // 旧データの1行目から最後の行までを順に読んでいく
            for (int oldRow = 0; oldRow < imageParameter.ImageHeight; oldRow++)
            {
                // oldRowの列を最後から順に読んで行けばよい
                for (int oldCol = imageParameter.ImageWidth; 0 < oldCol; oldCol--)
                {
                    int readStartPosition = oldCol * COLORBYTE_RGBA - COLORBYTE_RGBA    // この列の要素の先頭
                                                    + oldRow * imageParameter.ImageWidth * COLORBYTE_RGBA;   // 行数を加味する

                    // BGRAの順に入れ込んでいく
                    newBMPBinary[newBinaryCount] = oldWBMPBinary[readStartPosition];
                    newBMPBinary[newBinaryCount + 1] = oldWBMPBinary[readStartPosition + 1];
                    newBMPBinary[newBinaryCount + 2] = oldWBMPBinary[readStartPosition + 2];
                    newBMPBinary[newBinaryCount + 3] = oldWBMPBinary[readStartPosition + 3];
                    newBinaryCount += 4;
                }
            }

            // imageParameterの更新
            imageParameter.BinaryData = newBMPBinary;

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                imageParameter.ImageWidth,
                imageParameter.ImageHeight,
                imageParameter.HorizontalResolutionDPI,
                imageParameter.VerticalResolutionDPI,
                System.Windows.Media.PixelFormats.Pbgra32,
                null
                );

            // WBMPに書き込む
            wbmp.WritePixels
                (
                new System.Windows.Int32Rect(0, 0, imageParameter.ImageWidth, imageParameter.ImageHeight),
                imageParameter.BinaryData,
                imageParameter.ImageWidth * COLORBYTE_RGBA, // ストライド：行の要素数*色pixel数
                0
                );

            return wbmp;
        }

        #endregion

        #region 拡縮

        /// <summary>
        /// NearestNeighbor/最近傍法による画像の拡縮処理
        /// </summary>
        /// <param name="imageParameter"></param>
        /// <param name="widthPercent"></param>
        /// <param name="heightPercent"></param>
        /// <returns></returns>
        internal WriteableBitmap ScalingNearestNeighbor(ImageParameter imageParameter, double widthPercent, double heightPercent)
        {
            if (imageParameter == null)
            {
                return null;
            }

            // 新たに作成する画像の縦横ピクセルを取得する
            int newWidth = GetNewPixelLength(imageParameter.ImageWidth, widthPercent);
            int newHeight = GetNewPixelLength(imageParameter.ImageHeight, heightPercent);

            // 新たに作成する画像のバイナリ情報を代入できるデータ配列を作成する
            byte[] newDataArray = CreateDataArray(newWidth, newHeight, COLORBYTE_RGBA);

            // 旧データのバイナリ配列を取得しておく
            byte[] oldDataArray = imageParameter.BinaryData;
            if (oldDataArray == null || oldDataArray.Any() == false)
            {
                return null;
            }

            // 拡縮Percentを比率(ratio)になおしておく
            double widthRatio = widthPercent / 100;
            double heightRatio = heightPercent / 100;

            // 新配列の0行目から、順に行を見ていく
            for (int row = 0; row < newHeight; row++)
            {
                // 当該行の0列目から、順に列を見ていく
                for (int col = 0; col < newWidth; col++)
                {
                    // 新要素のインデックス
                    int index = row * newWidth * COLORBYTE_RGBA        // 何行目か
                                            + col * COLORBYTE_RGBA;    // 何列目か

                    // 最近傍として対応する旧データの要素のインデックス// todo: percentに端数がある場合の対応
                    int oldIndex = (int)((int)(row / widthRatio) * (newWidth / widthRatio) * COLORBYTE_RGBA   // 何行目に対応するか
                                                          + (int)(col / heightRatio) * COLORBYTE_RGBA);            // 何列目に対応するか

                    // 中間値の場合は旧データの近傍(等間隔⇒左上)に合わせる
                    if (oldIndex % COLORBYTE_RGBA != 0)
                    {
                        int remainder = oldIndex % COLORBYTE_RGBA;
                        oldIndex -= remainder;
                    }

                    // BGRAの順に入れ込む
                    newDataArray[index] = oldDataArray[oldIndex];
                    newDataArray[index + 1] = oldDataArray[oldIndex + 1];
                    newDataArray[index + 2] = oldDataArray[oldIndex + 2];
                    newDataArray[index + 3] = oldDataArray[oldIndex + 3];
                }

            }

            // ImageParameterの更新
            imageParameter.ImageWidth = newWidth;
            imageParameter.ImageHeight = newHeight;
            imageParameter.BinaryData = newDataArray;

            // WBMPを作成する
            WriteableBitmap wbmp = new WriteableBitmap
               (
               newWidth,
               newHeight,
               imageParameter.HorizontalResolutionDPI,
               imageParameter.VerticalResolutionDPI,
               System.Windows.Media.PixelFormats.Pbgra32,
               null
               );

            // 書き込む
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, newWidth, newHeight),
            newDataArray,
            newWidth * COLORBYTE_RGBA, // ストライド：行の要素数*色pixel数
            0
            );

            return wbmp;
        }

        /// <summary>
        /// Bilinear/線形補間法による画像の拡縮処理
        /// </summary>
        /// <param name="imageParameter"></param>
        /// <param name="widthPercent"></param>
        /// <param name="heightPercent"></param>
        /// <returns></returns>
        internal WriteableBitmap ScalingBilinear(ImageParameter imageParameter, double widthPercent, double heightPercent)
        {
            return null;
        }

        /// <summary>
        /// 拡縮後の画像におけるピクセル数を取得する
        /// </summary>
        /// <param name="oldPixel">旧値</param>
        /// <param name="percent">拡縮比率</param>
        /// <returns></returns>
        private int GetNewPixelLength(int oldPixel, double percent)
        {
            int newScale = (int)(oldPixel * (percent / 100));

            return newScale;
        }


        #endregion

        #region CreateDataArray

        /// <summary>
        /// 指定した画像をバイナリデータとして代入可能な空のbyte[]を作成する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="colorByte"></param>
        /// <returns></returns>
        private byte[] CreateDataArray(WriteableBitmap target, int colorByte)
        {
            if (target == null)
            {
                return null;
            }
            else
            {
                return CreateDataArray(target.PixelWidth, target.PixelHeight, colorByte);
            }
        }

        /// <summary>
        /// 指定した画像の情報を代入可能な、空のbyte[]を作成する
        /// </summary>
        /// <param name="width">画像幅(pixel単位)</param>
        /// <param name="height">画像高さ(pixel単位)</param>
        /// <param name="colorByte">1pixelあたりの画素数</param>
        /// <returns></returns>
        private byte[] CreateDataArray(int width, int height, int colorByte)
        {
            return CreateDataArray(width * colorByte, height);
        }

        /// <summary>
        /// 指定した画像の情報を代入可能な、空のbyte[]を作成する
        /// </summary>
        /// <param name="stride">画像ストライド(幅*pixel)</param>
        /// <param name="height">画像高さ</param>
        /// <returns></returns>
        private byte[] CreateDataArray(int stride, int height)
        {

            return new byte[stride * height];
        }

        #endregion

        /// <summary>
        /// double型PointをInt型Pointに置換する(小数以下切り捨て)
        /// </summary>
        /// <param name="dblPt">System.Windows.Pointオブジェクト</param>
        /// <returns>System.Drawing.Pointオブジェクト</returns>
        private Point ConvertDblPtToIntPt(System.Windows.Point dblPt)
        {
            Point intPt = new Point();

            intPt.X = (int)dblPt.X;
            intPt.Y = (int)dblPt.Y;

            return intPt;
        }

        #region ConversionDMPtoDPI

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

        #endregion

        /// <summary>
        /// BMPのバイナリデータからWriteableBitmapデータを作成する
        /// </summary>
        /// <param name="imageParameter">各データを入れ込んだエンティティ</param>
        /// <returns>WriteableBitampオブジェクト</returns>
        private WriteableBitmap CreateWBMP(ImageParameter imageParameter)
        {
            if (imageParameter == null)
            {
                return null;
            }

            // ストライド(画像幅*pixel)
            int stride = imageParameter.ImageWidth * imageParameter.Pixel / 8;  // bitからbyteに直すため

            // bmpはストライドが必ず4の倍数になっている(0埋めされている)
            // ※pixelはRGBで3固定なので、画像幅が4の倍数でなければ必ず補正されている
            // 1行あたりの補正されたバイト数
            int additional = imageParameter.ImageWidth % BMP_WIDTH_BOUNDARY;
            stride += additional;

            // 補正したストライドを使用する
            // データは最小でも255あるので補正しなければならない
            imageParameter.BinaryData = CorrectBMPDataArray(imageParameter.BinaryData, stride, imageParameter.ImageHeight);

            if (imageParameter.BinaryData == null || imageParameter.BinaryData.Any() == false)
            {
                return null;
            }

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                imageParameter.ImageWidth,
                imageParameter.ImageHeight,
                imageParameter.HorizontalResolutionDPI,
                imageParameter.VerticalResolutionDPI,
                // System.Windows.Media.PixelFormats.Gray8, // グレースケール用
                // System.Windows.Media.PixelFormats.Bgr32, // RGB用
                System.Windows.Media.PixelFormats.Pbgra32, // BGRA32用
                null // indexつきのbmp以外はnullで良い
                );

            // バイナリ配列データを用意する
            // 1次元配列の中で行列が区別される            
            // BMPの画像幅補正は含まなくて良いので、縦*横*BGRAが必要なバイト数
            // がWBMPへ書き込むデータ配列の要素数として必要
            byte[] dataArr = CreateDataArray(imageParameter.ImageWidth, imageParameter.ImageHeight, COLORBYTE_RGBA);

            if (dataArr == null || dataArr.Any() == false)
            {
                return null;
            }

            // 元データの最下行→そのひとつ上の行の順で、WBMPの上段に代入していく
            // Alpha値の要素を定数255にするカウンタ
            int alphaCount = 0;
            // WBMPへ代入する際のデータ行が何行目かを見るカウンタ
            int row = 0;

            // 元のデータ配列の最下行の先頭からスタートし、1行ずつ上にいく
            for (int i = imageParameter.BinaryData.Length - stride; i >= 0; i -= stride)
            {
                // BMPの境界の0埋めを判定するためのカウンタ
                int boundCount = 0;

                // 当該行の最初の要素から最後の要素まで読み取りを行う
                for (int j = i; boundCount != imageParameter.ImageWidth; j += imageParameter.Pixel / 8)
                {
                    // BMPはBGR, WBMPはBGRAの順に画素を並べる必要があるのでAの分のズレを考慮して代入していく
                    // WBMPは1行目から順に値を入れ込んでいく
                    dataArr[row + alphaCount] = imageParameter.BinaryData[j];
                    dataArr[row + 1 + alphaCount] = imageParameter.BinaryData[j + 1];
                    dataArr[row + 2 + alphaCount] = imageParameter.BinaryData[j + 2];
                    dataArr[row + 3 + alphaCount] = DEFAULT_ALPHA;
                    alphaCount += COLORBYTE_RGBA;

                    // カウンタをインクリメント
                    boundCount++;

                    // もし今読み込んでいる画素の先頭(BGRのBが格納されている位置)が行の最後なら次の行へ向かう
                }

                // 行頭に戻るのでAlpha値を入れる位置をリセットする
                alphaCount = 0;
                // 作成しているWBMPデータの次行となる配列の要素がどこから始まるかを出しておく
                row += imageParameter.ImageWidth * COLORBYTE_RGBA;
            }

            // ImageParameterを更新する
            imageParameter.BinaryData = dataArr;

            // wbmpに書き込む
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, imageParameter.ImageWidth, imageParameter.ImageHeight),
            dataArr,
            imageParameter.ImageWidth * COLORBYTE_RGBA, // ストライド：行の要素数*色pixel数
            0
            );
            return wbmp;
        }

        /// <summary>
        /// Data配列を補正する
        /// </summary>
        /// <param name="dataArr">現在のデータ配列</param>
        /// <param name="stride">ストライド</param>
        /// <param name="height">高さ</param>
        /// <returns>補正後のデータ配列</returns>
        private byte[] CorrectBMPDataArray(byte[] dataArr, int stride, int height)
        {
            if (dataArr == null)
            {
                // nullは異常
                return null;
            }
            // サイズが等しいなら何もしないでそのまま返す
            else if (dataArr.Length == stride * height)
            {
                return dataArr;
            }

            // ストライド*高さで作成する
            byte[] newDataArr = CreateDataArray(stride, height);

            // 旧データを入れ込んで返す
            for (int i = 0; i < newDataArr.Length; i++)
            {
                newDataArr[i] = dataArr[i];

                // 無いとは思うが、元配列を超えてしまうなら外に出る
                if (dataArr.Length <= i + 1)
                {
                    break;
                }
            }

            return newDataArr;
        }
    }
}
