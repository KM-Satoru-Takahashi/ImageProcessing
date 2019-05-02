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

        #region ドラッグアンドドロップ

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

            // とりあえずフィールドに入れ込んで返す
            _dropDatas = _imageManager.GetBitmapDropData(targetFileList);

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

        #region 回転

        /// <summary>
        /// 画像回転処理実行メソッド
        /// </summary>
        /// <returns>右回転した画像のWriteableBMP形式</returns>
        /// <remarks>処理の実体は画処理クラスに定義</remarks>
        internal DropData Rotate(DropData dropData, object sender)
        {
            if (dropData == null || sender == null)
            {
                return null;
            }

            RotateDirection rotate = (RotateDirection)sender;

            if (_imageManager != null)
            {
                if (rotate == RotateDirection.Right)
                {
                    dropData.ImageData = _imageManager.RightRotate(dropData.ImageParameter);
                }
                else if (rotate == RotateDirection.Left)
                {
                    dropData.ImageData = _imageManager.LeftRotate(dropData.ImageParameter);
                }
                else
                {
                    return dropData;
                }

                return dropData;
            }

            return null;
        }
        
        #endregion

        #region 画素取得と更新

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
            pixelData = _imageManager.GetPixelInfo(dropData.ImageParameter, mousePoint);

            return pixelData;
        }

        /// <summary>
        /// 更新ボタン押下時に指示された画素情報を更新する
        /// </summary>
        /// <param name="pixelData"></param>
        /// <returns></returns>
        internal WriteableBitmap UpdatePixelInfo(ImageParameter imageParameter, PixelData pixelData)
        {
            // TODO: CheckDropdata みたいなメソッドを作ってupdateDataの正当性をここで担保してもいいかも
            // TODO: ImageManagerにわたす前に他のメソッドでも汎用的に使える可能性があるので
            return _imageManager.SetPixelInfo(imageParameter, pixelData);
        }

        #endregion

        #region 背景色変更

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

            // 文字列化して扱う
            string colorcode = sender.ToString();

            // カラーコードは6文字あるはず
            if (colorcode == null || colorcode.Length != 6)
            {
                return new SolidColorBrush(Colors.White);
            }

            string stringR = colorcode.Substring(0, 2);
            string stringG = colorcode.Substring(2, 2);
            string stringB = colorcode.Substring(4, 2);

            byte byteR = 0;
            byte byteG = 0;
            byte byteB = 0;

            // 対応するRGBの作成
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

            // 背景色生成
            System.Windows.Media.Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byteR, byteG, byteB));

            return brush;
        }

        #endregion

        #region 反転

        /// <summary>
        /// 画像反転処理(左右)
        /// </summary>
        /// <param name="dropData">反転対象の画像データ</param>
        /// <returns>反転後の画像データ</returns>
        internal DropData Flip(DropData dropData)
        {
            if (dropData == null)
            {
                return null;
            }

            if (_imageManager != null)
            {
                dropData.ImageData = _imageManager.Flip(dropData.ImageParameter);
                return dropData;
            }

            return null;
        }


        #endregion

        #region 拡縮

        /// <summary>
        /// 画像拡縮時、コマンドを判断して下流に判断を依頼する
        /// </summary>
        /// <param name="dropData"></param>
        /// <returns>拡縮後のDropDataオブジェクト</returns>
        internal WriteableBitmap Scaling(DropData dropData, object sender, string widthPercent, string heightPercent)
        {
            if (dropData == null || sender == null)
            {
                return null;
            }

            // 値のパルス
            double widthScale = 0;
            double heightScale = 0;

            // 入力値が不正なときは何もしないで返す
            if (Double.TryParse(widthPercent, out widthScale) == false)
            {
                return dropData.ImageData;
            }

            if (Double.TryParse(heightPercent, out heightScale) == false)
            {
                return dropData.ImageData;
            }

            // 縦横比が0以下は異常なので何もせずに返す
            if (widthScale <= 0 || heightScale <= 0)
            {
                return dropData.ImageData;
            }

            // キャストして判別する
            InterpolationStyle style = (InterpolationStyle)sender;

            if (_imageManager == null)
            {
                return dropData.ImageData;
            }

            // 正常に受け取れていれば拡縮処理を依頼する
            if (style == InterpolationStyle.NearestNeighbor)
            {
                return _imageManager.ScalingNearestNeighbor(dropData.ImageParameter, widthScale, heightScale);
            }
            else if (style == InterpolationStyle.Bilinear)
            {
                return _imageManager.ScalingBilinear(dropData.ImageParameter, widthScale, heightScale);
            }

            return null;
        }


        #endregion

        #endregion
    }
}
