// Sample.h 
//

#if !defined(AFX_SAMPLE_H__7A9D5D43_AF2F_4E51_86D2_33037DC2D86C__INCLUDED_)
#define AFX_SAMPLE_H__7A9D5D43_AF2F_4E51_86D2_33037DC2D86C__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbol

/////////////////////////////////////////////////////////////////////////////
// CSampleApp:
// 
//

class CSampleApp : public CWinApp
{
public:
	CSampleApp();

	// ClassWizard 
	//{{AFX_VIRTUAL(CSampleApp)
	public:
	virtual BOOL InitInstance();
	//}}AFX_VIRTUAL

	//{{AFX_MSG(CSampleApp)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};


/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// 

#endif // !defined(AFX_SAMPLE_H__7A9D5D43_AF2F_4E51_86D2_33037DC2D86C__INCLUDED_)
