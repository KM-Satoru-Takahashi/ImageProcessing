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
        /// ドロップされたファイルパスのコレクション
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }
    }
}
