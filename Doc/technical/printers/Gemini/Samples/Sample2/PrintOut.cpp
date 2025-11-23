#include "stdafx.h"
#include "PrintOut.h"
#include "Winspool.h"
/*
#define		CODE_ESC		0x1b
#define		CODE_STA_K		'K'
#define		CODE_KANJI		0x05
#define		CODE_STA_H		'H'
#define		CODE_STX		0x02
#define		CODE_SOH		0x01
#define		CODE_FF			0x0c
#define		CODE_ETX		0x03
#define		CODE_PRINTEND	'Z'
#define		CODE_FS			0x1c
*/
/******************************************************************************
[NAME]
[PARAMS]
[RETURN]
******************************************************************************/
CPrintOut::CPrintOut()
{
	m_bOutputDirectPort = TRUE;
}

/******************************************************************************
[NAME]
[PARAMS]
[RETURN]
******************************************************************************/
CPrintOut::~CPrintOut()
{
}

/******************************************************************************
[NAME]
[PARAMS]
[RETURN]
******************************************************************************/
int CPrintOut::PrintDone(const char* pData, int nlen, BOOL bDirectPort, char* pPortName)
{
	DWORD dwNum=0;
	int res=0;

	m_bOutputDirectPort = bDirectPort;
	int (CPrintOut::* pOpenTarget)(char* pPort);
	void (CPrintOut::* pCloseTarget)();

	// The pointer to the member function for PortOpen and PortClose is set up.
	//
	pOpenTarget = ( bDirectPort ? &CPrintOut::OpenOutputPort : &CPrintOut::OpenOutputPrinter );
	pCloseTarget= ( bDirectPort ? &CPrintOut::CloseOutputPort: &CPrintOut::CloseOutputPrinter);

	res = (this->* pOpenTarget)(pPortName);	// open the output port.
	if( res!= 0 ) return res;

	res = (this->* OutputData)((void*)pData, nlen, &dwNum);

	(this->* pCloseTarget)();	// close output port

	return 0;
}

/******************************************************************************
[NAME]	open the printer
[PARAMS]
[RETURN]	success => 0
******************************************************************************/
int CPrintOut::OpenOutputPrinter(char* pPort)
{
	if( OpenPrinter(pPort, &m_hOutput, NULL ) == 0 ) return -1;

	char pDocName[] = "BrailleDocument";

	DOC_INFO_1 docinfo;
	docinfo.pDocName = pDocName;
	docinfo.pDatatype	= NULL;
	docinfo.pOutputFile	= NULL;
	return StartDocPrinter(m_hOutput, 1, (LPBYTE)&docinfo)==0 ? -1 : 0;
}

/******************************************************************************
[NAME]	close the printer handle
[PARAMS]
[RETURN]
******************************************************************************/
void CPrintOut::CloseOutputPrinter()
{
	if( m_hOutput ) {
		EndDocPrinter( m_hOutput );
		ClosePrinter( m_hOutput );
	}
	m_hOutput=NULL;
}

/******************************************************************************
[NAME]	close the output port
[PARAMS]
[RETURN]
******************************************************************************/
void CPrintOut::CloseOutputPort()
{
	if( m_hOutput ) CloseHandle( m_hOutput );
	m_hOutput=NULL;
}

/******************************************************************************
[NAME]	open the output port
[PARAMS]
[RETURN]
******************************************************************************/
int CPrintOut::OpenOutputPort(char* pPort)
{
	DCB dcb;
	BOOL bRes;

	m_hOutput = CreateFile( pPort,
						 GENERIC_READ | GENERIC_WRITE,
						 0,						// comm devices must be opened w/exclusive-access
						 NULL,					// no security attributes
						 OPEN_EXISTING,			// comm devices must use OPEN_EXISTING
						 NULL,					// not overlapped I/O
						 NULL					// hTemplate must be NULL for comm devices
						);

	if ( m_hOutput == INVALID_HANDLE_VALUE) {
		m_hOutput=NULL;
		return -1;
	}

	// check the port name
	// It return except the "COM" port.
	//
	char buff[100];
	strcpy(buff, pPort);
	buff[strlen(buff)-2]=NULL;
	if( stricmp(buff, "com") !=0 ) return 0;

	bRes = GetCommState( m_hOutput, &dcb );

	if (!bRes) {
		CloseHandle(m_hOutput);
		m_hOutput=NULL;
		return -2;
	}

	// Fill in the DCB: baud=9,600 bps, 8 data bits, no parity, and 1 stop bit.
	dcb.BaudRate	= CBR_9600;		// set the baud rate
	dcb.ByteSize	= 8;			// data size, xmit, and rcv
	dcb.Parity		= NOPARITY;		// no parity bit
	dcb.StopBits	= ONESTOPBIT;	// one stop bit
	dcb.fOutX		= TRUE;			// XON/XOFF out flow control
	bRes = SetCommState( m_hOutput, &dcb );

	if (!bRes) {
		CloseHandle(m_hOutput);
		m_hOutput=NULL;
		return -3;
	}

	return 0;
}

/******************************************************************************
[NAME]	output the data
[PARAMS]
[RETURN]
******************************************************************************/
BOOL CPrintOut::OutputData(LPVOID pBuff, DWORD dwSize, LPDWORD pdwWritten)
{
	BOOL bRes = FALSE;
	if( m_bOutputDirectPort ) 
		bRes = WriteFile(m_hOutput, pBuff, dwSize, pdwWritten, NULL);	// output to the port;
	else
		bRes = WritePrinter(m_hOutput, pBuff, dwSize, pdwWritten);		// output to the printer

	if( !bRes ) {
		int res = GetLastError();
	}

	return bRes;
}
