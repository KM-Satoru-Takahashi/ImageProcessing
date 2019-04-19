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
        /// 画素コントロール部分表示文言管理クラス
        /// </summary>
        private PixelDataViewManager _pxlDataViewMng = null;

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

        /// <summary>
        /// 画像データ更新ボタンに対するコマンド
        /// </summary>
        private UpdateCommand _update = null;

        /// <summary>
        /// 画像クリック時に表示される画像情報
        /// </summary>
        private Entities.PixelData _pixelData = null;

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

        #region 画素値変更部分固定文言

        /// <summary>
        /// X座標
        /// </summary>
        public string XPosition
        {
            get
            {
                return _pxlDataViewMng.XPosition;
            }
        }

        /// <summary>
        /// Y座標
        /// </summary>
        public string YPosition
        {
            get
            {
                return _pxlDataViewMng.YPosition;
            }
        }

        /// <summary>
        /// 現在の画素値
        /// </summary>
        public string OldPixel
        {
            get
            {
                return _pxlDataViewMng.OldPixel;
            }
        }

        /// <summary>
        /// 新しい画素値
        /// </summary>
        public string NewPixel
        {
            get
            {
                return _pxlDataViewMng.NewPixel;
            }
        }

        /// <summary>
        /// "R"表示
        /// </summary>
        public string Red
        {
            get
            {
                return _pxlDataViewMng.Red;
            }
        }

        /// <summary>
        /// "G"表示
        /// </summary>
        public string Green
        {
            get
            {
                return _pxlDataViewMng.Green;
            }
        }

        /// <summary>
        /// "B"表示
        /// </summary>
        public string Blue
        {
            get
            {
                return _pxlDataViewMng.Blue;
            }
        }

        /// <summary>
        /// "A"表示
        /// </summary>
        public string Alpha
        {
            get
            {
                return _pxlDataViewMng.Alpha;
            }
        }

        /// <summary>
        /// "更新"表示
        /// </summary>
        public string Update
        {
            get
            {
                return _pxlDataViewMng.Update;
            }
        }


        #endregion


        public UpdateCommand UpdateCommand
        {
            get
            {
                return _update;
            }
            set
            {
                _update = value;
            }
        }

        #region 画素情報

        /// <summary>
        /// 画素のX座標
        /// </summary>
        public int XCoordinate
        {
            get
            {
                return _pixelData.XCoordinate;
            }
            set
            {
                _pixelData.XCoordinate = value;
                RaisePropertyChanged("XCoordinate");
            }
        }

        /// <summary>
        /// 画素のY座標
        /// </summary>
        public int YCoordinate
        {
            get
            {
                return _pixelData.YCoordinate;
            }
            set
            {
                _pixelData.YCoordinate = value;
                RaisePropertyChanged("YCoordinate");
            }
        }


        /// <summary>
        /// 現在のR値
        /// </summary>
        public byte OldRed
        {
            get
            {
                return _pixelData.OldRed;
            }
            set
            {
                _pixelData.OldRed = value;
                RaisePropertyChanged("OldRed");
            }
        }

        /// <summary>
        /// 現在のG値
        /// </summary>
        public byte OldGreen
        {
            get
            {
                return _pixelData.OldGreen;
            }
            set
            {
                _pixelData.OldGreen = value;
                RaisePropertyChanged("OldGreen");
            }
        }

        /// <summary>
        /// 現在のB値
        /// </summary>
        public byte OldBlue
        {
            get
            {
                return _pixelData.OldBlue;
            }
            set
            {
                _pixelData.OldBlue = value;
                RaisePropertyChanged("OldBlue");
            }
        }

        /// <summary>
        /// 現在のA値
        /// </summary>
        public byte OldAlpha
        {
            get
            {
                return _pixelData.OldAlpha;
            }
            set
            {
                _pixelData.OldAlpha = value;
                RaisePropertyChanged("OldAlpha");
            }
        }

        /// <summary>
        /// 更新後のR値
        /// </summary>
        public byte NewRed
        {
            get
            {
                return _pixelData.NewRed;
            }
            set
            {
                _pixelData.NewRed = value;
                RaisePropertyChanged("NewRed");
            }
        }

        /// <summary>
        /// 更新後のG値
        /// </summary>
        public byte NewGreen
        {
            get
            {
                return _pixelData.NewGreen;
            }
            set
            {
                _pixelData.NewGreen = value;
                RaisePropertyChanged("NewGreen");
            }
        }

        /// <summary>
        /// 更新後のB値
        /// </summary>
        public byte NewBlue
        {
            get
            {
                return _pixelData.NewBlue;
            }
            set
            {
                _pixelData.NewBlue = value;
                RaisePropertyChanged("NewBlue");
            }
        }

        /// <summary>
        /// 更新後のA値
        /// </summary>
        public byte NewAlpha
        {
            get
            {
                return _pixelData.NewAlpha;
            }
            set
            {
                _pixelData.NewAlpha = value;
                RaisePropertyChanged("NewAlpha");
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
            _pxlDataViewMng = new PixelDataViewManager();
            _pixelData = new Entities.PixelData();
            _rightRotate = new RightRotateCommand(RightRotate, IsRightRotateEnabled);
            _leftRotate = new LeftRotateCommand(LeftRotate, IsLeftRotateEnabled);
            _update = new UpdateCommand(param => { UpdatePixelInfo(param); }, IsUpdateEnabled);
            _wbmp = new WriteableBitmapCommand(param => { ShowPixelInfo(param); }, IsWBMPEnabled);
        }

        #region 右回転ボタンの登録メソッド

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

        #region 左回転ボタンの登録メソッド

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

        #region WriteableBitmap表示ボタンのメソッド

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
        private void ShowPixelInfo(object sender)
        {
            // テスト段階なのでとりあえずリストの0番目を渡す
            _pixelData = _model.GetPixelInfo(_dropFiles[0], sender);

            // 異常時でもオブジェクトは返ってくるので値を代入する
            XCoordinate = _pixelData.XCoordinate;
            YCoordinate = _pixelData.YCoordinate;
            OldRed = _pixelData.OldRed;
            OldGreen = _pixelData.OldGreen;
            OldBlue = _pixelData.OldBlue;
            OldAlpha = _pixelData.OldAlpha;

        }

        #endregion

        #region 更新ボタン押下時の処理

        /// <summary>
        /// 画素情報が存在すればtrue
        /// </summary>
        private bool IsUpdateEnabled()
        {
            if (_pixelData == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新ボタン押下時の処理
        /// </summary>
        private void UpdatePixelInfo(object sender)
        {
            // テスト段階なのでとりあえずリストの先頭要素を更新する
            _dropFiles[0].ImageData = _model.UpdatePixelInfo(_dropFiles[0].ImageData, _pixelData);
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

        #endregion

    }
}
