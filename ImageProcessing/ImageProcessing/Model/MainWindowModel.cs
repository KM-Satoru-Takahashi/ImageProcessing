﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.ViewModel;
using ImageProcessing.Entities;
using System.Windows;
using System.IO;

using System.Drawing;
using System.Windows.Media.Imaging;

using GongSolutions.Wpf.DragDrop;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageProcessing.Model
{
    /// <summary>
    /// メイン画面Model
    /// </summary>
    /// <remarks>バインディング関連クラス以外は全てここから呼ぶべき</remarks>
    internal class MainWindowModel
    {
        #region field

        /// <summary>
        /// ViewModel
        /// </summary>
        private MainWindowViewModel _vm = null;

        /// <summary>
        /// 画像管理クラス
        /// </summary>
        private ImageManager _imageManager = null;

        /// <summary>
        /// View上の情報取得クラス
        /// </summary>
        private ViewInfoAcquisition _viewInfoAcquisition = null;

        /// <summary>
        /// ドロップ操作を許可するファイルの拡張子
        /// </summary>
        private readonly string TARGET_EXTENSION = @".bmp";

        /// <summary>
        /// ドロップされたファイルの一覧から作成した画像オブジェクトを保存する
        /// </summary>
        private List<DropData> _dropDatas = null;

        #endregion

        #region method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="vm">ViewModel</param>
        internal MainWindowModel(MainWindowViewModel vm)
        {
            _vm = vm;
            _imageManager = new ImageManager(this);
            _viewInfoAcquisition = new ViewInfoAcquisition();
            _dropDatas = new List<DropData>();
        }

        /// <summary>
        /// マウスオーバーされたファイルによってドロップ可不可を判定する
        /// </summary>
        /// <param name="mouseOverFile"></param>
        /// <returns>true: DropOK, false: DropNG</returns>
        internal bool CanDropFiles(IDropInfo mouseOverFile)
        {
            bool result = false;

            DataObject dataObj = GetDataObject(mouseOverFile);
            if (dataObj == null)
            {
                return result;
            }

            var files = dataObj.GetFileDropList().Cast<string>();

            // 指定の拡張子で終わっているファイルであればdropを許可
            result = files.Any(fname => fname.ToLower().EndsWith(TARGET_EXTENSION));

            return result;
        }

        /// <summary>
        /// ドロップされたファイルの情報を得る
        /// </summary>
        /// <param name="dropFile"></param>
        /// <returns></returns>
        internal List<DropData> GetDropFileInfo(IDropInfo dropFile)
        {
            DataObject dataObj = GetDataObject(dropFile);
            if (dataObj == null)
            {
                return null;
            }

            // ドロップされたファイル全てのパスを一旦取得
            List<string> dropFileList = dataObj.GetFileDropList().Cast<string>().ToList();

            // 拡張子がTARGET_EXTENSIONと合致するもののみを抽出
            List<string> targetFileList = CreateTargetFilesPathList(dropFileList);

            // 対象ファイルパスから画像オブジェクトを生成して内部リストに保持する
            CreateDropData(targetFileList);

            // テストコード
            _dropDatas = _imageManager.GetBitmapDropData(targetFileList);

            return _dropDatas;
        }

        /// <summary>
        /// 画像右回転処理実行メソッド
        /// </summary>
        /// <returns>右回転した画像のWriteableBMP形式</returns>
        /// <remarks>処理の実体は画処理クラスに定義</remarks>
        internal DropData RightRotate(DropData dropData)
        {
            if (dropData == null)
            {
                return null;
            }

            if (_imageManager != null)
            {
                dropData.ImageData = _imageManager.RightRotate(dropData);
                // 縦横も変える必要がある
                byte[] oldWidth = new byte[4];
                // 旧幅をとっておく
                Array.Copy(dropData.ImageWidth, 0, oldWidth, 0, 4);
                // 旧高さを幅に代入
                Array.Copy(dropData.ImageHeight, 0, dropData.ImageWidth, 0, 4);
                // 旧幅を高さに代入
                Array.Copy(oldWidth, 0, dropData.ImageHeight, 0, 4);

                return dropData;
            }

            return null;
        }

        /// <summary>
        /// WBMP画像の押下された位置の画素情報を取得する
        /// </summary>
        /// <param name="dropData"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        internal PixelData GetPixelInfo(DropData dropData, object sender)
        {
            PixelData pixelData = new PixelData();

            if (_viewInfoAcquisition == null || dropData == null || sender == null)
            {
                return pixelData;
            }
            // ターゲット上のマウス座標を取得
            System.Windows.Point mousePoint = new System.Windows.Point();

            // 変換失敗時
            // Point(0,0)では判断できない
            if (_viewInfoAcquisition.GetMousePositionOnButton(sender, out mousePoint) == false)
            {
                return pixelData;
            }

            // 座標から該当する画素値情報を取得する
            pixelData = _imageManager.GetPixelInfo(dropData.ImageData, mousePoint);

            return pixelData;
        }

        /// <summary>
        /// 更新ボタン押下時に指示された画素情報を更新する
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        internal WriteableBitmap UpdatePixelInfo(WriteableBitmap updateData, PixelData pixelData)
        {
            // TODO: CheckDropdata みたいなメソッドを作ってupdateDataの正当性をここで担保してもいいかも
            // TODO: ImageManagerにわたす前に他のメソッドでも汎用的に使える可能性があるので
            return _imageManager.SetPixelInfo(updateData, pixelData);
        }

        /// <summary>
        /// 入力されたカラーコードから背景色を生成する
        /// </summary>
        /// <param name="sender">VMから渡されるパラメタ</param>
        /// <returns>生成した背景色, エラー時は白色</returns>
        internal System.Windows.Media.Brush CreateBackgroundColor(object sender)
        {
            if (sender == null)
            {
                return new SolidColorBrush(Colors.White);
            }

            string s = "";

            string stringR = s.Substring(0, 2);
            string stringG = s.Substring(2, 2);
            string stringB = s.Substring(4, 2);

            byte byteR = 0;
            byte byteG = 0;
            byte byteB = 0;

            if (byte.TryParse(stringR, System.Globalization.NumberStyles.HexNumber, null, out byteR) == false)
            {
                return new SolidColorBrush(Colors.White);
            }
            if (byte.TryParse(stringG, System.Globalization.NumberStyles.HexNumber, null, out byteG) == false)
            {
                return new SolidColorBrush(Colors.White);
            }
            if (byte.TryParse(stringB, System.Globalization.NumberStyles.HexNumber, null, out byteB) == false)
            {
                return new SolidColorBrush(Colors.White);
            }

            // 対応するRGBの作成
            System.Windows.Media.Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byteR, byteG, byteB));

            return brush;
        }
        
        /// <summary>
        /// 画像ファイルパスから対応する画像オブジェクトの一覧を作成する
        /// </summary>
        /// <param name="filePathList"></param>
        /// <returns></returns>
        private void CreateDropData(List<string> filePathList)
        {
            if (filePathList == null || filePathList.Any() == false)
            {
                return;
            }

            foreach (string targetPath in filePathList)
            {
                DropData dropData = new DropData(targetPath);
                _dropDatas.Add(dropData);
            }
        }

        /// <summary>
        /// ドロップされたファイルから対象のファイルパスのみを取得する
        /// </summary>
        /// <param name="files">ドロップされたファイル</param>
        private List<string> CreateTargetFilesPathList(List<string> files)
        {
            List<string> targetFilePathList = new List<string>();

            foreach (string filePath in files)
            {
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    string extension = Path.GetExtension(filePath).ToLower();
                    if (extension == TARGET_EXTENSION)
                    {
                        targetFilePathList.Add(filePath);
                    }
                }
            }

            return targetFilePathList;
        }

        /// <summary>
        /// IDropInfoをDataObjectに変換する
        /// </summary>
        /// <param name="dropInfo"></param>
        /// <returns></returns>
        private DataObject GetDataObject(IDropInfo dropInfo)
        {
            DataObject dataObj = null;

            dataObj = (DataObject)dropInfo.Data;

            return dataObj;
        }

        #endregion

    }
}
