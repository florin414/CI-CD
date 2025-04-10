using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace XUnit.UnitTests.Verify;
public static class VerifierHelper
{
    public static Task VerifyResult(object result, string callerClass = "", [CallerMemberName] string testName = "", string parentDirectory = "Verify")
    {
        VerifySettings settings = new();
        if (string.IsNullOrEmpty(callerClass))
        {
            callerClass = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name;
        }
        settings.UseDirectory(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, $"{parentDirectory}/__snapshots__/{callerClass}/{testName}"));
        settings.UseFileName("test");

        return Verifier.Verify(result, settings);
    }
}