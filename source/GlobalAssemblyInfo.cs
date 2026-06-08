using System.Diagnostics.CodeAnalysis;
using System.Resources;

[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Windows-only app")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Address properly at some point? Almost all of these get logged")]

[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Keep code portable")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Keep code portable")]
[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Keep code portable")]
[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Keep code portable")]

[assembly: SuppressMessage("Performance", "CA1510:Use throw helper methods", Justification = "Keep code consistent with existing patterns")]

[assembly: NeutralResourcesLanguage("en")]