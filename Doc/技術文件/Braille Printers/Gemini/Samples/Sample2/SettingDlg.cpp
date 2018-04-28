// SettingDlg.cpp : インプリメンテーション ファイル
//

#include "stdafx.h"
#include "sample.h"
#include "SettingDlg.h"
#include "winspool.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSettingDlg ダイアログ


CSettingDlg::CSettingDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CSettingDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CSettingDlg)
		// メモ - ClassWizard はこの位置にマッピング用のマクロを追加または削除します。
	//}}AFX_DATA_INIT
}


void CSettingDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CSettingDlg)
		// メモ - ClassWizard はこの位置にマッピング用のマクロを追加または削除します。
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(CSettingDlg, CDialog)
	//{{AFX_MSG_MAP(CSettingDlg)
	ON_BN_CLICKED(IDC_RAD_DRIVER, OnRadDriver)
	ON_BN_CLICKED(IDC_RAD_DIRECT, OnRadDirect)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSettingDlg メッセージ ハンドラ

BOOL CSettingDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	

	// ドライバ情報を取得
	GetPrinterDriverList();
	CComboBox *pcmb = (CComboBox*)GetDlgItem(IDC_CMB_DRIVER);
	pcmb->SetTopIndex(0);

	// ポート情報を取得
	DWORD dwUse = 0;
	DWORD dwNum = 0;
	int res = EnumPorts( NULL, 1, NULL, 0, &dwUse, &dwNum );
	if(!dwUse) {
		return TRUE;;
	}

	BYTE* pData = new BYTE[dwUse];
	res = EnumPorts( NULL, 1, pData, dwUse, &dwUse, &dwNum );
	if(!res) {
		delete pData;
		return TRUE;;
	}

	pcmb = (CComboBox*)GetDlgItem(IDC_CMB_PORT);
	PORT_INFO_1* pPortInfo = (PORT_INFO_1*)pData;
	for(int i=0;i<(int)dwNum;i++)
		pcmb->AddString( pPortInfo[i].pName );

	pcmb->SetTopIndex(0);

	delete pData;

	((CButton*)GetDlgItem(IDC_RAD_DIRECT))->SetCheck(TRUE);
	ChangeEnable(TRUE);

	return TRUE;  
}

void CSettingDlg::GetPrinterDriverList()
{
	DWORD dwUse=0, dwNum=0;
	
	// 取得するPRINTER_INFO_2データを取得するための情報を取得
	BOOL bRes=EnumPrinters( PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS,
							NULL,
							2,
							NULL,
							0,
							&dwUse,
							&dwNum);
	
	if( dwUse==0) return;

	// PRINTER_INFO_2データを取得
	BYTE *pData = new BYTE[dwUse];
	bRes=EnumPrinters( PRINTER_ENUM_LOCAL| PRINTER_ENUM_CONNECTIONS,
							NULL,
							2,
							pData,
							dwUse,
							&dwUse,
							&dwNum);

	
	if(!bRes) {
		delete pData;
		return;
	}

	PRINTER_INFO_2 *pPrn = (PRINTER_INFO_2*)pData;				//

	// コンボのリストへプリンター名称を追加
	CComboBox *pcmb;
	pcmb = (CComboBox*)GetDlgItem(IDC_CMB_DRIVER);

	for(int i=0;i<(int)dwNum;i++) {
		pcmb->AddString( pPrn[i].pPrinterName );
	}

	delete pPrn;
}

void CSettingDlg::OnRadDriver() 
{
	ChangeEnable(FALSE);
}

void CSettingDlg::OnRadDirect() 
{
	ChangeEnable(TRUE);
}

void CSettingDlg::ChangeEnable(BOOL bPort)
{
	GetDlgItem(IDC_CMB_PORT)->EnableWindow(bPort);
	GetDlgItem(IDC_CMB_DRIVER)->EnableWindow(!bPort);
}


void CSettingDlg::OnOK() 
{
	m_bOutputPort = ((CButton*)GetDlgItem(IDC_RAD_DIRECT))->GetCheck();
	CComboBox* pcmb = (CComboBox*)GetDlgItem(m_bOutputPort ? IDC_CMB_PORT : IDC_CMB_DRIVER);
	int n = pcmb->GetCurSel();
	if( n==CB_ERR) return;
	pcmb->GetLBText(n, m_strOutputPort);

	CDialog::OnOK();
}
