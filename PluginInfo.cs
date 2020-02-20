using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITToolKit_3
{
    /// <summary>
    /// プラグインに関する情報
    /// </summary>
    public class PluginInfo
    {

        /// <summary>
        /// PluginInfoクラスのコンストラクタ
        /// </summary>

        private PluginInfo(string path, string cls)
        {
            Location = path;
            ClassName = cls;
        }

        /// <summary>
        /// アセンブリファイルのパス
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// クラスの名前
        /// </summary>
        public string ClassName { get; }

        /// <summary>
        /// 有効なプラグインを探す
        /// </summary>

        public static PluginInfo[] FindPlugins()
        {
            System.Collections.ArrayList plugins =
                new System.Collections.ArrayList();
            //IPlugin型の名前
            string ipluginName = typeof(IPlugin).FullName;

            //プラグインフォルダ
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            folder += "\\plugins";
            if (!Directory.Exists(folder)) Directory.CreateDirectory("./plugins");
               

            //.dllファイルを探す
            string[] dlls = Directory.GetFiles(folder, "*.dll");

            foreach (string dll in dlls)
            {
                try
                {
                    //アセンブリとして読み込む
                    Assembly asm = Assembly.LoadFrom(dll);
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(ipluginName) != null)
                        {
                            //PluginInfoをコレクションに追加する
                            plugins.Add(new PluginInfo(dll, t.FullName));
                        }
                    }
                }
                catch
                {
                }
            }

            //コレクションを配列にして返す
            return (PluginInfo[])plugins.ToArray(typeof(PluginInfo));
        }

        /// <summary>
        /// プラグインクラスのインスタンスを作成する
        /// </summary>

        public IPlugin CreateInstance(MainWindow main)
        {
            try
            {
                //アセンブリを読み込む
                Assembly asm = Assembly.LoadFrom(Location);
                //クラス名からインスタンスを作成する
                return (IPlugin)asm.CreateInstance(ClassName,                           // 名前空間を含めたクラス名
                                                    false,                              // 大文字小文字を無視するかどうか
                                                    BindingFlags.CreateInstance,        // インスタンスを生成
                                                    null,                               // 通常はnullでOK,
                                                    new object[] {main},                // コンストラクタの引数
                                                    null,                               // カルチャ設定（通常はnullでOK）
                                                    null                                // ローカル実行の場合はnullでOK
                                                  );
            }
            catch
            {
                return null;
            }
        }
    }

}
