
#ifndef __FONTIMAGE_H__
#define __FONTIMAGE_H__

/*
 * data list
 */
class CBuffList : public CObject
{
public:
	CBuffList();
	~CBuffList();

	CBuffList(byte* pData, int nSize);
	CBuffList* AddNext(byte* pData, int nSize);
	CBuffList* GetNext(){ return m_pNext; }
	const byte* GetData() { return m_pData; }
	int SetData(byte* pData, int nSize);
	int GetSize() { return m_nSize; }

	static int GetListCount(CBuffList* pFirst) {	// count list data
		CBuffList *pCurr=NULL, *pTemp=NULL;
		int nCnt=1;
		
		pCurr = pFirst;
		while( pTemp=pCurr->GetNext() ) {
			pCurr = pTemp;
			nCnt++;
		}
		return nCnt;
	}

private:
	int m_nSize;
	byte* m_pData;
	CBuffList* m_pNext;
};

/*
 * For one characters
 */
class CFontImage : public CObject
{
public:
	CFontImage();
	~CFontImage();

	int Init(const char* pFontName, SIZE* pszImage = NULL);
	byte* GetCharImage(const char*& pText);
	int GetSize() { return m_nDataSize; }
	int IsLeadByte(unsigned char c);

protected:
private:
	CDC 	m_dcMem;
	CBitmap m_bmpMem;
	CFont	m_fnt;
	SIZE	m_szImg;
	byte*	m_pData;
	int		m_nDataSize;
};

/*
 * For one lines
 */
class CFontImageLine : public CObject
{
public:
	CFontImageLine(){};
	~CFontImageLine(){};

	int GetImageData(const char* pText, byte*& pData, int& nSize);
protected:
private:
	CFontImage m_fntImg;
};

#endif //__FONTIMAGE_H__
