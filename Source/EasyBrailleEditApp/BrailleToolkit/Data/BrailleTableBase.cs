using System.Reflection;

namespace BrailleToolkit.Data
{
    /// <summary>
    /// 點字對應表的基礎類別。
    /// </summary>
    public abstract class BrailleTableBase
    {
        public abstract void Load();
        public abstract void Load(string filename);
        public abstract void LoadFromResource(Assembly asmb, string resourceName);

        public abstract string this[string text]
        {
            get;
        }

        /// <summary>
        /// 搜尋某個文字符號，並傳回對應的點字碼。
        /// <b>注意：</b>此搜尋方法是搜尋整個對照表，建議使用其他版本的搜尋方法，
        /// 以免找到錯誤的結果。尤其是注音符號和聲調，一定要分別呼叫
        /// FindPhonetic 和 FindTone，否則會因為輸入的字串有全形空白而傳回錯誤的結果。
        /// </summary>
        /// <param name="text">欲搜尋的符號。</param>
        /// <param name="type">限定欲搜尋的符號類型。若不同類型當中存在相同的文字符號，便應指定此參數，以確保找到正確的符號。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public abstract string Find(string text, string type=null);

        /// <summary>
        /// 搜尋某個文字符號，並傳回對應的點字位置字串。例如: "134 26"。
        /// </summary>
        /// <param name="text">欲搜尋的符號。</param>
        /// <param name="type">限定欲搜尋的符號類型。若不同類型當中存在相同的文字符號，便應指定此參數，以確保找到正確的符號。</param>
        /// <returns>範例："24 1345 36"。</returns>
        public abstract string[] GetDots(string text, string type=null);

        /// <summary>
        /// 單純用來判斷某符號是否存在此點字對照表，不取出點字碼。速度比 Find 快。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract bool Exists(string text);
    }
}
