#include "stdafx.h"
#include "math.h"
#include "FontImage.h"


/*
 * �R���X�g���N�^
 */
CDataBuff::CDataBuff(byte* pData, int nSize)
{
	m_nSize = nSize;
	m_pData = new byte[m_nSize];
	memcpy(m_pData, pData, m_nSize);
	m_pNext = NULL;
}

/*
 * �f�X�g���N�^
 */
CDataBuff::~CDataBuff()
{
	if(m_pData) 
		delete[] m_pData;
}

/*
 * �R���X�g���N�^
 */
CDataBuffList::CDataBuffList()
{
	m_pFirst	= NULL;
	m_pCurrent	= NULL;
	m_pData		= NULL;
	m_nDataSize	= 0;

	/*m_bUseCtrl	= FALSE;
	m_pCtrl[0]	= 0x1b;
	m_pCtrl[1]	= '\\';
	m_pCtrl[2]	= 0x0d;
	m_pCtrl[3]	= 0x0;*/
}

/*
 * �f�X�g���N�^
 */
CDataBuffList::~CDataBuffList()
{
	Clear();
}

/*
 * �f�[�^�̒ǉ�
 */
void CDataBuffList::Add(CDataBuff* pData)
{
	if(m_pFirst==NULL) {
		m_pFirst = m_pCurrent = pData;
	} else {
		m_pCurrent->m_pNext = pData;
		m_pCurrent = pData;
	}
}

/*
 * �f�[�^�̒ǉ�
 */
void CDataBuffList::Add(byte* pData, int nSize)
{
	Add(new CDataBuff(pData, nSize));
}

/*
 * �o�b�t�@��TOTAL�T�C�Y���擾
 */
int CDataBuffList::CalcDataSize()
{
	int nSize = 0;
	CDataBuff* p = m_pFirst;

	while(p) {
		nSize += p->m_nSize;
		//if(m_bUseCtrl && p->m_pNext) 
		//	nSize+=sizeof(m_pCtrl);	// +4bytes
		p = p->m_pNext;
	}
	return nSize;
}

/*
 * ���X�g�ɂ���o�b�t�@����f�[�^���쐬
 */
int CDataBuffList::CreateData()
{
	if( m_pData!=NULL ) { delete[] m_pData;	}

	// �o�b�t�@�T�C�Y���v�Z
	m_nDataSize = CalcDataSize();
	// �o�b�t�@���m��
	m_pData = new byte[m_nDataSize];

	byte* pDest=m_pData;	// �R�s�[��̃|�C���^

	CDataBuff* p = m_pFirst;
	while(p) {
		memcpy(pDest, p->m_pData, p->m_nSize);
		pDest += p->m_nSize;

		//if(m_bUseCtrl && p->m_pNext) {	// �����ԃX�y�[�X�ݒ萧��R�[�h
		//	memcpy(pDest, m_pCtrl, sizeof(m_pCtrl));	
		//	pDest += sizeof(m_pCtrl);
		//}
		p = p->m_pNext;
	}
	return m_nDataSize;
}

/*
 * �o�b�t�@���N���A
 */
void CDataBuffList::Clear()
{
	CDataBuff* p;

	while(m_pFirst) {
		p = m_pFirst->m_pNext;
		delete m_pFirst;
		m_pFirst = p;
	}

	if(m_pData) delete[] m_pData;
}

/*
 * �R���X�g���N�^
 */
CFontImage::CFontImage()
{
	m_szImg.cx		= 24;	// 1�����̉���
	m_szImg.cy		= 24;	// 1�����̏c��
	m_nFontHeight	= 25;	// �t�H���g�̍���
	m_pData			= NULL;
	m_nBuffSize		= 0;
	m_nDataSize		= 0;
	m_nResFlag		= 2;	// �𑜓x�t���O�@1:180dpi, 2:90dpi

	m_bUseCharSpace	= FALSE;
	m_pCtrlSpace[0]	= 0x1b;
	m_pCtrlSpace[1]	= '\\';
	m_pCtrlSpace[2]	= 0x0d;
	m_pCtrlSpace[3]	= 0x0;
}

/*
 * �f�X�g���N�^
 */
CFontImage::~CFontImage()
{
	// �f�[�^�o�b�t�@���J��
	delete[] m_pData;
	// ������DC���J��
	m_dcMem.DeleteDC();
	// �r�b�g�}�b�v�I�u�W�F�N�g���J��
	m_bmpMem.DeleteObject();
	// �t�H���g�I�u�W�F�N�g���J��
	m_fnt.DeleteObject();
}

/*
 * ������
 */
int CFontImage::Init(const char* pFontName, bool isHalfResolution/*=TRUE*/, int nFontHeight /*=24*/)
{
	// �R���g���[���R�[�h�� (ESC * 39 n n)
	const int CTRLCODENUM = 5;

	// �𑜓x�i�����H�j->�t���O��ݒ�
	m_nResFlag = isHalfResolution ? 2 : 1;
	// �t�H���g�����ݒ�(20�`26)
	if( nFontHeight>=20 && nFontHeight <=26 ) m_nFontHeight = nFontHeight;
	// �K�v�o�C�g�T�C�Y
	m_nBuffSize = m_szImg.cx*m_szImg.cy/8/m_nResFlag /* /2 �𑜓x�����̏ꍇ */ + CTRLCODENUM;
	// �o�b�t�@���m��
	if( m_pData ) delete[] m_pData;
	m_pData = new byte[m_nBuffSize];
	// ������DC�쐬
	m_dcMem.CreateCompatibleDC(NULL);
	// �r�b�g�}�b�v�쐬
	m_bmpMem.CreateCompatibleBitmap(&m_dcMem, m_szImg.cx, m_szImg.cy);
	// DC�Ƀr�b�g�}�b�v���Z�b�g
	m_dcMem.SelectObject(&m_bmpMem);
	// �t�H���g�I�u�W�F�N�g���쐬
	m_fnt.CreateFont( m_nFontHeight,0,0,0,0,
						FALSE, FALSE, FALSE,
						DEFAULT_CHARSET,
						OUT_DEFAULT_PRECIS,
						CLIP_DEFAULT_PRECIS,
						DEFAULT_QUALITY,
						FIXED_PITCH | FF_MODERN,
						pFontName );

	// DC�Ƀt�H���g��ݒ�
	m_dcMem.SelectObject(&m_fnt);
	return 0;
}

/**** important *********************************************
  this function is check to multi bytes (for japanese) code. 
  please change check logic as your language code.
 ************************************************************/
/*
 * 2�o�C�g�����̐擪�o�C�g�`�F�b�N
 */
int CFontImage::IsLeadByte(unsigned char c)
{
	return (0x81 <= c && c <= 0x9f || 0xe0 <= c && c <= 0xfc);
}

/*
 * �X�y�[�X�����𐧌�R�[�h�ɒu��������
 * �O���t�B�b�N�f�[�^�����T�C�Y���������}�������
 */
bool CFontImage::CheckSpaceString(const char* pOne)
{
	int size=0;
	int nPos=0;

	if( ((int)strlen(pOne))==1 ) {
		// ���p
		if(*pOne==' ')
			size = m_szImg.cx/2;
	} else {
		// �S�p
		if( strcmp(pOne, "�@")==0 )
			size = m_szImg.cx;
	}

	if( size>0 ) {
		m_pData[nPos++] = 0x1b;
		m_pData[nPos++] = '\\';
		m_pData[nPos++] = size;
		m_pData[nPos++] = 0;
		m_nDataSize = 4;	// 4bytes
		return true;
	}
	return false;
}

/*
 * pText : text for convert
 */
byte* CFontImage::GetCharImage(const char*& pText)
{
	char pOne[3];

	// �o�b�t�@��������
	memset(m_pData, 0, m_nBuffSize);
	memset(pOne,	0, sizeof(pOne));

	// 2�o�C�g�����̏ꍇ�A2�o�C�g��������
	// �ipOne �֏�������f�[�^���R�s�[�j
	if( IsLeadByte((unsigned char)*pText) ) {
		strncpy(pOne, pText, 2);
		pText+=2;
	} else
		*pOne = *pText++;
	
	// �X�y�[�X�����`�F�b�N
	if( CheckSpaceString(pOne) ) return m_pData;

	// �w�i�𔒂œh��Ԃ�
	m_dcMem.FillSolidRect(0,0,m_szImg.cx,m_szImg.cy,RGB(255,255,255));
	// �������o��
	m_dcMem.TextOut(0, 0, pOne, (int)strlen(pOne));

	int nPos=0,nBit;
	int cx = m_szImg.cx/m_nResFlag;

	// ����R�[�h��
	m_pData[nPos++] = 0x1b;
	m_pData[nPos++] = '*';
	m_pData[nPos++] = (byte)(40-m_nResFlag);	// 38:90dpi 39:180dpi
	m_pData[nPos++] = (byte)(cx % 256);
	m_pData[nPos++] = (byte)(cx / 256);

	/*
	        1  4  7 10 13 ... (byte)
	   8--+ o  o  o  o  o
	   7  | o
	   6  | o
	   5  | o
	   4  | o
	   3  | o
	   2  | o
	   1--+ o
	   8--+ o <--- 2 byte
	   7  | o
	   6  | o
	   5  | o
	   4  | o
	   3  | o
	   2  | o
	   1--+ o
	   8--+ o <--- 3 byte
	   7  | o
	   6  | o
	   5  | o
	   4  | o
	   3  | o
	   2  | o
	   1--+ o
	 (bit)

	*/

	//
	// check pixel color & set values
	//
	nPos--;
	for(int x=0;x<m_szImg.cx;x+=m_nResFlag) {
		for(int y=0;y<8*3;y++) {
			if( (nBit=(y%8))==0) nPos++;
			if( m_dcMem.GetPixel(x,y)!=0xffffff )
				m_pData[nPos] |= (byte)pow((double)2,(double)(7-nBit));
		}
	}

	m_nDataSize = m_nBuffSize;	// �f�[�^�T�C�Y���o�b�t�@�T�C�Y�ɍ��킹��i1�������̃O���t�B�b�N�f�[�^�j
	return m_pData;
}

/*
 * pText : text for convert
 * pData : recive data pointer
 * nSize : recive data size
 */
int CFontImage::GetImageData(const char* pText, byte*& pData, int& nSize)
{
	CDataBuffList buffList;

	do {
		byte *p=GetCharImage(pText);	// ����->�O���t�B�b�N�f�[�^�쐬
		buffList.Add(p, GetSize());		// �o�b�t�@�֒ǉ�

		if(*pText && m_bUseCharSpace) {
			// ���ɕ����������A���A�����ԃX�y�[�X�g�p�t���O�������Ă���ꍇ
			// �����ԃX�y�[�X����R�[�h��ǉ�����
			buffList.Add(m_pCtrlSpace, sizeof(m_pCtrlSpace));
		}

	} while(*pText);

	nSize = buffList.CreateData();	// �f�[�^�\�z
	pData = new byte[nSize];
	memcpy(pData, buffList.GetData(), nSize);
	
	return 0;
}

/*
 * �����ԃX�y�[�X�ʂ̐ݒ�
 */
void CFontImage::SetSpaceSize(short nDot)
{
	if(nDot==0) { m_bUseCharSpace=FALSE; return; }
	m_bUseCharSpace		= TRUE;
	*(m_pCtrlSpace+2)	= (byte)(nDot & 0xff);
	*(m_pCtrlSpace+3)	= (byte)(nDot >> 8);
}



/*
 * �R���X�g���N�^
 */
CFontImageHalf::CFontImageHalf()
: CFontImage()
{
	m_szImg.cx = 12; 
}

/*
 * �X�y�[�X�����𐧌�R�[�h�ɒu��������
 * �O���t�B�b�N�f�[�^�����T�C�Y���������}�������
 */
bool CFontImageHalf::CheckSpaceString(const char* pOne)
{
	int size=0;
	int nPos=0;

	if( ((int)strlen(pOne))==1 ) {
		// ���p
		if(*pOne==' ')
			size = m_szImg.cx;	// ���p�����ł��t�H���g�T�C�Y�̕���
	} else {
		// �S�p
		if( strcmp(pOne, "�@")==0 )
			size = m_szImg.cx;
	}

	if( size>0 ) {
		m_pData[nPos++] = 0x1b;
		m_pData[nPos++] = '\\';
		m_pData[nPos++] = size;
		m_pData[nPos++] = 0;
		m_nDataSize = 4;	// 4bytes
		return true;
	}
	return false;
}
