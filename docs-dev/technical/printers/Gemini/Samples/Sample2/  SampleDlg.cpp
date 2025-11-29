// SampleDlg.cpp
//

#include "stdafx.h"
#include "Sample.h"
#include "SampleDlg.h"
#include "PrintOut.h"
#include "SettingDlg.h"
#include "math.h"
#include "FontImage.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSampleDlg dialog

CSampleDlg::CSampleDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CSampleDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSampleDlg)

	//}}AFX_DATA_INIT
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CSampleDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSampleDlg)

	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CSampleDlg, CDialog)
	//{{AFX_MSG_MAP(CSampleDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_PRINT, OnPrint)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSampleDlg message handler

BOOL CSampleDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	SetIcon(m_hIcon, TRUE);
	SetIcon(m_hIcon, FALSE);
	
	return TRUE; 
}


void CSampleDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this);

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

HCURSOR CSampleDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CSampleDlg::OnPrint() 
{
	byte* pDt=NULL;
	int nn=0;
	CFontImageLine fntImg;
	fntImg.GetImageData("Ç†Ç¢Ç§Ç¶Ç®Å@ÇƒÇ∑Ç∆", pDt, nn);
	delete[] pDt;

	return;

	CPrintOut prn;
	char pPortName[100];
	char *pFileName[]	= { "./ctrl.dmp","./text.txt","./braille.dmp" };
	
	CSettingDlg dlg;

	if( dlg.DoModal()==IDOK ) {
		int nlen[3];
		char *pData[3];

		TRY {
			CFile f;
			for(int i=0;i<3;i++) {
				if( i==1 ) {
					byte *tmp;
					int n = GetGraphic("Ç†Ç¢Ç§Ç¶Ç®Å@ÇƒÇ∑Ç∆",tmp);
					nlen[i] = n+7;	// stx 0 1 nn nn ... cr lf
					pData[i] = new char[nlen[i]];
					char* p = pData[i];
					*(p++)=2;
					*(p++)=0;
					*(p++)=1;
					short nn = (short)n+2;
					memcpy(p,&nn,sizeof(short));
					p+=2;
					memcpy(p, tmp, n);
					p+=n;
					*(p++)=0x0d;
					*p=0x0a;

					delete[] tmp;
				} else {
					f.Open( pFileName[i], CFile::modeRead );
					nlen[i] = f.GetLength();
					pData[i] = new char[nlen[i]];
					f.Read(pData[i], nlen[i]);
					f.Close();
				}
			}

		} CATCH( CFileException, e ) {
		   #ifdef _DEBUG
			  afxDump << "File could not be opened " << e->m_cause << "\n";
		   #endif
		}
		END_CATCH

		strcpy(pPortName, dlg.m_strOutputPort);

		int n = nlen[0]+nlen[1]+nlen[2];
		char* p = new char[n];
		char* p2 = p;

		for(int i=0;i<3;i++) {
			memcpy(p2,pData[i],nlen[i]);
			p2 += nlen[i];

			delete[] pData[i];
		}

//		prn.PrintDone(p, n, dlg.m_bOutputPort, pPortName);
		delete[] p;

	} else return;
}

int CSampleDlg::GetGraphic(const char* pText, byte*& pData)
{
	CSize szLine(32*(24+6),24);
	CDC* pDC, dcMem;
	CBitmap bmpMem, *pOldBmp;
	
	pDC = GetDC();
	dcMem.CreateCompatibleDC(pDC);
	bmpMem.CreateCompatibleBitmap(pDC, szLine.cx, szLine.cy);
	pOldBmp = dcMem.SelectObject(&bmpMem);

	CFont fntMoji, *pOldFont;
	fntMoji.CreateFont( 25,0,0,0,0,
						FALSE, FALSE, FALSE,
						DEFAULT_CHARSET,
						OUT_DEFAULT_PRECIS,
						CLIP_DEFAULT_PRECIS,
						DEFAULT_QUALITY,
						FIXED_PITCH | FF_MODERN,
						"ÇlÇr ÉSÉVÉbÉN");

	pOldFont = dcMem.SelectObject(&fntMoji);
	dcMem.FillSolidRect(0,0,szLine.cx,szLine.cy,RGB(255,255,255));
	dcMem.TextOut(0,0,pText,strlen(pText));
//	CRect rct(0,0,strlen(pText)*2*24,24);
//	dcMem.DrawText(pText,strlen(pText),&rct,DT_BOTTOM );
//	dcMem.ExtTextOut(0,0,ETO_CLIPPED,&rct,pText,strlen(pText),0);

	CDC* pDlgDC = GetDC();
	pDlgDC->BitBlt(0,-1,szLine.cx,szLine.cy,&dcMem,0,0,SRCCOPY);

	byte buff[4096];
	memset(buff,0,sizeof(buff));
	int nPos = 0;
	buff[nPos++] = 0x1b;
	buff[nPos++] = '*';
	buff[nPos++] = 39;
	int nTxtLen = strlen(pText);
	int nDot = nTxtLen * 30;

	buff[nPos++] = (nDot % 256);
	buff[nPos++] = (nDot / 256);

	int dt=0;
	int nBit=0;

	for(int x=0;x<nDot;x++) {
		for(int y=0;y<8*3;y++) {
			if( (nBit=(y+1)%8)==0 ) nPos++;
			if( dcMem.GetPixel(x,y)!=0xffffff )
				buff[nPos] |= (byte)pow(2,(7-nBit));
		}
	}

	nPos++;
	
	pData = new byte[nPos];
	memcpy(pData, buff, nPos);
	
	dcMem.SelectObject(pOldFont);
	dcMem.SelectObject(pOldBmp);
	dcMem.DeleteDC();
	bmpMem.DeleteObject();
	fntMoji.DeleteObject();

	return nPos;
}

