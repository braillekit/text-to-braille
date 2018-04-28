// Sample.cpp
//

#include "stdafx.h"
#include "Sample.h"
#include "SampleDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CSampleApp

BEGIN_MESSAGE_MAP(CSampleApp, CWinApp)
	//{{AFX_MSG_MAP(CSampleApp)
	//}}AFX_MSG
	ON_COMMAND(ID_HELP, CWinApp::OnHelp)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CSampleApp class

CSampleApp::CSampleApp()
{
}

/////////////////////////////////////////////////////////////////////////////
// CSampleApp object

CSampleApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CSampleApp class init

BOOL CSampleApp::InitInstance()
{
	AfxEnableControlContainer();


#ifdef _AFXDLL
	Enable3dControls();			// dynamic link
#else
	Enable3dControlsStatic();	// static link
#endif

	CSampleDlg dlg;
	m_pMainWnd = &dlg;
	int nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
	}
	else if (nResponse == IDCANCEL)
	{
	}

	return FALSE;
}
