using Klocman.Forms.Tools;
using UninstallTools.Properties;

namespace UninstallTools.Uninstaller;

/// <summary>
/// Thrown when there are no usable ways of uninstalling an application entry.
/// </summary>
/// <inheritdoc />
public sealed class NoWayToUninstallException() : PremadeDialogs.NotSendableException(Localisation.UninstallError_Nowaytouninstall);
