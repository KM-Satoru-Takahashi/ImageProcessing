using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing.Entities
{
    /// <summary>
    /// ドロップされたファイルを内部に保持するエンティティ
    /// </summary>
    public class DropData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DropData(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// ドロップされたファイルパス
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// 画像ファイル情報
        /// </summary>
        public WriteableBitmap ImageData
        {
            get;
            set;
        }

        /// <summary>
        /// 読み取った画像データの生データを保存するエンティティ
        /// </summary>
        internal RowData RowData { get; } = new RowData();

        /// <summary>
        /// 画像処理関連で動的に変化するデータを保持するエンティティ
        /// </summary>
        internal ImageParameter ImageParameter { get; } = new ImageParameter();

    }
}
