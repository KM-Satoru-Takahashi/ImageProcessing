using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.ViewModel;
using ImageProcessing.Entities;
using System.Windows;

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
        /// ドロップ操作を許可するファイルの拡張子
        /// </summary>
        private readonly string EXTENSION = @".bmp";

        #endregion

        #region method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="vm">ViewModel</param>
        internal MainWindowModel(MainWindowViewModel vm)
        {
            _vm = vm;
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
            if(dataObj == null)
            {
                return result;
            }

            var files = dataObj.GetFileDropList().Cast<string>();

            // 指定の拡張子で終わっているファイルであればdropを許可
            result = files.Any(fname => fname.EndsWith(EXTENSION));

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
            if(dataObj == null)
            {
                return null;
            }

            // ドロップされたファイルのパスを取得
            var files = dataObj.GetFileDropList().Cast<string>().Where(fname => fname.EndsWith(EXTENSION)).ToList();
            if(files == null || files.Any() == false)
            {
                return null;
            }

            List<DropData> dropDataList = new List<DropData>();
            foreach (string path in files)
            {
                if(string.IsNullOrEmpty(path)==false)
                {
                    DropData dropData = new DropData(path);
                    dropDataList.Add(dropData);                    
                }
            }

            return dropDataList;
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
