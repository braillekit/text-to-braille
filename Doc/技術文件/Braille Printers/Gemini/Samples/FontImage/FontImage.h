
#ifndef __FONTIMAGE_H__
#define __FONTIMAGE_H__

class CDataBuff : public CObject
{
	friend class CDataBuffList;
public:
	CDataBuff(byte* pData, int nSize);	// コンストラクタ
	~CDataBuff();						// デストラクタ

protected:
	int m_nSize;			// バッファサイズ
	byte* m_pData;			// データバッファ
	CDataBuff* m_pNext;		// 次のCDataBuffポインタ
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

	// 初期化
	int Init(const char* pFontName, bool isHalfResolution = TRUE, int nFontHeight = 24);

	// データ取得
	byte* GetCharImage(const char*& pText);

	// サイズ取得
	int GetSize() { return m_nDataSize; }

	// 2バイト文字のリードバイトチェック
	int IsLeadByte(unsigned char c);

	// 文字間スペース量設定
	void SetSpaceSize(short nDot);

	// グラフィックデータを取得
	int GetImageData(const char* pText, byte*& pData, int& nSize);

protected:
	// スペース文字を制御（相対位置移動）へ置き換える
	virtual bool CheckSpaceString(const char* pOne);

protected:
	CDC 	m_dcMem;			// メモリDC
	CBitmap m_bmpMem;			// ビットマップ
	CFont	m_fnt;				// フォント
	int		m_nFontHeight;		// フォントの高さ
	SIZE	m_szImg;			// 1文字のドットサイズ（固定:24*24dot）
	byte*	m_pData;			// データバッファ
	int		m_nDataSize;		// データサイズ（実際に使用したデータサイズ）
	int		m_nBuffSize;		// バッファサイズ（MAX）

	bool	m_bHalfResolution;	// 横の解像度フラグ	TRUE:90dpi、FALSE:180dpi
	int		m_nResFlag;			// 解像度フラグ	1:180dpi、2:90dpi

	BOOL	m_bUseCharSpace;	// 文字間スペース有無フラグ
	byte	m_pCtrlSpace[4];	// 文字間スペース制御データ
};

/*
 * 半角文字Only用
 */
class CFontImageHalf : public CFontImage
{
public:
	CFontImageHalf();
	~CFontImageHalf(){}

protected:
	// スペース文字を制御（相対位置移動）へ置き換える
	bool CheckSpaceString(const char* pOne);
};

#endif //__FONTIMAGE_H__
