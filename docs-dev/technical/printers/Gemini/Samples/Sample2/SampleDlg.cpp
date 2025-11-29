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
					//
					// this sample is print text only 1 line. 
					//
					byte* pDt=NULL;
					int n=0;
					CFontImageLine fntImg;

					//
					// please change string data (multi byte string)
					// fntImg.GetImageData("[chinese strings here ...]", pDt, n);
					//
					// ****************************************************
					// *** The buffer of a printer should not overflow. ***
					// *** This program has not checked data size.      ***
					// ****************************************************
					//
					fntImg.GetImageData("abcdefg hijklmn", pDt, n);
					
					nlen[i] = n+7;	// +7 ---> stx 0 1 nn nn ... cr lf
					pData[i] = new char[nlen[i]];
					char* p = pData[i];
					*(p++)=2;		// stx
					*(p++)=0;		// 0
					*(p++)=1;		// 1

					// data size
					short nn = (short)n+2;
					memcpy(p,&nn,sizeof(short));
					p+=2;
					
					// copy data
					memcpy(p, pDt, n);
					p+=n;
					
					// CR+LF
					*(p++)=0x0d;
					*p=0x0a;

					delete[] pDt;

				} else {
					//
					// read data from file
					// pFileName[1]('./text.txt') is not use in this sample
					//
					// read control data and braille data from file
					//
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

		prn.PrintDone(p, n, dlg.m_bOutputPort, pPortName);
		delete[] p;

	} else return;
}

