using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.ViewModel;
using ImageProcessing.Entities;
using System.Windows;
using System.IO;

using GongSolutions.Wpf.DragDrop;


namespace ImageProcessing.Model
{
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
            result = files.Any(fname => fname.EndsWith(TARGET_EXTENSION));

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

            return _dropDatas;
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
