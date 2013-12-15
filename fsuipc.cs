/**
*   FSUIPC Developer Kit: Library for C#.NET programmers
*   Version 1.11 (03/05/2004) by Scott McCrory <scott@mccrory.us> and Bob Scott <w6kd@yahoo.com>
*
*   Original Copyright (?) 2000 Chris Brett.  All rights reserved.
*   e-mail: chris@formulate.clara.net
*
*   Rewritten for Visual Basic .NET By Bob Scott
*   e-mail w6kd@amsat.org
*
*   Rewritten for C#.NET by Scott McCrory and revised by Bob Scott
*   e-mail scott@mccrory.us
*
*   Function library for FSUIPC based on C code supplied by Pete Dowson
*
*   BASIC USAGE GOES LIKE THIS:
*      0. Instantiate the FsuipcSdk.Fsuipc class (i.e. Fsuipc f = new Fsuipc();).
*      1. Call f.FSUIPC_Initialization() at beginning of program (optional).
*      2. Call f.FSUIPC_Open() to connect with FSUIPC if it's available.
*      3. Call f.FSUIPC_Read and/or f.FSUIPC_Write one or more times to build
*         the list of actions to be performed by the next step.
*      4. Call f.FSUIPC_Process() to actually get/set the data from/to FS.
*      5. For reads, call f.FSUIPC_Get to retrieve data read from FSUIPC.
*      6. Repeat steps 3, 4, and 5 as necessary.
*      7. At program termination, call f.FSUIPC_Close().
*
**/

using System.Runtime.InteropServices;
using System.Text;
using System;

namespace FsuipcSdk
{

	public class Fsuipc 
	{

		// Constants
		public const int SMTO_ABORTIFHUNG = 0x2;
		public const int SMTO_BLOCK = 0x1;
		public const int SMTO_NORMAL = 0x0;
		public const int PAGE_READWRITE = 0x4;
		public const int NO_ERROR = 0;
		public const int ERROR_ALREADY_EXISTS = 0x183;
		public const int SECTION_MAP_WRITE = 0x2;
		public const int FILE_MAP_WRITE = SECTION_MAP_WRITE;

		// FSUIPC SPECIFICS
		// Supported Sims
		public const int SIM_ANY = 0;
		public const int SIM_FS98 = 1;
		public const int SIM_FS2K = 2;
		public const int SIM_CFS2 = 3;
		public const int SIM_CFS1 = 4;
		public const int SIM_FLY = 5;
		public const int SIM_FS2K2 = 6;
		public const int SIM_FS2K4 = 7;
		public const int SIM_FSX = 8;
		public const int SIM_ESP = 9;
		

		// Error numbers
		public const int FSUIPC_ERR_OK = 0;
		public const int FSUIPC_ERR_OPEN = 1;              // Attempt to Open when already Open
		public const int FSUIPC_ERR_NOFS = 2;              // Cannot link to FSUIPC or WideClient
		public const int FSUIPC_ERR_REGMSG = 3;            // Failed to Register common message with Windows
		public const int FSUIPC_ERR_ATOM = 4;              // Failed to create Atom for mapping filename
		public const int FSUIPC_ERR_MAP = 5;               // Failed to create a file mapping object
		public const int FSUIPC_ERR_VIEW = 6;              // Failed to open a view to the file map
		public const int FSUIPC_ERR_VERSION = 7;           // Incorrect version of FSUIPC, or not FSUIPC
		public const int FSUIPC_ERR_WRONGFS = 8;           // Sim is not version requested
		public const int FSUIPC_ERR_NOTOPEN = 9;           // Call cannot execute, link not Open
		public const int FSUIPC_ERR_NODATA = 10;           // Call cannot execute: no requests accumulated
		public const int FSUIPC_ERR_TIMEOUT = 11;          // IPC timed out all retries
		public const int FSUIPC_ERR_SENDMSG = 12;          // IPC sendmessage failed all retries
		public const int FSUIPC_ERR_DATA = 13;             // IPC request contains bad data
		public const int FSUIPC_ERR_RUNNING = 14;          // Maybe running on WideClient, but FS not running on Server, or wrong FSUIPC
		public const int FSUIPC_ERR_SIZE = 15;
		public const int FSUIPC_ERR_BUFOVERFLOW = 16;

		public const int IPC_BUFFER_SIZE = 65568;
		public const int LEN_RD_HDR = 16;                  // Length of an IPC Read Header (4x 4-byte Integers)
		public const int LEN_WR_HDR = 12;                  // Length of an IPC Write Header (3x 4-byte Integers)

		public const int LIB_VERSION = 2000;              // 2.000
		public const int MAX_SIZE = 0x7F00;               // Largest data (kept below 32k to avoid

		// any possible 16-bit sign problems
		public const int MAX_REQ_SIZE = 1024;             // Maximum single request block size

		public const string FS6IPC_MSGNAME1 = "FSASMLIB:IPC";
		public const string FS6IPC_MSGNAME2 = "EFISFSCOM:IPC";
		public const int FS6IPC_MESSAGE_SUCCESS = 1;
		public const int FS6IPC_MESSAGE_FAILURE = 0;

		// IPC message types
		public const int FS6IPC_READSTATEDATA_ID = 1;
		public const int FS6IPC_WRITESTATEDATA_ID = 2;
		public const int FS6IPC_SPECIALREQUEST_ID = 0xABAC;

		// Instance variables for the UIPC communication
		byte[] IPC = new byte[IPC_BUFFER_SIZE];		// IPC Data Buffer
		bool[] IPCdr = new bool[IPC_BUFFER_SIZE];   // Data waiting flags
		int IPCAlloc;								// Data Buffer Allocation Index
		int FSUIPC_Version;
		int FSUIPC_FS_Version;
		int FSUIPC_Lib_Version = 0;
		int m_hWnd;       // FS window handle
		int m_msg;        // ID of registered window message
		int m_atom;       // global atom containing name of file-mapping object
		int m_hMap;       // handle of file-mapping object
		int m_pView;      // pointer to view of file-mapping object
		int m_pNext;      // pointer into FSUIPC data buffer area

		// External Win32 libraries
		public const string KER_DLL = "kernel32.dll";	// Import library for Kernel on Win32
		public const string USR_DLL = "user32.dll";		// Import library for User on Win32
		
		// We'll need to access some Windows API functions located in kernel32.dll
		[ DllImport(KER_DLL) ] public static extern void RtlZeroMemory(IntPtr dDestination, int length);
		[ DllImport(KER_DLL) ] public static extern void RtlMoveMemory(IntPtr pDest, IntPtr pSrc, int ByteLen);
		[ DllImport(KER_DLL) ] public static extern void Sleep (int dwMilliseconds);
		[ DllImport(KER_DLL) ] public static extern int GetCurrentProcessId ();
		[ DllImport(KER_DLL) ] public static extern int GlobalAddAtomA(string lpstring);
		[ DllImport(KER_DLL) ] public static extern int CreateFileMappingA(uint hFile, int lpFileMappigAttributes, int flProtect, int dwMaximumSizeHigh, int dwMaximumSizeLow, string lpName);
		[ DllImport(KER_DLL) ] public static extern int GetLastError ();
		[ DllImport(KER_DLL) ] public static extern int MapViewOfFile (int hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap);
		[ DllImport(KER_DLL) ] public static extern int MapViewOfFileEx (int hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap, IntPtr lpBaseAddress);
		[ DllImport(KER_DLL) ] public static extern int GlobalDeleteAtom (int nAtom);
		[ DllImport(KER_DLL) ] public static extern int UnmapViewOfFile (IntPtr lpBaseAddress);
		[ DllImport(KER_DLL) ] public static extern int CloseHandle (int hObject);

		// We'll also need to access some Windows API functions located in user32.dll
		[ DllImport(USR_DLL) ] public static extern int SendMessageTimeoutA(int hwnd, int msg, int wparam, int lparam, int fuFlags, int uTimeout, int lpdwResult);
		[ DllImport(USR_DLL) ] public static extern int FindWindowExA(int hWnd1, int hWnd2, string lpsz1, string lpsz2);
		[ DllImport(USR_DLL) ] public static extern int RegisterWindowMessageA(string lpstring);

		// Our main object contructor
		public Fsuipc ()
		{
			FSUIPC_Close();
			FSUIPC_Initialization();
		}

		// --- Misc toggle functions ---------------------------------------------------
		///<summary>Toggles a byte between 0 and 1</summary>
		///<param name="arg1">The byte to toggle</param>
		///<return>The toggled byte</return>
		public byte toggle(byte arg1) 
		{
			if (arg1 == 0) 
			{
				return 1;
			} 
			else 
			{
				return 0;
			}
		}

		///<summary>Toggles a short between 0 and 1</summary>
		///<param name="arg1">The short to toggle</param>
		///<return>The toggled short</return>
		public short toggle(short arg1) 
		{
			if (arg1 == 0) 
			{
				return 1;
			} 
			else 
			{
				return 0;
			}
		}

		///<summary>Toggles an int between 0 and 1</summary>
		///<param name="arg1">The int to toggle</param>
		///<return>The toggled int</return>
		public int toggle(int arg1) 
		{
			if (arg1 == 0) 
			{
				return 1;
			} 
			else 
			{
				return 0;
			}
		}

		// --- Initialize the Client ---------------------------------------------------
		///<summary>Initializes the FSUIPC Client</summary>
		public void FSUIPC_Initialization() 
		{

			for (int idx = 0; idx < IPC_BUFFER_SIZE; idx++) 
			{
				IPC[idx] = 0;
				IPCdr[idx] = false;
			}

			IPCAlloc = 0;
			m_hWnd = 0;
			m_msg = 0;
			m_atom = 0;
			m_hMap = 0;
			m_pView = 0;
			m_pNext = 0;

		}
	
		// --- Opens/Start the Client --------------------------------------------------------
		///<summary>Opens an FSUIPC client connection</summary>
		///<param name="dwFSReq">An int describing what FS version requested</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Open(int dwFSReq, ref int dwResult) 
		{

			string szName = ""; 
			string szTemp = "";
			bool fWideFS = false;
			int nTry = -1;
			int i = -1;

			// initialize vars
			nTry = 0;
			fWideFS = false;
			i = 0;
			FSUIPC_Version = 0;
			FSUIPC_FS_Version = 0;

			// abort if already started
			if (m_pView != 0) 
			{
				dwResult = FSUIPC_ERR_OPEN;
				return false;
			}

			// Connect via FSUIPC, which is known to be FSUIPC's own
			// and isn't subject to user modification

			m_hWnd = FindWindowExA(0, 0, "UIPCMAIN", null);

			if (m_hWnd == 0) 
			{

				// if there's no UIPCMAIN, we may be using WideClient
				// which only simulates FS98
				m_hWnd = FindWindowExA(0, 0, "FS98MAIN", null);
				fWideFS = true;
				if (m_hWnd == 0) 
				{
					dwResult = FSUIPC_ERR_NOFS;
					return false;
				}

			}

			// register the window message
			m_msg = RegisterWindowMessageA(FS6IPC_MSGNAME1);
			if (m_msg == 0) 
			{
				dwResult = FSUIPC_ERR_REGMSG;
				return false;
			}

			// create the name of our file-mapping object
			nTry = nTry + 1; // Ensures a unique string is used in case user closes and reopens
			szName = FS6IPC_MSGNAME1 + ":" + GetCurrentProcessId().ToString("x") + ":" + nTry.ToString("x") + Convert.ToChar(0).ToString();
			if (szName.Length > 23) 
			{    //24 chars max
				szName = szName.Substring(0, 23) + Convert.ToChar(0).ToString();
			}

			// stuff the name into a global atom
			m_atom = GlobalAddAtomA(szName);

			if (m_atom == 0) 
			{
				dwResult = FSUIPC_ERR_ATOM;
				FSUIPC_Close();
				return false;
			}

			// create the file-mapping object
			// use system paging file
			// security attrubute (0 = can! inherit)
			// protection
			// size
			// name

			m_hMap = CreateFileMappingA(0xFFFFFFFF, 0, PAGE_READWRITE, 0, MAX_SIZE + 256, szName);

			if ((m_hMap == 0) || (GetLastError() == ERROR_ALREADY_EXISTS)) 
			{
				dwResult = FSUIPC_ERR_MAP;
				FSUIPC_Close();
				return false;
			}

			// spawn a view of the file-mapping object
			m_pView = MapViewOfFile(m_hMap, FILE_MAP_WRITE, 0, 0, 0);

			if (m_pView == 0) 
			{
				dwResult = FSUIPC_ERR_VIEW;
				FSUIPC_Close();
				return false;
			}

			// Okay, now determine FSUIPC version AND FS type
			m_pNext = m_pView;

			// Try up to 5 times with a 100msec rest between each
			// Note that WideClient returns zeros initially, whilst waiting
			// for the Server to get the data
			do 
			{

				i++;
				// Read FSUIPC version
				int t_FSUIPC_Version = 0;

				if (! FSUIPC_Read(0x3304, 4, ref t_FSUIPC_Version, ref dwResult)) 
				{
					FSUIPC_Close();
					return false;
				}

				// and FS version and validity check pattern
				int t_FSUIPC_FS_Version = 0;
				if (! FSUIPC_Read(0x3308, 4, ref t_FSUIPC_FS_Version, ref dwResult)) 
				{
					FSUIPC_Close();
					return false;
				}

				// write our Library version number to a special read-only offset
				// This is to assist diagnosis from FSUIPC logging
				// But only do this on first try
				int t_FSUIPC_Lib_Version = 0;
				if (i < 2 && FSUIPC_Write(0x330A, 2, ref t_FSUIPC_Lib_Version, ref dwResult) == false) 
				{
					FSUIPC_Close();
					return false;
				}

				// Actually send the request and get the responses ("process")

				if (!FSUIPC_Process(ref dwResult)) 
				{
					FSUIPC_Close();
					return false;
				}

				bool VersionGet = FSUIPC_Get(ref t_FSUIPC_Version, ref FSUIPC_Version);
				VersionGet = FSUIPC_Get(ref t_FSUIPC_FS_Version, ref FSUIPC_FS_Version);
				VersionGet = FSUIPC_Get(ref t_FSUIPC_Lib_Version, ref FSUIPC_Lib_Version);

				// Maybe running on WideClient, and need another try
				Sleep(100); // Give it a chance
			}
			while ((i < 5) && ((FSUIPC_Version == 0) || (FSUIPC_FS_Version == 0)));

			// Only allow running on FSUIPC 1.998e or later
			// with correct check pattern &HFADE
			if ((FSUIPC_Version < 0x19980005) || ((FSUIPC_FS_Version & 0xFFFF0000) != 0xFADE0000)) 
			{
				if (fWideFS) 
				{
					dwResult = FSUIPC_ERR_RUNNING;
				}
				else 
				{
					dwResult = FSUIPC_ERR_VERSION;
				}
				FSUIPC_Close();
				return false;
			}

			// grab the FS version number
			FSUIPC_FS_Version = (FSUIPC_FS_Version & 0xFFFF);

			// Optional version-specific request made?  if so and wrong version, return error
			if (dwFSReq != 0 && dwFSReq != FSUIPC_FS_Version) 
			{
				dwResult = FSUIPC_ERR_WRONGFS;
				FSUIPC_Close();
				return false;
			}
			dwResult = FSUIPC_ERR_OK;
			return true;

		}

    
		//--- Read request ---------------------------------------------------------------
		///<summary>Submits a read request</summary>
		///<param name="dwOffset">The memory offset where the requested value is located</param>
		///<param name="dwSize">The number of bytes comprising of the requested value</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Read(int dwOffset, int dwSize, ref int Token, ref int dwResult) 
		{
            
			int[] hdr = new int[4];

			if ((IPCAlloc + dwSize + 4) >= (IPC_BUFFER_SIZE - 1)) 
			{  //Reset ptr to start of FifO buf
				IPCAlloc = 0;
			}
			// Assign Token as index into IPC Buffer FifO, and clear data ready flags
			Token = IPCAlloc;
			for (int i = IPCAlloc; i < (IPCAlloc + 4 + dwSize - 1); i++) 
			{ // 4=size of integer datablock size hdr
				IPC[i] = 0;
				IPCdr[i] = false;
			}
			IPCAlloc = IPCAlloc + 4 + dwSize;    //first four bytes data block size (Int) plus data

			if (Token >= IPC_BUFFER_SIZE) 
			{
				Token = -1;
				dwResult = FSUIPC_ERR_BUFOVERFLOW;
				return false;
			}

			// Check link is open
			if (m_pView == 0) 
			{
				dwResult = FSUIPC_ERR_NOTOPEN;
				return false;
			}

			// Check have space for ( this request (including terminator)
			if ((m_pNext - m_pView + 4 + dwSize + LEN_RD_HDR) > MAX_SIZE) 
			{
				dwResult = FSUIPC_ERR_SIZE;
				return false;
			}

			hdr[0] = FS6IPC_READSTATEDATA_ID;     //Read request ID
			hdr[1] = dwOffset;                    //FSUIPC offset
			hdr[2] = dwSize;                      //data element size
			hdr[3] = Token;                       //index into managed data buffer IPC

			IntPtr _pNext = new IntPtr(m_pNext);
			Marshal.Copy(hdr, 0, _pNext, 4);    //Header to FSUIPC data buffer 

			// Move pointer past the Record
			m_pNext = m_pNext + LEN_RD_HDR;
			if (dwSize != 0) 
			{
				// Zero the data reception area, so rubbish won//t be returned
				_pNext = new IntPtr(m_pNext);
				RtlZeroMemory(_pNext, dwSize);
				// Update the pointer ready for ( more data
				m_pNext = m_pNext + dwSize;
			}

			dwResult = FSUIPC_ERR_OK;
			return true;

		}

		//--- Process read/write ------------------------------------------------------
		///<summary>Processes read or write request</summary>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Process(ref int dwResult) 
		{

			int dwError = 0;
			int[] hdr = new int[4];
			int i = 0;
			if (m_pView == 0) 
			{
				dwResult = FSUIPC_ERR_NOTOPEN;
				return false;
			}

			if (m_pView == m_pNext) 
			{
				dwResult = FSUIPC_ERR_NODATA;
				return false;
			}

			// Write FSUIPC Data Area Terminator
			IntPtr _pNext = new IntPtr(m_pNext);
			RtlZeroMemory(_pNext, 4);

			m_pNext = m_pView;

			// send the request (allow up to 10 tries)
			i = 0;
			while(i < 10 && SendMessageTimeoutA(m_hWnd, m_msg, m_atom, 0, SMTO_BLOCK, 5000, dwError) == 0)
            //                         m_hWnd,              // FS6 window handle
            //                         m_msg,               // our registered message id
            //                         m_atom,              // wparam: name of file-mapping object
            //                         0,                   // lparam: offset of request into file-mapping obj
            //                         SMTO_BLOCK,          // halt this thread until we get a response
            //                         2000,                // time out interval
            //                         dwError)) = 0) do begin  // return value
			{
				i++;
				Sleep(100);  // Allow for things to happen

			}


			if (i >= 10) 
			{   // Failed all tries

				if (GetLastError() == 0) 
				{
					dwResult = FSUIPC_ERR_TIMEOUT;
				} 
				else 
				{
					dwResult = FSUIPC_ERR_SENDMSG;
				}
				return false;
			}
			
			// did IPC like the data request?
			//if (dwError != FS6IPC_MESSAGE_SUCCESS) {
			//	// no...
			//	dwResult = FSUIPC_ERR_DATA;
			//	return false;
			//}
			
			// Decode and store results of Read requests
			m_pNext = m_pView;
			_pNext = new IntPtr(m_pNext);

			Marshal.Copy(_pNext, hdr, 0, 1);
			m_pNext = m_pNext + 4;              //Advance ptr 4 bytes for Header pre-read

			// hdr[0) Operation ID (1=Read, 2=Write)
			// hdr[1) FSUIPC Offset value
			// hdr[2) Size of bata block to pass (bytes)
			// hdr[3) Token (offset into IPC FifO buffer)

			while (hdr[0] != 0) 
			{
        
				switch (hdr[0]) 
				{
            
					case FS6IPC_READSTATEDATA_ID:

						// copy the data FSUIPC read into the local FifO buffer
						_pNext = new IntPtr(m_pNext);
						Marshal.Copy(_pNext, hdr, 1, 3);
						m_pNext = m_pNext + LEN_RD_HDR - 4;

						if (hdr[2] != 0)
						{
							_pNext = new IntPtr(m_pNext);
							IntPtr _nBytes = Marshal.AllocHGlobal(4);    //Ptr to # of bytes in record
							Marshal.WriteInt32(_nBytes, hdr[2]);                //Write datablock size as block header
							Marshal.Copy(_nBytes, IPC, hdr[3], 4);              //Write data into FifO buffer
							Marshal.FreeHGlobal(_nBytes);
							Marshal.Copy(_pNext, IPC, hdr[3] + 4, hdr[2]);      //Copy data to IPC(Token)
							IPCdr[hdr[3]] = true;                               //Flag - data available for retrieval
						}

						m_pNext = m_pNext + hdr[2];  //Increment read buffer ptr by size of data element passed
						break;

					case FS6IPC_WRITESTATEDATA_ID:

						// Write transaction, no return data...pull header from datastream and waste
						_pNext = new IntPtr(m_pNext);
						Marshal.Copy(_pNext, hdr, 1, 2);
						m_pNext = m_pNext + LEN_WR_HDR - 4 + hdr[2];
						break;

					default:
						// Invalid Operation ID...abort
						m_pNext = m_pView;
						return false;

				}

				_pNext = new IntPtr(m_pNext);
				Marshal.Copy(_pNext, hdr, 0, 1);
				m_pNext = m_pNext + 4;

			}

			m_pNext = m_pView;
			dwResult = FSUIPC_ERR_OK;
			return true;
    
		}

		//--- Internal Write request ------------------------------------------------------
		///<summary>Submits an "internal" write request, used by FSUIPC_Write</summary>
		///<param name="dwOffset">The memory offset where the referenced value is located</param>
		///<param name="dwSize">The number of bytes comprising of the referenced value</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		private bool FSUIPC_Write_Req(int dwOffset, int dwSize, int Token, ref int dwResult) 
		{

			int[] hdr = new int[4];

			if (Token >= IPC_BUFFER_SIZE) 
			{
				Token = -1;
				dwResult = FSUIPC_ERR_BUFOVERFLOW;
				return false;
			}

			// abort if necessary
			if (m_pView == 0) 
			{
				dwResult = FSUIPC_ERR_NOTOPEN;
				return false;
			}

			// Check have FSUIPC buffer space for this request (including terminator)
			if ((((m_pNext) - (m_pView)) + 4 + (dwSize + LEN_WR_HDR)) > MAX_SIZE) 
			{
				dwResult = FSUIPC_ERR_SIZE;
				return false;
			}

			// Initialise header for write request

			hdr[0] = FS6IPC_WRITESTATEDATA_ID;
			hdr[1] = dwOffset;
			hdr[2] = dwSize;

			IntPtr _pNext = new IntPtr(m_pNext);
			Marshal.Copy(hdr, 0, _pNext, 3);    //Write header to FSUIPC data buffer
			// Move pointer past the record
			m_pNext = m_pNext + LEN_WR_HDR;
			if (dwSize != 0) 
			{
				_pNext = new IntPtr(m_pNext);    //convert int index to ptr into FSUIPC buffer
				Marshal.Copy(IPC, Token + 4, _pNext, dwSize);
				// Update the pointer ready for more data
				m_pNext = m_pNext + dwSize;
			}
			dwResult = FSUIPC_ERR_OK;
			return true;

		}

		//--- Write requests --------------------------------------------------------------
		///<summary>Submits a write request</summary>
		///<param name="dwOffset">The memory offset where the referenced value is located</param>
		///<param name="param">The value to be written</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Write(int dwOffset, byte param, ref int Token, ref int dwResult) 
		{

			int DataElementSize = 1;

			if ((IPCAlloc + DataElementSize + 4) >= (IPC_BUFFER_SIZE - 1)) 
			{  //Reset ptr to startbuf
				IPCAlloc = 0;
			}
			// Assign Token as index into IPC Buffer FifO, and clear data ready flags
			Token = IPCAlloc;
			int i;
			for (i = IPCAlloc; i < (IPCAlloc + 4 + DataElementSize - 1); i++) 
			{ // 4=size of int datablock size hdr
				IPC[i] = 0;
				IPCdr[i] = false;
			}
			IPCAlloc = IPCAlloc + 4 + DataElementSize;    //first four bytes data block size (Int) plus data
			IntPtr heapbuf= Marshal.AllocHGlobal(4 + DataElementSize);
			Marshal.WriteInt32(heapbuf, DataElementSize);      // Translate token to unmanaged byte stream
			Marshal.Copy(heapbuf, IPC, Token, 4);
			Marshal.WriteByte(heapbuf, param);                 // Transloate data and put in IPC array
			Marshal.Copy(heapbuf, IPC, Token + 4, DataElementSize);
			Marshal.FreeHGlobal(heapbuf);
			return FSUIPC_Write_Req(dwOffset, DataElementSize, Token, ref dwResult);
		}

		///<summary>Submits a write request</summary>
		///<param name="dwOffset">The memory offset where the referenced value is located</param>
		///<param name="param">The value to be written</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Write(int dwOffset, short param, ref int Token, ref int dwResult) 
		{

			int DataElementSize = 2;

			if ((IPCAlloc + DataElementSize + 4) >= (IPC_BUFFER_SIZE - 1)) 
			{  //Reset ptr to startbuf
				IPCAlloc = 0;
			}
			// Assign Token as index into IPC Buffer FifO, and clear data ready flags
			Token = IPCAlloc;
			int i;
			for ( i = IPCAlloc; i < (IPCAlloc + 4 + DataElementSize - 1); i++) 
			{ // 4=size of int datablock size hdr
				IPC[i] = 0;
				IPCdr[i] = false;
			}
			IPCAlloc = IPCAlloc + 4 + DataElementSize;    //first four bytes data block size (Int) plus data

			IntPtr heapbuf = Marshal.AllocHGlobal(4 + DataElementSize);
			Marshal.WriteInt32(heapbuf, DataElementSize);      // Translate token to unmanaged byte stream
			Marshal.Copy(heapbuf, IPC, Token, 4);
			Marshal.WriteInt16(heapbuf, param);                 // Transloate data and put in IPC array
			Marshal.Copy(heapbuf, IPC, Token + 4, DataElementSize);
			Marshal.FreeHGlobal(heapbuf);
			return FSUIPC_Write_Req(dwOffset, DataElementSize, Token, ref dwResult);
		}

		///<summary>Submits a write request</summary>
		///<param name="dwOffset">The memory offset where the referenced value is located</param>
		///<param name="param">The value to be written</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Write(int dwOffset, int param, ref int Token, ref int dwResult) 
		{
        
			int DataElementSize = 4;

			if ((IPCAlloc + DataElementSize + 4) >= (IPC_BUFFER_SIZE - 1)) 
			{  //Reset ptr to startbuf
				IPCAlloc = 0;
			}
			// Assign Token as index into IPC Buffer FifO, and clear data + ready flags
			Token = IPCAlloc;
			int i;
			for (i = IPCAlloc; i < (IPCAlloc + 4 + DataElementSize - 1); i++) 
			{ // 4=size of int datablock size hdr
				IPC[i] = 0;
				IPCdr[i] = false;
			}
			IPCAlloc = IPCAlloc + 4 + DataElementSize;    //first four bytes data block size (Int) plus data
			IntPtr heapbuf = Marshal.AllocHGlobal(4 + DataElementSize);
			Marshal.WriteInt32(heapbuf, DataElementSize);      // Translate token to unmanaged byte stream
			Marshal.Copy(heapbuf, IPC, Token, 4);
			Marshal.WriteInt32(heapbuf, param);                 // Transloate data and put in IPC array
			Marshal.Copy(heapbuf, IPC, Token + 4, DataElementSize);
			Marshal.FreeHGlobal(heapbuf);
			return FSUIPC_Write_Req(dwOffset, DataElementSize, Token, ref dwResult);
		}

		///<summary>Submits a write request</summary>
		///<param name="dwOffset">The memory offset where the referenced value is located</param>
		///<param name="param">The value to be written</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Write(int dwOffset, long param, ref int Token, ref int dwResult) 
		{

			int DataElementSize = 8;

			if ((IPCAlloc + DataElementSize + 4) >= (IPC_BUFFER_SIZE - 1)) 
			{  //Reset ptr to startbuf
				IPCAlloc = 0;
			}
			// Assign Token as index into IPC Buffer FifO, and clear data ready flags
			Token = IPCAlloc;
			int i;
			for ( i = IPCAlloc; i < (IPCAlloc + 4 + DataElementSize - 1); i++) 
			{ // 4=size of int datablock size hdr
				IPC[i] = 0;
				IPCdr[i] = false;
			}
			IPCAlloc = IPCAlloc + 4 + DataElementSize;    //first four bytes data block size (Int) plus data
			IntPtr heapbuf = Marshal.AllocHGlobal(4 + DataElementSize);
			Marshal.WriteInt32(heapbuf, DataElementSize);      // Translate token to unmanaged byte stream
			Marshal.Copy(heapbuf, IPC, Token, 4);
			Marshal.WriteInt64(heapbuf, param);                 // Transloate data and put in IPC array
			Marshal.Copy(heapbuf, IPC, Token + 4, DataElementSize);
			Marshal.FreeHGlobal(heapbuf);
			return FSUIPC_Write_Req(dwOffset, DataElementSize, Token, ref dwResult);
		}

		///<summary>Submits a write request</summary>
		///<param name="dwOffset">The memory offset where the referenced value is located</param>
		///<param name="param">The value to be written</param>
		///<param name="token">Contains the unique identifier token used to Get the value</param>
		///<param name="dwResult">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Write(int dwOffset, int dwSize, ref byte[] param, ref int Token, ref int dwResult) 
		{

			if ((IPCAlloc + dwSize + 4) >= (IPC_BUFFER_SIZE - 1)) 
			{  //Reset ptr to startbuf
				IPCAlloc = 0;
			}
			// Assign Token as index into IPC Buffer FifO, and clear data ready flags
			Token = IPCAlloc;
			int i;
			for (i = IPCAlloc; i < (IPCAlloc + 4 + dwSize - 1); i++) 
			{ // 4=size of int datablock size hdr
				IPC[i] = 0;
				IPCdr[i] = false;
			}
			IPCAlloc = IPCAlloc + 4 + dwSize;    //first four bytes data block size (Int) plus data
			IntPtr heapbuf = Marshal.AllocHGlobal(4);
			Marshal.WriteInt32(heapbuf, dwSize);      // Translate size Int32 to unmanaged byte stream
			Marshal.Copy(heapbuf, IPC, Token, 4);     // Write size header to IPC array
			Marshal.FreeHGlobal(heapbuf);
			for (i= 0; i< dwSize; i++)
			{
				IPC[Token + 4 + i]= param[i] ;       //xfer byte array to IPC managed FifO buffer
			}
			return FSUIPC_Write_Req(dwOffset, dwSize, Token, ref dwResult);
		}


		//--- Read requests ----------------------------------------------------------------
		///<summary>Retrieve data read from FSUIPC using token passed during read request</summary>
		///<param name="token">The unique identifier token returned from the Read call</param>
		///<param name="result">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Get(ref int Token, ref byte Result) 
		{
			int Size = 1;    // 1 byte
			if ((Token < 0) || (Token > IPC_BUFFER_SIZE - (4 + Size)) ) 
			{ //Token out of range
				Result = 0;
				return false;
			}
			IntPtr heapbuf = Marshal.AllocHGlobal(Size);
			Marshal.Copy(IPC, Token + 4, heapbuf, 1);
			Result = Marshal.ReadByte(heapbuf);
			Marshal.FreeHGlobal(heapbuf);
			if (IPCdr[Token] ) 
			{
				IPCdr[Token] = false;    // reset data ready flag
				return true;
			} 
			else 
			{  // if (data ready flag not set, function returns false and old buffer value
				return false;
			}
		}


		///<summary>Retrieve data read from FSUIPC using token passed during read request</summary>
		///<param name="token">The unique identifier token returned from the Read call</param>
		///<param name="result">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Get(ref int Token, ref short Result) 
		{
			int Size = 2;    // 2 bytes in a short
			if ((Token < 0) || (Token > IPC_BUFFER_SIZE - (4 + Size)) ) 
			{ //Token out of range
				Result = 0;
				return false;
			}
			IntPtr heapbuf = Marshal.AllocHGlobal(Size);
			Marshal.Copy(IPC, Token + 4, heapbuf, 2);
			Result = Marshal.ReadInt16(heapbuf);
			Marshal.FreeHGlobal(heapbuf);
			if (IPCdr[Token] ) 
			{
				IPCdr[Token] = false;    // reset data ready flag
				return true;
			} 
			else 
			{  // if (data ready flag not set, function returns false and old buffer value
				return false;
			}
		}

		///<summary>Retrieve data read from FSUIPC using token passed during read request</summary>
		///<param name="token">The unique identifier token returned from the Read call</param>
		///<param name="result">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Get(ref int Token, ref int Result) 
		{
			int Size = 4;    // 2 bytes in an int
			if ((Token < 0) || (Token > IPC_BUFFER_SIZE - (4 + Size)) ) 
			{ //Token out of range
				Result = 0;
				return false;
			}
			IntPtr heapbuf = Marshal.AllocHGlobal(Size);
			Marshal.Copy(IPC, Token + 4, heapbuf, 4);
			Result = Marshal.ReadInt32(heapbuf);
			Marshal.FreeHGlobal(heapbuf);
			if (IPCdr[Token] ) 
			{
				IPCdr[Token] = false;
				return true;
			} 
			else 
			{    // if (data ready flag not set, function returns false and value found
				return false;
			}
		}

		///<summary>Retrieve data read from FSUIPC using token passed during read request</summary>
		///<param name="token">The unique identifier token returned from the Read call</param>
		///<param name="result">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Get(ref int Token, ref long Result) 
		{
			int Size = 8;    // 8 bytes in a Long Int
			IntPtr heapbuf = Marshal.AllocHGlobal(Size);
			if ((Token < 0) || (Token > IPC_BUFFER_SIZE - (4 + Size)) ) 
			{ //Token out of range
				Result = 0;
				return false;
			}
			Marshal.Copy(IPC, Token + 4, heapbuf, 8);
			Result = Marshal.ReadInt64(heapbuf);
			Marshal.FreeHGlobal(heapbuf);
			if (IPCdr[Token] ) 
			{
				IPCdr[Token] = false;
				return true;
			} 
			else 
			{    // if (data ready flag not set, function returns false and value found
				return false;
			}
		}

		///<summary>Retrieve data read from FSUIPC using token passed during read request</summary>
		///<param name="token">The unique identifier token returned from the Read call</param>
		///<param name="dwSize">The size of memory locations in bytes</param>
		///<param name="result">Contains the "error-code" if method's boolean comes back false</param>
		///<return>true if successful, false otherwise.  If false, dwResult contains the "error-code"</return>
		public bool FSUIPC_Get(ref int Token, int dwSize, ref byte[] Result) 
		{

			if ((Token < 0) || (Token > IPC_BUFFER_SIZE - (4 + dwSize)) ) 
			{ //Token out of range
				return false;
			}
			IntPtr heapbuf = Marshal.AllocHGlobal(4);
			Marshal.Copy(IPC, Token, heapbuf, 4);
			int Size = Marshal.ReadInt32(heapbuf);
                        Marshal.FreeHGlobal(heapbuf);
			if (dwSize > Size ) 
			{
				dwSize = Size;    //Max size of return block is size of block written
			}
			int idx = Token + 4;    //go past size block
			while (idx < Token + 4 + dwSize) 
			{
				Result[idx - Token - 4] = IPC[idx];
				idx = idx + 1;
			}

			if (IPCdr[Token] ) 
			{
				IPCdr[Token] = false;
				return true;
			} 
			else 
			{    // if (data ready flag not set, function returns false and value found
				return false;
			}
		}

		// --- Stop the Client ---------------------------------------------------------
		///<summary>Closes the FSUIPC client connection</summary>
		public void FSUIPC_Close() 
		{

			m_hWnd = 0;
			m_msg = 0;
			m_pNext = 0;

			if (m_atom != 0) 
			{
				GlobalDeleteAtom(m_atom);
				m_atom = 0;
			}

			if (m_pView != 0) 
			{
				IntPtr _pView = new IntPtr(m_pView);
				UnmapViewOfFile(_pView);
				m_pView = 0;
			}
	
			if (m_hMap != 0) 
			{
				CloseHandle(m_hMap);
				m_hMap = 0;
			}

		}
	}
}

