using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ImageProcessing.Entities
{
    /// <summary>
    /// ドロップされたファイルを内部に保持するエンティティ
    /// </summary>
   internal class DropData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DropData()
        {
            FileNames = new ObservableCollection<string>();
        }

        /// <summary>
        /// ドロップされたファイル名のコレクション
        /// </summary>
        internal ObservableCollection<string> FileNames
        {
            get;
            private set;
        }
    }
}
