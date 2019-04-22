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
        /// バイナリデータの右回転処理
        /// </summary>
        /// <param name="dropData">回転対称となるデータ</param>
        /// <returns>右回転した画像データのWriteableBMP形式</returns>
        internal WriteableBitmap RightRotate(DropData dropData)
        {
            if (dropData == null)
            {
                return null;
            }

            // 縦横が入れ替わる
            int newWidth = BitConverter.ToInt32(dropData.ImageHeight, 0);
            int newHeight = BitConverter.ToInt32(dropData.ImageWidth, 0);

            // 現在表示されているWBMP画像のバイナリを取得する
            byte[] oldWBMPBinary = GetWBMPDataArray(dropData.ImageData, COLORBYTE_RGBA);
            if (oldWBMPBinary == null || oldWBMPBinary.Any() == false)
            {
                return null;
            }

            // 一時的にデータを保存しておくリストを用意する
            List<byte> tempList = new List<byte>();

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
                    tempList.Add(oldWBMPBinary[element]);       // B
                    tempList.Add(oldWBMPBinary[element + 1]);   // G
                    tempList.Add(oldWBMPBinary[element + 2]);   // R
                    tempList.Add(oldWBMPBinary[element + 3]);   // A
                }
            }

            // 新たなデータ配列を取得
            byte[] newImageData = tempList.ToArray();

            if (dropData.ImageData == null)
            {
                return null;
            }

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                newWidth,
                newHeight,
                dropData.HorizontalResolutionDPI,
                dropData.VerticalResolutionDPI,
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
        /// 表示されている画像データのバイナリ配列から要求された画素を返す
        /// </summary>
        /// <returns>画素情報オブジェクト</returns>
        /// <param name="targetImg">対象画像</param>
        /// <param name="doublePixelPoint">マウス座標</param>
        /// <remarks>RGBAの[n]~[n+2]における[n]の位置を返す。異常時は0</remarks>
        internal PixelData GetPixelInfo(WriteableBitmap targetImg, System.Windows.Point doublePixelPoint)
        {
            if (targetImg == null)
            {
                return null;
            }

            // WBMPの画像データを取得する
            byte[] dataArr = GetWBMPDataArray(targetImg, COLORBYTE_RGBA);

            if (dataArr == null || dataArr.Any() == false)
            {
                return null;
            }

            // マウス座標をintに変換する
            Point intPixelPoint = ConvertDblPtToIntPt(doublePixelPoint);

            // 画像のバイナリデータ内での位置を取得する
            int pixelPos = GetPixelPosition(intPixelPoint.X, intPixelPoint.Y, targetImg.PixelWidth, COLORBYTE_RGBA);

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
        /// <param name="target">更新対象のWBMP</param>
        /// <param name="updateData">更新したい画素情報</param>
        /// <returns>更新したWBMP</returns>
        internal WriteableBitmap SetPixelInfo(WriteableBitmap target, PixelData updateData)
        {
            if (target == null || updateData == null)
            {
                return null;
            }

            // WBMPの画像データを取得する
            byte[] dataArr = GetWBMPDataArray(target, COLORBYTE_RGBA);

            if (dataArr == null || dataArr.Any() == false)
            {
                return null;
            }

            // 画像のバイナリデータ内での位置を取得する
            int pixelPos = GetPixelPosition(updateData.XCoordinate, updateData.YCoordinate, target.PixelWidth, COLORBYTE_RGBA);

            return UpdateWBMPPixel(target, dataArr, pixelPos, updateData);
        }

        /// <summary>
        /// 与えられたBMPオブジェクトを指定された画素データで更新する
        /// </summary>
        /// <param name="targetWBMP">更新対象のWBMPデータ</param> 
        /// <param name="targetArr">更新対象のWBMPデータ配列</param>
        /// <param name="pixelPosition">更新開始バイナリ位置</param>
        /// <param name="pixelData">更新データ</param>
        /// <returns>更新したWBMPオブジェクト</returns>
        private WriteableBitmap UpdateWBMPPixel(WriteableBitmap targetWBMP, byte[] targetArr, int pixelPosition, PixelData pixelData)
        {
            // 引数オブジェクトのnullチェック
            if (targetWBMP == null || pixelData == null)
            {
                return null;
            }
            // 引数配列のnullチェック
            else if (targetArr == null || targetArr.Any() == false)
            {
                return null;
            }
            // 画像の領域を超えていれば異常
            // BGRAであれば、BGRAのBの位置を取得しているのでAの位置まであるかをチェックする必要がある
            else if (targetArr.Length < pixelPosition + COLORBYTE_RGBA - 1)
            {
                return null;
            }

            // 値の埋め込み
            targetArr[pixelPosition] = pixelData.NewBlue;       // B
            targetArr[pixelPosition + 1] = pixelData.NewGreen;  // G
            targetArr[pixelPosition + 2] = pixelData.NewRed;    // R
            targetArr[pixelPosition + 3] = pixelData.NewAlpha;  // A

            // wbmpに書き込む
            targetWBMP.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, targetWBMP.PixelWidth, targetWBMP.PixelHeight),
            targetArr,
            targetWBMP.PixelWidth * COLORBYTE_RGBA,
            0
            );

            return targetWBMP;
        }


        /// <summary>
        /// 渡されたWBMPオブジェクトのバイナリデータを取得する
        /// </summary>
        /// <param name="target">バイナリデータを取得したいWBMP</param>
        /// <returns>WBMPのバイナリデータ</returns>
        private byte[] GetWBMPDataArray(WriteableBitmap target, int colorByte)
        {
            // 現在表示されている画像の縦横を取得
            int wbmpWidth = target.PixelWidth;
            int wbmpHeight = target.PixelHeight;

            // wbmpの画像データを取得するための配列を用意する
            byte[] dataArr = CreateDataArray(wbmpWidth, wbmpHeight, colorByte);

            // wbmpの画像情報を取得する
            target.CopyPixels(dataArr, wbmpWidth * colorByte, 0);

            return dataArr;
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
            if (bmpData == null)
            {
                return null;
            }

            // 画像幅
            int width = BitConverter.ToInt32(bmpData.ImageWidth, 0);
            // 画像高さ
            int height = BitConverter.ToInt32(bmpData.ImageHeight, 0);
            // ピクセル(byte)
            int pixel = (BitConverter.ToInt16(bmpData.Pixel, 0)) / 8;
            // ストライド(画像幅*pixel)
            int stride = width * pixel;

            // bmpはストライドが必ず4の倍数になっている(0埋めされている)
            // ※pixelはRGBで3固定なので、画像幅が4の倍数でなければ必ず補正されている
            // 1行あたりの補正されたバイト数
            int additional = width % BMP_WIDTH_BOUNDARY;
            stride += additional;

            // 補正したストライドを使用する
            // データは最小でも255あるので補正しなければならない
            bmpData.Data = CorrectBMPDataArray(bmpData.Data, stride, height);

            if (bmpData.Data == null || bmpData.Data.Any() == false)
            {
                return null;
            }

            // WBMPオブジェクトを用意する
            WriteableBitmap wbmp = new WriteableBitmap
                (
                width,
                height,
                bmpData.HorizontalResolutionDPI,
                bmpData.VerticalResolutionDPI,
                // System.Windows.Media.PixelFormats.Gray8, // グレースケール用
                // System.Windows.Media.PixelFormats.Bgr32, // RGB用
                System.Windows.Media.PixelFormats.Pbgra32, // BGRA32用
                null // indexつきのbmp以外はnullで良い
                );

            // バイナリ配列データを用意する
            // 1次元配列の中で行列が区別される            
            // BMPの画像幅補正は含まなくて良いので、縦*横*BGRAが必要なバイト数
            // がWBMPへ書き込むデータ配列の要素数として必要
            byte[] dataArr = CreateDataArray(width, height, COLORBYTE_RGBA);

            if (dataArr == null || dataArr.Any() == false)
            {
                return null;
            }

            // 元データの最下行→そのひとつ上の行の順で、WBMPの上段に代入していく
            // Alpha値の要素を定数255にするカウンタ
            int alphaCount = 0;
            // WBMPへ代入する際のデータ行が何行目かを見るカウンタ
            int row = 0;

            // 元のデータ配列の最下行からスタートし、1行ずつ上にいく
            for (int i = bmpData.Data.Length - stride; i >= 0; i -= stride)
            {
                // BMPの境界の0埋めを判定するためのカウンタ
                int boundCount = 0;

                // 当該行の最初の要素から最後の要素まで読み取りを行う
                for (int j = i; boundCount != width; j += pixel)
                {
                    // BMPはBGR, WBMPはBGRAの順に画素を並べる必要があるのでAの分のズレを考慮して代入していく
                    // WBMPは1行目から順に値を入れ込んでいく
                    dataArr[row + alphaCount] = bmpData.Data[j];
                    dataArr[row + 1 + alphaCount] = bmpData.Data[j + 1];
                    dataArr[row + 2 + alphaCount] = bmpData.Data[j + 2];
                    dataArr[row + 3 + alphaCount] = DEFAULT_ALPHA;
                    alphaCount += COLORBYTE_RGBA;

                    // カウンタをインクリメント
                    boundCount++;

                    // もし今読み込んでいる画素の先頭(BGRのBが格納されている位置)が行の最後なら次の行へ向かう
                }

                // 行頭に戻るのでAlpha値を入れる位置をリセットする
                alphaCount = 0;
                // 作成しているWBMPデータの次行となる配列の要素がどこから始まるかを出しておく
                row += width * COLORBYTE_RGBA;
            }

            // wbmpに書き込む
            wbmp.WritePixels
            (
            new System.Windows.Int32Rect(0, 0, width, height),
            dataArr,
            width * COLORBYTE_RGBA, // ストライド：行の要素数*色pixel数
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
