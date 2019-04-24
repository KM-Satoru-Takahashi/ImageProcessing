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

using System.Windows.Media;
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
        private ImageProcessingRegionViewManager _imgProcessingViewMng = null;

        /// <summary>
        /// 画素コントロール部分表示文言管理クラス
        /// </summary>
        private PixelDataDisplayRegionViewManager _pxlDataViewMng = null;

        /// <summary>
        /// 背景色変更部分管理クラス
        /// </summary>
        private BackgroundChangeRegionViewManager _backChangeViewMng = null;

        /// <summary>
        /// ドロップファイルエンティティ
        /// </summary>
        ObservableCollection<Entities.DropData> _dropFiles = null;

        /// <summary>
        /// 右回転ボタンコマンド
        /// </summary>
        private RotateCommand _rotate = null;

        /// <summary>
        /// 反転ボタン押下時コマンド
        /// </summary>
        private FlipCommand _flip = null;

        /// <summary>
        /// 画像拡縮ボタン押下時コマンド
        /// </summary>
        private ScalingCommand _scaling = null;

        /// <summary>
        /// 背景色変更ボタン押下時コマンド
        /// </summary>
        private BackgroundChangeCommand _backgroundChange = null;

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

        /// <summary>
        /// 通常のボタンやテキストブロックの幅
        /// </summary>
        public int GeneralContentWidth { get; } = 75;

        /// <summary>
        /// 小さめのボタンやテキストブロックの幅
        /// </summary>
        public int SmallContetWidth { get; } = 40;

        /// <summary>
        /// 小さめのボタンやテキストブロックの高さ
        /// </summary>
        public int SmallContetHeight { get; } = 20;

        #region 画像処理領域

        #region 回転ボタン

        /// <summary>
        /// 回転ボタンコマンド
        /// </summary>
        public RotateCommand RotateCommand
        {
            get
            {
                return _rotate;
            }
            private set
            {
                _rotate = value;
            }
        }

        /// <summary>
        /// 右回転90度のボタン名
        /// </summary>
        public string RightRotate90ButtonName
        {
            get
            {
                return _imgProcessingViewMng.RightRotate90;
            }
        }

        /// <summary>
        /// 左回転90度のボタン名
        /// </summary>
        public string LeftRotate90ButtonName
        {
            get
            {
                return _imgProcessingViewMng.LeftRotate90;
            }
        }

        #endregion

        #region 反転ボタン

        /// <summary>
        /// 反転ボタン押下時のコマンド
        /// </summary>
        public FlipCommand FlipCommand
        {
            get
            {
                return _flip;
            }
            set
            {
                _flip = value;
            }
        }

        /// <summary>
        /// 反転ボタン名
        /// </summary>
        public string FlipButtonName
        {
            get
            {
                return _imgProcessingViewMng.Flip;
            }
        }

        #endregion

        #region 拡縮

        /// <summary>
        /// 拡縮ボタン押下時コマンド
        /// </summary>
        public ScalingCommand ScalingCommand
        {
            get
            {
                return _scaling;
            }
            set
            {
                _scaling = value;
            }
        }


        /// <summary>
        /// 最近傍法ボタン表示文言
        /// </summary>
        public string NearestNeighborButtonMessage
        {
            get
            {
                return _imgProcessingViewMng.NearestNeighborButtonMessage;
            }
        }

        /// <summary>
        /// 線形補間法ボタン表示文言
        /// </summary>
        public string BilinearButtonMessage
        {
            get
            {
                return _imgProcessingViewMng.BilinearButtonMessage;
            }
        }

        /// <summary>
        /// 幅(%)文言
        /// </summary>
        public string WidthPersentLabel
        {
            get
            {
                return _imgProcessingViewMng.WidthPercent;
            }
        }

        /// <summary>
        /// 高さ(%)文言
        /// </summary>
        public string HeightPercentLabel
        {
            get
            {
                return _imgProcessingViewMng.HeightPercent;
            }
        }

        /// <summary>
        /// 入力された幅
        /// </summary>
        public string WidthScale
        {
            get;
            set;
        }

        /// <summary>
        /// 入力された高さ
        /// </summary>
        public string HeightScale
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region 画素値更新ボタン関連

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
        public string RedLabel
        {
            get
            {
                return _pxlDataViewMng.RedLabel;
            }
        }

        /// <summary>
        /// "G"表示
        /// </summary>
        public string GreenLabel
        {
            get
            {
                return _pxlDataViewMng.GreenLabel;
            }
        }

        /// <summary>
        /// "B"表示
        /// </summary>
        public string BlueLabel
        {
            get
            {
                return _pxlDataViewMng.BlueLabel;
            }
        }

        /// <summary>
        /// "A"表示
        /// </summary>
        public string AlphaLabel
        {
            get
            {
                return _pxlDataViewMng.AlphaLabel;
            }
        }

        /// <summary>
        /// "更新"表示
        /// </summary>
        public string UpdateButtonMessage
        {
            get
            {
                return _pxlDataViewMng.UpdateButtonMessage;
            }
        }

        #endregion

        /// <summary>
        /// 更新ボタン押下時のコマンド
        /// </summary>
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

        #region 背景色変更関連

        /// <summary>
        /// 背景色変更ボタン表示文言
        /// </summary>
        public string BackgroundChangeButtonName
        {
            get
            {
                return _backChangeViewMng.BackgroundChangeButtonMessage;
            }
        }

        /// <summary>
        /// 背景色変更領域ラベル表示文言
        /// </summary>
        public string BackgroundChangeMessage
        {
            get
            {
                return _backChangeViewMng.BackgroundChangeMessage;
            }
        }

        /// <summary>
        /// カラーコード入力領域表示文言
        /// </summary>
        public string InputColorcodeMessage
        {
            get
            {
                return _backChangeViewMng.InputColorcodeMessage;
            }
        }

        /// <summary>
        /// 背景変更ボタン押下時コマンド
        /// </summary>
        public BackgroundChangeCommand BackgroundChangeCommand
        {
            get
            {
                return _backgroundChange;
            }
            private set
            {
                _backgroundChange = value;
            }
        }

        #region 色ボタン

        #region 黒

        public string BlackColorcode
        {
            get
            {
                return _backChangeViewMng.BlackColorcode;
            }
        }

        public Brush BlackColor
        {
            get
            {
                return _backChangeViewMng.BlackColor;
            }
        }

        #endregion

        #region 灰

        public string GrayColorcode
        {
            get
            {
                return _backChangeViewMng.GrayColorcode;
            }
        }

        public Brush GrayColor
        {
            get
            {
                return _backChangeViewMng.GrayColor;
            }
        }

        #endregion

        #region 薄灰

        public string LightgrayColorcode
        {
            get
            {
                return _backChangeViewMng.LightgrayColorcode;
            }
        }

        public Brush LightgrayColor
        {
            get
            {
                return _backChangeViewMng.LightgrayColor;
            }
        }

        #endregion

        #region 白

        public string WhiteColorcode
        {
            get
            {
                return _backChangeViewMng.WhiteColorcode;
            }
        }

        public Brush WhiteColor
        {
            get
            {
                return _backChangeViewMng.WhiteColor;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// 入力されたカラーコードに対応する背景色
        /// </summary>
        public Brush InputColorcodeColor
        {
            get
            {
                return _backChangeViewMng.InputColorcodeColor;
            }
            set
            {
                _backChangeViewMng.InputColorcodeColor = value;
                RaisePropertyChanged("InputColorcodeColor");
            }
        }

        /// <summary>
        /// 入力されたカラーコード
        /// </summary>
        public string InputColorcode
        {
            get
            {
                return _backChangeViewMng.InputColorcode;
            }
            set
            {
                _backChangeViewMng.InputColorcode = value;
                RaisePropertyChanged("InputColorcode");
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
            _imgProcessingViewMng = new ImageProcessingRegionViewManager();
            _pxlDataViewMng = new PixelDataDisplayRegionViewManager();
            _backChangeViewMng = new BackgroundChangeRegionViewManager();
            _pixelData = new Entities.PixelData();

            // コマンドの初期化処理
            CommandInitialize();
        }

        /// <summary>
        /// 各ボタンに対応するコマンドの初期化処理
        /// </summary>
        /// <remarks>長くなったのでInitializeから分離しただけ</remarks>
        private void CommandInitialize()
        {
            // Grid.Row = 1
            _rotate = new RotateCommand(Rotate, IsRotateEnabled);
            _flip = new FlipCommand(Flip, IsFlipEnabled);
            _scaling = new ScalingCommand(Scaling, IsScalingEnabled);

            // Grid.Row = 2
            _update = new UpdateCommand(UpdatePixelInfo, IsUpdateEnabled);

            // Grid.Row = 3
            // 表示した画像を押下した際の処理
            _wbmp = new WriteableBitmapCommand(param => { ShowPixelInfo(param); }, IsWBMPEnabled);

            // Grid.Row = 4
            // 背景変更ボタン(規定の色ボタンも含む)
            _backgroundChange = new BackgroundChangeCommand(BackgroundChange, IsBackgroundChangeEnabled);
        }

        #region 回転ボタンの登録メソッド

        /// <summary>
        /// 回転ボタン押下可否状態判定
        /// </summary>
        /// <returns></returns>
        private bool IsRotateEnabled()
        {
            if (_imgProcessingViewMng != null)
            {
                return _imgProcessingViewMng.IsRotateButtonEnabled;
            }

            // 異常時false
            return false;
        }

        /// <summary>
        /// 回転ボタン押下時の処理
        /// </summary>
        private void Rotate(object sender)
        {
            // ボタン自体を非活性にはできていないのでここで弾く
            if (_dropFiles == null || _dropFiles.Any() == false)
            {
                return;
            }

            // テスト段階なのでとりあえずリストの0番目を渡す
            Entities.DropData data = _model.Rotate(_dropFiles[0], sender);
            Files.Clear();
            Files.Add(data);
        }

        #endregion

        #region 反転ボタンのコマンド

        /// <summary>
        /// 反転ボタン押下可否
        /// </summary>
        /// <returns></returns>
        private bool IsFlipEnabled()
        {
            if (_imgProcessingViewMng != null)
            {
                return _imgProcessingViewMng.IsFlipButtonEnabled;
            }
            return false;
        }

        /// <summary>
        /// 反転ボタンのコマンド
        /// </summary>
        private void Flip()
        {
            // ボタン自体を非活性にはできていないのでここで弾く
            if (_dropFiles == null || _dropFiles.Any() == false)
            {
                return;
            }

            // テスト段階なのでとりあえずリストの0番目を渡す
            Entities.DropData data = _model.Flip(_dropFiles[0]);
            Files.Clear();
            Files.Add(data);
        }

        #endregion

        #region 画像拡縮ボタン関連

        /// <summary>
        /// 画像拡縮ボタン押下可否
        /// </summary>
        /// <returns></returns>
        private bool IsScalingEnabled()
        {
            if (_imgProcessingViewMng != null)
            {
                return _imgProcessingViewMng.IsScalingButtonEnabled;
            }
            return false;
        }

        /// <summary>
        /// 画像拡縮ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        private void Scaling(object sender)
        {
            // ボタン自体を非活性にはできていないのでここで弾く
            if (_dropFiles == null || _dropFiles.Any() == false)
            {
                return;
            }

            if (_model != null)
            {
                Entities.DropData data = Files[0];
                data.ImageData = _model.Scaling(_dropFiles[0], sender, WidthScale, HeightScale);
                Files.Clear();
                Files.Add(data);
            }
        }

        #endregion

        #region 背景色変更ボタン関連

        /// <summary>
        /// 背景色変更ボタン押下可否
        /// </summary>
        /// <returns></returns>
        private bool IsBackgroundChangeEnabled()
        {
            if (_backChangeViewMng != null)
            {
                return _backChangeViewMng.IsBackChangeEnabled;
            }
            return false;
        }

        /// <summary>
        /// 背景色変更ボタン押下時の処理
        /// </summary>
        private void BackgroundChange(object sender)
        {
            if (_model != null)
            {
                InputColorcodeColor = _model.CreateBackgroundColor(sender);
            }
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
            // _pixelDataに代入するとNew~が初期化されてしまうのでローカルで一旦保持して代入する
            Entities.PixelData pixelData = _model.GetPixelInfo(_dropFiles[0], sender);

            // 異常時でもオブジェクトは返ってくるので値を代入する
            XCoordinate = pixelData.XCoordinate;
            YCoordinate = pixelData.YCoordinate;
            OldRed = pixelData.OldRed;
            OldGreen = pixelData.OldGreen;
            OldBlue = pixelData.OldBlue;
            OldAlpha = pixelData.OldAlpha;

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
        private void UpdatePixelInfo()
        {
            // ボタン自体を非活性にはできていないのでここで弾く
            if (_dropFiles == null || _dropFiles.Any() == false)
            {
                return;
            }

            // テスト段階なのでとりあえずリストの先頭要素を更新する
            Entities.DropData data = _dropFiles[0];
            data.ImageData = _model.UpdatePixelInfo(_dropFiles[0].ImageParameter, _pixelData);
            Files.Clear();
            Files.Add(data);
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
            // debug: リスト要素を空にする
            Files.Clear();

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
