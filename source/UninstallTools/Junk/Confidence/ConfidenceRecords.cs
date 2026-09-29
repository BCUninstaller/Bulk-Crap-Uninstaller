/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using UninstallTools.Properties;

namespace UninstallTools.Junk.Confidence
{
    /// <summary>
    /// Universal confidence pieces
    /// </summary>
    public static class ConfidenceRecords
    {
        public static readonly ConfidenceRecord CompanyNameDidNotMatch = new(-2, Localisation.ConfidencePart_CompanyNameDidNotMatch);

        public static readonly ConfidenceRecord CompanyNameMatch = new(4, Localisation.ConfidencePart_CompanyNameMatch);

        public static readonly ConfidenceRecord AllSubdirsMatched = new(4, Localisation.ConfidencePart_AllSubdirsMatched);

        public static readonly ConfidenceRecord DirectoryStillUsed = new(-7, Localisation.ConfidencePart_DirectoryStillUsed);

        public static readonly ConfidenceRecord ExplicitConnection = new(4, Localisation.ConfidencePart_ExplicitConnection);

        public static readonly ConfidenceRecord ItemNameEqualsCompanyName = new(-2, Localisation.ConfidencePart_ItemNameEqualsCompanyName);

        public static readonly ConfidenceRecord ProductNameDodgyMatch = new(-2, Localisation.ConfidencePart_ProductNameDodgyMatch);

        public static readonly ConfidenceRecord ProductNamePerfectMatch = new(2, Localisation.ConfidencePart_ProductNamePerfectMatch);

        public static readonly ConfidenceRecord QuestionableDirectoryName = new(-3, Localisation.ConfidencePart_QuestionableDirectoryName);

        public static readonly ConfidenceRecord IsUninstallerRegistryKey = new(20, Localisation.ConfidencePart_IsUninstallerRegistryKey);

        public static readonly ConfidenceRecord IsStoreApp = new(-10, Localisation.ConfidencePart_IsStoreApp);

        public static readonly ConfidenceRecord IsEmptyFolder = new(4, Localisation.Confidence_PF_EmptyFolder);

        public static readonly ConfidenceRecord ExecutablesArePresent = new(-4, Localisation.Confidence_PF_ExecsPresent);

        public static readonly ConfidenceRecord FilesArePresent = new(0, Localisation.Confidence_PF_FilesPresent);

        public static readonly ConfidenceRecord ManyFilesArePresent = new(-2, Localisation.Confidence_PF_ManyFilesPresent);

        public static readonly ConfidenceRecord ProgramNameIsStillUsed = new(-4, Localisation.Confidence_PF_NameIsUsed);

        public static readonly ConfidenceRecord FolderHasNoSubdirectories = new(2, Localisation.Confidence_PF_NoSubdirs);

        public static readonly ConfidenceRecord PublisherIsStillUsed = new(-4, Localisation.Confidence_PF_PublisherIsUsed);

        public static readonly ConfidenceRecord UsedBySimilarNamedApp = new(-2, Localisation.Confidence_UsedBySimilarNamedApp);

        public static readonly ConfidenceRecord SameProductNameStillInstalled = new(-7, Localisation.Confidence_UsedBySimilarNamedApp);

        public static readonly ConfidenceRecord DirectlyInsideKnownFolder = new(-1, Localisation.Confidence_DirectlyInsideKnownFolder);
    }
}