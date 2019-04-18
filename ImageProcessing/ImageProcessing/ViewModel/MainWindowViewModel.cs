using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;
using ImageProcessing.Model;
using ImageProcessing.ViewModel.Command;

using System.Windows.Input;

namespace ImageProcessing.ViewModel
{
    /// <summary>
    /// メイン画面に対応するViewModel
    /// </summary>
    public class MainWindowViewModel : ViewModelBase, IDropTarget
    {
        #region filed

        /// <summary>
        /// model
        /// </summary>
        MainWindowModel _model = null;

        /// <summary>
        /// ボタン表示文言管理クラス
        /// </summary>
        private ButtonViewManager _viewManager = null;

        /// <summary>
        /// ドロップファイルエンティティ
        /// </summary>
        ObservableCollection<Entities.DropData> _dropFiles = null;

        /// <summary>
        /// 右回転ボタンコマンド
        /// </summary>
        private RightRotateCommand _rightRotate = null;

        /// <summary>
        ///  左回転ボタンコマンド
        /// </summary>
        private LeftRotateCommand _leftRotate = null;

        /// <summary>
        /// D&Dで表示したWBMPオブジェクト(button)に対するコマンド
        /// </summary>
        private WriteableBitmapCommand _wbmp = null;

        #endregion

        #region プロパティ

        #region 右回転ボタン

        /// <summary>
        /// 右回転ボタンコマンド
        /// </summary>
        public RightRotateCommand RightRotateCommand
        {
            get
            {
                return _rightRotate;
            }
            private set
            {
                _rightRotate = value;
            }
        }

        /// <summary>
        /// 右回転90度のボタン名
        /// </summary>
        public string RightRotate90ButtonName
        {
            get
            {
                return _viewManager.RightRotate90;
            }
        }

        #endregion

        #region 左回転ボタン

        /// <summary>
        /// 左回転ボタンコマンド
        /// </summary>
        public LeftRotateCommand LeftRotateCommand
        {
            get
            {
                return _leftRotate;
            }
            private set
            {
                _leftRotate = value;
            }
        }

        /// <summary>
        /// 左回転90度のボタン名
        /// </summary>
        public string LeftRotate90ButtonName
        {
            get
            {
                return _viewManager.LeftRotate90;
            }
        }

        #endregion

        #region WriteableBitmapオブジェクト表示ボタン

        /// <summary>
        /// WriteableBitmapオブジェクト表示ボタンコマンド
        /// </summary>
        public WriteableBitmapCommand WriteableBitmapCommand
        {
            get
            {
                return _wbmp;
            }
            private set
            {
                _wbmp = value;
            }
        }

        #endregion

        /// <summary>
        /// ドロップされたオブジェクトを保存する
        /// </summary>
        public ObservableCollection<Entities.DropData> Files
        {
            get
            {
                return _dropFiles;
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>アクセスは初回実行時publicでないと実行時エラー</remarks>
        public MainWindowViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            _dropFiles = new ObservableCollection<Entities.DropData>();
            _model = new MainWindowModel(this);
            _viewManager = new ButtonViewManager();
            _rightRotate = new RightRotateCommand(RightRotate, IsRightRotateEnabled);
            _leftRotate = new LeftRotateCommand(LeftRotate, IsLeftRotateEnabled);
            _wbmp = new WriteableBitmapCommand(WBMPImageProcessing, IsWBMPEnabled);
        }

        #region 右回転ボタンのデリゲート登録メソッド

        /// <summary>
        /// 右回転ボタン押下可否状態判定
        /// </summary>
        /// <returns></returns>
        private bool IsRightRotateEnabled()
        {
            if (_viewManager != null)
            {
                return _viewManager.IsRightRotateButtonEnabled;
            }

            // 異常時false
            return false;
        }

        /// <summary>
        /// 右回転ボタン押下時の処理
        /// </summary>
        private void RightRotate()
        {
            // テスト段階なのでとりあえずリストの0番目を渡す
            _dropFiles[0].ImageData = _model.RightRotate(_dropFiles[0]);
        }

        #endregion

        #region 左回転ボタンのデリゲート登録メソッド

        /// <summary>
        /// 左回転ボタン押下可否状態判定
        /// </summary>
        /// <returns></returns>
        private bool IsLeftRotateEnabled()
        {
            if (_viewManager != null)
            {
                return _viewManager.IsLeftRotateButtonEnabled;
            }

            // 異常時false
            return false;
        }

        /// <summary>
        /// 左回転ボタン押下時の処理
        /// </summary>
        private void LeftRotate()
        {

        }

        #endregion

        #region WriteableBitmap表示ボタンのデリゲート

        /// <summary>
        /// wbmp押下可否状態判定
        /// </summary>
        /// <returns>常にtrue</returns>
        private bool IsWBMPEnabled()
        {
            return true;
        }

        /// <summary>
        /// wbmpボタン押下時の処理
        /// </summary>
        private void WBMPImageProcessing()
        {
            // テスト段階なのでとりあえずリストの0番目を渡す
            _dropFiles[0].ImageData = _model.RightRotate(_dropFiles[0]);
        }

        #endregion

        #region ドラッグアンドドロップ時の処理

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

        #endregion

    }
}
