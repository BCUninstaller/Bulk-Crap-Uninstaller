/*
   Taken from https://www.pinvoke.net/default.aspx/Constants/WINERROR.html

   It is documented that you should NOT use the pinvoke GetLastError() due to the fact that the runtime
   makes it's own API calls without any notification to the application at all. And since most user-API calls
   are made through a call setup with DllImportAttribute, the .NET framework has made available a method
   that will record your call's ErrorCode until such time as you need to get it (although, most times, subsequent calls
   to other API's that you make will overwrite the previous - read ahead to find out why). Use Marshal.GetLastWin32Error() to get the
   last ErrorCode recorded by your most recent API call. You will also need to make sure you change SetLastError=true when you define
   your Pinvoke signature, unless you purposely don't want it or don't care or need the lasterror preserved. If you have
   questions on this remit to: http://www.paradisim.net in the FAQ forums or gabriel@paradisim.net.
   With this said, you can still use the following code to identify the errorcode:
*/

/******************************************************************************/
/*       _                                    */
/*       \`*-.                                */
/*    )  _`-.                                 */
/*       .  : `. .                                */
/*       : _   '  \                               */
/*       ; *` _.   `*-._                              */
/*       `-.-'      `-.                           */
/*     ;       `       `.                         */
/*     :.       .    \                        */
/*     . \  .   :   .-'   .     NABU Project                  */
/*     '  `+.;  ;  '      :     Microsoft.Win32.Interop Library       */
/*     :  '  |    ;       ;-.   Copyright © 2005, adontz          */
/*     ; '   : :`-:     _.`* ;                        */
/*      .*' /  .*' ; .*`- +'  `*'                         */
/*      `*-*   `*-*  `*-*'                            */
/*                                        */
/******************************************************************************/
using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Microsoft.Win32.Interop
{
    internal enum ResultWin32
    {
        /// <summary>
        /// Failed to parse the code
        /// </summary>
        INVALID_ERROR_CODE = -1,

        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        ERROR_SUCCESS = 0,
        /// <summary>
        /// Incorrect function.
        /// </summary>
        ERROR_INVALID_FUNCTION = 1,
        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        ERROR_FILE_NOT_FOUND = 2,
        /// <summary>
        /// The system cannot find the path specified.
        /// </summary>
        ERROR_PATH_NOT_FOUND = 3,
        /// <summary>
        /// The system cannot open the file.
        /// </summary>
        ERROR_TOO_MANY_OPEN_FILES = 4,
        /// <summary>
        /// Access is denied.
        /// </summary>
        ERROR_ACCESS_DENIED = 5,
        /// <summary>
        /// The handle is invalid.
        /// </summary>
        ERROR_INVALID_HANDLE = 6,
        /// <summary>
        /// The storage control blocks were destroyed.
        /// </summary>
        ERROR_ARENA_TRASHED = 7,
        /// <summary>
        /// Not enough storage is available to process this command.
        /// </summary>
        ERROR_NOT_ENOUGH_MEMORY = 8,
        /// <summary>
        /// The storage control block address is invalid.
        /// </summary>
        ERROR_INVALID_BLOCK = 9,
        /// <summary>
        /// The environment is incorrect.
        /// </summary>
        ERROR_BAD_ENVIRONMENT = 10,
        /// <summary>
        /// An attempt was made to load a program with an incorrect format.
        /// </summary>
        ERROR_BAD_FORMAT = 11,
        /// <summary>
        /// The access code is invalid.
        /// </summary>
        ERROR_INVALID_ACCESS = 12,
        /// <summary>
        /// The data is invalid.
        /// </summary>
        ERROR_INVALID_DATA = 13,
        /// <summary>
        /// Not enough storage is available to complete this operation.
        /// </summary>
        ERROR_OUTOFMEMORY = 14,
        /// <summary>
        /// The system cannot find the drive specified.
        /// </summary>
        ERROR_INVALID_DRIVE = 15,
        /// <summary>
        /// The directory cannot be removed.
        /// </summary>
        ERROR_CURRENT_DIRECTORY = 16,
        /// <summary>
        /// The system cannot move the file to a different disk drive.
        /// </summary>
        ERROR_NOT_SAME_DEVICE = 17,
        /// <summary>
        /// There are no more files.
        /// </summary>
        ERROR_NO_MORE_FILES = 18,
        /// <summary>
        /// The media is write protected.
        /// </summary>
        ERROR_WRITE_PROTECT = 19,
        /// <summary>
        /// The system cannot find the device specified.
        /// </summary>
        ERROR_BAD_UNIT = 20,
        /// <summary>
        /// The device is not ready.
        /// </summary>
        ERROR_NOT_READY = 21,
        /// <summary>
        /// The device does not recognize the command.
        /// </summary>
        ERROR_BAD_COMMAND = 22,
        /// <summary>
        /// Data error (cyclic redundancy check).
        /// </summary>
        ERROR_CRC = 23,
        /// <summary>
        /// The program issued a command but the command length is incorrect.
        /// </summary>
        ERROR_BAD_LENGTH = 24,
        /// <summary>
        /// The drive cannot locate a specific area or track on the disk.
        /// </summary>
        ERROR_SEEK = 25,
        /// <summary>
        /// The specified disk or diskette cannot be accessed.
        /// </summary>
        ERROR_NOT_DOS_DISK = 26,
        /// <summary>
        /// The drive cannot find the sector requested.
        /// </summary>
        ERROR_SECTOR_NOT_FOUND = 27,
        /// <summary>
        /// The printer is out of paper.
        /// </summary>
        ERROR_OUT_OF_PAPER = 28,
        /// <summary>
        /// The system cannot write to the specified device.
        /// </summary>
        ERROR_WRITE_FAULT = 29,
        /// <summary>
        /// The system cannot read from the specified device.
        /// </summary>
        ERROR_READ_FAULT = 30,
        /// <summary>
        /// A device attached to the system is not functioning.
        /// </summary>
        ERROR_GEN_FAILURE = 31,
        /// <summary>
        /// The process cannot access the file because it is being used by another process.
        /// </summary>
        ERROR_SHARING_VIOLATION = 32,
        /// <summary>
        /// The process cannot access the file because another process has locked a portion of the file.
        /// </summary>
        ERROR_LOCK_VIOLATION = 33,
        /// <summary>
        /// The wrong diskette is in the drive.
        /// Insert %2 (Volume Serial Number: %3) into drive %1.
        /// </summary>
        ERROR_WRONG_DISK = 34,
        /// <summary>
        /// Too many files opened for sharing.
        /// </summary>
        ERROR_SHARING_BUFFER_EXCEEDED = 36,
        /// <summary>
        /// Reached the end of the file.
        /// </summary>
        ERROR_HANDLE_EOF = 38,
        /// <summary>
        /// The disk is full.
        /// </summary>
        ERROR_HANDLE_DISK_FULL = 39,
        /// <summary>
        /// The request is not supported.
        /// </summary>
        ERROR_NOT_SUPPORTED = 50,
        /// <summary>
        /// Windows cannot find the network path. Verify that the network path is correct and the destination computer is not busy or turned off. If Windows still cannot find the network path, contact your network administrator.
        /// </summary>
        ERROR_REM_NOT_LIST = 51,
        /// <summary>
        /// You were not connected because a duplicate name exists on the network. Go to System in Control Panel to change the computer name and try again.
        /// </summary>
        ERROR_DUP_NAME = 52,
        /// <summary>
        /// The network path was not found.
        /// </summary>
        ERROR_BAD_NETPATH = 53,
        /// <summary>
        /// The network is busy.
        /// </summary>
        ERROR_NETWORK_BUSY = 54,
        /// <summary>
        /// The specified network resource or device is no longer available.
        /// </summary>
        ERROR_DEV_NOT_EXIST = 55,
        /// <summary>
        /// The network BIOS command limit has been reached.
        /// </summary>
        ERROR_TOO_MANY_CMDS = 56,
        /// <summary>
        /// A network adapter hardware error occurred.
        /// </summary>
        ERROR_ADAP_HDW_ERR = 57,
        /// <summary>
        /// The specified server cannot perform the requested operation.
        /// </summary>
        ERROR_BAD_NET_RESP = 58,
        /// <summary>
        /// An unexpected network error occurred.
        /// </summary>
        ERROR_UNEXP_NET_ERR = 59,
        /// <summary>
        /// The remote adapter is not compatible.
        /// </summary>
        ERROR_BAD_REM_ADAP = 60,
        /// <summary>
        /// The printer queue is full.
        /// </summary>
        ERROR_PRINTQ_FULL = 61,
        /// <summary>
        /// Space to store the file waiting to be printed is not available on the server.
        /// </summary>
        ERROR_NO_SPOOL_SPACE = 62,
        /// <summary>
        /// Your file waiting to be printed was deleted.
        /// </summary>
        ERROR_PRINT_CANCELLED = 63,
        /// <summary>
        /// The specified network name is no longer available.
        /// </summary>
        ERROR_NETNAME_DELETED = 64,
        /// <summary>
        /// Network access is denied.
        /// </summary>
        ERROR_NETWORK_ACCESS_DENIED = 65,
        /// <summary>
        /// The network resource type is not correct.
        /// </summary>
        ERROR_BAD_DEV_TYPE = 66,
        /// <summary>
        /// The network name cannot be found.
        /// </summary>
        ERROR_BAD_NET_NAME = 67,
        /// <summary>
        /// The name limit for the local computer network adapter card was exceeded.
        /// </summary>
        ERROR_TOO_MANY_NAMES = 68,
        /// <summary>
        /// The network BIOS session limit was exceeded.
        /// </summary>
        ERROR_TOO_MANY_SESS = 69,
        /// <summary>
        /// The remote server has been paused or is in the process of being started.
        /// </summary>
        ERROR_SHARING_PAUSED = 70,
        /// <summary>
        /// No more connections can be made to this remote computer at this time because there are already as many connections as the computer can accept.
        /// </summary>
        ERROR_REQ_NOT_ACCEP = 71,
        /// <summary>
        /// The specified printer or disk device has been paused.
        /// </summary>
        ERROR_REDIR_PAUSED = 72,
        /// <summary>
        /// The file exists.
        /// </summary>
        ERROR_FILE_EXISTS = 80,
        /// <summary>
        /// The directory or file cannot be created.
        /// </summary>
        ERROR_CANNOT_MAKE = 82,
        /// <summary>
        /// Fail on INT 24.
        /// </summary>
        ERROR_FAIL_I24 = 83,
        /// <summary>
        /// Storage to process this request is not available.
        /// </summary>
        ERROR_OUT_OF_STRUCTURES = 84,
        /// <summary>
        /// The local device name is already in use.
        /// </summary>
        ERROR_ALREADY_ASSIGNED = 85,
        /// <summary>
        /// The specified network password is not correct.
        /// </summary>
        ERROR_INVALID_PASSWORD = 86,
        /// <summary>
        /// The parameter is incorrect.
        /// </summary>
        ERROR_INVALID_PARAMETER = 87,
        /// <summary>
        /// A write fault occurred on the network.
        /// </summary>
        ERROR_NET_WRITE_FAULT = 88,
        /// <summary>
        /// The system cannot start another process at this time.
        /// </summary>
        ERROR_NO_PROC_SLOTS = 89,
        /// <summary>
        /// Cannot create another system semaphore.
        /// </summary>
        ERROR_TOO_MANY_SEMAPHORES = 100,
        /// <summary>
        /// The exclusive semaphore is owned by another process.
        /// </summary>
        ERROR_EXCL_SEM_ALREADY_OWNED = 101,
        /// <summary>
        /// The semaphore is set and cannot be closed.
        /// </summary>
        ERROR_SEM_IS_SET = 102,
        /// <summary>
        /// The semaphore cannot be set again.
        /// </summary>
        ERROR_TOO_MANY_SEM_REQUESTS = 103,
        /// <summary>
        /// Cannot request exclusive semaphores at interrupt time.
        /// </summary>
        ERROR_INVALID_AT_INTERRUPT_TIME = 104,
        /// <summary>
        /// The previous ownership of this semaphore has ended.
        /// </summary>
        ERROR_SEM_OWNER_DIED = 105,
        /// <summary>
        /// Insert the diskette for drive %1.
        /// </summary>
        ERROR_SEM_USER_LIMIT = 106,
        /// <summary>
        /// The program stopped because an alternate diskette was not inserted.
        /// </summary>
        ERROR_DISK_CHANGE = 107,
        /// <summary>
        /// The disk is in use or locked by another process.
        /// </summary>
        ERROR_DRIVE_LOCKED = 108,
        /// <summary>
        /// The pipe has been ended.
        /// </summary>
        ERROR_BROKEN_PIPE = 109,
        /// <summary>
        /// The system cannot open the device or file specified.
        /// </summary>
        ERROR_OPEN_FAILED = 110,
        /// <summary>
        /// The file name is too long.
        /// </summary>
        ERROR_BUFFER_OVERFLOW = 111,
        /// <summary>
        /// There is not enough space on the disk.
        /// </summary>
        ERROR_DISK_FULL = 112,
        /// <summary>
        /// No more internal file identifiers available.
        /// </summary>
        ERROR_NO_MORE_SEARCH_HANDLES = 113,
        /// <summary>
        /// The target internal file identifier is incorrect.
        /// </summary>
        ERROR_INVALID_TARGET_HANDLE = 114,
        /// <summary>
        /// The IOCTL call made by the application program is not correct.
        /// </summary>
        ERROR_INVALID_CATEGORY = 117,
        /// <summary>
        /// The verify-on-write switch parameter value is not correct.
        /// </summary>
        ERROR_INVALID_VERIFY_SWITCH = 118,
        /// <summary>
        /// The system does not support the command requested.
        /// </summary>
        ERROR_BAD_DRIVER_LEVEL = 119,
        /// <summary>
        /// This function is not supported on this system.
        /// </summary>
        ERROR_CALL_NOT_IMPLEMENTED = 120,
        /// <summary>
        /// The semaphore timeout period has expired.
        /// </summary>
        ERROR_SEM_TIMEOUT = 121,
        /// <summary>
        /// The data area passed to a system call is too small.
        /// </summary>
        ERROR_INSUFFICIENT_BUFFER = 122,
        /// <summary>
        /// The filename, directory name, or volume label syntax is incorrect.
        /// </summary>
        ERROR_INVALID_NAME = 123,
        /// <summary>
        /// The system call level is not correct.
        /// </summary>
        ERROR_INVALID_LEVEL = 124,
        /// <summary>
        /// The disk has no volume label.
        /// </summary>
        ERROR_NO_VOLUME_LABEL = 125,
        /// <summary>
        /// The specified module could not be found.
        /// </summary>
        ERROR_MOD_NOT_FOUND = 126,
        /// <summary>
        /// The specified procedure could not be found.
        /// </summary>
        ERROR_PROC_NOT_FOUND = 127,
        /// <summary>
        /// There are no child processes to wait for.
        /// </summary>
        ERROR_WAIT_NO_CHILDREN = 128,
        /// <summary>
        /// The %1 application cannot be run in Win32 mode.
        /// </summary>
        ERROR_CHILD_NOT_COMPLETE = 129,
        /// <summary>
        /// Attempt to use a file handle to an open disk partition for an operation other than raw disk I/O.
        /// </summary>
        ERROR_DIRECT_ACCESS_HANDLE = 130,
        /// <summary>
        /// An attempt was made to move the file pointer before the beginning of the file.
        /// </summary>
        ERROR_NEGATIVE_SEEK = 131,
        /// <summary>
        /// The file pointer cannot be set on the specified device or file.
        /// </summary>
        ERROR_SEEK_ON_DEVICE = 132,
        /// <summary>
        /// A JOIN or SUBST command cannot be used for a drive that contains previously joined drives.
        /// </summary>
        ERROR_IS_JOIN_TARGET = 133,
        /// <summary>
        /// An attempt was made to use a JOIN or SUBST command on a drive that has already been joined.
        /// </summary>
        ERROR_IS_JOINED = 134,
        /// <summary>
        /// An attempt was made to use a JOIN or SUBST command on a drive that has already been substituted.
        /// </summary>
        ERROR_IS_SUBSTED = 135,
        /// <summary>
        /// The system tried to delete the JOIN of a drive that is not joined.
        /// </summary>
        ERROR_NOT_JOINED = 136,
        /// <summary>
        /// The system tried to delete the substitution of a drive that is not substituted.
        /// </summary>
        ERROR_NOT_SUBSTED = 137,
        /// <summary>
        /// The system tried to join a drive to a directory on a joined drive.
        /// </summary>
        ERROR_JOIN_TO_JOIN = 138,
        /// <summary>
        /// The system tried to substitute a drive to a directory on a substituted drive.
        /// </summary>
        ERROR_SUBST_TO_SUBST = 139,
        /// <summary>
        /// The system tried to join a drive to a directory on a substituted drive.
        /// </summary>
        ERROR_JOIN_TO_SUBST = 140,
        /// <summary>
        /// The system tried to SUBST a drive to a directory on a joined drive.
        /// </summary>
        ERROR_SUBST_TO_JOIN = 141,
        /// <summary>
        /// The system cannot perform a JOIN or SUBST at this time.
        /// </summary>
        ERROR_BUSY_DRIVE = 142,
        /// <summary>
        /// The system cannot join or substitute a drive to or for a directory on the same drive.
        /// </summary>
        ERROR_SAME_DRIVE = 143,
        /// <summary>
        /// The directory is not a subdirectory of the root directory.
        /// </summary>
        ERROR_DIR_NOT_ROOT = 144,
        /// <summary>
        /// The directory is not empty.
        /// </summary>
        ERROR_DIR_NOT_EMPTY = 145,
        /// <summary>
        /// The path specified is being used in a substitute.
        /// </summary>
        ERROR_IS_SUBST_PATH = 146,
        /// <summary>
        /// Not enough resources are available to process this command.
        /// </summary>
        ERROR_IS_JOIN_PATH = 147,
        /// <summary>
        /// The path specified cannot be used at this time.
        /// </summary>
        ERROR_PATH_BUSY = 148,
        /// <summary>
        /// An attempt was made to join or substitute a drive for which a directory on the drive is the target of a previous substitute.
        /// </summary>
        ERROR_IS_SUBST_TARGET = 149,
        /// <summary>
        /// System trace information was not specified in your CONFIG.SYS file, or tracing is disallowed.
        /// </summary>
        ERROR_SYSTEM_TRACE = 150,
        /// <summary>
        /// The number of specified semaphore events for DosMuxSemWait is not correct.
        /// </summary>
        ERROR_INVALID_EVENT_COUNT = 151,
        /// <summary>
        /// DosMuxSemWait did not execute, too many semaphores are already set.
        /// </summary>
        ERROR_TOO_MANY_MUXWAITERS = 152,
        /// <summary>
        /// The DosMuxSemWait list is not correct.
        /// </summary>
        ERROR_INVALID_LIST_FORMAT = 153,
        /// <summary>
        /// The volume label you entered exceeds the label character limit of the target file system.
        /// </summary>
        ERROR_LABEL_TOO_Int32 = 154,
        /// <summary>
        /// Cannot create another thread.
        /// </summary>
        ERROR_TOO_MANY_TCBS = 155,
        /// <summary>
        /// The recipient process has refused the signal.
        /// </summary>
        ERROR_SIGNAL_REFUSED = 156,
        /// <summary>
        /// The segment is already discarded and cannot be locked.
        /// </summary>
        ERROR_DISCARDED = 157,
        /// <summary>
        /// The segment is already unlocked.
        /// </summary>
        ERROR_NOT_LOCKED = 158,
        /// <summary>
        /// The address for the thread ID is not correct.
        /// </summary>
        ERROR_BAD_THREADID_ADDR = 159,
        /// <summary>
        /// One or more arguments are not correct.
        /// </summary>
        ERROR_BAD_ARGUMENTS = 160,
        /// <summary>
        /// The specified path is invalid.
        /// </summary>
        ERROR_BAD_PATHNAME = 161,
        /// <summary>
        /// A signal is already pending.
        /// </summary>
        ERROR_SIGNAL_PENDING = 162,
        /// <summary>
        /// No more threads can be created in the system.
        /// </summary>
        ERROR_MAX_THRDS_REACHED = 164,
        /// <summary>
        /// Unable to lock a region of a file.
        /// </summary>
        ERROR_LOCK_FAILED = 167,
        /// <summary>
        /// The requested resource is in use.
        /// </summary>
        ERROR_BUSY = 170,
        /// <summary>
        /// A lock request was not outstanding for the supplied cancel region.
        /// </summary>
        ERROR_CANCEL_VIOLATION = 173,
        /// <summary>
        /// The file system does not support atomic changes to the lock type.
        /// </summary>
        ERROR_ATOMIC_LOCKS_NOT_SUPPORTED = 174,
        /// <summary>
        /// The system detected a segment number that was not correct.
        /// </summary>
        ERROR_INVALID_SEGMENT_NUMBER = 180,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INVALID_ORDINAL = 182,
        /// <summary>
        /// Cannot create a file when that file already exists.
        /// </summary>
        ERROR_ALREADY_EXISTS = 183,
        /// <summary>
        /// The flag passed is not correct.
        /// </summary>
        ERROR_INVALID_FLAG_NUMBER = 186,
        /// <summary>
        /// The specified system semaphore name was not found.
        /// </summary>
        ERROR_SEM_NOT_FOUND = 187,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INVALID_STARTING_CODESEG = 188,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INVALID_STACKSEG = 189,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INVALID_MODULETYPE = 190,
        /// <summary>
        /// Cannot run %1 in Win32 mode.
        /// </summary>
        ERROR_INVALID_EXE_SIGNATURE = 191,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_EXE_MARKED_INVALID = 192,
        /// <summary>
        /// %1 is not a valid Win32 application.

        /// </summary>
        ERROR_BAD_EXE_FORMAT = 193,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_ITERATED_DATA_EXCEEDS_64k = 194,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INVALID_MINALLOCSIZE = 195,
        /// <summary>
        /// The operating system cannot run this application program.
        /// </summary>
        ERROR_DYNLINK_FROM_INVALID_RING = 196,
        /// <summary>
        /// The operating system is not presently configured to run this application.
        /// </summary>
        ERROR_IOPL_NOT_ENABLED = 197,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INVALID_SEGDPL = 198,
        /// <summary>
        /// The operating system cannot run this application program.
        /// </summary>
        ERROR_AUTODATASEG_EXCEEDS_64k = 199,
        /// <summary>
        /// The code segment cannot be greater than or equal to 64K.
        /// </summary>
        ERROR_RING2SEG_MUST_BE_MOVABLE = 200,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_RELOC_CHAIN_XEEDS_SEGLIM = 201,
        /// <summary>
        /// The operating system cannot run %1.
        /// </summary>
        ERROR_INFLOOP_IN_RELOC_CHAIN = 202,
        /// <summary>
        /// The system could not find the environment option that was entered.
        /// </summary>
        ERROR_ENVVAR_NOT_FOUND = 203,
        /// <summary>
        /// No process in the command subtree has a signal handler.
        /// </summary>
        ERROR_NO_SIGNAL_SENT = 205,
        /// <summary>
        /// The filename or extension is too long.
        /// </summary>
        ERROR_FILENAME_EXCED_RANGE = 206,
        /// <summary>
        /// The ring 2 stack is in use.
        /// </summary>
        ERROR_RING2_STACK_IN_USE = 207,
        /// <summary>
        /// The global filename characters, * or ?, are entered incorrectly or too many global filename characters are specified.
        /// </summary>
        ERROR_META_EXPANSION_TOO_Int32 = 208,
        /// <summary>
        /// The signal being posted is not correct.
        /// </summary>
        ERROR_INVALID_SIGNAL_NUMBER = 209,
        /// <summary>
        /// The signal handler cannot be set.
        /// </summary>
        ERROR_THREAD_1_INACTIVE = 210,
        /// <summary>
        /// The segment is locked and cannot be reallocated.
        /// </summary>
        ERROR_LOCKED = 212,
        /// <summary>
        /// Too many dynamic-link modules are attached to this program or dynamic-link module.
        /// </summary>
        ERROR_TOO_MANY_MODULES = 214,
        /// <summary>
        /// Cannot nest calls to LoadModule.
        /// </summary>
        ERROR_NESTING_NOT_ALLOWED = 215,
        /// <summary>
        /// The image file %1 is valid, but is for a machine type other than the current machine.
        /// </summary>
        ERROR_EXE_MACHINE_TYPE_MISMATCH = 216,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_EXE_CANNOT_MODIFY_SIGNED_BINARY = 217,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_EXE_CANNOT_MODIFY_STRONG_SIGNED_BINARY = 218,
        /// <summary>
        /// The pipe state is invalid.
        /// </summary>
        ERROR_BAD_PIPE = 230,
        /// <summary>
        /// All pipe instances are busy.
        /// </summary>
        ERROR_PIPE_BUSY = 231,
        /// <summary>
        /// The pipe is being closed.
        /// </summary>
        ERROR_NO_DATA = 232,
        /// <summary>
        /// No process is on the other end of the pipe.
        /// </summary>
        ERROR_PIPE_NOT_CONNECTED = 233,
        /// <summary>
        /// More data is available.
        /// </summary>
        ERROR_MORE_DATA = 234,
        /// <summary>
        /// The session was canceled.
        /// </summary>
        ERROR_VC_DISCONNECTED = 240,
        /// <summary>
        /// The specified extended attribute name was invalid.
        /// </summary>
        ERROR_INVALID_EA_NAME = 254,
        /// <summary>
        /// The extended attributes are inconsistent.
        /// </summary>
        ERROR_EA_LIST_INCONSISTENT = 255,
        /// <summary>
        /// The wait operation timed out.
        /// </summary>
        WAIT_TIMEOUT = 258,
        /// <summary>
        /// No more data is available.
        /// </summary>
        ERROR_NO_MORE_ITEMS = 259,
        /// <summary>
        /// The copy functions cannot be used.
        /// </summary>
        ERROR_CANNOT_COPY = 266,
        /// <summary>
        /// The directory name is invalid.
        /// </summary>
        ERROR_DIRECTORY = 267,
        /// <summary>
        /// The extended attributes did not fit in the buffer.
        /// </summary>
        ERROR_EAS_DIDNT_FIT = 275,
        /// <summary>
        /// The extended attribute file on the mounted file system is corrupt.
        /// </summary>
        ERROR_EA_FILE_CORRUPT = 276,
        /// <summary>
        /// The extended attribute table file is full.
        /// </summary>
        ERROR_EA_TABLE_FULL = 277,
        /// <summary>
        /// The specified extended attribute handle is invalid.
        /// </summary>
        ERROR_INVALID_EA_HANDLE = 278,
        /// <summary>
        /// The mounted file system does not support extended attributes.
        /// </summary>
        ERROR_EAS_NOT_SUPPORTED = 282,
        /// <summary>
        /// Attempt to release mutex not owned by caller.
        /// </summary>
        ERROR_NOT_OWNER = 288,
        /// <summary>
        /// Too many posts were made to a semaphore.
        /// </summary>
        ERROR_TOO_MANY_POSTS = 298,
        /// <summary>
        /// Only part of a ReadProcessMemory or WriteProcessMemory request was completed.
        /// </summary>
        ERROR_PARTIAL_COPY = 299,
        /// <summary>
        /// The oplock request is denied.
        /// </summary>
        ERROR_OPLOCK_NOT_GRANTED = 300,
        /// <summary>
        /// An invalid oplock acknowledgment was received by the system.
        /// </summary>
        ERROR_INVALID_OPLOCK_PROTOCOL = 301,
        /// <summary>
        /// The volume is too fragmented to complete this operation.
        /// </summary>
        ERROR_DISK_TOO_FRAGMENTED = 302,
        /// <summary>
        /// The file cannot be opened because it is in the process of being deleted.
        /// </summary>
        ERROR_DELETE_PENDING = 303,
        /// <summary>
        /// The system cannot find message text for message number 0x%1 in the message file for %2.
        /// </summary>
        ERROR_MR_MID_NOT_FOUND = 317,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_SCOPE_NOT_FOUND = 318,
        /// <summary>
        /// Attempt to access invalid address.
        /// </summary>
        ERROR_INVALID_ADDRESS = 487,
        /// <summary>
        /// Arithmetic result exceeded 32 bits.
        /// </summary>
        ERROR_ARITHMETIC_OVERFLOW = 534,
        /// <summary>
        /// There is a process on other end of the pipe.
        /// </summary>
        ERROR_PIPE_CONNECTED = 535,
        /// <summary>
        /// Waiting for a process to open the other end of the pipe.
        /// </summary>
        ERROR_PIPE_LISTENING = 536,
        /// <summary>
        /// Access to the extended attribute was denied.
        /// </summary>
        ERROR_EA_ACCESS_DENIED = 994,
        /// <summary>
        /// The I/O operation has been aborted because of either a thread exit or an application request.
        /// </summary>
        ERROR_OPERATION_ABORTED = 995,
        /// <summary>
        /// Overlapped I/O event is not in a signaled state.
        /// </summary>
        ERROR_IO_INCOMPLETE = 996,
        /// <summary>
        /// Overlapped I/O operation is in progress.
        /// </summary>
        ERROR_IO_PENDING = 997,
        /// <summary>
        /// Invalid access to memory location.
        /// </summary>
        ERROR_NOACCESS = 998,
        /// <summary>
        /// Error performing inpage operation.
        /// </summary>
        ERROR_SWAPERROR = 999,
        /// <summary>
        /// Recursion too deep, the stack overflowed.
        /// </summary>
        ERROR_STACK_OVERFLOW = 1001,
        /// <summary>
        /// The window cannot act on the sent message.
        /// </summary>
        ERROR_INVALID_MESSAGE = 1002,
        /// <summary>
        /// Cannot complete this function.
        /// </summary>
        ERROR_CAN_NOT_COMPLETE = 1003,
        /// <summary>
        /// Invalid flags.
        /// </summary>
        ERROR_INVALID_FLAGS = 1004,
        /// <summary>
        /// The volume does not contain a recognized file system.
        /// Please make sure that all required file system drivers are loaded and that the volume is not corrupted.
        /// </summary>
        ERROR_UNRECOGNIZED_VOLUME = 1005,
        /// <summary>
        /// The volume for a file has been externally altered so that the opened file is no longer valid.
        /// </summary>
        ERROR_FILE_INVALID = 1006,
        /// <summary>
        /// The requested operation cannot be performed in full-screen mode.
        /// </summary>
        ERROR_FULLSCREEN_MODE = 1007,
        /// <summary>
        /// An attempt was made to reference a token that does not exist.
        /// </summary>
        ERROR_NO_TOKEN = 1008,
        /// <summary>
        /// The configuration registry database is corrupt.
        /// </summary>
        ERROR_BADDB = 1009,
        /// <summary>
        /// The configuration registry key is invalid.
        /// </summary>
        ERROR_BADKEY = 1010,
        /// <summary>
        /// The configuration registry key could not be opened.
        /// </summary>
        ERROR_CANTOPEN = 1011,
        /// <summary>
        /// The configuration registry key could not be read.
        /// </summary>
        ERROR_CANTREAD = 1012,
        /// <summary>
        /// The configuration registry key could not be written.
        /// </summary>
        ERROR_CANTWRITE = 1013,
        /// <summary>
        /// One of the files in the registry database had to be recovered by use of a log or alternate copy. The recovery was successful.
        /// </summary>
        ERROR_REGISTRY_RECOVERED = 1014,
        /// <summary>
        /// The registry is corrupted. The structure of one of the files containing registry data is corrupted, or the system's memory image of the file is corrupted, or the file could not be recovered because the alternate copy or log was absent or corrupted.
        /// </summary>
        ERROR_REGISTRY_CORRUPT = 1015,
        /// <summary>
        /// An I/O operation initiated by the registry failed unrecoverably. The registry could not read in, or write out, or flush, one of the files that contain the system's image of the registry.
        /// </summary>
        ERROR_REGISTRY_IO_FAILED = 1016,
        /// <summary>
        /// The system has attempted to load or restore a file into the registry, but the specified file is not in a registry file format.
        /// </summary>
        ERROR_NOT_REGISTRY_FILE = 1017,
        /// <summary>
        /// Illegal operation attempted on a registry key that has been marked for deletion.
        /// </summary>
        ERROR_KEY_DELETED = 1018,
        /// <summary>
        /// System could not allocate the required space in a registry log.
        /// </summary>
        ERROR_NO_LOG_SPACE = 1019,
        /// <summary>
        /// Cannot create a symbolic link in a registry key that already has subkeys or values.
        /// </summary>
        ERROR_KEY_HAS_CHILDREN = 1020,
        /// <summary>
        /// Cannot create a stable subkey under a volatile parent key.
        /// </summary>
        ERROR_CHILD_MUST_BE_VOLATILE = 1021,
        /// <summary>
        /// A notify change request is being completed and the information is not being returned in the caller's buffer. The caller now needs to enumerate the files to find the changes.
        /// </summary>
        ERROR_NOTIFY_ENUM_DIR = 1022,
        /// <summary>
        /// A stop control has been sent to a service that other running services are dependent on.
        /// </summary>
        ERROR_DEPENDENT_SERVICES_RUNNING = 1051,
        /// <summary>
        /// The requested control is not valid for this service.
        /// </summary>
        ERROR_INVALID_SERVICE_CONTROL = 1052,
        /// <summary>
        /// The service did not respond to the start or control request in a timely fashion.
        /// </summary>
        ERROR_SERVICE_REQUEST_TIMEOUT = 1053,
        /// <summary>
        /// A thread could not be created for the service.
        /// </summary>
        ERROR_SERVICE_NO_THREAD = 1054,
        /// <summary>
        /// The service database is locked.
        /// </summary>
        ERROR_SERVICE_DATABASE_LOCKED = 1055,
        /// <summary>
        /// An instance of the service is already running.
        /// </summary>
        ERROR_SERVICE_ALREADY_RUNNING = 1056,
        /// <summary>
        /// The account name is invalid or does not exist, or the password is invalid for the account name specified.
        /// </summary>
        ERROR_INVALID_SERVICE_ACCOUNT = 1057,
        /// <summary>
        /// The service cannot be started, either because it is disabled or because it has no enabled devices associated with it.
        /// </summary>
        ERROR_SERVICE_DISABLED = 1058,
        /// <summary>
        /// Circular service dependency was specified.
        /// </summary>
        ERROR_CIRCULAR_DEPENDENCY = 1059,
        /// <summary>
        /// The specified service does not exist as an installed service.
        /// </summary>
        ERROR_SERVICE_DOES_NOT_EXIST = 1060,
        /// <summary>
        /// The service cannot accept control messages at this time.
        /// </summary>
        ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061,
        /// <summary>
        /// The service has not been started.
        /// </summary>
        ERROR_SERVICE_NOT_ACTIVE = 1062,
        /// <summary>
        /// The service process could not connect to the service controller.
        /// </summary>
        ERROR_FAILED_SERVICE_CONTROLLER_CONNECT = 1063,
        /// <summary>
        /// An exception occurred in the service when handling the control request.
        /// </summary>
        ERROR_EXCEPTION_IN_SERVICE = 1064,
        /// <summary>
        /// The database specified does not exist.
        /// </summary>
        ERROR_DATABASE_DOES_NOT_EXIST = 1065,
        /// <summary>
        /// The service has returned a service-specific error code.
        /// </summary>
        ERROR_SERVICE_SPECIFIC_ERROR = 1066,
        /// <summary>
        /// The process terminated unexpectedly.
        /// </summary>
        ERROR_PROCESS_ABORTED = 1067,
        /// <summary>
        /// The dependency service or group failed to start.
        /// </summary>
        ERROR_SERVICE_DEPENDENCY_FAIL = 1068,
        /// <summary>
        /// The service did not start due to a logon failure.
        /// </summary>
        ERROR_SERVICE_LOGON_FAILED = 1069,
        /// <summary>
        /// After starting, the service hung in a start-pending state.
        /// </summary>
        ERROR_SERVICE_START_HANG = 1070,
        /// <summary>
        /// The specified service database lock is invalid.
        /// </summary>
        ERROR_INVALID_SERVICE_LOCK = 1071,
        /// <summary>
        /// The specified service has been marked for deletion.
        /// </summary>
        ERROR_SERVICE_MARKED_FOR_DELETE = 1072,
        /// <summary>
        /// The specified service already exists.
        /// </summary>
        ERROR_SERVICE_EXISTS = 1073,
        /// <summary>
        /// The system is currently running with the last-known-good configuration.
        /// </summary>
        ERROR_ALREADY_RUNNING_LKG = 1074,
        /// <summary>
        /// The dependency service does not exist or has been marked for deletion.
        /// </summary>
        ERROR_SERVICE_DEPENDENCY_DELETED = 1075,
        /// <summary>
        /// The current boot has already been accepted for use as the last-known-good control set.
        /// </summary>
        ERROR_BOOT_ALREADY_ACCEPTED = 1076,
        /// <summary>
        /// No attempts to start the service have been made since the last boot.
        /// </summary>
        ERROR_SERVICE_NEVER_STARTED = 1077,
        /// <summary>
        /// The name is already in use as either a service name or a service display name.
        /// </summary>
        ERROR_DUPLICATE_SERVICE_NAME = 1078,
        /// <summary>
        /// The account specified for this service is different from the account specified for other services running in the same process.
        /// </summary>
        ERROR_DIFFERENT_SERVICE_ACCOUNT = 1079,
        /// <summary>
        /// Failure actions can only be set for Win32 services, not for drivers.
        /// </summary>
        ERROR_CANNOT_DETECT_DRIVER_FAILURE = 1080,
        /// <summary>
        /// This service runs in the same process as the service control manager.
        /// Therefore, the service control manager cannot take action if this service's process terminates unexpectedly.
        /// </summary>
        ERROR_CANNOT_DETECT_PROCESS_ABORT = 1081,
        /// <summary>
        /// No recovery program has been configured for this service.
        /// </summary>
        ERROR_NO_RECOVERY_PROGRAM = 1082,
        /// <summary>
        /// The executable program that this service is configured to run in does not implement the service.
        /// </summary>
        ERROR_SERVICE_NOT_IN_EXE = 1083,
        /// <summary>
        /// This service cannot be started in Safe Mode
        /// </summary>
        ERROR_NOT_SAFEBOOT_SERVICE = 1084,
        /// <summary>
        /// The physical end of the tape has been reached.
        /// </summary>
        ERROR_END_OF_MEDIA = 1100,
        /// <summary>
        /// A tape access reached a filemark.
        /// </summary>
        ERROR_FILEMARK_DETECTED = 1101,
        /// <summary>
        /// The beginning of the tape or a partition was encountered.
        /// </summary>
        ERROR_BEGINNING_OF_MEDIA = 1102,
        /// <summary>
        /// A tape access reached the end of a set of files.
        /// </summary>
        ERROR_SETMARK_DETECTED = 1103,
        /// <summary>
        /// No more data is on the tape.
        /// </summary>
        ERROR_NO_DATA_DETECTED = 1104,
        /// <summary>
        /// Tape could not be partitioned.
        /// </summary>
        ERROR_PARTITION_FAILURE = 1105,
        /// <summary>
        /// When accessing a new tape of a multivolume partition, the current block size is incorrect.
        /// </summary>
        ERROR_INVALID_BLOCK_LENGTH = 1106,
        /// <summary>
        /// Tape partition information could not be found when loading a tape.
        /// </summary>
        ERROR_DEVICE_NOT_PARTITIONED = 1107,
        /// <summary>
        /// Unable to lock the media eject mechanism.
        /// </summary>
        ERROR_UNABLE_TO_LOCK_MEDIA = 1108,
        /// <summary>
        /// Unable to unload the media.
        /// </summary>
        ERROR_UNABLE_TO_UNLOAD_MEDIA = 1109,
        /// <summary>
        /// The media in the drive may have changed.
        /// </summary>
        ERROR_MEDIA_CHANGED = 1110,
        /// <summary>
        /// The I/O bus was reset.
        /// </summary>
        ERROR_BUS_RESET = 1111,
        /// <summary>
        /// No media in drive.
        /// </summary>
        ERROR_NO_MEDIA_IN_DRIVE = 1112,
        /// <summary>
        /// No mapping for the Unicode character exists in the target multi-byte code page.
        /// </summary>
        ERROR_NO_UNICODE_TRANSLATION = 1113,
        /// <summary>
        /// A dynamic link library (DLL) initialization routine failed.
        /// </summary>
        ERROR_DLL_INIT_FAILED = 1114,
        /// <summary>
        /// A system shutdown is in progress.
        /// </summary>
        ERROR_SHUTDOWN_IN_PROGRESS = 1115,
        /// <summary>
        /// Unable to abort the system shutdown because no shutdown was in progress.
        /// </summary>
        ERROR_NO_SHUTDOWN_IN_PROGRESS = 1116,
        /// <summary>
        /// The request could not be performed because of an I/O device error.
        /// </summary>
        ERROR_IO_DEVICE = 1117,
        /// <summary>
        /// No serial device was successfully initialized. The serial driver will unload.
        /// </summary>
        ERROR_SERIAL_NO_DEVICE = 1118,
        /// <summary>
        /// Unable to open a device that was sharing an interrupt request (IRQ) with other devices. At least one other device that uses that IRQ was already opened.
        /// </summary>
        ERROR_IRQ_BUSY = 1119,
        /// <summary>
        /// A serial I/O operation was completed by another write to the serial port.
        /// (The IOCTL_SERIAL_XOFF_COUNTER reached zero.)
        /// </summary>
        ERROR_MORE_WRITES = 1120,
        /// <summary>
        /// A serial I/O operation completed because the timeout period expired.
        /// (The IOCTL_SERIAL_XOFF_COUNTER did not reach zero.)
        /// </summary>
        ERROR_COUNTER_TIMEOUT = 1121,
        /// <summary>
        /// No ID address mark was found on the floppy disk.
        /// </summary>
        ERROR_FLOPPY_ID_MARK_NOT_FOUND = 1122,
        /// <summary>
        /// Mismatch between the floppy disk sector ID field and the floppy disk controller track address.
        /// </summary>
        ERROR_FLOPPY_WRONG_CYLINDER = 1123,
        /// <summary>
        /// The floppy disk controller reported an error that is not recognized by the floppy disk driver.
        /// </summary>
        ERROR_FLOPPY_UNKNOWN_ERROR = 1124,
        /// <summary>
        /// The floppy disk controller returned inconsistent results in its registers.
        /// </summary>
        ERROR_FLOPPY_BAD_REGISTERS = 1125,
        /// <summary>
        /// While accessing the hard disk, a recalibrate operation failed, even after retries.
        /// </summary>
        ERROR_DISK_RECALIBRATE_FAILED = 1126,
        /// <summary>
        /// While accessing the hard disk, a disk operation failed even after retries.
        /// </summary>
        ERROR_DISK_OPERATION_FAILED = 1127,
        /// <summary>
        /// While accessing the hard disk, a disk controller reset was needed, but even that failed.
        /// </summary>
        ERROR_DISK_RESET_FAILED = 1128,
        /// <summary>
        /// Physical end of tape encountered.
        /// </summary>
        ERROR_EOM_OVERFLOW = 1129,
        /// <summary>
        /// Not enough server storage is available to process this command.
        /// </summary>
        ERROR_NOT_ENOUGH_SERVER_MEMORY = 1130,
        /// <summary>
        /// A potential deadlock condition has been detected.
        /// </summary>
        ERROR_POSSIBLE_DEADLOCK = 1131,
        /// <summary>
        /// The base address or the file offset specified does not have the proper alignment.
        /// </summary>
        ERROR_MAPPED_ALIGNMENT = 1132,
        /// <summary>
        /// An attempt to change the system power state was vetoed by another application or driver.
        /// </summary>
        ERROR_SET_POWER_STATE_VETOED = 1140,
        /// <summary>
        /// The system BIOS failed an attempt to change the system power state.
        /// </summary>
        ERROR_SET_POWER_STATE_FAILED = 1141,
        /// <summary>
        /// An attempt was made to create more links on a file than the file system supports.
        /// </summary>
        ERROR_TOO_MANY_LINKS = 1142,
        /// <summary>
        /// The specified program requires a newer version of Windows.
        /// </summary>
        ERROR_OLD_WIN_VERSION = 1150,
        /// <summary>
        /// The specified program is not a Windows or MS-DOS program.
        /// </summary>
        ERROR_APP_WRONG_OS = 1151,
        /// <summary>
        /// Cannot start more than one instance of the specified program.
        /// </summary>
        ERROR_SINGLE_INSTANCE_APP = 1152,
        /// <summary>
        /// The specified program was written for an earlier version of Windows.
        /// </summary>
        ERROR_RMODE_APP = 1153,
        /// <summary>
        /// One of the library files needed to run this application is damaged.
        /// </summary>
        ERROR_INVALID_DLL = 1154,
        /// <summary>
        /// No application is associated with the specified file for this operation.
        /// </summary>
        ERROR_NO_ASSOCIATION = 1155,
        /// <summary>
        /// An error occurred in sending the command to the application.
        /// </summary>
        ERROR_DDE_FAIL = 1156,
        /// <summary>
        /// One of the library files needed to run this application cannot be found.
        /// </summary>
        ERROR_DLL_NOT_FOUND = 1157,
        /// <summary>
        /// The current process has used all of its system allowance of handles for Window Manager objects.
        /// </summary>
        ERROR_NO_MORE_USER_HANDLES = 1158,
        /// <summary>
        /// The message can be used only with synchronous operations.
        /// </summary>
        ERROR_MESSAGE_SYNC_ONLY = 1159,
        /// <summary>
        /// The indicated source element has no media.
        /// </summary>
        ERROR_SOURCE_ELEMENT_EMPTY = 1160,
        /// <summary>
        /// The indicated destination element already contains media.
        /// </summary>
        ERROR_DESTINATION_ELEMENT_FULL = 1161,
        /// <summary>
        /// The indicated element does not exist.
        /// </summary>
        ERROR_ILLEGAL_ELEMENT_ADDRESS = 1162,
        /// <summary>
        /// The indicated element is part of a magazine that is not present.
        /// </summary>
        ERROR_MAGAZINE_NOT_PRESENT = 1163,
        /// <summary>
        /// The indicated device requires reinitialization due to hardware errors.
        /// </summary>
        ERROR_DEVICE_REINITIALIZATION_NEEDED = 1164,
        /// <summary>
        /// The device has indicated that cleaning is required before further operations are attempted.
        /// </summary>
        ERROR_DEVICE_REQUIRES_CLEANING = 1165,
        /// <summary>
        /// The device has indicated that its door is open.
        /// </summary>
        ERROR_DEVICE_DOOR_OPEN = 1166,
        /// <summary>
        /// The device is not connected.
        /// </summary>
        ERROR_DEVICE_NOT_CONNECTED = 1167,
        /// <summary>
        /// Element not found.
        /// </summary>
        ERROR_NOT_FOUND = 1168,
        /// <summary>
        /// There was no match for the specified key in the index.
        /// </summary>
        ERROR_NO_MATCH = 1169,
        /// <summary>
        /// The property set specified does not exist on the object.
        /// </summary>
        ERROR_SET_NOT_FOUND = 1170,
        /// <summary>
        /// The point passed to GetMouseMovePoints is not in the buffer.
        /// </summary>
        ERROR_POINT_NOT_FOUND = 1171,
        /// <summary>
        /// The tracking (workstation) service is not running.
        /// </summary>
        ERROR_NO_TRACKING_SERVICE = 1172,
        /// <summary>
        /// The Volume ID could not be found.
        /// </summary>
        ERROR_NO_VOLUME_ID = 1173,
        /// <summary>
        /// Unable to remove the file to be replaced.
        /// </summary>
        ERROR_UNABLE_TO_REMOVE_REPLACED = 1175,
        /// <summary>
        /// Unable to move the replacement file to the file to be replaced. The file to be replaced has retained its original name.
        /// </summary>
        ERROR_UNABLE_TO_MOVE_REPLACEMENT = 1176,
        /// <summary>
        /// Unable to move the replacement file to the file to be replaced. The file to be replaced has been renamed using the backup name.
        /// </summary>
        ERROR_UNABLE_TO_MOVE_REPLACEMENT_2 = 1177,
        /// <summary>
        /// The volume change journal is being deleted.
        /// </summary>
        ERROR_JOURNAL_DELETE_IN_PROGRESS = 1178,
        /// <summary>
        /// The volume change journal is not active.
        /// </summary>
        ERROR_JOURNAL_NOT_ACTIVE = 1179,
        /// <summary>
        /// A file was found, but it may not be the correct file.
        /// </summary>
        ERROR_POTENTIAL_FILE_FOUND = 1180,
        /// <summary>
        /// The journal entry has been deleted from the journal.
        /// </summary>
        ERROR_JOURNAL_ENTRY_DELETED = 1181,
        /// <summary>
        /// The specified device name is invalid.
        /// </summary>
        ERROR_BAD_DEVICE = 1200,
        /// <summary>
        /// The device is not currently connected but it is a remembered connection.
        /// </summary>
        ERROR_CONNECTION_UNAVAIL = 1201,
        /// <summary>
        /// The local device name has a remembered connection to another network resource.
        /// </summary>
        ERROR_DEVICE_ALREADY_REMEMBERED = 1202,
        /// <summary>
        /// No network provider accepted the given network path.
        /// </summary>
        ERROR_NO_NET_OR_BAD_PATH = 1203,
        /// <summary>
        /// The specified network provider name is invalid.
        /// </summary>
        ERROR_BAD_PROVIDER = 1204,
        /// <summary>
        /// Unable to open the network connection profile.
        /// </summary>
        ERROR_CANNOT_OPEN_PROFILE = 1205,
        /// <summary>
        /// The network connection profile is corrupted.
        /// </summary>
        ERROR_BAD_PROFILE = 1206,
        /// <summary>
        /// Cannot enumerate a noncontainer.
        /// </summary>
        ERROR_NOT_CONTAINER = 1207,
        /// <summary>
        /// An extended error has occurred.
        /// </summary>
        ERROR_EXTENDED_ERROR = 1208,
        /// <summary>
        /// The format of the specified group name is invalid.
        /// </summary>
        ERROR_INVALID_GROUPNAME = 1209,
        /// <summary>
        /// The format of the specified computer name is invalid.
        /// </summary>
        ERROR_INVALID_COMPUTERNAME = 1210,
        /// <summary>
        /// The format of the specified event name is invalid.
        /// </summary>
        ERROR_INVALID_EVENTNAME = 1211,
        /// <summary>
        /// The format of the specified domain name is invalid.
        /// </summary>
        ERROR_INVALID_DOMAINNAME = 1212,
        /// <summary>
        /// The format of the specified service name is invalid.
        /// </summary>
        ERROR_INVALID_SERVICENAME = 1213,
        /// <summary>
        /// The format of the specified network name is invalid.
        /// </summary>
        ERROR_INVALID_NETNAME = 1214,
        /// <summary>
        /// The format of the specified share name is invalid.
        /// </summary>
        ERROR_INVALID_SHARENAME = 1215,
        /// <summary>
        /// The format of the specified password is invalid.
        /// </summary>
        ERROR_INVALID_PASSUInt16NAME = 1216,
        /// <summary>
        /// The format of the specified message name is invalid.
        /// </summary>
        ERROR_INVALID_MESSAGENAME = 1217,
        /// <summary>
        /// The format of the specified message destination is invalid.
        /// </summary>
        ERROR_INVALID_MESSAGEDEST = 1218,
        /// <summary>
        /// Multiple connections to a server or shared resource by the same user, using more than one user name, are not allowed. Disconnect all previous connections to the server or shared resource and try again..
        /// </summary>
        ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,
        /// <summary>
        /// An attempt was made to establish a session to a network server, but there are already too many sessions established to that server.
        /// </summary>
        ERROR_REMOTE_SESSION_LIMIT_EXCEEDED = 1220,
        /// <summary>
        /// The workgroup or domain name is already in use by another computer on the network.
        /// </summary>
        ERROR_DUP_DOMAINNAME = 1221,
        /// <summary>
        /// The network is not present or not started.
        /// </summary>
        ERROR_NO_NETWORK = 1222,
        /// <summary>
        /// The operation was canceled by the user.
        /// </summary>
        ERROR_CANCELLED = 1223,
        /// <summary>
        /// The requested operation cannot be performed on a file with a user-mapped section open.
        /// </summary>
        ERROR_USER_MAPPED_FILE = 1224,
        /// <summary>
        /// The remote system refused the network connection.
        /// </summary>
        ERROR_CONNECTION_REFUSED = 1225,
        /// <summary>
        /// The network connection was gracefully closed.
        /// </summary>
        ERROR_GRACEFUL_DISCONNECT = 1226,
        /// <summary>
        /// The network transport endpoint already has an address associated with it.
        /// </summary>
        ERROR_ADDRESS_ALREADY_ASSOCIATED = 1227,
        /// <summary>
        /// An address has not yet been associated with the network endpoint.
        /// </summary>
        ERROR_ADDRESS_NOT_ASSOCIATED = 1228,
        /// <summary>
        /// An operation was attempted on a nonexistent network connection.
        /// </summary>
        ERROR_CONNECTION_INVALID = 1229,
        /// <summary>
        /// An invalid operation was attempted on an active network connection.
        /// </summary>
        ERROR_CONNECTION_ACTIVE = 1230,
        /// <summary>
        /// The network location cannot be reached. For information about network troubleshooting, see Windows Help.
        /// </summary>
        ERROR_NETWORK_UNREACHABLE = 1231,
        /// <summary>
        /// The network location cannot be reached. For information about network troubleshooting, see Windows Help.
        /// </summary>
        ERROR_HOST_UNREACHABLE = 1232,
        /// <summary>
        /// The network location cannot be reached. For information about network troubleshooting, see Windows Help.
        /// </summary>
        ERROR_PROTOCOL_UNREACHABLE = 1233,
        /// <summary>
        /// No service is operating at the destination network endpoint on the remote system.
        /// </summary>
        ERROR_PORT_UNREACHABLE = 1234,
        /// <summary>
        /// The request was aborted.
        /// </summary>
        ERROR_REQUEST_ABORTED = 1235,
        /// <summary>
        /// The network connection was aborted by the local system.
        /// </summary>
        ERROR_CONNECTION_ABORTED = 1236,
        /// <summary>
        /// The operation could not be completed. A retry should be performed.
        /// </summary>
        ERROR_RETRY = 1237,
        /// <summary>
        /// A connection to the server could not be made because the limit on the number of concurrent connections for this account has been reached.
        /// </summary>
        ERROR_CONNECTION_COUNT_LIMIT = 1238,
        /// <summary>
        /// Attempting to log in during an unauthorized time of day for this account.
        /// </summary>
        ERROR_LOGIN_TIME_RESTRICTION = 1239,
        /// <summary>
        /// The account is not authorized to log in from this station.
        /// </summary>
        ERROR_LOGIN_WKSTA_RESTRICTION = 1240,
        /// <summary>
        /// The network address could not be used for the operation requested.
        /// </summary>
        ERROR_INCORRECT_ADDRESS = 1241,
        /// <summary>
        /// The service is already registered.
        /// </summary>
        ERROR_ALREADY_REGISTERED = 1242,
        /// <summary>
        /// The specified service does not exist.
        /// </summary>
        ERROR_SERVICE_NOT_FOUND = 1243,
        /// <summary>
        /// The operation being requested was not performed because the user has not been authenticated.
        /// </summary>
        ERROR_NOT_AUTHENTICATED = 1244,
        /// <summary>
        /// The operation being requested was not performed because the user has not logged on to the network.
        /// The specified service does not exist.
        /// </summary>
        ERROR_NOT_LOGGED_ON = 1245,
        /// <summary>
        /// Continue with work in progress.
        /// </summary>
        ERROR_CONTINUE = 1246,
        /// <summary>
        /// An attempt was made to perform an initialization operation when initialization has already been completed.
        /// </summary>
        ERROR_ALREADY_INITIALIZED = 1247,
        /// <summary>
        /// No more local devices.
        /// </summary>
        ERROR_NO_MORE_DEVICES = 1248,
        /// <summary>
        /// The specified site does not exist.
        /// </summary>
        ERROR_NO_SUCH_SITE = 1249,
        /// <summary>
        /// A domain controller with the specified name already exists.
        /// </summary>
        ERROR_DOMAIN_CONTROLLER_EXISTS = 1250,
        /// <summary>
        /// This operation is supported only when you are connected to the server.
        /// </summary>
        ERROR_ONLY_IF_CONNECTED = 1251,
        /// <summary>
        /// The group policy framework should call the extension even if there are no changes.
        /// </summary>
        ERROR_OVERRIDE_NOCHANGES = 1252,
        /// <summary>
        /// The specified user does not have a valid profile.
        /// </summary>
        ERROR_BAD_USER_PROFILE = 1253,
        /// <summary>
        /// This operation is not supported on a Microsoft Small Business Server
        /// </summary>
        ERROR_NOT_SUPPORTED_ON_SBS = 1254,
        /// <summary>
        /// The server machine is shutting down.
        /// </summary>
        ERROR_SERVER_SHUTDOWN_IN_PROGRESS = 1255,
        /// <summary>
        /// The remote system is not available. For information about network troubleshooting, see Windows Help.
        /// </summary>
        ERROR_HOST_DOWN = 1256,
        /// <summary>
        /// The security identifier provided is not from an account domain.
        /// </summary>
        ERROR_NON_ACCOUNT_SID = 1257,
        /// <summary>
        /// The security identifier provided does not have a domain component.
        /// </summary>
        ERROR_NON_DOMAIN_SID = 1258,
        /// <summary>
        /// AppHelp dialog canceled thus preventing the application from starting.
        /// </summary>
        ERROR_APPHELP_BLOCK = 1259,
        /// <summary>
        /// Windows cannot open this program because it has been prevented by a software restriction policy. For more information, open Event Viewer or contact your system administrator.
        /// </summary>
        ERROR_ACCESS_DISABLED_BY_POLICY = 1260,
        /// <summary>
        /// A program attempt to use an invalid register value.  Normally caused by an uninitialized register. This error is Itanium specific.
        /// </summary>
        ERROR_REG_NAT_CONSUMPTION = 1261,
        /// <summary>
        /// The share is currently offline or does not exist.
        /// </summary>
        ERROR_CSCSHARE_OFFLINE = 1262,
        /// <summary>
        /// The kerberos protocol encountered an error while validating the
        /// KDC certificate during smartcard logon.
        /// </summary>
        ERROR_PKINIT_FAILURE = 1263,
        /// <summary>
        /// The kerberos protocol encountered an error while attempting to utilize
        /// the smartcard subsystem.
        /// </summary>
        ERROR_SMARTCARD_SUBSYSTEM_FAILURE = 1264,
        /// <summary>
        /// The system detected a possible attempt to compromise security. Please ensure that you can contact the server that authenticated you.
        /// </summary>
        ERROR_DOWNGRADE_DETECTED = 1265,
        /// <summary>
        /// The machine is locked and can not be shut down without the force option.
        /// </summary>
        ERROR_MACHINE_LOCKED = 1271,
        /// <summary>
        /// An application-defined callback gave invalid data when called.
        /// </summary>
        ERROR_CALLBACK_SUPPLIED_INVALID_DATA = 1273,
        /// <summary>
        /// The group policy framework should call the extension in the synchronous foreground policy refresh.
        /// </summary>
        ERROR_SYNC_FOREGROUND_REFRESH_REQUIRED = 1274,
        /// <summary>
        /// This driver has been blocked from loading
        /// </summary>
        ERROR_DRIVER_BLOCKED = 1275,
        /// <summary>
        /// A dynamic link library (DLL) referenced a module that was neither a DLL nor the process's executable image.
        /// </summary>
        ERROR_INVALID_IMPORT_OF_NON_DLL = 1276,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_ACCESS_DISABLED_WEBBLADE = 1277,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_ACCESS_DISABLED_WEBBLADE_TAMPER = 1278,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_RECOVERY_FAILURE = 1279,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_ALREADY_FIBER = 1280,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_ALREADY_THREAD = 1281,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_STACK_BUFFER_OVERRUN = 1282,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_PARAMETER_QUOTA_EXCEEDED = 1283,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DEBUGGER_INACTIVE = 1284,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DELAY_LOAD_FAILED = 1285,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_VDM_DISALLOWED = 1286,
        /// <summary>
        /// Not all privileges referenced are assigned to the caller.
        /// </summary>
        ERROR_NOT_ALL_ASSIGNED = 1300,
        /// <summary>
        /// Some mapping between account names and security IDs was not done.
        /// </summary>
        ERROR_SOME_NOT_MAPPED = 1301,
        /// <summary>
        /// No system quota limits are specifically set for this account.
        /// </summary>
        ERROR_NO_QUOTAS_FOR_ACCOUNT = 1302,
        /// <summary>
        /// No encryption key is available. A well-known encryption key was returned.
        /// </summary>
        ERROR_LOCAL_USER_SESSION_KEY = 1303,
        /// <summary>
        /// The password is too complex to be converted to a LAN Manager password. The LAN Manager password returned is a NULL string.
        /// </summary>
        ERROR_NULL_LM_PASSUInt16 = 1304,
        /// <summary>
        /// The revision level is unknown.
        /// </summary>
        ERROR_UNKNOWN_REVISION = 1305,
        /// <summary>
        /// Indicates two revision levels are incompatible.
        /// </summary>
        ERROR_REVISION_MISMATCH = 1306,
        /// <summary>
        /// This security ID may not be assigned as the owner of this object.
        /// </summary>
        ERROR_INVALID_OWNER = 1307,
        /// <summary>
        /// This security ID may not be assigned as the primary group of an object.
        /// </summary>
        ERROR_INVALID_PRIMARY_GROUP = 1308,
        /// <summary>
        /// An attempt has been made to operate on an impersonation token by a thread that is not currently impersonating a client.
        /// </summary>
        ERROR_NO_IMPERSONATION_TOKEN = 1309,
        /// <summary>
        /// The group may not be disabled.
        /// </summary>
        ERROR_CANT_DISABLE_MANDATORY = 1310,
        /// <summary>
        /// There are currently no logon servers available to service the logon request.
        /// </summary>
        ERROR_NO_LOGON_SERVERS = 1311,
        /// <summary>
        /// A specified logon session does not exist. It may already have been terminated.
        /// </summary>
        ERROR_NO_SUCH_LOGON_SESSION = 1312,
        /// <summary>
        /// A specified privilege does not exist.
        /// </summary>
        ERROR_NO_SUCH_PRIVILEGE = 1313,
        /// <summary>
        /// A required privilege is not held by the client.
        /// </summary>
        ERROR_PRIVILEGE_NOT_HELD = 1314,
        /// <summary>
        /// The name provided is not a properly formed account name.
        /// </summary>
        ERROR_INVALID_ACCOUNT_NAME = 1315,
        /// <summary>
        /// The specified user already exists.
        /// </summary>
        ERROR_USER_EXISTS = 1316,
        /// <summary>
        /// The specified user does not exist.
        /// </summary>
        ERROR_NO_SUCH_USER = 1317,
        /// <summary>
        /// The specified group already exists.
        /// </summary>
        ERROR_GROUP_EXISTS = 1318,
        /// <summary>
        /// The specified group does not exist.
        /// </summary>
        ERROR_NO_SUCH_GROUP = 1319,
        /// <summary>
        /// Either the specified user account is already a member of the specified group, or the specified group cannot be deleted because it contains a member.
        /// </summary>
        ERROR_MEMBER_IN_GROUP = 1320,
        /// <summary>
        /// The specified user account is not a member of the specified group account.
        /// </summary>
        ERROR_MEMBER_NOT_IN_GROUP = 1321,
        /// <summary>
        /// The last remaining administration account cannot be disabled or deleted.
        /// </summary>
        ERROR_LAST_ADMIN = 1322,
        /// <summary>
        /// Unable to update the password. The value provided as the current password is incorrect.
        /// </summary>
        ERROR_WRONG_PASSWORD = 1323,
        /// <summary>
        /// Unable to update the password. The value provided for the new password contains values that are not allowed in passwords.
        /// </summary>
        ERROR_ILL_FORMED_PASSWORD = 1324,
        /// <summary>
        /// Unable to update the password. The value provided for the new password does not meet the length, complexity, or history requirement of the domain.
        /// </summary>
        ERROR_PASSWORD_RESTRICTION = 1325,
        /// <summary>
        /// Logon failure: unknown user name or bad password.
        /// </summary>
        ERROR_LOGON_FAILURE = 1326,
        /// <summary>
        /// Logon failure: user account restriction.  Possible reasons are blank passwords not allowed, logon hour restrictions, or a policy restriction has been enforced.
        /// </summary>
        ERROR_ACCOUNT_RESTRICTION = 1327,
        /// <summary>
        /// Logon failure: account logon time restriction violation.
        /// </summary>
        ERROR_INVALID_LOGON_HOURS = 1328,
        /// <summary>
        /// Logon failure: user not allowed to log on to this computer.
        /// </summary>
        ERROR_INVALID_WORKSTATION = 1329,
        /// <summary>
        /// Logon failure: the specified account password has expired.
        /// </summary>
        ERROR_PASSUInt16_EXPIRED = 1330,
        /// <summary>
        /// Logon failure: account currently disabled.
        /// </summary>
        ERROR_ACCOUNT_DISABLED = 1331,
        /// <summary>
        /// No mapping between account names and security IDs was done.
        /// </summary>
        ERROR_NONE_MAPPED = 1332,
        /// <summary>
        /// Too many local user identifiers (LUIDs) were requested at one time.
        /// </summary>
        ERROR_TOO_MANY_LUIDS_REQUESTED = 1333,
        /// <summary>
        /// No more local user identifiers (LUIDs) are available.
        /// </summary>
        ERROR_LUIDS_EXHAUSTED = 1334,
        /// <summary>
        /// The subauthority part of a security ID is invalid for this particular use.
        /// </summary>
        ERROR_INVALID_SUB_AUTHORITY = 1335,
        /// <summary>
        /// The access control list (ACL) structure is invalid.
        /// </summary>
        ERROR_INVALID_ACL = 1336,
        /// <summary>
        /// The security ID structure is invalid.
        /// </summary>
        ERROR_INVALID_SID = 1337,
        /// <summary>
        /// The security descriptor structure is invalid.
        /// </summary>
        ERROR_INVALID_SECURITY_DESCR = 1338,
        /// <summary>
        /// The inherited access control list (ACL) or access control entry (ACE) could not be built.
        /// </summary>
        ERROR_BAD_INHERITANCE_ACL = 1340,
        /// <summary>
        /// The server is currently disabled.
        /// </summary>
        ERROR_SERVER_DISABLED = 1341,
        /// <summary>
        /// The server is currently enabled.
        /// </summary>
        ERROR_SERVER_NOT_DISABLED = 1342,
        /// <summary>
        /// The value provided was an invalid value for an identifier authority.
        /// </summary>
        ERROR_INVALID_ID_AUTHORITY = 1343,
        /// <summary>
        /// No more memory is available for security information updates.
        /// </summary>
        ERROR_ALLOTTED_SPACE_EXCEEDED = 1344,
        /// <summary>
        /// The specified attributes are invalid, or incompatible with the attributes for the group as a whole.
        /// </summary>
        ERROR_INVALID_GROUP_ATTRIBUTES = 1345,
        /// <summary>
        /// Either a required impersonation level was not provided, or the provided impersonation level is invalid.
        /// </summary>
        ERROR_BAD_IMPERSONATION_LEVEL = 1346,
        /// <summary>
        /// Cannot open an anonymous level security token.
        /// </summary>
        ERROR_CANT_OPEN_ANONYMOUS = 1347,
        /// <summary>
        /// The validation information class requested was invalid.
        /// </summary>
        ERROR_BAD_VALIDATION_CLASS = 1348,
        /// <summary>
        /// The type of the token is inappropriate for its attempted use.
        /// </summary>
        ERROR_BAD_TOKEN_TYPE = 1349,
        /// <summary>
        /// Unable to perform a security operation on an object that has no associated security.
        /// </summary>
        ERROR_NO_SECURITY_ON_OBJECT = 1350,
        /// <summary>
        /// Configuration information could not be read from the domain controller, either because the machine is unavailable, or access has been denied.
        /// </summary>
        ERROR_CANT_ACCESS_DOMAIN_INFO = 1351,
        /// <summary>
        /// The security account manager (SAM) or local security authority (LSA) server was in the wrong state to perform the security operation.
        /// </summary>
        ERROR_INVALID_SERVER_STATE = 1352,
        /// <summary>
        /// The domain was in the wrong state to perform the security operation.
        /// </summary>
        ERROR_INVALID_DOMAIN_STATE = 1353,
        /// <summary>
        /// This operation is only allowed for the Primary Domain Controller of the domain.
        /// </summary>
        ERROR_INVALID_DOMAIN_ROLE = 1354,
        /// <summary>
        /// The specified domain either does not exist or could not be contacted.
        /// </summary>
        ERROR_NO_SUCH_DOMAIN = 1355,
        /// <summary>
        /// The specified domain already exists.
        /// </summary>
        ERROR_DOMAIN_EXISTS = 1356,
        /// <summary>
        /// An attempt was made to exceed the limit on the number of domains per server.
        /// </summary>
        ERROR_DOMAIN_LIMIT_EXCEEDED = 1357,
        /// <summary>
        /// Unable to complete the requested operation because of either a catastrophic media failure or a data structure corruption on the disk.
        /// </summary>
        ERROR_INTERNAL_DB_CORRUPTION = 1358,
        /// <summary>
        /// An internal error occurred.
        /// </summary>
        ERROR_INTERNAL_ERROR = 1359,
        /// <summary>
        /// Generic access types were contained in an access mask which should already be mapped to nongeneric types.
        /// </summary>
        ERROR_GENERIC_NOT_MAPPED = 1360,
        /// <summary>
        /// A security descriptor is not in the right format (absolute or self-relative).
        /// </summary>
        ERROR_BAD_DESCRIPTOR_FORMAT = 1361,
        /// <summary>
        /// The requested action is restricted for use by logon processes only. The calling process has not registered as a logon process.
        /// </summary>
        ERROR_NOT_LOGON_PROCESS = 1362,
        /// <summary>
        /// Cannot start a new logon session with an ID that is already in use.
        /// </summary>
        ERROR_LOGON_SESSION_EXISTS = 1363,
        /// <summary>
        /// A specified authentication package is unknown.
        /// </summary>
        ERROR_NO_SUCH_PACKAGE = 1364,
        /// <summary>
        /// The logon session is not in a state that is consistent with the requested operation.
        /// </summary>
        ERROR_BAD_LOGON_SESSION_STATE = 1365,
        /// <summary>
        /// The logon session ID is already in use.
        /// </summary>
        ERROR_LOGON_SESSION_COLLISION = 1366,
        /// <summary>
        /// A logon request contained an invalid logon type value.
        /// </summary>
        ERROR_INVALID_LOGON_TYPE = 1367,
        /// <summary>
        /// Unable to impersonate using a named pipe until data has been read from that pipe.
        /// </summary>
        ERROR_CANNOT_IMPERSONATE = 1368,
        /// <summary>
        /// The transaction state of a registry subtree is incompatible with the requested operation.
        /// </summary>
        ERROR_RXACT_INVALID_STATE = 1369,
        /// <summary>
        /// An internal security database corruption has been encountered.
        /// </summary>
        ERROR_RXACT_COMMIT_FAILURE = 1370,
        /// <summary>
        /// Cannot perform this operation on built-in accounts.
        /// </summary>
        ERROR_SPECIAL_ACCOUNT = 1371,
        /// <summary>
        /// Cannot perform this operation on this built-in special group.
        /// </summary>
        ERROR_SPECIAL_GROUP = 1372,
        /// <summary>
        /// Cannot perform this operation on this built-in special user.
        /// </summary>
        ERROR_SPECIAL_USER = 1373,
        /// <summary>
        /// The user cannot be removed from a group because the group is currently the user's primary group.
        /// </summary>
        ERROR_MEMBERS_PRIMARY_GROUP = 1374,
        /// <summary>
        /// The token is already in use as a primary token.
        /// </summary>
        ERROR_TOKEN_ALREADY_IN_USE = 1375,
        /// <summary>
        /// The specified local group does not exist.
        /// </summary>
        ERROR_NO_SUCH_ALIAS = 1376,
        /// <summary>
        /// The specified account name is not a member of the local group.
        /// </summary>
        ERROR_MEMBER_NOT_IN_ALIAS = 1377,
        /// <summary>
        /// The specified account name is already a member of the local group.
        /// </summary>
        ERROR_MEMBER_IN_ALIAS = 1378,
        /// <summary>
        /// The specified local group already exists.
        /// </summary>
        ERROR_ALIAS_EXISTS = 1379,
        /// <summary>
        /// Logon failure: the user has not been granted the requested logon type at this computer.
        /// </summary>
        ERROR_LOGON_NOT_GRANTED = 1380,
        /// <summary>
        /// The maximum number of secrets that may be stored in a single system has been exceeded.
        /// </summary>
        ERROR_TOO_MANY_SECRETS = 1381,
        /// <summary>
        /// The length of a secret exceeds the maximum length allowed.
        /// </summary>
        ERROR_SECRET_TOO_Int32 = 1382,
        /// <summary>
        /// The local security authority database contains an internal inconsistency.
        /// </summary>
        ERROR_INTERNAL_DB_ERROR = 1383,
        /// <summary>
        /// During a logon attempt, the user's security context accumulated too many security IDs.
        /// </summary>
        ERROR_TOO_MANY_CONTEXT_IDS = 1384,
        /// <summary>
        /// Logon failure: the user has not been granted the requested logon type at this computer.
        /// </summary>
        ERROR_LOGON_TYPE_NOT_GRANTED = 1385,
        /// <summary>
        /// A cross-encrypted password is necessary to change a user password.
        /// </summary>
        ERROR_NT_CROSS_ENCRYPTION_REQUIRED = 1386,
        /// <summary>
        /// A member could not be added to or removed from the local group because the member does not exist.
        /// </summary>
        ERROR_NO_SUCH_MEMBER = 1387,
        /// <summary>
        /// A new member could not be added to a local group because the member has the wrong account type.
        /// </summary>
        ERROR_INVALID_MEMBER = 1388,
        /// <summary>
        /// Too many security IDs have been specified.
        /// </summary>
        ERROR_TOO_MANY_SIDS = 1389,
        /// <summary>
        /// A cross-encrypted password is necessary to change this user password.
        /// </summary>
        ERROR_LM_CROSS_ENCRYPTION_REQUIRED = 1390,
        /// <summary>
        /// Indicates an ACL contains no inheritable components.
        /// </summary>
        ERROR_NO_INHERITANCE = 1391,
        /// <summary>
        /// The file or directory is corrupted and unreadable.
        /// </summary>
        ERROR_FILE_CORRUPT = 1392,
        /// <summary>
        /// The disk structure is corrupted and unreadable.
        /// </summary>
        ERROR_DISK_CORRUPT = 1393,
        /// <summary>
        /// There is no user session key for the specified logon session.
        /// </summary>
        ERROR_NO_USER_SESSION_KEY = 1394,
        /// <summary>
        /// The service being accessed is licensed for a particular number of connections.
        /// No more connections can be made to the service at this time because there are already as many connections as the service can accept.
        /// </summary>
        ERROR_LICENSE_QUOTA_EXCEEDED = 1395,
        /// <summary>
        /// Logon Failure: The target account name is incorrect.
        /// </summary>
        ERROR_WRONG_TARGET_NAME = 1396,
        /// <summary>
        /// Mutual Authentication failed. The server's password is out of date at the domain controller.
        /// </summary>
        ERROR_MUTUAL_AUTH_FAILED = 1397,
        /// <summary>
        /// There is a time and/or date difference between the client and server.
        /// </summary>
        ERROR_TIME_SKEW = 1398,
        /// <summary>
        /// This operation can not be performed on the current domain.
        /// </summary>
        ERROR_CURRENT_DOMAIN_NOT_ALLOWED = 1399,
        /// <summary>
        /// Invalid window handle.
        /// </summary>
        ERROR_INVALID_WINDOW_HANDLE = 1400,
        /// <summary>
        /// Invalid menu handle.
        /// </summary>
        ERROR_INVALID_MENU_HANDLE = 1401,
        /// <summary>
        /// Invalid cursor handle.
        /// </summary>
        ERROR_INVALID_CURSOR_HANDLE = 1402,
        /// <summary>
        /// Invalid accelerator table handle.
        /// </summary>
        ERROR_INVALID_ACCEL_HANDLE = 1403,
        /// <summary>
        /// Invalid hook handle.
        /// </summary>
        ERROR_INVALID_HOOK_HANDLE = 1404,
        /// <summary>
        /// Invalid handle to a multiple-window position structure.
        /// </summary>
        ERROR_INVALID_DWP_HANDLE = 1405,
        /// <summary>
        /// Cannot create a top-level child window.
        /// </summary>
        ERROR_TLW_WITH_WSCHILD = 1406,
        /// <summary>
        /// Cannot find window class.
        /// </summary>
        ERROR_CANNOT_FIND_WND_CLASS = 1407,
        /// <summary>
        /// Invalid window, it belongs to other thread.
        /// </summary>
        ERROR_WINDOW_OF_OTHER_THREAD = 1408,
        /// <summary>
        /// Hot key is already registered.
        /// </summary>
        ERROR_HOTKEY_ALREADY_REGISTERED = 1409,
        /// <summary>
        /// Class already exists.
        /// </summary>
        ERROR_CLASS_ALREADY_EXISTS = 1410,
        /// <summary>
        /// Class does not exist.
        /// </summary>
        ERROR_CLASS_DOES_NOT_EXIST = 1411,
        /// <summary>
        /// Class still has open windows.
        /// </summary>
        ERROR_CLASS_HAS_WINDOWS = 1412,
        /// <summary>
        /// Invalid index.
        /// </summary>
        ERROR_INVALID_INDEX = 1413,
        /// <summary>
        /// Invalid icon handle.
        /// </summary>
        ERROR_INVALID_ICON_HANDLE = 1414,
        /// <summary>
        /// Using private DIALOG window words.
        /// </summary>
        ERROR_PRIVATE_DIALOG_INDEX = 1415,
        /// <summary>
        /// The list box identifier was not found.
        /// </summary>
        ERROR_LISTBOX_ID_NOT_FOUND = 1416,
        /// <summary>
        /// No wildcards were found.
        /// </summary>
        ERROR_NO_WILDCARD_CHARACTERS = 1417,
        /// <summary>
        /// Thread does not have a clipboard open.
        /// </summary>
        ERROR_CLIPBOARD_NOT_OPEN = 1418,
        /// <summary>
        /// Hot key is not registered.
        /// </summary>
        ERROR_HOTKEY_NOT_REGISTERED = 1419,
        /// <summary>
        /// The window is not a valid dialog window.
        /// </summary>
        ERROR_WINDOW_NOT_DIALOG = 1420,
        /// <summary>
        /// Control ID not found.
        /// </summary>
        ERROR_CONTROL_ID_NOT_FOUND = 1421,
        /// <summary>
        /// Invalid message for a combo box because it does not have an edit control.
        /// </summary>
        ERROR_INVALID_COMBOBOX_MESSAGE = 1422,
        /// <summary>
        /// The window is not a combo box.
        /// </summary>
        ERROR_WINDOW_NOT_COMBOBOX = 1423,
        /// <summary>
        /// Height must be less than 256.
        /// </summary>
        ERROR_INVALID_EDIT_HEIGHT = 1424,
        /// <summary>
        /// Invalid device context (DC) handle.
        /// </summary>
        ERROR_DC_NOT_FOUND = 1425,
        /// <summary>
        /// Invalid hook procedure type.
        /// </summary>
        ERROR_INVALID_HOOK_FILTER = 1426,
        /// <summary>
        /// Invalid hook procedure.
        /// </summary>
        ERROR_INVALID_FILTER_PROC = 1427,
        /// <summary>
        /// Cannot set nonlocal hook without a module handle.
        /// </summary>
        ERROR_HOOK_NEEDS_HMOD = 1428,
        /// <summary>
        /// This hook procedure can only be set globally.
        /// </summary>
        ERROR_GLOBAL_ONLY_HOOK = 1429,
        /// <summary>
        /// The journal hook procedure is already installed.
        /// </summary>
        ERROR_JOURNAL_HOOK_SET = 1430,
        /// <summary>
        /// The hook procedure is not installed.
        /// </summary>
        ERROR_HOOK_NOT_INSTALLED = 1431,
        /// <summary>
        /// Invalid message for single-selection list box.
        /// </summary>
        ERROR_INVALID_LB_MESSAGE = 1432,
        /// <summary>
        /// LB_SETCOUNT sent to non-lazy list box.
        /// </summary>
        ERROR_SETCOUNT_ON_BAD_LB = 1433,
        /// <summary>
        /// This list box does not support tab stops.
        /// </summary>
        ERROR_LB_WITHOUT_TABSTOPS = 1434,
        /// <summary>
        /// Cannot destroy object created by another thread.
        /// </summary>
        ERROR_DESTROY_OBJECT_OF_OTHER_THREAD = 1435,
        /// <summary>
        /// Child windows cannot have menus.
        /// </summary>
        ERROR_CHILD_WINDOW_MENU = 1436,
        /// <summary>
        /// The window does not have a system menu.
        /// </summary>
        ERROR_NO_SYSTEM_MENU = 1437,
        /// <summary>
        /// Invalid message box style.
        /// </summary>
        ERROR_INVALID_MSGBOX_STYLE = 1438,
        /// <summary>
        /// Invalid system-wide (SPI_*) parameter.
        /// </summary>
        ERROR_INVALID_SPI_VALUE = 1439,
        /// <summary>
        /// Screen already locked.
        /// </summary>
        ERROR_SCREEN_ALREADY_LOCKED = 1440,
        /// <summary>
        /// All handles to windows in a multiple-window position structure must have the same parent.
        /// </summary>
        ERROR_HWNDS_HAVE_DIFF_PARENT = 1441,
        /// <summary>
        /// The window is not a child window.
        /// </summary>
        ERROR_NOT_CHILD_WINDOW = 1442,
        /// <summary>
        /// Invalid GW_* command.
        /// </summary>
        ERROR_INVALID_GW_COMMAND = 1443,
        /// <summary>
        /// Invalid thread identifier.
        /// </summary>
        ERROR_INVALID_THREAD_ID = 1444,
        /// <summary>
        /// Cannot process a message from a window that is not a multiple document interface (MDI) window.
        /// </summary>
        ERROR_NON_MDICHILD_WINDOW = 1445,
        /// <summary>
        /// Popup menu already active.
        /// </summary>
        ERROR_POPUP_ALREADY_ACTIVE = 1446,
        /// <summary>
        /// The window does not have scroll bars.
        /// </summary>
        ERROR_NO_SCROLLBARS = 1447,
        /// <summary>
        /// Scroll bar range cannot be greater than MAXLONG.
        /// </summary>
        ERROR_INVALID_SCROLLBAR_RANGE = 1448,
        /// <summary>
        /// Cannot show or remove the window in the way specified.
        /// </summary>
        ERROR_INVALID_SHOWWIN_COMMAND = 1449,
        /// <summary>
        /// Insufficient system resources exist to complete the requested service.
        /// </summary>
        ERROR_NO_SYSTEM_RESOURCES = 1450,
        /// <summary>
        /// Insufficient system resources exist to complete the requested service.
        /// </summary>
        ERROR_NONPAGED_SYSTEM_RESOURCES = 1451,
        /// <summary>
        /// Insufficient system resources exist to complete the requested service.
        /// </summary>
        ERROR_PAGED_SYSTEM_RESOURCES = 1452,
        /// <summary>
        /// Insufficient quota to complete the requested service.
        /// </summary>
        ERROR_WORKING_SET_QUOTA = 1453,
        /// <summary>
        /// Insufficient quota to complete the requested service.
        /// </summary>
        ERROR_PAGEFILE_QUOTA = 1454,
        /// <summary>
        /// The paging file is too small for this operation to complete.
        /// </summary>
        ERROR_COMMITMENT_LIMIT = 1455,
        /// <summary>
        /// A menu item was not found.
        /// </summary>
        ERROR_MENU_ITEM_NOT_FOUND = 1456,
        /// <summary>
        /// Invalid keyboard layout handle.
        /// </summary>
        ERROR_INVALID_KEYBOARD_HANDLE = 1457,
        /// <summary>
        /// Hook type not allowed.
        /// </summary>
        ERROR_HOOK_TYPE_NOT_ALLOWED = 1458,
        /// <summary>
        /// This operation requires an interactive window station.
        /// </summary>
        ERROR_REQUIRES_INTERACTIVE_WINDOWSTATION = 1459,
        /// <summary>
        /// This operation returned because the timeout period expired.
        /// </summary>
        ERROR_TIMEOUT = 1460,
        /// <summary>
        /// Invalid monitor handle.
        /// </summary>
        ERROR_INVALID_MONITOR_HANDLE = 1461,
        /// <summary>
        /// The event log file is corrupted.
        /// </summary>
        ERROR_EVENTLOG_FILE_CORRUPT = 1500,
        /// <summary>
        /// No event log file could be opened, so the event logging service did not start.
        /// </summary>
        ERROR_EVENTLOG_CANT_START = 1501,
        /// <summary>
        /// The event log file is full.
        /// </summary>
        ERROR_LOG_FILE_FULL = 1502,
        /// <summary>
        /// The event log file has changed between read operations.
        /// </summary>
        ERROR_EVENTLOG_FILE_CHANGED = 1503,
        /// <summary>
        /// The Windows Installer Service could not be accessed. This can occur if you are running Windows in safe mode, or if the Windows Installer is not correctly installed. Contact your support personnel for assistance.
        /// </summary>
        ERROR_INSTALL_SERVICE_FAILURE = 1601,
        /// <summary>
        /// User cancelled installation.
        /// </summary>
        ERROR_INSTALL_USEREXIT = 1602,
        /// <summary>
        /// Fatal error during installation.
        /// </summary>
        ERROR_INSTALL_FAILURE = 1603,
        /// <summary>
        /// Installation suspended, incomplete.
        /// </summary>
        ERROR_INSTALL_SUSPEND = 1604,
        /// <summary>
        /// This action is only valid for products that are currently installed.
        /// </summary>
        ERROR_UNKNOWN_PRODUCT = 1605,
        /// <summary>
        /// Feature ID not registered.
        /// </summary>
        ERROR_UNKNOWN_FEATURE = 1606,
        /// <summary>
        /// Component ID not registered.
        /// </summary>
        ERROR_UNKNOWN_COMPONENT = 1607,
        /// <summary>
        /// Unknown property.
        /// </summary>
        ERROR_UNKNOWN_PROPERTY = 1608,
        /// <summary>
        /// Handle is in an invalid state.
        /// </summary>
        ERROR_INVALID_HANDLE_STATE = 1609,
        /// <summary>
        /// The configuration data for this product is corrupt.  Contact your support personnel.
        /// </summary>
        ERROR_BAD_CONFIGURATION = 1610,
        /// <summary>
        /// Component qualifier not present.
        /// </summary>
        ERROR_INDEX_ABSENT = 1611,
        /// <summary>
        /// The installation source for this product is not available.  Verify that the source exists and that you can access it.
        /// </summary>
        ERROR_INSTALL_SOURCE_ABSENT = 1612,
        /// <summary>
        /// This installation package cannot be installed by the Windows Installer service.  You must install a Windows service pack that contains a newer version of the Windows Installer service.
        /// </summary>
        ERROR_INSTALL_PACKAGE_VERSION = 1613,
        /// <summary>
        /// Product is uninstalled.
        /// </summary>
        ERROR_PRODUCT_UNINSTALLED = 1614,
        /// <summary>
        /// SQL query syntax invalid or unsupported.
        /// </summary>
        ERROR_BAD_QUERY_SYNTAX = 1615,
        /// <summary>
        /// Record field does not exist.
        /// </summary>
        ERROR_INVALID_FIELD = 1616,
        /// <summary>
        /// The device has been removed.
        /// </summary>
        ERROR_DEVICE_REMOVED = 1617,
        /// <summary>
        /// Another installation is already in progress.  Complete that installation before proceeding with this install.
        /// </summary>
        ERROR_INSTALL_ALREADY_RUNNING = 1618,
        /// <summary>
        /// This installation package could not be opened.  Verify that the package exists and that you can access it, or contact the application vendor to verify that this is a valid Windows Installer package.
        /// </summary>
        ERROR_INSTALL_PACKAGE_OPEN_FAILED = 1619,
        /// <summary>
        /// This installation package could not be opened.  Contact the application vendor to verify that this is a valid Windows Installer package.
        /// </summary>
        ERROR_INSTALL_PACKAGE_INVALID = 1620,
        /// <summary>
        /// There was an error starting the Windows Installer service user interface.  Contact your support personnel.
        /// </summary>
        ERROR_INSTALL_UI_FAILURE = 1621,
        /// <summary>
        /// Error opening installation log file. Verify that the specified log file location exists and that you can write to it.
        /// </summary>
        ERROR_INSTALL_LOG_FAILURE = 1622,
        /// <summary>
        /// The language of this installation package is not supported by your system.
        /// </summary>
        ERROR_INSTALL_LANGUAGE_UNSUPPORTED = 1623,
        /// <summary>
        /// Error applying transforms.  Verify that the specified transform paths are valid.
        /// </summary>
        ERROR_INSTALL_TRANSFORM_FAILURE = 1624,
        /// <summary>
        /// This installation is forbidden by system policy.  Contact your system administrator.
        /// </summary>
        ERROR_INSTALL_PACKAGE_REJECTED = 1625,
        /// <summary>
        /// Function could not be executed.
        /// </summary>
        ERROR_FUNCTION_NOT_CALLED = 1626,
        /// <summary>
        /// Function failed during execution.
        /// </summary>
        ERROR_FUNCTION_FAILED = 1627,
        /// <summary>
        /// Invalid or unknown table specified.
        /// </summary>
        ERROR_INVALID_TABLE = 1628,
        /// <summary>
        /// Data supplied is of wrong type.
        /// </summary>
        ERROR_DATATYPE_MISMATCH = 1629,
        /// <summary>
        /// Data of this type is not supported.
        /// </summary>
        ERROR_UNSUPPORTED_TYPE = 1630,
        /// <summary>
        /// The Windows Installer service failed to start.  Contact your support personnel.
        /// </summary>
        ERROR_CREATE_FAILED = 1631,
        /// <summary>
        /// The Temp folder is on a drive that is full or is inaccessible. Free up space on the drive or verify that you have write permission on the Temp folder.
        /// </summary>
        ERROR_INSTALL_TEMP_UNWRITABLE = 1632,
        /// <summary>
        /// This installation package is not supported by this processor type. Contact your product vendor.
        /// </summary>
        ERROR_INSTALL_PLATFORM_UNSUPPORTED = 1633,
        /// <summary>
        /// Component not used on this computer.
        /// </summary>
        ERROR_INSTALL_NOTUSED = 1634,
        /// <summary>
        /// This patch package could not be opened.  Verify that the patch package exists and that you can access it, or contact the application vendor to verify that this is a valid Windows Installer patch package.
        /// </summary>
        ERROR_PATCH_PACKAGE_OPEN_FAILED = 1635,
        /// <summary>
        /// This patch package could not be opened.  Contact the application vendor to verify that this is a valid Windows Installer patch package.
        /// </summary>
        ERROR_PATCH_PACKAGE_INVALID = 1636,
        /// <summary>
        /// This patch package cannot be processed by the Windows Installer service.  You must install a Windows service pack that contains a newer version of the Windows Installer service.
        /// </summary>
        ERROR_PATCH_PACKAGE_UNSUPPORTED = 1637,
        /// <summary>
        /// Another version of this product is already installed.  Installation of this version cannot continue.  To configure or remove the existing version of this product, use Add/Remove Programs on the Control Panel.
        /// </summary>
        ERROR_PRODUCT_VERSION = 1638,
        /// <summary>
        /// Invalid command line argument.  Consult the Windows Installer SDK for detailed command line help.
        /// </summary>
        ERROR_INVALID_COMMAND_LINE = 1639,
        /// <summary>
        /// Only administrators have permission to add, remove, or configure server software during a Terminal services remote session. If you want to install or configure software on the server, contact your network administrator.
        /// </summary>
        ERROR_INSTALL_REMOTE_DISALLOWED = 1640,
        /// <summary>
        /// The requested operation completed successfully.  The system will be restarted so the changes can take effect.
        /// </summary>
        ERROR_SUCCESS_REBOOT_INITIATED = 1641,
        /// <summary>
        /// The upgrade patch cannot be installed by the Windows Installer service because the program to be upgraded may be missing, or the upgrade patch may update a different version of the program. Verify that the program to be upgraded exists on your computer an
        /// d that you have the correct upgrade patch.
        /// </summary>
        ERROR_PATCH_TARGET_NOT_FOUND = 1642,
        /// <summary>
        /// The patch package is not permitted by software restriction policy.
        /// </summary>
        ERROR_PATCH_PACKAGE_REJECTED = 1643,
        /// <summary>
        /// One or more customizations are not permitted by software restriction policy.
        /// </summary>
        ERROR_INSTALL_TRANSFORM_REJECTED = 1644,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_INSTALL_REMOTE_PROHIBITED = 1645,
        /// <summary>
        /// The string binding is invalid.
        /// </summary>
        RPC_S_INVALID_STRING_BINDING = 1700,
        /// <summary>
        /// The binding handle is not the correct type.
        /// </summary>
        RPC_S_WRONG_KIND_OF_BINDING = 1701,
        /// <summary>
        /// The binding handle is invalid.
        /// </summary>
        RPC_S_INVALID_BINDING = 1702,
        /// <summary>
        /// The RPC protocol sequence is not supported.
        /// </summary>
        RPC_S_PROTSEQ_NOT_SUPPORTED = 1703,
        /// <summary>
        /// The RPC protocol sequence is invalid.
        /// </summary>
        RPC_S_INVALID_RPC_PROTSEQ = 1704,
        /// <summary>
        /// The string universal unique identifier (UUID) is invalid.
        /// </summary>
        RPC_S_INVALID_STRING_UUID = 1705,
        /// <summary>
        /// The endpoint format is invalid.
        /// </summary>
        RPC_S_INVALID_ENDPOINT_FORMAT = 1706,
        /// <summary>
        /// The network address is invalid.
        /// </summary>
        RPC_S_INVALID_NET_ADDR = 1707,
        /// <summary>
        /// No endpoint was found.
        /// </summary>
        RPC_S_NO_ENDPOINT_FOUND = 1708,
        /// <summary>
        /// The timeout value is invalid.
        /// </summary>
        RPC_S_INVALID_TIMEOUT = 1709,
        /// <summary>
        /// The object universal unique identifier (UUID) was not found.
        /// </summary>
        RPC_S_OBJECT_NOT_FOUND = 1710,
        /// <summary>
        /// The object universal unique identifier (UUID) has already been registered.
        /// </summary>
        RPC_S_ALREADY_REGISTERED = 1711,
        /// <summary>
        /// The type universal unique identifier (UUID) has already been registered.
        /// </summary>
        RPC_S_TYPE_ALREADY_REGISTERED = 1712,
        /// <summary>
        /// The RPC server is already listening.
        /// </summary>
        RPC_S_ALREADY_LISTENING = 1713,
        /// <summary>
        /// No protocol sequences have been registered.
        /// </summary>
        RPC_S_NO_PROTSEQS_REGISTERED = 1714,
        /// <summary>
        /// The RPC server is not listening.
        /// </summary>
        RPC_S_NOT_LISTENING = 1715,
        /// <summary>
        /// The manager type is unknown.
        /// </summary>
        RPC_S_UNKNOWN_MGR_TYPE = 1716,
        /// <summary>
        /// The interface is unknown.
        /// </summary>
        RPC_S_UNKNOWN_IF = 1717,
        /// <summary>
        /// There are no bindings.
        /// </summary>
        RPC_S_NO_BINDINGS = 1718,
        /// <summary>
        /// There are no protocol sequences.
        /// </summary>
        RPC_S_NO_PROTSEQS = 1719,
        /// <summary>
        /// The endpoint cannot be created.
        /// </summary>
        RPC_S_CANT_CREATE_ENDPOINT = 1720,
        /// <summary>
        /// Not enough resources are available to complete this operation.
        /// </summary>
        RPC_S_OUT_OF_RESOURCES = 1721,
        /// <summary>
        /// The RPC server is unavailable.
        /// </summary>
        RPC_S_SERVER_UNAVAILABLE = 1722,
        /// <summary>
        /// The RPC server is too busy to complete this operation.
        /// </summary>
        RPC_S_SERVER_TOO_BUSY = 1723,
        /// <summary>
        /// The network options are invalid.
        /// </summary>
        RPC_S_INVALID_NETWORK_OPTIONS = 1724,
        /// <summary>
        /// There are no remote procedure calls active on this thread.
        /// </summary>
        RPC_S_NO_CALL_ACTIVE = 1725,
        /// <summary>
        /// The remote procedure call failed.
        /// </summary>
        RPC_S_CALL_FAILED = 1726,
        /// <summary>
        /// The remote procedure call failed and did not execute.
        /// </summary>
        RPC_S_CALL_FAILED_DNE = 1727,
        /// <summary>
        /// A remote procedure call (RPC) protocol error occurred.
        /// </summary>
        RPC_S_PROTOCOL_ERROR = 1728,
        /// <summary>
        /// The transfer syntax is not supported by the RPC server.
        /// </summary>
        RPC_S_UNSUPPORTED_TRANS_SYN = 1730,
        /// <summary>
        /// The universal unique identifier (UUID) type is not supported.
        /// </summary>
        RPC_S_UNSUPPORTED_TYPE = 1732,
        /// <summary>
        /// The tag is invalid.
        /// </summary>
        RPC_S_INVALID_TAG = 1733,
        /// <summary>
        /// The array bounds are invalid.
        /// </summary>
        RPC_S_INVALID_BOUND = 1734,
        /// <summary>
        /// The binding does not contain an entry name.
        /// </summary>
        RPC_S_NO_ENTRY_NAME = 1735,
        /// <summary>
        /// The name syntax is invalid.
        /// </summary>
        RPC_S_INVALID_NAME_SYNTAX = 1736,
        /// <summary>
        /// The name syntax is not supported.
        /// </summary>
        RPC_S_UNSUPPORTED_NAME_SYNTAX = 1737,
        /// <summary>
        /// No network address is available to use to construct a universal unique identifier (UUID).
        /// </summary>
        RPC_S_UUID_NO_ADDRESS = 1739,
        /// <summary>
        /// The endpoint is a duplicate.
        /// </summary>
        RPC_S_DUPLICATE_ENDPOINT = 1740,
        /// <summary>
        /// The authentication type is unknown.
        /// </summary>
        RPC_S_UNKNOWN_AUTHN_TYPE = 1741,
        /// <summary>
        /// The maximum number of calls is too small.
        /// </summary>
        RPC_S_MAX_CALLS_TOO_SMALL = 1742,
        /// <summary>
        /// The string is too long.
        /// </summary>
        RPC_S_STRING_TOO_Int32 = 1743,
        /// <summary>
        /// The RPC protocol sequence was not found.
        /// </summary>
        RPC_S_PROTSEQ_NOT_FOUND = 1744,
        /// <summary>
        /// The procedure number is out of range.
        /// </summary>
        RPC_S_PROCNUM_OUT_OF_RANGE = 1745,
        /// <summary>
        /// The binding does not contain any authentication information.
        /// </summary>
        RPC_S_BINDING_HAS_NO_AUTH = 1746,
        /// <summary>
        /// The authentication service is unknown.
        /// </summary>
        RPC_S_UNKNOWN_AUTHN_SERVICE = 1747,
        /// <summary>
        /// The authentication level is unknown.
        /// </summary>
        RPC_S_UNKNOWN_AUTHN_LEVEL = 1748,
        /// <summary>
        /// The security context is invalid.
        /// </summary>
        RPC_S_INVALID_AUTH_IDENTITY = 1749,
        /// <summary>
        /// The authorization service is unknown.
        /// </summary>
        RPC_S_UNKNOWN_AUTHZ_SERVICE = 1750,
        /// <summary>
        /// The entry is invalid.
        /// </summary>
        EPT_S_INVALID_ENTRY = 1751,
        /// <summary>
        /// The server endpoint cannot perform the operation.
        /// </summary>
        EPT_S_CANT_PERFORM_OP = 1752,
        /// <summary>
        /// There are no more endpoints available from the endpoint mapper.
        /// </summary>
        EPT_S_NOT_REGISTERED = 1753,
        /// <summary>
        /// No interfaces have been exported.
        /// </summary>
        RPC_S_NOTHING_TO_EXPORT = 1754,
        /// <summary>
        /// The entry name is incomplete.
        /// </summary>
        RPC_S_INCOMPLETE_NAME = 1755,
        /// <summary>
        /// The version option is invalid.
        /// </summary>
        RPC_S_INVALID_VERS_OPTION = 1756,
        /// <summary>
        /// There are no more members.
        /// </summary>
        RPC_S_NO_MORE_MEMBERS = 1757,
        /// <summary>
        /// There is nothing to unexport.
        /// </summary>
        RPC_S_NOT_ALL_OBJS_UNEXPORTED = 1758,
        /// <summary>
        /// The interface was not found.
        /// </summary>
        RPC_S_INTERFACE_NOT_FOUND = 1759,
        /// <summary>
        /// The entry already exists.
        /// </summary>
        RPC_S_ENTRY_ALREADY_EXISTS = 1760,
        /// <summary>
        /// The entry is not found.
        /// </summary>
        RPC_S_ENTRY_NOT_FOUND = 1761,
        /// <summary>
        /// The name service is unavailable.
        /// </summary>
        RPC_S_NAME_SERVICE_UNAVAILABLE = 1762,
        /// <summary>
        /// The network address family is invalid.
        /// </summary>
        RPC_S_INVALID_NAF_ID = 1763,
        /// <summary>
        /// The requested operation is not supported.
        /// </summary>
        RPC_S_CANNOT_SUPPORT = 1764,
        /// <summary>
        /// No security context is available to allow impersonation.
        /// </summary>
        RPC_S_NO_CONTEXT_AVAILABLE = 1765,
        /// <summary>
        /// An internal error occurred in a remote procedure call (RPC).
        /// </summary>
        RPC_S_INTERNAL_ERROR = 1766,
        /// <summary>
        /// The RPC server attempted an integer division by zero.
        /// </summary>
        RPC_S_ZERO_DIVIDE = 1767,
        /// <summary>
        /// An addressing error occurred in the RPC server.
        /// </summary>
        RPC_S_ADDRESS_ERROR = 1768,
        /// <summary>
        /// A floating-point operation at the RPC server caused a division by zero.
        /// </summary>
        RPC_S_FP_DIV_ZERO = 1769,
        /// <summary>
        /// A floating-point underflow occurred at the RPC server.
        /// </summary>
        RPC_S_FP_UNDERFLOW = 1770,
        /// <summary>
        /// A floating-point overflow occurred at the RPC server.
        /// </summary>
        RPC_S_FP_OVERFLOW = 1771,
        /// <summary>
        /// The list of RPC servers available for the binding of auto handles has been exhausted.
        /// </summary>
        RPC_X_NO_MORE_ENTRIES = 1772,
        /// <summary>
        /// Unable to open the character translation table file.
        /// </summary>
        RPC_X_SS_CHAR_TRANS_OPEN_FAIL = 1773,
        /// <summary>
        /// The file containing the character translation table has fewer than 512 bytes.
        /// </summary>
        RPC_X_SS_CHAR_TRANS_Int16_FILE = 1774,
        /// <summary>
        /// A null context handle was passed from the client to the host during a remote procedure call.
        /// </summary>
        RPC_X_SS_IN_NULL_CONTEXT = 1775,
        /// <summary>
        /// The context handle changed during a remote procedure call.
        /// </summary>
        RPC_X_SS_CONTEXT_DAMAGED = 1777,
        /// <summary>
        /// The binding handles passed to a remote procedure call do not match.
        /// </summary>
        RPC_X_SS_HANDLES_MISMATCH = 1778,
        /// <summary>
        /// The stub is unable to get the remote procedure call handle.
        /// </summary>
        RPC_X_SS_CANNOT_GET_CALL_HANDLE = 1779,
        /// <summary>
        /// A null reference pointer was passed to the stub.
        /// </summary>
        RPC_X_NULL_REF_POINTER = 1780,
        /// <summary>
        /// The enumeration value is out of range.
        /// </summary>
        RPC_X_ENUM_VALUE_OUT_OF_RANGE = 1781,
        /// <summary>
        /// The byte count is too small.
        /// </summary>
        RPC_X_BYTE_COUNT_TOO_SMALL = 1782,
        /// <summary>
        /// The stub received bad data.
        /// </summary>
        RPC_X_BAD_STUB_DATA = 1783,
        /// <summary>
        /// The supplied user buffer is not valid for the requested operation.
        /// </summary>
        ERROR_INVALID_USER_BUFFER = 1784,
        /// <summary>
        /// The disk media is not recognized. It may not be formatted.
        /// </summary>
        ERROR_UNRECOGNIZED_MEDIA = 1785,
        /// <summary>
        /// The workstation does not have a trust secret.
        /// </summary>
        ERROR_NO_TRUST_LSA_SECRET = 1786,
        /// <summary>
        /// The security database on the server does not have a computer account for this workstation trust relationship.
        /// </summary>
        ERROR_NO_TRUST_SAM_ACCOUNT = 1787,
        /// <summary>
        /// The trust relationship between the primary domain and the trusted domain failed.
        /// </summary>
        ERROR_TRUSTED_DOMAIN_FAILURE = 1788,
        /// <summary>
        /// The trust relationship between this workstation and the primary domain failed.
        /// </summary>
        ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789,
        /// <summary>
        /// The network logon failed.
        /// </summary>
        ERROR_TRUST_FAILURE = 1790,
        /// <summary>
        /// A remote procedure call is already in progress for this thread.
        /// </summary>
        RPC_S_CALL_IN_PROGRESS = 1791,
        /// <summary>
        /// An attempt was made to logon, but the network logon service was not started.
        /// </summary>
        ERROR_NETLOGON_NOT_STARTED = 1792,
        /// <summary>
        /// The user's account has expired.
        /// </summary>
        ERROR_ACCOUNT_EXPIRED = 1793,
        /// <summary>
        /// The redirector is in use and cannot be unloaded.
        /// </summary>
        ERROR_REDIRECTOR_HAS_OPEN_HANDLES = 1794,
        /// <summary>
        /// The specified printer driver is already installed.
        /// </summary>
        ERROR_PRINTER_DRIVER_ALREADY_INSTALLED = 1795,
        /// <summary>
        /// The specified port is unknown.
        /// </summary>
        ERROR_UNKNOWN_PORT = 1796,
        /// <summary>
        /// The printer driver is unknown.
        /// </summary>
        ERROR_UNKNOWN_PRINTER_DRIVER = 1797,
        /// <summary>
        /// The print processor is unknown.
        /// </summary>
        ERROR_UNKNOWN_PRINTPROCESSOR = 1798,
        /// <summary>
        /// The specified separator file is invalid.
        /// </summary>
        ERROR_INVALID_SEPARATOR_FILE = 1799,
        /// <summary>
        /// The specified priority is invalid.
        /// </summary>
        ERROR_INVALID_PRIORITY = 1800,
        /// <summary>
        /// The printer name is invalid.
        /// </summary>
        ERROR_INVALID_PRINTER_NAME = 1801,
        /// <summary>
        /// The printer already exists.
        /// </summary>
        ERROR_PRINTER_ALREADY_EXISTS = 1802,
        /// <summary>
        /// The printer command is invalid.
        /// </summary>
        ERROR_INVALID_PRINTER_COMMAND = 1803,
        /// <summary>
        /// The specified datatype is invalid.
        /// </summary>
        ERROR_INVALID_DATATYPE = 1804,
        /// <summary>
        /// The environment specified is invalid.
        /// </summary>
        ERROR_INVALID_ENVIRONMENT = 1805,
        /// <summary>
        /// There are no more bindings.
        /// </summary>
        RPC_S_NO_MORE_BINDINGS = 1806,
        /// <summary>
        /// The account used is an interdomain trust account. Use your global user account or local user account to access this server.
        /// </summary>
        ERROR_NOLOGON_INTERDOMAIN_TRUST_ACCOUNT = 1807,
        /// <summary>
        /// The account used is a computer account. Use your global user account or local user account to access this server.
        /// </summary>
        ERROR_NOLOGON_WORKSTATION_TRUST_ACCOUNT = 1808,
        /// <summary>
        /// The account used is a server trust account. Use your global user account or local user account to access this server.
        /// </summary>
        ERROR_NOLOGON_SERVER_TRUST_ACCOUNT = 1809,
        /// <summary>
        /// The name or security ID (SID) of the domain specified is inconsistent with the trust information for that domain.
        /// </summary>
        ERROR_DOMAIN_TRUST_INCONSISTENT = 1810,
        /// <summary>
        /// The server is in use and cannot be unloaded.
        /// </summary>
        ERROR_SERVER_HAS_OPEN_HANDLES = 1811,
        /// <summary>
        /// The specified image file did not contain a resource section.
        /// </summary>
        ERROR_RESOURCE_DATA_NOT_FOUND = 1812,
        /// <summary>
        /// The specified resource type cannot be found in the image file.
        /// </summary>
        ERROR_RESOURCE_TYPE_NOT_FOUND = 1813,
        /// <summary>
        /// The specified resource name cannot be found in the image file.
        /// </summary>
        ERROR_RESOURCE_NAME_NOT_FOUND = 1814,
        /// <summary>
        /// The specified resource language ID cannot be found in the image file.
        /// </summary>
        ERROR_RESOURCE_LANG_NOT_FOUND = 1815,
        /// <summary>
        /// Not enough quota is available to process this command.
        /// </summary>
        ERROR_NOT_ENOUGH_QUOTA = 1816,
        /// <summary>
        /// No interfaces have been registered.
        /// </summary>
        RPC_S_NO_INTERFACES = 1817,
        /// <summary>
        /// The remote procedure call was cancelled.
        /// </summary>
        RPC_S_CALL_CANCELLED = 1818,
        /// <summary>
        /// The binding handle does not contain all required information.
        /// </summary>
        RPC_S_BINDING_INCOMPLETE = 1819,
        /// <summary>
        /// A communications failure occurred during a remote procedure call.
        /// </summary>
        RPC_S_COMM_FAILURE = 1820,
        /// <summary>
        /// The requested authentication level is not supported.
        /// </summary>
        RPC_S_UNSUPPORTED_AUTHN_LEVEL = 1821,
        /// <summary>
        /// No principal name registered.
        /// </summary>
        RPC_S_NO_PRINC_NAME = 1822,
        /// <summary>
        /// The error specified is not a valid Windows RPC error code.
        /// </summary>
        RPC_S_NOT_RPC_ERROR = 1823,
        /// <summary>
        /// A UUID that is valid only on this computer has been allocated.
        /// </summary>
        RPC_S_UUID_LOCAL_ONLY = 1824,
        /// <summary>
        /// A security package specific error occurred.
        /// </summary>
        RPC_S_SEC_PKG_ERROR = 1825,
        /// <summary>
        /// Thread is not canceled.
        /// </summary>
        RPC_S_NOT_CANCELLED = 1826,
        /// <summary>
        /// Invalid operation on the encoding/decoding handle.
        /// </summary>
        RPC_X_INVALID_ES_ACTION = 1827,
        /// <summary>
        /// Incompatible version of the serializing package.
        /// </summary>
        RPC_X_WRONG_ES_VERSION = 1828,
        /// <summary>
        /// Incompatible version of the RPC stub.
        /// </summary>
        RPC_X_WRONG_STUB_VERSION = 1829,
        /// <summary>
        /// The RPC pipe object is invalid or corrupted.
        /// </summary>
        RPC_X_INVALID_PIPE_OBJECT = 1830,
        /// <summary>
        /// An invalid operation was attempted on an RPC pipe object.
        /// </summary>
        RPC_X_WRONG_PIPE_ORDER = 1831,
        /// <summary>
        /// Unsupported RPC pipe version.
        /// </summary>
        RPC_X_WRONG_PIPE_VERSION = 1832,
        /// <summary>
        /// The group member was not found.
        /// </summary>
        RPC_S_GROUP_MEMBER_NOT_FOUND = 1898,
        /// <summary>
        /// The endpoint mapper database entry could not be created.
        /// </summary>
        EPT_S_CANT_CREATE = 1899,
        /// <summary>
        /// The object universal unique identifier (UUID) is the nil UUID.
        /// </summary>
        RPC_S_INVALID_OBJECT = 1900,
        /// <summary>
        /// The specified time is invalid.
        /// </summary>
        ERROR_INVALID_TIME = 1901,
        /// <summary>
        /// The specified form name is invalid.
        /// </summary>
        ERROR_INVALID_FORM_NAME = 1902,
        /// <summary>
        /// The specified form size is invalid.
        /// </summary>
        ERROR_INVALID_FORM_SIZE = 1903,
        /// <summary>
        /// The specified printer handle is already being waited on
        /// </summary>
        ERROR_ALREADY_WAITING = 1904,
        /// <summary>
        /// The specified printer has been deleted.
        /// </summary>
        ERROR_PRINTER_DELETED = 1905,
        /// <summary>
        /// The state of the printer is invalid.
        /// </summary>
        ERROR_INVALID_PRINTER_STATE = 1906,
        /// <summary>
        /// The user's password must be changed before logging on the first time.
        /// </summary>
        ERROR_PASSUInt16_MUST_CHANGE = 1907,
        /// <summary>
        /// Could not find the domain controller for this domain.
        /// </summary>
        ERROR_DOMAIN_CONTROLLER_NOT_FOUND = 1908,
        /// <summary>
        /// The referenced account is currently locked out and may not be logged on to.
        /// </summary>
        ERROR_ACCOUNT_LOCKED_OUT = 1909,
        /// <summary>
        /// The object exporter specified was not found.
        /// </summary>
        OR_INVALID_OXID = 1910,
        /// <summary>
        /// The object specified was not found.
        /// </summary>
        OR_INVALID_OID = 1911,
        /// <summary>
        /// The object resolver set specified was not found.
        /// </summary>
        OR_INVALID_SET = 1912,
        /// <summary>
        /// Some data remains to be sent in the request buffer.
        /// </summary>
        RPC_S_SEND_INCOMPLETE = 1913,
        /// <summary>
        /// Invalid asynchronous remote procedure call handle.
        /// </summary>
        RPC_S_INVALID_ASYNC_HANDLE = 1914,
        /// <summary>
        /// Invalid asynchronous RPC call handle for this operation.
        /// </summary>
        RPC_S_INVALID_ASYNC_CALL = 1915,
        /// <summary>
        /// The RPC pipe object has already been closed.
        /// </summary>
        RPC_X_PIPE_CLOSED = 1916,
        /// <summary>
        /// The RPC call completed before all pipes were processed.
        /// </summary>
        RPC_X_PIPE_DISCIPLINE_ERROR = 1917,
        /// <summary>
        /// No more data is available from the RPC pipe.
        /// </summary>
        RPC_X_PIPE_EMPTY = 1918,
        /// <summary>
        /// No site name is available for this machine.
        /// </summary>
        ERROR_NO_SITENAME = 1919,
        /// <summary>
        /// The file can not be accessed by the system.
        /// </summary>
        ERROR_CANT_ACCESS_FILE = 1920,
        /// <summary>
        /// The name of the file cannot be resolved by the system.
        /// </summary>
        ERROR_CANT_RESOLVE_FILENAME = 1921,
        /// <summary>
        /// The entry is not of the expected type.
        /// </summary>
        RPC_S_ENTRY_TYPE_MISMATCH = 1922,
        /// <summary>
        /// Not all object UUIDs could be exported to the specified entry.
        /// </summary>
        RPC_S_NOT_ALL_OBJS_EXPORTED = 1923,
        /// <summary>
        /// Interface could not be exported to the specified entry.
        /// </summary>
        RPC_S_INTERFACE_NOT_EXPORTED = 1924,
        /// <summary>
        /// The specified profile entry could not be added.
        /// </summary>
        RPC_S_PROFILE_NOT_ADDED = 1925,
        /// <summary>
        /// The specified profile element could not be added.
        /// </summary>
        RPC_S_PRF_ELT_NOT_ADDED = 1926,
        /// <summary>
        /// The specified profile element could not be removed.
        /// </summary>
        RPC_S_PRF_ELT_NOT_REMOVED = 1927,
        /// <summary>
        /// The group element could not be added.
        /// </summary>
        RPC_S_GRP_ELT_NOT_ADDED = 1928,
        /// <summary>
        /// The group element could not be removed.
        /// </summary>
        RPC_S_GRP_ELT_NOT_REMOVED = 1929,
        /// <summary>
        /// The printer driver is not compatible with a policy enabled on your computer that blocks NT 4.0 drivers.
        /// </summary>
        ERROR_KM_DRIVER_BLOCKED = 1930,
        /// <summary>
        /// The context has expired and can no longer be used.
        /// </summary>
        ERROR_CONTEXT_EXPIRED = 1931,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_PER_USER_TRUST_QUOTA_EXCEEDED = 1932,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_ALL_USER_TRUST_QUOTA_EXCEEDED = 1933,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_USER_DELETE_TRUST_QUOTA_EXCEEDED = 1934,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_AUTHENTICATION_FIREWALL_FAILED = 1935,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_REMOTE_PRINT_CONNECTIONS_BLOCKED = 1936,
        /// <summary>
        /// The pixel format is invalid.
        /// </summary>
        ERROR_INVALID_PIXEL_FORMAT = 2000,
        /// <summary>
        /// The specified driver is invalid.
        /// </summary>
        ERROR_BAD_DRIVER = 2001,
        /// <summary>
        /// The window style or class attribute is invalid for this operation.
        /// </summary>
        ERROR_INVALID_WINDOW_STYLE = 2002,
        /// <summary>
        /// The requested metafile operation is not supported.
        /// </summary>
        ERROR_METAFILE_NOT_SUPPORTED = 2003,
        /// <summary>
        /// The requested transformation operation is not supported.
        /// </summary>
        ERROR_TRANSFORM_NOT_SUPPORTED = 2004,
        /// <summary>
        /// The requested clipping operation is not supported.
        /// </summary>
        ERROR_CLIPPING_NOT_SUPPORTED = 2005,
        /// <summary>
        /// The specified color management module is invalid.
        /// </summary>
        ERROR_INVALID_CMM = 2010,
        /// <summary>
        /// The specified color profile is invalid.
        /// </summary>
        ERROR_INVALID_PROFILE = 2011,
        /// <summary>
        /// The specified tag was not found.
        /// </summary>
        ERROR_TAG_NOT_FOUND = 2012,
        /// <summary>
        /// A required tag is not present.
        /// </summary>
        ERROR_TAG_NOT_PRESENT = 2013,
        /// <summary>
        /// The specified tag is already present.
        /// </summary>
        ERROR_DUPLICATE_TAG = 2014,
        /// <summary>
        /// The specified color profile is not associated with any device.
        /// </summary>
        ERROR_PROFILE_NOT_ASSOCIATED_WITH_DEVICE = 2015,
        /// <summary>
        /// The specified color profile was not found.
        /// </summary>
        ERROR_PROFILE_NOT_FOUND = 2016,
        /// <summary>
        /// The specified color space is invalid.
        /// </summary>
        ERROR_INVALID_COLORSPACE = 2017,
        /// <summary>
        /// Image Color Management is not enabled.
        /// </summary>
        ERROR_ICM_NOT_ENABLED = 2018,
        /// <summary>
        /// There was an error while deleting the color transform.
        /// </summary>
        ERROR_DELETING_ICM_XFORM = 2019,
        /// <summary>
        /// The specified color transform is invalid.
        /// </summary>
        ERROR_INVALID_TRANSFORM = 2020,
        /// <summary>
        /// The specified transform does not match the bitmap's color space.
        /// </summary>
        ERROR_COLORSPACE_MISMATCH = 2021,
        /// <summary>
        /// The specified named color index is not present in the profile.
        /// </summary>
        ERROR_INVALID_COLORINDEX = 2022,
        /// <summary>
        /// The network connection was made successfully, but the user had to be prompted for a password other than the one originally specified.
        /// </summary>
        ERROR_CONNECTED_OTHER_PASSUInt16 = 2108,
        /// <summary>
        /// The network connection was made successfully using default credentials.
        /// </summary>
        ERROR_CONNECTED_OTHER_PASSUInt16_DEFAULT = 2109,
        /// <summary>
        /// The specified username is invalid.
        /// </summary>
        ERROR_BAD_USERNAME = 2202,
        /// <summary>
        /// This network connection does not exist.
        /// </summary>
        ERROR_NOT_CONNECTED = 2250,
        /// <summary>
        /// This network connection has files open or requests pending.
        /// </summary>
        ERROR_OPEN_FILES = 2401,
        /// <summary>
        /// Active connections still exist.
        /// </summary>
        ERROR_ACTIVE_CONNECTIONS = 2402,
        /// <summary>
        /// The device is in use by an active process and cannot be disconnected.
        /// </summary>
        ERROR_DEVICE_IN_USE = 2404,
        /// <summary>
        /// The specified print monitor is unknown.
        /// </summary>
        ERROR_UNKNOWN_PRINT_MONITOR = 3000,
        /// <summary>
        /// The specified printer driver is currently in use.
        /// </summary>
        ERROR_PRINTER_DRIVER_IN_USE = 3001,
        /// <summary>
        /// The spool file was not found.
        /// </summary>
        ERROR_SPOOL_FILE_NOT_FOUND = 3002,
        /// <summary>
        /// A StartDocPrinter call was not issued.
        /// </summary>
        ERROR_SPL_NO_STARTDOC = 3003,
        /// <summary>
        /// An AddJob call was not issued.
        /// </summary>
        ERROR_SPL_NO_ADDJOB = 3004,
        /// <summary>
        /// The specified print processor has already been installed.
        /// </summary>
        ERROR_PRINT_PROCESSOR_ALREADY_INSTALLED = 3005,
        /// <summary>
        /// The specified print monitor has already been installed.
        /// </summary>
        ERROR_PRINT_MONITOR_ALREADY_INSTALLED = 3006,
        /// <summary>
        /// The specified print monitor does not have the required functions.
        /// </summary>
        ERROR_INVALID_PRINT_MONITOR = 3007,
        /// <summary>
        /// The specified print monitor is currently in use.
        /// </summary>
        ERROR_PRINT_MONITOR_IN_USE = 3008,
        /// <summary>
        /// The requested operation is not allowed when there are jobs queued to the printer.
        /// </summary>
        ERROR_PRINTER_HAS_JOBS_QUEUED = 3009,
        /// <summary>
        /// The requested operation is successful. Changes will not be effective until the system is rebooted.
        /// </summary>
        ERROR_SUCCESS_REBOOT_REQUIRED = 3010,
        /// <summary>
        /// The requested operation is successful. Changes will not be effective until the service is restarted.
        /// </summary>
        ERROR_SUCCESS_RESTART_REQUIRED = 3011,
        /// <summary>
        /// No printers were found.
        /// </summary>
        ERROR_PRINTER_NOT_FOUND = 3012,
        /// <summary>
        /// The printer driver is known to be unreliable.
        /// </summary>
        ERROR_PRINTER_DRIVER_WARNED = 3013,
        /// <summary>
        /// The printer driver is known to harm the system.
        /// </summary>
        ERROR_PRINTER_DRIVER_BLOCKED = 3014,
        /// <summary>
        /// WINS encountered an error while processing the command.
        /// </summary>
        ERROR_WINS_INTERNAL = 4000,
        /// <summary>
        /// The local WINS can not be deleted.
        /// </summary>
        ERROR_CAN_NOT_DEL_LOCAL_WINS = 4001,
        /// <summary>
        /// The importation from the file failed.
        /// </summary>
        ERROR_STATIC_INIT = 4002,
        /// <summary>
        /// The backup failed. Was a full backup done before?
        /// </summary>
        ERROR_INC_BACKUP = 4003,
        /// <summary>
        /// The backup failed. Check the directory to which you are backing the database.
        /// </summary>
        ERROR_FULL_BACKUP = 4004,
        /// <summary>
        /// The name does not exist in the WINS database.
        /// </summary>
        ERROR_REC_NON_EXISTENT = 4005,
        /// <summary>
        /// Replication with a nonconfigured partner is not allowed.
        /// </summary>
        ERROR_RPL_NOT_ALLOWED = 4006,
        /// <summary>
        /// The DHCP client has obtained an IP address that is already in use on the network. The local interface will be disabled until the DHCP client can obtain a new address.
        /// </summary>
        ERROR_DHCP_ADDRESS_CONFLICT = 4100,
        /// <summary>
        /// The GUID passed was not recognized as valid by a WMI data provider.
        /// </summary>
        ERROR_WMI_GUID_NOT_FOUND = 4200,
        /// <summary>
        /// The instance name passed was not recognized as valid by a WMI data provider.
        /// </summary>
        ERROR_WMI_INSTANCE_NOT_FOUND = 4201,
        /// <summary>
        /// The data item ID passed was not recognized as valid by a WMI data provider.
        /// </summary>
        ERROR_WMI_ITEMID_NOT_FOUND = 4202,
        /// <summary>
        /// The WMI request could not be completed and should be retried.
        /// </summary>
        ERROR_WMI_TRY_AGAIN = 4203,
        /// <summary>
        /// The WMI data provider could not be located.
        /// </summary>
        ERROR_WMI_DP_NOT_FOUND = 4204,
        /// <summary>
        /// The WMI data provider references an instance set that has not been registered.
        /// </summary>
        ERROR_WMI_UNRESOLVED_INSTANCE_REF = 4205,
        /// <summary>
        /// The WMI data block or event notification has already been enabled.
        /// </summary>
        ERROR_WMI_ALREADY_ENABLED = 4206,
        /// <summary>
        /// The WMI data block is no longer available.
        /// </summary>
        ERROR_WMI_GUID_DISCONNECTED = 4207,
        /// <summary>
        /// The WMI data service is not available.
        /// </summary>
        ERROR_WMI_SERVER_UNAVAILABLE = 4208,
        /// <summary>
        /// The WMI data provider failed to carry out the request.
        /// </summary>
        ERROR_WMI_DP_FAILED = 4209,
        /// <summary>
        /// The WMI MOF information is not valid.
        /// </summary>
        ERROR_WMI_INVALID_MOF = 4210,
        /// <summary>
        /// The WMI registration information is not valid.
        /// </summary>
        ERROR_WMI_INVALID_REGINFO = 4211,
        /// <summary>
        /// The WMI data block or event notification has already been disabled.
        /// </summary>
        ERROR_WMI_ALREADY_DISABLED = 4212,
        /// <summary>
        /// The WMI data item or data block is read only.
        /// </summary>
        ERROR_WMI_READ_ONLY = 4213,
        /// <summary>
        /// The WMI data item or data block could not be changed.
        /// </summary>
        ERROR_WMI_SET_FAILURE = 4214,
        /// <summary>
        /// The media identifier does not represent a valid medium.
        /// </summary>
        ERROR_INVALID_MEDIA = 4300,
        /// <summary>
        /// The library identifier does not represent a valid library.
        /// </summary>
        ERROR_INVALID_LIBRARY = 4301,
        /// <summary>
        /// The media pool identifier does not represent a valid media pool.
        /// </summary>
        ERROR_INVALID_MEDIA_POOL = 4302,
        /// <summary>
        /// The drive and medium are not compatible or exist in different libraries.
        /// </summary>
        ERROR_DRIVE_MEDIA_MISMATCH = 4303,
        /// <summary>
        /// The medium currently exists in an offline library and must be online to perform this operation.
        /// </summary>
        ERROR_MEDIA_OFFLINE = 4304,
        /// <summary>
        /// The operation cannot be performed on an offline library.
        /// </summary>
        ERROR_LIBRARY_OFFLINE = 4305,
        /// <summary>
        /// The library, drive, or media pool is empty.
        /// </summary>
        ERROR_EMPTY = 4306,
        /// <summary>
        /// The library, drive, or media pool must be empty to perform this operation.
        /// </summary>
        ERROR_NOT_EMPTY = 4307,
        /// <summary>
        /// No media is currently available in this media pool or library.
        /// </summary>
        ERROR_MEDIA_UNAVAILABLE = 4308,
        /// <summary>
        /// A resource required for this operation is disabled.
        /// </summary>
        ERROR_RESOURCE_DISABLED = 4309,
        /// <summary>
        /// The media identifier does not represent a valid cleaner.
        /// </summary>
        ERROR_INVALID_CLEANER = 4310,
        /// <summary>
        /// The drive cannot be cleaned or does not support cleaning.
        /// </summary>
        ERROR_UNABLE_TO_CLEAN = 4311,
        /// <summary>
        /// The object identifier does not represent a valid object.
        /// </summary>
        ERROR_OBJECT_NOT_FOUND = 4312,
        /// <summary>
        /// Unable to read from or write to the database.
        /// </summary>
        ERROR_DATABASE_FAILURE = 4313,
        /// <summary>
        /// The database is full.
        /// </summary>
        ERROR_DATABASE_FULL = 4314,
        /// <summary>
        /// The medium is not compatible with the device or media pool.
        /// </summary>
        ERROR_MEDIA_INCOMPATIBLE = 4315,
        /// <summary>
        /// The resource required for this operation does not exist.
        /// </summary>
        ERROR_RESOURCE_NOT_PRESENT = 4316,
        /// <summary>
        /// The operation identifier is not valid.
        /// </summary>
        ERROR_INVALID_OPERATION = 4317,
        /// <summary>
        /// The media is not mounted or ready for use.
        /// </summary>
        ERROR_MEDIA_NOT_AVAILABLE = 4318,
        /// <summary>
        /// The device is not ready for use.
        /// </summary>
        ERROR_DEVICE_NOT_AVAILABLE = 4319,
        /// <summary>
        /// The operator or administrator has refused the request.
        /// </summary>
        ERROR_REQUEST_REFUSED = 4320,
        /// <summary>
        /// The drive identifier does not represent a valid drive.
        /// </summary>
        ERROR_INVALID_DRIVE_OBJECT = 4321,
        /// <summary>
        /// Library is full.  No slot is available for use.
        /// </summary>
        ERROR_LIBRARY_FULL = 4322,
        /// <summary>
        /// The transport cannot access the medium.
        /// </summary>
        ERROR_MEDIUM_NOT_ACCESSIBLE = 4323,
        /// <summary>
        /// Unable to load the medium into the drive.
        /// </summary>
        ERROR_UNABLE_TO_LOAD_MEDIUM = 4324,
        /// <summary>
        /// Unable to retrieve the drive status.
        /// </summary>
        ERROR_UNABLE_TO_INVENTORY_DRIVE = 4325,
        /// <summary>
        /// Unable to retrieve the slot status.
        /// </summary>
        ERROR_UNABLE_TO_INVENTORY_SLOT = 4326,
        /// <summary>
        /// Unable to retrieve status about the transport.
        /// </summary>
        ERROR_UNABLE_TO_INVENTORY_TRANSPORT = 4327,
        /// <summary>
        /// Cannot use the transport because it is already in use.
        /// </summary>
        ERROR_TRANSPORT_FULL = 4328,
        /// <summary>
        /// Unable to open or close the inject/eject port.
        /// </summary>
        ERROR_CONTROLLING_IEPORT = 4329,
        /// <summary>
        /// Unable to eject the medium because it is in a drive.
        /// </summary>
        ERROR_UNABLE_TO_EJECT_MOUNTED_MEDIA = 4330,
        /// <summary>
        /// A cleaner slot is already reserved.
        /// </summary>
        ERROR_CLEANER_SLOT_SET = 4331,
        /// <summary>
        /// A cleaner slot is not reserved.
        /// </summary>
        ERROR_CLEANER_SLOT_NOT_SET = 4332,
        /// <summary>
        /// The cleaner cartridge has performed the maximum number of drive cleanings.
        /// </summary>
        ERROR_CLEANER_CARTRIDGE_SPENT = 4333,
        /// <summary>
        /// Unexpected on-medium identifier.
        /// </summary>
        ERROR_UNEXPECTED_OMID = 4334,
        /// <summary>
        /// The last remaining item in this group or resource cannot be deleted.
        /// </summary>
        ERROR_CANT_DELETE_LAST_ITEM = 4335,
        /// <summary>
        /// The message provided exceeds the maximum size allowed for this parameter.
        /// </summary>
        ERROR_MESSAGE_EXCEEDS_MAX_SIZE = 4336,
        /// <summary>
        /// The volume contains system or paging files.
        /// </summary>
        ERROR_VOLUME_CONTAINS_SYS_FILES = 4337,
        /// <summary>
        /// The media type cannot be removed from this library since at least one drive in the library reports it can support this media type.
        /// </summary>
        ERROR_INDIGENOUS_TYPE = 4338,
        /// <summary>
        /// This offline media cannot be mounted on this system since no enabled drives are present which can be used.
        /// </summary>
        ERROR_NO_SUPPORTING_DRIVES = 4339,
        /// <summary>
        /// A cleaner cartridge is present in the tape library.
        /// </summary>
        ERROR_CLEANER_CARTRIDGE_INSTALLED = 4340,
        /// <summary>
        /// The remote storage service was not able to recall the file.
        /// </summary>
        ERROR_FILE_OFFLINE = 4350,
        /// <summary>
        /// The remote storage service is not operational at this time.
        /// </summary>
        ERROR_REMOTE_STORAGE_NOT_ACTIVE = 4351,
        /// <summary>
        /// The remote storage service encountered a media error.
        /// </summary>
        ERROR_REMOTE_STORAGE_MEDIA_ERROR = 4352,
        /// <summary>
        /// The file or directory is not a reparse point.
        /// </summary>
        ERROR_NOT_A_REPARSE_POINT = 4390,
        /// <summary>
        /// The reparse point attribute cannot be set because it conflicts with an existing attribute.
        /// </summary>
        ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391,
        /// <summary>
        /// The data present in the reparse point buffer is invalid.
        /// </summary>
        ERROR_INVALID_REPARSE_DATA = 4392,
        /// <summary>
        /// The tag present in the reparse point buffer is invalid.
        /// </summary>
        ERROR_REPARSE_TAG_INVALID = 4393,
        /// <summary>
        /// There is a mismatch between the tag specified in the request and the tag present in the reparse point.
        /// </summary>
        ERROR_REPARSE_TAG_MISMATCH = 4394,
        /// <summary>
        /// Single Instance Storage is not available on this volume.
        /// </summary>
        ERROR_VOLUME_NOT_SIS_ENABLED = 4500,
        /// <summary>
        /// The cluster resource cannot be moved to another group because other resources are dependent on it.
        /// </summary>
        ERROR_DEPENDENT_RESOURCE_EXISTS = 5001,
        /// <summary>
        /// The cluster resource dependency cannot be found.
        /// </summary>
        ERROR_DEPENDENCY_NOT_FOUND = 5002,
        /// <summary>
        /// The cluster resource cannot be made dependent on the specified resource because it is already dependent.
        /// </summary>
        ERROR_DEPENDENCY_ALREADY_EXISTS = 5003,
        /// <summary>
        /// The cluster resource is not online.
        /// </summary>
        ERROR_RESOURCE_NOT_ONLINE = 5004,
        /// <summary>
        /// A cluster node is not available for this operation.
        /// </summary>
        ERROR_HOST_NODE_NOT_AVAILABLE = 5005,
        /// <summary>
        /// The cluster resource is not available.
        /// </summary>
        ERROR_RESOURCE_NOT_AVAILABLE = 5006,
        /// <summary>
        /// The cluster resource could not be found.
        /// </summary>
        ERROR_RESOURCE_NOT_FOUND = 5007,
        /// <summary>
        /// The cluster is being shut down.
        /// </summary>
        ERROR_SHUTDOWN_CLUSTER = 5008,
        /// <summary>
        /// A cluster node cannot be evicted from the cluster unless the node is down or it is the last node.
        /// </summary>
        ERROR_CANT_EVICT_ACTIVE_NODE = 5009,
        /// <summary>
        /// The object already exists.
        /// </summary>
        ERROR_OBJECT_ALREADY_EXISTS = 5010,
        /// <summary>
        /// The object is already in the list.
        /// </summary>
        ERROR_OBJECT_IN_LIST = 5011,
        /// <summary>
        /// The cluster group is not available for any new requests.
        /// </summary>
        ERROR_GROUP_NOT_AVAILABLE = 5012,
        /// <summary>
        /// The cluster group could not be found.
        /// </summary>
        ERROR_GROUP_NOT_FOUND = 5013,
        /// <summary>
        /// The operation could not be completed because the cluster group is not online.
        /// </summary>
        ERROR_GROUP_NOT_ONLINE = 5014,
        /// <summary>
        /// The cluster node is not the owner of the resource.
        /// </summary>
        ERROR_HOST_NODE_NOT_RESOURCE_OWNER = 5015,
        /// <summary>
        /// The cluster node is not the owner of the group.
        /// </summary>
        ERROR_HOST_NODE_NOT_GROUP_OWNER = 5016,
        /// <summary>
        /// The cluster resource could not be created in the specified resource monitor.
        /// </summary>
        ERROR_RESMON_CREATE_FAILED = 5017,
        /// <summary>
        /// The cluster resource could not be brought online by the resource monitor.
        /// </summary>
        ERROR_RESMON_ONLINE_FAILED = 5018,
        /// <summary>
        /// The operation could not be completed because the cluster resource is online.
        /// </summary>
        ERROR_RESOURCE_ONLINE = 5019,
        /// <summary>
        /// The cluster resource could not be deleted or brought offline because it is the quorum resource.
        /// </summary>
        ERROR_QUORUM_RESOURCE = 5020,
        /// <summary>
        /// The cluster could not make the specified resource a quorum resource because it is not capable of being a quorum resource.
        /// </summary>
        ERROR_NOT_QUORUM_CAPABLE = 5021,
        /// <summary>
        /// The cluster software is shutting down.
        /// </summary>
        ERROR_CLUSTER_SHUTTING_DOWN = 5022,
        /// <summary>
        /// The group or resource is not in the correct state to perform the requested operation.
        /// </summary>
        ERROR_INVALID_STATE = 5023,
        /// <summary>
        /// The properties were stored but not all changes will take effect until the next time the resource is brought online.
        /// </summary>
        ERROR_RESOURCE_PROPERTIES_STORED = 5024,
        /// <summary>
        /// The cluster could not make the specified resource a quorum resource because it does not belong to a shared storage class.
        /// </summary>
        ERROR_NOT_QUORUM_CLASS = 5025,
        /// <summary>
        /// The cluster resource could not be deleted since it is a core resource.
        /// </summary>
        ERROR_CORE_RESOURCE = 5026,
        /// <summary>
        /// The quorum resource failed to come online.
        /// </summary>
        ERROR_QUORUM_RESOURCE_ONLINE_FAILED = 5027,
        /// <summary>
        /// The quorum log could not be created or mounted successfully.
        /// </summary>
        ERROR_QUORUMLOG_OPEN_FAILED = 5028,
        /// <summary>
        /// The cluster log is corrupt.
        /// </summary>
        ERROR_CLUSTERLOG_CORRUPT = 5029,
        /// <summary>
        /// The record could not be written to the cluster log since it exceeds the maximum size.
        /// </summary>
        ERROR_CLUSTERLOG_RECORD_EXCEEDS_MAXSIZE = 5030,
        /// <summary>
        /// The cluster log exceeds its maximum size.
        /// </summary>
        ERROR_CLUSTERLOG_EXCEEDS_MAXSIZE = 5031,
        /// <summary>
        /// No checkpoint record was found in the cluster log.
        /// </summary>
        ERROR_CLUSTERLOG_CHKPOINT_NOT_FOUND = 5032,
        /// <summary>
        /// The minimum required disk space needed for logging is not available.
        /// </summary>
        ERROR_CLUSTERLOG_NOT_ENOUGH_SPACE = 5033,
        /// <summary>
        /// The cluster node failed to take control of the quorum resource because the resource is owned by another active node.
        /// </summary>
        ERROR_QUORUM_OWNER_ALIVE = 5034,
        /// <summary>
        /// A cluster network is not available for this operation.
        /// </summary>
        ERROR_NETWORK_NOT_AVAILABLE = 5035,
        /// <summary>
        /// A cluster node is not available for this operation.
        /// </summary>
        ERROR_NODE_NOT_AVAILABLE = 5036,
        /// <summary>
        /// All cluster nodes must be running to perform this operation.
        /// </summary>
        ERROR_ALL_NODES_NOT_AVAILABLE = 5037,
        /// <summary>
        /// A cluster resource failed.
        /// </summary>
        ERROR_RESOURCE_FAILED = 5038,
        /// <summary>
        /// The cluster node is not valid.
        /// </summary>
        ERROR_CLUSTER_INVALID_NODE = 5039,
        /// <summary>
        /// The cluster node already exists.
        /// </summary>
        ERROR_CLUSTER_NODE_EXISTS = 5040,
        /// <summary>
        /// A node is in the process of joining the cluster.
        /// </summary>
        ERROR_CLUSTER_JOIN_IN_PROGRESS = 5041,
        /// <summary>
        /// The cluster node was not found.
        /// </summary>
        ERROR_CLUSTER_NODE_NOT_FOUND = 5042,
        /// <summary>
        /// The cluster local node information was not found.
        /// </summary>
        ERROR_CLUSTER_LOCAL_NODE_NOT_FOUND = 5043,
        /// <summary>
        /// The cluster network already exists.
        /// </summary>
        ERROR_CLUSTER_NETWORK_EXISTS = 5044,
        /// <summary>
        /// The cluster network was not found.
        /// </summary>
        ERROR_CLUSTER_NETWORK_NOT_FOUND = 5045,
        /// <summary>
        /// The cluster network interface already exists.
        /// </summary>
        ERROR_CLUSTER_NETINTERFACE_EXISTS = 5046,
        /// <summary>
        /// The cluster network interface was not found.
        /// </summary>
        ERROR_CLUSTER_NETINTERFACE_NOT_FOUND = 5047,
        /// <summary>
        /// The cluster request is not valid for this object.
        /// </summary>
        ERROR_CLUSTER_INVALID_REQUEST = 5048,
        /// <summary>
        /// The cluster network provider is not valid.
        /// </summary>
        ERROR_CLUSTER_INVALID_NETWORK_PROVIDER = 5049,
        /// <summary>
        /// The cluster node is down.
        /// </summary>
        ERROR_CLUSTER_NODE_DOWN = 5050,
        /// <summary>
        /// The cluster node is not reachable.
        /// </summary>
        ERROR_CLUSTER_NODE_UNREACHABLE = 5051,
        /// <summary>
        /// The cluster node is not a member of the cluster.
        /// </summary>
        ERROR_CLUSTER_NODE_NOT_MEMBER = 5052,
        /// <summary>
        /// A cluster join operation is not in progress.
        /// </summary>
        ERROR_CLUSTER_JOIN_NOT_IN_PROGRESS = 5053,
        /// <summary>
        /// The cluster network is not valid.
        /// </summary>
        ERROR_CLUSTER_INVALID_NETWORK = 5054,
        /// <summary>
        /// The cluster node is up.
        /// </summary>
        ERROR_CLUSTER_NODE_UP = 5056,
        /// <summary>
        /// The cluster IP address is already in use.
        /// </summary>
        ERROR_CLUSTER_IPADDR_IN_USE = 5057,
        /// <summary>
        /// The cluster node is not paused.
        /// </summary>
        ERROR_CLUSTER_NODE_NOT_PAUSED = 5058,
        /// <summary>
        /// No cluster security context is available.
        /// </summary>
        ERROR_CLUSTER_NO_SECURITY_CONTEXT = 5059,
        /// <summary>
        /// The cluster network is not configured for internal cluster communication.
        /// </summary>
        ERROR_CLUSTER_NETWORK_NOT_INTERNAL = 5060,
        /// <summary>
        /// The cluster node is already up.
        /// </summary>
        ERROR_CLUSTER_NODE_ALREADY_UP = 5061,
        /// <summary>
        /// The cluster node is already down.
        /// </summary>
        ERROR_CLUSTER_NODE_ALREADY_DOWN = 5062,
        /// <summary>
        /// The cluster network is already online.
        /// </summary>
        ERROR_CLUSTER_NETWORK_ALREADY_ONLINE = 5063,
        /// <summary>
        /// The cluster network is already offline.
        /// </summary>
        ERROR_CLUSTER_NETWORK_ALREADY_OFFLINE = 5064,
        /// <summary>
        /// The cluster node is already a member of the cluster.
        /// </summary>
        ERROR_CLUSTER_NODE_ALREADY_MEMBER = 5065,
        /// <summary>
        /// The cluster network is the only one configured for internal cluster communication between two or more active cluster nodes. The internal communication capability cannot be removed from the network.
        /// </summary>
        ERROR_CLUSTER_LAST_INTERNAL_NETWORK = 5066,
        /// <summary>
        /// One or more cluster resources depend on the network to provide service to clients. The client access capability cannot be removed from the network.
        /// </summary>
        ERROR_CLUSTER_NETWORK_HAS_DEPENDENTS = 5067,
        /// <summary>
        /// This operation cannot be performed on the cluster resource as it the quorum resource. You may not bring the quorum resource offline or modify its possible owners list.
        /// </summary>
        ERROR_INVALID_OPERATION_ON_QUORUM = 5068,
        /// <summary>
        /// The cluster quorum resource is not allowed to have any dependencies.
        /// </summary>
        ERROR_DEPENDENCY_NOT_ALLOWED = 5069,
        /// <summary>
        /// The cluster node is paused.
        /// </summary>
        ERROR_CLUSTER_NODE_PAUSED = 5070,
        /// <summary>
        /// The cluster resource cannot be brought online. The owner node cannot run this resource.
        /// </summary>
        ERROR_NODE_CANT_HOST_RESOURCE = 5071,
        /// <summary>
        /// The cluster node is not ready to perform the requested operation.
        /// </summary>
        ERROR_CLUSTER_NODE_NOT_READY = 5072,
        /// <summary>
        /// The cluster node is shutting down.
        /// </summary>
        ERROR_CLUSTER_NODE_SHUTTING_DOWN = 5073,
        /// <summary>
        /// The cluster join operation was aborted.
        /// </summary>
        ERROR_CLUSTER_JOIN_ABORTED = 5074,
        /// <summary>
        /// The cluster join operation failed due to incompatible software versions between the joining node and its sponsor.
        /// </summary>
        ERROR_CLUSTER_INCOMPATIBLE_VERSIONS = 5075,
        /// <summary>
        /// This resource cannot be created because the cluster has reached the limit on the number of resources it can monitor.
        /// </summary>
        ERROR_CLUSTER_MAXNUM_OF_RESOURCES_EXCEEDED = 5076,
        /// <summary>
        /// The system configuration changed during the cluster join or form operation. The join or form operation was aborted.
        /// </summary>
        ERROR_CLUSTER_SYSTEM_CONFIG_CHANGED = 5077,
        /// <summary>
        /// The specified resource type was not found.
        /// </summary>
        ERROR_CLUSTER_RESOURCE_TYPE_NOT_FOUND = 5078,
        /// <summary>
        /// The specified node does not support a resource of this type.  This may be due to version inconsistencies or due to the absence of the resource DLL on this node.
        /// </summary>
        ERROR_CLUSTER_RESTYPE_NOT_SUPPORTED = 5079,
        /// <summary>
        /// The specified resource name is not supported by this resource DLL. This may be due to a bad (or changed) name supplied to the resource DLL.
        /// </summary>
        ERROR_CLUSTER_RESNAME_NOT_FOUND = 5080,
        /// <summary>
        /// No authentication package could be registered with the RPC server.
        /// </summary>
        ERROR_CLUSTER_NO_RPC_PACKAGES_REGISTERED = 5081,
        /// <summary>
        /// You cannot bring the group online because the owner of the group is not in the preferred list for the group. To change the owner node for the group, move the group.
        /// </summary>
        ERROR_CLUSTER_OWNER_NOT_IN_PREFLIST = 5082,
        /// <summary>
        /// The join operation failed because the cluster database sequence number has changed or is incompatible with the locker node. This may happen during a join operation if the cluster database was changing during the join.
        /// </summary>
        ERROR_CLUSTER_DATABASE_SEQMISMATCH = 5083,
        /// <summary>
        /// The resource monitor will not allow the fail operation to be performed while the resource is in its current state. This may happen if the resource is in a pending state.
        /// </summary>
        ERROR_RESMON_INVALID_STATE = 5084,
        /// <summary>
        /// A non locker code got a request to reserve the lock for making global updates.
        /// </summary>
        ERROR_CLUSTER_GUM_NOT_LOCKER = 5085,
        /// <summary>
        /// The quorum disk could not be located by the cluster service.
        /// </summary>
        ERROR_QUORUM_DISK_NOT_FOUND = 5086,
        /// <summary>
        /// The backed up cluster database is possibly corrupt.
        /// </summary>
        ERROR_DATABASE_BACKUP_CORRUPT = 5087,
        /// <summary>
        /// A DFS root already exists in this cluster node.
        /// </summary>
        ERROR_CLUSTER_NODE_ALREADY_HAS_DFS_ROOT = 5088,
        /// <summary>
        /// An attempt to modify a resource property failed because it conflicts with another existing property.
        /// </summary>
        ERROR_RESOURCE_PROPERTY_UNCHANGEABLE = 5089,
        /// <summary>
        /// An operation was attempted that is incompatible with the current membership state of the node.
        /// </summary>
        ERROR_CLUSTER_MEMBERSHIP_INVALID_STATE = 5890,
        /// <summary>
        /// The quorum resource does not contain the quorum log.
        /// </summary>
        ERROR_CLUSTER_QUORUMLOG_NOT_FOUND = 5891,
        /// <summary>
        /// The membership engine requested shutdown of the cluster service on this node.
        /// </summary>
        ERROR_CLUSTER_MEMBERSHIP_HALT = 5892,
        /// <summary>
        /// The join operation failed because the cluster instance ID of the joining node does not match the cluster instance ID of the sponsor node.
        /// </summary>
        ERROR_CLUSTER_INSTANCE_ID_MISMATCH = 5893,
        /// <summary>
        /// A matching network for the specified IP address could not be found. Please also specify a subnet mask and a cluster network.
        /// </summary>
        ERROR_CLUSTER_NETWORK_NOT_FOUND_FOR_IP = 5894,
        /// <summary>
        /// The actual data type of the property did not match the expected data type of the property.
        /// </summary>
        ERROR_CLUSTER_PROPERTY_DATA_TYPE_MISMATCH = 5895,
        /// <summary>
        /// The cluster node was evicted from the cluster successfully, but the node was not cleaned up.  Extended status information explaining why the node was not cleaned up is available.
        /// </summary>
        ERROR_CLUSTER_EVICT_WITHOUT_CLEANUP = 5896,
        /// <summary>
        /// Two or more parameter values specified for a resource's properties are in conflict.
        /// </summary>
        ERROR_CLUSTER_PARAMETER_MISMATCH = 5897,
        /// <summary>
        /// This computer cannot be made a member of a cluster.
        /// </summary>
        ERROR_NODE_CANNOT_BE_CLUSTERED = 5898,
        /// <summary>
        /// This computer cannot be made a member of a cluster because it does not have the correct version of Windows installed.
        /// </summary>
        ERROR_CLUSTER_WRONG_OS_VERSION = 5899,
        /// <summary>
        /// A cluster cannot be created with the specified cluster name because that cluster name is already in use. Specify a different name for the cluster.
        /// </summary>
        ERROR_CLUSTER_CANT_CREATE_DUP_CLUSTER_NAME = 5900,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_CLUSCFG_ALREADY_COMMITTED = 5901,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_CLUSCFG_ROLLBACK_FAILED = 5902,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_CLUSCFG_SYSTEM_DISK_DRIVE_LETTER_CONFLICT = 5903,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_CLUSTER_OLD_VERSION = 5904,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_CLUSTER_MISMATCHED_COMPUTER_ACCT_NAME = 5905,
        /// <summary>
        /// The specified file could not be encrypted.
        /// </summary>
        ERROR_ENCRYPTION_FAILED = 6000,
        /// <summary>
        /// The specified file could not be decrypted.
        /// </summary>
        ERROR_DECRYPTION_FAILED = 6001,
        /// <summary>
        /// The specified file is encrypted and the user does not have the ability to decrypt it.
        /// </summary>
        ERROR_FILE_ENCRYPTED = 6002,
        /// <summary>
        /// There is no valid encryption recovery policy configured for this system.
        /// </summary>
        ERROR_NO_RECOVERY_POLICY = 6003,
        /// <summary>
        /// The required encryption driver is not loaded for this system.
        /// </summary>
        ERROR_NO_EFS = 6004,
        /// <summary>
        /// The file was encrypted with a different encryption driver than is currently loaded.
        /// </summary>
        ERROR_WRONG_EFS = 6005,
        /// <summary>
        /// There are no EFS keys defined for the user.
        /// </summary>
        ERROR_NO_USER_KEYS = 6006,
        /// <summary>
        /// The specified file is not encrypted.
        /// </summary>
        ERROR_FILE_NOT_ENCRYPTED = 6007,
        /// <summary>
        /// The specified file is not in the defined EFS export format.
        /// </summary>
        ERROR_NOT_EXPORT_FORMAT = 6008,
        /// <summary>
        /// The specified file is read only.
        /// </summary>
        ERROR_FILE_READ_ONLY = 6009,
        /// <summary>
        /// The directory has been disabled for encryption.
        /// </summary>
        ERROR_DIR_EFS_DISALLOWED = 6010,
        /// <summary>
        /// The server is not trusted for remote encryption operation.
        /// </summary>
        ERROR_EFS_SERVER_NOT_TRUSTED = 6011,
        /// <summary>
        /// Recovery policy configured for this system contains invalid recovery certificate.
        /// </summary>
        ERROR_BAD_RECOVERY_POLICY = 6012,
        /// <summary>
        /// The encryption algorithm used on the source file needs a bigger key buffer than the one on the destination file.
        /// </summary>
        ERROR_EFS_ALG_BLOB_TOO_BIG = 6013,
        /// <summary>
        /// The disk partition does not support file encryption.
        /// </summary>
        ERROR_VOLUME_NOT_SUPPORT_EFS = 6014,
        /// <summary>
        /// This machine is disabled for file encryption.
        /// </summary>
        ERROR_EFS_DISABLED = 6015,
        /// <summary>
        /// A newer system is required to decrypt this encrypted file.
        /// </summary>
        ERROR_EFS_VERSION_NOT_SUPPORT = 6016,
        /// <summary>
        /// The list of servers for this workgroup is not currently available
        /// </summary>
        ERROR_NO_BROWSER_SERVERS_FOUND = 6118,
        /// <summary>
        /// The Task Scheduler service must be configured to run in the System account to function properly.  Individual tasks may be configured to run in other accounts.
        /// </summary>
        SCHED_E_SERVICE_NOT_LOCALSYSTEM = 6200,
        /// <summary>
        /// The specified session name is invalid.
        /// </summary>
        ERROR_CTX_WINSTATION_NAME_INVALID = 7001,
        /// <summary>
        /// The specified protocol driver is invalid.
        /// </summary>
        ERROR_CTX_INVALID_PD = 7002,
        /// <summary>
        /// The specified protocol driver was not found in the system path.
        /// </summary>
        ERROR_CTX_PD_NOT_FOUND = 7003,
        /// <summary>
        /// The specified terminal connection driver was not found in the system path.
        /// </summary>
        ERROR_CTX_WD_NOT_FOUND = 7004,
        /// <summary>
        /// A registry key for event logging could not be created for this session.
        /// </summary>
        ERROR_CTX_CANNOT_MAKE_EVENTLOG_ENTRY = 7005,
        /// <summary>
        /// A service with the same name already exists on the system.
        /// </summary>
        ERROR_CTX_SERVICE_NAME_COLLISION = 7006,
        /// <summary>
        /// A close operation is pending on the session.
        /// </summary>
        ERROR_CTX_CLOSE_PENDING = 7007,
        /// <summary>
        /// There are no free output buffers available.
        /// </summary>
        ERROR_CTX_NO_OUTBUF = 7008,
        /// <summary>
        /// The MODEM.INF file was not found.
        /// </summary>
        ERROR_CTX_MODEM_INF_NOT_FOUND = 7009,
        /// <summary>
        /// The modem name was not found in MODEM.INF.
        /// </summary>
        ERROR_CTX_INVALID_MODEMNAME = 7010,
        /// <summary>
        /// The modem did not accept the command sent to it. Verify that the configured modem name matches the attached modem.
        /// </summary>
        ERROR_CTX_MODEM_RESPONSE_ERROR = 7011,
        /// <summary>
        /// The modem did not respond to the command sent to it. Verify that the modem is properly cabled and powered on.
        /// </summary>
        ERROR_CTX_MODEM_RESPONSE_TIMEOUT = 7012,
        /// <summary>
        /// Carrier detect has failed or carrier has been dropped due to disconnect.
        /// </summary>
        ERROR_CTX_MODEM_RESPONSE_NO_CARRIER = 7013,
        /// <summary>
        /// Dial tone not detected within the required time. Verify that the phone cable is properly attached and functional.
        /// </summary>
        ERROR_CTX_MODEM_RESPONSE_NO_DIALTONE = 7014,
        /// <summary>
        /// Busy signal detected at remote site on callback.
        /// </summary>
        ERROR_CTX_MODEM_RESPONSE_BUSY = 7015,
        /// <summary>
        /// Voice detected at remote site on callback.
        /// </summary>
        ERROR_CTX_MODEM_RESPONSE_VOICE = 7016,
        /// <summary>
        /// Transport driver error
        /// </summary>
        ERROR_CTX_TD_ERROR = 7017,
        /// <summary>
        /// The specified session cannot be found.
        /// </summary>
        ERROR_CTX_WINSTATION_NOT_FOUND = 7022,
        /// <summary>
        /// The specified session name is already in use.
        /// </summary>
        ERROR_CTX_WINSTATION_ALREADY_EXISTS = 7023,
        /// <summary>
        /// The requested operation cannot be completed because the terminal connection is currently busy processing a connect, disconnect, reset, or delete operation.
        /// </summary>
        ERROR_CTX_WINSTATION_BUSY = 7024,
        /// <summary>
        /// An attempt has been made to connect to a session whose video mode is not supported by the current client.
        /// </summary>
        ERROR_CTX_BAD_VIDEO_MODE = 7025,
        /// <summary>
        /// The application attempted to enable DOS graphics mode.
        /// DOS graphics mode is not supported.
        /// </summary>
        ERROR_CTX_GRAPHICS_INVALID = 7035,
        /// <summary>
        /// Your interactive logon privilege has been disabled.
        /// Please contact your administrator.
        /// </summary>
        ERROR_CTX_LOGON_DISABLED = 7037,
        /// <summary>
        /// The requested operation can be performed only on the system console.
        /// This is most often the result of a driver or system DLL requiring direct console access.
        /// </summary>
        ERROR_CTX_NOT_CONSOLE = 7038,
        /// <summary>
        /// The client failed to respond to the server connect message.
        /// </summary>
        ERROR_CTX_CLIENT_QUERY_TIMEOUT = 7040,
        /// <summary>
        /// Disconnecting the console session is not supported.
        /// </summary>
        ERROR_CTX_CONSOLE_DISCONNECT = 7041,
        /// <summary>
        /// Reconnecting a disconnected session to the console is not supported.
        /// </summary>
        ERROR_CTX_CONSOLE_CONNECT = 7042,
        /// <summary>
        /// The request to control another session remotely was denied.
        /// </summary>
        ERROR_CTX_SHADOW_DENIED = 7044,
        /// <summary>
        /// The requested session access is denied.
        /// </summary>
        ERROR_CTX_WINSTATION_ACCESS_DENIED = 7045,
        /// <summary>
        /// The specified terminal connection driver is invalid.
        /// </summary>
        ERROR_CTX_INVALID_WD = 7049,
        /// <summary>
        /// The requested session cannot be controlled remotely.
        /// This may be because the session is disconnected or does not currently have a user logged on.
        /// </summary>
        ERROR_CTX_SHADOW_INVALID = 7050,
        /// <summary>
        /// The requested session is not configured to allow remote control.
        /// </summary>
        ERROR_CTX_SHADOW_DISABLED = 7051,
        /// <summary>
        /// Your request to connect to this Terminal Server has been rejected. Your Terminal Server client license number is currently being used by another user.
        /// Please call your system administrator to obtain a unique license number.
        /// </summary>
        ERROR_CTX_CLIENT_LICENSE_IN_USE = 7052,
        /// <summary>
        /// Your request to connect to this Terminal Server has been rejected. Your Terminal Server client license number has not been entered for this copy of the Terminal Server client.
        /// Please contact your system administrator.
        /// </summary>
        ERROR_CTX_CLIENT_LICENSE_NOT_SET = 7053,
        /// <summary>
        /// The system has reached its licensed logon limit.
        /// Please try again later.
        /// </summary>
        ERROR_CTX_LICENSE_NOT_AVAILABLE = 7054,
        /// <summary>
        /// The client you are using is not licensed to use this system.  Your logon request is denied.
        /// </summary>
        ERROR_CTX_LICENSE_CLIENT_INVALID = 7055,
        /// <summary>
        /// The system license has expired.  Your logon request is denied.
        /// </summary>
        ERROR_CTX_LICENSE_EXPIRED = 7056,
        /// <summary>
        /// Remote control could not be terminated because the specified session is not currently being remotely controlled.
        /// </summary>
        ERROR_CTX_SHADOW_NOT_RUNNING = 7057,
        /// <summary>
        /// The remote control of the console was terminated because the display mode was changed. Changing the display mode in a remote control session is not supported.
        /// </summary>
        ERROR_CTX_SHADOW_ENDED_BY_MODE_CHANGE = 7058,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_ACTIVATION_COUNT_EXCEEDED = 7059,
        /// <summary>
        /// The file replication service API was called incorrectly.
        /// </summary>
        FRS_ERR_INVALID_API_SEQUENCE = 8001,
        /// <summary>
        /// The file replication service cannot be started.
        /// </summary>
        FRS_ERR_STARTING_SERVICE = 8002,
        /// <summary>
        /// The file replication service cannot be stopped.
        /// </summary>
        FRS_ERR_STOPPING_SERVICE = 8003,
        /// <summary>
        /// The file replication service API terminated the request.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_INTERNAL_API = 8004,
        /// <summary>
        /// The file replication service terminated the request.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_INTERNAL = 8005,
        /// <summary>
        /// The file replication service cannot be contacted.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_SERVICE_COMM = 8006,
        /// <summary>
        /// The file replication service cannot satisfy the request because the user has insufficient privileges.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_INSUFFICIENT_PRIV = 8007,
        /// <summary>
        /// The file replication service cannot satisfy the request because authenticated RPC is not available.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_AUTHENTICATION = 8008,
        /// <summary>
        /// The file replication service cannot satisfy the request because the user has insufficient privileges on the domain controller.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_PARENT_INSUFFICIENT_PRIV = 8009,
        /// <summary>
        /// The file replication service cannot satisfy the request because authenticated RPC is not available on the domain controller.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_PARENT_AUTHENTICATION = 8010,
        /// <summary>
        /// The file replication service cannot communicate with the file replication service on the domain controller.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_CHILD_TO_PARENT_COMM = 8011,
        /// <summary>
        /// The file replication service on the domain controller cannot communicate with the file replication service on this computer.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_PARENT_TO_CHILD_COMM = 8012,
        /// <summary>
        /// The file replication service cannot populate the system volume because of an internal error.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_SYSVOL_POPULATE = 8013,
        /// <summary>
        /// The file replication service cannot populate the system volume because of an internal timeout.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_SYSVOL_POPULATE_TIMEOUT = 8014,
        /// <summary>
        /// The file replication service cannot process the request. The system volume is busy with a previous request.
        /// </summary>
        FRS_ERR_SYSVOL_IS_BUSY = 8015,
        /// <summary>
        /// The file replication service cannot stop replicating the system volume because of an internal error.
        /// The event log may have more information.
        /// </summary>
        FRS_ERR_SYSVOL_DEMOTE = 8016,
        /// <summary>
        /// The file replication service detected an invalid parameter.
        /// </summary>
        FRS_ERR_INVALID_SERVICE_PARAMETER = 8017,
        /// <summary>
        /// An error occurred while installing the directory service. For more information, see the event log.
        /// </summary>
        ERROR_DS_NOT_INSTALLED = 8200,
        /// <summary>
        /// The directory service evaluated group memberships locally.
        /// </summary>
        ERROR_DS_MEMBERSHIP_EVALUATED_LOCALLY = 8201,
        /// <summary>
        /// The specified directory service attribute or value does not exist.
        /// </summary>
        ERROR_DS_NO_ATTRIBUTE_OR_VALUE = 8202,
        /// <summary>
        /// The attribute syntax specified to the directory service is invalid.
        /// </summary>
        ERROR_DS_INVALID_ATTRIBUTE_SYNTAX = 8203,
        /// <summary>
        /// The attribute type specified to the directory service is not defined.
        /// </summary>
        ERROR_DS_ATTRIBUTE_TYPE_UNDEFINED = 8204,
        /// <summary>
        /// The specified directory service attribute or value already exists.
        /// </summary>
        ERROR_DS_ATTRIBUTE_OR_VALUE_EXISTS = 8205,
        /// <summary>
        /// The directory service is busy.
        /// </summary>
        ERROR_DS_BUSY = 8206,
        /// <summary>
        /// The directory service is unavailable.
        /// </summary>
        ERROR_DS_UNAVAILABLE = 8207,
        /// <summary>
        /// The directory service was unable to allocate a relative identifier.
        /// </summary>
        ERROR_DS_NO_RIDS_ALLOCATED = 8208,
        /// <summary>
        /// The directory service has exhausted the pool of relative identifiers.
        /// </summary>
        ERROR_DS_NO_MORE_RIDS = 8209,
        /// <summary>
        /// The requested operation could not be performed because the directory service is not the master for that type of operation.
        /// </summary>
        ERROR_DS_INCORRECT_ROLE_OWNER = 8210,
        /// <summary>
        /// The directory service was unable to initialize the subsystem that allocates relative identifiers.
        /// </summary>
        ERROR_DS_RIDMGR_INIT_ERROR = 8211,
        /// <summary>
        /// The requested operation did not satisfy one or more constraints associated with the class of the object.
        /// </summary>
        ERROR_DS_OBJ_CLASS_VIOLATION = 8212,
        /// <summary>
        /// The directory service can perform the requested operation only on a leaf object.
        /// </summary>
        ERROR_DS_CANT_ON_NON_LEAF = 8213,
        /// <summary>
        /// The directory service cannot perform the requested operation on the RDN attribute of an object.
        /// </summary>
        ERROR_DS_CANT_ON_RDN = 8214,
        /// <summary>
        /// The directory service detected an attempt to modify the object class of an object.
        /// </summary>
        ERROR_DS_CANT_MOD_OBJ_CLASS = 8215,
        /// <summary>
        /// The requested cross-domain move operation could not be performed.
        /// </summary>
        ERROR_DS_CROSS_DOM_MOVE_ERROR = 8216,
        /// <summary>
        /// Unable to contact the global catalog server.
        /// </summary>
        ERROR_DS_GC_NOT_AVAILABLE = 8217,
        /// <summary>
        /// The policy object is shared and can only be modified at the root.
        /// </summary>
        ERROR_SHARED_POLICY = 8218,
        /// <summary>
        /// The policy object does not exist.
        /// </summary>
        ERROR_POLICY_OBJECT_NOT_FOUND = 8219,
        /// <summary>
        /// The requested policy information is only in the directory service.
        /// </summary>
        ERROR_POLICY_ONLY_IN_DS = 8220,
        /// <summary>
        /// A domain controller promotion is currently active.
        /// </summary>
        ERROR_PROMOTION_ACTIVE = 8221,
        /// <summary>
        /// A domain controller promotion is not currently active
        /// </summary>
        ERROR_NO_PROMOTION_ACTIVE = 8222,
        /// <summary>
        /// An operations error occurred.
        /// </summary>
        ERROR_DS_OPERATIONS_ERROR = 8224,
        /// <summary>
        /// A protocol error occurred.
        /// </summary>
        ERROR_DS_PROTOCOL_ERROR = 8225,
        /// <summary>
        /// The time limit for this request was exceeded.
        /// </summary>
        ERROR_DS_TIMELIMIT_EXCEEDED = 8226,
        /// <summary>
        /// The size limit for this request was exceeded.
        /// </summary>
        ERROR_DS_SIZELIMIT_EXCEEDED = 8227,
        /// <summary>
        /// The administrative limit for this request was exceeded.
        /// </summary>
        ERROR_DS_ADMIN_LIMIT_EXCEEDED = 8228,
        /// <summary>
        /// The compare response was false.
        /// </summary>
        ERROR_DS_COMPARE_FALSE = 8229,
        /// <summary>
        /// The compare response was true.
        /// </summary>
        ERROR_DS_COMPARE_TRUE = 8230,
        /// <summary>
        /// The requested authentication method is not supported by the server.
        /// </summary>
        ERROR_DS_AUTH_METHOD_NOT_SUPPORTED = 8231,
        /// <summary>
        /// A more secure authentication method is required for this server.
        /// </summary>
        ERROR_DS_STRONG_AUTH_REQUIRED = 8232,
        /// <summary>
        /// Inappropriate authentication.
        /// </summary>
        ERROR_DS_INAPPROPRIATE_AUTH = 8233,
        /// <summary>
        /// The authentication mechanism is unknown.
        /// </summary>
        ERROR_DS_AUTH_UNKNOWN = 8234,
        /// <summary>
        /// A referral was returned from the server.
        /// </summary>
        ERROR_DS_REFERRAL = 8235,
        /// <summary>
        /// The server does not support the requested critical extension.
        /// </summary>
        ERROR_DS_UNAVAILABLE_CRIT_EXTENSION = 8236,
        /// <summary>
        /// This request requires a secure connection.
        /// </summary>
        ERROR_DS_CONFIDENTIALITY_REQUIRED = 8237,
        /// <summary>
        /// Inappropriate matching.
        /// </summary>
        ERROR_DS_INAPPROPRIATE_MATCHING = 8238,
        /// <summary>
        /// A constraint violation occurred.
        /// </summary>
        ERROR_DS_CONSTRAINT_VIOLATION = 8239,
        /// <summary>
        /// There is no such object on the server.
        /// </summary>
        ERROR_DS_NO_SUCH_OBJECT = 8240,
        /// <summary>
        /// There is an alias problem.
        /// </summary>
        ERROR_DS_ALIAS_PROBLEM = 8241,
        /// <summary>
        /// An invalid dn syntax has been specified.
        /// </summary>
        ERROR_DS_INVALID_DN_SYNTAX = 8242,
        /// <summary>
        /// The object is a leaf object.
        /// </summary>
        ERROR_DS_IS_LEAF = 8243,
        /// <summary>
        /// There is an alias dereferencing problem.
        /// </summary>
        ERROR_DS_ALIAS_DEREF_PROBLEM = 8244,
        /// <summary>
        /// The server is unwilling to process the request.
        /// </summary>
        ERROR_DS_UNWILLING_TO_PERFORM = 8245,
        /// <summary>
        /// A loop has been detected.
        /// </summary>
        ERROR_DS_LOOP_DETECT = 8246,
        /// <summary>
        /// There is a naming violation.
        /// </summary>
        ERROR_DS_NAMING_VIOLATION = 8247,
        /// <summary>
        /// The result set is too large.
        /// </summary>
        ERROR_DS_OBJECT_RESULTS_TOO_LARGE = 8248,
        /// <summary>
        /// The operation affects multiple DSAs
        /// </summary>
        ERROR_DS_AFFECTS_MULTIPLE_DSAS = 8249,
        /// <summary>
        /// The server is not operational.
        /// </summary>
        ERROR_DS_SERVER_DOWN = 8250,
        /// <summary>
        /// A local error has occurred.
        /// </summary>
        ERROR_DS_LOCAL_ERROR = 8251,
        /// <summary>
        /// An encoding error has occurred.
        /// </summary>
        ERROR_DS_ENCODING_ERROR = 8252,
        /// <summary>
        /// A decoding error has occurred.
        /// </summary>
        ERROR_DS_DECODING_ERROR = 8253,
        /// <summary>
        /// The search filter cannot be recognized.
        /// </summary>
        ERROR_DS_FILTER_UNKNOWN = 8254,
        /// <summary>
        /// One or more parameters are illegal.
        /// </summary>
        ERROR_DS_PARAM_ERROR = 8255,
        /// <summary>
        /// The specified method is not supported.
        /// </summary>
        ERROR_DS_NOT_SUPPORTED = 8256,
        /// <summary>
        /// No results were returned.
        /// </summary>
        ERROR_DS_NO_RESULTS_RETURNED = 8257,
        /// <summary>
        /// The specified control is not supported by the server.
        /// </summary>
        ERROR_DS_CONTROL_NOT_FOUND = 8258,
        /// <summary>
        /// A referral loop was detected by the client.
        /// </summary>
        ERROR_DS_CLIENT_LOOP = 8259,
        /// <summary>
        /// The preset referral limit was exceeded.
        /// </summary>
        ERROR_DS_REFERRAL_LIMIT_EXCEEDED = 8260,
        /// <summary>
        /// The search requires a SORT control.
        /// </summary>
        ERROR_DS_SORT_CONTROL_MISSING = 8261,
        /// <summary>
        /// The search results exceed the offset range specified.
        /// </summary>
        ERROR_DS_OFFSET_RANGE_ERROR = 8262,
        /// <summary>
        /// The root object must be the head of a naming context. The root object cannot have an instantiated parent.
        /// </summary>
        ERROR_DS_ROOT_MUST_BE_NC = 8301,
        /// <summary>
        /// The add replica operation cannot be performed. The naming context must be writable in order to create the replica.
        /// </summary>
        ERROR_DS_ADD_REPLICA_INHIBITED = 8302,
        /// <summary>
        /// A reference to an attribute that is not defined in the schema occurred.
        /// </summary>
        ERROR_DS_ATT_NOT_DEF_IN_SCHEMA = 8303,
        /// <summary>
        /// The maximum size of an object has been exceeded.
        /// </summary>
        ERROR_DS_MAX_OBJ_SIZE_EXCEEDED = 8304,
        /// <summary>
        /// An attempt was made to add an object to the directory with a name that is already in use.
        /// </summary>
        ERROR_DS_OBJ_STRING_NAME_EXISTS = 8305,
        /// <summary>
        /// An attempt was made to add an object of a class that does not have an RDN defined in the schema.
        /// </summary>
        ERROR_DS_NO_RDN_DEFINED_IN_SCHEMA = 8306,
        /// <summary>
        /// An attempt was made to add an object using an RDN that is not the RDN defined in the schema.
        /// </summary>
        ERROR_DS_RDN_DOESNT_MATCH_SCHEMA = 8307,
        /// <summary>
        /// None of the requested attributes were found on the objects.
        /// </summary>
        ERROR_DS_NO_REQUESTED_ATTS_FOUND = 8308,
        /// <summary>
        /// The user buffer is too small.
        /// </summary>
        ERROR_DS_USER_BUFFER_TO_SMALL = 8309,
        /// <summary>
        /// The attribute specified in the operation is not present on the object.
        /// </summary>
        ERROR_DS_ATT_IS_NOT_ON_OBJ = 8310,
        /// <summary>
        /// Illegal modify operation. Some aspect of the modification is not permitted.
        /// </summary>
        ERROR_DS_ILLEGAL_MOD_OPERATION = 8311,
        /// <summary>
        /// The specified object is too large.
        /// </summary>
        ERROR_DS_OBJ_TOO_LARGE = 8312,
        /// <summary>
        /// The specified instance type is not valid.
        /// </summary>
        ERROR_DS_BAD_INSTANCE_TYPE = 8313,
        /// <summary>
        /// The operation must be performed at a master DSA.
        /// </summary>
        ERROR_DS_MASTERDSA_REQUIRED = 8314,
        /// <summary>
        /// The object class attribute must be specified.
        /// </summary>
        ERROR_DS_OBJECT_CLASS_REQUIRED = 8315,
        /// <summary>
        /// A required attribute is missing.
        /// </summary>
        ERROR_DS_MISSING_REQUIRED_ATT = 8316,
        /// <summary>
        /// An attempt was made to modify an object to include an attribute that is not legal for its class.
        /// </summary>
        ERROR_DS_ATT_NOT_DEF_FOR_CLASS = 8317,
        /// <summary>
        /// The specified attribute is already present on the object.
        /// </summary>
        ERROR_DS_ATT_ALREADY_EXISTS = 8318,
        /// <summary>
        /// The specified attribute is not present, or has no values.
        /// </summary>
        ERROR_DS_CANT_ADD_ATT_VALUES = 8320,
        /// <summary>
        /// Mutliple values were specified for an attribute that can have only one value.
        /// </summary>
        ERROR_DS_SINGLE_VALUE_CONSTRAINT = 8321,
        /// <summary>
        /// A value for the attribute was not in the acceptable range of values.
        /// </summary>
        ERROR_DS_RANGE_CONSTRAINT = 8322,
        /// <summary>
        /// The specified value already exists.
        /// </summary>
        ERROR_DS_ATT_VAL_ALREADY_EXISTS = 8323,
        /// <summary>
        /// The attribute cannot be removed because it is not present on the object.
        /// </summary>
        ERROR_DS_CANT_REM_MISSING_ATT = 8324,
        /// <summary>
        /// The attribute value cannot be removed because it is not present on the object.
        /// </summary>
        ERROR_DS_CANT_REM_MISSING_ATT_VAL = 8325,
        /// <summary>
        /// The specified root object cannot be a subref.
        /// </summary>
        ERROR_DS_ROOT_CANT_BE_SUBREF = 8326,
        /// <summary>
        /// Chaining is not permitted.
        /// </summary>
        ERROR_DS_NO_CHAINING = 8327,
        /// <summary>
        /// Chained evaluation is not permitted.
        /// </summary>
        ERROR_DS_NO_CHAINED_EVAL = 8328,
        /// <summary>
        /// The operation could not be performed because the object's parent is either uninstantiated or deleted.
        /// </summary>
        ERROR_DS_NO_PARENT_OBJECT = 8329,
        /// <summary>
        /// Having a parent that is an alias is not permitted. Aliases are leaf objects.
        /// </summary>
        ERROR_DS_PARENT_IS_AN_ALIAS = 8330,
        /// <summary>
        /// The object and parent must be of the same type, either both masters or both replicas.
        /// </summary>
        ERROR_DS_CANT_MIX_MASTER_AND_REPS = 8331,
        /// <summary>
        /// The operation cannot be performed because child objects exist. This operation can only be performed on a leaf object.
        /// </summary>
        ERROR_DS_CHILDREN_EXIST = 8332,
        /// <summary>
        /// Directory object not found.
        /// </summary>
        ERROR_DS_OBJ_NOT_FOUND = 8333,
        /// <summary>
        /// The aliased object is missing.
        /// </summary>
        ERROR_DS_ALIASED_OBJ_MISSING = 8334,
        /// <summary>
        /// The object name has bad syntax.
        /// </summary>
        ERROR_DS_BAD_NAME_SYNTAX = 8335,
        /// <summary>
        /// It is not permitted for an alias to refer to another alias.
        /// </summary>
        ERROR_DS_ALIAS_POINTS_TO_ALIAS = 8336,
        /// <summary>
        /// The alias cannot be dereferenced.
        /// </summary>
        ERROR_DS_CANT_DEREF_ALIAS = 8337,
        /// <summary>
        /// The operation is out of scope.
        /// </summary>
        ERROR_DS_OUT_OF_SCOPE = 8338,
        /// <summary>
        /// The operation cannot continue because the object is in the process of being removed.
        /// </summary>
        ERROR_DS_OBJECT_BEING_REMOVED = 8339,
        /// <summary>
        /// The DSA object cannot be deleted.
        /// </summary>
        ERROR_DS_CANT_DELETE_DSA_OBJ = 8340,
        /// <summary>
        /// A directory service error has occurred.
        /// </summary>
        ERROR_DS_GENERIC_ERROR = 8341,
        /// <summary>
        /// The operation can only be performed on an internal master DSA object.
        /// </summary>
        ERROR_DS_DSA_MUST_BE_INT_MASTER = 8342,
        /// <summary>
        /// The object must be of class DSA.
        /// </summary>
        ERROR_DS_CLASS_NOT_DSA = 8343,
        /// <summary>
        /// Insufficient access rights to perform the operation.
        /// </summary>
        ERROR_DS_INSUFF_ACCESS_RIGHTS = 8344,
        /// <summary>
        /// The object cannot be added because the parent is not on the list of possible superiors.
        /// </summary>
        ERROR_DS_ILLEGAL_SUPERIOR = 8345,
        /// <summary>
        /// Access to the attribute is not permitted because the attribute is owned by the Security Accounts Manager (SAM).
        /// </summary>
        ERROR_DS_ATTRIBUTE_OWNED_BY_SAM = 8346,
        /// <summary>
        /// The name has too many parts.
        /// </summary>
        ERROR_DS_NAME_TOO_MANY_PARTS = 8347,
        /// <summary>
        /// The name is too long.
        /// </summary>
        ERROR_DS_NAME_TOO_Int32 = 8348,
        /// <summary>
        /// The name value is too long.
        /// </summary>
        ERROR_DS_NAME_VALUE_TOO_Int32 = 8349,
        /// <summary>
        /// The directory service encountered an error parsing a name.
        /// </summary>
        ERROR_DS_NAME_UNPARSEABLE = 8350,
        /// <summary>
        /// The directory service cannot get the attribute type for a name.
        /// </summary>
        ERROR_DS_NAME_TYPE_UNKNOWN = 8351,
        /// <summary>
        /// The name does not identify an object, the name identifies a phantom.
        /// </summary>
        ERROR_DS_NOT_AN_OBJECT = 8352,
        /// <summary>
        /// The security descriptor is too short.
        /// </summary>
        ERROR_DS_SEC_DESC_TOO_Int16 = 8353,
        /// <summary>
        /// The security descriptor is invalid.
        /// </summary>
        ERROR_DS_SEC_DESC_INVALID = 8354,
        /// <summary>
        /// Failed to create name for deleted object.
        /// </summary>
        ERROR_DS_NO_DELETED_NAME = 8355,
        /// <summary>
        /// The parent of a new subref must exist.
        /// </summary>
        ERROR_DS_SUBREF_MUST_HAVE_PARENT = 8356,
        /// <summary>
        /// The object must be a naming context.
        /// </summary>
        ERROR_DS_NCNAME_MUST_BE_NC = 8357,
        /// <summary>
        /// It is not permitted to add an attribute which is owned by the system.
        /// </summary>
        ERROR_DS_CANT_ADD_SYSTEM_ONLY = 8358,
        /// <summary>
        /// The class of the object must be structural, you cannot instantiate an abstract class.
        /// </summary>
        ERROR_DS_CLASS_MUST_BE_CONCRETE = 8359,
        /// <summary>
        /// The schema object could not be found.
        /// </summary>
        ERROR_DS_INVALID_DMD = 8360,
        /// <summary>
        /// A local object with this GUID (dead or alive) already exists.
        /// </summary>
        ERROR_DS_OBJ_GUID_EXISTS = 8361,
        /// <summary>
        /// The operation cannot be performed on a back link.
        /// </summary>
        ERROR_DS_NOT_ON_BACKLINK = 8362,
        /// <summary>
        /// The cross reference for the specified naming context could not be found.
        /// </summary>
        ERROR_DS_NO_CROSSREF_FOR_NC = 8363,
        /// <summary>
        /// The operation could not be performed because the directory service is shutting down.
        /// </summary>
        ERROR_DS_SHUTTING_DOWN = 8364,
        /// <summary>
        /// The directory service request is invalid.
        /// </summary>
        ERROR_DS_UNKNOWN_OPERATION = 8365,
        /// <summary>
        /// The role owner attribute could not be read.
        /// </summary>
        ERROR_DS_INVALID_ROLE_OWNER = 8366,
        /// <summary>
        /// The requested FSMO operation failed. The current FSMO holder could not be contacted.
        /// </summary>
        ERROR_DS_COULDNT_CONTACT_FSMO = 8367,
        /// <summary>
        /// Modification of a DN across a naming context is not permitted.
        /// </summary>
        ERROR_DS_CROSS_NC_DN_RENAME = 8368,
        /// <summary>
        /// The attribute cannot be modified because it is owned by the system.
        /// </summary>
        ERROR_DS_CANT_MOD_SYSTEM_ONLY = 8369,
        /// <summary>
        /// Only the replicator can perform this function.
        /// </summary>
        ERROR_DS_REPLICATOR_ONLY = 8370,
        /// <summary>
        /// The specified class is not defined.
        /// </summary>
        ERROR_DS_OBJ_CLASS_NOT_DEFINED = 8371,
        /// <summary>
        /// The specified class is not a subclass.
        /// </summary>
        ERROR_DS_OBJ_CLASS_NOT_SUBCLASS = 8372,
        /// <summary>
        /// The name reference is invalid.
        /// </summary>
        ERROR_DS_NAME_REFERENCE_INVALID = 8373,
        /// <summary>
        /// A cross reference already exists.
        /// </summary>
        ERROR_DS_CROSS_REF_EXISTS = 8374,
        /// <summary>
        /// It is not permitted to delete a master cross reference.
        /// </summary>
        ERROR_DS_CANT_DEL_MASTER_CROSSREF = 8375,
        /// <summary>
        /// Subtree notifications are only supported on NC heads.
        /// </summary>
        ERROR_DS_SUBTREE_NOTIFY_NOT_NC_HEAD = 8376,
        /// <summary>
        /// Notification filter is too complex.
        /// </summary>
        ERROR_DS_NOTIFY_FILTER_TOO_COMPLEX = 8377,
        /// <summary>
        /// Schema update failed: duplicate RDN.
        /// </summary>
        ERROR_DS_DUP_RDN = 8378,
        /// <summary>
        /// Schema update failed: duplicate OID.
        /// </summary>
        ERROR_DS_DUP_OID = 8379,
        /// <summary>
        /// Schema update failed: duplicate MAPI identifier.
        /// </summary>
        ERROR_DS_DUP_MAPI_ID = 8380,
        /// <summary>
        /// Schema update failed: duplicate schema-id GUID.
        /// </summary>
        ERROR_DS_DUP_SCHEMA_ID_GUID = 8381,
        /// <summary>
        /// Schema update failed: duplicate LDAP display name.
        /// </summary>
        ERROR_DS_DUP_LDAP_DISPLAY_NAME = 8382,
        /// <summary>
        /// Schema update failed: range-lower less than range upper.
        /// </summary>
        ERROR_DS_SEMANTIC_ATT_TEST = 8383,
        /// <summary>
        /// Schema update failed: syntax mismatch.
        /// </summary>
        ERROR_DS_SYNTAX_MISMATCH = 8384,
        /// <summary>
        /// Schema deletion failed: attribute is used in must-contain.
        /// </summary>
        ERROR_DS_EXISTS_IN_MUST_HAVE = 8385,
        /// <summary>
        /// Schema deletion failed: attribute is used in may-contain.
        /// </summary>
        ERROR_DS_EXISTS_IN_MAY_HAVE = 8386,
        /// <summary>
        /// Schema update failed: attribute in may-contain does not exist.
        /// </summary>
        ERROR_DS_NONEXISTENT_MAY_HAVE = 8387,
        /// <summary>
        /// Schema update failed: attribute in must-contain does not exist.
        /// </summary>
        ERROR_DS_NONEXISTENT_MUST_HAVE = 8388,
        /// <summary>
        /// Schema update failed: class in aux-class list does not exist or is not an auxiliary class.
        /// </summary>
        ERROR_DS_AUX_CLS_TEST_FAIL = 8389,
        /// <summary>
        /// Schema update failed: class in poss-superiors does not exist.
        /// </summary>
        ERROR_DS_NONEXISTENT_POSS_SUP = 8390,
        /// <summary>
        /// Schema update failed: class in subclassof list does not exist or does not satisfy hierarchy rules.
        /// </summary>
        ERROR_DS_SUB_CLS_TEST_FAIL = 8391,
        /// <summary>
        /// Schema update failed: Rdn-Att-Id has wrong syntax.
        /// </summary>
        ERROR_DS_BAD_RDN_ATT_ID_SYNTAX = 8392,
        /// <summary>
        /// Schema deletion failed: class is used as auxiliary class.
        /// </summary>
        ERROR_DS_EXISTS_IN_AUX_CLS = 8393,
        /// <summary>
        /// Schema deletion failed: class is used as sub class.
        /// </summary>
        ERROR_DS_EXISTS_IN_SUB_CLS = 8394,
        /// <summary>
        /// Schema deletion failed: class is used as poss superior.
        /// </summary>
        ERROR_DS_EXISTS_IN_POSS_SUP = 8395,
        /// <summary>
        /// Schema update failed in recalculating validation cache.
        /// </summary>
        ERROR_DS_RECALCSCHEMA_FAILED = 8396,
        /// <summary>
        /// The tree deletion is not finished.  The request must be made again to continue deleting the tree.
        /// </summary>
        ERROR_DS_TREE_DELETE_NOT_FINISHED = 8397,
        /// <summary>
        /// The requested delete operation could not be performed.
        /// </summary>
        ERROR_DS_CANT_DELETE = 8398,
        /// <summary>
        /// Cannot read the governs class identifier for the schema record.
        /// </summary>
        ERROR_DS_ATT_SCHEMA_REQ_ID = 8399,
        /// <summary>
        /// The attribute schema has bad syntax.
        /// </summary>
        ERROR_DS_BAD_ATT_SCHEMA_SYNTAX = 8400,
        /// <summary>
        /// The attribute could not be cached.
        /// </summary>
        ERROR_DS_CANT_CACHE_ATT = 8401,
        /// <summary>
        /// The class could not be cached.
        /// </summary>
        ERROR_DS_CANT_CACHE_CLASS = 8402,
        /// <summary>
        /// The attribute could not be removed from the cache.
        /// </summary>
        ERROR_DS_CANT_REMOVE_ATT_CACHE = 8403,
        /// <summary>
        /// The class could not be removed from the cache.
        /// </summary>
        ERROR_DS_CANT_REMOVE_CLASS_CACHE = 8404,
        /// <summary>
        /// The distinguished name attribute could not be read.
        /// </summary>
        ERROR_DS_CANT_RETRIEVE_DN = 8405,
        /// <summary>
        /// A required subref is missing.
        /// </summary>
        ERROR_DS_MISSING_SUPREF = 8406,
        /// <summary>
        /// The instance type attribute could not be retrieved.
        /// </summary>
        ERROR_DS_CANT_RETRIEVE_INSTANCE = 8407,
        /// <summary>
        /// An internal error has occurred.
        /// </summary>
        ERROR_DS_CODE_INCONSISTENCY = 8408,
        /// <summary>
        /// A database error has occurred.
        /// </summary>
        ERROR_DS_DATABASE_ERROR = 8409,
        /// <summary>
        /// The attribute GOVERNSID is missing.
        /// </summary>
        ERROR_DS_GOVERNSID_MISSING = 8410,
        /// <summary>
        /// An expected attribute is missing.
        /// </summary>
        ERROR_DS_MISSING_EXPECTED_ATT = 8411,
        /// <summary>
        /// The specified naming context is missing a cross reference.
        /// </summary>
        ERROR_DS_NCNAME_MISSING_CR_REF = 8412,
        /// <summary>
        /// A security checking error has occurred.
        /// </summary>
        ERROR_DS_SECURITY_CHECKING_ERROR = 8413,
        /// <summary>
        /// The schema is not loaded.
        /// </summary>
        ERROR_DS_SCHEMA_NOT_LOADED = 8414,
        /// <summary>
        /// Schema allocation failed. Please check if the machine is running low on memory.
        /// </summary>
        ERROR_DS_SCHEMA_ALLOC_FAILED = 8415,
        /// <summary>
        /// Failed to obtain the required syntax for the attribute schema.
        /// </summary>
        ERROR_DS_ATT_SCHEMA_REQ_SYNTAX = 8416,
        /// <summary>
        /// The global catalog verification failed. The global catalog is not available or does not support the operation. Some part of the directory is currently not available.
        /// </summary>
        ERROR_DS_GCVERIFY_ERROR = 8417,
        /// <summary>
        /// The replication operation failed because of a schema mismatch between the servers involved.
        /// </summary>
        ERROR_DS_DRA_SCHEMA_MISMATCH = 8418,
        /// <summary>
        /// The DSA object could not be found.
        /// </summary>
        ERROR_DS_CANT_FIND_DSA_OBJ = 8419,
        /// <summary>
        /// The naming context could not be found.
        /// </summary>
        ERROR_DS_CANT_FIND_EXPECTED_NC = 8420,
        /// <summary>
        /// The naming context could not be found in the cache.
        /// </summary>
        ERROR_DS_CANT_FIND_NC_IN_CACHE = 8421,
        /// <summary>
        /// The child object could not be retrieved.
        /// </summary>
        ERROR_DS_CANT_RETRIEVE_CHILD = 8422,
        /// <summary>
        /// The modification was not permitted for security reasons.
        /// </summary>
        ERROR_DS_SECURITY_ILLEGAL_MODIFY = 8423,
        /// <summary>
        /// The operation cannot replace the hidden record.
        /// </summary>
        ERROR_DS_CANT_REPLACE_HIDDEN_REC = 8424,
        /// <summary>
        /// The hierarchy file is invalid.
        /// </summary>
        ERROR_DS_BAD_HIERARCHY_FILE = 8425,
        /// <summary>
        /// The attempt to build the hierarchy table failed.
        /// </summary>
        ERROR_DS_BUILD_HIERARCHY_TABLE_FAILED = 8426,
        /// <summary>
        /// The directory configuration parameter is missing from the registry.
        /// </summary>
        ERROR_DS_CONFIG_PARAM_MISSING = 8427,
        /// <summary>
        /// The attempt to count the address book indices failed.
        /// </summary>
        ERROR_DS_COUNTING_AB_INDICES_FAILED = 8428,
        /// <summary>
        /// The allocation of the hierarchy table failed.
        /// </summary>
        ERROR_DS_HIERARCHY_TABLE_MALLOC_FAILED = 8429,
        /// <summary>
        /// The directory service encountered an internal failure.
        /// </summary>
        ERROR_DS_INTERNAL_FAILURE = 8430,
        /// <summary>
        /// The directory service encountered an unknown failure.
        /// </summary>
        ERROR_DS_UNKNOWN_ERROR = 8431,
        /// <summary>
        /// A root object requires a class of 'top'.
        /// </summary>
        ERROR_DS_ROOT_REQUIRES_CLASS_TOP = 8432,
        /// <summary>
        /// This directory server is shutting down, and cannot take ownership of new floating single-master operation roles.
        /// </summary>
        ERROR_DS_REFUSING_FSMO_ROLES = 8433,
        /// <summary>
        /// The directory service is missing mandatory configuration information, and is unable to determine the ownership of floating single-master operation roles.
        /// </summary>
        ERROR_DS_MISSING_FSMO_SETTINGS = 8434,
        /// <summary>
        /// The directory service was unable to transfer ownership of one or more floating single-master operation roles to other servers.
        /// </summary>
        ERROR_DS_UNABLE_TO_SURRENDER_ROLES = 8435,
        /// <summary>
        /// The replication operation failed.
        /// </summary>
        ERROR_DS_DRA_GENERIC = 8436,
        /// <summary>
        /// An invalid parameter was specified for this replication operation.
        /// </summary>
        ERROR_DS_DRA_INVALID_PARAMETER = 8437,
        /// <summary>
        /// The directory service is too busy to complete the replication operation at this time.
        /// </summary>
        ERROR_DS_DRA_BUSY = 8438,
        /// <summary>
        /// The distinguished name specified for this replication operation is invalid.
        /// </summary>
        ERROR_DS_DRA_BAD_DN = 8439,
        /// <summary>
        /// The naming context specified for this replication operation is invalid.
        /// </summary>
        ERROR_DS_DRA_BAD_NC = 8440,
        /// <summary>
        /// The distinguished name specified for this replication operation already exists.
        /// </summary>
        ERROR_DS_DRA_DN_EXISTS = 8441,
        /// <summary>
        /// The replication system encountered an internal error.
        /// </summary>
        ERROR_DS_DRA_INTERNAL_ERROR = 8442,
        /// <summary>
        /// The replication operation encountered a database inconsistency.
        /// </summary>
        ERROR_DS_DRA_INCONSISTENT_DIT = 8443,
        /// <summary>
        /// The server specified for this replication operation could not be contacted.
        /// </summary>
        ERROR_DS_DRA_CONNECTION_FAILED = 8444,
        /// <summary>
        /// The replication operation encountered an object with an invalid instance type.
        /// </summary>
        ERROR_DS_DRA_BAD_INSTANCE_TYPE = 8445,
        /// <summary>
        /// The replication operation failed to allocate memory.
        /// </summary>
        ERROR_DS_DRA_OUT_OF_MEM = 8446,
        /// <summary>
        /// The replication operation encountered an error with the mail system.
        /// </summary>
        ERROR_DS_DRA_MAIL_PROBLEM = 8447,
        /// <summary>
        /// The replication reference information for the target server already exists.
        /// </summary>
        ERROR_DS_DRA_REF_ALREADY_EXISTS = 8448,
        /// <summary>
        /// The replication reference information for the target server does not exist.
        /// </summary>
        ERROR_DS_DRA_REF_NOT_FOUND = 8449,
        /// <summary>
        /// The naming context cannot be removed because it is replicated to another server.
        /// </summary>
        ERROR_DS_DRA_OBJ_IS_REP_SOURCE = 8450,
        /// <summary>
        /// The replication operation encountered a database error.
        /// </summary>
        ERROR_DS_DRA_DB_ERROR = 8451,
        /// <summary>
        /// The naming context is in the process of being removed or is not replicated from the specified server.
        /// </summary>
        ERROR_DS_DRA_NO_REPLICA = 8452,
        /// <summary>
        /// Replication access was denied.
        /// </summary>
        ERROR_DS_DRA_ACCESS_DENIED = 8453,
        /// <summary>
        /// The requested operation is not supported by this version of the directory service.
        /// </summary>
        ERROR_DS_DRA_NOT_SUPPORTED = 8454,
        /// <summary>
        /// The replication remote procedure call was cancelled.
        /// </summary>
        ERROR_DS_DRA_RPC_CANCELLED = 8455,
        /// <summary>
        /// The source server is currently rejecting replication requests.
        /// </summary>
        ERROR_DS_DRA_SOURCE_DISABLED = 8456,
        /// <summary>
        /// The destination server is currently rejecting replication requests.
        /// </summary>
        ERROR_DS_DRA_SINK_DISABLED = 8457,
        /// <summary>
        /// The replication operation failed due to a collision of object names.
        /// </summary>
        ERROR_DS_DRA_NAME_COLLISION = 8458,
        /// <summary>
        /// The replication source has been reinstalled.
        /// </summary>
        ERROR_DS_DRA_SOURCE_REINSTALLED = 8459,
        /// <summary>
        /// The replication operation failed because a required parent object is missing.
        /// </summary>
        ERROR_DS_DRA_MISSING_PARENT = 8460,
        /// <summary>
        /// The replication operation was preempted.
        /// </summary>
        ERROR_DS_DRA_PREEMPTED = 8461,
        /// <summary>
        /// The replication synchronization attempt was abandoned because of a lack of updates.
        /// </summary>
        ERROR_DS_DRA_ABANDON_SYNC = 8462,
        /// <summary>
        /// The replication operation was terminated because the system is shutting down.
        /// </summary>
        ERROR_DS_DRA_SHUTDOWN = 8463,
        /// <summary>
        /// The replication synchronization attempt failed as the destination partial attribute set is not a subset of source partial attribute set.
        /// </summary>
        ERROR_DS_DRA_INCOMPATIBLE_PARTIAL_SET = 8464,
        /// <summary>
        /// The replication synchronization attempt failed because a master replica attempted to sync from a partial replica.
        /// </summary>
        ERROR_DS_DRA_SOURCE_IS_PARTIAL_REPLICA = 8465,
        /// <summary>
        /// The server specified for this replication operation was contacted, but that server was unable to contact an additional server needed to complete the operation.
        /// </summary>
        ERROR_DS_DRA_EXTN_CONNECTION_FAILED = 8466,
        /// <summary>
        /// The version of the Active Directory schema of the source forest is not compatible with the version of Active Directory on this computer.  You must upgrade the operating system on a domain controller in the source forest before this computer can be added as a domain controller to that forest.
        /// </summary>
        ERROR_DS_INSTALL_SCHEMA_MISMATCH = 8467,
        /// <summary>
        /// Schema update failed: An attribute with the same link identifier already exists.
        /// </summary>
        ERROR_DS_DUP_LINK_ID = 8468,
        /// <summary>
        /// Name translation: Generic processing error.
        /// </summary>
        ERROR_DS_NAME_ERROR_RESOLVING = 8469,
        /// <summary>
        /// Name translation: Could not find the name or insufficient right to see name.
        /// </summary>
        ERROR_DS_NAME_ERROR_NOT_FOUND = 8470,
        /// <summary>
        /// Name translation: Input name mapped to more than one output name.
        /// </summary>
        ERROR_DS_NAME_ERROR_NOT_UNIQUE = 8471,
        /// <summary>
        /// Name translation: Input name found, but not the associated output format.
        /// </summary>
        ERROR_DS_NAME_ERROR_NO_MAPPING = 8472,
        /// <summary>
        /// Name translation: Unable to resolve completely, only the domain was found.
        /// </summary>
        ERROR_DS_NAME_ERROR_DOMAIN_ONLY = 8473,
        /// <summary>
        /// Name translation: Unable to perform purely syntactical mapping at the client without going out to the wire.
        /// </summary>
        ERROR_DS_NAME_ERROR_NO_SYNTACTICAL_MAPPING = 8474,
        /// <summary>
        /// Modification of a constructed att is not allowed.
        /// </summary>
        ERROR_DS_CONSTRUCTED_ATT_MOD = 8475,
        /// <summary>
        /// The OM-Object-Class specified is incorrect for an attribute with the specified syntax.
        /// </summary>
        ERROR_DS_WRONG_OM_OBJ_CLASS = 8476,
        /// <summary>
        /// The replication request has been posted, waiting for reply.
        /// </summary>
        ERROR_DS_DRA_REPL_PENDING = 8477,
        /// <summary>
        /// The requested operation requires a directory service, and none was available.
        /// </summary>
        ERROR_DS_DS_REQUIRED = 8478,
        /// <summary>
        /// The LDAP display name of the class or attribute contains non-ASCII characters.
        /// </summary>
        ERROR_DS_INVALID_LDAP_DISPLAY_NAME = 8479,
        /// <summary>
        /// The requested search operation is only supported for base searches.
        /// </summary>
        ERROR_DS_NON_BASE_SEARCH = 8480,
        /// <summary>
        /// The search failed to retrieve attributes from the database.
        /// </summary>
        ERROR_DS_CANT_RETRIEVE_ATTS = 8481,
        /// <summary>
        /// The schema update operation tried to add a backward link attribute that has no corresponding forward link.
        /// </summary>
        ERROR_DS_BACKLINK_WITHOUT_LINK = 8482,
        /// <summary>
        /// Source and destination of a cross-domain move do not agree on the object's epoch number.  Either source or destination does not have the latest version of the object.
        /// </summary>
        ERROR_DS_EPOCH_MISMATCH = 8483,
        /// <summary>
        /// Source and destination of a cross-domain move do not agree on the object's current name.  Either source or destination does not have the latest version of the object.
        /// </summary>
        ERROR_DS_SRC_NAME_MISMATCH = 8484,
        /// <summary>
        /// Source and destination for the cross-domain move operation are identical.  Caller should use local move operation instead of cross-domain move operation.
        /// </summary>
        ERROR_DS_SRC_AND_DST_NC_IDENTICAL = 8485,
        /// <summary>
        /// Source and destination for a cross-domain move are not in agreement on the naming contexts in the forest.  Either source or destination does not have the latest version of the Partitions container.
        /// </summary>
        ERROR_DS_DST_NC_MISMATCH = 8486,
        /// <summary>
        /// Destination of a cross-domain move is not authoritative for the destination naming context.
        /// </summary>
        ERROR_DS_NOT_AUTHORITIVE_FOR_DST_NC = 8487,
        /// <summary>
        /// Source and destination of a cross-domain move do not agree on the identity of the source object.  Either source or destination does not have the latest version of the source object.
        /// </summary>
        ERROR_DS_SRC_GUID_MISMATCH = 8488,
        /// <summary>
        /// Object being moved across-domains is already known to be deleted by the destination server.  The source server does not have the latest version of the source object.
        /// </summary>
        ERROR_DS_CANT_MOVE_DELETED_OBJECT = 8489,
        /// <summary>
        /// Another operation which requires exclusive access to the PDC FSMO is already in progress.
        /// </summary>
        ERROR_DS_PDC_OPERATION_IN_PROGRESS = 8490,
        /// <summary>
        /// A cross-domain move operation failed such that two versions of the moved object exist - one each in the source and destination domains.  The destination object needs to be removed to restore the system to a consistent state.
        /// </summary>
        ERROR_DS_CROSS_DOMAIN_CLEANUP_REQD = 8491,
        /// <summary>
        /// This object may not be moved across domain boundaries either because cross-domain moves for this class are disallowed, or the object has some special characteristics, eg: trust account or restricted RID, which prevent its move.
        /// </summary>
        ERROR_DS_ILLEGAL_XDOM_MOVE_OPERATION = 8492,
        /// <summary>
        /// Can't move objects with memberships across domain boundaries as once moved, this would violate the membership conditions of the account group.  Remove the object from any account group memberships and retry.
        /// </summary>
        ERROR_DS_CANT_WITH_ACCT_GROUP_MEMBERSHPS = 8493,
        /// <summary>
        /// A naming context head must be the immediate child of another naming context head, not of an interior node.
        /// </summary>
        ERROR_DS_NC_MUST_HAVE_NC_PARENT = 8494,
        /// <summary>
        /// The directory cannot validate the proposed naming context name because it does not hold a replica of the naming context above the proposed naming context.  Please ensure that the domain naming master role is held by a server that is configured as a global catalog server, and that the server is up to date with its replication partners. (Applies only to Windows 2000 Domain Naming masters)
        /// </summary>
        ERROR_DS_CR_IMPOSSIBLE_TO_VALIDATE = 8495,
        /// <summary>
        /// Destination domain must be in native mode.
        /// </summary>
        ERROR_DS_DST_DOMAIN_NOT_NATIVE = 8496,
        /// <summary>
        /// The operation can not be performed because the server does not have an infrastructure container in the domain of interest.
        /// </summary>
        ERROR_DS_MISSING_INFRASTRUCTURE_CONTAINER = 8497,
        /// <summary>
        /// Cross-domain move of non-empty account groups is not allowed.
        /// </summary>
        ERROR_DS_CANT_MOVE_ACCOUNT_GROUP = 8498,
        /// <summary>
        /// Cross-domain move of non-empty resource groups is not allowed.
        /// </summary>
        ERROR_DS_CANT_MOVE_RESOURCE_GROUP = 8499,
        /// <summary>
        /// The search flags for the attribute are invalid. The ANR bit is valid only on attributes of Unicode or Teletex strings.
        /// </summary>
        ERROR_DS_INVALID_SEARCH_FLAG = 8500,
        /// <summary>
        /// Tree deletions starting at an object which has an NC head as a descendant are not allowed.
        /// </summary>
        ERROR_DS_NO_TREE_DELETE_ABOVE_NC = 8501,
        /// <summary>
        /// The directory service failed to lock a tree in preparation for a tree deletion because the tree was in use.
        /// </summary>
        ERROR_DS_COULDNT_LOCK_TREE_FOR_DELETE = 8502,
        /// <summary>
        /// The directory service failed to identify the list of objects to delete while attempting a tree deletion.
        /// </summary>
        ERROR_DS_COULDNT_IDENTIFY_OBJECTS_FOR_TREE_DELETE = 8503,
        /// <summary>
        /// Security Accounts Manager initialization failed because of the following error: %1.
        /// Error Status: 0x%2. Click OK to shut down the system and reboot into Directory Services Restore Mode. Check the event log for detailed information.
        /// </summary>
        ERROR_DS_SAM_INIT_FAILURE = 8504,
        /// <summary>
        /// Only an administrator can modify the membership list of an administrative group.
        /// </summary>
        ERROR_DS_SENSITIVE_GROUP_VIOLATION = 8505,
        /// <summary>
        /// Cannot change the primary group ID of a domain controller account.
        /// </summary>
        ERROR_DS_CANT_MOD_PRIMARYGROUPID = 8506,
        /// <summary>
        /// An attempt is made to modify the base schema.
        /// </summary>
        ERROR_DS_ILLEGAL_BASE_SCHEMA_MOD = 8507,
        /// <summary>
        /// Adding a new mandatory attribute to an existing class, deleting a mandatory attribute from an existing class, or adding an optional attribute to the special class Top that is not a backlink attribute (directly or through inheritance, for example, by adding or deleting an auxiliary class) is not allowed.
        /// </summary>
        ERROR_DS_NONSAFE_SCHEMA_CHANGE = 8508,
        /// <summary>
        /// Schema update is not allowed on this DC because the DC is not the schema FSMO Role Owner.
        /// </summary>
        ERROR_DS_SCHEMA_UPDATE_DISALLOWED = 8509,
        /// <summary>
        /// An object of this class cannot be created under the schema container. You can only create attribute-schema and class-schema objects under the schema container.
        /// </summary>
        ERROR_DS_CANT_CREATE_UNDER_SCHEMA = 8510,
        /// <summary>
        /// The replica/child install failed to get the objectVersion attribute on the schema container on the source DC. Either the attribute is missing on the schema container or the credentials supplied do not have permission to read it.
        /// </summary>
        ERROR_DS_INSTALL_NO_SRC_SCH_VERSION = 8511,
        /// <summary>
        /// The replica/child install failed to read the objectVersion attribute in the SCHEMA section of the file schema.ini in the system32 directory.
        /// </summary>
        ERROR_DS_INSTALL_NO_SCH_VERSION_IN_INIFILE = 8512,
        /// <summary>
        /// The specified group type is invalid.
        /// </summary>
        ERROR_DS_INVALID_GROUP_TYPE = 8513,
        /// <summary>
        /// You cannot nest global groups in a mixed domain if the group is security-enabled.
        /// </summary>
        ERROR_DS_NO_NEST_GLOBALGROUP_IN_MIXEDDOMAIN = 8514,
        /// <summary>
        /// You cannot nest local groups in a mixed domain if the group is security-enabled.
        /// </summary>
        ERROR_DS_NO_NEST_LOCALGROUP_IN_MIXEDDOMAIN = 8515,
        /// <summary>
        /// A global group cannot have a local group as a member.
        /// </summary>
        ERROR_DS_GLOBAL_CANT_HAVE_LOCAL_MEMBER = 8516,
        /// <summary>
        /// A global group cannot have a universal group as a member.
        /// </summary>
        ERROR_DS_GLOBAL_CANT_HAVE_UNIVERSAL_MEMBER = 8517,
        /// <summary>
        /// A universal group cannot have a local group as a member.
        /// </summary>
        ERROR_DS_UNIVERSAL_CANT_HAVE_LOCAL_MEMBER = 8518,
        /// <summary>
        /// A global group cannot have a cross-domain member.
        /// </summary>
        ERROR_DS_GLOBAL_CANT_HAVE_CROSSDOMAIN_MEMBER = 8519,
        /// <summary>
        /// A local group cannot have another cross domain local group as a member.
        /// </summary>
        ERROR_DS_LOCAL_CANT_HAVE_CROSSDOMAIN_LOCAL_MEMBER = 8520,
        /// <summary>
        /// A group with primary members cannot change to a security-disabled group.
        /// </summary>
        ERROR_DS_HAVE_PRIMARY_MEMBERS = 8521,
        /// <summary>
        /// The schema cache load failed to convert the string default SD on a class-schema object.
        /// </summary>
        ERROR_DS_STRING_SD_CONVERSION_FAILED = 8522,
        /// <summary>
        /// Only DSAs configured to be Global Catalog servers should be allowed to hold the Domain Naming Master FSMO role. (Applies only to Windows 2000 servers)
        /// </summary>
        ERROR_DS_NAMING_MASTER_GC = 8523,
        /// <summary>
        /// The DSA operation is unable to proceed because of a DNS lookup failure.
        /// </summary>
        ERROR_DS_DNS_LOOKUP_FAILURE = 8524,
        /// <summary>
        /// While processing a change to the DNS Host Name for an object, the Service Principal Name values could not be kept in sync.
        /// </summary>
        ERROR_DS_COULDNT_UPDATE_SPNS = 8525,
        /// <summary>
        /// The Security Descriptor attribute could not be read.
        /// </summary>
        ERROR_DS_CANT_RETRIEVE_SD = 8526,
        /// <summary>
        /// The object requested was not found, but an object with that key was found.
        /// </summary>
        ERROR_DS_KEY_NOT_UNIQUE = 8527,
        /// <summary>
        /// The syntax of the linked attribute being added is incorrect. Forward links can only have syntax 2.5.5.1, 2.5.5.7, and 2.5.5.14, and backlinks can only have syntax 2.5.5.1
        /// </summary>
        ERROR_DS_WRONG_LINKED_ATT_SYNTAX = 8528,
        /// <summary>
        /// Security Account Manager needs to get the boot password.
        /// </summary>
        ERROR_DS_SAM_NEED_BOOTKEY_PASSUInt16 = 8529,
        /// <summary>
        /// Security Account Manager needs to get the boot key from floppy disk.
        /// </summary>
        ERROR_DS_SAM_NEED_BOOTKEY_FLOPPY = 8530,
        /// <summary>
        /// Directory Service cannot start.
        /// </summary>
        ERROR_DS_CANT_START = 8531,
        /// <summary>
        /// Directory Services could not start.
        /// </summary>
        ERROR_DS_INIT_FAILURE = 8532,
        /// <summary>
        /// The connection between client and server requires packet privacy or better.
        /// </summary>
        ERROR_DS_NO_PKT_PRIVACY_ON_CONNECTION = 8533,
        /// <summary>
        /// The source domain may not be in the same forest as destination.
        /// </summary>
        ERROR_DS_SOURCE_DOMAIN_IN_FOREST = 8534,
        /// <summary>
        /// The destination domain must be in the forest.
        /// </summary>
        ERROR_DS_DESTINATION_DOMAIN_NOT_IN_FOREST = 8535,
        /// <summary>
        /// The operation requires that destination domain auditing be enabled.
        /// </summary>
        ERROR_DS_DESTINATION_AUDITING_NOT_ENABLED = 8536,
        /// <summary>
        /// The operation couldn't locate a DC for the source domain.
        /// </summary>
        ERROR_DS_CANT_FIND_DC_FOR_SRC_DOMAIN = 8537,
        /// <summary>
        /// The source object must be a group or user.
        /// </summary>
        ERROR_DS_SRC_OBJ_NOT_GROUP_OR_USER = 8538,
        /// <summary>
        /// The source object's SID already exists in destination forest.
        /// </summary>
        ERROR_DS_SRC_SID_EXISTS_IN_FOREST = 8539,
        /// <summary>
        /// The source and destination object must be of the same type.
        /// </summary>
        ERROR_DS_SRC_AND_DST_OBJECT_CLASS_MISMATCH = 8540,
        /// <summary>
        /// Security Accounts Manager initialization failed because of the following error: %1.
        /// Error Status: 0x%2. Click OK to shut down the system and reboot into Safe Mode. Check the event log for detailed information.
        /// </summary>
        ERROR_SAM_INIT_FAILURE = 8541,
        /// <summary>
        /// Schema information could not be included in the replication request.
        /// </summary>
        ERROR_DS_DRA_SCHEMA_INFO_SHIP = 8542,
        /// <summary>
        /// The replication operation could not be completed due to a schema incompatibility.
        /// </summary>
        ERROR_DS_DRA_SCHEMA_CONFLICT = 8543,
        /// <summary>
        /// The replication operation could not be completed due to a previous schema incompatibility.
        /// </summary>
        ERROR_DS_DRA_EARLIER_SCHEMA_CONFLICT = 8544,
        /// <summary>
        /// The replication update could not be applied because either the source or the destination has not yet received information regarding a recent cross-domain move operation.
        /// </summary>
        ERROR_DS_DRA_OBJ_NC_MISMATCH = 8545,
        /// <summary>
        /// The requested domain could not be deleted because there exist domain controllers that still host this domain.
        /// </summary>
        ERROR_DS_NC_STILL_HAS_DSAS = 8546,
        /// <summary>
        /// The requested operation can be performed only on a global catalog server.
        /// </summary>
        ERROR_DS_GC_REQUIRED = 8547,
        /// <summary>
        /// A local group can only be a member of other local groups in the same domain.
        /// </summary>
        ERROR_DS_LOCAL_MEMBER_OF_LOCAL_ONLY = 8548,
        /// <summary>
        /// Foreign security principals cannot be members of universal groups.
        /// </summary>
        ERROR_DS_NO_FPO_IN_UNIVERSAL_GROUPS = 8549,
        /// <summary>
        /// The attribute is not allowed to be replicated to the GC because of security reasons.
        /// </summary>
        ERROR_DS_CANT_ADD_TO_GC = 8550,
        /// <summary>
        /// The checkpoint with the PDC could not be taken because there too many modifications being processed currently.
        /// </summary>
        ERROR_DS_NO_CHECKPOINT_WITH_PDC = 8551,
        /// <summary>
        /// The operation requires that source domain auditing be enabled.
        /// </summary>
        ERROR_DS_SOURCE_AUDITING_NOT_ENABLED = 8552,
        /// <summary>
        /// Security principal objects can only be created inside domain naming contexts.
        /// </summary>
        ERROR_DS_CANT_CREATE_IN_NONDOMAIN_NC = 8553,
        /// <summary>
        /// A Service Principal Name (SPN) could not be constructed because the provided hostname is not in the necessary format.
        /// </summary>
        ERROR_DS_INVALID_NAME_FOR_SPN = 8554,
        /// <summary>
        /// A Filter was passed that uses constructed attributes.
        /// </summary>
        ERROR_DS_FILTER_USES_CONTRUCTED_ATTRS = 8555,
        /// <summary>
        /// The unicodePwd attribute value must be enclosed in double quotes.
        /// </summary>
        ERROR_DS_UNICODEPWD_NOT_IN_QUOTES = 8556,
        /// <summary>
        /// Your computer could not be joined to the domain. You have exceeded the maximum number of computer accounts you are allowed to create in this domain. Contact your system administrator to have this limit reset or increased.
        /// </summary>
        ERROR_DS_MACHINE_ACCOUNT_QUOTA_EXCEEDED = 8557,
        /// <summary>
        /// For security reasons, the operation must be run on the destination DC.
        /// </summary>
        ERROR_DS_MUST_BE_RUN_ON_DST_DC = 8558,
        /// <summary>
        /// For security reasons, the source DC must be NT4SP4 or greater.
        /// </summary>
        ERROR_DS_SRC_DC_MUST_BE_SP4_OR_GREATER = 8559,
        /// <summary>
        /// Critical Directory Service System objects cannot be deleted during tree delete operations.  The tree delete may have been partially performed.
        /// </summary>
        ERROR_DS_CANT_TREE_DELETE_CRITICAL_OBJ = 8560,
        /// <summary>
        /// Directory Services could not start because of the following error: %1.
        /// Error Status: 0x%2. Please click OK to shutdown the system. You can use the recovery console to diagnose the system further.
        /// </summary>
        ERROR_DS_INIT_FAILURE_CONSOLE = 8561,
        /// <summary>
        /// Security Accounts Manager initialization failed because of the following error: %1.
        /// Error Status: 0x%2. Please click OK to shutdown the system. You can use the recovery console to diagnose the system further.
        /// </summary>
        ERROR_DS_SAM_INIT_FAILURE_CONSOLE = 8562,
        /// <summary>
        /// This version of Windows is too old to support the current directory forest behavior.  You must upgrade the operating system on this server before it can become a domain controller in this forest.
        /// </summary>
        ERROR_DS_FOREST_VERSION_TOO_HIGH = 8563,
        /// <summary>
        /// This version of Windows is too old to support the current domain behavior.  You must upgrade the operating system on this server before it can become a domain controller in this domain.
        /// </summary>
        ERROR_DS_DOMAIN_VERSION_TOO_HIGH = 8564,
        /// <summary>
        /// This version of Windows no longer supports the behavior version in use in this directory forest.  You must advance the forest behavior version before this server can become a domain controller in the forest.
        /// </summary>
        ERROR_DS_FOREST_VERSION_TOO_LOW = 8565,
        /// <summary>
        /// This version of Windows no longer supports the behavior version in use in this domain.  You must advance the domain behavior version before this server can become a domain controller in the domain.
        /// </summary>
        ERROR_DS_DOMAIN_VERSION_TOO_LOW = 8566,
        /// <summary>
        /// The version of Windows is incompatible with the behavior version of the domain or forest.
        /// </summary>
        ERROR_DS_INCOMPATIBLE_VERSION = 8567,
        /// <summary>
        /// The behavior version cannot be increased to the requested value because Domain Controllers still exist with versions lower than the requested value.
        /// </summary>
        ERROR_DS_LOW_DSA_VERSION = 8568,
        /// <summary>
        /// The behavior version value cannot be increased while the domain is still in mixed domain mode.  You must first change the domain to native mode before increasing the behavior version.
        /// </summary>
        ERROR_DS_NO_BEHAVIOR_VERSION_IN_MIXEDDOMAIN = 8569,
        /// <summary>
        /// The sort order requested is not supported.
        /// </summary>
        ERROR_DS_NOT_SUPPORTED_SORT_ORDER = 8570,
        /// <summary>
        /// Found an object with a non unique name.
        /// </summary>
        ERROR_DS_NAME_NOT_UNIQUE = 8571,
        /// <summary>
        /// The machine account was created pre-NT4.  The account needs to be recreated.
        /// </summary>
        ERROR_DS_MACHINE_ACCOUNT_CREATED_PRENT4 = 8572,
        /// <summary>
        /// The database is out of version store.
        /// </summary>
        ERROR_DS_OUT_OF_VERSION_STORE = 8573,
        /// <summary>
        /// Unable to continue operation because multiple conflicting controls were used.
        /// </summary>
        ERROR_DS_INCOMPATIBLE_CONTROLS_USED = 8574,
        /// <summary>
        /// Unable to find a valid security descriptor reference domain for this partition.
        /// </summary>
        ERROR_DS_NO_REF_DOMAIN = 8575,
        /// <summary>
        /// Schema update failed: The link identifier is reserved.
        /// </summary>
        ERROR_DS_RESERVED_LINK_ID = 8576,
        /// <summary>
        /// Schema update failed: There are no link identifiers available.
        /// </summary>
        ERROR_DS_LINK_ID_NOT_AVAILABLE = 8577,
        /// <summary>
        /// A account group can not have a universal group as a member.
        /// </summary>
        ERROR_DS_AG_CANT_HAVE_UNIVERSAL_MEMBER = 8578,
        /// <summary>
        /// Rename or move operations on naming context heads or read-only objects are not allowed.
        /// </summary>
        ERROR_DS_MODIFYDN_DISALLOWED_BY_INSTANCE_TYPE = 8579,
        /// <summary>
        /// Move operations on objects in the schema naming context are not allowed.
        /// </summary>
        ERROR_DS_NO_OBJECT_MOVE_IN_SCHEMA_NC = 8580,
        /// <summary>
        /// A system flag has been set on the object and does not allow the object to be moved or renamed.
        /// </summary>
        ERROR_DS_MODIFYDN_DISALLOWED_BY_FLAG = 8581,
        /// <summary>
        /// This object is not allowed to change its grandparent container. Moves are not forbidden on this object, but are restricted to sibling containers.
        /// </summary>
        ERROR_DS_MODIFYDN_WRONG_GRANDPARENT = 8582,
        /// <summary>
        /// Unable to resolve completely, a referral to another forest is generated.
        /// </summary>
        ERROR_DS_NAME_ERROR_TRUST_REFERRAL = 8583,
        /// <summary>
        /// The requested action is not supported on standard server.
        /// </summary>
        ERROR_NOT_SUPPORTED_ON_STANDARD_SERVER = 8584,
        /// <summary>
        /// Could not access a partition of the Active Directory located on a remote server.  Make sure at least one server is running for the partition in question.
        /// </summary>
        ERROR_DS_CANT_ACCESS_REMOTE_PART_OF_AD = 8585,
        /// <summary>
        /// The directory cannot validate the proposed naming context (or partition) name because it does not hold a replica nor can it contact a replica of the naming context above the proposed naming context.  Please ensure that the parent naming context is properly registered in DNS, and at least one replica of this naming context is reachable by the Domain Naming master.
        /// </summary>
        ERROR_DS_CR_IMPOSSIBLE_TO_VALIDATE_V2 = 8586,
        /// <summary>
        /// The thread limit for this request was exceeded.
        /// </summary>
        ERROR_DS_THREAD_LIMIT_EXCEEDED = 8587,
        /// <summary>
        /// The Global catalog server is not in the closest site.
        /// </summary>
        ERROR_DS_NOT_CLOSEST = 8588,
        /// <summary>
        /// The DS cannot derive a service principal name (SPN) with which to mutually authenticate the target server because the corresponding server object in the local DS database has no serverReference attribute.
        /// </summary>
        ERROR_DS_CANT_DERIVE_SPN_WITHOUT_SERVER_REF = 8589,
        /// <summary>
        /// The Directory Service failed to enter single user mode.
        /// </summary>
        ERROR_DS_SINGLE_USER_MODE_FAILED = 8590,
        /// <summary>
        /// The Directory Service cannot parse the script because of a syntax error.
        /// </summary>
        ERROR_DS_NTDSCRIPT_SYNTAX_ERROR = 8591,
        /// <summary>
        /// The Directory Service cannot process the script because of an error.
        /// </summary>
        ERROR_DS_NTDSCRIPT_PROCESS_ERROR = 8592,
        /// <summary>
        /// The directory service cannot perform the requested operation because the servers
        /// involved are of different replication epochs (which is usually related to a
        /// domain rename that is in progress).
        /// </summary>
        ERROR_DS_DIFFERENT_REPL_EPOCHS = 8593,
        /// <summary>
        /// The directory service binding must be renegotiated due to a change in the server
        /// extensions information.
        /// </summary>
        ERROR_DS_DRS_EXTENSIONS_CHANGED = 8594,
        /// <summary>
        /// Operation not allowed on a disabled cross ref.
        /// </summary>
        ERROR_DS_REPLICA_SET_CHANGE_NOT_ALLOWED_ON_DISABLED_CR = 8595,
        /// <summary>
        /// Schema update failed: No values for msDS-IntId are available.
        /// </summary>
        ERROR_DS_NO_MSDS_INTID = 8596,
        /// <summary>
        /// Schema update failed: Duplicate msDS-INtId. Retry the operation.
        /// </summary>
        ERROR_DS_DUP_MSDS_INTID = 8597,
        /// <summary>
        /// Schema deletion failed: attribute is used in rDNAttID.
        /// </summary>
        ERROR_DS_EXISTS_IN_RDNATTID = 8598,
        /// <summary>
        /// The directory service failed to authorize the request.
        /// </summary>
        ERROR_DS_AUTHORIZATION_FAILED = 8599,
        /// <summary>
        /// The Directory Service cannot process the script because it is invalid.
        /// </summary>
        ERROR_DS_INVALID_SCRIPT = 8600,
        /// <summary>
        /// The remote create cross reference operation failed on the Domain Naming Master FSMO.  The operation's error is in the extended data.
        /// </summary>
        ERROR_DS_REMOTE_CROSSREF_OP_FAILED = 8601,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_CROSS_REF_BUSY = 8602,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_CANT_DERIVE_SPN_FOR_DELETED_DOMAIN = 8603,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_CANT_DEMOTE_WITH_WRITEABLE_NC = 8604,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_DUPLICATE_ID_FOUND = 8605,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_INSUFFICIENT_ATTR_TO_CREATE_OBJECT = 8606,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_GROUP_CONVERSION_ERROR = 8607,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_CANT_MOVE_APP_BASIC_GROUP = 8608,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_CANT_MOVE_APP_QUERY_GROUP = 8609,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_ROLE_NOT_VERIFIED = 8610,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_WKO_CONTAINER_CANNOT_BE_SPECIAL = 8611,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_DOMAIN_RENAME_IN_PROGRESS = 8612,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_EXISTING_AD_CHILD_NC = 8613,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_REPL_LIFETIME_EXCEEDED = 8614,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_DISALLOWED_IN_SYSTEM_CONTAINER = 8615,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ERROR_DS_LDAP_SEND_QUEUE_FULL = 8616,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_RESPONSE_CODES_BASE = 9000,
        /// <summary>
        /// DNS server unable to interpret format.
        /// </summary>
        DNS_ERROR_RCODE_FORMAT_ERROR = 9001,
        /// <summary>
        /// DNS server failure.
        /// </summary>
        DNS_ERROR_RCODE_SERVER_FAILURE = 9002,
        /// <summary>
        /// DNS name does not exist.
        /// </summary>
        DNS_ERROR_RCODE_NAME_ERROR = 9003,
        /// <summary>
        /// DNS request not supported by name server.
        /// </summary>
        DNS_ERROR_RCODE_NOT_IMPLEMENTED = 9004,
        /// <summary>
        /// DNS operation refused.
        /// </summary>
        DNS_ERROR_RCODE_REFUSED = 9005,
        /// <summary>
        /// DNS name that ought not exist, does exist.
        /// </summary>
        DNS_ERROR_RCODE_YXDOMAIN = 9006,
        /// <summary>
        /// DNS RR set that ought not exist, does exist.
        /// </summary>
        DNS_ERROR_RCODE_YXRRSET = 9007,
        /// <summary>
        /// DNS RR set that ought to exist, does not exist.
        /// </summary>
        DNS_ERROR_RCODE_NXRRSET = 9008,
        /// <summary>
        /// DNS server not authoritative for zone.
        /// </summary>
        DNS_ERROR_RCODE_NOTAUTH = 9009,
        /// <summary>
        /// DNS name in update or prereq is not in zone.
        /// </summary>
        DNS_ERROR_RCODE_NOTZONE = 9010,
        /// <summary>
        /// DNS signature failed to verify.
        /// </summary>
        DNS_ERROR_RCODE_BADSIG = 9016,
        /// <summary>
        /// DNS bad key.
        /// </summary>
        DNS_ERROR_RCODE_BADKEY = 9017,
        /// <summary>
        /// DNS signature validity expired.
        /// </summary>
        DNS_ERROR_RCODE_BADTIME = 9018,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_PACKET_FMT_BASE = 9500,
        /// <summary>
        /// No records found for given DNS query.
        /// </summary>
        DNS_INFO_NO_RECORDS = 9501,
        /// <summary>
        /// Bad DNS packet.
        /// </summary>
        DNS_ERROR_BAD_PACKET = 9502,
        /// <summary>
        /// No DNS packet.
        /// </summary>
        DNS_ERROR_NO_PACKET = 9503,
        /// <summary>
        /// DNS error, check rcode.
        /// </summary>
        DNS_ERROR_RCODE = 9504,
        /// <summary>
        /// Unsecured DNS packet.
        /// </summary>
        DNS_ERROR_UNSECURE_PACKET = 9505,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_NO_MEMORY = ERROR_OUTOFMEMORY,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_INVALID_NAME = ERROR_INVALID_NAME,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_INVALID_DATA = ERROR_INVALID_DATA,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_GENERAL_API_BASE = 9550,
        /// <summary>
        /// Invalid DNS type.
        /// </summary>
        DNS_ERROR_INVALID_TYPE = 9551,
        /// <summary>
        /// Invalid IP address.
        /// </summary>
        DNS_ERROR_INVALID_IP_ADDRESS = 9552,
        /// <summary>
        /// Invalid property.
        /// </summary>
        DNS_ERROR_INVALID_PROPERTY = 9553,
        /// <summary>
        /// Try DNS operation again later.
        /// </summary>
        DNS_ERROR_TRY_AGAIN_LATER = 9554,
        /// <summary>
        /// Record for given name and type is not unique.
        /// </summary>
        DNS_ERROR_NOT_UNIQUE = 9555,
        /// <summary>
        /// DNS name does not comply with RFC specifications.
        /// </summary>
        DNS_ERROR_NON_RFC_NAME = 9556,
        /// <summary>
        /// DNS name is a fully-qualified DNS name.
        /// </summary>
        DNS_STATUS_FQDN = 9557,
        /// <summary>
        /// DNS name is dotted (multi-label).
        /// </summary>
        DNS_STATUS_DOTTED_NAME = 9558,
        /// <summary>
        /// DNS name is a single-part name.
        /// </summary>
        DNS_STATUS_SINGLE_PART_NAME = 9559,
        /// <summary>
        /// DNS name contains an invalid character.
        /// </summary>
        DNS_ERROR_INVALID_NAME_CHAR = 9560,
        /// <summary>
        /// DNS name is entirely numeric.
        /// </summary>
        DNS_ERROR_NUMERIC_NAME = 9561,
        /// <summary>
        /// The operation requested is not permitted on a DNS root server.
        /// </summary>
        DNS_ERROR_NOT_ALLOWED_ON_ROOT_SERVER = 9562,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_NOT_ALLOWED_UNDER_DELEGATION = 9563,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_CANNOT_FIND_ROOT_HINTS = 9564,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_INCONSISTENT_ROOT_HINTS = 9565,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_ZONE_BASE = 9600,
        /// <summary>
        /// DNS zone does not exist.
        /// </summary>
        DNS_ERROR_ZONE_DOES_NOT_EXIST = 9601,
        /// <summary>
        /// DNS zone information not available.
        /// </summary>
        DNS_ERROR_NO_ZONE_INFO = 9602,
        /// <summary>
        /// Invalid operation for DNS zone.
        /// </summary>
        DNS_ERROR_INVALID_ZONE_OPERATION = 9603,
        /// <summary>
        /// Invalid DNS zone configuration.
        /// </summary>
        DNS_ERROR_ZONE_CONFIGURATION_ERROR = 9604,
        /// <summary>
        /// DNS zone has no start of authority (SOA) record.
        /// </summary>
        DNS_ERROR_ZONE_HAS_NO_SOA_RECORD = 9605,
        /// <summary>
        /// DNS zone has no Name Server (NS) record.
        /// </summary>
        DNS_ERROR_ZONE_HAS_NO_NS_RECORDS = 9606,
        /// <summary>
        /// DNS zone is locked.
        /// </summary>
        DNS_ERROR_ZONE_LOCKED = 9607,
        /// <summary>
        /// DNS zone creation failed.
        /// </summary>
        DNS_ERROR_ZONE_CREATION_FAILED = 9608,
        /// <summary>
        /// DNS zone already exists.
        /// </summary>
        DNS_ERROR_ZONE_ALREADY_EXISTS = 9609,
        /// <summary>
        /// DNS automatic zone already exists.
        /// </summary>
        DNS_ERROR_AUTOZONE_ALREADY_EXISTS = 9610,
        /// <summary>
        /// Invalid DNS zone type.
        /// </summary>
        DNS_ERROR_INVALID_ZONE_TYPE = 9611,
        /// <summary>
        /// Secondary DNS zone requires master IP address.
        /// </summary>
        DNS_ERROR_SECONDARY_REQUIRES_MASTER_IP = 9612,
        /// <summary>
        /// DNS zone not secondary.
        /// </summary>
        DNS_ERROR_ZONE_NOT_SECONDARY = 9613,
        /// <summary>
        /// Need secondary IP address.
        /// </summary>
        DNS_ERROR_NEED_SECONDARY_ADDRESSES = 9614,
        /// <summary>
        /// WINS initialization failed.
        /// </summary>
        DNS_ERROR_WINS_INIT_FAILED = 9615,
        /// <summary>
        /// Need WINS servers.
        /// </summary>
        DNS_ERROR_NEED_WINS_SERVERS = 9616,
        /// <summary>
        /// NBTSTAT initialization call failed.
        /// </summary>
        DNS_ERROR_NBSTAT_INIT_FAILED = 9617,
        /// <summary>
        /// Invalid delete of start of authority (SOA)
        /// </summary>
        DNS_ERROR_SOA_DELETE_INVALID = 9618,
        /// <summary>
        /// A conditional forwarding zone already exists for that name.
        /// </summary>
        DNS_ERROR_FORWARDER_ALREADY_EXISTS = 9619,
        /// <summary>
        /// This zone must be configured with one or more master DNS server IP addresses.
        /// </summary>
        DNS_ERROR_ZONE_REQUIRES_MASTER_IP = 9620,
        /// <summary>
        /// The operation cannot be performed because this zone is shutdown.
        /// </summary>
        DNS_ERROR_ZONE_IS_SHUTDOWN = 9621,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_DATAFILE_BASE = 9650,
        /// <summary>
        /// Primary DNS zone requires datafile.
        /// </summary>
        DNS_ERROR_PRIMARY_REQUIRES_DATAFILE = 9651,
        /// <summary>
        /// Invalid datafile name for DNS zone.
        /// </summary>
        DNS_ERROR_INVALID_DATAFILE_NAME = 9652,
        /// <summary>
        /// Failed to open datafile for DNS zone.
        /// </summary>
        DNS_ERROR_DATAFILE_OPEN_FAILURE = 9653,
        /// <summary>
        /// Failed to write datafile for DNS zone.
        /// </summary>
        DNS_ERROR_FILE_WRITEBACK_FAILED = 9654,
        /// <summary>
        /// Failure while reading datafile for DNS zone.
        /// </summary>
        DNS_ERROR_DATAFILE_PARSING = 9655,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_DATABASE_BASE = 9700,
        /// <summary>
        /// DNS record does not exist.
        /// </summary>
        DNS_ERROR_RECORD_DOES_NOT_EXIST = 9701,
        /// <summary>
        /// DNS record format error.
        /// </summary>
        DNS_ERROR_RECORD_FORMAT = 9702,
        /// <summary>
        /// Node creation failure in DNS.
        /// </summary>
        DNS_ERROR_NODE_CREATION_FAILED = 9703,
        /// <summary>
        /// Unknown DNS record type.
        /// </summary>
        DNS_ERROR_UNKNOWN_RECORD_TYPE = 9704,
        /// <summary>
        /// DNS record timed out.
        /// </summary>
        DNS_ERROR_RECORD_TIMED_OUT = 9705,
        /// <summary>
        /// Name not in DNS zone.
        /// </summary>
        DNS_ERROR_NAME_NOT_IN_ZONE = 9706,
        /// <summary>
        /// CNAME loop detected.
        /// </summary>
        DNS_ERROR_CNAME_LOOP = 9707,
        /// <summary>
        /// Node is a CNAME DNS record.
        /// </summary>
        DNS_ERROR_NODE_IS_CNAME = 9708,
        /// <summary>
        /// A CNAME record already exists for given name.
        /// </summary>
        DNS_ERROR_CNAME_COLLISION = 9709,
        /// <summary>
        /// Record only at DNS zone root.
        /// </summary>
        DNS_ERROR_RECORD_ONLY_AT_ZONE_ROOT = 9710,
        /// <summary>
        /// DNS record already exists.
        /// </summary>
        DNS_ERROR_RECORD_ALREADY_EXISTS = 9711,
        /// <summary>
        /// Secondary DNS zone data error.
        /// </summary>
        DNS_ERROR_SECONDARY_DATA = 9712,
        /// <summary>
        /// Could not create DNS cache data.
        /// </summary>
        DNS_ERROR_NO_CREATE_CACHE_DATA = 9713,
        /// <summary>
        /// DNS name does not exist.
        /// </summary>
        DNS_ERROR_NAME_DOES_NOT_EXIST = 9714,
        /// <summary>
        /// Could not create pointer (PTR) record.
        /// </summary>
        DNS_WARNING_PTR_CREATE_FAILED = 9715,
        /// <summary>
        /// DNS domain was undeleted.
        /// </summary>
        DNS_WARNING_DOMAIN_UNDELETED = 9716,
        /// <summary>
        /// The directory service is unavailable.
        /// </summary>
        DNS_ERROR_DS_UNAVAILABLE = 9717,
        /// <summary>
        /// DNS zone already exists in the directory service.
        /// </summary>
        DNS_ERROR_DS_ZONE_ALREADY_EXISTS = 9718,
        /// <summary>
        /// DNS server not creating or reading the boot file for the directory service integrated DNS zone.
        /// </summary>
        DNS_ERROR_NO_BOOTFILE_IF_DS_ZONE = 9719,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_OPERATION_BASE = 9750,
        /// <summary>
        /// DNS AXFR (zone transfer) complete.
        /// </summary>
        DNS_INFO_AXFR_COMPLETE = 9751,
        /// <summary>
        /// DNS zone transfer failed.
        /// </summary>
        DNS_ERROR_AXFR = 9752,
        /// <summary>
        /// Added local WINS server.
        /// </summary>
        DNS_INFO_ADDED_LOCAL_WINS = 9753,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_SECURE_BASE = 9800,
        /// <summary>
        /// Secure update call needs to continue update request.
        /// </summary>
        DNS_STATUS_CONTINUE_NEEDED = 9801,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_SETUP_BASE = 9850,
        /// <summary>
        /// TCP/IP network protocol not installed.
        /// </summary>
        DNS_ERROR_NO_TCPIP = 9851,
        /// <summary>
        /// No DNS servers configured for local system.
        /// </summary>
        DNS_ERROR_NO_DNS_SERVERS = 9852,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_DP_BASE = 9900,
        /// <summary>
        /// The specified directory partition does not exist.
        /// </summary>
        DNS_ERROR_DP_DOES_NOT_EXIST = 9901,
        /// <summary>
        /// The specified directory partition already exists.
        /// </summary>
        DNS_ERROR_DP_ALREADY_EXISTS = 9902,
        /// <summary>
        /// The DS is not enlisted in the specified directory partition.
        /// </summary>
        DNS_ERROR_DP_NOT_ENLISTED = 9903,
        /// <summary>
        /// The DS is already enlisted in the specified directory partition.
        /// </summary>
        DNS_ERROR_DP_ALREADY_ENLISTED = 9904,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DNS_ERROR_DP_NOT_AVAILABLE = 9905,
        /// <summary>
        /// No information avialable.
        /// </summary>
        WSABASEERR = 10000,
        /// <summary>
        /// A blocking operation was interrupted by a call to WSACancelBlockingCall.
        /// </summary>
        WSAEINTR = 10004,
        /// <summary>
        /// The file handle supplied is not valid.
        /// </summary>
        WSAEBADF = 10009,
        /// <summary>
        /// An attempt was made to access a socket in a way forbidden by its access permissions.
        /// </summary>
        WSAEACCES = 10013,
        /// <summary>
        /// The system detected an invalid pointer address in attempting to use a pointer argument in a call.
        /// </summary>
        WSAEFAULT = 10014,
        /// <summary>
        /// An invalid argument was supplied.
        /// </summary>
        WSAEINVAL = 10022,
        /// <summary>
        /// Too many open sockets.
        /// </summary>
        WSAEMFILE = 10024,
        /// <summary>
        /// A non-blocking socket operation could not be completed immediately.
        /// </summary>
        WSAEWOULDBLOCK = 10035,
        /// <summary>
        /// A blocking operation is currently executing.
        /// </summary>
        WSAEINPROGRESS = 10036,
        /// <summary>
        /// An operation was attempted on a non-blocking socket that already had an operation in progress.
        /// </summary>
        WSAEALREADY = 10037,
        /// <summary>
        /// An operation was attempted on something that is not a socket.
        /// </summary>
        WSAENOTSOCK = 10038,
        /// <summary>
        /// A required address was omitted from an operation on a socket.
        /// </summary>
        WSAEDESTADDRREQ = 10039,
        /// <summary>
        /// A message sent on a datagram socket was larger than the internal message buffer or some other network limit, or the buffer used to receive a datagram into was smaller than the datagram itself.
        /// </summary>
        WSAEMSGSIZE = 10040,
        /// <summary>
        /// A protocol was specified in the socket function call that does not support the semantics of the socket type requested.
        /// </summary>
        WSAEPROTOTYPE = 10041,
        /// <summary>
        /// An unknown, invalid, or unsupported option or level was specified in a getsockopt or setsockopt call.
        /// </summary>
        WSAENOPROTOOPT = 10042,
        /// <summary>
        /// The requested protocol has not been configured into the system, or no implementation for it exists.
        /// </summary>
        WSAEPROTONOSUPPORT = 10043,
        /// <summary>
        /// The support for the specified socket type does not exist in this address family.
        /// </summary>
        WSAESOCKTNOSUPPORT = 10044,
        /// <summary>
        /// The attempted operation is not supported for the type of object referenced.
        /// </summary>
        WSAEOPNOTSUPP = 10045,
        /// <summary>
        /// The protocol family has not been configured into the system or no implementation for it exists.
        /// </summary>
        WSAEPFNOSUPPORT = 10046,
        /// <summary>
        /// An address incompatible with the requested protocol was used.
        /// </summary>
        WSAEAFNOSUPPORT = 10047,
        /// <summary>
        /// Only one usage of each socket address (protocol/network address/port) is normally permitted.
        /// </summary>
        WSAEADDRINUSE = 10048,
        /// <summary>
        /// The requested address is not valid in its context.
        /// </summary>
        WSAEADDRNOTAVAIL = 10049,
        /// <summary>
        /// A socket operation encountered a dead network.
        /// </summary>
        WSAENETDOWN = 10050,
        /// <summary>
        /// A socket operation was attempted to an unreachable network.
        /// </summary>
        WSAENETUNREACH = 10051,
        /// <summary>
        /// The connection has been broken due to keep-alive activity detecting a failure while the operation was in progress.
        /// </summary>
        WSAENETRESET = 10052,
        /// <summary>
        /// An established connection was aborted by the software in your host machine.
        /// </summary>
        WSAECONNABORTED = 10053,
        /// <summary>
        /// An existing connection was forcibly closed by the remote host.
        /// </summary>
        WSAECONNRESET = 10054,
        /// <summary>
        /// An operation on a socket could not be performed because the system lacked sufficient buffer space or because a queue was full.
        /// </summary>
        WSAENOBUFS = 10055,
        /// <summary>
        /// A connect request was made on an already connected socket.
        /// </summary>
        WSAEISCONN = 10056,
        /// <summary>
        /// A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied.
        /// </summary>
        WSAENOTCONN = 10057,
        /// <summary>
        /// A request to send or receive data was disallowed because the socket had already been shut down in that direction with a previous shutdown call.
        /// </summary>
        WSAESHUTDOWN = 10058,
        /// <summary>
        /// Too many references to some kernel object.
        /// </summary>
        WSAETOOMANYREFS = 10059,
        /// <summary>
        /// A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.
        /// </summary>
        WSAETIMEDOUT = 10060,
        /// <summary>
        /// No connection could be made because the target machine actively refused it.
        /// </summary>
        WSAECONNREFUSED = 10061,
        /// <summary>
        /// Cannot translate name.
        /// </summary>
        WSAELOOP = 10062,
        /// <summary>
        /// Name component or name was too long.
        /// </summary>
        WSAENAMETOOInt32 = 10063,
        /// <summary>
        /// A socket operation failed because the destination host was down.
        /// </summary>
        WSAEHOSTDOWN = 10064,
        /// <summary>
        /// A socket operation was attempted to an unreachable host.
        /// </summary>
        WSAEHOSTUNREACH = 10065,
        /// <summary>
        /// Cannot remove a directory that is not empty.
        /// </summary>
        WSAENOTEMPTY = 10066,
        /// <summary>
        /// A Windows Sockets implementation may have a limit on the number of applications that may use it simultaneously.
        /// </summary>
        WSAEPROCLIM = 10067,
        /// <summary>
        /// Ran out of quota.
        /// </summary>
        WSAEUSERS = 10068,
        /// <summary>
        /// Ran out of disk quota.
        /// </summary>
        WSAEDQUOT = 10069,
        /// <summary>
        /// File handle reference is no longer available.
        /// </summary>
        WSAESTALE = 10070,
        /// <summary>
        /// Item is not available locally.
        /// </summary>
        WSAEREMOTE = 10071,
        /// <summary>
        /// WSAStartup cannot function at this time because the underlying system it uses to provide network services is currently unavailable.
        /// </summary>
        WSASYSNOTREADY = 10091,
        /// <summary>
        /// The Windows Sockets version requested is not supported.
        /// </summary>
        WSAVERNOTSUPPORTED = 10092,
        /// <summary>
        /// Either the application has not called WSAStartup, or WSAStartup failed.
        /// </summary>
        WSANOTINITIALISED = 10093,
        /// <summary>
        /// Returned by WSARecv or WSARecvFrom to indicate the remote party has initiated a graceful shutdown sequence.
        /// </summary>
        WSAEDISCON = 10101,
        /// <summary>
        /// No more results can be returned by WSALookupServiceNext.
        /// </summary>
        WSAENOMORE = 10102,
        /// <summary>
        /// A call to WSALookupServiceEnd was made while this call was still processing. The call has been canceled.
        /// </summary>
        WSAECANCELLED = 10103,
        /// <summary>
        /// The procedure call table is invalid.
        /// </summary>
        WSAEINVALIDPROCTABLE = 10104,
        /// <summary>
        /// The requested service provider is invalid.
        /// </summary>
        WSAEINVALIDPROVIDER = 10105,
        /// <summary>
        /// The requested service provider could not be loaded or initialized.
        /// </summary>
        WSAEPROVIDERFAILEDINIT = 10106,
        /// <summary>
        /// A system call that should never fail has failed.
        /// </summary>
        WSASYSCALLFAILURE = 10107,
        /// <summary>
        /// No such service is known. The service cannot be found in the specified name space.
        /// </summary>
        WSASERVICE_NOT_FOUND = 10108,
        /// <summary>
        /// The specified class was not found.
        /// </summary>
        WSATYPE_NOT_FOUND = 10109,
        /// <summary>
        /// No more results can be returned by WSALookupServiceNext.
        /// </summary>
        WSA_E_NO_MORE = 10110,
        /// <summary>
        /// A call to WSALookupServiceEnd was made while this call was still processing. The call has been canceled.
        /// </summary>
        WSA_E_CANCELLED = 10111,
        /// <summary>
        /// A database query failed because it was actively refused.
        /// </summary>
        WSAEREFUSED = 10112,
        /// <summary>
        /// No such host is known.
        /// </summary>
        WSAHOST_NOT_FOUND = 11001,
        /// <summary>
        /// This is usually a temporary error during hostname resolution and means that the local server did not receive a response from an authoritative server.
        /// </summary>
        WSATRY_AGAIN = 11002,
        /// <summary>
        /// A non-recoverable error occurred during a database lookup.
        /// </summary>
        WSANO_RECOVERY = 11003,
        /// <summary>
        /// The requested name is valid and was found in the database, but it does not have the correct associated data being resolved for.
        /// </summary>
        WSANO_DATA = 11004,
        /// <summary>
        /// At least one reserve has arrived.
        /// </summary>
        WSA_QOS_RECEIVERS = 11005,
        /// <summary>
        /// At least one path has arrived.
        /// </summary>
        WSA_QOS_SENDERS = 11006,
        /// <summary>
        /// There are no senders.
        /// </summary>
        WSA_QOS_NO_SENDERS = 11007,
        /// <summary>
        /// There are no receivers.
        /// </summary>
        WSA_QOS_NO_RECEIVERS = 11008,
        /// <summary>
        /// Reserve has been confirmed.
        /// </summary>
        WSA_QOS_REQUEST_CONFIRMED = 11009,
        /// <summary>
        /// Error due to lack of resources.
        /// </summary>
        WSA_QOS_ADMISSION_FAILURE = 11010,
        /// <summary>
        /// Rejected for administrative reasons - bad credentials.
        /// </summary>
        WSA_QOS_POLICY_FAILURE = 11011,
        /// <summary>
        /// Unknown or conflicting style.
        /// </summary>
        WSA_QOS_BAD_STYLE = 11012,
        /// <summary>
        /// Problem with some part of the filterspec or providerspecific buffer in general.
        /// </summary>
        WSA_QOS_BAD_OBJECT = 11013,
        /// <summary>
        /// Problem with some part of the flowspec.
        /// </summary>
        WSA_QOS_TRAFFIC_CTRL_ERROR = 11014,
        /// <summary>
        /// General QOS error.
        /// </summary>
        WSA_QOS_GENERIC_ERROR = 11015,
        /// <summary>
        /// An invalid or unrecognized service type was found in the flowspec.
        /// </summary>
        WSA_QOS_ESERVICETYPE = 11016,
        /// <summary>
        /// An invalid or inconsistent flowspec was found in the QOS structure.
        /// </summary>
        WSA_QOS_EFLOWSPEC = 11017,
        /// <summary>
        /// Invalid QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_EPROVSPECBUF = 11018,
        /// <summary>
        /// An invalid QOS filter style was used.
        /// </summary>
        WSA_QOS_EFILTERSTYLE = 11019,
        /// <summary>
        /// An invalid QOS filter type was used.
        /// </summary>
        WSA_QOS_EFILTERTYPE = 11020,
        /// <summary>
        /// An incorrect number of QOS FILTERSPECs were specified in the FLOWDESCRIPTOR.
        /// </summary>
        WSA_QOS_EFILTERCOUNT = 11021,
        /// <summary>
        /// An object with an invalid ObjectLength field was specified in the QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_EOBJLENGTH = 11022,
        /// <summary>
        /// An incorrect number of flow descriptors was specified in the QOS structure.
        /// </summary>
        WSA_QOS_EFLOWCOUNT = 11023,
        /// <summary>
        /// An unrecognized object was found in the QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_EUNKOWNPSOBJ = 11024,
        /// <summary>
        /// An invalid policy object was found in the QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_EPOLICYOBJ = 11025,
        /// <summary>
        /// An invalid QOS flow descriptor was found in the flow descriptor list.
        /// </summary>
        WSA_QOS_EFLOWDESC = 11026,
        /// <summary>
        /// An invalid or inconsistent flowspec was found in the QOS provider specific buffer.
        /// </summary>
        WSA_QOS_EPSFLOWSPEC = 11027,
        /// <summary>
        /// An invalid FILTERSPEC was found in the QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_EPSFILTERSPEC = 11028,
        /// <summary>
        /// An invalid shape discard mode object was found in the QOS provider specific buffer.
        /// </summary>
        WSA_QOS_ESDMODEOBJ = 11029,
        /// <summary>
        /// An invalid shaping rate object was found in the QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_ESHAPERATEOBJ = 11030,
        /// <summary>
        /// A reserved policy element was found in the QOS provider-specific buffer.
        /// </summary>
        WSA_QOS_RESERVED_PETYPE = 11031,
        /// <summary>
        /// The requested section was not present in the activation context.
        /// </summary>
        ERROR_SXS_SECTION_NOT_FOUND = 14000,
        /// <summary>
        /// This application has failed to start because the application configuration is incorrect. Reinstalling the application may fix this problem.
        /// </summary>
        ERROR_SXS_CANT_GEN_ACTCTX = 14001,
        /// <summary>
        /// The application binding data format is invalid.
        /// </summary>
        ERROR_SXS_INVALID_ACTCTXDATA_FORMAT = 14002,
        /// <summary>
        /// The referenced assembly is not installed on your system.
        /// </summary>
        ERROR_SXS_ASSEMBLY_NOT_FOUND = 14003,
        /// <summary>
        /// The manifest file does not begin with the required tag and format information.
        /// </summary>
        ERROR_SXS_MANIFEST_FORMAT_ERROR = 14004,
        /// <summary>
        /// The manifest file contains one or more syntax errors.
        /// </summary>
        ERROR_SXS_MANIFEST_PARSE_ERROR = 14005,
        /// <summary>
        /// The application attempted to activate a disabled activation context.
        /// </summary>
        ERROR_SXS_ACTIVATION_CONTEXT_DISABLED = 14006,
        /// <summary>
        /// The requested lookup key was not found in any active activation context.
        /// </summary>
        ERROR_SXS_KEY_NOT_FOUND = 14007,
        /// <summary>
        /// A component version required by the application conflicts with another component version already active.
        /// </summary>
        ERROR_SXS_VERSION_CONFLICT = 14008,
        /// <summary>
        /// The type requested activation context section does not match the query API used.
        /// </summary>
        ERROR_SXS_WRONG_SECTION_TYPE = 14009,
        /// <summary>
        /// Lack of system resources has required isolated activation to be disabled for the current thread of execution.
        /// </summary>
        ERROR_SXS_THREAD_QUERIES_DISABLED = 14010,
        /// <summary>
        /// An attempt to set the process default activation context failed because the process default activation context was already set.
        /// </summary>
        ERROR_SXS_PROCESS_DEFAULT_ALREADY_SET = 14011,
        /// <summary>
        /// The encoding group identifier specified is not recognized.
        /// </summary>
        ERROR_SXS_UNKNOWN_ENCODING_GROUP = 14012,
        /// <summary>
        /// The encoding requested is not recognized.
        /// </summary>
        ERROR_SXS_UNKNOWN_ENCODING = 14013,
        /// <summary>
        /// The manifest contains a reference to an invalid URI.
        /// </summary>
        ERROR_SXS_INVALID_XML_NAMESPACE_URI = 14014,
        /// <summary>
        /// The application manifest contains a reference to a dependent assembly which is not installed
        /// </summary>
        ERROR_SXS_ROOT_MANIFEST_DEPENDENCY_NOT_INSTALLED = 14015,
        /// <summary>
        /// The manifest for an assembly used by the application has a reference to a dependent assembly which is not installed
        /// </summary>
        ERROR_SXS_LEAF_MANIFEST_DEPENDENCY_NOT_INSTALLED = 14016,
        /// <summary>
        /// The manifest contains an attribute for the assembly identity which is not valid.
        /// </summary>
        ERROR_SXS_INVALID_ASSEMBLY_IDENTITY_ATTRIBUTE = 14017,
        /// <summary>
        /// The manifest is missing the required default namespace specification on the assembly element.
        /// </summary>
        ERROR_SXS_MANIFEST_MISSING_REQUIRED_DEFAULT_NAMESPACE = 14018,
        /// <summary>
        /// The manifest has a default namespace specified on the assembly element but its value is not "urn:schemas-microsoft-com:asm.v1".
        /// </summary>
        ERROR_SXS_MANIFEST_INVALID_REQUIRED_DEFAULT_NAMESPACE = 14019,
        /// <summary>
        /// The private manifest probed has crossed reparse-point-associated path
        /// </summary>
        ERROR_SXS_PRIVATE_MANIFEST_CROSS_PATH_WITH_REPARSE_POINT = 14020,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest have files by the same name.
        /// </summary>
        ERROR_SXS_DUPLICATE_DLL_NAME = 14021,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest have window classes with the same name.
        /// </summary>
        ERROR_SXS_DUPLICATE_WINDOWCLASS_NAME = 14022,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest have the same COM server CLSIDs.
        /// </summary>
        ERROR_SXS_DUPLICATE_CLSID = 14023,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest have proxies for the same COM interface IIDs.
        /// </summary>
        ERROR_SXS_DUPLICATE_IID = 14024,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest have the same COM type library TLBIDs.
        /// </summary>
        ERROR_SXS_DUPLICATE_TLBID = 14025,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest have the same COM ProgIDs.
        /// </summary>
        ERROR_SXS_DUPLICATE_PROGID = 14026,
        /// <summary>
        /// Two or more components referenced directly or indirectly by the application manifest are different versions of the same component which is not permitted.
        /// </summary>
        ERROR_SXS_DUPLICATE_ASSEMBLY_NAME = 14027,
        /// <summary>
        /// A component's file does not match the verification information present in the
        /// component manifest.
        /// </summary>
        ERROR_SXS_FILE_HASH_MISMATCH = 14028,
        /// <summary>
        /// The policy manifest contains one or more syntax errors.
        /// </summary>
        ERROR_SXS_POLICY_PARSE_ERROR = 14029,
        /// <summary>
        /// Manifest Parse Error : A string literal was expected, but no opening quote character was found.
        /// </summary>
        ERROR_SXS_XML_E_MISSINGQUOTE = 14030,
        /// <summary>
        /// Manifest Parse Error : Incorrect syntax was used in a comment.
        /// </summary>
        ERROR_SXS_XML_E_COMMENTSYNTAX = 14031,
        /// <summary>
        /// Manifest Parse Error : A name was started with an invalid character.
        /// </summary>
        ERROR_SXS_XML_E_BADSTARTNAMECHAR = 14032,
        /// <summary>
        /// Manifest Parse Error : A name contained an invalid character.
        /// </summary>
        ERROR_SXS_XML_E_BADNAMECHAR = 14033,
        /// <summary>
        /// Manifest Parse Error : A string literal contained an invalid character.
        /// </summary>
        ERROR_SXS_XML_E_BADCHARINSTRING = 14034,
        /// <summary>
        /// Manifest Parse Error : Invalid syntax for an xml declaration.
        /// </summary>
        ERROR_SXS_XML_E_XMLDECLSYNTAX = 14035,
        /// <summary>
        /// Manifest Parse Error : An Invalid character was found in text content.
        /// </summary>
        ERROR_SXS_XML_E_BADCHARDATA = 14036,
        /// <summary>
        /// Manifest Parse Error : Required white space was missing.
        /// </summary>
        ERROR_SXS_XML_E_MISSINGWHITESPACE = 14037,
        /// <summary>
        /// Manifest Parse Error : The character '>' was expected.
        /// </summary>
        ERROR_SXS_XML_E_EXPECTINGTAGEND = 14038,
        /// <summary>
        /// Manifest Parse Error : A semi colon character was expected.
        /// </summary>
        ERROR_SXS_XML_E_MISSINGSEMICOLON = 14039,
        /// <summary>
        /// Manifest Parse Error : Unbalanced parentheses.
        /// </summary>
        ERROR_SXS_XML_E_UNBALANCEDPAREN = 14040,
        /// <summary>
        /// Manifest Parse Error : Internal error.
        /// </summary>
        ERROR_SXS_XML_E_INTERNALERROR = 14041,
        /// <summary>
        /// Manifest Parse Error : Whitespace is not allowed at this location.
        /// </summary>
        ERROR_SXS_XML_E_UNEXPECTED_WHITESPACE = 14042,
        /// <summary>
        /// Manifest Parse Error : End of file reached in invalid state for current encoding.
        /// </summary>
        ERROR_SXS_XML_E_INCOMPLETE_ENCODING = 14043,
        /// <summary>
        /// Manifest Parse Error : Missing parenthesis.
        /// </summary>
        ERROR_SXS_XML_E_MISSING_PAREN = 14044,
        /// <summary>
        /// Manifest Parse Error : A single or double closing quote character (\' or \") is missing.
        /// </summary>
        ERROR_SXS_XML_E_EXPECTINGCLOSEQUOTE = 14045,
        /// <summary>
        /// Manifest Parse Error : Multiple colons are not allowed in a name.
        /// </summary>
        ERROR_SXS_XML_E_MULTIPLE_COLONS = 14046,
        /// <summary>
        /// Manifest Parse Error : Invalid character for decimal digit.
        /// </summary>
        ERROR_SXS_XML_E_INVALID_DECIMAL = 14047,
        /// <summary>
        /// Manifest Parse Error : Invalid character for hexidecimal digit.
        /// </summary>
        ERROR_SXS_XML_E_INVALID_HEXIDECIMAL = 14048,
        /// <summary>
        /// Manifest Parse Error : Invalid unicode character value for this platform.
        /// </summary>
        ERROR_SXS_XML_E_INVALID_UNICODE = 14049,
        /// <summary>
        /// Manifest Parse Error : Expecting whitespace or '?'.
        /// </summary>
        ERROR_SXS_XML_E_WHITESPACEORQUESTIONMARK = 14050,
        /// <summary>
        /// Manifest Parse Error : End tag was not expected at this location.
        /// </summary>
        ERROR_SXS_XML_E_UNEXPECTEDENDTAG = 14051,
        /// <summary>
        /// Manifest Parse Error : The following tags were not closed: %1.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDTAG = 14052,
        /// <summary>
        /// Manifest Parse Error : Duplicate attribute.
        /// </summary>
        ERROR_SXS_XML_E_DUPLICATEATTRIBUTE = 14053,
        /// <summary>
        /// Manifest Parse Error : Only one top level element is allowed in an XML document.
        /// </summary>
        ERROR_SXS_XML_E_MULTIPLEROOTS = 14054,
        /// <summary>
        /// Manifest Parse Error : Invalid at the top level of the document.
        /// </summary>
        ERROR_SXS_XML_E_INVALIDATROOTLEVEL = 14055,
        /// <summary>
        /// Manifest Parse Error : Invalid xml declaration.
        /// </summary>
        ERROR_SXS_XML_E_BADXMLDECL = 14056,
        /// <summary>
        /// Manifest Parse Error : XML document must have a top level element.
        /// </summary>
        ERROR_SXS_XML_E_MISSINGROOT = 14057,
        /// <summary>
        /// Manifest Parse Error : Unexpected end of file.
        /// </summary>
        ERROR_SXS_XML_E_UNEXPECTEDEOF = 14058,
        /// <summary>
        /// Manifest Parse Error : Parameter entities cannot be used inside markup declarations in an internal subset.
        /// </summary>
        ERROR_SXS_XML_E_BADPEREFINSUBSET = 14059,
        /// <summary>
        /// Manifest Parse Error : Element was not closed.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDSTARTTAG = 14060,
        /// <summary>
        /// Manifest Parse Error : End element was missing the character '>'.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDENDTAG = 14061,
        /// <summary>
        /// Manifest Parse Error : A string literal was not closed.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDSTRING = 14062,
        /// <summary>
        /// Manifest Parse Error : A comment was not closed.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDCOMMENT = 14063,
        /// <summary>
        /// Manifest Parse Error : A declaration was not closed.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDDECL = 14064,
        /// <summary>
        /// Manifest Parse Error : A CDATA section was not closed.
        /// </summary>
        ERROR_SXS_XML_E_UNCLOSEDCDATA = 14065,
        /// <summary>
        /// Manifest Parse Error : The namespace prefix is not allowed to start with the reserved string "xml".
        /// </summary>
        ERROR_SXS_XML_E_RESERVEDNAMESPACE = 14066,
        /// <summary>
        /// Manifest Parse Error : System does not support the specified encoding.
        /// </summary>
        ERROR_SXS_XML_E_INVALIDENCODING = 14067,
        /// <summary>
        /// Manifest Parse Error : Switch from current encoding to specified encoding not supported.
        /// </summary>
        ERROR_SXS_XML_E_INVALIDSWITCH = 14068,
        /// <summary>
        /// Manifest Parse Error : The name 'xml' is reserved and must be lower case.
        /// </summary>
        ERROR_SXS_XML_E_BADXMLCASE = 14069,
        /// <summary>
        /// Manifest Parse Error : The standalone attribute must have the value 'yes' or 'no'.
        /// </summary>
        ERROR_SXS_XML_E_INVALID_STANDALONE = 14070,
        /// <summary>
        /// Manifest Parse Error : The standalone attribute cannot be used in external entities.
        /// </summary>
        ERROR_SXS_XML_E_UNEXPECTED_STANDALONE = 14071,
        /// <summary>
        /// Manifest Parse Error : Invalid version number.
        /// </summary>
        ERROR_SXS_XML_E_INVALID_VERSION = 14072,
        /// <summary>
        /// Manifest Parse Error : Missing equals sign between attribute and attribute value.
        /// </summary>
        ERROR_SXS_XML_E_MISSINGEQUALS = 14073,
        /// <summary>
        /// Assembly Protection Error : Unable to recover the specified assembly.
        /// </summary>
        ERROR_SXS_PROTECTION_RECOVERY_FAILED = 14074,
        /// <summary>
        /// Assembly Protection Error : The public key for an assembly was too short to be allowed.
        /// </summary>
        ERROR_SXS_PROTECTION_PUBLIC_KEY_TOO_Int16 = 14075,
        /// <summary>
        /// Assembly Protection Error : The catalog for an assembly is not valid, or does not match the assembly's manifest.
        /// </summary>
        ERROR_SXS_PROTECTION_CATALOG_NOT_VALID = 14076,
        /// <summary>
        /// An HRESULT could not be translated to a corresponding Win32 error code.
        /// </summary>
        ERROR_SXS_UNTRANSLATABLE_HRESULT = 14077,
        /// <summary>
        /// Assembly Protection Error : The catalog for an assembly is missing.
        /// </summary>
        ERROR_SXS_PROTECTION_CATALOG_FILE_MISSING = 14078,
        /// <summary>
        /// The supplied assembly identity is missing one or more attributes which must be present in this context.
        /// </summary>
        ERROR_SXS_MISSING_ASSEMBLY_IDENTITY_ATTRIBUTE = 14079,
        /// <summary>
        /// The supplied assembly identity has one or more attribute names that contain characters not permitted in XML names.
        /// </summary>
        ERROR_SXS_INVALID_ASSEMBLY_IDENTITY_ATTRIBUTE_NAME = 14080,
        /// <summary>
        /// The specified quick mode policy already exists.
        /// </summary>
        ERROR_IPSEC_QM_POLICY_EXISTS = 13000,
        /// <summary>
        /// The specified quick mode policy was not found.
        /// </summary>
        ERROR_IPSEC_QM_POLICY_NOT_FOUND = 13001,
        /// <summary>
        /// The specified quick mode policy is being used.
        /// </summary>
        ERROR_IPSEC_QM_POLICY_IN_USE = 13002,
        /// <summary>
        /// The specified main mode policy already exists.
        /// </summary>
        ERROR_IPSEC_MM_POLICY_EXISTS = 13003,
        /// <summary>
        /// The specified main mode policy was not found
        /// </summary>
        ERROR_IPSEC_MM_POLICY_NOT_FOUND = 13004,
        /// <summary>
        /// The specified main mode policy is being used.
        /// </summary>
        ERROR_IPSEC_MM_POLICY_IN_USE = 13005,
        /// <summary>
        /// The specified main mode filter already exists.
        /// </summary>
        ERROR_IPSEC_MM_FILTER_EXISTS = 13006,
        /// <summary>
        /// The specified main mode filter was not found.
        /// </summary>
        ERROR_IPSEC_MM_FILTER_NOT_FOUND = 13007,
        /// <summary>
        /// The specified transport mode filter already exists.
        /// </summary>
        ERROR_IPSEC_TRANSPORT_FILTER_EXISTS = 13008,
        /// <summary>
        /// The specified transport mode filter does not exist.
        /// </summary>
        ERROR_IPSEC_TRANSPORT_FILTER_NOT_FOUND = 13009,
        /// <summary>
        /// The specified main mode authentication list exists.
        /// </summary>
        ERROR_IPSEC_MM_AUTH_EXISTS = 13010,
        /// <summary>
        /// The specified main mode authentication list was not found.
        /// </summary>
        ERROR_IPSEC_MM_AUTH_NOT_FOUND = 13011,
        /// <summary>
        /// The specified quick mode policy is being used.
        /// </summary>
        ERROR_IPSEC_MM_AUTH_IN_USE = 13012,
        /// <summary>
        /// The specified main mode policy was not found.
        /// </summary>
        ERROR_IPSEC_DEFAULT_MM_POLICY_NOT_FOUND = 13013,
        /// <summary>
        /// The specified quick mode policy was not found
        /// </summary>
        ERROR_IPSEC_DEFAULT_MM_AUTH_NOT_FOUND = 13014,
        /// <summary>
        /// The manifest file contains one or more syntax errors.
        /// </summary>
        ERROR_IPSEC_DEFAULT_QM_POLICY_NOT_FOUND = 13015,
        /// <summary>
        /// The application attempted to activate a disabled activation context.
        /// </summary>
        ERROR_IPSEC_TUNNEL_FILTER_EXISTS = 13016,
        /// <summary>
        /// The requested lookup key was not found in any active activation context.
        /// </summary>
        ERROR_IPSEC_TUNNEL_FILTER_NOT_FOUND = 13017,
        /// <summary>
        /// The Main Mode filter is pending deletion.
        /// </summary>
        ERROR_IPSEC_MM_FILTER_PENDING_DELETION = 13018,
        /// <summary>
        /// The transport filter is pending deletion.
        /// </summary>
        ERROR_IPSEC_TRANSPORT_FILTER_PENDING_DELETION = 13019,
        /// <summary>
        /// The tunnel filter is pending deletion.
        /// </summary>
        ERROR_IPSEC_TUNNEL_FILTER_PENDING_DELETION = 13020,
        /// <summary>
        /// The Main Mode policy is pending deletion.
        /// </summary>
        ERROR_IPSEC_MM_POLICY_PENDING_DELETION = 13021,
        /// <summary>
        /// The Main Mode authentication bundle is pending deletion.
        /// </summary>
        ERROR_IPSEC_MM_AUTH_PENDING_DELETION = 13022,
        /// <summary>
        /// The Quick Mode policy is pending deletion.
        /// </summary>
        ERROR_IPSEC_QM_POLICY_PENDING_DELETION = 13023,
        /// <summary>
        /// No information avialable.
        /// </summary>
        WARNING_IPSEC_MM_POLICY_PRUNED = 13024,
        /// <summary>
        /// No information avialable.
        /// </summary>
        WARNING_IPSEC_QM_POLICY_PRUNED = 13025,
        /// <summary>
        /// ERROR_IPSEC_IKE_NEG_STATUS_BEGIN
        /// </summary>
        ERROR_IPSEC_IKE_NEG_STATUS_BEGIN = 13800,
        /// <summary>
        /// IKE authentication credentials are unacceptable
        /// </summary>
        ERROR_IPSEC_IKE_AUTH_FAIL = 13801,
        /// <summary>
        /// IKE security attributes are unacceptable
        /// </summary>
        ERROR_IPSEC_IKE_ATTRIB_FAIL = 13802,
        /// <summary>
        /// IKE Negotiation in progress
        /// </summary>
        ERROR_IPSEC_IKE_NEGOTIATION_PENDING = 13803,
        /// <summary>
        /// General processing error
        /// </summary>
        ERROR_IPSEC_IKE_GENERAL_PROCESSING_ERROR = 13804,
        /// <summary>
        /// Negotiation timed out
        /// </summary>
        ERROR_IPSEC_IKE_TIMED_OUT = 13805,
        /// <summary>
        /// IKE failed to find valid machine certificate
        /// </summary>
        ERROR_IPSEC_IKE_NO_CERT = 13806,
        /// <summary>
        /// IKE SA deleted by peer before establishment completed
        /// </summary>
        ERROR_IPSEC_IKE_SA_DELETED = 13807,
        /// <summary>
        /// IKE SA deleted before establishment completed
        /// </summary>
        ERROR_IPSEC_IKE_SA_REAPED = 13808,
        /// <summary>
        /// Negotiation request sat in Queue too long
        /// </summary>
        ERROR_IPSEC_IKE_MM_ACQUIRE_DROP = 13809,
        /// <summary>
        /// Negotiation request sat in Queue too long
        /// </summary>
        ERROR_IPSEC_IKE_QM_ACQUIRE_DROP = 13810,
        /// <summary>
        /// Negotiation request sat in Queue too long
        /// </summary>
        ERROR_IPSEC_IKE_QUEUE_DROP_MM = 13811,
        /// <summary>
        /// Negotiation request sat in Queue too long
        /// </summary>
        ERROR_IPSEC_IKE_QUEUE_DROP_NO_MM = 13812,
        /// <summary>
        /// No response from peer
        /// </summary>
        ERROR_IPSEC_IKE_DROP_NO_RESPONSE = 13813,
        /// <summary>
        /// Negotiation took too long
        /// </summary>
        ERROR_IPSEC_IKE_MM_DELAY_DROP = 13814,
        /// <summary>
        /// Negotiation took too long
        /// </summary>
        ERROR_IPSEC_IKE_QM_DELAY_DROP = 13815,
        /// <summary>
        /// Unknown error occurred
        /// </summary>
        ERROR_IPSEC_IKE_ERROR = 13816,
        /// <summary>
        /// Certificate Revocation Check failed
        /// </summary>
        ERROR_IPSEC_IKE_CRL_FAILED = 13817,
        /// <summary>
        /// Invalid certificate key usage
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_KEY_USAGE = 13818,
        /// <summary>
        /// Invalid certificate type
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_CERT_TYPE = 13819,
        /// <summary>
        /// No private key associated with machine certificate
        /// </summary>
        ERROR_IPSEC_IKE_NO_PRIVATE_KEY = 13820,
        /// <summary>
        /// Failure in Diffie-Helman computation
        /// </summary>
        ERROR_IPSEC_IKE_DH_FAIL = 13822,
        /// <summary>
        /// Invalid header
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_HEADER = 13824,
        /// <summary>
        /// No policy configured
        /// </summary>
        ERROR_IPSEC_IKE_NO_POLICY = 13825,
        /// <summary>
        /// Failed to verify signature
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_SIGNATURE = 13826,
        /// <summary>
        /// Failed to authenticate using kerberos
        /// </summary>
        ERROR_IPSEC_IKE_KERBEROS_ERROR = 13827,
        /// <summary>
        /// Peer's certificate did not have a public key
        /// </summary>
        ERROR_IPSEC_IKE_NO_PUBLIC_KEY = 13828,
        /// <summary>
        /// Error processing error payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR = 13829,
        /// <summary>
        /// Error processing SA payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_SA = 13830,
        /// <summary>
        /// Error processing Proposal payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_PROP = 13831,
        /// <summary>
        /// Error processing Transform payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_TRANS = 13832,
        /// <summary>
        /// Error processing KE payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_KE = 13833,
        /// <summary>
        /// Error processing ID payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_ID = 13834,
        /// <summary>
        /// Error processing Cert payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_CERT = 13835,
        /// <summary>
        /// Error processing Certificate Request payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_CERT_REQ = 13836,
        /// <summary>
        /// Error processing Hash payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_HASH = 13837,
        /// <summary>
        /// Error processing Signature payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_SIG = 13838,
        /// <summary>
        /// Error processing Nonce payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_NONCE = 13839,
        /// <summary>
        /// Error processing Notify payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_NOTIFY = 13840,
        /// <summary>
        /// Error processing Delete Payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_DELETE = 13841,
        /// <summary>
        /// Error processing VendorId payload
        /// </summary>
        ERROR_IPSEC_IKE_PROCESS_ERR_VENDOR = 13842,
        /// <summary>
        /// Invalid payload received
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_PAYLOAD = 13843,
        /// <summary>
        /// Soft SA loaded
        /// </summary>
        ERROR_IPSEC_IKE_LOAD_SOFT_SA = 13844,
        /// <summary>
        /// Soft SA torn down
        /// </summary>
        ERROR_IPSEC_IKE_SOFT_SA_TORN_DOWN = 13845,
        /// <summary>
        /// Invalid cookie received.
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_COOKIE = 13846,
        /// <summary>
        /// Peer failed to send valid machine certificate
        /// </summary>
        ERROR_IPSEC_IKE_NO_PEER_CERT = 13847,
        /// <summary>
        /// Certification Revocation check of peer's certificate failed
        /// </summary>
        ERROR_IPSEC_IKE_PEER_CRL_FAILED = 13848,
        /// <summary>
        /// New policy invalidated SAs formed with old policy
        /// </summary>
        ERROR_IPSEC_IKE_POLICY_CHANGE = 13849,
        /// <summary>
        /// There is no available Main Mode IKE policy.
        /// </summary>
        ERROR_IPSEC_IKE_NO_MM_POLICY = 13850,
        /// <summary>
        /// Failed to enabled TCB privilege.
        /// </summary>
        ERROR_IPSEC_IKE_NOTCBPRIV = 13851,
        /// <summary>
        /// Failed to load SECURITY.DLL.
        /// </summary>
        ERROR_IPSEC_IKE_SECLOADFAIL = 13852,
        /// <summary>
        /// Failed to obtain security function table dispatch address from SSPI.
        /// </summary>
        ERROR_IPSEC_IKE_FAILSSPINIT = 13853,
        /// <summary>
        /// Failed to query Kerberos package to obtain max token size.
        /// </summary>
        ERROR_IPSEC_IKE_FAILQUERYSSP = 13854,
        /// <summary>
        /// Failed to obtain Kerberos server credentials for ISAKMP/ERROR_IPSEC_IKE service.  Kerberos authentication will not function.  The most likely reason for this is lack of domain membership.  This is normal if your computer is a member of a workgroup.
        /// </summary>
        ERROR_IPSEC_IKE_SRVACQFAIL = 13855,
        /// <summary>
        /// Failed to determine SSPI principal name for ISAKMP/ERROR_IPSEC_IKE service (QueryCredentialsAttributes).
        /// </summary>
        ERROR_IPSEC_IKE_SRVQUERYCRED = 13856,
        /// <summary>
        /// Failed to obtain new SPI for the inbound SA from Ipsec driver.  The most common cause for this is that the driver does not have the correct filter.  Check your policy to verify the filters.
        /// </summary>
        ERROR_IPSEC_IKE_GETSPIFAIL = 13857,
        /// <summary>
        /// Given filter is invalid
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_FILTER = 13858,
        /// <summary>
        /// Memory allocation failed.
        /// </summary>
        ERROR_IPSEC_IKE_OUT_OF_MEMORY = 13859,
        /// <summary>
        /// Failed to add Security Association to IPSec Driver.  The most common cause for this is if the IKE negotiation took too long to complete.  If the problem persists, reduce the load on the faulting machine.
        /// </summary>
        ERROR_IPSEC_IKE_ADD_UPDATE_KEY_FAILED = 13860,
        /// <summary>
        /// Invalid policy
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_POLICY = 13861,
        /// <summary>
        /// Invalid DOI
        /// </summary>
        ERROR_IPSEC_IKE_UNKNOWN_DOI = 13862,
        /// <summary>
        /// Invalid situation
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_SITUATION = 13863,
        /// <summary>
        /// Diffie-Hellman failure
        /// </summary>
        ERROR_IPSEC_IKE_DH_FAILURE = 13864,
        /// <summary>
        /// Invalid Diffie-Hellman group
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_GROUP = 13865,
        /// <summary>
        /// Error encrypting payload
        /// </summary>
        ERROR_IPSEC_IKE_ENCRYPT = 13866,
        /// <summary>
        /// Error decrypting payload
        /// </summary>
        ERROR_IPSEC_IKE_DECRYPT = 13867,
        /// <summary>
        /// Policy match error
        /// </summary>
        ERROR_IPSEC_IKE_POLICY_MATCH = 13868,
        /// <summary>
        /// Unsupported ID
        /// </summary>
        ERROR_IPSEC_IKE_UNSUPPORTED_ID = 13869,
        /// <summary>
        /// Hash verification failed
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_HASH = 13870,
        /// <summary>
        /// Invalid hash algorithm
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_HASH_ALG = 13871,
        /// <summary>
        /// Invalid hash size
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_HASH_SIZE = 13872,
        /// <summary>
        /// Invalid encryption algorithm
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_ENCRYPT_ALG = 13873,
        /// <summary>
        /// Invalid authentication algorithm
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_AUTH_ALG = 13874,
        /// <summary>
        /// Invalid certificate signature
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_SIG = 13875,
        /// <summary>
        /// Load failed
        /// </summary>
        ERROR_IPSEC_IKE_LOAD_FAILED = 13876,
        /// <summary>
        /// Deleted via RPC call
        /// </summary>
        ERROR_IPSEC_IKE_RPC_DELETE = 13877,
        /// <summary>
        /// Temporary state created to perform reinit. This is not a real failure.
        /// </summary>
        ERROR_IPSEC_IKE_BENIGN_REINIT = 13878,
        /// <summary>
        /// The lifetime value received in the Responder Lifetime Notify is below the Windows 2000 configured minimum value.  Please fix the policy on the peer machine.
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_RESPONDER_LIFETIME_NOTIFY = 13879,
        /// <summary>
        /// Key length in certificate is too small for configured security requirements.
        /// </summary>
        ERROR_IPSEC_IKE_INVALID_CERT_KEYLEN = 13881,
        /// <summary>
        /// Max number of established MM SAs to peer exceeded.
        /// </summary>
        ERROR_IPSEC_IKE_MM_LIMIT = 13882,
        /// <summary>
        /// IKE received a policy that disables negotiation.
        /// </summary>
        ERROR_IPSEC_IKE_NEGOTIATION_DISABLED = 13883,
        /// <summary>
        /// ERROR_IPSEC_IKE_NEG_STATUS_END
        /// </summary>
        ERROR_IPSEC_IKE_NEG_STATUS_END = 13884,
    }

    /// <summary>
    /// COM error codes. Negative value is failure, positive is success.
    /// </summary>
    internal enum ResultCom
    {
        /// <summary>
        /// Not implemented
        /// </summary>
        E_NOTIMPL = (int)(0x80000001 - 0x100000000),
        /// <summary>
        /// Ran out of memory
        /// </summary>
        E_OUTOFMEMORY = (int)(0x80000002 - 0x100000000),
        /// <summary>
        /// One or more arguments are invalid
        /// </summary>
        E_INVALIDARG = (int)(0x80000003 - 0x100000000),
        /// <summary>
        /// No such interface supported
        /// </summary>
        E_NOINTERFACE = (int)(0x80000004 - 0x100000000),
        /// <summary>
        /// Invalid pointer
        /// </summary>
        E_POINTER = (int)(0x80000005 - 0x100000000),
        /// <summary>
        /// Invalid handle
        /// </summary>
        E_HANDLE = (int)(0x80000006 - 0x100000000),
        /// <summary>
        /// Operation aborted
        /// </summary>
        E_ABORT = (int)(0x80000007 - 0x100000000),
        /// <summary>
        /// Unspecified error
        /// </summary>
        E_FAIL = (int)(0x80000008 - 0x100000000),
        /// <summary>
        /// General access denied error
        /// </summary>
        E_ACCESSDENIED = (int)(0x80000009 - 0x100000000),
        /// <summary>
        /// The data necessary to complete this operation is not yet available.
        /// </summary>
        E_PENDING = (int)(0x8000000A - 0x100000000),
        /// <summary>
        /// Thread local storage failure
        /// </summary>
        CO_E_INIT_TLS = (int)(0x80004006 - 0x100000000),
        /// <summary>
        /// Get shared memory allocator failure
        /// </summary>
        CO_E_INIT_SHARED_ALLOCATOR = (int)(0x80004007 - 0x100000000),
        /// <summary>
        /// Get memory allocator failure
        /// </summary>
        CO_E_INIT_MEMORY_ALLOCATOR = (int)(0x80004008 - 0x100000000),
        /// <summary>
        /// Unable to initialize class cache
        /// </summary>
        CO_E_INIT_CLASS_CACHE = (int)(0x80004009 - 0x100000000),
        /// <summary>
        /// Unable to initialize RPC services
        /// </summary>
        CO_E_INIT_RPC_CHANNEL = (int)(0x8000400A - 0x100000000),
        /// <summary>
        /// Cannot set thread local storage channel control
        /// </summary>
        CO_E_INIT_TLS_SET_CHANNEL_CONTROL = (int)(0x8000400B - 0x100000000),
        /// <summary>
        /// Could not allocate thread local storage channel control
        /// </summary>
        CO_E_INIT_TLS_CHANNEL_CONTROL = (int)(0x8000400C - 0x100000000),
        /// <summary>
        /// The user supplied memory allocator is unacceptable
        /// </summary>
        CO_E_INIT_UNACCEPTED_USER_ALLOCATOR = (int)(0x8000400D - 0x100000000),
        /// <summary>
        /// The OLE service mutex already exists
        /// </summary>
        CO_E_INIT_SCM_MUTEX_EXISTS = (int)(0x8000400E - 0x100000000),
        /// <summary>
        /// The OLE service file mapping already exists
        /// </summary>
        CO_E_INIT_SCM_FILE_MAPPING_EXISTS = (int)(0x8000400F - 0x100000000),
        /// <summary>
        /// Unable to map view of file for OLE service
        /// </summary>
        CO_E_INIT_SCM_MAP_VIEW_OF_FILE = (int)(0x80004010 - 0x100000000),
        /// <summary>
        /// Failure attempting to launch OLE service
        /// </summary>
        CO_E_INIT_SCM_EXEC_FAILURE = (int)(0x80004011 - 0x100000000),
        /// <summary>
        /// There was an attempt to call CoInitialize a second time while single threaded
        /// </summary>
        CO_E_INIT_ONLY_SINGLE_THREADED = (int)(0x80004012 - 0x100000000),
        /// <summary>
        /// A Remote activation was necessary but was not allowed
        /// </summary>
        CO_E_CANT_REMOTE = (int)(0x80004013 - 0x100000000),
        /// <summary>
        /// A Remote activation was necessary but the server name provided was invalid
        /// </summary>
        CO_E_BAD_SERVER_NAME = (int)(0x80004014 - 0x100000000),
        /// <summary>
        /// The class is configured to run as a security id different from the caller
        /// </summary>
        CO_E_WRONG_SERVER_IDENTITY = (int)(0x80004015 - 0x100000000),
        /// <summary>
        /// Use of Ole1 services requiring DDE windows is disabled
        /// </summary>
        CO_E_OLE1DDE_DISABLED = (int)(0x80004016 - 0x100000000),
        /// <summary>
        /// A RunAs specification must be <domain name>\<user name> or simply <user name>
        /// </summary>
        CO_E_RUNAS_SYNTAX = (int)(0x80004017 - 0x100000000),
        /// <summary>
        /// The server process could not be started.  The pathname may be incorrect.
        /// </summary>
        CO_E_CREATEPROCESS_FAILURE = (int)(0x80004018 - 0x100000000),
        /// <summary>
        /// The server process could not be started as the configured identity.  The pathname may be incorrect or unavailable.
        /// </summary>
        CO_E_RUNAS_CREATEPROCESS_FAILURE = (int)(0x80004019 - 0x100000000),
        /// <summary>
        /// The server process could not be started because the configured identity is incorrect.  Check the username and password.
        /// </summary>
        CO_E_RUNAS_LOGON_FAILURE = (int)(0x8000401A - 0x100000000),
        /// <summary>
        /// The client is not allowed to launch this server.
        /// </summary>
        CO_E_LAUNCH_PERMSSION_DENIED = (int)(0x8000401B - 0x100000000),
        /// <summary>
        /// The service providing this server could not be started.
        /// </summary>
        CO_E_START_SERVICE_FAILURE = (int)(0x8000401C - 0x100000000),
        /// <summary>
        /// This computer was unable to communicate with the computer providing the server.
        /// </summary>
        CO_E_REMOTE_COMMUNICATION_FAILURE = (int)(0x8000401D - 0x100000000),
        /// <summary>
        /// The server did not respond after being launched.
        /// </summary>
        CO_E_SERVER_START_TIMEOUT = (int)(0x8000401E - 0x100000000),
        /// <summary>
        /// The registration information for this server is inconsistent or incomplete.
        /// </summary>
        CO_E_CLSREG_INCONSISTENT = (int)(0x8000401F - 0x100000000),
        /// <summary>
        /// The registration information for this interface is inconsistent or incomplete.
        /// </summary>
        CO_E_IIDREG_INCONSISTENT = (int)(0x80004020 - 0x100000000),
        /// <summary>
        /// The operation attempted is not supported.
        /// </summary>
        CO_E_NOT_SUPPORTED = (int)(0x80004021 - 0x100000000),
        /// <summary>
        /// A dll must be loaded.
        /// </summary>
        CO_E_RELOAD_DLL = (int)(0x80004022 - 0x100000000),
        /// <summary>
        /// A Microsoft Software Installer error was encountered.
        /// </summary>
        CO_E_MSI_ERROR = (int)(0x80004023 - 0x100000000),
        /// <summary>
        /// The specified activation could not occur in the client context as specified.
        /// </summary>
        CO_E_ATTEMPT_TO_CREATE_OUTSIDE_CLIENT_CONTEXT = (int)(0x80004024 - 0x100000000),
        /// <summary>
        /// Activations on the server are paused.
        /// </summary>
        CO_E_SERVER_PAUSED = (int)(0x80004025 - 0x100000000),
        /// <summary>
        /// Activations on the server are not paused.
        /// </summary>
        CO_E_SERVER_NOT_PAUSED = (int)(0x80004026 - 0x100000000),
        /// <summary>
        /// The component or application containing the component has been disabled.
        /// </summary>
        CO_E_CLASS_DISABLED = (int)(0x80004027 - 0x100000000),
        /// <summary>
        /// The common language runtime is not available
        /// </summary>
        CO_E_CLRNOTAVAILABLE = (int)(0x80004028 - 0x100000000),
        /// <summary>
        /// The thread-pool rejected the submitted asynchronous work.
        /// </summary>
        CO_E_ASYNC_WORK_REJECTED = (int)(0x80004029 - 0x100000000),
        /// <summary>
        /// The server started, but did not finish initializing in a timely fashion.
        /// </summary>
        CO_E_SERVER_INIT_TIMEOUT = (int)(0x8000402A - 0x100000000),
        /// <summary>
        /// Unable to complete the call since there is no COM+ security context inside IObjectControl.Activate.
        /// </summary>
        CO_E_NO_SECCTX_IN_ACTIVATE = (int)(0x8000402B - 0x100000000),
        /// <summary>
        /// The provided tracker configuration is invalid
        /// </summary>
        CO_E_TRACKER_CONFIG = (int)(0x80004030 - 0x100000000),
        /// <summary>
        /// The provided thread pool configuration is invalid
        /// </summary>
        CO_E_THREADPOOL_CONFIG = (int)(0x80004031 - 0x100000000),
        /// <summary>
        /// The provided side-by-side configuration is invalid
        /// </summary>
        CO_E_SXS_CONFIG = (int)(0x80004032 - 0x100000000),
        /// <summary>
        /// The server principal name (SPN) obtained during security negotiation is malformed.
        /// </summary>
        CO_E_MALFORMED_SPN = (int)(0x80004033 - 0x100000000),
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        S_OK = 0x00000000,
        /// <summary>
        /// Incorrect function.
        /// </summary>
        S_FALSE = 0x00000001,
        /// <summary>
        /// Invalid OLEVERB structure
        /// </summary>
        OLE_E_FIRST = (int)(0x80040000 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        OLE_E_LAST = (int)(0x800400FF - 0x100000000),
        /// <summary>
        /// Use the registry database to provide the requested information
        /// </summary>
        OLE_S_FIRST = 0x00040000,
        /// <summary>
        /// No information avialable.
        /// </summary>
        OLE_S_LAST = 0x000400FF,
        /// <summary>
        /// Invalid OLEVERB structure
        /// </summary>
        OLE_E_OLEVERB = (int)(0x80040000 - 0x100000000),
        /// <summary>
        /// Invalid advise flags
        /// </summary>
        OLE_E_ADVF = (int)(0x80040001 - 0x100000000),
        /// <summary>
        /// Can't enumerate any more, because the associated data is missing
        /// </summary>
        OLE_E_ENUM_NOMORE = (int)(0x80040002 - 0x100000000),
        /// <summary>
        /// This implementation doesn't take advises
        /// </summary>
        OLE_E_ADVISENOTSUPPORTED = (int)(0x80040003 - 0x100000000),
        /// <summary>
        /// There is no connection for this connection ID
        /// </summary>
        OLE_E_NOCONNECTION = (int)(0x80040004 - 0x100000000),
        /// <summary>
        /// Need to run the object to perform this operation
        /// </summary>
        OLE_E_NOTRUNNING = (int)(0x80040005 - 0x100000000),
        /// <summary>
        /// There is no cache to operate on
        /// </summary>
        OLE_E_NOCACHE = (int)(0x80040006 - 0x100000000),
        /// <summary>
        /// Uninitialized object
        /// </summary>
        OLE_E_BLANK = (int)(0x80040007 - 0x100000000),
        /// <summary>
        /// Linked object's source class has changed
        /// </summary>
        OLE_E_CLASSDIFF = (int)(0x80040008 - 0x100000000),
        /// <summary>
        /// Not able to get the moniker of the object
        /// </summary>
        OLE_E_CANT_GETMONIKER = (int)(0x80040009 - 0x100000000),
        /// <summary>
        /// Not able to bind to the source
        /// </summary>
        OLE_E_CANT_BINDTOSOURCE = (int)(0x8004000A - 0x100000000),
        /// <summary>
        /// Object is static, operation not allowed
        /// </summary>
        OLE_E_STATIC = (int)(0x8004000B - 0x100000000),
        /// <summary>
        /// User canceled out of save dialog
        /// </summary>
        OLE_E_PROMPTSAVECANCELLED = (int)(0x8004000C - 0x100000000),
        /// <summary>
        /// Invalid rectangle
        /// </summary>
        OLE_E_INVALIDRECT = (int)(0x8004000D - 0x100000000),
        /// <summary>
        /// compobj.dll is too old for the ole2.dll initialized
        /// </summary>
        OLE_E_WRONGCOMPOBJ = (int)(0x8004000E - 0x100000000),
        /// <summary>
        /// Invalid window handle
        /// </summary>
        OLE_E_INVALIDHWND = (int)(0x8004000F - 0x100000000),
        /// <summary>
        /// Object is not in any of the inplace active states
        /// </summary>
        OLE_E_NOT_INPLACEACTIVE = (int)(0x80040010 - 0x100000000),
        /// <summary>
        /// Not able to convert object
        /// </summary>
        OLE_E_CANTCONVERT = (int)(0x80040011 - 0x100000000),
        /// <summary>
        /// Not able to perform the operation because object is not given storage yet
        /// </summary>
        OLE_E_NOSTORAGE = (int)(0x80040012 - 0x100000000),
        /// <summary>
        /// Invalid FORMATETC structure
        /// </summary>
        DV_E_FORMATETC = (int)(0x80040064 - 0x100000000),
        /// <summary>
        /// Invalid DVTARGETDEVICE structure
        /// </summary>
        DV_E_DVTARGETDEVICE = (int)(0x80040065 - 0x100000000),
        /// <summary>
        /// Invalid STDGMEDIUM structure
        /// </summary>
        DV_E_STGMEDIUM = (int)(0x80040066 - 0x100000000),
        /// <summary>
        /// Invalid STATDATA structure
        /// </summary>
        DV_E_STATDATA = (int)(0x80040067 - 0x100000000),
        /// <summary>
        /// Invalid lindex
        /// </summary>
        DV_E_LINDEX = (int)(0x80040068 - 0x100000000),
        /// <summary>
        /// Invalid tymed
        /// </summary>
        DV_E_TYMED = (int)(0x80040069 - 0x100000000),
        /// <summary>
        /// Invalid clipboard format
        /// </summary>
        DV_E_CLIPFORMAT = (int)(0x8004006A - 0x100000000),
        /// <summary>
        /// Invalid aspect(s)
        /// </summary>
        DV_E_DVASPECT = (int)(0x8004006B - 0x100000000),
        /// <summary>
        /// tdSize parameter of the DVTARGETDEVICE structure is invalid
        /// </summary>
        DV_E_DVTARGETDEVICE_SIZE = (int)(0x8004006C - 0x100000000),
        /// <summary>
        /// Object doesn't support IViewObject interface
        /// </summary>
        DV_E_NOIVIEWOBJECT = (int)(0x8004006D - 0x100000000),
        /// <summary>
        /// Trying to revoke a drop target that has not been registered
        /// </summary>
        DRAGDROP_E_FIRST = (int)(0x80040100 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        DRAGDROP_E_LAST = (int)(0x8004010F - 0x100000000),
        /// <summary>
        /// Successful drop took place
        /// </summary>
        DRAGDROP_S_FIRST = 0x00040100,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DRAGDROP_S_LAST = 0x0004010F,
        /// <summary>
        /// Trying to revoke a drop target that has not been registered
        /// </summary>
        DRAGDROP_E_NOTREGISTERED = (int)(0x80040100 - 0x100000000),
        /// <summary>
        /// This window has already been registered as a drop target
        /// </summary>
        DRAGDROP_E_ALREADYREGISTERED = (int)(0x80040101 - 0x100000000),
        /// <summary>
        /// Invalid window handle
        /// </summary>
        DRAGDROP_E_INVALIDHWND = (int)(0x80040102 - 0x100000000),
        /// <summary>
        /// Class does not support aggregation (or class object is remote)
        /// </summary>
        CLASSFACTORY_E_FIRST = (int)(0x80040110 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLASSFACTORY_E_LAST = (int)(0x8004011F - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLASSFACTORY_S_FIRST = 0x00040110,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLASSFACTORY_S_LAST = 0x0004011F,
        /// <summary>
        /// Class does not support aggregation (or class object is remote)
        /// </summary>
        CLASS_E_NOAGGREGATION = (int)(0x80040110 - 0x100000000),
        /// <summary>
        /// ClassFactory cannot supply requested class
        /// </summary>
        CLASS_E_CLASSNOTAVAILABLE = (int)(0x80040111 - 0x100000000),
        /// <summary>
        /// Class is not licensed for use
        /// </summary>
        CLASS_E_NOTLICENSED = (int)(0x80040112 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        MARSHAL_E_FIRST = (int)(0x80040120 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        MARSHAL_E_LAST = (int)(0x8004012F - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        MARSHAL_S_FIRST = 0x00040120,
        /// <summary>
        /// No information avialable.
        /// </summary>
        MARSHAL_S_LAST = 0x0004012F,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DATA_E_FIRST = (int)(0x80040130 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        DATA_E_LAST = (int)(0x8004013F - 0x100000000),
        /// <summary>
        /// Data has same FORMATETC
        /// </summary>
        DATA_S_FIRST = 0x00040130,
        /// <summary>
        /// No information avialable.
        /// </summary>
        DATA_S_LAST = 0x0004013F,
        /// <summary>
        /// Error drawing view
        /// </summary>
        VIEW_E_FIRST = (int)(0x80040140 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        VIEW_E_LAST = (int)(0x8004014F - 0x100000000),
        /// <summary>
        /// View is already frozen
        /// </summary>
        VIEW_S_FIRST = 0x00040140,
        /// <summary>
        /// No information avialable.
        /// </summary>
        VIEW_S_LAST = 0x0004014F,
        /// <summary>
        /// Error drawing view
        /// </summary>
        VIEW_E_DRAW = (int)(0x80040140 - 0x100000000),
        /// <summary>
        /// Could not read key from registry
        /// </summary>
        REGDB_E_FIRST = (int)(0x80040150 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        REGDB_E_LAST = (int)(0x8004015F - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        REGDB_S_FIRST = 0x00040150,
        /// <summary>
        /// No information avialable.
        /// </summary>
        REGDB_S_LAST = 0x0004015F,
        /// <summary>
        /// Could not read key from registry
        /// </summary>
        REGDB_E_READREGDB = (int)(0x80040150 - 0x100000000),
        /// <summary>
        /// Could not write key to registry
        /// </summary>
        REGDB_E_WRITEREGDB = (int)(0x80040151 - 0x100000000),
        /// <summary>
        /// Could not find the key in the registry
        /// </summary>
        REGDB_E_KEYMISSING = (int)(0x80040152 - 0x100000000),
        /// <summary>
        /// Invalid value for registry
        /// </summary>
        REGDB_E_INVALIDVALUE = (int)(0x80040153 - 0x100000000),
        /// <summary>
        /// Class not registered
        /// </summary>
        REGDB_E_CLASSNOTREG = (int)(0x80040154 - 0x100000000),
        /// <summary>
        /// Interface not registered
        /// </summary>
        REGDB_E_IIDNOTREG = (int)(0x80040155 - 0x100000000),
        /// <summary>
        /// Threading model entry is not valid
        /// </summary>
        REGDB_E_BADTHREADINGMODEL = (int)(0x80040156 - 0x100000000),
        /// <summary>
        /// CATID does not exist
        /// </summary>
        CAT_E_FIRST = (int)(0x80040160 - 0x100000000),
        /// <summary>
        /// Description not found
        /// </summary>
        CAT_E_LAST = (int)(0x80040161 - 0x100000000),
        /// <summary>
        /// CATID does not exist
        /// </summary>
        CAT_E_CATIDNOEXIST = (int)(0x80040160 - 0x100000000),
        /// <summary>
        /// Description not found
        /// </summary>
        CAT_E_NODESCRIPTION = (int)(0x80040161 - 0x100000000),
        /// <summary>
        /// No package in the software installation data in the Active Directory meets this criteria.
        /// </summary>
        CS_E_FIRST = (int)(0x80040164 - 0x100000000),
        /// <summary>
        /// An error occurred in the software installation data in the Active Directory.
        /// </summary>
        CS_E_LAST = (int)(0x8004016F - 0x100000000),
        /// <summary>
        /// No package in the software installation data in the Active Directory meets this criteria.
        /// </summary>
        CS_E_PACKAGE_NOTFOUND = (int)(0x80040164 - 0x100000000),
        /// <summary>
        /// Deleting this will break the referential integrity of the software installation data in the Active Directory.
        /// </summary>
        CS_E_NOT_DELETABLE = (int)(0x80040165 - 0x100000000),
        /// <summary>
        /// The CLSID was not found in the software installation data in the Active Directory.
        /// </summary>
        CS_E_CLASS_NOTFOUND = (int)(0x80040166 - 0x100000000),
        /// <summary>
        /// The software installation data in the Active Directory is corrupt.
        /// </summary>
        CS_E_INVALID_VERSION = (int)(0x80040167 - 0x100000000),
        /// <summary>
        /// There is no software installation data in the Active Directory.
        /// </summary>
        CS_E_NO_CLASSSTORE = (int)(0x80040168 - 0x100000000),
        /// <summary>
        /// There is no software installation data object in the Active Directory.
        /// </summary>
        CS_E_OBJECT_NOTFOUND = (int)(0x80040169 - 0x100000000),
        /// <summary>
        /// The software installation data object in the Active Directory already exists.
        /// </summary>
        CS_E_OBJECT_ALREADY_EXISTS = (int)(0x8004016A - 0x100000000),
        /// <summary>
        /// The path to the software installation data in the Active Directory is not correct.
        /// </summary>
        CS_E_INVALID_PATH = (int)(0x8004016B - 0x100000000),
        /// <summary>
        /// A network error interrupted the operation.
        /// </summary>
        CS_E_NETWORK_ERROR = (int)(0x8004016C - 0x100000000),
        /// <summary>
        /// The size of this object exceeds the maximum size set by the Administrator.
        /// </summary>
        CS_E_ADMIN_LIMIT_EXCEEDED = (int)(0x8004016D - 0x100000000),
        /// <summary>
        /// The schema for the software installation data in the Active Directory does not match the required schema.
        /// </summary>
        CS_E_SCHEMA_MISMATCH = (int)(0x8004016E - 0x100000000),
        /// <summary>
        /// An error occurred in the software installation data in the Active Directory.
        /// </summary>
        CS_E_INTERNAL_ERROR = (int)(0x8004016F - 0x100000000),
        /// <summary>
        /// Cache not updated
        /// </summary>
        CACHE_E_FIRST = (int)(0x80040170 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CACHE_E_LAST = (int)(0x8004017F - 0x100000000),
        /// <summary>
        /// FORMATETC not supported
        /// </summary>
        CACHE_S_FIRST = 0x00040170,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CACHE_S_LAST = 0x0004017F,
        /// <summary>
        /// Cache not updated
        /// </summary>
        CACHE_E_NOCACHE_UPDATED = (int)(0x80040170 - 0x100000000),
        /// <summary>
        /// No verbs for OLE object
        /// </summary>
        OLEOBJ_E_FIRST = (int)(0x80040180 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        OLEOBJ_E_LAST = (int)(0x8004018F - 0x100000000),
        /// <summary>
        /// Invalid verb for OLE object
        /// </summary>
        OLEOBJ_S_FIRST = 0x00040180,
        /// <summary>
        /// No information avialable.
        /// </summary>
        OLEOBJ_S_LAST = 0x0004018F,
        /// <summary>
        /// No verbs for OLE object
        /// </summary>
        OLEOBJ_E_NOVERBS = (int)(0x80040180 - 0x100000000),
        /// <summary>
        /// Invalid verb for OLE object
        /// </summary>
        OLEOBJ_E_INVALIDVERB = (int)(0x80040181 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIENTSITE_E_FIRST = (int)(0x80040190 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIENTSITE_E_LAST = (int)(0x8004019F - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIENTSITE_S_FIRST = 0x00040190,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIENTSITE_S_LAST = 0x0004019F,
        /// <summary>
        /// Undo is not available
        /// </summary>
        INPLACE_E_NOTUNDOABLE = (int)(0x800401A0 - 0x100000000),
        /// <summary>
        /// Space for tools is not available
        /// </summary>
        INPLACE_E_NOTOOLSPACE = (int)(0x800401A1 - 0x100000000),
        /// <summary>
        /// Undo is not available
        /// </summary>
        INPLACE_E_FIRST = (int)(0x800401A0 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        INPLACE_E_LAST = (int)(0x800401AF - 0x100000000),
        /// <summary>
        /// Message is too long, some of it had to be truncated before displaying
        /// </summary>
        INPLACE_S_FIRST = 0x000401A0,
        /// <summary>
        /// No information avialable.
        /// </summary>
        INPLACE_S_LAST = 0x000401AF,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ENUM_E_FIRST = (int)(0x800401B0 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        ENUM_E_LAST = (int)(0x800401BF - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        ENUM_S_FIRST = 0x000401B0,
        /// <summary>
        /// No information avialable.
        /// </summary>
        ENUM_S_LAST = 0x000401BF,
        /// <summary>
        /// OLESTREAM Get method failed
        /// </summary>
        CONVERT10_E_FIRST = (int)(0x800401C0 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CONVERT10_E_LAST = (int)(0x800401CF - 0x100000000),
        /// <summary>
        /// Unable to convert OLESTREAM to IStorage
        /// </summary>
        CONVERT10_S_FIRST = 0x000401C0,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CONVERT10_S_LAST = 0x000401CF,
        /// <summary>
        /// OLESTREAM Get method failed
        /// </summary>
        CONVERT10_E_OLESTREAM_GET = (int)(0x800401C0 - 0x100000000),
        /// <summary>
        /// OLESTREAM Put method failed
        /// </summary>
        CONVERT10_E_OLESTREAM_PUT = (int)(0x800401C1 - 0x100000000),
        /// <summary>
        /// Contents of the OLESTREAM not in correct format
        /// </summary>
        CONVERT10_E_OLESTREAM_FMT = (int)(0x800401C2 - 0x100000000),
        /// <summary>
        /// There was an error in a Windows GDI call while converting the bitmap to a DIB
        /// </summary>
        CONVERT10_E_OLESTREAM_BITMAP_TO_DIB = (int)(0x800401C3 - 0x100000000),
        /// <summary>
        /// Contents of the IStorage not in correct format
        /// </summary>
        CONVERT10_E_STG_FMT = (int)(0x800401C4 - 0x100000000),
        /// <summary>
        /// Contents of IStorage is missing one of the standard streams
        /// </summary>
        CONVERT10_E_STG_NO_STD_STREAM = (int)(0x800401C5 - 0x100000000),
        /// <summary>
        /// There was an error in a Windows GDI call while converting the DIB to a bitmap.
        /// </summary>
        CONVERT10_E_STG_DIB_TO_BITMAP = (int)(0x800401C6 - 0x100000000),
        /// <summary>
        /// OpenClipboard Failed
        /// </summary>
        CLIPBRD_E_FIRST = (int)(0x800401D0 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIPBRD_E_LAST = (int)(0x800401DF - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIPBRD_S_FIRST = 0x000401D0,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CLIPBRD_S_LAST = 0x000401DF,
        /// <summary>
        /// OpenClipboard Failed
        /// </summary>
        CLIPBRD_E_CANT_OPEN = (int)(0x800401D0 - 0x100000000),
        /// <summary>
        /// EmptyClipboard Failed
        /// </summary>
        CLIPBRD_E_CANT_EMPTY = (int)(0x800401D1 - 0x100000000),
        /// <summary>
        /// SetClipboard Failed
        /// </summary>
        CLIPBRD_E_CANT_SET = (int)(0x800401D2 - 0x100000000),
        /// <summary>
        /// Data on clipboard is invalid
        /// </summary>
        CLIPBRD_E_BAD_DATA = (int)(0x800401D3 - 0x100000000),
        /// <summary>
        /// CloseClipboard Failed
        /// </summary>
        CLIPBRD_E_CANT_CLOSE = (int)(0x800401D4 - 0x100000000),
        /// <summary>
        /// Moniker needs to be connected manually
        /// </summary>
        MK_E_FIRST = (int)(0x800401E0 - 0x100000000),
        /// <summary>
        /// Moniker could not be enumerated
        /// </summary>
        MK_E_LAST = (int)(0x800401EF - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        MK_S_FIRST = 0x000401E0,
        /// <summary>
        /// No information avialable.
        /// </summary>
        MK_S_LAST = 0x000401EF,
        /// <summary>
        /// Moniker needs to be connected manually
        /// </summary>
        MK_E_CONNECTMANUALLY = (int)(0x800401E0 - 0x100000000),
        /// <summary>
        /// Operation exceeded deadline
        /// </summary>
        MK_E_EXCEEDEDDEADLINE = (int)(0x800401E1 - 0x100000000),
        /// <summary>
        /// Moniker needs to be generic
        /// </summary>
        MK_E_NEEDGENERIC = (int)(0x800401E2 - 0x100000000),
        /// <summary>
        /// Operation unavailable
        /// </summary>
        MK_E_UNAVAILABLE = (int)(0x800401E3 - 0x100000000),
        /// <summary>
        /// Invalid syntax
        /// </summary>
        MK_E_SYNTAX = (int)(0x800401E4 - 0x100000000),
        /// <summary>
        /// No object for moniker
        /// </summary>
        MK_E_NOOBJECT = (int)(0x800401E5 - 0x100000000),
        /// <summary>
        /// Bad extension for file
        /// </summary>
        MK_E_INVALIDEXTENSION = (int)(0x800401E6 - 0x100000000),
        /// <summary>
        /// Intermediate operation failed
        /// </summary>
        MK_E_INTERMEDIATEINTERFACENOTSUPPORTED = (int)(0x800401E7 - 0x100000000),
        /// <summary>
        /// Moniker is not bindable
        /// </summary>
        MK_E_NOTBINDABLE = (int)(0x800401E8 - 0x100000000),
        /// <summary>
        /// Moniker is not bound
        /// </summary>
        MK_E_NOTBOUND = (int)(0x800401E9 - 0x100000000),
        /// <summary>
        /// Moniker cannot open file
        /// </summary>
        MK_E_CANTOPENFILE = (int)(0x800401EA - 0x100000000),
        /// <summary>
        /// User input required for operation to succeed
        /// </summary>
        MK_E_MUSTBOTHERUSER = (int)(0x800401EB - 0x100000000),
        /// <summary>
        /// Moniker class has no inverse
        /// </summary>
        MK_E_NOINVERSE = (int)(0x800401EC - 0x100000000),
        /// <summary>
        /// Moniker does not refer to storage
        /// </summary>
        MK_E_NOSTORAGE = (int)(0x800401ED - 0x100000000),
        /// <summary>
        /// No common prefix
        /// </summary>
        MK_E_NOPREFIX = (int)(0x800401EE - 0x100000000),
        /// <summary>
        /// Moniker could not be enumerated
        /// </summary>
        MK_E_ENUMERATION_FAILED = (int)(0x800401EF - 0x100000000),
        /// <summary>
        /// CoInitialize has not been called.
        /// </summary>
        CO_E_FIRST = (int)(0x800401F0 - 0x100000000),
        /// <summary>
        /// Object has been released
        /// </summary>
        CO_E_LAST = (int)(0x800401FF - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CO_S_FIRST = 0x000401F0,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CO_S_LAST = 0x000401FF,
        /// <summary>
        /// CoInitialize has not been called.
        /// </summary>
        CO_E_NOTINITIALIZED = (int)(0x800401F0 - 0x100000000),
        /// <summary>
        /// CoInitialize has already been called.
        /// </summary>
        CO_E_ALREADYINITIALIZED = (int)(0x800401F1 - 0x100000000),
        /// <summary>
        /// Class of object cannot be determined
        /// </summary>
        CO_E_CANTDETERMINECLASS = (int)(0x800401F2 - 0x100000000),
        /// <summary>
        /// Invalid class string
        /// </summary>
        CO_E_CLASSSTRING = (int)(0x800401F3 - 0x100000000),
        /// <summary>
        /// Invalid interface string
        /// </summary>
        CO_E_IIDSTRING = (int)(0x800401F4 - 0x100000000),
        /// <summary>
        /// Application not found
        /// </summary>
        CO_E_APPNOTFOUND = (int)(0x800401F5 - 0x100000000),
        /// <summary>
        /// Application cannot be run more than once
        /// </summary>
        CO_E_APPSINGLEUSE = (int)(0x800401F6 - 0x100000000),
        /// <summary>
        /// Some error in application program
        /// </summary>
        CO_E_ERRORINAPP = (int)(0x800401F7 - 0x100000000),
        /// <summary>
        /// DLL for class not found
        /// </summary>
        CO_E_DLLNOTFOUND = (int)(0x800401F8 - 0x100000000),
        /// <summary>
        /// Error in the DLL
        /// </summary>
        CO_E_ERRORINDLL = (int)(0x800401F9 - 0x100000000),
        /// <summary>
        /// Wrong OS or OS version for application
        /// </summary>
        CO_E_WRONGOSFORAPP = (int)(0x800401FA - 0x100000000),
        /// <summary>
        /// Object is not registered
        /// </summary>
        CO_E_OBJNOTREG = (int)(0x800401FB - 0x100000000),
        /// <summary>
        /// Object is already registered
        /// </summary>
        CO_E_OBJISREG = (int)(0x800401FC - 0x100000000),
        /// <summary>
        /// Object is not connected to server
        /// </summary>
        CO_E_OBJNOTCONNECTED = (int)(0x800401FD - 0x100000000),
        /// <summary>
        /// Application was launched but it didn't register a class factory
        /// </summary>
        CO_E_APPDIDNTREG = (int)(0x800401FE - 0x100000000),
        /// <summary>
        /// Object has been released
        /// </summary>
        CO_E_RELEASED = (int)(0x800401FF - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        EVENT_E_FIRST = (int)(0x80040200 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        EVENT_E_LAST = (int)(0x8004021F - 0x100000000),
        /// <summary>
        /// An event was able to invoke some but not all of the subscribers
        /// </summary>
        EVENT_S_FIRST = 0x00040200,
        /// <summary>
        /// No information avialable.
        /// </summary>
        EVENT_S_LAST = 0x0004021F,
        /// <summary>
        /// An event was able to invoke some but not all of the subscribers
        /// </summary>
        EVENT_S_SOME_SUBSCRIBERS_FAILED = 0x00040200,
        /// <summary>
        /// An event was unable to invoke any of the subscribers
        /// </summary>
        EVENT_E_ALL_SUBSCRIBERS_FAILED = (int)(0x80040201 - 0x100000000),
        /// <summary>
        /// An event was delivered but there were no subscribers
        /// </summary>
        EVENT_S_NOSUBSCRIBERS = 0x00040202,
        /// <summary>
        /// A syntax error occurred trying to evaluate a query string
        /// </summary>
        EVENT_E_QUERYSYNTAX = (int)(0x80040203 - 0x100000000),
        /// <summary>
        /// An invalid field name was used in a query string
        /// </summary>
        EVENT_E_QUERYFIELD = (int)(0x80040204 - 0x100000000),
        /// <summary>
        /// An unexpected exception was raised
        /// </summary>
        EVENT_E_INTERNALEXCEPTION = (int)(0x80040205 - 0x100000000),
        /// <summary>
        /// An unexpected internal error was detected
        /// </summary>
        EVENT_E_INTERNALERROR = (int)(0x80040206 - 0x100000000),
        /// <summary>
        /// The owner SID on a per-user subscription doesn't exist
        /// </summary>
        EVENT_E_INVALID_PER_USER_SID = (int)(0x80040207 - 0x100000000),
        /// <summary>
        /// A user-supplied component or subscriber raised an exception
        /// </summary>
        EVENT_E_USER_EXCEPTION = (int)(0x80040208 - 0x100000000),
        /// <summary>
        /// An interface has too many methods to fire events from
        /// </summary>
        EVENT_E_TOO_MANY_METHODS = (int)(0x80040209 - 0x100000000),
        /// <summary>
        /// A subscription cannot be stored unless its event class already exists
        /// </summary>
        EVENT_E_MISSING_EVENTCLASS = (int)(0x8004020A - 0x100000000),
        /// <summary>
        /// Not all the objects requested could be removed
        /// </summary>
        EVENT_E_NOT_ALL_REMOVED = (int)(0x8004020B - 0x100000000),
        /// <summary>
        /// COM+ is required for this operation, but is not installed
        /// </summary>
        EVENT_E_COMPLUS_NOT_INSTALLED = (int)(0x8004020C - 0x100000000),
        /// <summary>
        /// Cannot modify or delete an object that was not added using the COM+ Admin SDK
        /// </summary>
        EVENT_E_CANT_MODIFY_OR_DELETE_UNCONFIGURED_OBJECT = (int)(0x8004020D - 0x100000000),
        /// <summary>
        /// Cannot modify or delete an object that was added using the COM+ Admin SDK
        /// </summary>
        EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT = (int)(0x8004020E - 0x100000000),
        /// <summary>
        /// The event class for this subscription is in an invalid partition
        /// </summary>
        EVENT_E_INVALID_EVENT_CLASS_PARTITION = (int)(0x8004020F - 0x100000000),
        /// <summary>
        /// The owner of the PerUser subscription is not logged on to the system specified
        /// </summary>
        EVENT_E_PER_USER_SID_NOT_LOGGED_ON = (int)(0x80040210 - 0x100000000),
        /// <summary>
        /// Another single phase resource manager has already been enlisted in this transaction.
        /// </summary>
        XACT_E_FIRST = (int)(0x8004D000 - 0x100000000),
        /// <summary>
        /// The local transaction has aborted.
        /// </summary>
        XACT_E_LAST = (int)(0x8004D029 - 0x100000000),
        /// <summary>
        /// An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.
        /// </summary>
        XACT_S_FIRST = 0x0004D000,
        /// <summary>
        /// The resource manager has requested to be the coordinator (last resource manager) for the transaction.
        /// </summary>
        XACT_S_LAST = 0x0004D010,
        /// <summary>
        /// Another single phase resource manager has already been enlisted in this transaction.
        /// </summary>
        XACT_E_ALREADYOTHERSINGLEPHASE = (int)(0x8004D000 - 0x100000000),
        /// <summary>
        /// A retaining commit or abort is not supported
        /// </summary>
        XACT_E_CANTRETAIN = (int)(0x8004D001 - 0x100000000),
        /// <summary>
        /// The transaction failed to commit for an unknown reason. The transaction was aborted.
        /// </summary>
        XACT_E_COMMITFAILED = (int)(0x8004D002 - 0x100000000),
        /// <summary>
        /// Cannot call commit on this transaction object because the calling application did not initiate the transaction.
        /// </summary>
        XACT_E_COMMITPREVENTED = (int)(0x8004D003 - 0x100000000),
        /// <summary>
        /// Instead of committing, the resource heuristically aborted.
        /// </summary>
        XACT_E_HEURISTICABORT = (int)(0x8004D004 - 0x100000000),
        /// <summary>
        /// Instead of aborting, the resource heuristically committed.
        /// </summary>
        XACT_E_HEURISTICCOMMIT = (int)(0x8004D005 - 0x100000000),
        /// <summary>
        /// Some of the states of the resource were committed while others were aborted, likely because of heuristic decisions.
        /// </summary>
        XACT_E_HEURISTICDAMAGE = (int)(0x8004D006 - 0x100000000),
        /// <summary>
        /// Some of the states of the resource may have been committed while others may have been aborted, likely because of heuristic decisions.
        /// </summary>
        XACT_E_HEURISTICDANGER = (int)(0x8004D007 - 0x100000000),
        /// <summary>
        /// The requested isolation level is not valid or supported.
        /// </summary>
        XACT_E_ISOLATIONLEVEL = (int)(0x8004D008 - 0x100000000),
        /// <summary>
        /// The transaction manager doesn't support an asynchronous operation for this method.
        /// </summary>
        XACT_E_NOASYNC = (int)(0x8004D009 - 0x100000000),
        /// <summary>
        /// Unable to enlist in the transaction.
        /// </summary>
        XACT_E_NOENLIST = (int)(0x8004D00A - 0x100000000),
        /// <summary>
        /// The requested semantics of retention of isolation across retaining commit and abort boundaries cannot be supported by this transaction implementation, or isoFlags was not equal to zero.
        /// </summary>
        XACT_E_NOISORETAIN = (int)(0x8004D00B - 0x100000000),
        /// <summary>
        /// There is no resource presently associated with this enlistment
        /// </summary>
        XACT_E_NORESOURCE = (int)(0x8004D00C - 0x100000000),
        /// <summary>
        /// The transaction failed to commit due to the failure of optimistic concurrency control in at least one of the resource managers.
        /// </summary>
        XACT_E_NOTCURRENT = (int)(0x8004D00D - 0x100000000),
        /// <summary>
        /// The transaction has already been implicitly or explicitly committed or aborted
        /// </summary>
        XACT_E_NOTRANSACTION = (int)(0x8004D00E - 0x100000000),
        /// <summary>
        /// An invalid combination of flags was specified
        /// </summary>
        XACT_E_NOTSUPPORTED = (int)(0x8004D00F - 0x100000000),
        /// <summary>
        /// The resource manager id is not associated with this transaction or the transaction manager.
        /// </summary>
        XACT_E_UNKNOWNRMGRID = (int)(0x8004D010 - 0x100000000),
        /// <summary>
        /// This method was called in the wrong state
        /// </summary>
        XACT_E_WRONGSTATE = (int)(0x8004D011 - 0x100000000),
        /// <summary>
        /// The indicated unit of work does not match the unit of work expected by the resource manager.
        /// </summary>
        XACT_E_WRONGUOW = (int)(0x8004D012 - 0x100000000),
        /// <summary>
        /// An enlistment in a transaction already exists.
        /// </summary>
        XACT_E_XTIONEXISTS = (int)(0x8004D013 - 0x100000000),
        /// <summary>
        /// An import object for the transaction could not be found.
        /// </summary>
        XACT_E_NOIMPORTOBJECT = (int)(0x8004D014 - 0x100000000),
        /// <summary>
        /// The transaction cookie is invalid.
        /// </summary>
        XACT_E_INVALIDCOOKIE = (int)(0x8004D015 - 0x100000000),
        /// <summary>
        /// The transaction status is in doubt. A communication failure occurred, or a transaction manager or resource manager has failed
        /// </summary>
        XACT_E_INDOUBT = (int)(0x8004D016 - 0x100000000),
        /// <summary>
        /// A time-out was specified, but time-outs are not supported.
        /// </summary>
        XACT_E_NOTIMEOUT = (int)(0x8004D017 - 0x100000000),
        /// <summary>
        /// The requested operation is already in progress for the transaction.
        /// </summary>
        XACT_E_ALREADYINPROGRESS = (int)(0x8004D018 - 0x100000000),
        /// <summary>
        /// The transaction has already been aborted.
        /// </summary>
        XACT_E_ABORTED = (int)(0x8004D019 - 0x100000000),
        /// <summary>
        /// The Transaction Manager returned a log full error.
        /// </summary>
        XACT_E_LOGFULL = (int)(0x8004D01A - 0x100000000),
        /// <summary>
        /// The Transaction Manager is not available.
        /// </summary>
        XACT_E_TMNOTAVAILABLE = (int)(0x8004D01B - 0x100000000),
        /// <summary>
        /// A connection with the transaction manager was lost.
        /// </summary>
        XACT_E_CONNECTION_DOWN = (int)(0x8004D01C - 0x100000000),
        /// <summary>
        /// A request to establish a connection with the transaction manager was denied.
        /// </summary>
        XACT_E_CONNECTION_DENIED = (int)(0x8004D01D - 0x100000000),
        /// <summary>
        /// Resource manager reenlistment to determine transaction status timed out.
        /// </summary>
        XACT_E_REENLISTTIMEOUT = (int)(0x8004D01E - 0x100000000),
        /// <summary>
        /// This transaction manager failed to establish a connection with another TIP transaction manager.
        /// </summary>
        XACT_E_TIP_CONNECT_FAILED = (int)(0x8004D01F - 0x100000000),
        /// <summary>
        /// This transaction manager encountered a protocol error with another TIP transaction manager.
        /// </summary>
        XACT_E_TIP_PROTOCOL_ERROR = (int)(0x8004D020 - 0x100000000),
        /// <summary>
        /// This transaction manager could not propagate a transaction from another TIP transaction manager.
        /// </summary>
        XACT_E_TIP_PULL_FAILED = (int)(0x8004D021 - 0x100000000),
        /// <summary>
        /// The Transaction Manager on the destination machine is not available.
        /// </summary>
        XACT_E_DEST_TMNOTAVAILABLE = (int)(0x8004D022 - 0x100000000),
        /// <summary>
        /// The Transaction Manager has disabled its support for TIP.
        /// </summary>
        XACT_E_TIP_DISABLED = (int)(0x8004D023 - 0x100000000),
        /// <summary>
        /// The transaction manager has disabled its support for remote/network transactions.
        /// </summary>
        XACT_E_NETWORK_TX_DISABLED = (int)(0x8004D024 - 0x100000000),
        /// <summary>
        /// The partner transaction manager has disabled its support for remote/network transactions.
        /// </summary>
        XACT_E_PARTNER_NETWORK_TX_DISABLED = (int)(0x8004D025 - 0x100000000),
        /// <summary>
        /// The transaction manager has disabled its support for XA transactions.
        /// </summary>
        XACT_E_XA_TX_DISABLED = (int)(0x8004D026 - 0x100000000),
        /// <summary>
        /// MSDTC was unable to read its configuration information.
        /// </summary>
        XACT_E_UNABLE_TO_READ_DTC_CONFIG = (int)(0x8004D027 - 0x100000000),
        /// <summary>
        /// MSDTC was unable to load the dtc proxy dll.
        /// </summary>
        XACT_E_UNABLE_TO_LOAD_DTC_PROXY = (int)(0x8004D028 - 0x100000000),
        /// <summary>
        /// The local transaction has aborted.
        /// </summary>
        XACT_E_ABORTING = (int)(0x8004D029 - 0x100000000),
        /// <summary>
        /// XACT_E_CLERKNOTFOUND
        /// </summary>
        XACT_E_CLERKNOTFOUND = (int)(0x8004D080 - 0x100000000),
        /// <summary>
        /// XACT_E_CLERKEXISTS
        /// </summary>
        XACT_E_CLERKEXISTS = (int)(0x8004D081 - 0x100000000),
        /// <summary>
        /// XACT_E_RECOVERYINPROGRESS
        /// </summary>
        XACT_E_RECOVERYINPROGRESS = (int)(0x8004D082 - 0x100000000),
        /// <summary>
        /// XACT_E_TRANSACTIONCLOSED
        /// </summary>
        XACT_E_TRANSACTIONCLOSED = (int)(0x8004D083 - 0x100000000),
        /// <summary>
        /// XACT_E_INVALIDLSN
        /// </summary>
        XACT_E_INVALIDLSN = (int)(0x8004D084 - 0x100000000),
        /// <summary>
        /// XACT_E_REPLAYREQUEST
        /// </summary>
        XACT_E_REPLAYREQUEST = (int)(0x8004D085 - 0x100000000),
        /// <summary>
        /// An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.
        /// </summary>
        XACT_S_ASYNC = 0x0004D000,
        /// <summary>
        /// XACT_S_DEFECT
        /// </summary>
        XACT_S_DEFECT = 0x0004D001,
        /// <summary>
        /// The method call succeeded because the transaction was read-only.
        /// </summary>
        XACT_S_READONLY = 0x0004D002,
        /// <summary>
        /// The transaction was successfully aborted. However, this is a coordinated transaction, and some number of enlisted resources were aborted outright because they could not support abort-retaining semantics
        /// </summary>
        XACT_S_SOMENORETAIN = 0x0004D003,
        /// <summary>
        /// No changes were made during this call, but the sink wants another chance to look if any other sinks make further changes.
        /// </summary>
        XACT_S_OKINFORM = 0x0004D004,
        /// <summary>
        /// The sink is content and wishes the transaction to proceed. Changes were made to one or more resources during this call.
        /// </summary>
        XACT_S_MADECHANGESCONTENT = 0x0004D005,
        /// <summary>
        /// The sink is for the moment and wishes the transaction to proceed, but if other changes are made following this return by other event sinks then this sink wants another chance to look
        /// </summary>
        XACT_S_MADECHANGESINFORM = 0x0004D006,
        /// <summary>
        /// The transaction was successfully aborted. However, the abort was non-retaining.
        /// </summary>
        XACT_S_ALLNORETAIN = 0x0004D007,
        /// <summary>
        /// An abort operation was already in progress.
        /// </summary>
        XACT_S_ABORTING = 0x0004D008,
        /// <summary>
        /// The resource manager has performed a single-phase commit of the transaction.
        /// </summary>
        XACT_S_SINGLEPHASE = 0x0004D009,
        /// <summary>
        /// The local transaction has not aborted.
        /// </summary>
        XACT_S_LOCALLY_OK = 0x0004D00A,
        /// <summary>
        /// The resource manager has requested to be the coordinator (last resource manager) for the transaction.
        /// </summary>
        XACT_S_LASTRESOURCEMANAGER = 0x0004D010,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CONTEXT_E_FIRST = (int)(0x8004E000 - 0x100000000),
        /// <summary>
        /// The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the "root" component for the transaction.  The creation failed.
        /// </summary>
        CONTEXT_E_LAST = (int)(0x8004E02F - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CONTEXT_S_FIRST = 0x0004E000,
        /// <summary>
        /// No information avialable.
        /// </summary>
        CONTEXT_S_LAST = 0x0004E02F,
        /// <summary>
        /// The root transaction wanted to commit, but transaction aborted
        /// </summary>
        CONTEXT_E_ABORTED = (int)(0x8004E002 - 0x100000000),
        /// <summary>
        /// You made a method call on a COM+ component that has a transaction that has already aborted or in the process of aborting.
        /// </summary>
        CONTEXT_E_ABORTING = (int)(0x8004E003 - 0x100000000),
        /// <summary>
        /// There is no MTS object context
        /// </summary>
        CONTEXT_E_NOCONTEXT = (int)(0x8004E004 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CONTEXT_E_WOULD_DEADLOCK = (int)(0x8004E005 - 0x100000000),
        /// <summary>
        /// The component is configured to use synchronization and a thread has timed out waiting to enter the context.
        /// </summary>
        CONTEXT_E_SYNCH_TIMEOUT = (int)(0x8004E006 - 0x100000000),
        /// <summary>
        /// You made a method call on a COM+ component that has a transaction that has already committed or aborted.
        /// </summary>
        CONTEXT_E_OLDREF = (int)(0x8004E007 - 0x100000000),
        /// <summary>
        /// The specified role was not configured for the application
        /// </summary>
        CONTEXT_E_ROLENOTFOUND = (int)(0x8004E00C - 0x100000000),
        /// <summary>
        /// COM+ was unable to talk to the Microsoft Distributed Transaction Coordinator
        /// </summary>
        CONTEXT_E_TMNOTAVAILABLE = (int)(0x8004E00F - 0x100000000),
        /// <summary>
        /// An unexpected error occurred during COM+ Activation.
        /// </summary>
        CO_E_ACTIVATIONFAILED = (int)(0x8004E021 - 0x100000000),
        /// <summary>
        /// COM+ Activation failed. Check the event log for more information
        /// </summary>
        CO_E_ACTIVATIONFAILED_EVENTLOGGED = (int)(0x8004E022 - 0x100000000),
        /// <summary>
        /// COM+ Activation failed due to a catalog or configuration error.
        /// </summary>
        CO_E_ACTIVATIONFAILED_CATALOGERROR = (int)(0x8004E023 - 0x100000000),
        /// <summary>
        /// COM+ activation failed because the activation could not be completed in the specified amount of time.
        /// </summary>
        CO_E_ACTIVATIONFAILED_TIMEOUT = (int)(0x8004E024 - 0x100000000),
        /// <summary>
        /// COM+ Activation failed because an initialization function failed.  Check the event log for more information.
        /// </summary>
        CO_E_INITIALIZATIONFAILED = (int)(0x8004E025 - 0x100000000),
        /// <summary>
        /// The requested operation requires that JIT be in the current context and it is not
        /// </summary>
        CONTEXT_E_NOJIT = (int)(0x8004E026 - 0x100000000),
        /// <summary>
        /// The requested operation requires that the current context have a Transaction, and it does not
        /// </summary>
        CONTEXT_E_NOTRANSACTION = (int)(0x8004E027 - 0x100000000),
        /// <summary>
        /// The components threading model has changed after install into a COM+ Application.  Please re-install component.
        /// </summary>
        CO_E_THREADINGMODEL_CHANGED = (int)(0x8004E028 - 0x100000000),
        /// <summary>
        /// IIS intrinsics not available.  Start your work with IIS.
        /// </summary>
        CO_E_NOIISINTRINSICS = (int)(0x8004E029 - 0x100000000),
        /// <summary>
        /// An attempt to write a cookie failed.
        /// </summary>
        CO_E_NOCOOKIES = (int)(0x8004E02A - 0x100000000),
        /// <summary>
        /// An attempt to use a database generated a database specific error.
        /// </summary>
        CO_E_DBERROR = (int)(0x8004E02B - 0x100000000),
        /// <summary>
        /// The COM+ component you created must use object pooling to work.
        /// </summary>
        CO_E_NOTPOOLED = (int)(0x8004E02C - 0x100000000),
        /// <summary>
        /// The COM+ component you created must use object construction to work correctly.
        /// </summary>
        CO_E_NOTCONSTRUCTED = (int)(0x8004E02D - 0x100000000),
        /// <summary>
        /// The COM+ component requires synchronization, and it is not configured for it.
        /// </summary>
        CO_E_NOSYNCHRONIZATION = (int)(0x8004E02E - 0x100000000),
        /// <summary>
        /// The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the "root" component for the transaction.  The creation failed.
        /// </summary>
        CO_E_ISOLEVELMISMATCH = (int)(0x8004E02F - 0x100000000),
        /// <summary>
        /// Use the registry database to provide the requested information
        /// </summary>
        OLE_S_USEREG = 0x00040000,
        /// <summary>
        /// Success, but static
        /// </summary>
        OLE_S_STATIC = 0x00040001,
        /// <summary>
        /// Macintosh clipboard format
        /// </summary>
        OLE_S_MAC_CLIPFORMAT = 0x00040002,
        /// <summary>
        /// Successful drop took place
        /// </summary>
        DRAGDROP_S_DROP = 0x00040100,
        /// <summary>
        /// Drag-drop operation canceled
        /// </summary>
        DRAGDROP_S_CANCEL = 0x00040101,
        /// <summary>
        /// Use the default cursor
        /// </summary>
        DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102,
        /// <summary>
        /// Data has same FORMATETC
        /// </summary>
        DATA_S_SAMEFORMATETC = 0x00040130,
        /// <summary>
        /// View is already frozen
        /// </summary>
        VIEW_S_ALREADY_FROZEN = 0x00040140,
        /// <summary>
        /// FORMATETC not supported
        /// </summary>
        CACHE_S_FORMATETC_NOTSUPPORTED = 0x00040170,
        /// <summary>
        /// Same cache
        /// </summary>
        CACHE_S_SAMECACHE = 0x00040171,
        /// <summary>
        /// Some cache(s) not updated
        /// </summary>
        CACHE_S_SOMECACHES_NOTUPDATED = 0x00040172,
        /// <summary>
        /// Invalid verb for OLE object
        /// </summary>
        OLEOBJ_S_INVALIDVERB = 0x00040180,
        /// <summary>
        /// Verb number is valid but verb cannot be done now
        /// </summary>
        OLEOBJ_S_CANNOT_DOVERB_NOW = 0x00040181,
        /// <summary>
        /// Invalid window handle passed
        /// </summary>
        OLEOBJ_S_INVALIDHWND = 0x00040182,
        /// <summary>
        /// Message is too long, some of it had to be truncated before displaying
        /// </summary>
        INPLACE_S_TRUNCATED = 0x000401A0,
        /// <summary>
        /// Unable to convert OLESTREAM to IStorage
        /// </summary>
        CONVERT10_S_NO_PRESENTATION = 0x000401C0,
        /// <summary>
        /// Moniker reduced to itself
        /// </summary>
        MK_S_REDUCED_TO_SELF = 0x000401E2,
        /// <summary>
        /// Common prefix is this moniker
        /// </summary>
        MK_S_ME = 0x000401E4,
        /// <summary>
        /// Common prefix is input moniker
        /// </summary>
        MK_S_HIM = 0x000401E5,
        /// <summary>
        /// Common prefix is both monikers
        /// </summary>
        MK_S_US = 0x000401E6,
        /// <summary>
        /// Moniker is already registered in running object table
        /// </summary>
        MK_S_MONIKERALREADYREGISTERED = 0x000401E7,
        /// <summary>
        /// The task is ready to run at its next scheduled time.
        /// </summary>
        SCHED_S_TASK_READY = 0x00041300,
        /// <summary>
        /// The task is currently running.
        /// </summary>
        SCHED_S_TASK_RUNNING = 0x00041301,
        /// <summary>
        /// The task will not run at the scheduled times because it has been disabled.
        /// </summary>
        SCHED_S_TASK_DISABLED = 0x00041302,
        /// <summary>
        /// The task has not yet run.
        /// </summary>
        SCHED_S_TASK_HAS_NOT_RUN = 0x00041303,
        /// <summary>
        /// There are no more runs scheduled for this task.
        /// </summary>
        SCHED_S_TASK_NO_MORE_RUNS = 0x00041304,
        /// <summary>
        /// One or more of the properties that are needed to run this task on a schedule have not been set.
        /// </summary>
        SCHED_S_TASK_NOT_SCHEDULED = 0x00041305,
        /// <summary>
        /// The last run of the task was terminated by the user.
        /// </summary>
        SCHED_S_TASK_TERMINATED = 0x00041306,
        /// <summary>
        /// Either the task has no triggers or the existing triggers are disabled or not set.
        /// </summary>
        SCHED_S_TASK_NO_VALID_TRIGGERS = 0x00041307,
        /// <summary>
        /// Event triggers don't have set run times.
        /// </summary>
        SCHED_S_EVENT_TRIGGER = 0x00041308,
        /// <summary>
        /// Trigger not found.
        /// </summary>
        SCHED_E_TRIGGER_NOT_FOUND = (int)(0x80041309 - 0x100000000),
        /// <summary>
        /// One or more of the properties that are needed to run this task have not been set.
        /// </summary>
        SCHED_E_TASK_NOT_READY = (int)(0x8004130A - 0x100000000),
        /// <summary>
        /// There is no running instance of the task to terminate.
        /// </summary>
        SCHED_E_TASK_NOT_RUNNING = (int)(0x8004130B - 0x100000000),
        /// <summary>
        /// The Task Scheduler Service is not installed on this computer.
        /// </summary>
        SCHED_E_SERVICE_NOT_INSTALLED = (int)(0x8004130C - 0x100000000),
        /// <summary>
        /// The task object could not be opened.
        /// </summary>
        SCHED_E_CANNOT_OPEN_TASK = (int)(0x8004130D - 0x100000000),
        /// <summary>
        /// The object is either an invalid task object or is not a task object.
        /// </summary>
        SCHED_E_INVALID_TASK = (int)(0x8004130E - 0x100000000),
        /// <summary>
        /// No account information could be found in the Task Scheduler security database for the task indicated.
        /// </summary>
        SCHED_E_ACCOUNT_INFORMATION_NOT_SET = (int)(0x8004130F - 0x100000000),
        /// <summary>
        /// Unable to establish existence of the account specified.
        /// </summary>
        SCHED_E_ACCOUNT_NAME_NOT_FOUND = (int)(0x80041310 - 0x100000000),
        /// <summary>
        /// Corruption was detected in the Task Scheduler security database, the database has been reset.
        /// </summary>
        SCHED_E_ACCOUNT_DBASE_CORRUPT = (int)(0x80041311 - 0x100000000),
        /// <summary>
        /// Task Scheduler security services are available only on Windows NT.
        /// </summary>
        SCHED_E_NO_SECURITY_SERVICES = (int)(0x80041312 - 0x100000000),
        /// <summary>
        /// The task object version is either unsupported or invalid.
        /// </summary>
        SCHED_E_UNKNOWN_OBJECT_VERSION = (int)(0x80041313 - 0x100000000),
        /// <summary>
        /// The task has been configured with an unsupported combination of account settings and run time options.
        /// </summary>
        SCHED_E_UNSUPPORTED_ACCOUNT_OPTION = (int)(0x80041314 - 0x100000000),
        /// <summary>
        /// The Task Scheduler Service is not running.
        /// </summary>
        SCHED_E_SERVICE_NOT_RUNNING = (int)(0x80041315 - 0x100000000),
        /// <summary>
        /// Attempt to create a class object failed
        /// </summary>
        CO_E_CLASS_CREATE_FAILED = (int)(0x80080001 - 0x100000000),
        /// <summary>
        /// OLE service could not bind object
        /// </summary>
        CO_E_SCM_ERROR = (int)(0x80080002 - 0x100000000),
        /// <summary>
        /// RPC communication failed with OLE service
        /// </summary>
        CO_E_SCM_RPC_FAILURE = (int)(0x80080003 - 0x100000000),
        /// <summary>
        /// Bad path to object
        /// </summary>
        CO_E_BAD_PATH = (int)(0x80080004 - 0x100000000),
        /// <summary>
        /// Server execution failed
        /// </summary>
        CO_E_SERVER_EXEC_FAILURE = (int)(0x80080005 - 0x100000000),
        /// <summary>
        /// OLE service could not communicate with the object server
        /// </summary>
        CO_E_OBJSRV_RPC_FAILURE = (int)(0x80080006 - 0x100000000),
        /// <summary>
        /// Moniker path could not be normalized
        /// </summary>
        MK_E_NO_NORMALIZED = (int)(0x80080007 - 0x100000000),
        /// <summary>
        /// Object server is stopping when OLE service contacts it
        /// </summary>
        CO_E_SERVER_STOPPING = (int)(0x80080008 - 0x100000000),
        /// <summary>
        /// An invalid root block pointer was specified
        /// </summary>
        MEM_E_INVALID_ROOT = (int)(0x80080009 - 0x100000000),
        /// <summary>
        /// An allocation chain contained an invalid link pointer
        /// </summary>
        MEM_E_INVALID_LINK = (int)(0x80080010 - 0x100000000),
        /// <summary>
        /// The requested allocation size was too large
        /// </summary>
        MEM_E_INVALID_SIZE = (int)(0x80080011 - 0x100000000),
        /// <summary>
        /// Not all the requested interfaces were available
        /// </summary>
        CO_S_NOTALLINTERFACES = 0x00080012,
        /// <summary>
        /// The specified machine name was not found in the cache.
        /// </summary>
        CO_S_MACHINENAMENOTFOUND = 0x00080013,
        /// <summary>
        /// Unknown interface.
        /// </summary>
        DISP_E_UNKNOWNINTERFACE = (int)(0x80020001 - 0x100000000),
        /// <summary>
        /// Member not found.
        /// </summary>
        DISP_E_MEMBERNOTFOUND = (int)(0x80020003 - 0x100000000),
        /// <summary>
        /// Parameter not found.
        /// </summary>
        DISP_E_PARAMNOTFOUND = (int)(0x80020004 - 0x100000000),
        /// <summary>
        /// Type mismatch.
        /// </summary>
        DISP_E_TYPEMISMATCH = (int)(0x80020005 - 0x100000000),
        /// <summary>
        /// Unknown name.
        /// </summary>
        DISP_E_UNKNOWNNAME = (int)(0x80020006 - 0x100000000),
        /// <summary>
        /// No named arguments.
        /// </summary>
        DISP_E_NONAMEDARGS = (int)(0x80020007 - 0x100000000),
        /// <summary>
        /// Bad variable type.
        /// </summary>
        DISP_E_BADVARTYPE = (int)(0x80020008 - 0x100000000),
        /// <summary>
        /// Exception occurred.
        /// </summary>
        DISP_E_EXCEPTION = (int)(0x80020009 - 0x100000000),
        /// <summary>
        /// Out of present range.
        /// </summary>
        DISP_E_OVERFLOW = (int)(0x8002000A - 0x100000000),
        /// <summary>
        /// Invalid index.
        /// </summary>
        DISP_E_BADINDEX = (int)(0x8002000B - 0x100000000),
        /// <summary>
        /// Unknown language.
        /// </summary>
        DISP_E_UNKNOWNLCID = (int)(0x8002000C - 0x100000000),
        /// <summary>
        /// Memory is locked.
        /// </summary>
        DISP_E_ARRAYISLOCKED = (int)(0x8002000D - 0x100000000),
        /// <summary>
        /// Invalid number of parameters.
        /// </summary>
        DISP_E_BADPARAMCOUNT = (int)(0x8002000E - 0x100000000),
        /// <summary>
        /// Parameter not optional.
        /// </summary>
        DISP_E_PARAMNOTOPTIONAL = (int)(0x8002000F - 0x100000000),
        /// <summary>
        /// Invalid callee.
        /// </summary>
        DISP_E_BADCALLEE = (int)(0x80020010 - 0x100000000),
        /// <summary>
        /// Does not support a collection.
        /// </summary>
        DISP_E_NOTACOLLECTION = (int)(0x80020011 - 0x100000000),
        /// <summary>
        /// Division by zero.
        /// </summary>
        DISP_E_DIVBYZERO = (int)(0x80020012 - 0x100000000),
        /// <summary>
        /// Buffer too small
        /// </summary>
        DISP_E_BUFFERTOOSMALL = (int)(0x80020013 - 0x100000000),
        /// <summary>
        /// Buffer too small.
        /// </summary>
        TYPE_E_BUFFERTOOSMALL = (int)(0x80028016 - 0x100000000),
        /// <summary>
        /// Field name not defined in the record.
        /// </summary>
        TYPE_E_FIELDNOTFOUND = (int)(0x80028017 - 0x100000000),
        /// <summary>
        /// Old format or invalid type library.
        /// </summary>
        TYPE_E_INVDATAREAD = (int)(0x80028018 - 0x100000000),
        /// <summary>
        /// Old format or invalid type library.
        /// </summary>
        TYPE_E_UNSUPFORMAT = (int)(0x80028019 - 0x100000000),
        /// <summary>
        /// Error accessing the OLE registry.
        /// </summary>
        TYPE_E_REGISTRYACCESS = (int)(0x8002801C - 0x100000000),
        /// <summary>
        /// Library not registered.
        /// </summary>
        TYPE_E_LIBNOTREGISTERED = (int)(0x8002801D - 0x100000000),
        /// <summary>
        /// Bound to unknown type.
        /// </summary>
        TYPE_E_UNDEFINEDTYPE = (int)(0x80028027 - 0x100000000),
        /// <summary>
        /// Qualified name disallowed.
        /// </summary>
        TYPE_E_QUALIFIEDNAMEDISALLOWED = (int)(0x80028028 - 0x100000000),
        /// <summary>
        /// Invalid forward reference, or reference to uncompiled type.
        /// </summary>
        TYPE_E_INVALIDSTATE = (int)(0x80028029 - 0x100000000),
        /// <summary>
        /// Type mismatch.
        /// </summary>
        TYPE_E_WRONGTYPEKIND = (int)(0x8002802A - 0x100000000),
        /// <summary>
        /// Element not found.
        /// </summary>
        TYPE_E_ELEMENTNOTFOUND = (int)(0x8002802B - 0x100000000),
        /// <summary>
        /// Ambiguous name.
        /// </summary>
        TYPE_E_AMBIGUOUSNAME = (int)(0x8002802C - 0x100000000),
        /// <summary>
        /// Name already exists in the library.
        /// </summary>
        TYPE_E_NAMECONFLICT = (int)(0x8002802D - 0x100000000),
        /// <summary>
        /// Unknown LCID.
        /// </summary>
        TYPE_E_UNKNOWNLCID = (int)(0x8002802E - 0x100000000),
        /// <summary>
        /// Function not defined in specified DLL.
        /// </summary>
        TYPE_E_DLLFUNCTIONNOTFOUND = (int)(0x8002802F - 0x100000000),
        /// <summary>
        /// Wrong module kind for the operation.
        /// </summary>
        TYPE_E_BADMODULEKIND = (int)(0x800288BD - 0x100000000),
        /// <summary>
        /// Size may not exceed 64K.
        /// </summary>
        TYPE_E_SIZETOOBIG = (int)(0x800288C5 - 0x100000000),
        /// <summary>
        /// Duplicate ID in inheritance hierarchy.
        /// </summary>
        TYPE_E_DUPLICATEID = (int)(0x800288C6 - 0x100000000),
        /// <summary>
        /// Incorrect inheritance depth in standard OLE hmember.
        /// </summary>
        TYPE_E_INVALIDID = (int)(0x800288CF - 0x100000000),
        /// <summary>
        /// Type mismatch.
        /// </summary>
        TYPE_E_TYPEMISMATCH = (int)(0x80028CA0 - 0x100000000),
        /// <summary>
        /// Invalid number of arguments.
        /// </summary>
        TYPE_E_OUTOFBOUNDS = (int)(0x80028CA1 - 0x100000000),
        /// <summary>
        /// I/O Error.
        /// </summary>
        TYPE_E_IOERROR = (int)(0x80028CA2 - 0x100000000),
        /// <summary>
        /// Error creating unique tmp file.
        /// </summary>
        TYPE_E_CANTCREATETMPFILE = (int)(0x80028CA3 - 0x100000000),
        /// <summary>
        /// Error loading type library/DLL.
        /// </summary>
        TYPE_E_CANTLOADLIBRARY = (int)(0x80029C4A - 0x100000000),
        /// <summary>
        /// Inconsistent property functions.
        /// </summary>
        TYPE_E_INCONSISTENTPROPFUNCS = (int)(0x80029C83 - 0x100000000),
        /// <summary>
        /// Circular dependency between types/modules.
        /// </summary>
        TYPE_E_CIRCULARTYPE = (int)(0x80029C84 - 0x100000000),
        /// <summary>
        /// Unable to perform requested operation.
        /// </summary>
        STG_E_INVALIDFUNCTION = (int)(0x80030001 - 0x100000000),
        /// <summary>
        /// %1 could not be found.
        /// </summary>
        STG_E_FILENOTFOUND = (int)(0x80030002 - 0x100000000),
        /// <summary>
        /// The path %1 could not be found.
        /// </summary>
        STG_E_PATHNOTFOUND = (int)(0x80030003 - 0x100000000),
        /// <summary>
        /// There are insufficient resources to open another file.
        /// </summary>
        STG_E_TOOMANYOPENFILES = (int)(0x80030004 - 0x100000000),
        /// <summary>
        /// Access Denied.
        /// </summary>
        STG_E_ACCESSDENIED = (int)(0x80030005 - 0x100000000),
        /// <summary>
        /// Attempted an operation on an invalid object.
        /// </summary>
        STG_E_INVALIDHANDLE = (int)(0x80030006 - 0x100000000),
        /// <summary>
        /// There is insufficient memory available to complete operation.
        /// </summary>
        STG_E_INSUFFICIENTMEMORY = (int)(0x80030008 - 0x100000000),
        /// <summary>
        /// Invalid pointer error.
        /// </summary>
        STG_E_INVALIDPOINTER = (int)(0x80030009 - 0x100000000),
        /// <summary>
        /// There are no more entries to return.
        /// </summary>
        STG_E_NOMOREFILES = (int)(0x80030012 - 0x100000000),
        /// <summary>
        /// Disk is write-protected.
        /// </summary>
        STG_E_DISKISWRITEPROTECTED = (int)(0x80030013 - 0x100000000),
        /// <summary>
        /// An error occurred during a seek operation.
        /// </summary>
        STG_E_SEEKERROR = (int)(0x80030019 - 0x100000000),
        /// <summary>
        /// A disk error occurred during a write operation.
        /// </summary>
        STG_E_WRITEFAULT = (int)(0x8003001D - 0x100000000),
        /// <summary>
        /// A disk error occurred during a read operation.
        /// </summary>
        STG_E_READFAULT = (int)(0x8003001E - 0x100000000),
        /// <summary>
        /// A share violation has occurred.
        /// </summary>
        STG_E_SHAREVIOLATION = (int)(0x80030020 - 0x100000000),
        /// <summary>
        /// A lock violation has occurred.
        /// </summary>
        STG_E_LOCKVIOLATION = (int)(0x80030021 - 0x100000000),
        /// <summary>
        /// %1 already exists.
        /// </summary>
        STG_E_FILEALREADYEXISTS = (int)(0x80030050 - 0x100000000),
        /// <summary>
        /// Invalid parameter error.
        /// </summary>
        STG_E_INVALIDPARAMETER = (int)(0x80030057 - 0x100000000),
        /// <summary>
        /// There is insufficient disk space to complete operation.
        /// </summary>
        STG_E_MEDIUMFULL = (int)(0x80030070 - 0x100000000),
        /// <summary>
        /// Illegal write of non-simple property to simple property set.
        /// </summary>
        STG_E_PROPSETMISMATCHED = (int)(0x800300F0 - 0x100000000),
        /// <summary>
        /// An API call exited abnormally.
        /// </summary>
        STG_E_ABNORMALAPIEXIT = (int)(0x800300FA - 0x100000000),
        /// <summary>
        /// The file %1 is not a valid compound file.
        /// </summary>
        STG_E_INVALIDHEADER = (int)(0x800300FB - 0x100000000),
        /// <summary>
        /// The name %1 is not valid.
        /// </summary>
        STG_E_INVALIDNAME = (int)(0x800300FC - 0x100000000),
        /// <summary>
        /// An unexpected error occurred.
        /// </summary>
        STG_E_UNKNOWN = (int)(0x800300FD - 0x100000000),
        /// <summary>
        /// That function is not implemented.
        /// </summary>
        STG_E_UNIMPLEMENTEDFUNCTION = (int)(0x800300FE - 0x100000000),
        /// <summary>
        /// Invalid flag error.
        /// </summary>
        STG_E_INVALIDFLAG = (int)(0x800300FF - 0x100000000),
        /// <summary>
        /// Attempted to use an object that is busy.
        /// </summary>
        STG_E_INUSE = (int)(0x80030100 - 0x100000000),
        /// <summary>
        /// The storage has been changed since the last commit.
        /// </summary>
        STG_E_NOTCURRENT = (int)(0x80030101 - 0x100000000),
        /// <summary>
        /// Attempted to use an object that has ceased to exist.
        /// </summary>
        STG_E_REVERTED = (int)(0x80030102 - 0x100000000),
        /// <summary>
        /// Can't save.
        /// </summary>
        STG_E_CANTSAVE = (int)(0x80030103 - 0x100000000),
        /// <summary>
        /// The compound file %1 was produced with an incompatible version of storage.
        /// </summary>
        STG_E_OLDFORMAT = (int)(0x80030104 - 0x100000000),
        /// <summary>
        /// The compound file %1 was produced with a newer version of storage.
        /// </summary>
        STG_E_OLDDLL = (int)(0x80030105 - 0x100000000),
        /// <summary>
        /// Share.exe or equivalent is required for operation.
        /// </summary>
        STG_E_SHAREREQUIRED = (int)(0x80030106 - 0x100000000),
        /// <summary>
        /// Illegal operation called on non-file based storage.
        /// </summary>
        STG_E_NOTFILEBASEDSTORAGE = (int)(0x80030107 - 0x100000000),
        /// <summary>
        /// Illegal operation called on object with extant marshallings.
        /// </summary>
        STG_E_EXTANTMARSHALLINGS = (int)(0x80030108 - 0x100000000),
        /// <summary>
        /// The docfile has been corrupted.
        /// </summary>
        STG_E_DOCFILECORRUPT = (int)(0x80030109 - 0x100000000),
        /// <summary>
        /// OLE32.DLL has been loaded at the wrong address.
        /// </summary>
        STG_E_BADBASEADDRESS = (int)(0x80030110 - 0x100000000),
        /// <summary>
        /// The compound file is too large for the current implementation
        /// </summary>
        STG_E_DOCFILETOOLARGE = (int)(0x80030111 - 0x100000000),
        /// <summary>
        /// The compound file was not created with the STGM_SIMPLE flag
        /// </summary>
        STG_E_NOTSIMPLEFORMAT = (int)(0x80030112 - 0x100000000),
        /// <summary>
        /// The file download was aborted abnormally.  The file is incomplete.
        /// </summary>
        STG_E_INCOMPLETE = (int)(0x80030201 - 0x100000000),
        /// <summary>
        /// The file download has been terminated.
        /// </summary>
        STG_E_TERMINATED = (int)(0x80030202 - 0x100000000),
        /// <summary>
        /// The underlying file was converted to compound file format.
        /// </summary>
        STG_S_CONVERTED = 0x00030200,
        /// <summary>
        /// The storage operation should block until more data is available.
        /// </summary>
        STG_S_BLOCK = 0x00030201,
        /// <summary>
        /// The storage operation should retry immediately.
        /// </summary>
        STG_S_RETRYNOW = 0x00030202,
        /// <summary>
        /// The notified event sink will not influence the storage operation.
        /// </summary>
        STG_S_MONITORING = 0x00030203,
        /// <summary>
        /// Multiple opens prevent consolidated. (commit succeeded).
        /// </summary>
        STG_S_MULTIPLEOPENS = 0x00030204,
        /// <summary>
        /// Consolidation of the storage file failed. (commit succeeded).
        /// </summary>
        STG_S_CONSOLIDATIONFAILED = 0x00030205,
        /// <summary>
        /// Consolidation of the storage file is inappropriate. (commit succeeded).
        /// </summary>
        STG_S_CANNOTCONSOLIDATE = 0x00030206,
        /// <summary>
        /// Generic Copy Protection Error.
        /// </summary>
        STG_E_STATUS_COPY_PROTECTION_FAILURE = (int)(0x80030305 - 0x100000000),
        /// <summary>
        /// Copy Protection Error - DVD CSS Authentication failed.
        /// </summary>
        STG_E_CSS_AUTHENTICATION_FAILURE = (int)(0x80030306 - 0x100000000),
        /// <summary>
        /// Copy Protection Error - The given sector does not have a valid CSS key.
        /// </summary>
        STG_E_CSS_KEY_NOT_PRESENT = (int)(0x80030307 - 0x100000000),
        /// <summary>
        /// Copy Protection Error - DVD session key not established.
        /// </summary>
        STG_E_CSS_KEY_NOT_ESTABLISHED = (int)(0x80030308 - 0x100000000),
        /// <summary>
        /// Copy Protection Error - The read failed because the sector is encrypted.
        /// </summary>
        STG_E_CSS_SCRAMBLED_SECTOR = (int)(0x80030309 - 0x100000000),
        /// <summary>
        /// Copy Protection Error - The current DVD's region does not correspond to the region setting of the drive.
        /// </summary>
        STG_E_CSS_REGION_MISMATCH = (int)(0x8003030A - 0x100000000),
        /// <summary>
        /// Copy Protection Error - The drive's region setting may be permanent or the number of user resets has been exhausted.
        /// </summary>
        STG_E_RESETS_EXHAUSTED = (int)(0x8003030B - 0x100000000),
        /// <summary>
        /// Call was rejected by callee.
        /// </summary>
        RPC_E_CALL_REJECTED = (int)(0x80010001 - 0x100000000),
        /// <summary>
        /// Call was canceled by the message filter.
        /// </summary>
        RPC_E_CALL_CANCELED = (int)(0x80010002 - 0x100000000),
        /// <summary>
        /// The caller is dispatching an intertask SendMessage call and cannot call out via PostMessage.
        /// </summary>
        RPC_E_CANTPOST_INSENDCALL = (int)(0x80010003 - 0x100000000),
        /// <summary>
        /// The caller is dispatching an asynchronous call and cannot make an outgoing call on behalf of this call.
        /// </summary>
        RPC_E_CANTCALLOUT_INASYNCCALL = (int)(0x80010004 - 0x100000000),
        /// <summary>
        /// It is illegal to call out while inside message filter.
        /// </summary>
        RPC_E_CANTCALLOUT_INEXTERNALCALL = (int)(0x80010005 - 0x100000000),
        /// <summary>
        /// The connection terminated or is in a bogus state and cannot be used any more. Other connections are still valid.
        /// </summary>
        RPC_E_CONNECTION_TERMINATED = (int)(0x80010006 - 0x100000000),
        /// <summary>
        /// The callee (server [not server application]) is not available and disappeared, all connections are invalid. The call may have executed.
        /// </summary>
        RPC_E_SERVER_DIED = (int)(0x80010007 - 0x100000000),
        /// <summary>
        /// The caller (client) disappeared while the callee (server) was processing a call.
        /// </summary>
        RPC_E_CLIENT_DIED = (int)(0x80010008 - 0x100000000),
        /// <summary>
        /// The data packet with the marshalled parameter data is incorrect.
        /// </summary>
        RPC_E_INVALID_DATAPACKET = (int)(0x80010009 - 0x100000000),
        /// <summary>
        /// The call was not transmitted properly, the message queue was full and was not emptied after yielding.
        /// </summary>
        RPC_E_CANTTRANSMIT_CALL = (int)(0x8001000A - 0x100000000),
        /// <summary>
        /// The client (caller) cannot marshall the parameter data - low memory, etc.
        /// </summary>
        RPC_E_CLIENT_CANTMARSHAL_DATA = (int)(0x8001000B - 0x100000000),
        /// <summary>
        /// The client (caller) cannot unmarshall the return data - low memory, etc.
        /// </summary>
        RPC_E_CLIENT_CANTUNMARSHAL_DATA = (int)(0x8001000C - 0x100000000),
        /// <summary>
        /// The server (callee) cannot marshall the return data - low memory, etc.
        /// </summary>
        RPC_E_SERVER_CANTMARSHAL_DATA = (int)(0x8001000D - 0x100000000),
        /// <summary>
        /// The server (callee) cannot unmarshall the parameter data - low memory, etc.
        /// </summary>
        RPC_E_SERVER_CANTUNMARSHAL_DATA = (int)(0x8001000E - 0x100000000),
        /// <summary>
        /// Received data is invalid, could be server or client data.
        /// </summary>
        RPC_E_INVALID_DATA = (int)(0x8001000F - 0x100000000),
        /// <summary>
        /// A particular parameter is invalid and cannot be (un)marshalled.
        /// </summary>
        RPC_E_INVALID_PARAMETER = (int)(0x80010010 - 0x100000000),
        /// <summary>
        /// There is no second outgoing call on same channel in DDE conversation.
        /// </summary>
        RPC_E_CANTCALLOUT_AGAIN = (int)(0x80010011 - 0x100000000),
        /// <summary>
        /// The callee (server [not server application]) is not available and disappeared, all connections are invalid. The call did not execute.
        /// </summary>
        RPC_E_SERVER_DIED_DNE = (int)(0x80010012 - 0x100000000),
        /// <summary>
        /// System call failed.
        /// </summary>
        RPC_E_SYS_CALL_FAILED = (int)(0x80010100 - 0x100000000),
        /// <summary>
        /// Could not allocate some required resource (memory, events, ...)
        /// </summary>
        RPC_E_OUT_OF_RESOURCES = (int)(0x80010101 - 0x100000000),
        /// <summary>
        /// Attempted to make calls on more than one thread in single threaded mode.
        /// </summary>
        RPC_E_ATTEMPTED_MULTITHREAD = (int)(0x80010102 - 0x100000000),
        /// <summary>
        /// The requested interface is not registered on the server object.
        /// </summary>
        RPC_E_NOT_REGISTERED = (int)(0x80010103 - 0x100000000),
        /// <summary>
        /// RPC could not call the server or could not return the results of calling the server.
        /// </summary>
        RPC_E_FAULT = (int)(0x80010104 - 0x100000000),
        /// <summary>
        /// The server threw an exception.
        /// </summary>
        RPC_E_SERVERFAULT = (int)(0x80010105 - 0x100000000),
        /// <summary>
        /// Cannot change thread mode after it is set.
        /// </summary>
        RPC_E_CHANGED_MODE = (int)(0x80010106 - 0x100000000),
        /// <summary>
        /// The method called does not exist on the server.
        /// </summary>
        RPC_E_INVALIDMETHOD = (int)(0x80010107 - 0x100000000),
        /// <summary>
        /// The object invoked has disconnected from its clients.
        /// </summary>
        RPC_E_DISCONNECTED = (int)(0x80010108 - 0x100000000),
        /// <summary>
        /// The object invoked chose not to process the call now.  Try again later.
        /// </summary>
        RPC_E_RETRY = (int)(0x80010109 - 0x100000000),
        /// <summary>
        /// The message filter indicated that the application is busy.
        /// </summary>
        RPC_E_SERVERCALL_RETRYLATER = (int)(0x8001010A - 0x100000000),
        /// <summary>
        /// The message filter rejected the call.
        /// </summary>
        RPC_E_SERVERCALL_REJECTED = (int)(0x8001010B - 0x100000000),
        /// <summary>
        /// A call control interfaces was called with invalid data.
        /// </summary>
        RPC_E_INVALID_CALLDATA = (int)(0x8001010C - 0x100000000),
        /// <summary>
        /// An outgoing call cannot be made since the application is dispatching an input-synchronous call.
        /// </summary>
        RPC_E_CANTCALLOUT_ININPUTSYNCCALL = (int)(0x8001010D - 0x100000000),
        /// <summary>
        /// The application called an interface that was marshalled for a different thread.
        /// </summary>
        RPC_E_WRONG_THREAD = (int)(0x8001010E - 0x100000000),
        /// <summary>
        /// CoInitialize has not been called on the current thread.
        /// </summary>
        RPC_E_THREAD_NOT_INIT = (int)(0x8001010F - 0x100000000),
        /// <summary>
        /// The version of OLE on the client and server machines does not match.
        /// </summary>
        RPC_E_VERSION_MISMATCH = (int)(0x80010110 - 0x100000000),
        /// <summary>
        /// OLE received a packet with an invalid header.
        /// </summary>
        RPC_E_INVALID_HEADER = (int)(0x80010111 - 0x100000000),
        /// <summary>
        /// OLE received a packet with an invalid extension.
        /// </summary>
        RPC_E_INVALID_EXTENSION = (int)(0x80010112 - 0x100000000),
        /// <summary>
        /// The requested object or interface does not exist.
        /// </summary>
        RPC_E_INVALID_IPID = (int)(0x80010113 - 0x100000000),
        /// <summary>
        /// The requested object does not exist.
        /// </summary>
        RPC_E_INVALID_OBJECT = (int)(0x80010114 - 0x100000000),
        /// <summary>
        /// OLE has sent a request and is waiting for a reply.
        /// </summary>
        RPC_S_CALLPENDING = (int)(0x80010115 - 0x100000000),
        /// <summary>
        /// OLE is waiting before retrying a request.
        /// </summary>
        RPC_S_WAITONTIMER = (int)(0x80010116 - 0x100000000),
        /// <summary>
        /// Call context cannot be accessed after call completed.
        /// </summary>
        RPC_E_CALL_COMPLETE = (int)(0x80010117 - 0x100000000),
        /// <summary>
        /// Impersonate on unsecure calls is not supported.
        /// </summary>
        RPC_E_UNSECURE_CALL = (int)(0x80010118 - 0x100000000),
        /// <summary>
        /// Security must be initialized before any interfaces are marshalled or unmarshalled. It cannot be changed once initialized.
        /// </summary>
        RPC_E_TOO_LATE = (int)(0x80010119 - 0x100000000),
        /// <summary>
        /// No security packages are installed on this machine or the user is not logged on or there are no compatible security packages between the client and server.
        /// </summary>
        RPC_E_NO_GOOD_SECURITY_PACKAGES = (int)(0x8001011A - 0x100000000),
        /// <summary>
        /// Access is denied.
        /// </summary>
        RPC_E_ACCESS_DENIED = (int)(0x8001011B - 0x100000000),
        /// <summary>
        /// Remote calls are not allowed for this process.
        /// </summary>
        RPC_E_REMOTE_DISABLED = (int)(0x8001011C - 0x100000000),
        /// <summary>
        /// The marshaled interface data packet (OBJREF) has an invalid or unknown format.
        /// </summary>
        RPC_E_INVALID_OBJREF = (int)(0x8001011D - 0x100000000),
        /// <summary>
        /// No context is associated with this call. This happens for some custom marshalled calls and on the client side of the call.
        /// </summary>
        RPC_E_NO_CONTEXT = (int)(0x8001011E - 0x100000000),
        /// <summary>
        /// This operation returned because the timeout period expired.
        /// </summary>
        RPC_E_TIMEOUT = (int)(0x8001011F - 0x100000000),
        /// <summary>
        /// There are no synchronize objects to wait on.
        /// </summary>
        RPC_E_NO_SYNC = (int)(0x80010120 - 0x100000000),
        /// <summary>
        /// Full subject issuer chain SSL principal name expected from the server.
        /// </summary>
        RPC_E_FULLSIC_REQUIRED = (int)(0x80010121 - 0x100000000),
        /// <summary>
        /// Principal name is not a valid MSSTD name.
        /// </summary>
        RPC_E_INVALID_STD_NAME = (int)(0x80010122 - 0x100000000),
        /// <summary>
        /// Unable to impersonate DCOM client
        /// </summary>
        CO_E_FAILEDTOIMPERSONATE = (int)(0x80010123 - 0x100000000),
        /// <summary>
        /// Unable to obtain server's security context
        /// </summary>
        CO_E_FAILEDTOGETSECCTX = (int)(0x80010124 - 0x100000000),
        /// <summary>
        /// Unable to open the access token of the current thread
        /// </summary>
        CO_E_FAILEDTOOPENTHREADTOKEN = (int)(0x80010125 - 0x100000000),
        /// <summary>
        /// Unable to obtain user info from an access token
        /// </summary>
        CO_E_FAILEDTOGETTOKENINFO = (int)(0x80010126 - 0x100000000),
        /// <summary>
        /// The client who called IAccessControl::IsAccessPermitted was not the trustee provided to the method
        /// </summary>
        CO_E_TRUSTEEDOESNTMATCHCLIENT = (int)(0x80010127 - 0x100000000),
        /// <summary>
        /// Unable to obtain the client's security blanket
        /// </summary>
        CO_E_FAILEDTOQUERYCLIENTBLANKET = (int)(0x80010128 - 0x100000000),
        /// <summary>
        /// Unable to set a discretionary ACL into a security descriptor
        /// </summary>
        CO_E_FAILEDTOSETDACL = (int)(0x80010129 - 0x100000000),
        /// <summary>
        /// The system function, AccessCheck, returned false
        /// </summary>
        CO_E_ACCESSCHECKFAILED = (int)(0x8001012A - 0x100000000),
        /// <summary>
        /// Either NetAccessDel or NetAccessAdd returned an error code.
        /// </summary>
        CO_E_NETACCESSAPIFAILED = (int)(0x8001012B - 0x100000000),
        /// <summary>
        /// One of the trustee strings provided by the user did not conform to the <Domain>\<Name> syntax and it was not the "*" string
        /// </summary>
        CO_E_WRONGTRUSTEENAMESYNTAX = (int)(0x8001012C - 0x100000000),
        /// <summary>
        /// One of the security identifiers provided by the user was invalid
        /// </summary>
        CO_E_INVALIDSID = (int)(0x8001012D - 0x100000000),
        /// <summary>
        /// Unable to convert a wide character trustee string to a multibyte trustee string
        /// </summary>
        CO_E_CONVERSIONFAILED = (int)(0x8001012E - 0x100000000),
        /// <summary>
        /// Unable to find a security identifier that corresponds to a trustee string provided by the user
        /// </summary>
        CO_E_NOMATCHINGSIDFOUND = (int)(0x8001012F - 0x100000000),
        /// <summary>
        /// The system function, LookupAccountSID, failed
        /// </summary>
        CO_E_LOOKUPACCSIDFAILED = (int)(0x80010130 - 0x100000000),
        /// <summary>
        /// Unable to find a trustee name that corresponds to a security identifier provided by the user
        /// </summary>
        CO_E_NOMATCHINGNAMEFOUND = (int)(0x80010131 - 0x100000000),
        /// <summary>
        /// The system function, LookupAccountName, failed
        /// </summary>
        CO_E_LOOKUPACCNAMEFAILED = (int)(0x80010132 - 0x100000000),
        /// <summary>
        /// Unable to set or reset a serialization handle
        /// </summary>
        CO_E_SETSERLHNDLFAILED = (int)(0x80010133 - 0x100000000),
        /// <summary>
        /// Unable to obtain the Windows directory
        /// </summary>
        CO_E_FAILEDTOGETWINDIR = (int)(0x80010134 - 0x100000000),
        /// <summary>
        /// Path too long
        /// </summary>
        CO_E_PATHTOOInt32 = (int)(0x80010135 - 0x100000000),
        /// <summary>
        /// Unable to generate a uuid.
        /// </summary>
        CO_E_FAILEDTOGENUUID = (int)(0x80010136 - 0x100000000),
        /// <summary>
        /// Unable to create file
        /// </summary>
        CO_E_FAILEDTOCREATEFILE = (int)(0x80010137 - 0x100000000),
        /// <summary>
        /// Unable to close a serialization handle or a file handle.
        /// </summary>
        CO_E_FAILEDTOCLOSEHANDLE = (int)(0x80010138 - 0x100000000),
        /// <summary>
        /// The number of ACEs in an ACL exceeds the system limit.
        /// </summary>
        CO_E_EXCEEDSYSACLLIMIT = (int)(0x80010139 - 0x100000000),
        /// <summary>
        /// Not all the DENY_ACCESS ACEs are arranged in front of the GRANT_ACCESS ACEs in the stream.
        /// </summary>
        CO_E_ACESINWRONGORDER = (int)(0x8001013A - 0x100000000),
        /// <summary>
        /// The version of ACL format in the stream is not supported by this implementation of IAccessControl
        /// </summary>
        CO_E_INCOMPATIBLESTREAMVERSION = (int)(0x8001013B - 0x100000000),
        /// <summary>
        /// Unable to open the access token of the server process
        /// </summary>
        CO_E_FAILEDTOOPENPROCESSTOKEN = (int)(0x8001013C - 0x100000000),
        /// <summary>
        /// Unable to decode the ACL in the stream provided by the user
        /// </summary>
        CO_E_DECODEFAILED = (int)(0x8001013D - 0x100000000),
        /// <summary>
        /// The COM IAccessControl object is not initialized
        /// </summary>
        CO_E_ACNOTINITIALIZED = (int)(0x8001013F - 0x100000000),
        /// <summary>
        /// Call Cancellation is disabled
        /// </summary>
        CO_E_CANCEL_DISABLED = (int)(0x80010140 - 0x100000000),
        /// <summary>
        /// An internal error occurred.
        /// </summary>
        RPC_E_UNEXPECTED = (int)(0x8001FFFF - 0x100000000),
        /// <summary>
        /// The specified event is currently not being audited.
        /// </summary>
        ERROR_AUDITING_DISABLED = (int)(0xC0090001 - 0x100000000),
        /// <summary>
        /// The SID filtering operation removed all SIDs.
        /// </summary>
        ERROR_ALL_SIDS_FILTERED = (int)(0xC0090002 - 0x100000000),
        /// <summary>
        /// Bad UID.
        /// </summary>
        NTE_BAD_UID = (int)(0x80090001 - 0x100000000),
        /// <summary>
        /// Bad Hash.
        /// </summary>
        NTE_BAD_HASH = (int)(0x80090002 - 0x100000000),
        /// <summary>
        /// Bad Key.
        /// </summary>
        NTE_BAD_KEY = (int)(0x80090003 - 0x100000000),
        /// <summary>
        /// Bad Length.
        /// </summary>
        NTE_BAD_LEN = (int)(0x80090004 - 0x100000000),
        /// <summary>
        /// Bad Data.
        /// </summary>
        NTE_BAD_DATA = (int)(0x80090005 - 0x100000000),
        /// <summary>
        /// Invalid Signature.
        /// </summary>
        NTE_BAD_SIGNATURE = (int)(0x80090006 - 0x100000000),
        /// <summary>
        /// Bad Version of provider.
        /// </summary>
        NTE_BAD_VER = (int)(0x80090007 - 0x100000000),
        /// <summary>
        /// Invalid algorithm specified.
        /// </summary>
        NTE_BAD_ALGID = (int)(0x80090008 - 0x100000000),
        /// <summary>
        /// Invalid flags specified.
        /// </summary>
        NTE_BAD_FLAGS = (int)(0x80090009 - 0x100000000),
        /// <summary>
        /// Invalid type specified.
        /// </summary>
        NTE_BAD_TYPE = (int)(0x8009000A - 0x100000000),
        /// <summary>
        /// Key not valid for use in specified state.
        /// </summary>
        NTE_BAD_KEY_STATE = (int)(0x8009000B - 0x100000000),
        /// <summary>
        /// Hash not valid for use in specified state.
        /// </summary>
        NTE_BAD_HASH_STATE = (int)(0x8009000C - 0x100000000),
        /// <summary>
        /// Key does not exist.
        /// </summary>
        NTE_NO_KEY = (int)(0x8009000D - 0x100000000),
        /// <summary>
        /// Insufficient memory available for the operation.
        /// </summary>
        NTE_NO_MEMORY = (int)(0x8009000E - 0x100000000),
        /// <summary>
        /// Object already exists.
        /// </summary>
        NTE_EXISTS = (int)(0x8009000F - 0x100000000),
        /// <summary>
        /// Access denied.
        /// </summary>
        NTE_PERM = (int)(0x80090010 - 0x100000000),
        /// <summary>
        /// Object was not found.
        /// </summary>
        NTE_NOT_FOUND = (int)(0x80090011 - 0x100000000),
        /// <summary>
        /// Data already encrypted.
        /// </summary>
        NTE_DOUBLE_ENCRYPT = (int)(0x80090012 - 0x100000000),
        /// <summary>
        /// Invalid provider specified.
        /// </summary>
        NTE_BAD_PROVIDER = (int)(0x80090013 - 0x100000000),
        /// <summary>
        /// Invalid provider type specified.
        /// </summary>
        NTE_BAD_PROV_TYPE = (int)(0x80090014 - 0x100000000),
        /// <summary>
        /// Provider's public key is invalid.
        /// </summary>
        NTE_BAD_PUBLIC_KEY = (int)(0x80090015 - 0x100000000),
        /// <summary>
        /// Keyset does not exist
        /// </summary>
        NTE_BAD_KEYSET = (int)(0x80090016 - 0x100000000),
        /// <summary>
        /// Provider type not defined.
        /// </summary>
        NTE_PROV_TYPE_NOT_DEF = (int)(0x80090017 - 0x100000000),
        /// <summary>
        /// Provider type as registered is invalid.
        /// </summary>
        NTE_PROV_TYPE_ENTRY_BAD = (int)(0x80090018 - 0x100000000),
        /// <summary>
        /// The keyset is not defined.
        /// </summary>
        NTE_KEYSET_NOT_DEF = (int)(0x80090019 - 0x100000000),
        /// <summary>
        /// Keyset as registered is invalid.
        /// </summary>
        NTE_KEYSET_ENTRY_BAD = (int)(0x8009001A - 0x100000000),
        /// <summary>
        /// Provider type does not match registered value.
        /// </summary>
        NTE_PROV_TYPE_NO_MATCH = (int)(0x8009001B - 0x100000000),
        /// <summary>
        /// The digital signature file is corrupt.
        /// </summary>
        NTE_SIGNATURE_FILE_BAD = (int)(0x8009001C - 0x100000000),
        /// <summary>
        /// Provider DLL failed to initialize correctly.
        /// </summary>
        NTE_PROVIDER_DLL_FAIL = (int)(0x8009001D - 0x100000000),
        /// <summary>
        /// Provider DLL could not be found.
        /// </summary>
        NTE_PROV_DLL_NOT_FOUND = (int)(0x8009001E - 0x100000000),
        /// <summary>
        /// The Keyset parameter is invalid.
        /// </summary>
        NTE_BAD_KEYSET_PARAM = (int)(0x8009001F - 0x100000000),
        /// <summary>
        /// An internal error occurred.
        /// </summary>
        NTE_FAIL = (int)(0x80090020 - 0x100000000),
        /// <summary>
        /// A base error occurred.
        /// </summary>
        NTE_SYS_ERR = (int)(0x80090021 - 0x100000000),
        /// <summary>
        /// Provider could not perform the action since the context was acquired as silent.
        /// </summary>
        NTE_SILENT_CONTEXT = (int)(0x80090022 - 0x100000000),
        /// <summary>
        /// The security token does not have storage space available for an additional container.
        /// </summary>
        NTE_TOKEN_KEYSET_STORAGE_FULL = (int)(0x80090023 - 0x100000000),
        /// <summary>
        /// The profile for the user is a temporary profile.
        /// </summary>
        NTE_TEMPORARY_PROFILE = (int)(0x80090024 - 0x100000000),
        /// <summary>
        /// The key parameters could not be set because the CSP uses fixed parameters.
        /// </summary>
        NTE_FIXEDPARAMETER = (int)(0x80090025 - 0x100000000),
        /// <summary>
        /// Not enough memory is available to complete this request
        /// </summary>
        SEC_E_INSUFFICIENT_MEMORY = (int)(0x80090300 - 0x100000000),
        /// <summary>
        /// The handle specified is invalid
        /// </summary>
        SEC_E_INVALID_HANDLE = (int)(0x80090301 - 0x100000000),
        /// <summary>
        /// The function requested is not supported
        /// </summary>
        SEC_E_UNSUPPORTED_FUNCTION = (int)(0x80090302 - 0x100000000),
        /// <summary>
        /// The specified target is unknown or unreachable
        /// </summary>
        SEC_E_TARGET_UNKNOWN = (int)(0x80090303 - 0x100000000),
        /// <summary>
        /// The Local Security Authority cannot be contacted
        /// </summary>
        SEC_E_INTERNAL_ERROR = (int)(0x80090304 - 0x100000000),
        /// <summary>
        /// The requested security package does not exist
        /// </summary>
        SEC_E_SECPKG_NOT_FOUND = (int)(0x80090305 - 0x100000000),
        /// <summary>
        /// The caller is not the owner of the desired credentials
        /// </summary>
        SEC_E_NOT_OWNER = (int)(0x80090306 - 0x100000000),
        /// <summary>
        /// The security package failed to initialize, and cannot be installed
        /// </summary>
        SEC_E_CANNOT_INSTALL = (int)(0x80090307 - 0x100000000),
        /// <summary>
        /// The token supplied to the function is invalid
        /// </summary>
        SEC_E_INVALID_TOKEN = (int)(0x80090308 - 0x100000000),
        /// <summary>
        /// The security package is not able to marshall the logon buffer, so the logon attempt has failed
        /// </summary>
        SEC_E_CANNOT_PACK = (int)(0x80090309 - 0x100000000),
        /// <summary>
        /// The per-message Quality of Protection is not supported by the security package
        /// </summary>
        SEC_E_QOP_NOT_SUPPORTED = (int)(0x8009030A - 0x100000000),
        /// <summary>
        /// The security context does not allow impersonation of the client
        /// </summary>
        SEC_E_NO_IMPERSONATION = (int)(0x8009030B - 0x100000000),
        /// <summary>
        /// The logon attempt failed
        /// </summary>
        SEC_E_LOGON_DENIED = (int)(0x8009030C - 0x100000000),
        /// <summary>
        /// The credentials supplied to the package were not recognized
        /// </summary>
        SEC_E_UNKNOWN_CREDENTIALS = (int)(0x8009030D - 0x100000000),
        /// <summary>
        /// No credentials are available in the security package
        /// </summary>
        SEC_E_NO_CREDENTIALS = (int)(0x8009030E - 0x100000000),
        /// <summary>
        /// The message or signature supplied for verification has been altered
        /// </summary>
        SEC_E_MESSAGE_ALTERED = (int)(0x8009030F - 0x100000000),
        /// <summary>
        /// The message supplied for verification is out of sequence
        /// </summary>
        SEC_E_OUT_OF_SEQUENCE = (int)(0x80090310 - 0x100000000),
        /// <summary>
        /// No authority could be contacted for authentication.
        /// </summary>
        SEC_E_NO_AUTHENTICATING_AUTHORITY = (int)(0x80090311 - 0x100000000),
        /// <summary>
        /// The function completed successfully, but must be called again to complete the context
        /// </summary>
        SEC_I_CONTINUE_NEEDED = 0x00090312,
        /// <summary>
        /// The function completed successfully, but CompleteToken must be called
        /// </summary>
        SEC_I_COMPLETE_NEEDED = 0x00090313,
        /// <summary>
        /// The function completed successfully, but both CompleteToken and this function must be called to complete the context
        /// </summary>
        SEC_I_COMPLETE_AND_CONTINUE = 0x00090314,
        /// <summary>
        /// The logon was completed, but no network authority was available. The logon was made using locally known information
        /// </summary>
        SEC_I_LOCAL_LOGON = 0x00090315,
        /// <summary>
        /// The requested security package does not exist
        /// </summary>
        SEC_E_BAD_PKGID = (int)(0x80090316 - 0x100000000),
        /// <summary>
        /// The context has expired and can no longer be used.
        /// </summary>
        SEC_E_CONTEXT_EXPIRED = (int)(0x80090317 - 0x100000000),
        /// <summary>
        /// The context has expired and can no longer be used.
        /// </summary>
        SEC_I_CONTEXT_EXPIRED = 0x00090317,
        /// <summary>
        /// The supplied message is incomplete.  The signature was not verified.
        /// </summary>
        SEC_E_INCOMPLETE_MESSAGE = (int)(0x80090318 - 0x100000000),
        /// <summary>
        /// The credentials supplied were not complete, and could not be verified. The context could not be initialized.
        /// </summary>
        SEC_E_INCOMPLETE_CREDENTIALS = (int)(0x80090320 - 0x100000000),
        /// <summary>
        /// The buffers supplied to a function was too small.
        /// </summary>
        SEC_E_BUFFER_TOO_SMALL = (int)(0x80090321 - 0x100000000),
        /// <summary>
        /// The credentials supplied were not complete, and could not be verified. Additional information can be returned from the context.
        /// </summary>
        SEC_I_INCOMPLETE_CREDENTIALS = 0x00090320,
        /// <summary>
        /// The context data must be renegotiated with the peer.
        /// </summary>
        SEC_I_RENEGOTIATE = 0x00090321,
        /// <summary>
        /// The target principal name is incorrect.
        /// </summary>
        SEC_E_WRONG_PRINCIPAL = (int)(0x80090322 - 0x100000000),
        /// <summary>
        /// There is no LSA mode context associated with this context.
        /// </summary>
        SEC_I_NO_LSA_CONTEXT = 0x00090323,
        /// <summary>
        /// The clocks on the client and server machines are skewed.
        /// </summary>
        SEC_E_TIME_SKEW = (int)(0x80090324 - 0x100000000),
        /// <summary>
        /// The certificate chain was issued by an authority that is not trusted.
        /// </summary>
        SEC_E_UNTRUSTED_ROOT = (int)(0x80090325 - 0x100000000),
        /// <summary>
        /// The message received was unexpected or badly formatted.
        /// </summary>
        SEC_E_ILLEGAL_MESSAGE = (int)(0x80090326 - 0x100000000),
        /// <summary>
        /// An unknown error occurred while processing the certificate.
        /// </summary>
        SEC_E_CERT_UNKNOWN = (int)(0x80090327 - 0x100000000),
        /// <summary>
        /// The received certificate has expired.
        /// </summary>
        SEC_E_CERT_EXPIRED = (int)(0x80090328 - 0x100000000),
        /// <summary>
        /// The specified data could not be encrypted.
        /// </summary>
        SEC_E_ENCRYPT_FAILURE = (int)(0x80090329 - 0x100000000),
        /// <summary>
        /// The specified data could not be decrypted.
        /// </summary>
        SEC_E_DECRYPT_FAILURE = (int)(0x80090330 - 0x100000000),
        /// <summary>
        /// The client and server cannot communicate, because they do not possess a common algorithm.
        /// </summary>
        SEC_E_ALGORITHM_MISMATCH = (int)(0x80090331 - 0x100000000),
        /// <summary>
        /// The security context could not be established due to a failure in the requested quality of service (e.g. mutual authentication or delegation).
        /// </summary>
        SEC_E_SECURITY_QOS_FAILED = (int)(0x80090332 - 0x100000000),
        /// <summary>
        /// A security context was deleted before the context was completed.  This is considered a logon failure.
        /// </summary>
        SEC_E_UNFINISHED_CONTEXT_DELETED = (int)(0x80090333 - 0x100000000),
        /// <summary>
        /// The client is trying to negotiate a context and the server requires user-to-user but didn't send a TGT reply.
        /// </summary>
        SEC_E_NO_TGT_REPLY = (int)(0x80090334 - 0x100000000),
        /// <summary>
        /// Unable to accomplish the requested task because the local machine does not have any IP addresses.
        /// </summary>
        SEC_E_NO_IP_ADDRESSES = (int)(0x80090335 - 0x100000000),
        /// <summary>
        /// The supplied credential handle does not match the credential associated with the security context.
        /// </summary>
        SEC_E_WRONG_CREDENTIAL_HANDLE = (int)(0x80090336 - 0x100000000),
        /// <summary>
        /// The crypto system or checksum function is invalid because a required function is unavailable.
        /// </summary>
        SEC_E_CRYPTO_SYSTEM_INVALID = (int)(0x80090337 - 0x100000000),
        /// <summary>
        /// The number of maximum ticket referrals has been exceeded.
        /// </summary>
        SEC_E_MAX_REFERRALS_EXCEEDED = (int)(0x80090338 - 0x100000000),
        /// <summary>
        /// The local machine must be a Kerberos KDC (domain controller) and it is not.
        /// </summary>
        SEC_E_MUST_BE_KDC = (int)(0x80090339 - 0x100000000),
        /// <summary>
        /// The other end of the security negotiation is requires strong crypto but it is not supported on the local machine.
        /// </summary>
        SEC_E_STRONG_CRYPTO_NOT_SUPPORTED = (int)(0x8009033A - 0x100000000),
        /// <summary>
        /// The KDC reply contained more than one principal name.
        /// </summary>
        SEC_E_TOO_MANY_PRINCIPALS = (int)(0x8009033B - 0x100000000),
        /// <summary>
        /// Expected to find PA data for a hint of what etype to use, but it was not found.
        /// </summary>
        SEC_E_NO_PA_DATA = (int)(0x8009033C - 0x100000000),
        /// <summary>
        /// The client cert name does not matches the user name or the KDC name is incorrect.
        /// </summary>
        SEC_E_PKINIT_NAME_MISMATCH = (int)(0x8009033D - 0x100000000),
        /// <summary>
        /// Smartcard logon is required and was not used.
        /// </summary>
        SEC_E_SMARTCARD_LOGON_REQUIRED = (int)(0x8009033E - 0x100000000),
        /// <summary>
        /// A system shutdown is in progress.
        /// </summary>
        SEC_E_SHUTDOWN_IN_PROGRESS = (int)(0x8009033F - 0x100000000),
        /// <summary>
        /// An invalid request was sent to the KDC.
        /// </summary>
        SEC_E_KDC_INVALID_REQUEST = (int)(0x80090340 - 0x100000000),
        /// <summary>
        /// The KDC was unable to generate a referral for the service requested.
        /// </summary>
        SEC_E_KDC_UNABLE_TO_REFER = (int)(0x80090341 - 0x100000000),
        /// <summary>
        /// The encryption type requested is not supported by the KDC.
        /// </summary>
        SEC_E_KDC_UNKNOWN_ETYPE = (int)(0x80090342 - 0x100000000),
        /// <summary>
        /// An unsupported preauthentication mechanism was presented to the kerberos package.
        /// </summary>
        SEC_E_UNSUPPORTED_PREAUTH = (int)(0x80090343 - 0x100000000),
        /// <summary>
        /// The requested operation requires delegation to be enabled on the machine.
        /// </summary>
        SEC_E_DELEGATION_REQUIRED = (int)(0x80090345 - 0x100000000),
        /// <summary>
        /// Client's supplied SSPI channel bindings were incorrect.
        /// </summary>
        SEC_E_BAD_BINDINGS = (int)(0x80090346 - 0x100000000),
        /// <summary>
        /// The received certificate was mapped to multiple accounts.
        /// </summary>
        SEC_E_MULTIPLE_ACCOUNTS = (int)(0x80090347 - 0x100000000),
        /// <summary>
        /// SEC_E_NO_KERB_KEY
        /// </summary>
        SEC_E_NO_KERB_KEY = (int)(0x80090348 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_CERT_WRONG_USAGE = (int)(0x80090349 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_DOWNGRADE_DETECTED = (int)(0x80090350 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_SMARTCARD_CERT_REVOKED = (int)(0x80090351 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_ISSUING_CA_UNTRUSTED = (int)(0x80090352 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_REVOCATION_OFFLINE_C = (int)(0x80090353 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_PKINIT_CLIENT_FAILURE = (int)(0x80090354 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_SMARTCARD_CERT_EXPIRED = (int)(0x80090355 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_NO_S4U_PROT_SUPPORT = (int)(0x80090356 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_CROSSREALM_DELEGATION_FAILURE = (int)(0x80090357 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_NO_SPM = SEC_E_INTERNAL_ERROR,
        /// <summary>
        /// No information avialable.
        /// </summary>
        SEC_E_NOT_SUPPORTED = SEC_E_UNSUPPORTED_FUNCTION,
        /// <summary>
        /// An error occurred while performing an operation on a cryptographic message.
        /// </summary>
        CRYPT_E_MSG_ERROR = (int)(0x80091001 - 0x100000000),
        /// <summary>
        /// Unknown cryptographic algorithm.
        /// </summary>
        CRYPT_E_UNKNOWN_ALGO = (int)(0x80091002 - 0x100000000),
        /// <summary>
        /// The object identifier is poorly formatted.
        /// </summary>
        CRYPT_E_OID_FORMAT = (int)(0x80091003 - 0x100000000),
        /// <summary>
        /// Invalid cryptographic message type.
        /// </summary>
        CRYPT_E_INVALID_MSG_TYPE = (int)(0x80091004 - 0x100000000),
        /// <summary>
        /// Unexpected cryptographic message encoding.
        /// </summary>
        CRYPT_E_UNEXPECTED_ENCODING = (int)(0x80091005 - 0x100000000),
        /// <summary>
        /// The cryptographic message does not contain an expected authenticated attribute.
        /// </summary>
        CRYPT_E_AUTH_ATTR_MISSING = (int)(0x80091006 - 0x100000000),
        /// <summary>
        /// The hash value is not correct.
        /// </summary>
        CRYPT_E_HASH_VALUE = (int)(0x80091007 - 0x100000000),
        /// <summary>
        /// The index value is not valid.
        /// </summary>
        CRYPT_E_INVALID_INDEX = (int)(0x80091008 - 0x100000000),
        /// <summary>
        /// The content of the cryptographic message has already been decrypted.
        /// </summary>
        CRYPT_E_ALREADY_DECRYPTED = (int)(0x80091009 - 0x100000000),
        /// <summary>
        /// The content of the cryptographic message has not been decrypted yet.
        /// </summary>
        CRYPT_E_NOT_DECRYPTED = (int)(0x8009100A - 0x100000000),
        /// <summary>
        /// The enveloped-data message does not contain the specified recipient.
        /// </summary>
        CRYPT_E_RECIPIENT_NOT_FOUND = (int)(0x8009100B - 0x100000000),
        /// <summary>
        /// Invalid control type.
        /// </summary>
        CRYPT_E_CONTROL_TYPE = (int)(0x8009100C - 0x100000000),
        /// <summary>
        /// Invalid issuer and/or serial number.
        /// </summary>
        CRYPT_E_ISSUER_SERIALNUMBER = (int)(0x8009100D - 0x100000000),
        /// <summary>
        /// Cannot find the original signer.
        /// </summary>
        CRYPT_E_SIGNER_NOT_FOUND = (int)(0x8009100E - 0x100000000),
        /// <summary>
        /// The cryptographic message does not contain all of the requested attributes.
        /// </summary>
        CRYPT_E_ATTRIBUTES_MISSING = (int)(0x8009100F - 0x100000000),
        /// <summary>
        /// The streamed cryptographic message is not ready to return data.
        /// </summary>
        CRYPT_E_STREAM_MSG_NOT_READY = (int)(0x80091010 - 0x100000000),
        /// <summary>
        /// The streamed cryptographic message requires more data to complete the decode operation.
        /// </summary>
        CRYPT_E_STREAM_INSUFFICIENT_DATA = (int)(0x80091011 - 0x100000000),
        /// <summary>
        /// The protected data needs to be re-protected.
        /// </summary>
        CRYPT_I_NEW_PROTECTION_REQUIRED = 0x00091012,
        /// <summary>
        /// The length specified for the output data was insufficient.
        /// </summary>
        CRYPT_E_BAD_LEN = (int)(0x80092001 - 0x100000000),
        /// <summary>
        /// An error occurred during encode or decode operation.
        /// </summary>
        CRYPT_E_BAD_ENCODE = (int)(0x80092002 - 0x100000000),
        /// <summary>
        /// An error occurred while reading or writing to a file.
        /// </summary>
        CRYPT_E_FILE_ERROR = (int)(0x80092003 - 0x100000000),
        /// <summary>
        /// Cannot find object or property.
        /// </summary>
        CRYPT_E_NOT_FOUND = (int)(0x80092004 - 0x100000000),
        /// <summary>
        /// The object or property already exists.
        /// </summary>
        CRYPT_E_EXISTS = (int)(0x80092005 - 0x100000000),
        /// <summary>
        /// No provider was specified for the store or object.
        /// </summary>
        CRYPT_E_NO_PROVIDER = (int)(0x80092006 - 0x100000000),
        /// <summary>
        /// The specified certificate is self signed.
        /// </summary>
        CRYPT_E_SELF_SIGNED = (int)(0x80092007 - 0x100000000),
        /// <summary>
        /// The previous certificate or CRL context was deleted.
        /// </summary>
        CRYPT_E_DELETED_PREV = (int)(0x80092008 - 0x100000000),
        /// <summary>
        /// Cannot find the requested object.
        /// </summary>
        CRYPT_E_NO_MATCH = (int)(0x80092009 - 0x100000000),
        /// <summary>
        /// The certificate does not have a property that references a private key.
        /// </summary>
        CRYPT_E_UNEXPECTED_MSG_TYPE = (int)(0x8009200A - 0x100000000),
        /// <summary>
        /// Cannot find the certificate and private key for decryption.
        /// </summary>
        CRYPT_E_NO_KEY_PROPERTY = (int)(0x8009200B - 0x100000000),
        /// <summary>
        /// Cannot find the certificate and private key to use for decryption.
        /// </summary>
        CRYPT_E_NO_DECRYPT_CERT = (int)(0x8009200C - 0x100000000),
        /// <summary>
        /// Not a cryptographic message or the cryptographic message is not formatted correctly.
        /// </summary>
        CRYPT_E_BAD_MSG = (int)(0x8009200D - 0x100000000),
        /// <summary>
        /// The signed cryptographic message does not have a signer for the specified signer index.
        /// </summary>
        CRYPT_E_NO_SIGNER = (int)(0x8009200E - 0x100000000),
        /// <summary>
        /// Final closure is pending until additional frees or closes.
        /// </summary>
        CRYPT_E_PENDING_CLOSE = (int)(0x8009200F - 0x100000000),
        /// <summary>
        /// The certificate is revoked.
        /// </summary>
        CRYPT_E_REVOKED = (int)(0x80092010 - 0x100000000),
        /// <summary>
        /// No Dll or exported function was found to verify revocation.
        /// </summary>
        CRYPT_E_NO_REVOCATION_DLL = (int)(0x80092011 - 0x100000000),
        /// <summary>
        /// The revocation function was unable to check revocation for the certificate.
        /// </summary>
        CRYPT_E_NO_REVOCATION_CHECK = (int)(0x80092012 - 0x100000000),
        /// <summary>
        /// The revocation function was unable to check revocation because the revocation server was offline.
        /// </summary>
        CRYPT_E_REVOCATION_OFFLINE = (int)(0x80092013 - 0x100000000),
        /// <summary>
        /// The certificate is not in the revocation server's database.
        /// </summary>
        CRYPT_E_NOT_IN_REVOCATION_DATABASE = (int)(0x80092014 - 0x100000000),
        /// <summary>
        /// The string contains a non-numeric character.
        /// </summary>
        CRYPT_E_INVALID_NUMERIC_STRING = (int)(0x80092020 - 0x100000000),
        /// <summary>
        /// The string contains a non-printable character.
        /// </summary>
        CRYPT_E_INVALID_PRINTABLE_STRING = (int)(0x80092021 - 0x100000000),
        /// <summary>
        /// The string contains a character not in the 7 bit ASCII character set.
        /// </summary>
        CRYPT_E_INVALID_IA5_STRING = (int)(0x80092022 - 0x100000000),
        /// <summary>
        /// The string contains an invalid X500 name attribute key, oid, value or delimiter.
        /// </summary>
        CRYPT_E_INVALID_X500_STRING = (int)(0x80092023 - 0x100000000),
        /// <summary>
        /// The dwValueType for the CERT_NAME_VALUE is not one of the character strings.  Most likely it is either a CERT_RDN_ENCODED_BLOB or CERT_TDN_OCTED_STRING.
        /// </summary>
        CRYPT_E_NOT_CHAR_STRING = (int)(0x80092024 - 0x100000000),
        /// <summary>
        /// The Put operation can not continue.  The file needs to be resized.  However, there is already a signature present.  A complete signing operation must be done.
        /// </summary>
        CRYPT_E_FILERESIZED = (int)(0x80092025 - 0x100000000),
        /// <summary>
        /// The cryptographic operation failed due to a local security option setting.
        /// </summary>
        CRYPT_E_SECURITY_SETTINGS = (int)(0x80092026 - 0x100000000),
        /// <summary>
        /// No DLL or exported function was found to verify subject usage.
        /// </summary>
        CRYPT_E_NO_VERIFY_USAGE_DLL = (int)(0x80092027 - 0x100000000),
        /// <summary>
        /// The called function was unable to do a usage check on the subject.
        /// </summary>
        CRYPT_E_NO_VERIFY_USAGE_CHECK = (int)(0x80092028 - 0x100000000),
        /// <summary>
        /// Since the server was offline, the called function was unable to complete the usage check.
        /// </summary>
        CRYPT_E_VERIFY_USAGE_OFFLINE = (int)(0x80092029 - 0x100000000),
        /// <summary>
        /// The subject was not found in a Certificate Trust List (CTL).
        /// </summary>
        CRYPT_E_NOT_IN_CTL = (int)(0x8009202A - 0x100000000),
        /// <summary>
        /// None of the signers of the cryptographic message or certificate trust list is trusted.
        /// </summary>
        CRYPT_E_NO_TRUSTED_SIGNER = (int)(0x8009202B - 0x100000000),
        /// <summary>
        /// The public key's algorithm parameters are missing.
        /// </summary>
        CRYPT_E_MISSING_PUBKEY_PARA = (int)(0x8009202C - 0x100000000),
        /// <summary>
        /// OSS Certificate encode/decode error code base
        ///
        /// See asn1code.h for a definition of the OSS runtime errors. The OSS
        /// error values are offset by CRYPT_E_OSS_ERROR.
        /// </summary>
        CRYPT_E_OSS_ERROR = (int)(0x80093000 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Output Buffer is too small.
        /// </summary>
        OSS_MORE_BUF = (int)(0x80093001 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Signed integer is encoded as a unsigned integer.
        /// </summary>
        OSS_NEGATIVE_UINTEGER = (int)(0x80093002 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Unknown ASN.1 data type.
        /// </summary>
        OSS_PDU_RANGE = (int)(0x80093003 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Output buffer is too small, the decoded data has been truncated.
        /// </summary>
        OSS_MORE_INPUT = (int)(0x80093004 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_DATA_ERROR = (int)(0x80093005 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid argument.
        /// </summary>
        OSS_BAD_ARG = (int)(0x80093006 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Encode/Decode version mismatch.
        /// </summary>
        OSS_BAD_VERSION = (int)(0x80093007 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Out of memory.
        /// </summary>
        OSS_OUT_MEMORY = (int)(0x80093008 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Encode/Decode Error.
        /// </summary>
        OSS_PDU_MISMATCH = (int)(0x80093009 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Internal Error.
        /// </summary>
        OSS_LIMITED = (int)(0x8009300A - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_BAD_PTR = (int)(0x8009300B - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_BAD_TIME = (int)(0x8009300C - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Unsupported BER indefinite-length encoding.
        /// </summary>
        OSS_INDEFINITE_NOT_SUPPORTED = (int)(0x8009300D - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Access violation.
        /// </summary>
        OSS_MEM_ERROR = (int)(0x8009300E - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_BAD_TABLE = (int)(0x8009300F - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_TOO_Int32 = (int)(0x80093010 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_CONSTRAINT_VIOLATED = (int)(0x80093011 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Internal Error.
        /// </summary>
        OSS_FATAL_ERROR = (int)(0x80093012 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Multi-threading conflict.
        /// </summary>
        OSS_ACCESS_SERIALIZATION_ERROR = (int)(0x80093013 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_NULL_TBL = (int)(0x80093014 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_NULL_FCN = (int)(0x80093015 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_BAD_ENCRULES = (int)(0x80093016 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Encode/Decode function not implemented.
        /// </summary>
        OSS_UNAVAIL_ENCRULES = (int)(0x80093017 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Trace file error.
        /// </summary>
        OSS_CANT_OPEN_TRACE_WINDOW = (int)(0x80093018 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Function not implemented.
        /// </summary>
        OSS_UNIMPLEMENTED = (int)(0x80093019 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_OID_DLL_NOT_LINKED = (int)(0x8009301A - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Trace file error.
        /// </summary>
        OSS_CANT_OPEN_TRACE_FILE = (int)(0x8009301B - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Trace file error.
        /// </summary>
        OSS_TRACE_FILE_ALREADY_OPEN = (int)(0x8009301C - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_TABLE_MISMATCH = (int)(0x8009301D - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Invalid data.
        /// </summary>
        OSS_TYPE_NOT_SUPPORTED = (int)(0x8009301E - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_REAL_DLL_NOT_LINKED = (int)(0x8009301F - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_REAL_CODE_NOT_LINKED = (int)(0x80093020 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_OUT_OF_RANGE = (int)(0x80093021 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_COPIER_DLL_NOT_LINKED = (int)(0x80093022 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_CONSTRAINT_DLL_NOT_LINKED = (int)(0x80093023 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_COMPARATOR_DLL_NOT_LINKED = (int)(0x80093024 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_COMPARATOR_CODE_NOT_LINKED = (int)(0x80093025 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_MEM_MGR_DLL_NOT_LINKED = (int)(0x80093026 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_PDV_DLL_NOT_LINKED = (int)(0x80093027 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_PDV_CODE_NOT_LINKED = (int)(0x80093028 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_API_DLL_NOT_LINKED = (int)(0x80093029 - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_BERDER_DLL_NOT_LINKED = (int)(0x8009302A - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_PER_DLL_NOT_LINKED = (int)(0x8009302B - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Program link error.
        /// </summary>
        OSS_OPEN_TYPE_ERROR = (int)(0x8009302C - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: System resource error.
        /// </summary>
        OSS_MUTEX_NOT_CREATED = (int)(0x8009302D - 0x100000000),
        /// <summary>
        /// OSS ASN.1 Error: Trace file error.
        /// </summary>
        OSS_CANT_CLOSE_TRACE_FILE = (int)(0x8009302E - 0x100000000),
        /// <summary>
        /// ASN1 Certificate encode/decode error code base.
        ///
        /// The ASN1 error values are offset by CRYPT_E_ASN1_ERROR.
        /// </summary>
        CRYPT_E_ASN1_ERROR = (int)(0x80093100 - 0x100000000),
        /// <summary>
        /// ASN1 internal encode or decode error.
        /// </summary>
        CRYPT_E_ASN1_INTERNAL = (int)(0x80093101 - 0x100000000),
        /// <summary>
        /// ASN1 unexpected end of data.
        /// </summary>
        CRYPT_E_ASN1_EOD = (int)(0x80093102 - 0x100000000),
        /// <summary>
        /// ASN1 corrupted data.
        /// </summary>
        CRYPT_E_ASN1_CORRUPT = (int)(0x80093103 - 0x100000000),
        /// <summary>
        /// ASN1 value too large.
        /// </summary>
        CRYPT_E_ASN1_LARGE = (int)(0x80093104 - 0x100000000),
        /// <summary>
        /// ASN1 constraint violated.
        /// </summary>
        CRYPT_E_ASN1_CONSTRAINT = (int)(0x80093105 - 0x100000000),
        /// <summary>
        /// ASN1 out of memory.
        /// </summary>
        CRYPT_E_ASN1_MEMORY = (int)(0x80093106 - 0x100000000),
        /// <summary>
        /// ASN1 buffer overflow.
        /// </summary>
        CRYPT_E_ASN1_OVERFLOW = (int)(0x80093107 - 0x100000000),
        /// <summary>
        /// ASN1 function not supported for this PDU.
        /// </summary>
        CRYPT_E_ASN1_BADPDU = (int)(0x80093108 - 0x100000000),
        /// <summary>
        /// ASN1 bad arguments to function call.
        /// </summary>
        CRYPT_E_ASN1_BADARGS = (int)(0x80093109 - 0x100000000),
        /// <summary>
        /// ASN1 bad real value.
        /// </summary>
        CRYPT_E_ASN1_BADREAL = (int)(0x8009310A - 0x100000000),
        /// <summary>
        /// ASN1 bad tag value met.
        /// </summary>
        CRYPT_E_ASN1_BADTAG = (int)(0x8009310B - 0x100000000),
        /// <summary>
        /// ASN1 bad choice value.
        /// </summary>
        CRYPT_E_ASN1_CHOICE = (int)(0x8009310C - 0x100000000),
        /// <summary>
        /// ASN1 bad encoding rule.
        /// </summary>
        CRYPT_E_ASN1_RULE = (int)(0x8009310D - 0x100000000),
        /// <summary>
        /// ASN1 bad unicode (UTF8).
        /// </summary>
        CRYPT_E_ASN1_UTF8 = (int)(0x8009310E - 0x100000000),
        /// <summary>
        /// ASN1 bad PDU type.
        /// </summary>
        CRYPT_E_ASN1_PDU_TYPE = (int)(0x80093133 - 0x100000000),
        /// <summary>
        /// ASN1 not yet implemented.
        /// </summary>
        CRYPT_E_ASN1_NYI = (int)(0x80093134 - 0x100000000),
        /// <summary>
        /// ASN1 skipped unknown extension(s).
        /// </summary>
        CRYPT_E_ASN1_EXTENDED = (int)(0x80093201 - 0x100000000),
        /// <summary>
        /// ASN1 end of data expected
        /// </summary>
        CRYPT_E_ASN1_NOEOD = (int)(0x80093202 - 0x100000000),
        /// <summary>
        /// The request subject name is invalid or too long.
        /// </summary>
        CERTSRV_E_BAD_REQUESTSUBJECT = (int)(0x80094001 - 0x100000000),
        /// <summary>
        /// The request does not exist.
        /// </summary>
        CERTSRV_E_NO_REQUEST = (int)(0x80094002 - 0x100000000),
        /// <summary>
        /// The request's current status does not allow this operation.
        /// </summary>
        CERTSRV_E_BAD_REQUESTSTATUS = (int)(0x80094003 - 0x100000000),
        /// <summary>
        /// The requested property value is empty.
        /// </summary>
        CERTSRV_E_PROPERTY_EMPTY = (int)(0x80094004 - 0x100000000),
        /// <summary>
        /// The certification authority's certificate contains invalid data.
        /// </summary>
        CERTSRV_E_INVALID_CA_CERTIFICATE = (int)(0x80094005 - 0x100000000),
        /// <summary>
        /// Certificate service has been suspended for a database restore operation.
        /// </summary>
        CERTSRV_E_SERVER_SUSPENDED = (int)(0x80094006 - 0x100000000),
        /// <summary>
        /// The certificate contains an encoded length that is potentially incompatible with older enrollment software.
        /// </summary>
        CERTSRV_E_ENCODING_LENGTH = (int)(0x80094007 - 0x100000000),
        /// <summary>
        /// The operation is denied. The user has multiple roles assigned and the certification authority is configured to enforce role separation.
        /// </summary>
        CERTSRV_E_ROLECONFLICT = (int)(0x80094008 - 0x100000000),
        /// <summary>
        /// The operation is denied. It can only be performed by a certificate manager that is allowed to manage certificates for the current requester.
        /// </summary>
        CERTSRV_E_RESTRICTEDOFFICER = (int)(0x80094009 - 0x100000000),
        /// <summary>
        /// Cannot archive private key.  The certification authority is not configured for key archival.
        /// </summary>
        CERTSRV_E_KEY_ARCHIVAL_NOT_CONFIGURED = (int)(0x8009400A - 0x100000000),
        /// <summary>
        /// Cannot archive private key.  The certification authority could not verify one or more key recovery certificates.
        /// </summary>
        CERTSRV_E_NO_VALID_KRA = (int)(0x8009400B - 0x100000000),
        /// <summary>
        /// The request is incorrectly formatted.  The encrypted private key must be in an unauthenticated attribute in an outermost signature.
        /// </summary>
        CERTSRV_E_BAD_REQUEST_KEY_ARCHIVAL = (int)(0x8009400C - 0x100000000),
        /// <summary>
        /// At least one security principal must have the permission to manage this CA.
        /// </summary>
        CERTSRV_E_NO_CAADMIN_DEFINED = (int)(0x8009400D - 0x100000000),
        /// <summary>
        /// The request contains an invalid renewal certificate attribute.
        /// </summary>
        CERTSRV_E_BAD_RENEWAL_CERT_ATTRIBUTE = (int)(0x8009400E - 0x100000000),
        /// <summary>
        /// An attempt was made to open a Certification Authority database session, but there are already too many active sessions.  The server may need to be configured to allow additional sessions.
        /// </summary>
        CERTSRV_E_NO_DB_SESSIONS = (int)(0x8009400F - 0x100000000),
        /// <summary>
        /// A memory reference caused a data alignment fault.
        /// </summary>
        CERTSRV_E_ALIGNMENT_FAULT = (int)(0x80094010 - 0x100000000),
        /// <summary>
        /// The permissions on this certification authority do not allow the current user to enroll for certificates.
        /// </summary>
        CERTSRV_E_ENROLL_DENIED = (int)(0x80094011 - 0x100000000),
        /// <summary>
        /// The permissions on the certificate template do not allow the current user to enroll for this type of certificate.
        /// </summary>
        CERTSRV_E_TEMPLATE_DENIED = (int)(0x80094012 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CERTSRV_E_DOWNLEVEL_DC_SSL_OR_UPGRADE = (int)(0x80094013 - 0x100000000),
        /// <summary>
        /// The requested certificate template is not supported by this CA.
        /// </summary>
        CERTSRV_E_UNSUPPORTED_CERT_TYPE = (int)(0x80094800 - 0x100000000),
        /// <summary>
        /// The request contains no certificate template information.
        /// </summary>
        CERTSRV_E_NO_CERT_TYPE = (int)(0x80094801 - 0x100000000),
        /// <summary>
        /// The request contains conflicting template information.
        /// </summary>
        CERTSRV_E_TEMPLATE_CONFLICT = (int)(0x80094802 - 0x100000000),
        /// <summary>
        /// The request is missing a required Subject Alternate name extension.
        /// </summary>
        CERTSRV_E_SUBJECT_ALT_NAME_REQUIRED = (int)(0x80094803 - 0x100000000),
        /// <summary>
        /// The request is missing a required private key for archival by the server.
        /// </summary>
        CERTSRV_E_ARCHIVED_KEY_REQUIRED = (int)(0x80094804 - 0x100000000),
        /// <summary>
        /// The request is missing a required SMIME capabilities extension.
        /// </summary>
        CERTSRV_E_SMIME_REQUIRED = (int)(0x80094805 - 0x100000000),
        /// <summary>
        /// The request was made on behalf of a subject other than the caller.  The certificate template must be configured to require at least one signature to authorize the request.
        /// </summary>
        CERTSRV_E_BAD_RENEWAL_SUBJECT = (int)(0x80094806 - 0x100000000),
        /// <summary>
        /// The request template version is newer than the supported template version.
        /// </summary>
        CERTSRV_E_BAD_TEMPLATE_VERSION = (int)(0x80094807 - 0x100000000),
        /// <summary>
        /// The template is missing a required signature policy attribute.
        /// </summary>
        CERTSRV_E_TEMPLATE_POLICY_REQUIRED = (int)(0x80094808 - 0x100000000),
        /// <summary>
        /// The request is missing required signature policy information.
        /// </summary>
        CERTSRV_E_SIGNATURE_POLICY_REQUIRED = (int)(0x80094809 - 0x100000000),
        /// <summary>
        /// The request is missing one or more required signatures.
        /// </summary>
        CERTSRV_E_SIGNATURE_COUNT = (int)(0x8009480A - 0x100000000),
        /// <summary>
        /// One or more signatures did not include the required application or issuance policies.  The request is missing one or more required valid signatures.
        /// </summary>
        CERTSRV_E_SIGNATURE_REJECTED = (int)(0x8009480B - 0x100000000),
        /// <summary>
        /// The request is missing one or more required signature issuance policies.
        /// </summary>
        CERTSRV_E_ISSUANCE_POLICY_REQUIRED = (int)(0x8009480C - 0x100000000),
        /// <summary>
        /// The UPN is unavailable and cannot be added to the Subject Alternate name.
        /// </summary>
        CERTSRV_E_SUBJECT_UPN_REQUIRED = (int)(0x8009480D - 0x100000000),
        /// <summary>
        /// The Active Directory GUID is unavailable and cannot be added to the Subject Alternate name.
        /// </summary>
        CERTSRV_E_SUBJECT_DIRECTORY_GUID_REQUIRED = (int)(0x8009480E - 0x100000000),
        /// <summary>
        /// The DNS name is unavailable and cannot be added to the Subject Alternate name.
        /// </summary>
        CERTSRV_E_SUBJECT_DNS_REQUIRED = (int)(0x8009480F - 0x100000000),
        /// <summary>
        /// The request includes a private key for archival by the server, but key archival is not enabled for the specified certificate template.
        /// </summary>
        CERTSRV_E_ARCHIVED_KEY_UNEXPECTED = (int)(0x80094810 - 0x100000000),
        /// <summary>
        /// The public key does not meet the minimum size required by the specified certificate template.
        /// </summary>
        CERTSRV_E_KEY_LENGTH = (int)(0x80094811 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CERTSRV_E_SUBJECT_EMAIL_REQUIRED = (int)(0x80094812 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CERTSRV_E_UNKNOWN_CERT_TYPE = (int)(0x80094813 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        CERTSRV_E_CERT_TYPE_OVERLAP = (int)(0x80094814 - 0x100000000),
        /// <summary>
        /// The key is not exportable.
        /// </summary>
        XENROLL_E_KEY_NOT_EXPORTABLE = (int)(0x80095000 - 0x100000000),
        /// <summary>
        /// You cannot add the root CA certificate into your local store.
        /// </summary>
        XENROLL_E_CANNOT_ADD_ROOT_CERT = (int)(0x80095001 - 0x100000000),
        /// <summary>
        /// The key archival hash attribute was not found in the response.
        /// </summary>
        XENROLL_E_RESPONSE_KA_HASH_NOT_FOUND = (int)(0x80095002 - 0x100000000),
        /// <summary>
        /// An unexpetced key archival hash attribute was found in the response.
        /// </summary>
        XENROLL_E_RESPONSE_UNEXPECTED_KA_HASH = (int)(0x80095003 - 0x100000000),
        /// <summary>
        /// There is a key archival hash mismatch between the request and the response.
        /// </summary>
        XENROLL_E_RESPONSE_KA_HASH_MISMATCH = (int)(0x80095004 - 0x100000000),
        /// <summary>
        /// Signing certificate cannot include SMIME extension.
        /// </summary>
        XENROLL_E_KEYSPEC_SMIME_MISMATCH = (int)(0x80095005 - 0x100000000),
        /// <summary>
        /// A system-level error occurred while verifying trust.
        /// </summary>
        TRUST_E_SYSTEM_ERROR = (int)(0x80096001 - 0x100000000),
        /// <summary>
        /// The certificate for the signer of the message is invalid or not found.
        /// </summary>
        TRUST_E_NO_SIGNER_CERT = (int)(0x80096002 - 0x100000000),
        /// <summary>
        /// One of the counter signatures was invalid.
        /// </summary>
        TRUST_E_COUNTER_SIGNER = (int)(0x80096003 - 0x100000000),
        /// <summary>
        /// The signature of the certificate can not be verified.
        /// </summary>
        TRUST_E_CERT_SIGNATURE = (int)(0x80096004 - 0x100000000),
        /// <summary>
        /// The timestamp signature and/or certificate could not be verified or is malformed.
        /// </summary>
        TRUST_E_TIME_STAMP = (int)(0x80096005 - 0x100000000),
        /// <summary>
        /// The digital signature of the object did not verify.
        /// </summary>
        TRUST_E_BAD_DIGEST = (int)(0x80096010 - 0x100000000),
        /// <summary>
        /// A certificate's basic constraint extension has not been observed.
        /// </summary>
        TRUST_E_BASIC_CONSTRAINTS = (int)(0x80096019 - 0x100000000),
        /// <summary>
        /// The certificate does not meet or contain the Authenticode financial extensions.
        /// </summary>
        TRUST_E_FINANCIAL_CRITERIA = (int)(0x8009601E - 0x100000000),
        /// <summary>
        /// Tried to reference a part of the file outside the proper range.
        /// </summary>
        MSSIPOTF_E_OUTOFMEMRANGE = (int)(0x80097001 - 0x100000000),
        /// <summary>
        /// Could not retrieve an object from the file.
        /// </summary>
        MSSIPOTF_E_CANTGETOBJECT = (int)(0x80097002 - 0x100000000),
        /// <summary>
        /// Could not find the head table in the file.
        /// </summary>
        MSSIPOTF_E_NOHEADTABLE = (int)(0x80097003 - 0x100000000),
        /// <summary>
        /// The magic number in the head table is incorrect.
        /// </summary>
        MSSIPOTF_E_BAD_MAGICNUMBER = (int)(0x80097004 - 0x100000000),
        /// <summary>
        /// The offset table has incorrect values.
        /// </summary>
        MSSIPOTF_E_BAD_OFFSET_TABLE = (int)(0x80097005 - 0x100000000),
        /// <summary>
        /// Duplicate table tags or tags out of alphabetical order.
        /// </summary>
        MSSIPOTF_E_TABLE_TAGORDER = (int)(0x80097006 - 0x100000000),
        /// <summary>
        /// A table does not start on a long word boundary.
        /// </summary>
        MSSIPOTF_E_TABLE_Int32UInt16 = (int)(0x80097007 - 0x100000000),
        /// <summary>
        /// First table does not appear after header information.
        /// </summary>
        MSSIPOTF_E_BAD_FIRST_TABLE_PLACEMENT = (int)(0x80097008 - 0x100000000),
        /// <summary>
        /// Two or more tables overlap.
        /// </summary>
        MSSIPOTF_E_TABLES_OVERLAP = (int)(0x80097009 - 0x100000000),
        /// <summary>
        /// Too many pad bytes between tables or pad bytes are not 0.
        /// </summary>
        MSSIPOTF_E_TABLE_PADBYTES = (int)(0x8009700A - 0x100000000),
        /// <summary>
        /// File is too small to contain the last table.
        /// </summary>
        MSSIPOTF_E_FILETOOSMALL = (int)(0x8009700B - 0x100000000),
        /// <summary>
        /// A table checksum is incorrect.
        /// </summary>
        MSSIPOTF_E_TABLE_CHECKSUM = (int)(0x8009700C - 0x100000000),
        /// <summary>
        /// The file checksum is incorrect.
        /// </summary>
        MSSIPOTF_E_FILE_CHECKSUM = (int)(0x8009700D - 0x100000000),
        /// <summary>
        /// The signature does not have the correct attributes for the policy.
        /// </summary>
        MSSIPOTF_E_FAILED_POLICY = (int)(0x80097010 - 0x100000000),
        /// <summary>
        /// The file did not pass the hints check.
        /// </summary>
        MSSIPOTF_E_FAILED_HINTS_CHECK = (int)(0x80097011 - 0x100000000),
        /// <summary>
        /// The file is not an OpenType file.
        /// </summary>
        MSSIPOTF_E_NOT_OPENTYPE = (int)(0x80097012 - 0x100000000),
        /// <summary>
        /// Failed on a file operation (open, map, read, write).
        /// </summary>
        MSSIPOTF_E_FILE = (int)(0x80097013 - 0x100000000),
        /// <summary>
        /// A call to a CryptoAPI function failed.
        /// </summary>
        MSSIPOTF_E_CRYPT = (int)(0x80097014 - 0x100000000),
        /// <summary>
        /// There is a bad version number in the file.
        /// </summary>
        MSSIPOTF_E_BADVERSION = (int)(0x80097015 - 0x100000000),
        /// <summary>
        /// The structure of the DSIG table is incorrect.
        /// </summary>
        MSSIPOTF_E_DSIG_STRUCTURE = (int)(0x80097016 - 0x100000000),
        /// <summary>
        /// A check failed in a partially constant table.
        /// </summary>
        MSSIPOTF_E_PCONST_CHECK = (int)(0x80097017 - 0x100000000),
        /// <summary>
        /// Some kind of structural error.
        /// </summary>
        MSSIPOTF_E_STRUCTURE = (int)(0x80097018 - 0x100000000),
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        NTE_OP_OK = 0,
        /// <summary>
        /// Unknown trust provider.
        /// </summary>
        TRUST_E_PROVIDER_UNKNOWN = (int)(0x800B0001 - 0x100000000),
        /// <summary>
        /// The trust verification action specified is not supported by the specified trust provider.
        /// </summary>
        TRUST_E_ACTION_UNKNOWN = (int)(0x800B0002 - 0x100000000),
        /// <summary>
        /// The form specified for the subject is not one supported or known by the specified trust provider.
        /// </summary>
        TRUST_E_SUBJECT_FORM_UNKNOWN = (int)(0x800B0003 - 0x100000000),
        /// <summary>
        /// The subject is not trusted for the specified action.
        /// </summary>
        TRUST_E_SUBJECT_NOT_TRUSTED = (int)(0x800B0004 - 0x100000000),
        /// <summary>
        /// Error due to problem in ASN.1 encoding process.
        /// </summary>
        DIGSIG_E_ENCODE = (int)(0x800B0005 - 0x100000000),
        /// <summary>
        /// Error due to problem in ASN.1 decoding process.
        /// </summary>
        DIGSIG_E_DECODE = (int)(0x800B0006 - 0x100000000),
        /// <summary>
        /// Reading / writing Extensions where Attributes are appropriate, and visa versa.
        /// </summary>
        DIGSIG_E_EXTENSIBILITY = (int)(0x800B0007 - 0x100000000),
        /// <summary>
        /// Unspecified cryptographic failure.
        /// </summary>
        DIGSIG_E_CRYPTO = (int)(0x800B0008 - 0x100000000),
        /// <summary>
        /// The size of the data could not be determined.
        /// </summary>
        PERSIST_E_SIZEDEFINITE = (int)(0x800B0009 - 0x100000000),
        /// <summary>
        /// The size of the indefinite-sized data could not be determined.
        /// </summary>
        PERSIST_E_SIZEINDEFINITE = (int)(0x800B000A - 0x100000000),
        /// <summary>
        /// This object does not read and write self-sizing data.
        /// </summary>
        PERSIST_E_NOTSELFSIZING = (int)(0x800B000B - 0x100000000),
        /// <summary>
        /// No signature was present in the subject.
        /// </summary>
        TRUST_E_NOSIGNATURE = (int)(0x800B0100 - 0x100000000),
        /// <summary>
        /// A required certificate is not within its validity period when verifying against the current system clock or the timestamp in the signed file.
        /// </summary>
        CERT_E_EXPIRED = (int)(0x800B0101 - 0x100000000),
        /// <summary>
        /// The validity periods of the certification chain do not nest correctly.
        /// </summary>
        CERT_E_VALIDITYPERIODNESTING = (int)(0x800B0102 - 0x100000000),
        /// <summary>
        /// A certificate that can only be used as an end-entity is being used as a CA or visa versa.
        /// </summary>
        CERT_E_ROLE = (int)(0x800B0103 - 0x100000000),
        /// <summary>
        /// A path length constraint in the certification chain has been violated.
        /// </summary>
        CERT_E_PATHLENCONST = (int)(0x800B0104 - 0x100000000),
        /// <summary>
        /// A certificate contains an unknown extension that is marked 'critical'.
        /// </summary>
        CERT_E_CRITICAL = (int)(0x800B0105 - 0x100000000),
        /// <summary>
        /// A certificate being used for a purpose other than the ones specified by its CA.
        /// </summary>
        CERT_E_PURPOSE = (int)(0x800B0106 - 0x100000000),
        /// <summary>
        /// A parent of a given certificate in fact did not issue that child certificate.
        /// </summary>
        CERT_E_ISSUERCHAINING = (int)(0x800B0107 - 0x100000000),
        /// <summary>
        /// A certificate is missing or has an empty value for an important field, such as a subject or issuer name.
        /// </summary>
        CERT_E_MALFORMED = (int)(0x800B0108 - 0x100000000),
        /// <summary>
        /// A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider.
        /// </summary>
        CERT_E_UNTRUSTEDROOT = (int)(0x800B0109 - 0x100000000),
        /// <summary>
        /// An internal certificate chaining error has occurred.
        /// </summary>
        CERT_E_CHAINING = (int)(0x800B010A - 0x100000000),
        /// <summary>
        /// Generic trust failure.
        /// </summary>
        TRUST_E_FAIL = (int)(0x800B010B - 0x100000000),
        /// <summary>
        /// A certificate was explicitly revoked by its issuer.
        /// </summary>
        CERT_E_REVOKED = (int)(0x800B010C - 0x100000000),
        /// <summary>
        /// The certification path terminates with the test root which is not trusted with the current policy settings.
        /// </summary>
        CERT_E_UNTRUSTEDTESTROOT = (int)(0x800B010D - 0x100000000),
        /// <summary>
        /// The revocation process could not continue - the certificate(s) could not be checked.
        /// </summary>
        CERT_E_REVOCATION_FAILURE = (int)(0x800B010E - 0x100000000),
        /// <summary>
        /// The certificate's CN name does not match the passed value.
        /// </summary>
        CERT_E_CN_NO_MATCH = (int)(0x800B010F - 0x100000000),
        /// <summary>
        /// The certificate is not valid for the requested usage.
        /// </summary>
        CERT_E_WRONG_USAGE = (int)(0x800B0110 - 0x100000000),
        /// <summary>
        /// The certificate was explicitly marked as untrusted by the user.
        /// </summary>
        TRUST_E_EXPLICIT_DISTRUST = (int)(0x800B0111 - 0x100000000),
        /// <summary>
        /// A certification chain processed correctly, but one of the CA certificates is not trusted by the policy provider.
        /// </summary>
        CERT_E_UNTRUSTEDCA = (int)(0x800B0112 - 0x100000000),
        /// <summary>
        /// The certificate has invalid policy.
        /// </summary>
        CERT_E_INVALID_POLICY = (int)(0x800B0113 - 0x100000000),
        /// <summary>
        /// The certificate has an invalid name. The name is not included in the permitted list or is explicitly excluded.
        /// </summary>
        CERT_E_INVALID_NAME = (int)(0x800B0114 - 0x100000000),
        /// <summary>
        /// A non-empty line was encountered in the INF before the start of a section.
        /// </summary>
        SPAPI_E_EXPECTED_SECTION_NAME = (int)(0x800F0000 - 0x100000000),
        /// <summary>
        /// A section name marker in the INF is not complete, or does not exist on a line by itself.
        /// </summary>
        SPAPI_E_BAD_SECTION_NAME_LINE = (int)(0x800F0001 - 0x100000000),
        /// <summary>
        /// An INF section was encountered whose name exceeds the maximum section name length.
        /// </summary>
        SPAPI_E_SECTION_NAME_TOO_Int32 = (int)(0x800F0002 - 0x100000000),
        /// <summary>
        /// The syntax of the INF is invalid.
        /// </summary>
        SPAPI_E_GENERAL_SYNTAX = (int)(0x800F0003 - 0x100000000),
        /// <summary>
        /// The style of the INF is different than what was requested.
        /// </summary>
        SPAPI_E_WRONG_INF_STYLE = (int)(0x800F0100 - 0x100000000),
        /// <summary>
        /// The required section was not found in the INF.
        /// </summary>
        SPAPI_E_SECTION_NOT_FOUND = (int)(0x800F0101 - 0x100000000),
        /// <summary>
        /// The required line was not found in the INF.
        /// </summary>
        SPAPI_E_LINE_NOT_FOUND = (int)(0x800F0102 - 0x100000000),
        /// <summary>
        /// The files affected by the installation of this file queue have not been backed up for uninstall.
        /// </summary>
        SPAPI_E_NO_BACKUP = (int)(0x800F0103 - 0x100000000),
        /// <summary>
        /// The INF or the device information set or element does not have an associated install class.
        /// </summary>
        SPAPI_E_NO_ASSOCIATED_CLASS = (int)(0x800F0200 - 0x100000000),
        /// <summary>
        /// The INF or the device information set or element does not match the specified install class.
        /// </summary>
        SPAPI_E_CLASS_MISMATCH = (int)(0x800F0201 - 0x100000000),
        /// <summary>
        /// An existing device was found that is a duplicate of the device being manually installed.
        /// </summary>
        SPAPI_E_DUPLICATE_FOUND = (int)(0x800F0202 - 0x100000000),
        /// <summary>
        /// There is no driver selected for the device information set or element.
        /// </summary>
        SPAPI_E_NO_DRIVER_SELECTED = (int)(0x800F0203 - 0x100000000),
        /// <summary>
        /// The requested device registry key does not exist.
        /// </summary>
        SPAPI_E_KEY_DOES_NOT_EXIST = (int)(0x800F0204 - 0x100000000),
        /// <summary>
        /// The device instance name is invalid.
        /// </summary>
        SPAPI_E_INVALID_DEVINST_NAME = (int)(0x800F0205 - 0x100000000),
        /// <summary>
        /// The install class is not present or is invalid.
        /// </summary>
        SPAPI_E_INVALID_CLASS = (int)(0x800F0206 - 0x100000000),
        /// <summary>
        /// The device instance cannot be created because it already exists.
        /// </summary>
        SPAPI_E_DEVINST_ALREADY_EXISTS = (int)(0x800F0207 - 0x100000000),
        /// <summary>
        /// The operation cannot be performed on a device information element that has not been registered.
        /// </summary>
        SPAPI_E_DEVINFO_NOT_REGISTERED = (int)(0x800F0208 - 0x100000000),
        /// <summary>
        /// The device property code is invalid.
        /// </summary>
        SPAPI_E_INVALID_REG_PROPERTY = (int)(0x800F0209 - 0x100000000),
        /// <summary>
        /// The INF from which a driver list is to be built does not exist.
        /// </summary>
        SPAPI_E_NO_INF = (int)(0x800F020A - 0x100000000),
        /// <summary>
        /// The device instance does not exist in the hardware tree.
        /// </summary>
        SPAPI_E_NO_SUCH_DEVINST = (int)(0x800F020B - 0x100000000),
        /// <summary>
        /// The icon representing this install class cannot be loaded.
        /// </summary>
        SPAPI_E_CANT_LOAD_CLASS_ICON = (int)(0x800F020C - 0x100000000),
        /// <summary>
        /// The class installer registry entry is invalid.
        /// </summary>
        SPAPI_E_INVALID_CLASS_INSTALLER = (int)(0x800F020D - 0x100000000),
        /// <summary>
        /// The class installer has indicated that the default action should be performed for this installation request.
        /// </summary>
        SPAPI_E_DI_DO_DEFAULT = (int)(0x800F020E - 0x100000000),
        /// <summary>
        /// The operation does not require any files to be copied.
        /// </summary>
        SPAPI_E_DI_NOFILECOPY = (int)(0x800F020F - 0x100000000),
        /// <summary>
        /// The specified hardware profile does not exist.
        /// </summary>
        SPAPI_E_INVALID_HWPROFILE = (int)(0x800F0210 - 0x100000000),
        /// <summary>
        /// There is no device information element currently selected for this device information set.
        /// </summary>
        SPAPI_E_NO_DEVICE_SELECTED = (int)(0x800F0211 - 0x100000000),
        /// <summary>
        /// The operation cannot be performed because the device information set is locked.
        /// </summary>
        SPAPI_E_DEVINFO_LIST_LOCKED = (int)(0x800F0212 - 0x100000000),
        /// <summary>
        /// The operation cannot be performed because the device information element is locked.
        /// </summary>
        SPAPI_E_DEVINFO_DATA_LOCKED = (int)(0x800F0213 - 0x100000000),
        /// <summary>
        /// The specified path does not contain any applicable device INFs.
        /// </summary>
        SPAPI_E_DI_BAD_PATH = (int)(0x800F0214 - 0x100000000),
        /// <summary>
        /// No class installer parameters have been set for the device information set or element.
        /// </summary>
        SPAPI_E_NO_CLASSINSTALL_PARAMS = (int)(0x800F0215 - 0x100000000),
        /// <summary>
        /// The operation cannot be performed because the file queue is locked.
        /// </summary>
        SPAPI_E_FILEQUEUE_LOCKED = (int)(0x800F0216 - 0x100000000),
        /// <summary>
        /// A service installation section in this INF is invalid.
        /// </summary>
        SPAPI_E_BAD_SERVICE_INSTALLSECT = (int)(0x800F0217 - 0x100000000),
        /// <summary>
        /// There is no class driver list for the device information element.
        /// </summary>
        SPAPI_E_NO_CLASS_DRIVER_LIST = (int)(0x800F0218 - 0x100000000),
        /// <summary>
        /// The installation failed because a function driver was not specified for this device instance.
        /// </summary>
        SPAPI_E_NO_ASSOCIATED_SERVICE = (int)(0x800F0219 - 0x100000000),
        /// <summary>
        /// There is presently no default device interface designated for this interface class.
        /// </summary>
        SPAPI_E_NO_DEFAULT_DEVICE_INTERFACE = (int)(0x800F021A - 0x100000000),
        /// <summary>
        /// The operation cannot be performed because the device interface is currently active.
        /// </summary>
        SPAPI_E_DEVICE_INTERFACE_ACTIVE = (int)(0x800F021B - 0x100000000),
        /// <summary>
        /// The operation cannot be performed because the device interface has been removed from the system.
        /// </summary>
        SPAPI_E_DEVICE_INTERFACE_REMOVED = (int)(0x800F021C - 0x100000000),
        /// <summary>
        /// An interface installation section in this INF is invalid.
        /// </summary>
        SPAPI_E_BAD_INTERFACE_INSTALLSECT = (int)(0x800F021D - 0x100000000),
        /// <summary>
        /// This interface class does not exist in the system.
        /// </summary>
        SPAPI_E_NO_SUCH_INTERFACE_CLASS = (int)(0x800F021E - 0x100000000),
        /// <summary>
        /// The reference string supplied for this interface device is invalid.
        /// </summary>
        SPAPI_E_INVALID_REFERENCE_STRING = (int)(0x800F021F - 0x100000000),
        /// <summary>
        /// The specified machine name does not conform to UNC naming conventions.
        /// </summary>
        SPAPI_E_INVALID_MACHINENAME = (int)(0x800F0220 - 0x100000000),
        /// <summary>
        /// A general remote communication error occurred.
        /// </summary>
        SPAPI_E_REMOTE_COMM_FAILURE = (int)(0x800F0221 - 0x100000000),
        /// <summary>
        /// The machine selected for remote communication is not available at this time.
        /// </summary>
        SPAPI_E_MACHINE_UNAVAILABLE = (int)(0x800F0222 - 0x100000000),
        /// <summary>
        /// The Plug and Play service is not available on the remote machine.
        /// </summary>
        SPAPI_E_NO_CONFIGMGR_SERVICES = (int)(0x800F0223 - 0x100000000),
        /// <summary>
        /// The property page provider registry entry is invalid.
        /// </summary>
        SPAPI_E_INVALID_PROPPAGE_PROVIDER = (int)(0x800F0224 - 0x100000000),
        /// <summary>
        /// The requested device interface is not present in the system.
        /// </summary>
        SPAPI_E_NO_SUCH_DEVICE_INTERFACE = (int)(0x800F0225 - 0x100000000),
        /// <summary>
        /// The device's co-installer has additional work to perform after installation is complete.
        /// </summary>
        SPAPI_E_DI_POSTPROCESSING_REQUIRED = (int)(0x800F0226 - 0x100000000),
        /// <summary>
        /// The device's co-installer is invalid.
        /// </summary>
        SPAPI_E_INVALID_COINSTALLER = (int)(0x800F0227 - 0x100000000),
        /// <summary>
        /// There are no compatible drivers for this device.
        /// </summary>
        SPAPI_E_NO_COMPAT_DRIVERS = (int)(0x800F0228 - 0x100000000),
        /// <summary>
        /// There is no icon that represents this device or device type.
        /// </summary>
        SPAPI_E_NO_DEVICE_ICON = (int)(0x800F0229 - 0x100000000),
        /// <summary>
        /// A logical configuration specified in this INF is invalid.
        /// </summary>
        SPAPI_E_INVALID_INF_LOGCONFIG = (int)(0x800F022A - 0x100000000),
        /// <summary>
        /// The class installer has denied the request to install or upgrade this device.
        /// </summary>
        SPAPI_E_DI_DONT_INSTALL = (int)(0x800F022B - 0x100000000),
        /// <summary>
        /// One of the filter drivers installed for this device is invalid.
        /// </summary>
        SPAPI_E_INVALID_FILTER_DRIVER = (int)(0x800F022C - 0x100000000),
        /// <summary>
        /// The driver selected for this device does not support Windows XP.
        /// </summary>
        SPAPI_E_NON_WINDOWS_NT_DRIVER = (int)(0x800F022D - 0x100000000),
        /// <summary>
        /// The driver selected for this device does not support Windows.
        /// </summary>
        SPAPI_E_NON_WINDOWS_DRIVER = (int)(0x800F022E - 0x100000000),
        /// <summary>
        /// The third-party INF does not contain digital signature information.
        /// </summary>
        SPAPI_E_NO_CATALOG_FOR_OEM_INF = (int)(0x800F022F - 0x100000000),
        /// <summary>
        /// An invalid attempt was made to use a device installation file queue for verification of digital signatures relative to other platforms.
        /// </summary>
        SPAPI_E_DEVINSTALL_QUEUE_NONNATIVE = (int)(0x800F0230 - 0x100000000),
        /// <summary>
        /// The device cannot be disabled.
        /// </summary>
        SPAPI_E_NOT_DISABLEABLE = (int)(0x800F0231 - 0x100000000),
        /// <summary>
        /// The device could not be dynamically removed.
        /// </summary>
        SPAPI_E_CANT_REMOVE_DEVINST = (int)(0x800F0232 - 0x100000000),
        /// <summary>
        /// Cannot copy to specified target.
        /// </summary>
        SPAPI_E_INVALID_TARGET = (int)(0x800F0233 - 0x100000000),
        /// <summary>
        /// Driver is not intended for this platform.
        /// </summary>
        SPAPI_E_DRIVER_NONNATIVE = (int)(0x800F0234 - 0x100000000),
        /// <summary>
        /// Operation not allowed in WOW64.
        /// </summary>
        SPAPI_E_IN_WOW64 = (int)(0x800F0235 - 0x100000000),
        /// <summary>
        /// The operation involving unsigned file copying was rolled back, so that a system restore point could be set.
        /// </summary>
        SPAPI_E_SET_SYSTEM_RESTORE_POINT = (int)(0x800F0236 - 0x100000000),
        /// <summary>
        /// An INF was copied into the Windows INF directory in an improper manner.
        /// </summary>
        SPAPI_E_INCORRECTLY_COPIED_INF = (int)(0x800F0237 - 0x100000000),
        /// <summary>
        /// The Security Configuration Editor (SCE) APIs have been disabled on this Embedded product.
        /// </summary>
        SPAPI_E_SCE_DISABLED = (int)(0x800F0238 - 0x100000000),
        /// <summary>
        /// No installed components were detected.
        /// </summary>
        SPAPI_E_ERROR_NOT_INSTALLED = (int)(0x800F1000 - 0x100000000),
        /// <summary>
        /// An internal consistency check failed.
        /// </summary>
        SCARD_F_INTERNAL_ERROR = (int)(0x80100001 - 0x100000000),
        /// <summary>
        /// The action was cancelled by an SCardCancel request.
        /// </summary>
        SCARD_E_CANCELLED = (int)(0x80100002 - 0x100000000),
        /// <summary>
        /// The supplied handle was invalid.
        /// </summary>
        SCARD_E_INVALID_HANDLE = (int)(0x80100003 - 0x100000000),
        /// <summary>
        /// One or more of the supplied parameters could not be properly interpreted.
        /// </summary>
        SCARD_E_INVALID_PARAMETER = (int)(0x80100004 - 0x100000000),
        /// <summary>
        /// Registry startup information is missing or invalid.
        /// </summary>
        SCARD_E_INVALID_TARGET = (int)(0x80100005 - 0x100000000),
        /// <summary>
        /// Not enough memory available to complete this command.
        /// </summary>
        SCARD_E_NO_MEMORY = (int)(0x80100006 - 0x100000000),
        /// <summary>
        /// An internal consistency timer has expired.
        /// </summary>
        SCARD_F_WAITED_TOO_Int32 = (int)(0x80100007 - 0x100000000),
        /// <summary>
        /// The data buffer to receive returned data is too small for the returned data.
        /// </summary>
        SCARD_E_INSUFFICIENT_BUFFER = (int)(0x80100008 - 0x100000000),
        /// <summary>
        /// The specified reader name is not recognized.
        /// </summary>
        SCARD_E_UNKNOWN_READER = (int)(0x80100009 - 0x100000000),
        /// <summary>
        /// The user-specified timeout value has expired.
        /// </summary>
        SCARD_E_TIMEOUT = (int)(0x8010000A - 0x100000000),
        /// <summary>
        /// The smart card cannot be accessed because of other connections outstanding.
        /// </summary>
        SCARD_E_SHARING_VIOLATION = (int)(0x8010000B - 0x100000000),
        /// <summary>
        /// The operation requires a Smart Card, but no Smart Card is currently in the device.
        /// </summary>
        SCARD_E_NO_SMARTCARD = (int)(0x8010000C - 0x100000000),
        /// <summary>
        /// The specified smart card name is not recognized.
        /// </summary>
        SCARD_E_UNKNOWN_CARD = (int)(0x8010000D - 0x100000000),
        /// <summary>
        /// The system could not dispose of the media in the requested manner.
        /// </summary>
        SCARD_E_CANT_DISPOSE = (int)(0x8010000E - 0x100000000),
        /// <summary>
        /// The requested protocols are incompatible with the protocol currently in use with the smart card.
        /// </summary>
        SCARD_E_PROTO_MISMATCH = (int)(0x8010000F - 0x100000000),
        /// <summary>
        /// The reader or smart card is not ready to accept commands.
        /// </summary>
        SCARD_E_NOT_READY = (int)(0x80100010 - 0x100000000),
        /// <summary>
        /// One or more of the supplied parameters values could not be properly interpreted.
        /// </summary>
        SCARD_E_INVALID_VALUE = (int)(0x80100011 - 0x100000000),
        /// <summary>
        /// The action was cancelled by the system, presumably to log off or shut down.
        /// </summary>
        SCARD_E_SYSTEM_CANCELLED = (int)(0x80100012 - 0x100000000),
        /// <summary>
        /// An internal communications error has been detected.
        /// </summary>
        SCARD_F_COMM_ERROR = (int)(0x80100013 - 0x100000000),
        /// <summary>
        /// An internal error has been detected, but the source is unknown.
        /// </summary>
        SCARD_F_UNKNOWN_ERROR = (int)(0x80100014 - 0x100000000),
        /// <summary>
        /// An ATR obtained from the registry is not a valid ATR string.
        /// </summary>
        SCARD_E_INVALID_ATR = (int)(0x80100015 - 0x100000000),
        /// <summary>
        /// An attempt was made to end a non-existent transaction.
        /// </summary>
        SCARD_E_NOT_TRANSACTED = (int)(0x80100016 - 0x100000000),
        /// <summary>
        /// The specified reader is not currently available for use.
        /// </summary>
        SCARD_E_READER_UNAVAILABLE = (int)(0x80100017 - 0x100000000),
        /// <summary>
        /// The operation has been aborted to allow the server application to exit.
        /// </summary>
        SCARD_P_SHUTDOWN = (int)(0x80100018 - 0x100000000),
        /// <summary>
        /// The PCI Receive buffer was too small.
        /// </summary>
        SCARD_E_PCI_TOO_SMALL = (int)(0x80100019 - 0x100000000),
        /// <summary>
        /// The reader driver does not meet minimal requirements for support.
        /// </summary>
        SCARD_E_READER_UNSUPPORTED = (int)(0x8010001A - 0x100000000),
        /// <summary>
        /// The reader driver did not produce a unique reader name.
        /// </summary>
        SCARD_E_DUPLICATE_READER = (int)(0x8010001B - 0x100000000),
        /// <summary>
        /// The smart card does not meet minimal requirements for support.
        /// </summary>
        SCARD_E_CARD_UNSUPPORTED = (int)(0x8010001C - 0x100000000),
        /// <summary>
        /// The Smart card resource manager is not running.
        /// </summary>
        SCARD_E_NO_SERVICE = (int)(0x8010001D - 0x100000000),
        /// <summary>
        /// The Smart card resource manager has shut down.
        /// </summary>
        SCARD_E_SERVICE_STOPPED = (int)(0x8010001E - 0x100000000),
        /// <summary>
        /// An unexpected card error has occurred.
        /// </summary>
        SCARD_E_UNEXPECTED = (int)(0x8010001F - 0x100000000),
        /// <summary>
        /// No Primary Provider can be found for the smart card.
        /// </summary>
        SCARD_E_ICC_INSTALLATION = (int)(0x80100020 - 0x100000000),
        /// <summary>
        /// The requested order of object creation is not supported.
        /// </summary>
        SCARD_E_ICC_CREATEORDER = (int)(0x80100021 - 0x100000000),
        /// <summary>
        /// This smart card does not support the requested feature.
        /// </summary>
        SCARD_E_UNSUPPORTED_FEATURE = (int)(0x80100022 - 0x100000000),
        /// <summary>
        /// The identified directory does not exist in the smart card.
        /// </summary>
        SCARD_E_DIR_NOT_FOUND = (int)(0x80100023 - 0x100000000),
        /// <summary>
        /// The identified file does not exist in the smart card.
        /// </summary>
        SCARD_E_FILE_NOT_FOUND = (int)(0x80100024 - 0x100000000),
        /// <summary>
        /// The supplied path does not represent a smart card directory.
        /// </summary>
        SCARD_E_NO_DIR = (int)(0x80100025 - 0x100000000),
        /// <summary>
        /// The supplied path does not represent a smart card file.
        /// </summary>
        SCARD_E_NO_FILE = (int)(0x80100026 - 0x100000000),
        /// <summary>
        /// Access is denied to this file.
        /// </summary>
        SCARD_E_NO_ACCESS = (int)(0x80100027 - 0x100000000),
        /// <summary>
        /// The smartcard does not have enough memory to store the information.
        /// </summary>
        SCARD_E_WRITE_TOO_MANY = (int)(0x80100028 - 0x100000000),
        /// <summary>
        /// There was an error trying to set the smart card file object pointer.
        /// </summary>
        SCARD_E_BAD_SEEK = (int)(0x80100029 - 0x100000000),
        /// <summary>
        /// The supplied PIN is incorrect.
        /// </summary>
        SCARD_E_INVALID_CHV = (int)(0x8010002A - 0x100000000),
        /// <summary>
        /// An unrecognized error code was returned from a layered component.
        /// </summary>
        SCARD_E_UNKNOWN_RES_MNG = (int)(0x8010002B - 0x100000000),
        /// <summary>
        /// The requested certificate does not exist.
        /// </summary>
        SCARD_E_NO_SUCH_CERTIFICATE = (int)(0x8010002C - 0x100000000),
        /// <summary>
        /// The requested certificate could not be obtained.
        /// </summary>
        SCARD_E_CERTIFICATE_UNAVAILABLE = (int)(0x8010002D - 0x100000000),
        /// <summary>
        /// Cannot find a smart card reader.
        /// </summary>
        SCARD_E_NO_READERS_AVAILABLE = (int)(0x8010002E - 0x100000000),
        /// <summary>
        /// A communications error with the smart card has been detected.  Retry the operation.
        /// </summary>
        SCARD_E_COMM_DATA_LOST = (int)(0x8010002F - 0x100000000),
        /// <summary>
        /// The requested key container does not exist on the smart card.
        /// </summary>
        SCARD_E_NO_KEY_CONTAINER = (int)(0x80100030 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        SCARD_E_SERVER_TOO_BUSY = (int)(0x80100031 - 0x100000000),
        /// <summary>
        /// The reader cannot communicate with the smart card, due to ATR configuration conflicts.
        /// </summary>
        SCARD_W_UNSUPPORTED_CARD = (int)(0x80100065 - 0x100000000),
        /// <summary>
        /// The smart card is not responding to a reset.
        /// </summary>
        SCARD_W_UNRESPONSIVE_CARD = (int)(0x80100066 - 0x100000000),
        /// <summary>
        /// Power has been removed from the smart card, so that further communication is not possible.
        /// </summary>
        SCARD_W_UNPOWERED_CARD = (int)(0x80100067 - 0x100000000),
        /// <summary>
        /// The smart card has been reset, so any shared state information is invalid.
        /// </summary>
        SCARD_W_RESET_CARD = (int)(0x80100068 - 0x100000000),
        /// <summary>
        /// The smart card has been removed, so that further communication is not possible.
        /// </summary>
        SCARD_W_REMOVED_CARD = (int)(0x80100069 - 0x100000000),
        /// <summary>
        /// Access was denied because of a security violation.
        /// </summary>
        SCARD_W_SECURITY_VIOLATION = (int)(0x8010006A - 0x100000000),
        /// <summary>
        /// The card cannot be accessed because the wrong PIN was presented.
        /// </summary>
        SCARD_W_WRONG_CHV = (int)(0x8010006B - 0x100000000),
        /// <summary>
        /// The card cannot be accessed because the maximum number of PIN entry attempts has been reached.
        /// </summary>
        SCARD_W_CHV_BLOCKED = (int)(0x8010006C - 0x100000000),
        /// <summary>
        /// The end of the smart card file has been reached.
        /// </summary>
        SCARD_W_EOF = (int)(0x8010006D - 0x100000000),
        /// <summary>
        /// The action was cancelled by the user.
        /// </summary>
        SCARD_W_CANCELLED_BY_USER = (int)(0x8010006E - 0x100000000),
        /// <summary>
        /// No PIN was presented to the smart card.
        /// </summary>
        SCARD_W_CARD_NOT_AUTHENTICATED = (int)(0x8010006F - 0x100000000),
        /// <summary>
        /// Errors occurred accessing one or more objects - the ErrorInfo collection may have more detail
        /// </summary>
        COMADMIN_E_OBJECTERRORS = (int)(0x80110401 - 0x100000000),
        /// <summary>
        /// One or more of the object's properties are missing or invalid
        /// </summary>
        COMADMIN_E_OBJECTINVALID = (int)(0x80110402 - 0x100000000),
        /// <summary>
        /// The object was not found in the catalog
        /// </summary>
        COMADMIN_E_KEYMISSING = (int)(0x80110403 - 0x100000000),
        /// <summary>
        /// The object is already registered
        /// </summary>
        COMADMIN_E_ALREADYINSTALLED = (int)(0x80110404 - 0x100000000),
        /// <summary>
        /// Error occurred writing to the application file
        /// </summary>
        COMADMIN_E_APP_FILE_WRITEFAIL = (int)(0x80110407 - 0x100000000),
        /// <summary>
        /// Error occurred reading the application file
        /// </summary>
        COMADMIN_E_APP_FILE_READFAIL = (int)(0x80110408 - 0x100000000),
        /// <summary>
        /// Invalid version number in application file
        /// </summary>
        COMADMIN_E_APP_FILE_VERSION = (int)(0x80110409 - 0x100000000),
        /// <summary>
        /// The file path is invalid
        /// </summary>
        COMADMIN_E_BADPATH = (int)(0x8011040A - 0x100000000),
        /// <summary>
        /// The application is already installed
        /// </summary>
        COMADMIN_E_APPLICATIONEXISTS = (int)(0x8011040B - 0x100000000),
        /// <summary>
        /// The role already exists
        /// </summary>
        COMADMIN_E_ROLEEXISTS = (int)(0x8011040C - 0x100000000),
        /// <summary>
        /// An error occurred copying the file
        /// </summary>
        COMADMIN_E_CANTCOPYFILE = (int)(0x8011040D - 0x100000000),
        /// <summary>
        /// One or more users are not valid
        /// </summary>
        COMADMIN_E_NOUSER = (int)(0x8011040F - 0x100000000),
        /// <summary>
        /// One or more users in the application file are not valid
        /// </summary>
        COMADMIN_E_INVALIDUSERIDS = (int)(0x80110410 - 0x100000000),
        /// <summary>
        /// The component's CLSID is missing or corrupt
        /// </summary>
        COMADMIN_E_NOREGISTRYCLSID = (int)(0x80110411 - 0x100000000),
        /// <summary>
        /// The component's progID is missing or corrupt
        /// </summary>
        COMADMIN_E_BADREGISTRYPROGID = (int)(0x80110412 - 0x100000000),
        /// <summary>
        /// Unable to set required authentication level for update request
        /// </summary>
        COMADMIN_E_AUTHENTICATIONLEVEL = (int)(0x80110413 - 0x100000000),
        /// <summary>
        /// The identity or password set on the application is not valid
        /// </summary>
        COMADMIN_E_USERPASSWDNOTVALID = (int)(0x80110414 - 0x100000000),
        /// <summary>
        /// Application file CLSIDs or IIDs do not match corresponding DLLs
        /// </summary>
        COMADMIN_E_CLSIDORIIDMISMATCH = (int)(0x80110418 - 0x100000000),
        /// <summary>
        /// Interface information is either missing or changed
        /// </summary>
        COMADMIN_E_REMOTEINTERFACE = (int)(0x80110419 - 0x100000000),
        /// <summary>
        /// DllRegisterServer failed on component install
        /// </summary>
        COMADMIN_E_DLLREGISTERSERVER = (int)(0x8011041A - 0x100000000),
        /// <summary>
        /// No server file share available
        /// </summary>
        COMADMIN_E_NOSERVERSHARE = (int)(0x8011041B - 0x100000000),
        /// <summary>
        /// DLL could not be loaded
        /// </summary>
        COMADMIN_E_DLLLOADFAILED = (int)(0x8011041D - 0x100000000),
        /// <summary>
        /// The registered TypeLib ID is not valid
        /// </summary>
        COMADMIN_E_BADREGISTRYLIBID = (int)(0x8011041E - 0x100000000),
        /// <summary>
        /// Application install directory not found
        /// </summary>
        COMADMIN_E_APPDIRNOTFOUND = (int)(0x8011041F - 0x100000000),
        /// <summary>
        /// Errors occurred while in the component registrar
        /// </summary>
        COMADMIN_E_REGISTRARFAILED = (int)(0x80110423 - 0x100000000),
        /// <summary>
        /// The file does not exist
        /// </summary>
        COMADMIN_E_COMPFILE_DOESNOTEXIST = (int)(0x80110424 - 0x100000000),
        /// <summary>
        /// The DLL could not be loaded
        /// </summary>
        COMADMIN_E_COMPFILE_LOADDLLFAIL = (int)(0x80110425 - 0x100000000),
        /// <summary>
        /// GetClassObject failed in the DLL
        /// </summary>
        COMADMIN_E_COMPFILE_GETCLASSOBJ = (int)(0x80110426 - 0x100000000),
        /// <summary>
        /// The DLL does not support the components listed in the TypeLib
        /// </summary>
        COMADMIN_E_COMPFILE_CLASSNOTAVAIL = (int)(0x80110427 - 0x100000000),
        /// <summary>
        /// The TypeLib could not be loaded
        /// </summary>
        COMADMIN_E_COMPFILE_BADTLB = (int)(0x80110428 - 0x100000000),
        /// <summary>
        /// The file does not contain components or component information
        /// </summary>
        COMADMIN_E_COMPFILE_NOTINSTALLABLE = (int)(0x80110429 - 0x100000000),
        /// <summary>
        /// Changes to this object and its sub-objects have been disabled
        /// </summary>
        COMADMIN_E_NOTCHANGEABLE = (int)(0x8011042A - 0x100000000),
        /// <summary>
        /// The delete function has been disabled for this object
        /// </summary>
        COMADMIN_E_NOTDELETEABLE = (int)(0x8011042B - 0x100000000),
        /// <summary>
        /// The server catalog version is not supported
        /// </summary>
        COMADMIN_E_SESSION = (int)(0x8011042C - 0x100000000),
        /// <summary>
        /// The component move was disallowed, because the source or destination application is either a system application or currently locked against changes
        /// </summary>
        COMADMIN_E_COMP_MOVE_LOCKED = (int)(0x8011042D - 0x100000000),
        /// <summary>
        /// The component move failed because the destination application no longer exists
        /// </summary>
        COMADMIN_E_COMP_MOVE_BAD_DEST = (int)(0x8011042E - 0x100000000),
        /// <summary>
        /// The system was unable to register the TypeLib
        /// </summary>
        COMADMIN_E_REGISTERTLB = (int)(0x80110430 - 0x100000000),
        /// <summary>
        /// This operation can not be performed on the system application
        /// </summary>
        COMADMIN_E_SYSTEMAPP = (int)(0x80110433 - 0x100000000),
        /// <summary>
        /// The component registrar referenced in this file is not available
        /// </summary>
        COMADMIN_E_COMPFILE_NOREGISTRAR = (int)(0x80110434 - 0x100000000),
        /// <summary>
        /// A component in the same DLL is already installed
        /// </summary>
        COMADMIN_E_COREQCOMPINSTALLED = (int)(0x80110435 - 0x100000000),
        /// <summary>
        /// The service is not installed
        /// </summary>
        COMADMIN_E_SERVICENOTINSTALLED = (int)(0x80110436 - 0x100000000),
        /// <summary>
        /// One or more property settings are either invalid or in conflict with each other
        /// </summary>
        COMADMIN_E_PROPERTYSAVEFAILED = (int)(0x80110437 - 0x100000000),
        /// <summary>
        /// The object you are attempting to add or rename already exists
        /// </summary>
        COMADMIN_E_OBJECTEXISTS = (int)(0x80110438 - 0x100000000),
        /// <summary>
        /// The component already exists
        /// </summary>
        COMADMIN_E_COMPONENTEXISTS = (int)(0x80110439 - 0x100000000),
        /// <summary>
        /// The registration file is corrupt
        /// </summary>
        COMADMIN_E_REGFILE_CORRUPT = (int)(0x8011043B - 0x100000000),
        /// <summary>
        /// The property value is too large
        /// </summary>
        COMADMIN_E_PROPERTY_OVERFLOW = (int)(0x8011043C - 0x100000000),
        /// <summary>
        /// Object was not found in registry
        /// </summary>
        COMADMIN_E_NOTINREGISTRY = (int)(0x8011043E - 0x100000000),
        /// <summary>
        /// This object is not poolable
        /// </summary>
        COMADMIN_E_OBJECTNOTPOOLABLE = (int)(0x8011043F - 0x100000000),
        /// <summary>
        /// A CLSID with the same GUID as the new application ID is already installed on this machine
        /// </summary>
        COMADMIN_E_APPLID_MATCHES_CLSID = (int)(0x80110446 - 0x100000000),
        /// <summary>
        /// A role assigned to a component, interface, or method did not exist in the application
        /// </summary>
        COMADMIN_E_ROLE_DOES_NOT_EXIST = (int)(0x80110447 - 0x100000000),
        /// <summary>
        /// You must have components in an application in order to start the application
        /// </summary>
        COMADMIN_E_START_APP_NEEDS_COMPONENTS = (int)(0x80110448 - 0x100000000),
        /// <summary>
        /// This operation is not enabled on this platform
        /// </summary>
        COMADMIN_E_REQUIRES_DIFFERENT_PLATFORM = (int)(0x80110449 - 0x100000000),
        /// <summary>
        /// Application Proxy is not exportable
        /// </summary>
        COMADMIN_E_CAN_NOT_EXPORT_APP_PROXY = (int)(0x8011044A - 0x100000000),
        /// <summary>
        /// Failed to start application because it is either a library application or an application proxy
        /// </summary>
        COMADMIN_E_CAN_NOT_START_APP = (int)(0x8011044B - 0x100000000),
        /// <summary>
        /// System application is not exportable
        /// </summary>
        COMADMIN_E_CAN_NOT_EXPORT_SYS_APP = (int)(0x8011044C - 0x100000000),
        /// <summary>
        /// Can not subscribe to this component (the component may have been imported)
        /// </summary>
        COMADMIN_E_CANT_SUBSCRIBE_TO_COMPONENT = (int)(0x8011044D - 0x100000000),
        /// <summary>
        /// An event class cannot also be a subscriber component
        /// </summary>
        COMADMIN_E_EVENTCLASS_CANT_BE_SUBSCRIBER = (int)(0x8011044E - 0x100000000),
        /// <summary>
        /// Library applications and application proxies are incompatible
        /// </summary>
        COMADMIN_E_LIB_APP_PROXY_INCOMPATIBLE = (int)(0x8011044F - 0x100000000),
        /// <summary>
        /// This function is valid for the base partition only
        /// </summary>
        COMADMIN_E_BASE_PARTITION_ONLY = (int)(0x80110450 - 0x100000000),
        /// <summary>
        /// You cannot start an application that has been disabled
        /// </summary>
        COMADMIN_E_START_APP_DISABLED = (int)(0x80110451 - 0x100000000),
        /// <summary>
        /// The specified partition name is already in use on this computer
        /// </summary>
        COMADMIN_E_CAT_DUPLICATE_PARTITION_NAME = (int)(0x80110457 - 0x100000000),
        /// <summary>
        /// The specified partition name is invalid. Check that the name contains at least one visible character
        /// </summary>
        COMADMIN_E_CAT_INVALID_PARTITION_NAME = (int)(0x80110458 - 0x100000000),
        /// <summary>
        /// The partition cannot be deleted because it is the default partition for one or more users
        /// </summary>
        COMADMIN_E_CAT_PARTITION_IN_USE = (int)(0x80110459 - 0x100000000),
        /// <summary>
        /// The partition cannot be exported, because one or more components in the partition have the same file name
        /// </summary>
        COMADMIN_E_FILE_PARTITION_DUPLICATE_FILES = (int)(0x8011045A - 0x100000000),
        /// <summary>
        /// Applications that contain one or more imported components cannot be installed into a non-base partition
        /// </summary>
        COMADMIN_E_CAT_IMPORTED_COMPONENTS_NOT_ALLOWED = (int)(0x8011045B - 0x100000000),
        /// <summary>
        /// The application name is not unique and cannot be resolved to an application id
        /// </summary>
        COMADMIN_E_AMBIGUOUS_APPLICATION_NAME = (int)(0x8011045C - 0x100000000),
        /// <summary>
        /// The partition name is not unique and cannot be resolved to a partition id
        /// </summary>
        COMADMIN_E_AMBIGUOUS_PARTITION_NAME = (int)(0x8011045D - 0x100000000),
        /// <summary>
        /// The COM+ registry database has not been initialized
        /// </summary>
        COMADMIN_E_REGDB_NOTINITIALIZED = (int)(0x80110472 - 0x100000000),
        /// <summary>
        /// The COM+ registry database is not open
        /// </summary>
        COMADMIN_E_REGDB_NOTOPEN = (int)(0x80110473 - 0x100000000),
        /// <summary>
        /// The COM+ registry database detected a system error
        /// </summary>
        COMADMIN_E_REGDB_SYSTEMERR = (int)(0x80110474 - 0x100000000),
        /// <summary>
        /// The COM+ registry database is already running
        /// </summary>
        COMADMIN_E_REGDB_ALREADYRUNNING = (int)(0x80110475 - 0x100000000),
        /// <summary>
        /// This version of the COM+ registry database cannot be migrated
        /// </summary>
        COMADMIN_E_MIG_VERSIONNOTSUPPORTED = (int)(0x80110480 - 0x100000000),
        /// <summary>
        /// The schema version to be migrated could not be found in the COM+ registry database
        /// </summary>
        COMADMIN_E_MIG_SCHEMANOTFOUND = (int)(0x80110481 - 0x100000000),
        /// <summary>
        /// There was a type mismatch between binaries
        /// </summary>
        COMADMIN_E_CAT_BITNESSMISMATCH = (int)(0x80110482 - 0x100000000),
        /// <summary>
        /// A binary of unknown or invalid type was provided
        /// </summary>
        COMADMIN_E_CAT_UNACCEPTABLEBITNESS = (int)(0x80110483 - 0x100000000),
        /// <summary>
        /// There was a type mismatch between a binary and an application
        /// </summary>
        COMADMIN_E_CAT_WRONGAPPBITNESS = (int)(0x80110484 - 0x100000000),
        /// <summary>
        /// The application cannot be paused or resumed
        /// </summary>
        COMADMIN_E_CAT_PAUSE_RESUME_NOT_SUPPORTED = (int)(0x80110485 - 0x100000000),
        /// <summary>
        /// The COM+ Catalog Server threw an exception during execution
        /// </summary>
        COMADMIN_E_CAT_SERVERFAULT = (int)(0x80110486 - 0x100000000),
        /// <summary>
        /// Only COM+ Applications marked "queued" can be invoked using the "queue" moniker
        /// </summary>
        COMQC_E_APPLICATION_NOT_QUEUED = (int)(0x80110600 - 0x100000000),
        /// <summary>
        /// At least one interface must be marked "queued" in order to create a queued component instance with the "queue" moniker
        /// </summary>
        COMQC_E_NO_QUEUEABLE_INTERFACES = (int)(0x80110601 - 0x100000000),
        /// <summary>
        /// MSMQ is required for the requested operation and is not installed
        /// </summary>
        COMQC_E_QUEUING_SERVICE_NOT_AVAILABLE = (int)(0x80110602 - 0x100000000),
        /// <summary>
        /// Unable to marshal an interface that does not support IPersistStream
        /// </summary>
        COMQC_E_NO_IPERSISTSTREAM = (int)(0x80110603 - 0x100000000),
        /// <summary>
        /// The message is improperly formatted or was damaged in transit
        /// </summary>
        COMQC_E_BAD_MESSAGE = (int)(0x80110604 - 0x100000000),
        /// <summary>
        /// An unauthenticated message was received by an application that accepts only authenticated messages
        /// </summary>
        COMQC_E_UNAUTHENTICATED = (int)(0x80110605 - 0x100000000),
        /// <summary>
        /// The message was requeued or moved by a user not in the "QC Trusted User" role
        /// </summary>
        COMQC_E_UNTRUSTED_ENQUEUER = (int)(0x80110606 - 0x100000000),
        /// <summary>
        /// Cannot create a duplicate resource of type Distributed Transaction Coordinator
        /// </summary>
        MSDTC_E_DUPLICATE_RESOURCE = (int)(0x80110701 - 0x100000000),
        /// <summary>
        /// One of the objects being inserted or updated does not belong to a valid parent collection
        /// </summary>
        COMADMIN_E_OBJECT_PARENT_MISSING = (int)(0x80110808 - 0x100000000),
        /// <summary>
        /// One of the specified objects cannot be found
        /// </summary>
        COMADMIN_E_OBJECT_DOES_NOT_EXIST = (int)(0x80110809 - 0x100000000),
        /// <summary>
        /// The specified application is not currently running
        /// </summary>
        COMADMIN_E_APP_NOT_RUNNING = (int)(0x8011080A - 0x100000000),
        /// <summary>
        /// The partition(s) specified are not valid.
        /// </summary>
        COMADMIN_E_INVALID_PARTITION = (int)(0x8011080B - 0x100000000),
        /// <summary>
        /// COM+ applications that run as NT service may not be pooled or recycled
        /// </summary>
        COMADMIN_E_SVCAPP_NOT_POOLABLE_OR_RECYCLABLE = (int)(0x8011080D - 0x100000000),
        /// <summary>
        /// One or more users are already assigned to a local partition set.
        /// </summary>
        COMADMIN_E_USER_IN_SET = (int)(0x8011080E - 0x100000000),
        /// <summary>
        /// Library applications may not be recycled.
        /// </summary>
        COMADMIN_E_CANTRECYCLELIBRARYAPPS = (int)(0x8011080F - 0x100000000),
        /// <summary>
        /// Applications running as NT services may not be recycled.
        /// </summary>
        COMADMIN_E_CANTRECYCLESERVICEAPPS = (int)(0x80110811 - 0x100000000),
        /// <summary>
        /// The process has already been recycled.
        /// </summary>
        COMADMIN_E_PROCESSALREADYRECYCLED = (int)(0x80110812 - 0x100000000),
        /// <summary>
        /// A paused process may not be recycled.
        /// </summary>
        COMADMIN_E_PAUSEDPROCESSMAYNOTBERECYCLED = (int)(0x80110813 - 0x100000000),
        /// <summary>
        /// Library applications may not be NT services.
        /// </summary>
        COMADMIN_E_CANTMAKEINPROCSERVICE = (int)(0x80110814 - 0x100000000),
        /// <summary>
        /// The ProgID provided to the copy operation is invalid. The ProgID is in use by another registered CLSID.
        /// </summary>
        COMADMIN_E_PROGIDINUSEBYCLSID = (int)(0x80110815 - 0x100000000),
        /// <summary>
        /// The partition specified as default is not a member of the partition set.
        /// </summary>
        COMADMIN_E_DEFAULT_PARTITION_NOT_IN_SET = (int)(0x80110816 - 0x100000000),
        /// <summary>
        /// A recycled process may not be paused.
        /// </summary>
        COMADMIN_E_RECYCLEDPROCESSMAYNOTBEPAUSED = (int)(0x80110817 - 0x100000000),
        /// <summary>
        /// Access to the specified partition is denied.
        /// </summary>
        COMADMIN_E_PARTITION_ACCESSDENIED = (int)(0x80110818 - 0x100000000),
        /// <summary>
        /// Only Application Files (*.MSI files) can be installed into partitions.
        /// </summary>
        COMADMIN_E_PARTITION_MSI_ONLY = (int)(0x80110819 - 0x100000000),
        /// <summary>
        /// Applications containing one or more legacy components may not be exported to 1.0 format.
        /// </summary>
        COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_1_0_FORMAT = (int)(0x8011081A - 0x100000000),
        /// <summary>
        /// Legacy components may not exist in non-base partitions.
        /// </summary>
        COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_NONBASE_PARTITIONS = (int)(0x8011081B - 0x100000000),
        /// <summary>
        /// A component cannot be moved (or copied) from the System Application, an application proxy or a non-changeable application
        /// </summary>
        COMADMIN_E_COMP_MOVE_SOURCE = (int)(0x8011081C - 0x100000000),
        /// <summary>
        /// A component cannot be moved (or copied) to the System Application, an application proxy or a non-changeable application
        /// </summary>
        COMADMIN_E_COMP_MOVE_DEST = (int)(0x8011081D - 0x100000000),
        /// <summary>
        /// A private component cannot be moved (or copied) to a library application or to the base partition
        /// </summary>
        COMADMIN_E_COMP_MOVE_PRIVATE = (int)(0x8011081E - 0x100000000),
        /// <summary>
        /// The Base Application Partition exists in all partition sets and cannot be removed.
        /// </summary>
        COMADMIN_E_BASEPARTITION_REQUIRED_IN_SET = (int)(0x8011081F - 0x100000000),
        /// <summary>
        /// Alas, Event Class components cannot be aliased.
        /// </summary>
        COMADMIN_E_CANNOT_ALIAS_EVENTCLASS = (int)(0x80110820 - 0x100000000),
        /// <summary>
        /// Access is denied because the component is private.
        /// </summary>
        COMADMIN_E_PRIVATE_ACCESSDENIED = (int)(0x80110821 - 0x100000000),
        /// <summary>
        /// The specified SAFER level is invalid.
        /// </summary>
        COMADMIN_E_SAFERINVALID = (int)(0x80110822 - 0x100000000),
        /// <summary>
        /// The specified user cannot write to the system registry
        /// </summary>
        COMADMIN_E_REGISTRY_ACCESSDENIED = (int)(0x80110823 - 0x100000000),
        /// <summary>
        /// No information avialable.
        /// </summary>
        COMADMIN_E_PARTITIONS_DISABLED = (int)(0x80110824 - 0x100000000),
        /// <summary>
        /// Failed to open a file.
        /// </summary>
        NS_E_FILE_OPEN_FAILED = (int)(0xC00D001DL - 0x100000000),
    }
}
/******************************************************************************/
/*                END OF FILE                 */
/******************************************************************************/