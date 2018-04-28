#if !defined(AFX_SETTINGDLG_H__98FEEA4A_EDD2_4EB9_8CCA_E10B279F6E1E__INCLUDED_)
#define AFX_SETTINGDLG_H__98FEEA4A_EDD2_4EB9_8CCA_E10B279F6E1E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SettingDlg.h : ヘッダー ファイル
//

/////////////////////////////////////////////////////////////////////////////
// CSettingDlg ダイアログ

class CSettingDlg : public CDialog
{
// コンストラクション
public:
	CSettingDlg(CWnd* pParent = NULL);   // 標準のコンストラクタ

// ダイアログ データ
	//{{AFX_DATA(CSettingDlg)
	enum { IDD = IDD_SETTING };
		// メモ: ClassWizard はこの位置にデータ メンバを追加します。
	//}}AFX_DATA


// オーバーライド
	// ClassWizard は仮想関数のオーバーライドを生成します。
	//{{AFX_VIRTUAL(CSettingDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート
	//}}AFX_VIRTUAL

// インプリメンテーション
protected:

	// 生成されたメッセージ マップ関数
	//{{AFX_MSG(CSettingDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnRadDriver();
	afx_msg void OnRadDirect();
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()

private:
	void GetPrinterDriverList();
	void ChangeEnable(BOOL bPort);

public:
	CString m_strOutputPort;
	BOOL m_bOutputPort;
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ は前行の直前に追加の宣言を挿入します。

#endif // !defined(AFX_SETTINGDLG_H__98FEEA4A_EDD2_4EB9_8CCA_E10B279F6E1E__INCLUDED_)
