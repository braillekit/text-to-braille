namespace EasyBrailleEdit
{
    public class AppConst
    {
        public const string ProductID = "EASYBRAILLEEDIT10";
        public const string AppName = "EasyBrailleEdit";
        public const string UpdateWebUri = "UpdateWeb/EBE2/";    // e.g. http://www.huanlin.com/UpdateWeb/EBE2

        public const string DefaultAppUpdateFilesUri =
            "https://raw.githubusercontent.com/huanlin/EasyBrailleEdit/master/UpdateFiles/";

        // �w�]�@��̤j���
        public const int DefaultCellsPerLine = 40;
        public const int DefaultLinesPerPage = 25;

        public const string DefaultBrailleFileExt = ".brlj";	// �w�]���I�r�ɰ��ɦW (�ª��� .btx)

        // �Ȧs�ɮ�
        public const string CvtInputTempFileName = "cvt_in.tmp";			// ��J�������r��
        public const string CvtInputPhraseListFileName = "cvt_in_phrase.tmp";	// ��J�����w�]�w��
        public const string CvtOutputTempFileName = "cvt_out.tmp";			// ��X���I�r��
        public const string CvtErrorCharFileName = "cvt_errchar.tmp";		// �x�s�ഫ���Ѫ��r����T
        public const string CvtResultFileName = "cvt_result.tmp";	// �x�s���\�Υ��Ѫ��X���H�ο��~�T��

        public const string FileNameFilter = "�����ɮ� 1.x �� (*.btx)|*.btx|�����ɮ� 2.x �� (*.brlj)|*.brlj";
        public const int FileNameFilterIndex = 2;
        public const string SaveAsFileNameFilter = "�����ɮ� 2.x �� (*.brlj)|*.brlj";
        public const int SaveAsFileNameFilterIndex = 1;
    }
}
