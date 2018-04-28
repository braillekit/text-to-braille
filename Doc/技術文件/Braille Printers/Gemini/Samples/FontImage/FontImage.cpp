#include "stdafx.h"
#include "math.h"
#include "FontImage.h"


/*
 * コンストラクタ
 */
CDataBuff::CDataBuff(byte* pData, int nSize)
{
	m_nSize = nSize;
	m_pData = new byte[m_nSize];
	memcpy(m_pData, pData, m_nSize);
	m_pNext = NULL;
}

/*
 * デストラクタ
 */
CDataBuff::~CDataBuff()
{
	if(m_pData) 
		delete[] m_pData;
}

/*
 * コンストラクタ
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
 * デストラクタ
 */
CDataBuffList::~CDataBuffList()
{
	Clear();
}

/*
 * データの追加
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
 * データの追加
 */
void CDataBuffList::Add(byte* pData, int nSize)
{
	Add(new CDataBuff(pData, nSize));
}

/*
 * バッファのTOTALサイズを取得
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
 * リストにあるバッファからデータを作成
 */
int CDataBuffList::CreateData()
{
	if( m_pData!=NULL ) { delete[] m_pData;	}

	// バッファサイズを計算
	m_nDataSize = CalcDataSize();
	// バッファを確保
	m_pData = new byte[m_nDataSize];

	byte* pDest=m_pData;	// コピー先のポインタ

	CDataBuff* p = m_pFirst;
	while(p) {
		memcpy(pDest, p->m_pData, p->m_nSize);
		pDest += p->m_nSize;

		//if(m_bUseCtrl && p->m_pNext) {	// 文字間スペース設定制御コード
		//	memcpy(pDest, m_pCtrl, sizeof(m_pCtrl));	
		//	pDest += sizeof(m_pCtrl);
		//}
		p = p->m_pNext;
	}
	return m_nDataSize;
}

/*
 * バッファをクリア
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
 * コンストラクタ
 */
CFontImage::CFontImage()
{
	m_szImg.cx		= 24;	// 1文字の横幅
	m_szImg.cy		= 24;	// 1文字の縦幅
	m_nFontHeight	= 25;	// フォントの高さ
	m_pData			= NULL;
	m_nBuffSize		= 0;
	m_nDataSize		= 0;
	m_nResFlag		= 2;	// 解像度フラグ　1:180dpi, 2:90dpi

	m_bUseCharSpace	= FALSE;
	m_pCtrlSpace[0]	= 0x1b;
	m_pCtrlSpace[1]	= '\\';
	m_pCtrlSpace[2]	= 0x0d;
	m_pCtrlSpace[3]	= 0x0;
}

/*
 * デストラクタ
 */
CFontImage::~CFontImage()
{
	// データバッファを開放
	delete[] m_pData;
	// メモリDCを開放
	m_dcMem.DeleteDC();
	// ビットマップオブジェクトを開放
	m_bmpMem.DeleteObject();
	// フォントオブジェクトを開放
	m_fnt.DeleteObject();
}

/*
 * 初期化
 */
int CFontImage::Init(const char* pFontName, bool isHalfResolution/*=TRUE*/, int nFontHeight /*=24*/)
{
	// コントロールコード数 (ESC * 39 n n)
	const int CTRLCODENUM = 5;

	// 解像度（半分？）->フラグを設定
	m_nResFlag = isHalfResolution ? 2 : 1;
	// フォント高さ設定(20～26)
	if( nFontHeight>=20 && nFontHeight <=26 ) m_nFontHeight = nFontHeight;
	// 必要バイトサイズ
	m_nBuffSize = m_szImg.cx*m_szImg.cy/8/m_nResFlag /* /2 解像度半分の場合 */ + CTRLCODENUM;
	// バッファを確保
	if( m_pData ) delete[] m_pData;
	m_pData = new byte[m_nBuffSize];
	// メモリDC作成
	m_dcMem.CreateCompatibleDC(NULL);
	// ビットマップ作成
	m_bmpMem.CreateCompatibleBitmap(&m_dcMem, m_szImg.cx, m_szImg.cy);
	// DCにビットマップをセット
	m_dcMem.SelectObject(&m_bmpMem);
	// フォントオブジェクトを作成
	m_fnt.CreateFont( m_nFontHeight,0,0,0,0,
						FALSE, FALSE, FALSE,
						DEFAULT_CHARSET,
						OUT_DEFAULT_PRECIS,
						CLIP_DEFAULT_PRECIS,
						DEFAULT_QUALITY,
						FIXED_PITCH | FF_MODERN,
						pFontName );

	// DCにフォントを設定
	m_dcMem.SelectObject(&m_fnt);
	return 0;
}

/**** important *********************************************
  this function is check to multi bytes (for japanese) code. 
  please change check logic as your language code.
 ************************************************************/
/*
 * 2バイト文字の先頭バイトチェック
 */
int CFontImage::IsLeadByte(unsigned char c)
{
	return (0x81 <= c && c <= 0x9f || 0xe0 <= c && c <= 0xfc);
}

/*
 * スペース文字を制御コードに置き換える
 * グラフィックデータよりもサイズを小さく抑えられる為
 */
bool CFontImage::CheckSpaceString(const char* pOne)
{
	int size=0;
	int nPos=0;

	if( ((int)strlen(pOne))==1 ) {
		// 半角
		if(*pOne==' ')
			size = m_szImg.cx/2;
	} else {
		// 全角
		if( strcmp(pOne, "　")==0 )
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

	// バッファを初期化
	memset(m_pData, 0, m_nBuffSize);
	memset(pOne,	0, sizeof(pOne));

	// 2バイト文字の場合、2バイト処理する
	// （pOne へ処理するデータをコピー）
	if( IsLeadByte((unsigned char)*pText) ) {
		strncpy(pOne, pText, 2);
		pText+=2;
	} else
		*pOne = *pText++;
	
	// スペース文字チェック
	if( CheckSpaceString(pOne) ) return m_pData;

	// 背景を白で塗りつぶす
	m_dcMem.FillSolidRect(0,0,m_szImg.cx,m_szImg.cy,RGB(255,255,255));
	// 文字を出力
	m_dcMem.TextOut(0, 0, pOne, (int)strlen(pOne));

	int nPos=0,nBit;
	int cx = m_szImg.cx/m_nResFlag;

	// 制御コード部
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

	m_nDataSize = m_nBuffSize;	// データサイズをバッファサイズに合わせる（1文字分のグラフィックデータ）
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
		byte *p=GetCharImage(pText);	// 文字->グラフィックデータ作成
		buffList.Add(p, GetSize());		// バッファへ追加

		if(*pText && m_bUseCharSpace) {
			// 次に文字が続き、かつ、文字間スペース使用フラグが立っている場合
			// 文字間スペース制御コードを追加する
			buffList.Add(m_pCtrlSpace, sizeof(m_pCtrlSpace));
		}

	} while(*pText);

	nSize = buffList.CreateData();	// データ構築
	pData = new byte[nSize];
	memcpy(pData, buffList.GetData(), nSize);
	
	return 0;
}

/*
 * 文字間スペース量の設定
 */
void CFontImage::SetSpaceSize(short nDot)
{
	if(nDot==0) { m_bUseCharSpace=FALSE; return; }
	m_bUseCharSpace		= TRUE;
	*(m_pCtrlSpace+2)	= (byte)(nDot & 0xff);
	*(m_pCtrlSpace+3)	= (byte)(nDot >> 8);
}



/*
 * コンストラクタ
 */
CFontImageHalf::CFontImageHalf()
: CFontImage()
{
	m_szImg.cx = 12; 
}

/*
 * スペース文字を制御コードに置き換える
 * グラフィックデータよりもサイズを小さく抑えられる為
 */
bool CFontImageHalf::CheckSpaceString(const char* pOne)
{
	int size=0;
	int nPos=0;

	if( ((int)strlen(pOne))==1 ) {
		// 半角
		if(*pOne==' ')
			size = m_szImg.cx;	// 半角文字でもフォントサイズの幅分
	} else {
		// 全角
		if( strcmp(pOne, "　")==0 )
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
