// SampleDlg.h
//

#if !defined(AFX_SAMPLEDLG_H__3DF28ABB_9A7B_4ED3_8235_FBB918BFE846__INCLUDED_)
#define AFX_SAMPLEDLG_H__3DF28ABB_9A7B_4ED3_8235_FBB918BFE846__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CSampleDlg dialog

class CSampleDlg : public CDialog
{
//
public:
	CSampleDlg(CWnd* pParent = NULL);	// default constractor

	//{{AFX_DATA(CSampleDlg)
	enum { IDD = IDD_SAMPLE_DIALOG };

	//}}AFX_DATA

	//{{AFX_VIRTUAL(CSampleDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// support DDX/DDV
	//}}AFX_VIRTUAL

protected:
	HICON m_hIcon;

	//{{AFX_MSG(CSampleDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnPrint();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}

#endif // !defined(AFX_SAMPLEDLG_H__3DF28ABB_9A7B_4ED3_8235_FBB918BFE846__INCLUDED_)
