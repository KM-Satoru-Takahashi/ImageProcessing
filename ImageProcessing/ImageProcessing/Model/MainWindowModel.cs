using System;
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
        internal WriteableBitmap RightRotate(DropData dropData)
        {
            if (_imageManager != null)
            {
                return _imageManager.RightRotate(dropData);
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
            // マウス座標取得
            DependencyObject dependencyObject = (DependencyObject)sender;
            System.Windows.Controls.Button button = dependencyObject as System.Windows.Controls.Button;
            // クリックしたボタン上のマウス座標を取得する
            System.Windows.Point pt = Mouse.GetPosition(button);


            // マウスの座標取得
            _viewInfoAcquisition.GetMousePosition();

            // 座標から該当する画素値がどれかを出す
            _imageManager.GetTargetBinary();

            // その画素値の情報を作って返す
            _imageManager.CreateTargetPixelData();

            return null;
        }


        internal bool SetPixelInfo()
        {
            // 引数検討必要

            // imgManager側へ依頼する処理
            // ・バイナリのどの位置か
            // ・対象バイナリへ値のセット
            // ・セット後の画像データ構築

            return false;
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
                // WriteableBitmap処理を_imageManagerに委ねるならコンストラクタをいじる
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
