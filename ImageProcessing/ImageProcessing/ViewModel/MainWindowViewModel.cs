using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;
using ImageProcessing.Model;

namespace ImageProcessing.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDropTarget
    {
        #region filed

        /// <summary>
        /// model
        /// </summary>
        MainWindowModel _model = null;

        /// <summary>
        /// ドロップファイルエンティティ
        /// </summary>
        ObservableCollection<Entities.DropData> _dropFiles = null;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal MainWindowViewModel()
        {
            Initialize();
            _dropFiles = new ObservableCollection<Entities.DropData>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            _model = new MainWindowModel(this);

        }

        /// <summary>
        /// ドロップされたオブジェクトのパスを保存する
        /// </summary>
        //public ObservableCollection<string> Files { get; } = new ObservableCollection<string>();
        public ObservableCollection<Entities.DropData> Files
        {
            get
            {
                return _dropFiles;
            }
        }

        /// <summary>
        /// Window上にドラッグ状態でマウスオーバーされた際の処理
        /// </summary>
        /// <param name="dropInfo"></param>
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo == null)
            {
                return;
            }

            if (_model != null)
            {
                if (_model.CanDropFiles(dropInfo) == true)
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.None;
                }
            }
        }

        /// <summary>
        /// ドロップされた際の処理
        /// </summary>
        /// <param name="dropInfo"></param>
        public void Drop(IDropInfo dropInfo)
        {
            if (_model != null)
            {
                List<Entities.DropData> dropDataList = _model.GetDropFileInfo(dropInfo);
                if (dropDataList != null && dropDataList.Any() == true)
                {
                    foreach (var data in dropDataList)
                    {
                        Files.Add(data);
                    }
                }
            }
        }

        /// <summary>
        /// IDropInfo.DataをDataObjectに変換する
        /// </summary>
        /// <param name="dropInfo"></param>
        /// <returns></returns>
        private DataObject GetDataObject(IDropInfo dropInfo)
        {
            if (dropInfo != null)
            {
                return (DataObject)dropInfo.Data;
            }

            return null;
        }



    }
}
