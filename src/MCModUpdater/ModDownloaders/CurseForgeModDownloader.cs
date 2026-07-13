using MCModUpdater.Helpers;

namespace MCModUpdater.ModDownloaders;

public sealed class CurseForgeModDownloader : IModDownloader
{
    public string DownloadPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

    private readonly string _curseforgeURL = "https://www.curseforge.com/minecraft";

    public async Task Download(List<string> mods, string MCVersion)
    {
        var downloadTasks = mods.Select(mod => DownloadSingle(mod, MCVersion)).ToList();
        await Task.WhenAll(downloadTasks);
    }

    private async Task DownloadSingle(string mod, string MCversion)
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

        if(!await playwright.SelectDropdownOptionCurseForge("Select Game Version", MCversion))
        {
            Console.WriteLine($"couldn't find mod {mod} for version {MCversion} on CurseForge");
        }

        //download
        var downloadTask = playwright.Page.WaitForDownloadAsync();
        await playwright.Page.GetByLabel("Download file").ClickAsync();
        var download = await downloadTask;
        await download.SaveAsAsync(Path.Combine(DownloadPath, download.SuggestedFilename));

        await playwright.Dispose();
    }
}
