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
        /// <remarks>For Debugging</remarks>
        internal DropData(string path)
        {
            FilePath = path;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath">ドロップされたファイルのパス</param>
        /// <param name="imageData">対象ファイル</param>
        internal DropData(string filePath, WriteableBitmap imageData)
        {
            FilePath = filePath;
            ImageData = imageData;
        }

        /// <summary>
        /// ドロップされたファイルパスのコレクション
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
            private set;
        }
    }
}
