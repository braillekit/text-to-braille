
#ifndef __FONTIMAGE_H__
#define __FONTIMAGE_H__

class CDataBuff : public CObject
{
	friend class CDataBuffList;
public:
	CDataBuff(byte* pData, int nSize);	// �R���X�g���N�^
	~CDataBuff();						// �f�X�g���N�^

protected:
	int m_nSize;			// �o�b�t�@�T�C�Y
	byte* m_pData;			// �f�[�^�o�b�t�@
	CDataBuff* m_pNext;		// ����CDataBuff�|�C���^
};

class CDataBuffList : public CObject
{
public:
	CDataBuffList();
	~CDataBuffList();

	void Add(CDataBuff* pData);
	void Add(byte* pData, int nSize);

	void Clear();
	int CreateData();
	int GetDataSize()	{ return m_nDataSize; }
	byte* GetData()		{ return m_pData; }

	//void SetSpaceSize(short nDot);
protected:
	int CalcDataSize();
	CDataBuff* m_pFirst;
	CDataBuff* m_pCurrent;
	byte* m_pData;
	int m_nDataSize;

	//bool m_bUseCtrl;
	//byte m_pCtrl[4];
};



/*
 * For one characters
 */
class CFontImage : public CObject
{
public:
	CFontImage();
	~CFontImage();

	// ������
	int Init(const char* pFontName, bool isHalfResolution = TRUE, int nFontHeight = 24);

	// �f�[�^�擾
	byte* GetCharImage(const char*& pText);

	// �T�C�Y�擾
	int GetSize() { return m_nDataSize; }

	// 2�o�C�g�����̃��[�h�o�C�g�`�F�b�N
	int IsLeadByte(unsigned char c);

	// �����ԃX�y�[�X�ʐݒ�
	void SetSpaceSize(short nDot);

	// �O���t�B�b�N�f�[�^���擾
	int GetImageData(const char* pText, byte*& pData, int& nSize);

protected:
	// �X�y�[�X�����𐧌�i���Έʒu�ړ��j�֒u��������
	virtual bool CheckSpaceString(const char* pOne);

protected:
	CDC 	m_dcMem;			// ������DC
	CBitmap m_bmpMem;			// �r�b�g�}�b�v
	CFont	m_fnt;				// �t�H���g
	int		m_nFontHeight;		// �t�H���g�̍���
	SIZE	m_szImg;			// 1�����̃h�b�g�T�C�Y�i�Œ�:24*24dot�j
	byte*	m_pData;			// �f�[�^�o�b�t�@
	int		m_nDataSize;		// �f�[�^�T�C�Y�i���ۂɎg�p�����f�[�^�T�C�Y�j
	int		m_nBuffSize;		// �o�b�t�@�T�C�Y�iMAX�j

	bool	m_bHalfResolution;	// ���̉𑜓x�t���O	TRUE:90dpi�AFALSE:180dpi
	int		m_nResFlag;			// �𑜓x�t���O	1:180dpi�A2:90dpi

	BOOL	m_bUseCharSpace;	// �����ԃX�y�[�X�L���t���O
	byte	m_pCtrlSpace[4];	// �����ԃX�y�[�X����f�[�^
};

/*
 * ���p����Only�p
 */
class CFontImageHalf : public CFontImage
{
public:
	CFontImageHalf();
	~CFontImageHalf(){}

protected:
	// �X�y�[�X�����𐧌�i���Έʒu�ړ��j�֒u��������
	bool CheckSpaceString(const char* pOne);
};

#endif //__FONTIMAGE_H__
