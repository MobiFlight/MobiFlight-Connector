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

    Initial Messenger Library - Thomas Ouellet Fredericks.
    CmdMessenger Version 1    - Neil Dudman.
    CmdMessenger Version 2    - Dreamcat4.
	CmdMessenger Version 3    - Thijs Elenbaas.
	  3.3  - Fixed warnings
	       - Some code optimization
	  3.2  - Small fixes and sending long argument support
	  3.1  - Added examples
	  3.0  - Bugfixes on 2.2
	       - Wait for acknowlegde
		   - Sending of common type arguments (float, int, char)
	       - Multi-argument commands
           - Escaping of special characters
           - Sending of binary data of any type (uses escaping)
 */

extern "C" {
#include <stdlib.h>
#include <stdarg.h>
}
#include <stdio.h>
#include <CmdMessenger.h>

#define _CMDMESSENGER_VERSION 3_3 // software version of this library

// **** Initialization **** 

/**
 * CmdMessenger constructor
 */
CmdMessenger::CmdMessenger(Stream &ccomms, const char fld_separator, const char cmd_separator, const char esc_character)
{
    init(ccomms,fld_separator,cmd_separator, esc_character);
}

/**
 * Enables printing newline after a sent command
 */
void CmdMessenger::init(Stream &ccomms, const char fld_separator, const char cmd_separator, const char esc_character)
{
    default_callback = NULL;
    comms            = &ccomms;
    print_newlines    = false;
    field_separator   = fld_separator;
    command_separator = cmd_separator;
    escape_character  = esc_character;
    bufferLength      = MESSENGERBUFFERSIZE;
    bufferLastIndex   = MESSENGERBUFFERSIZE -1;
    reset();

    default_callback  = NULL;
    for (int i = 0; i < MAXCALLBACKS; i++)
        callbackList[i] = NULL;

    pauseProcessing   = false;
}

/**
 * Resets the command buffer and message state
 */
void CmdMessenger::reset()
{
    bufferIndex = 0;
    current     = NULL;
    last        = NULL;
    dumped      = true;
}

/**
 * Enables printing newline after a sent command
 */
void CmdMessenger::printLfCr(bool addNewLine)
{
    print_newlines = addNewLine;
}

/**
 * Attaches an default function for commands that are not explicitly attached
 */
void CmdMessenger::attach(messengerCallbackFunction newFunction)
{
    default_callback = newFunction;
}

/**
 * Attaches a function to a command ID
 */
void CmdMessenger::attach(byte msgId, messengerCallbackFunction newFunction)
{
    if (msgId >= 0 && msgId < MAXCALLBACKS)
        callbackList[msgId] = newFunction;
}

// **** Command processing ****

/**
 * Feeds serial data in CmdMessenger
 */
void CmdMessenger::feedinSerialData()
{
    while ( !pauseProcessing && comms->available() )
	{	   
		// The Stream class has a readBytes() function that reads many bytes at once. On Teensy 2.0 and 3.0, readBytes() is optimized. 
		// Benchmarks about the incredible difference it makes: http://www.pjrc.com/teensy/benchmark_usb_serial_receive.html

		size_t bytesAvailable = min(comms->available(),MAXSTREAMBUFFERSIZE);
		comms->readBytes(streamBuffer, bytesAvailable); 
		
		// Process the bytes in the stream buffer, and handles dispatches callbacks, if commands are received
		for (size_t byteNo = 0; byteNo < bytesAvailable ; byteNo++) 
		{   
		    int messageState = processLine(streamBuffer[byteNo]);

			// If waiting for acknowledge command
			if ( messageState == kEndOfMessage ) 
			{
				handleMessage();
			}
		}
	}
}

/**
 * Processes bytes and determines message state
 */
uint8_t CmdMessenger::processLine(char serialChar)
{
    messageState = kProccesingMessage;
    //char serialChar = (char)serialByte;
    bool escaped = isEscaped(&serialChar,escape_character,&CmdlastChar);
    //if (serialByte > 0 || escaped) {
        if((serialChar == command_separator) && !escaped) {
            commandBuffer[bufferIndex]=0;
            if(bufferIndex > 0) {
                messageState = kEndOfMessage;
                current = commandBuffer;
                CmdlastChar='\0';
            }
            reset();
        } else {
            commandBuffer[bufferIndex]=serialChar;
            bufferIndex++;
            if (bufferIndex >= bufferLastIndex) reset();
        }
    //}
    return messageState;
}

/**
 * Dispatches attached callbacks based on command
 */
void CmdMessenger::handleMessage()
{
    lastCommandId = readIntArg();
    // if command attached, we will call it
    if (lastCommandId >= 0 && lastCommandId < MAXCALLBACKS && ArgOk && callbackList[lastCommandId] != NULL)
        (*callbackList[lastCommandId])();
    else // If command not attached, call default callback (if attached)
        if (default_callback!=NULL) (*default_callback)();
}


/**
 * Waits for reply from sender or timeout before continuing
 */
bool CmdMessenger::blockedTillReply(unsigned long timeout, int ackCmdId)
{
    unsigned long time  = millis();
    unsigned long start = time;
    bool receivedAck    = false;
    while( (time - start ) < timeout && !receivedAck) {
        time = millis();
        receivedAck = CheckForAck(ackCmdId);
    }
    return receivedAck;
}

/**
 *   Loops as long data is available to determine if acknowledge has come in 
 */
bool CmdMessenger::CheckForAck(int AckCommand)
{
    while (  comms->available() ) {
		//Processes a byte and determines if an acknowlegde has come in
		int messageState = processLine(comms->read());
		if ( messageState == kEndOfMessage ) {
			int id = readIntArg();
			if (AckCommand==id && ArgOk) {
				return true;
			} else {
				return false;
			}
		}
		return false;
    }
    return false;
}

/**
 * Gets next argument. Returns true if an argument is available
 */
bool CmdMessenger::next()
{
    char * temppointer= NULL;
    // Currently, cmd messenger only supports 1 char for the field seperator
    switch (messageState) {
    case kProccesingMessage:
        return false;
    case kEndOfMessage:
        temppointer = commandBuffer;
        messageState = kProcessingArguments;
    default:
        if (dumped)
            current = split_r(temppointer,field_separator,&last);
        if (current != NULL) {
            dumped = true;
            return true;
        }
    }
    return false;
}

/**
 * Returns if an argument is available. Alias for next()
 */
bool CmdMessenger::available()
{
    return next();
}

/**
 * Returns if the latest argument is well formed. 
 */
bool CmdMessenger::isArgOk ()
{
	return ArgOk;
}

/**
 * Returns the CommandID of the current command
 */
uint8_t CmdMessenger::CommandID()
{
    return lastCommandId;
}

// ****  Command sending ****

/**
 * Send start of command. This makes it easy to send multiple arguments per command
 */
void CmdMessenger::sendCmdStart(int cmdId)
{
    if (!startCommand) {
		startCommand   = true;
		pauseProcessing = true;
		comms->print(cmdId);
	}
}

/**
 * Send an escaped command argument
 */
void CmdMessenger::sendCmdEscArg(char* arg)
{
    if (startCommand) {
        comms->print(field_separator);
        printEsc(arg);
    }
}

/**
 * Send formatted argument.
 *  Note that floating points are not supported and resulting string is limited to 128 chars
 */
void CmdMessenger::sendCmdfArg(char *fmt, ...)
{
    if (startCommand) {
        char msg[128];
        va_list args;
        va_start (args, fmt );
        vsnprintf(msg, 188, fmt, args);
        va_end (args);

        comms->print(field_separator);
        comms->print(msg);
    }
}

/**
 * Send end of command
 */
bool CmdMessenger::sendCmdEnd(bool reqAc, int ackCmdId, int timeout)
{
    bool ackReply = false;
    if (startCommand) {
        comms->print(command_separator);
        if(print_newlines)
            comms->println(); // should append BOTH \r\n
        if (reqAc) {
            ackReply = blockedTillReply(timeout, ackCmdId);
        }
    }
    pauseProcessing = false;
    startCommand   = false;
    return ackReply;
}

// **** Command receiving ****

/**
 * Find next argument in command
 */
int CmdMessenger::findNext(char *str, char delim)
{
    int pos = 0;
    bool escaped;
    ArglastChar = NULL;
    while (*str != '\0') {
        escaped = isEscaped(str,escape_character,&ArglastChar);
        if (*str==field_separator && !escaped) {
            return pos;
        } else {
            str++;
            pos++;
        }
    }
    return pos;
}

/**
 * Read the next argument as int
 */
int CmdMessenger::readIntArg()
{
    if (next()) {
        dumped = true;
		ArgOk  = true;
        return atoi(current);
    }
	ArgOk  = false;
    return 0;
}

/**
 * Read the next argument as int
 */
long CmdMessenger::readLongArg()
{
    if (next()) {
        dumped = true;
		ArgOk  = true;
        return atol(current);
    }
	ArgOk  = false;
    return 0L;
}

/**
 * Read the next argument as bool
 */
bool CmdMessenger::readBoolArg()
{
	return (readIntArg()!=0)?true:false;
    
}

/**
 * Read the next argument as char
 */
char CmdMessenger::readCharArg()
{
    if (next()) {
        dumped = true;
		ArgOk  = true;
        return current[0];
    }
	ArgOk  = false;
    return 0;
}

/**
 * Read the next argument as float
 */
float CmdMessenger::readFloatArg()
{
    if (next()) {
        dumped = true;
		ArgOk  = true;
        return atof(current);
    }
	ArgOk  = false;
    return 0;
}

/**
 * Read next argument as string.
 * Note that the String is valid until the current command is replaced
 */
char* CmdMessenger::readStringArg()
{
    if (next()) {
        dumped = true;
		ArgOk  = true;
        return current;
    }
	ArgOk  = false;
    return '\0';
}

/**
 * Return next argument as a new string
 * Note that this is useful if the string needs to be persisted
 */
void CmdMessenger::copyStringArg(char *string, uint8_t size)
{
    if (next()) {
        dumped = true;
		ArgOk  = true;
        strlcpy(string,current,size);
    } else {
		ArgOk  = false;
        if ( size ) string[0] = '\0';
    }
}

/**
 * Compare the next argument with a string
 */
uint8_t CmdMessenger::compareStringArg(char *string)
{
    if (next()) {
        if ( strcmp(string,current) == 0 ) {
            dumped = true;
			ArgOk  = true;
            return 1;
        } else {
			ArgOk  = false;
            return 0;
        }
    }
	return 0;
}

// **** Escaping tools ****

/**
 * Unescapes a string
 * Note that this is done inline
 */
void CmdMessenger::unescape(char *fromChar)
{
    // Move unescaped characters right
    char *toChar = fromChar;
    while (*fromChar != '\0') {
        if (*fromChar==escape_character) {
            fromChar++;
        }
        *toChar++=*fromChar++;
    }
    // Pad string with \0 if string was shortened
    for (; toChar<fromChar; toChar++) {
        *toChar='\0';
    }
}

/**
 * Split string in different tokens, based on delimiter
 * Note that this is basically strtok_r, but with support for an escape character
 */
char* CmdMessenger::split_r(char *str, const char delim, char **nextp)
{
    char *ret;
    // if input null, this is not the first call, use the nextp pointer instead
    if (str == NULL) {
        str = *nextp;
    }
    // Strip leading delimiters
    while (findNext(str, delim)==0 && *str) {
        str++;
    }
    // If this is a \0 char, return null
    if (*str == '\0') {
        return NULL;
    }
    // Set start of return pointer to this position
    ret = str;
    // Find next delimiter
    str += findNext(str, delim);
    // and exchange this for a a \0 char. This will terminate the char
    if (*str) {
        *str++ = '\0';
    }
    // Set the next pointer to this char
    *nextp = str;
    // return current pointer
    return ret;
}

/**
 * Indicates if the current character is escaped
 */
bool CmdMessenger::isEscaped(char *currChar, const char escapeChar, char *lastChar)
{
    bool escaped;
    escaped   = (*lastChar==escapeChar);
    *lastChar = *currChar;

    // special case: the escape char has been escaped:
    if (*lastChar == escape_character && escaped) {
        *lastChar = '\0';
    }
    return escaped;
}

/**
 * Escape and print a string
 */
void CmdMessenger::printEsc(char *str)
{
    while (*str != '\0') {
        printEsc(*str++);
    }
}

/**
 * Escape and print a character
 */
void CmdMessenger::printEsc(char str)
{
    if (str==field_separator || str==command_separator || str==escape_character || str=='\0') {
        comms->print(escape_character);
    }
    comms->print(str);
}