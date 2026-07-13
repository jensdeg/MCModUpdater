using MCModUpdater.Helpers;

namespace MCModUpdater.ModDownloaders;

public sealed class CurseForgeModDownloader : IModDownloader
{
    public string DownloadPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

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

            await playwright.Page.GetByLabel("Go To", new() { Exact = false }).First.ClickAsync();

            // select version
            await playwright.Press("download");
            await playwright.Press("Accept");

            if (!await playwright.SelectDropdownOptionCurseForge("Select Game Version", MCversion))
            {
                Console.WriteLine($"couldn't find mod {mod} for version {MCversion} on CurseForge");
            }
            await playwright.SelectDropdownOptionCurseForge("Select Mod Loaders", modLoader);

            //download
            var downloadTask = playwright.Page.WaitForDownloadAsync();
            await playwright.Page.GetByLabel("Download file").ClickAsync();
            var download = await downloadTask;
            await download.SaveAsAsync(Path.Combine(DownloadPath, download.SuggestedFilename));

            await playwright.Dispose();
        }
        catch
        {
            Console.WriteLine($"error install mod {mod} for version {MCversion} on CurseForge");
        }
    }
}
