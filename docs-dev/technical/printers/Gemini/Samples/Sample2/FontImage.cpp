#include "stdafx.h"
#include "math.h"
#include "FontImage.h"

/*
 * constractor
 */
CBuffList::CBuffList()
{
	m_nSize = 0;
	m_pData = NULL;
	m_pNext = NULL;
}

/*
 * constractor
 */
CBuffList::CBuffList(byte* pData, int nSize)
{
	m_nSize = nSize;
	m_pData = new byte[m_nSize];
	memcpy(m_pData, pData, m_nSize);
	m_pNext = NULL;
}

/*
 * destractor
 */
CBuffList::~CBuffList()
{
	delete[] m_pData;
}

/*
 * 
 */
CBuffList* CBuffList::AddNext(byte* pData, int nSize)
{
	m_pNext = new CBuffList(pData, nSize);
	return m_pNext;
}

/*
 * 
 */
int CBuffList::SetData(byte* pData, int nSize)
{
	if( m_pData ) delete[] m_pData;
	m_nSize = nSize;
	memcpy(m_pData, pData, m_nSize);
	return 0;
}

/*
 * constractor
 */
CFontImage::CFontImage()
{
	m_szImg.cx = 24;	// default
	m_szImg.cy = 24;	// default (cy can not change)
	m_pData = NULL;
	m_nDataSize = 0;
}

/*
 * destractor
 */
CFontImage::~CFontImage()
{
	delete[] m_pData;
	m_dcMem.DeleteDC();
	m_bmpMem.DeleteObject();
	m_fnt.DeleteObject();
}

/*
 * initialize
 */
int CFontImage::Init(const char* pFontName, SIZE* pszImage /*=NULL*/)
{
	if( pszImage ) {
		m_szImg = *pszImage;
	}
	
	const int CTRLCODENUM = 5;	// 5 is control code count (ESC * 39 n n)
	m_nDataSize = m_szImg.cx*m_szImg.cy/8 + CTRLCODENUM;

	m_pData = new byte[m_nDataSize];
	m_dcMem.CreateCompatibleDC(NULL);
	m_bmpMem.CreateCompatibleBitmap(&m_dcMem, m_szImg.cx, m_szImg.cy);
	m_dcMem.SelectObject(&m_bmpMem);

	m_fnt.CreateFont( m_szImg.cy,0,0,0,0,
						FALSE, FALSE, FALSE,
						DEFAULT_CHARSET,
						OUT_DEFAULT_PRECIS,
						CLIP_DEFAULT_PRECIS,
						DEFAULT_QUALITY,
						FIXED_PITCH | FF_MODERN,
						pFontName );

	m_dcMem.SelectObject(&m_fnt);
	return 0;
}

/**** important *********************************************
  this function is check to multi bytes (for japanese) code. 
  please change check logic as your language code.
 ************************************************************/
int CFontImage::IsLeadByte(unsigned char c)
{
	return (0x81 <= c && c <= 0x9f || 0xe0 <= c && c <= 0xfc);
}

/*
 * pText : text for convert
 */
byte* CFontImage::GetCharImage(const char*& pText)
{
	char pOne[3];

	// initialize buffer(fill zero)
	memset(m_pData, 0, m_nDataSize);
	memset(pOne, 0, sizeof(pOne));

	// this is for japanese.
	// is multi byte charactor ? (is leadbyte of japanese shift-jis)
	//
	// *** you have to do change of check function. ***> IsLeadByte()
	//	
	if( IsLeadByte((unsigned char)*pText) ) {
		strncpy(pOne, pText, 2);
		pText+=2;
	} else
		*pOne = *pText++;
	
	// fill background -> white color
	m_dcMem.FillSolidRect(0,0,m_szImg.cx,m_szImg.cy,RGB(255,255,255));
	// output Text
	m_dcMem.TextOut(0,0,pOne,strlen(pOne));

	int nPos=0,nBit;
	memset(m_pData,0,m_nDataSize);
	m_pData[nPos++] = 0x1b;
	m_pData[nPos++] = '*';
	m_pData[nPos++] = 39;
	m_pData[nPos++] = (byte)(m_szImg.cx % 256);
	m_pData[nPos++] = (byte)(m_szImg.cx / 256);

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
	for(int x=0;x<m_szImg.cx;x++) {
		for(int y=0;y<8*3;y++) {
			if( (nBit=(y+1)%8)==0 ) nPos++;
			if( m_dcMem.GetPixel(x,y)!=0xffffff )
				m_pData[nPos] |= (byte)pow(2,(7-nBit));
		}
	}
	return m_pData;
}

/*
 * pText : text for convert
 * pData : recive data pointer
 * nSize : recive data size
 */
int CFontImageLine::GetImageData(const char* pText, byte*& pData, int& nSize)
{
	CFontImage fntImg;
	CBuffList* pFirst=NULL, *pCurr=NULL, *pTemp=NULL;	// data buffer list

	// *** important ***
	// please set font name
	// 
	// fntImg.Init("[chinese font face name]")
	//
	fntImg.Init( NULL ); // NULL ---> "[font name]"
	
	do {
		byte *p=fntImg.GetCharImage(pText);
		if( !pCurr ) {
			pFirst = pCurr = new CBuffList(p, fntImg.GetSize());
		} else {
			pCurr = pCurr->AddNext(p, fntImg.GetSize());
		}
	} while(*pText);

	byte pCtrlCode[] = { 0x1b, 0x5c, 0x0d, 0x0 };		// string space is 13(0x0d)dots
	int nCtrlCode = sizeof(pCtrlCode);

	int nCnt = CBuffList::GetListCount(pFirst);			// buffer num

	//
	// buffer size = [string count] * ([string data] + [space data(4byte)]) - [space data]
	// 
	nSize = nCnt * (fntImg.GetSize() + nCtrlCode) - nCtrlCode;
	pData = new byte[nSize];
	byte* p = pData;

	//
	// copy data & free memory
	//
	pCurr = pFirst;
	while( pTemp=pCurr->GetNext() ) {
		memcpy(p, pCurr->GetData(), pCurr->GetSize());
		p+=pCurr->GetSize();
		memcpy(p, pCtrlCode, nCtrlCode);
		p+=nCtrlCode;

		delete[] pCurr;
		pCurr = pTemp;
	}

	memcpy(p, pCurr->GetData(), pCurr->GetSize());
	p+=pCurr->GetSize();
	delete pCurr;

	return 0;
}

