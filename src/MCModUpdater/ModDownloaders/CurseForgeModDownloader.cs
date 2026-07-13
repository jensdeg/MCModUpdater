using MCModUpdater.Helpers;
using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;

namespace MCModUpdater.ModDownloaders;

public sealed class CurseForgeModDownloader : IModDownloader
{
    public string DownloadPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";

    private readonly PlaywrightHelper _playwright = new();

    private readonly string _curseforgeURL = "https://www.curseforge.com/minecraft";

    public async Task Download(List<string> mods, string MCversion)
    {
        await _playwright.Open(_curseforgeURL);
        await _playwright.Press("Got it");

        foreach(var mod in mods)
        {
            // search mod
            await _playwright.Page.GetByLabel("Search for a project").FillAsync(mod);
            await _playwright.Page.Keyboard.PressAsync("Enter");
            await _playwright.Page.GetByLabel("Go To", new() { Exact = false }).First.ClickAsync();

            // select version
            await _playwright.Press("download");
            await _playwright.Page.WaitForTimeoutAsync(200);
            var needsToAccept = await _playwright.Page.GetByRole(AriaRole.Button).GetByText("Accept").IsVisibleAsync();
            if (needsToAccept) await _playwright.Press("Accept");

            await _playwright.SelectDropdownOptionCurseForge("Select Game Version", MCversion);

            //download
            var downloadTask = _playwright.Page.WaitForDownloadAsync();
            await _playwright.Page.GetByLabel("Download file").ClickAsync();
            var download = await downloadTask;
            await download.SaveAsAsync(Path.Combine(DownloadPath, download.SuggestedFilename));

            //go back
            await _playwright.Page.GotoAsync(_curseforgeURL);
        }
    }
}
