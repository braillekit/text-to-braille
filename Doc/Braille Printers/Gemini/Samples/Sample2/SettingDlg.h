#if !defined(AFX_SETTINGDLG_H__98FEEA4A_EDD2_4EB9_8CCA_E10B279F6E1E__INCLUDED_)
#define AFX_SETTINGDLG_H__98FEEA4A_EDD2_4EB9_8CCA_E10B279F6E1E__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// SettingDlg.h : �w�b�_�[ �t�@�C��
//

/////////////////////////////////////////////////////////////////////////////
// CSettingDlg �_�C�A���O

class CSettingDlg : public CDialog
{
// �R���X�g���N�V����
public:
	CSettingDlg(CWnd* pParent = NULL);   // �W���̃R���X�g���N�^

// �_�C�A���O �f�[�^
	//{{AFX_DATA(CSettingDlg)
	enum { IDD = IDD_SETTING };
		// ����: ClassWizard �͂��̈ʒu�Ƀf�[�^ �����o��ǉ����܂��B
	//}}AFX_DATA


// �I�[�o�[���C�h
	// ClassWizard �͉��z�֐��̃I�[�o�[���C�h�𐶐����܂��B
	//{{AFX_VIRTUAL(CSettingDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g
	//}}AFX_VIRTUAL

// �C���v�������e�[�V����
protected:

	// �������ꂽ���b�Z�[�W �}�b�v�֐�
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
// Microsoft Visual C++ �͑O�s�̒��O�ɒǉ��̐錾��}�����܂��B

#endif // !defined(AFX_SETTINGDLG_H__98FEEA4A_EDD2_4EB9_8CCA_E10B279F6E1E__INCLUDED_)
