using System.Windows.Forms;

namespace AAPD_Practical;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Contains("--self-test", StringComparer.OrdinalIgnoreCase))
        {
            EventRepositorySelfTests.Run();
            return;
        }
        ApplicationConfiguration.Initialize();
        Application.Run(new MainMenuForm());
    }
}
