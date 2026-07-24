using MCModUpdater.Helpers;

namespace MCModUpdater.ModDownloaders;

public sealed class CurseForgeModDownloader : IModDownloader
{
    public string DownloadPath { get; set; } 
        = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

    private readonly string _curseforgeURL = "https://www.curseforge.com/minecraft";

    public async Task Download(List<string> mods, string MCVersion, string modLoader)
    {
        foreach (var chunk in mods.Chunk(5).ToList())
        {
            var chunktasks = chunk.Select(mod => DownloadSingle(mod, MCVersion, modLoader)).ToList();
            await Task.WhenAny(chunktasks);
        }
    }

    private async Task DownloadSingle(string mod, string MCversion, string modLoader)
    {
        try
        {
            var playwright = new PlaywrightHelper();

            await playwright.Open(_curseforgeURL);
            await playwright.Press("Got it");

            // search mod
            await playwright.Page.GetByLabel("Search for a project").FillAsync(mod);
            await playwright.Page.Keyboard.PressAsync("Enter");

            await playwright.Page
                .GetByLabel("Go To", new() { Exact = false })
                .First
                .ClickAsync(PlaywrightOptions.Click);

            // select version
            await playwright.Press("Accept");
            await playwright.Press("download");

            if (!await playwright.SelectDropdownOptionCurseForge("Select Game Version", MCversion))
            {
                LogFailed(mod);
                Console.Error.WriteLine($"couldn't find mod {mod} for version {MCversion} on CurseForge");
                return;
            }
            await playwright.SelectDropdownOptionCurseForge("Select Mod Loaders", modLoader);

            //download
            var downloadTask = playwright.Page.WaitForDownloadAsync();
            await playwright.Page.GetByLabel("Download file").ClickAsync(PlaywrightOptions.Click);
            var download = await downloadTask;
            await download.SaveAsAsync(Path.Combine(DownloadPath, "Mods", download.SuggestedFilename));

            LogSucces(mod);
            await playwright.Dispose();
        }
        catch
        {
            LogFailed(mod);
            Console.Error.WriteLine($"error installing mod {mod} for version {MCversion} on CurseForge");
        }
    }

    private void LogFailed(string mod)
    {
        var file = File.AppendText(this.FailedModsFile);
        file.WriteLine(mod);
        file.Close();
    }

    private void LogSucces(string mod)
    {
        var file = File.AppendText(this.InstalledModsFile);
        file.WriteLine(mod);
        file.Close();
    }
}
