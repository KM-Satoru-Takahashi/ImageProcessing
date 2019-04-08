using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BMPOpen
{
    internal class BitMapBinary
    {
        /// <summary>
        /// BMPファイルのFileHeader部分
        /// </summary>
        /// <remarks>14Bytes</remarks>
        public struct BitMapFileHeader
        {
            /// <summary>
            /// BitMapファイルタイプ
            /// </summary>
            /// <remarks>2Bytes</remarks>
            public UInt16 bmpFileType;

            /// <summary>
            /// BitMapファイル全体のサイズ
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public UInt32 bmpFileSize;

            /// <summary>
            /// BitMapファイル予約領域1
            /// </summary>
            /// <remarks>2Bytes 予約領域なので値を操作しない</remarks>
            public UInt16 bmpReservedRegion1;

            /// <summary>
            /// BitMapファイル予約領域2
            /// </summary>
            /// <remarks>2Bytes 予約領域なので値を操作しない</remarks>
            public UInt16 bmpReservedRegion2;

            /// <summary>
            /// ファイルの先頭から画像データファイルまでのオフセット数
            /// </summary>
            /// <remarks>4Bytes バイト数であらわす</remarks>
            public UInt32 bmpOffsetBits;
        }

        /// <summary>
        /// BMPファイルのInfoHeader部分
        /// </summary>
        public struct BitMapInfoHeader
        {
            /// <summary>
            /// BMPファイルのInfoHeader部分ファイルサイズ
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public UInt32 bmpInfoSize;

            /// <summary>
            /// BMPファイルの幅
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public Int32 bmpWidth;

            /// <summary>
            /// BMPファイルの高さ
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public Int32 bmpHeight;

            /// <summary>
            /// BMPファイルのプレーン数
            /// </summary>
            /// <remarks>2Bytes 常に1</remarks>
            public UInt16 bmpPlains;

            /// <summary>
            /// 1pxあたりのビット数
            /// </summary>
            /// <remarks>2Bytes 1, 4, 8, 16, 24, 32のいずれか</remarks>
            public UInt16 bmpBitCount;

            /// <summary>
            /// BMPファイルの圧縮形式
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public UInt32 bmpCompression;

            /// <summary>
            /// BMPファイルのイメージサイズ
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public UInt32 bmpImageSize;

            /// <summary>
            /// BMPファイルの水平方向の解像度
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public Int32 bmpXResolution;

            /// <summary>
            /// BMPファイルの垂直方向の解像度
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public Int32 bmpYResolution;

            /// <summary>
            /// カラーパレット数
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public UInt32 bmpColorPalette;

            /// <summary>
            /// 重要なカラーパレットのインデックス
            /// </summary>
            /// <remarks>4Bytes</remarks>
            public UInt32 bmpImportantColor;
        }

        /// <summary>
        /// BMPファイルをバイナリ形式で開く
        /// </summary>
        /// <param name="filePath">BMPファイル名(*.bmp)</param>
        /// <param name="bmpFH">BMPファイルのFileHeader</param>
        /// <param name="bmpIH">BMPファイルのInfoHeader</param>
        /// <param name="ColorPalette">カラーパレット</param>
        /// <param name="bitData">画像のデータ（輝度値）</param>
        /// <returns></returns>
        internal bool Load
            (
            string filePath,
            out BitMapFileHeader bmpFH,
            out BitMapInfoHeader bmpIH,
            out System.Drawing.Color[] colorPalette,
            out Byte[] bitData
            )
        {
            // outオブジェクトを初期化しておく
            bmpFH = new BitMapFileHeader();
            bmpIH = new BitMapInfoHeader();
            // colorPaletteとbitDataは読み込んだ値から初期化するのでnullを割り当てておく
            colorPalette = null;
            bitData = null;

            // TODO: 拡張子チェックはドロップ時に行えば良いから後で消しておく

            // 拡張子を確認する
            // ファイルの拡張子を取得して小文字に
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension != ".bmp")
            {
                return false;
            }

            // データ読み込み用配列の用意
            byte[] readData = new byte[4];

            // ファイルを開く用意
            FileStream fs = null;
            try
            {
                // ファイルを開く
                fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                return false;
            }

            if (fs == null)
            {
                return false;
            }

            // TODO: fs.readで一気に読み取ってから分配したほうが良いかも
            // そうすることでfsを引数に渡さずにConvert/割付ができる

            #region BitmapFileHeaderの読み込み

            // todo: Readの第2引数が0はおかしいから修正する必要あるかも
            // bmpFileType
            fs.Read(readData, 0, 2);
            bmpFH.bmpFileType = BitConverter.ToUInt16(readData, 0);

            // bmpFileSize
            fs.Read(readData, 0, 4);
            bmpFH.bmpFileSize = BitConverter.ToUInt32(readData, 0);

            // bmpReserved1
            fs.Read(readData, 0, 2);
            bmpFH.bmpReservedRegion1 = BitConverter.ToUInt16(readData, 0);

            // bmpReserved2
            fs.Read(readData, 0, 2);
            bmpFH.bmpReservedRegion2 = BitConverter.ToUInt16(readData, 0);

            // bmpOffset
            fs.Read(readData, 0, 4);
            bmpFH.bmpOffsetBits = BitConverter.ToUInt32(readData, 0);

            #endregion

            #region BitmapInfoHeaderの読み込み

            // TODO: Readの第2引数が0はおかしいから修正する必要あるかも
            // bmpInfoSize
            fs.Read(readData, 0, 4);
            bmpIH.bmpInfoSize = BitConverter.ToUInt32(readData, 0);

            // bmpWidth
            fs.Read(readData, 0, 4);
            bmpIH.bmpWidth = BitConverter.ToInt32(readData, 0);

            // bmpHeight
            fs.Read(readData, 0, 4);
            bmpIH.bmpHeight = BitConverter.ToInt32(readData, 0);

            // bmpPlains
            fs.Read(readData, 0, 2);
            bmpIH.bmpPlains = BitConverter.ToUInt16(readData, 0);

            // bmpBitCount
            fs.Read(readData, 0, 2);
            bmpIH.bmpBitCount = BitConverter.ToUInt16(readData, 0);

            // bmpCompression
            fs.Read(readData, 0, 4);
            bmpIH.bmpCompression = BitConverter.ToUInt32(readData, 0);

            // bmpImageSize
            fs.Read(readData, 0, 4);
            bmpIH.bmpImageSize = BitConverter.ToUInt32(readData, 0);

            // bmpXResolution
            fs.Read(readData, 0, 4);
            bmpIH.bmpXResolution = BitConverter.ToInt32(readData, 0);

            // bmpYResolution
            fs.Read(readData, 0, 4);
            bmpIH.bmpYResolution = BitConverter.ToInt32(readData, 0);

            // bmpColorPalette
            fs.Read(readData, 0, 4);
            bmpIH.bmpColorPalette = BitConverter.ToUInt32(readData, 0);

            // bmpImportantColor
            fs.Read(readData, 0, 4);
            bmpIH.bmpImportantColor = BitConverter.ToUInt32(readData, 0);


            #endregion

            #region カラーパレットの取得

            // カラーパレットの計算を行う
            // bmpOffsetからFHとIHのを引いたものがカラーパレットのサイズ
            // byteにするので4で割る
            long colorPaletteSize = (bmpFH.bmpOffsetBits - 14 - 40) / 4;

            // カラーパレットに得た値を埋め込む
            // リトルエンディアンなので逆順に入れ込む必要あり
            if (colorPaletteSize != 0)
            {
                colorPalette = new System.Drawing.Color[colorPaletteSize];
                for (int i = 0; i < colorPaletteSize; i++)
                {
                    // TODO: Readの第2引数が0はおかしいから修正する必要あるかも
                    fs.Read(readData, 0, 4);
                    colorPalette[i] =
                        System.Drawing.Color.FromArgb(
                                // リトルエンディアンなので逆順に入れ込む必要あり
                                readData[3],
                                readData[2],
                                readData[1],
                                readData[0]);
                }
            }
            else
            {
                // colorPalette = null;
            }

            #endregion

            #region 画像データの取得

            // 画像データを取得する
            // 画像データの幅バイト数を計算する
            int imageWidthBits = ((bmpIH.bmpWidth * bmpIH.bmpBitCount + 31) / 32) * 4;

            // 読み込み時のメモリを確保する
            bitData = new byte[imageWidthBits * bmpIH.bmpHeight];

            // 画像データを読み込む
            for (int i = 0; i < bmpIH.bmpHeight - 1; i++)
            {
                fs.Read(bitData, i * imageWidthBits, i);
            }

            #endregion

            #region 開放処理

            // リソースを開放する
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }

            return true;

            #endregion
        }

        /// <summary>
        /// バイナリデータを.bmpとして保存する
        /// </summary>
        /// <param name="filePath">保存先ファイルパス(*.bmp)</param>
        /// <param name="width">bmpファイルの幅</param>
        /// <param name="height">bmpファイルの高さ</param>
        /// <param name="bitCount">bmpファイルのビット数</param>
        /// <param name="bitData">画像データ数</param>
        /// <returns></returns>
        internal bool Save
            (
            string filePath,
            int width,
            int height,
            int bitCount,
            byte[] bitData
            )
        {
            #region 引数の正常性を確認
            // 引数チェック
            // ファイルパス
            if (string.IsNullOrEmpty(filePath) == true)
            {
                return false;
            }
            // int関連
            else if (width < 1 || height < 1 || bitCount < 0)
            {
                return false;
            }
            // bitData
            else if (bitData == null || bitData.Any() == false)
            {
                return false;
            }

            // *.bmpでなければ保存しない
            if (Path.GetExtension(filePath).ToLower() != ".bmp")
            {
                return false;
            }

            #endregion

            // 画像データの幅バイト数を計算
            int imageWidthBytes = ((width * height + 31) / 32) * 4;

            // 画像データのサイズを確認する
            if (bitData.Length != imageWidthBytes * height)
            {
                return false;
            }

            // カラーパレット数を入手
            UInt32 paletteSize = 0;
            byte[] colorPalette = null;

            if (bitCount == 8)
            {
                // 8bit
                paletteSize = 256;
                colorPalette = new byte[paletteSize * 4];
                for (int i = 0; i < 256; i++)
                {
                    colorPalette[i * 4] = (byte)i; // B
                    colorPalette[i * 4 + 1] = (byte)i; // G
                    colorPalette[i * 4 + 2] = (byte)i; // R
                    colorPalette[i * 4 + 3] = (byte)0; // A
                }
            }
            else
            {
                // monochromeのとき？
            }

            #region bitmapFileHeaderの作成

            // bitmapFileHeaderの作成
            BitMapFileHeader bmpFH = new BitMapFileHeader();
            // TODO: 定数化、情報追加
            bmpFH.bmpFileType = 0x4d42;
            bmpFH.bmpReservedRegion1 = 0;
            bmpFH.bmpReservedRegion2 = 0;
            // TODO: MORE INFO
            bmpFH.bmpOffsetBits = 14 + 40 + paletteSize * 4;
            bmpFH.bmpFileSize = bmpFH.bmpOffsetBits + (uint)(imageWidthBytes * height);

            #endregion

            #region bitmapFielInfoの作成

            BitMapInfoHeader bmpFI = new BitMapInfoHeader();
            bmpFI.bmpInfoSize = 40;
            bmpFI.bmpWidth = width;
            bmpFI.bmpHeight = height;
            bmpFI.bmpPlains = 1;
            bmpFI.bmpBitCount = (ushort)bitCount;
            bmpFI.bmpCompression = 0;
            bmpFI.bmpImageSize = 0;
            bmpFI.bmpXResolution = 0;
            bmpFI.bmpYResolution = 0;
            bmpFI.bmpColorPalette = paletteSize;
            bmpFI.bmpImportantColor = paletteSize;

            #endregion


            // ファイルを開く
            FileStream fs = null;
            try
            {
                fs = File.Open(filePath, FileMode.Create, FileAccess.Write);
            }
            catch (Exception e)
            {
                // error manage
            }

            if (fs == null)
            {
                return false;
            }

            #region FileHeaderの書き込み

            // TODO: 第2引数がおかしいから修正する必要あるかも
            // bmpFHの書き込み
            fs.Write(BitConverter.GetBytes(bmpFH.bmpFileType), 0, 2);
            fs.Write(BitConverter.GetBytes(bmpFH.bmpFileSize), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFH.bmpReservedRegion1), 0, 2);
            fs.Write(BitConverter.GetBytes(bmpFH.bmpReservedRegion2), 0, 2);
            fs.Write(BitConverter.GetBytes(bmpFH.bmpOffsetBits), 0, 4);

            #endregion

            #region FileInfoの書き込み

            // TODO: 第2引数がおかしいから修正する必要あるかも
            // bmpFIの書き込み
            fs.Write(BitConverter.GetBytes(bmpFI.bmpImageSize), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpWidth), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpHeight), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpPlains), 0, 2);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpBitCount), 0, 2);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpCompression), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpImageSize), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpXResolution), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpYResolution), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpColorPalette), 0, 4);
            fs.Write(BitConverter.GetBytes(bmpFI.bmpImportantColor), 0, 4);

            #endregion

            #region カラーパレットの書き込み

            // カラーパレットの書き込み
            if (paletteSize != 0)
            {
                fs.Write(colorPalette, 0, colorPalette.Length);
            }

            #endregion

            #region 画像データの書き込み

            // 画像データの書き込み
            for (int i = 0; i < bmpFI.bmpHeight - 1; i++)
            {
                fs.Write(bitData, i * imageWidthBytes, imageWidthBytes);
            }

            #endregion

            #region 開放処理

            if (fs != null)
            {
                // dispose
                fs.Close();
                fs.Dispose();
            }

            #endregion

            return true;
        }

    }
}
