/*
  CmdMessenger - library that provides command based messaging

	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the
	"Software"), to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to
	permit persons to whom the Software is furnished to do so, subject to
	the following conditions:

	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
	LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
	OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
	WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

#ifndef CmdMessenger_h
#define CmdMessenger_h

#include <inttypes.h>
#if ARDUINO >= 100
#include <Arduino.h> 
#else
#include <WProgram.h> 
#endif

//#include "Stream.h"

extern "C"
{
  // callback functions always follow the signature: void cmd(void);
  typedef void (*messengerCallbackFunction) (void);
}

#define MAXCALLBACKS        50   // The maximum number of commands   (default: 50)
#define MESSENGERBUFFERSIZE 64   // The length of the commandbuffer  (default: 64)
#define MAXSTREAMBUFFERSIZE 512  // The length of the streambuffer   (default: 64)
#define DEFAULT_TIMEOUT     5000 // Time out on unanswered messages. (default: 5s)

// Message States
enum
{  
  kProccesingMessage,            // Message is being received, not reached command separator
  kEndOfMessage,				 // Message is fully received, reached command separator
  kProcessingArguments,			 // Message is received, arguments are being read parsed
};

#define white_space(c) ((c) == ' ' || (c) == '\t')
#define valid_digit(c) ((c) >= '0' && (c) <= '9')

class CmdMessenger
{
private:

  // **** Private variables *** 
  
  bool    startCommand;            // Indicates if sending of a command is underway
  uint8_t lastCommandId;		    // ID of last received command 
  uint8_t bufferIndex;              // Index where to write data in buffer
  uint8_t bufferLength;             // Is set to MESSENGERBUFFERSIZE
  uint8_t bufferLastIndex;          // The last index of the buffer
  char ArglastChar;                 // Bookkeeping of argument escape char 
  char CmdlastChar;                 // Bookkeeping of command escape char 
  bool pauseProcessing;             // pauses processing of new commands, during sending
  bool print_newlines;              // Indicates if \r\n should be added after send command
  char commandBuffer[MESSENGERBUFFERSIZE]; // Buffer that holds the data
  char streamBuffer[MAXSTREAMBUFFERSIZE]; // Buffer that holds the data
  uint8_t messageState;             // Current state of message processing
  bool dumped;                      // Indicates if last argument has been externally read 
  bool ArgOk;						// Indicated if last fetched argument could be read
  char *current;                    // Pointer to current buffer position
  char *last;                       // Pointer to previous buffer position
  char prevChar;                    // Previous char (needed for unescaping)
  Stream *comms;                    // Serial data stream
  
  char command_separator;           // Character indicating end of command (default: ';')
  char field_separator;				// Character indicating end of argument (default: ',')
  char escape_character;		    // Character indicating escaping of special chars
    
  messengerCallbackFunction default_callback;            // default callback function  
  messengerCallbackFunction callbackList[MAXCALLBACKS];  // list of attached callback functions 
  
  
  // **** Initialize ****
  
  void init (Stream & comms, const char fld_separator, const char cmd_separator, const char esc_character);
  void reset ();
  
  // **** Command processing ****
  
  inline uint8_t processLine (char serialChar) __attribute__((always_inline));
  inline void handleMessage() __attribute__((always_inline));
  inline bool blockedTillReply (unsigned long timeout = DEFAULT_TIMEOUT, int ackCmdId = 1) __attribute__((always_inline));
  inline bool CheckForAck (int AckCommand) __attribute__((always_inline));

  // **** Command sending ****
   
  /**
   * Print variable of type T binary in binary format
   */
  template < class T > 
    void writeBin (const T & value)
  {
	const byte *bytePointer = (const byte *) (const void *) &value;
    for (unsigned int i = 0; i < sizeof (value); i++)
      {
        printEsc (*bytePointer); 
		bytePointer++;
      }
  }
    
  // **** Command receiving ****
  
  int findNext (char *str, char delim);

  /**
   * Read a variable of any type in binary format
   */
  template < class T > 
    T readBin (char *str)
  {
    T value;
    unescape (str);
    byte *bytePointer = (byte *) (const void *) &value;
    for (unsigned int i = 0; i < sizeof (value); i++)
      {
        *bytePointer = str[i];
		bytePointer++;
      }
    return value;
  }
  
  template < class T > 
    T empty ()
  {
    T value;
    byte *bytePointer = (byte *) (const void *) &value;
    for (unsigned int i = 0; i < sizeof (value); i++)
      {
        *bytePointer = '\0';
		bytePointer++;
      }
    return value;
  }
  
  // **** Escaping tools ****
  
  char *split_r (char *str, const char delim, char **nextp);
  bool isEscaped (char *currChar, const char escapeChar, char *lastChar);
  
  void printEsc (char *str);
  void printEsc (char str); 
  
public:

  // ****** Public functions ******

  // **** Initialization ****
  
  CmdMessenger (Stream & comms, const char fld_separator = ',', 
				const char cmd_separator = ';', 
                const char esc_character = '/');
  
  void printLfCr (bool addNewLine=true);
  void attach (messengerCallbackFunction newFunction);
  void attach (byte msgId, messengerCallbackFunction newFunction);
  
  // **** Command processing ****
  
  void feedinSerialData ();
  bool next ();
  bool available ();
  bool isArgOk ();
  uint8_t CommandID ();
  
  // ****  Command sending ****
  
  /**
   * Send a command with a single argument of any type 
   * Note that the argument is sent as string
   */
  template < class T >
    bool sendCmd (int cmdId, T arg, bool reqAc = false, int ackCmdId = 1, 
				  int timeout = DEFAULT_TIMEOUT)
  {
    if (!startCommand) {
		sendCmdStart (cmdId);
		sendCmdArg (arg);
		return sendCmdEnd (reqAc, ackCmdId, timeout);
	}
	return false;
  }

  /**
   * Send a command with a single argument of any type 
   * Note that the argument is sent in binary format
   */
  template < class T >
    bool sendBinCmd (int cmdId, T arg, bool reqAc = false, int ackCmdId = 1,
                     int timeout = DEFAULT_TIMEOUT)
  {
    if (!startCommand) {
		sendCmdStart (cmdId);
		sendCmdBinArg (arg);
		return sendCmdEnd (reqAc, ackCmdId, timeout);
	}
	return false;
  }

  bool sendCmd (int cmdId);
  bool sendCmd (int cmdId, bool reqAc, int ackCmdId );
  // **** Command sending with multiple arguments ****
  
  void sendCmdStart (int cmdId);
  void sendCmdEscArg (char *arg);
  void sendCmdfArg (char *fmt, ...);
  bool sendCmdEnd (bool reqAc = false, int ackCmdId = 1, int timeout = DEFAULT_TIMEOUT);
  
  /**
   * Send a single argument as string 
   *  Note that this will only succeed if a sendCmdStart has been issued first
   */
  template < class T > void sendCmdArg (T arg)
  {
    if (startCommand) {
        comms->print (field_separator);
        comms->print (arg);
    }
  }
    
  /**
   * Send a single argument as string with custom accuracy
   *  Note that this will only succeed if a sendCmdStart has been issued first
   */
  template < class T > void sendCmdArg (T arg, int n)
  {
    if (startCommand) {
        comms->print (field_separator);
        comms->print (arg, n);
    }
  }
  
  /**
   * Send double argument in scientific format.
   *  This will overcome the boundary of normal d sending which is limited to abs(f) <= MAXLONG
  */
  void sendCmdSciArg(double arg, int n=6);

  
  /**
   * Send a single argument in binary format
   *  Note that this will only succeed if a sendCmdStart has been issued first
   */  
  template < class T > void sendCmdBinArg (T arg)
  {
    if (startCommand) {
        comms->print (field_separator);
        writeBin (arg);
    }
  }  

  // **** Command receiving ****
  bool readBoolArg();
  int16_t readInt16Arg ();
  int32_t readInt32Arg ();
  char readCharArg ();
  float readFloatArg ();
  double readDoubleArg();
  char *readStringArg ();
  void copyStringArg (char *string, uint8_t size);
  uint8_t compareStringArg (char *string);
 
  /**
   * Read an argument of any type in binary format
   */  
  template < class T > T readBinArg ()
  {
    if (next ()) {
        dumped = true;      
		return readBin < T > (current);
    } else {
		return empty < T > ();
	}
  }

  // **** Escaping tools ****
  
  void unescape (char *fromChar);	
  void printSci(double f, unsigned int digits);  
  
  
};
#endif
