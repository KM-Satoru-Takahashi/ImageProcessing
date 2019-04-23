using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotateUnitTest
{
    class Program
    {


        static void Main(string[] args)
        {
            // 右回転testPrg
            RightRotate();

            // 左回転testPrg
            LeftRotate();

            // test
            // 剰余で小数以下のときどうなる？
            // 普通の剰余
            Console.WriteLine("21%4: " + 21 % 4);
            // 小数以下
            Console.WriteLine("(3/4)%4: " + (3 / 4) % 4);

            // 画面停止
            Console.ReadLine();
        }

        static void RightRotate()
        {
            // 1次元バイナリデータ
            byte[] testArr = new byte[12];
            byte[] newArr = new byte[12];

            // 幅と高さ
            byte width = 4;
            byte height = 3;

            // 配列にテスト値を入れておく
            Console.WriteLine("変換前の配列");
            for (byte i = 0; i < testArr.Length; i++)
            {
                testArr[i] = i;

                // 出力
                if (testArr[i] % width == width - 1)
                {
                    Console.WriteLine("[" + testArr[i] + "]");
                }
                else
                {
                    Console.Write("[" + testArr[i] + "]\t");
                }
            }

            // 90度右回転して表示する
            // 元の配列のn列目を下から読んだものが変換後のn行目になる
            // 一時的に値を保持しておくリストを用意
            List<byte> tempList = new List<byte>();
            // 列数=幅だけ繰り返す
            for (byte oldCol = 0; oldCol < width; oldCol++)
            {
                // n列目を下から読んだものが新しい配列のn行目になる

                // 元の配列を末尾から読んでいく
                for (byte i = (byte)(testArr.Length - 1); i < testArr.Length; i--)
                {
                    // widthで割った剰余がn列のnと一致すればn列である証明となる
                    if (testArr[i] % width == oldCol)
                    {
                        tempList.Add(testArr[i]);
                    }

                }
            }

            // リストを新規配列に代入する
            if (tempList.Count == newArr.Length)
            {
                // 順序を保証したいのでfor文を使用
                //for(int i = 0; i<newArr.Length;i++)
                // {
                //     newArr[i] = tempList[i];
                // }
                newArr = tempList.ToArray();
            }


            Console.WriteLine("右回転後の配列");
            for (byte i = 0; i < newArr.Length; i++)
            {
                // 出力
                // widthとheightが入れ替わる
                if (i % height == height - 1)
                {
                    Console.WriteLine("[" + newArr[i] + "]");
                }
                else
                {
                    Console.Write("[" + newArr[i] + "]\t");
                }
            }
        }

        static void LeftRotate()
        {
            // 1次元バイナリデータ
            byte[] testArr = new byte[12];
            byte[] newArr = new byte[12];

            // 幅と高さ
            byte width = 4;
            byte height = 3;

            // 配列にテスト値を入れておく
            Console.WriteLine("変換前の配列");
            for (byte i = 0; i < testArr.Length; i++)
            {
                testArr[i] = i;

                // 出力
                if (testArr[i] % width == width - 1)
                {
                    Console.WriteLine("[" + testArr[i] + "]");
                }
                else
                {
                    Console.Write("[" + testArr[i] + "]\t");
                }
            }

            // 90度左回転して表示する
            // 元の配列の最後からn列目を上から読んだものが変換後のn行目になる
            // 一時的に値を保持しておくリストを用意
            List<byte> tempList = new List<byte>();
            // 列数=幅だけ繰り返す
            for (byte oldCol = (byte)(width - 1); oldCol < width; oldCol--)
            {
                // 最後からn列目を上から読んだものが新しい配列のn行目になる

                // 元の配列を先頭から読んでいく
                for (byte i = 0; i < testArr.Length; i++)
                {
                    // widthで割った剰余がn列のnと一致すればn列である証明となる
                    if (testArr[i] % width == oldCol)
                    {
                        tempList.Add(testArr[i]);
                    }

                }
            }

            // リストを新規配列に代入する
            if (tempList.Count == newArr.Length)
            {
                newArr = tempList.ToArray();
            }


            Console.WriteLine("左回転後の配列");
            for (byte i = 0; i < newArr.Length; i++)
            {
                // 出力
                // widthとheightが入れ替わる
                if (i % height == height - 1)
                {
                    Console.WriteLine("[" + newArr[i] + "]");
                }
                else
                {
                    Console.Write("[" + newArr[i] + "]\t");
                }
            }
        }
    }
}
