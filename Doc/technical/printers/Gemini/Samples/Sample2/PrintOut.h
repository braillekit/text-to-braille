
#ifndef __PRINTOUT_H__
#define __PRINTOUT_H__

class CPrintOut : public CObject
{
public:
	CPrintOut();
	~CPrintOut();

	int PrintDone(const char* pData, int nlen, BOOL bDirectPort, char* pPortName);
	
private:
protected:
	int OpenOutputPort(char* pPort);
	int OpenOutputPrinter(char* pPort);
	void CloseOutputPort();
	void CloseOutputPrinter();

	BOOL OutputData(LPVOID pBuff, DWORD dwSize, LPDWORD pdwWritten);
	HANDLE m_hOutput;
	BOOL m_bOutputDirectPort;	// this flag is that output to the port or printer.
};

#endif //__PRINTOUT_H__
