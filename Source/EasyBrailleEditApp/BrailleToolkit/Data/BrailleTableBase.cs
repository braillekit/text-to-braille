using System.Reflection;

namespace BrailleToolkit.Data
{
    /// <summary>
    /// �I�r��������¦���O�C
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
        /// �j�M�Y�Ӥ�r�Ÿ��A�öǦ^�������I�r�X�C
        /// <b>�`�N�G</b>���j�M��k�O�j�M��ӹ�Ӫ�A��ĳ�ϥΨ�L�������j�M��k�A
        /// �H�K�����~�����G�C�ר�O�`���Ÿ��M�n�աA�@�w�n���O�I�s
        /// FindPhonetic �M FindTone�A�_�h�|�]����J���r�꦳���ΪťզӶǦ^���~�����G�C
        /// </summary>
        /// <param name="text">���j�M���Ÿ��C</param>
        /// <param name="type">���w���j�M���Ÿ������C�Y���P�������s�b�ۦP����r�Ÿ��A�K�����w���ѼơA�H�T�O��쥿�T���Ÿ��C</param>
        /// <returns>�Y�����A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^ null�C</returns>
        public abstract string Find(string text, string type=null);

        /// <summary>
        /// �j�M�Y�Ӥ�r�Ÿ��A�öǦ^�������I�r��m�r��C�Ҧp: "134 26"�C
        /// </summary>
        /// <param name="text">���j�M���Ÿ��C</param>
        /// <param name="type">���w���j�M���Ÿ������C�Y���P�������s�b�ۦP����r�Ÿ��A�K�����w���ѼơA�H�T�O��쥿�T���Ÿ��C</param>
        /// <returns>�d�ҡG"24 1345 36"�C</returns>
        public abstract string[] GetDots(string text, string type=null);

        /// <summary>
        /// ��¥ΨӧP�_�Y�Ÿ��O�_�s�b���I�r��Ӫ�A�����X�I�r�X�C�t�פ� Find �֡C
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public abstract bool Exists(string text);
    }
}
