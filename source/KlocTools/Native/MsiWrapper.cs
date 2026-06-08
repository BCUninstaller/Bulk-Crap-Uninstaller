/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Klocman.Native
{
    public static class MsiWrapper
    {
        public enum INSTALLFEATUREATTRIBUTE // bit flags
        {
            INSTALLFEATUREATTRIBUTE_FAVORLOCAL = 1 << 0,
            INSTALLFEATUREATTRIBUTE_FAVORSOURCE = 1 << 1,
            INSTALLFEATUREATTRIBUTE_FOLLOWPARENT = 1 << 2,
            INSTALLFEATUREATTRIBUTE_FAVORADVERTISE = 1 << 3,
            INSTALLFEATUREATTRIBUTE_DISALLOWADVERTISE = 1 << 4,
            INSTALLFEATUREATTRIBUTE_NOUNSUPPORTEDADVERTISE = 1 << 5
        }

        public enum INSTALLLEVEL
        {
            INSTALLLEVEL_DEFAULT = 0, // install authored default
            INSTALLLEVEL_MINIMUM = 1, // install only required features
            INSTALLLEVEL_MAXIMUM = 0xFFFF // install all features
        } // intermediate levels dependent on authoring

        public enum INSTALLLOGATTRIBUTES // flag attributes for MsiEnableLog
        {
            INSTALLLOGATTRIBUTES_APPEND = (1 << 0),
            INSTALLLOGATTRIBUTES_FLUSHEACHLINE = (1 << 1)
        }

        public enum INSTALLLOGMODE // bit flags for use with MsiEnableLog and MsiSetExternalUI
        {
            INSTALLLOGMODE_FATALEXIT = (1 << (0x00 >> 24)),
            INSTALLLOGMODE_ERROR = (1 << (0x01 >> 24)),
            INSTALLLOGMODE_WARNING = (1 << (0x02 >> 24)),
            INSTALLLOGMODE_USER = (1 << (0x03 >> 24)),
            INSTALLLOGMODE_INFO = (1 << (0x04 >> 24)),
            INSTALLLOGMODE_RESOLVESOURCE = (1 << (0x06 >> 24)),
            INSTALLLOGMODE_OUTOFDISKSPACE = (1 << (0x07 >> 24)),
            INSTALLLOGMODE_ACTIONSTART = (1 << (0x08 >> 24)),
            INSTALLLOGMODE_ACTIONDATA = (1 << (0x09 >> 24)),
            INSTALLLOGMODE_COMMONDATA = (1 << (0x0B >> 24)),
            INSTALLLOGMODE_PROPERTYDUMP = (1 << (0x0A >> 24)), // log only
            INSTALLLOGMODE_VERBOSE = (1 << (0x0C >> 24)), // log only
            INSTALLLOGMODE_PROGRESS = (1 << (0x0A >> 24)), // external handler only
            INSTALLLOGMODE_INITIALIZE = (1 << (0x0C >> 24)), // external handler only
            INSTALLLOGMODE_TERMINATE = (1 << (0x0D >> 24)), // external handler only
            INSTALLLOGMODE_SHOWDIALOG = (1 << (0x0E >> 24)) // external handler only
        }

        // Install message type for callback is a combination of the following:
        //  A message box style:      MB_*, where MB_OK is the default
        //  A message box icon type:  MB_ICON*, where no icon is the default
        //  A default button:         MB_DEFBUTTON?, where MB_DEFBUTTON1 is the default
        //  One of the following install message types, no default
        public enum INSTALLMESSAGE : long
        {
            INSTALLMESSAGE_FATALEXIT = 0x00000000L, // premature termination, possibly fatal OOM
            INSTALLMESSAGE_ERROR = 0x01000000L, // formatted error message
            INSTALLMESSAGE_WARNING = 0x02000000L, // formatted warning message
            INSTALLMESSAGE_USER = 0x03000000L, // user request message
            INSTALLMESSAGE_INFO = 0x04000000L, // informative message for log
            INSTALLMESSAGE_FILESINUSE = 0x05000000L, // list of files in use that need to be replaced
            INSTALLMESSAGE_RESOLVESOURCE = 0x06000000L, // request to determine a valid source location
            INSTALLMESSAGE_OUTOFDISKSPACE = 0x07000000L, // insufficient disk space message
            INSTALLMESSAGE_ACTIONSTART = 0x08000000L, // start of action: action name & description
            INSTALLMESSAGE_ACTIONDATA = 0x09000000L, // formatted data associated with individual action item
            INSTALLMESSAGE_PROGRESS = 0x0A000000L, // progress gauge info: units so far, total
            INSTALLMESSAGE_COMMONDATA = 0x0B000000L, // product info for dialog: language Id, dialog caption
            INSTALLMESSAGE_INITIALIZE = 0x0C000000L, // sent prior to UI initialization, no string data
            INSTALLMESSAGE_TERMINATE = 0x0D000000L, // sent after UI termination, no string data
            INSTALLMESSAGE_SHOWDIALOG = 0x0E000000L // sent prior to display or authored dialog or wizard
        }

        public enum INSTALLMODE
        {
            INSTALLMODE_NOSOURCERESOLUTION = -3, // skip source resolution
            INSTALLMODE_NODETECTION = -2, // skip detection
            INSTALLMODE_EXISTING = -1, // provide, if available
            INSTALLMODE_DEFAULT = 0 // install, if absent
        }

        public enum INSTALLSTATE
        {
            INSTALLSTATE_NOTUSED = -7, // component disabled
            INSTALLSTATE_BADCONFIG = -6, // configuration data corrupt
            INSTALLSTATE_INCOMPLETE = -5, // installation suspended or in progress
            INSTALLSTATE_SOURCEABSENT = -4, // run from source, source is unavailable
            INSTALLSTATE_MOREDATA = -3, // return buffer overflow
            INSTALLSTATE_INVALIDARG = -2, // invalid function argument
            INSTALLSTATE_UNKNOWN = -1, // unrecognized product or feature
            INSTALLSTATE_BROKEN = 0, // broken
            INSTALLSTATE_ADVERTISED = 1, // advertised feature
            INSTALLSTATE_REMOVED = 1, // component being removed (action state, not settable)
            INSTALLSTATE_ABSENT = 2, // uninstalled (or action state absent but clients remain)
            INSTALLSTATE_LOCAL = 3, // installed on local drive
            INSTALLSTATE_SOURCE = 4, // run from source, CD or net
            INSTALLSTATE_DEFAULT = 5 // use default, local or source
        }

        public enum INSTALLTYPE
        {
            INSTALLTYPE_DEFAULT = 0, // set to indicate default behavior
            INSTALLTYPE_NETWORK_IMAGE = 1 // set to indicate network install
        }

        public enum INSTALLUILEVEL
        {
            INSTALLUILEVEL_NOCHANGE = 0, // UI level is unchanged
            INSTALLUILEVEL_DEFAULT = 1, // default UI is used
            INSTALLUILEVEL_NONE = 2, // completely silent installation
            INSTALLUILEVEL_BASIC = 3, // simple progress and error handling
            INSTALLUILEVEL_REDUCED = 4, // authored UI, wizard dialogs suppressed
            INSTALLUILEVEL_FULL = 5, // authored UI with wizards, progress, errors
            INSTALLUILEVEL_ENDDIALOG = 0x80, // display success/failure dialog at end of install
            INSTALLUILEVEL_PROGRESSONLY = 0x40 // display only progress dialog
        }

        public enum MSICOLINFO
        {
            MSICOLINFO_NAMES = 0, // return column names
            MSICOLINFO_TYPES = 1 // return column definitions, datatype code followed    by width
        }

        public enum MSICONDITION
        {
            MSICONDITION_FALSE = 0, // expression evaluates to False
            MSICONDITION_TRUE = 1, // expression evaluates to True
            MSICONDITION_NONE = 2, // no expression present
            MSICONDITION_ERROR = 3 // syntax error in expression
        }

        public enum MSICOSTTREE
        {
            MSICOSTTREE_SELFONLY = 0,
            MSICOSTTREE_CHILDREN = 1,
            MSICOSTTREE_PARENTS = 2,
            MSICOSTTREE_RESERVED = 3 // Reserved for future use
        }

        // -------------------------------------------------------------------------
        // Functions to query and configure a product as a whole.
        // -------------------------------------------------------------------------

        public enum MSIDBSTATE
        {
            MSIDBSTATE_ERROR = -1, // invalid database handle
            MSIDBSTATE_READ = 0, // database open read-only, no persistent changes
            MSIDBSTATE_WRITE = 1 // database readable and updatable
        }

        public enum MSIMODIFY
        {
            MSIMODIFY_SEEK = -1, // reposition to current record primary    key
            MSIMODIFY_REFRESH = 0, // refetch current record data
            MSIMODIFY_INSERT = 1, // insert new record, fails if matching    key exists
            MSIMODIFY_UPDATE = 2, // update existing non-key data of fetched    record
            MSIMODIFY_ASSIGN = 3, // insert record, replacing any existing    record
            MSIMODIFY_REPLACE = 4, // update record, delete old if primary    key edit
            MSIMODIFY_MERGE = 5, // fails if record with duplicate key not    identical
            MSIMODIFY_DELETE = 6, // remove row referenced by this record    from table
            MSIMODIFY_INSERT_TEMPORARY = 7, // insert a temporary record
            MSIMODIFY_VALIDATE = 8, // validate a fetched record
            MSIMODIFY_VALIDATE_NEW = 9, // validate a new record
            MSIMODIFY_VALIDATE_FIELD = 10, // validate field(s) of an incomplete    record
            MSIMODIFY_VALIDATE_DELETE = 11 // validate before deleting record
        }

        public enum REINSTALLMODE // bit flags
        {
            REINSTALLMODE_REPAIR = 0x00000001, // Reserved bit - currently ignored
            REINSTALLMODE_FILEMISSING = 0x00000002, // Reinstall only if file is missing
            REINSTALLMODE_FILEOLDERVERSION = 0x00000004, // Reinstall if file is missing, or older version
            REINSTALLMODE_FILEEQUALVERSION = 0x00000008, // Reinstall if file is missing, or equal or older version
            REINSTALLMODE_FILEEXACT = 0x00000010, // Reinstall if file is        missing, or not exact version
            REINSTALLMODE_FILEVERIFY = 0x00000020, // checksum executables,        reinstall if missing or corrupt
            REINSTALLMODE_FILEREPLACE = 0x00000040, // Reinstall all files,        regardless of version
            REINSTALLMODE_MACHINEDATA = 0x00000080, // insure required machine        reg entries
            REINSTALLMODE_USERDATA = 0x00000100, // insure required user reg        entries
            REINSTALLMODE_SHORTCUT = 0x00000200, // validate shortcuts items
            REINSTALLMODE_PACKAGE = 0x00000400 // use re-cache source        install package
        }

        public enum USERINFOSTATE
        {
            USERINFOSTATE_MOREDATA = -3, // return buffer overflow
            USERINFOSTATE_INVALIDARG = -2, // invalid function argument
            USERINFOSTATE_UNKNOWN = -1, // unrecognized product
            USERINFOSTATE_ABSENT = 0, // user info and PID not initialized
            USERINFOSTATE_PRESENT = 1 // user info and PID initialized
        }

        /// <summary>
        /// Flags that may be passed into the dwContext parameter of the <see cref="MsiEnumProducts"/> method.
        /// https://github.com/dotnet/pinvoke/blob/d974353ca67c4b8e6009096bd3ab7e6f284ed11d/src/Msi/Msi%2BMSIINSTALLCONTEXT.cs
        /// </summary>
        [Flags]
        public enum MSIINSTALLCONTEXT : uint
        {
            /// <summary>
            /// Product visible to the current user.
            /// </summary>
            MSIINSTALLCONTEXT_FIRSTVISIBLE = 0,

            /// <summary>
            /// Invalid context for a product
            /// </summary>
            MSIINSTALLCONTEXT_NONE = 0,

            /// <summary>
            /// Enumeration extended to all per–user–managed installations for the users specified by szUserSid. An invalid SID returns no items.
            /// </summary>
            MSIINSTALLCONTEXT_USERMANAGED = 0x1,

            /// <summary>
            /// Enumeration extended to all per–user–unmanaged installations for the users specified by szUserSid. An invalid SID returns no items.
            /// </summary>
            MSIINSTALLCONTEXT_USERUNMANAGED = 0x2,

            /// <summary>
            /// Enumeration extended to all per-machine installations. When dwInstallContext is set to MSIINSTALLCONTEXT_MACHINE only, the szUserSID parameter must be NULL.
            /// </summary>
            MSIINSTALLCONTEXT_MACHINE = 0x4,

            /// <summary>
            /// All contexts. OR of all valid values
            /// </summary>
            MSIINSTALLCONTEXT_ALL = MSIINSTALLCONTEXT_USERMANAGED | MSIINSTALLCONTEXT_USERUNMANAGED | MSIINSTALLCONTEXT_MACHINE,

            /// <summary>
            /// All user-managed contexts.
            /// </summary>
            MSIINSTALLCONTEXT_ALLUSERMANAGED = 0x8,
        }

        public const int MAX_FEATURE_CHARS = 38; // maximum chars in feature name (same as string GUID)
        // MsiOpenDatabase persist predefine values, otherwise output database path is used
        public const string MSIDBOPEN_READONLY = "0"; // database open read-only, no persistent changes
        public const string MSIDBOPEN_TRANSACT = "1"; // database read/write in transaction mode
        public const string MSIDBOPEN_DIRECT = "2"; // database direct read/write without transaction
        public const string MSIDBOPEN_CREATE = "3"; // create new database, transact mode read/write
        public const string MSIDBOPEN_CREATEDIRECT = "4"; // create new database, direct mode read/write
        // -------------------------------------------------------------------------

        // Functions to set the UI handling and logging. The UI will be used for error,
        // progress, and log messages for all subsequent calls to Installer Service
        // API functions that require UI.
        // -------------------------------------------------------------------------


        // Enable internal UI
        [DllImport("msi", CharSet = CharSet.Auto)] // UI level
        // handle of owner window
        public static extern INSTALLUILEVEL MsiSetInternalUI(INSTALLUILEVEL dwUILevel, ref IntPtr winhandle);

        // Enable logging to a file for all install sessions for the client process,
        // with control over which log messages are passed to the specified log file.
        // Messages are designated with a combination of bits from INSTALLLOGMODE enum.
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiEnableLog(
            int dwLogMode, // bit flags designating operations to report
            string szLogFile, // log file, or NULL to disable logging
            int dwLogAttributes);

        // Return the installed state for a product
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern INSTALLSTATE MsiQueryProductState(
            string szProduct);

        // Return product info
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiGetProductInfo(
            string szProduct, // product code
            string szAttribute, // attribute name, case-sensitive
            string lpValueBuf, // returned value, NULL if not desired ref?
            ref int len); // in/out buffer character count
        // Install a new product.
        // Either may be NULL, but the DATABASE property must be specfied
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiInstallProduct(
            string szPackagePath, // location of package to install
            string szCommandLine); // command line <property settings>
        // Install/uninstall an advertised or installed product
        // No action if installed and INSTALLSTATE_DEFAULT specified
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiConfigureProduct(
            string szProduct, // product code
            int iInstallLevel, // how much of the product to install
            INSTALLSTATE eInstallState); // local/source/default/absent/lock/uncache
        // Reinstall product, used to validate or correct problems
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiReinstallProduct(
            string szProduct, // product code
            int szReinstallMode); // one or more REINSTALLMODE modes
        // Return the product code for a registered component, called once by apps
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiGetProductCode(
            string szComponent, // component Id registered for this product
            [Out] StringBuilder lpBuf39); // returned string GUID, sized for 39 characters
        // Return the registered user information for an installed product
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern USERINFOSTATE MsiGetUserInfo(
            string szProduct, // product code, string GUID
            string UserNameBuf, // return user name
            ref int UserNameBufLen, // in/out buffer character count
            string OrgNameBuf, // return company name
            ref int OrgNameBufLen, // in/out buffer character count
            string SerialBuf, // return product serial number
            ref int SerialBufLen); // in/out buffer character count
        // Obtain and store user info and PID from installation wizard (first run)
        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiCollectUserInfo(
            string szProduct); // product code, string GUID
        //msiQuery.h


        // -------------------------------------------------------------------------
        // Installer database management functions - not used by custom actions
        // -------------------------------------------------------------------------

        // Open an installer database, specifying the persistance mode, which is a pointer.
        // Predefined persist values are reserved pointer values, requiring pointer arithmetic.
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiOpenDatabase(string dbpath, string persist,
            ref IntPtr msihandle);

        // Import an MSI text archive table into an open database
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiDatabaseImport(IntPtr msihandle,
            string FolderPath, // folder containing archive files
            string FileName); // table archive file to be imported
        // Export an MSI table from an open database to a text archive file
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiDatabaseExport(IntPtr msihandle,
            string TableName, // name of table in database <case-sensitive>
            string FolderPath, // folder containing archive files
            string FileName); // name of exported table archive file
        // Merge two database together, allowing duplicate rows
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiDatabaseMerge(IntPtr msihandle,
            IntPtr msihandle2, // database to be merged into hDatabase
            string TableName); // name of non-persistent table to receive errors
        // Write out all persistent table data, ignored if database opened read-only
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi")]
        public static extern int MsiDatabaseCommit(IntPtr msihandle);

        // Return the update state of a database

        [DllImport("msi")]
        public static extern MSIDBSTATE MsiGetDatabaseState(IntPtr msihandle);

        // -------------------------------------------------------------------------
        // Installer database access functions
        // -------------------------------------------------------------------------

        // Prepare a database query, creating a view object
        // Returns ERROR_SUCCESS if successful, and the view handle is returned,
        // else ERROR_INVALID_HANDLE, ERROR_INVALID_HANDLE_STATE, ERROR_BAD_QUERY_SYNTAX, ERROR_GEN_FAILURE
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi")]
        public static extern int MsiDatabaseOpenView(IntPtr handle, string query,
            ref IntPtr viewhandle);

        // Exectute the view query, supplying parameters as required
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_HANDLE_STATE, ERROR_GEN_FAILURE
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi")]
        public static extern int MsiViewExecute(IntPtr viewhandle, IntPtr
            recordhandle);

        // Fetch the next sequential record from the view
        // Result is ERROR_SUCCESS if a row is found, and its handle is returned
        // else ERROR_NO_MORE_ITEMS if no records remain, and a null handle is returned
        // else result is error: ERROR_INVALID_HANDLE_STATE, ERROR_INVALID_HANDLE, ERROR_GEN_FAILURE

        [DllImport("msi")]
        public static extern int MsiViewFetch(IntPtr viewhandle, ref IntPtr
            recordhandle);

        // Modify a database record, parameters must match types in query columns
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_HANDLE_STATE, ERROR_GEN_FAILURE, ERROR_ACCESS_DENIED
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi")]
        public static extern int MsiViewModify(IntPtr viewhandle, MSIMODIFY eModifyMode, // modify action to perform
            IntPtr recordhandle); // record obtained from fetch, or new record
        // Return the column names or specifications for the current view
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_PARAMETER, or ERROR_INVALID_HANDLE_STATE

        [DllImport("msi")]
        public static extern int MsiViewGetColumnInfo(IntPtr viewhandle, MSICOLINFO eColumnInfo,
            // retrieve columns names or definitions
            ref IntPtr recordhandle); // returned data record containing all names or definitions

        [DllImport("msi")]
        public static extern int MsiCloseHandle(IntPtr handle);

        // Release the result set for an executed view, to allow re-execution
        // Only needs to be called if not all records have been fetched
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_HANDLE_STATE

        [DllImport("msi")]
        public static extern int MsiViewClose(IntPtr viewhandle);

        // Return a record containing the names of all primary key columns for a given table
        // Returns an MSIHANDLE for a record containing the name of each column.
        // The field count of the record corresponds to the number of primary key columns.
        // Field [0] of the record contains the table name.
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_TABLE

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiDatabaseGetPrimaryKeys(IntPtr msihandle,
            string szTableName, // the name of a specific table <case-sensitive>
            ref IntPtr recordhandle); // returned record if ERROR_SUCCESS
        // -------------------------------------------------------------------------
        // Record object functions
        // -------------------------------------------------------------------------

        // Create a new record object with the requested number of fields
        // Field 0, not included in count, is used for format strings and op codes
        // All fields are initialized to null
        // Returns a handle to the created record, or 0 if memory could not be allocated

        [DllImport("msi")]
        public static extern IntPtr MsiCreateRecord(
            int Params); // the number of data fields
        // Report whether a record field is NULL
        // Returns TRUE if the field is null or does not exist
        // Returns FALSE if the field contains data, or the handle is invalid

        [DllImport("msi")]
        public static extern bool MsiRecordIsNull(IntPtr recordhandle,
            int Field);

        // Return the length of a record field
        // Returns 0 if field is NULL or non-existent
        // Returns sizeof(int) if integer data
        // Returns character count if string data (not counting null terminator)
        // Returns bytes count if stream data

        [DllImport("msi")]
        public static extern int MsiRecordDataSize(IntPtr recordhandle,
            int Field);

        // Set a record field to an integer value
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_FIELD

        [DllImport("msi")]
        public static extern int MsiRecordSetInteger(IntPtr recordhandle,
            int Field,
            int Value);

        // Copy a string into the designated field
        // A null string pointer and an empty string both set the field to null
        // Returns ERROR_SUCCESS, ERROR_INVALID_HANDLE, ERROR_INVALID_FIELD

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiRecordSetString(IntPtr recordhandle,
            int Field,
            string Value);

        // Return the integer value from a record field
        // Returns the value MSI_NULL_INTEGER if the field is null
        // or if the field is a string that cannot be converted to an integer

        [DllImport("msi")]
        public static extern int MsiRecordGetInteger(IntPtr recordhandle,
            int Field);

        // Return the string value of a record field
        // Integer fields will be converted to a string
        // Null and non-existent fields will report a value of 0
        // Fields containing stream data will return ERROR_INVALID_DATATYPE
        // Returns ERROR_SUCCESS, ERROR_MORE_DATA,
        //         ERROR_INVALID_HANDLE, ERROR_INVALID_FIELD, ERROR_BAD_ARGUMENTS

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiRecordGetString(IntPtr recordhandle,
            int Field,
            string ValueBuf, // buffer for returned value
            ref int len); // in/out buffer character count
        // Returns the number of fields allocated in the record
        // Does not count field 0, used for formatting and op codes

        [DllImport("msi")]
        public static extern int MsiRecordGetFieldCount(IntPtr recordhandle);

        // Set a record stream field from a file
        // The contents of the specified file will be read into a stream object
        // The stream will be persisted if the record is inserted into the database
        // Execution of this function sets the error record, accessible via MsiGetLastErrorRecord

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiRecordSetStream(IntPtr recordhandle,
            int Field,
            string FilePath); // path to file containing stream data
        // Read bytes from a record stream field into a buffer
        // Must set the in/out argument to the requested byte count to read
        // The number of bytes transferred is returned through the argument
        // If no more bytes are available, ERROR_SUCCESS is still returned

        [DllImport("msi", CharSet = CharSet.Auto)]
        public static extern int MsiRecordReadStream(IntPtr recordhandle,
            int Field,
            string DataBuf, // buffer to receive bytes from stream
            ref int len); // in/out buffer byte count
        // Clears all data fields in a record to NULL

        [DllImport("msi")]
        public static extern int MsiRecordClearData(IntPtr recordhandle);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiGetProductInfo(string product, string property, [Out] StringBuilder valueBuf,
            ref int len);

        [DllImport("msi.dll", SetLastError = true)]
        public static extern int MsiEnumProducts(int iProductIndex, StringBuilder lpProductBuf);

        // The MsiGetFileSignatureInformation function takes the path to a file that has been digitally
        // signed and returns the file's signer certificate and hash.
        [DllImport("msi")]
        public static extern int MsiGetFileSignatureInformation([In] string szSignedObjectPath, [In] uint dwFlags,
            out IntPtr ppcCertContext, [Out] byte[] pbHashData, ref uint pcbHashData);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        //http://msdn.microsoft.com/library/en-us/msi/setup/msienumcomponents.asp
        public static extern int MsiEnumComponents(int iComponentIndex, StringBuilder lpComponentBuf);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern INSTALLSTATE MsiGetComponentPath(string szProduct, string szComponent, StringBuilder lpPathBuf, ref int pcchPathBuf);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern int MsiQueryComponentState(string szProductCode, string szUserSid, [MarshalAs(UnmanagedType.I4)] MSIINSTALLCONTEXT dwContext, string szComponent, out INSTALLSTATE pdwState);

        public sealed class INSTALLPROPERTY
        {
            // Product info attributes: advertised information
            public static INSTALLPROPERTY PACKAGENAME = new("PackageName");
            public static INSTALLPROPERTY TRANSFORMS = new("Transforms");
            public static INSTALLPROPERTY LANGUAGE = new("Language");
            public static INSTALLPROPERTY PRODUCTNAME = new("ProductName");
            public static INSTALLPROPERTY ASSIGNMENTTYPE = new("AssignmentType");
            public static INSTALLPROPERTY PACKAGECODE = new("PackageCode");
            public static INSTALLPROPERTY VERSION = new("Version");
            public static INSTALLPROPERTY PRODUCTICON = new("ProductIcon");
            // Product info attributes: installed information
            public static INSTALLPROPERTY INSTALLEDPRODUCTNAME = new("InstalledProductName");
            public static INSTALLPROPERTY VERSIONSTRING = new("VersionString");
            public static INSTALLPROPERTY HELPLINK = new("HelpLink");
            public static INSTALLPROPERTY HELPTELEPHONE = new("HelpTelephone");
            public static INSTALLPROPERTY INSTALLLOCATION = new("InstallLocation");
            public static INSTALLPROPERTY INSTALLSOURCE = new("InstallSource");
            public static INSTALLPROPERTY INSTALLDATE = new("InstallDate");
            public static INSTALLPROPERTY PUBLISHER = new("Publisher");
            public static INSTALLPROPERTY LOCALPACKAGE = new("LocalPackage");
            public static INSTALLPROPERTY URLINFOABOUT = new("URLInfoAbout");
            public static INSTALLPROPERTY URLUPDATEINFO = new("URLUpdateInfo");
            public static INSTALLPROPERTY VERSIONMINOR = new("VersionMinor");
            public static INSTALLPROPERTY VERSIONMAJOR = new("VersionMajor");

            private INSTALLPROPERTY(string name)
            {
                PropertyName = name;
            }

            public string PropertyName { get; }

            public override string ToString()
            {
                return PropertyName;
            }
        }
    }
}