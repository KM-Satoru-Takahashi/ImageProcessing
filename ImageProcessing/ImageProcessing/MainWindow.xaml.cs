using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ImageProcessing.Entities;

namespace ImageProcessing
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        #region field

        /// <summary>
        /// バインディング先のVM
        /// </summary>
        private ViewModel.MainWindowViewModel _viewModel = null;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // 初期化処理

        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            _viewModel = new ViewModel.MainWindowViewModel();
            this.DataContext = _viewModel;
        }

        #region ドロップイベント

        /// <summary>
        /// ドラッグアンドドロップの判定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>ドロップされたものがファイルであるときだけドロップイベントとして受け取る</remarks>
        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e == null)
            {
                return;
            }
            else if (e.Data == null)
            {
                return;
            }
            // ファイルの時は有効
            else if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            // ファイル以外は無効
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;

        }

        /// <summary>
        /// ドラッグアンドドロップを受けるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>受けられるか受けられないかは_PreviewDragOrderで判定済とする</remarks>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            // ドロップされたファイルを保持するエンティティを用意
            DropData dropData = new DropData();

            // ドロップされたファイルのパスを取得
            string[] filePath = e.Data.GetData(DataFormats.FileDrop) as string[];

            if (dropData == null)
            {
                return;
            }
            if (filePath == null || filePath.Length < 1)
            {
                return;
            }

            foreach (var file in filePath)
            {
                dropData.FileNames.Add(file);
            }

        }

        #endregion

    }
}
