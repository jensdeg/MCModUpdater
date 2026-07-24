using MCModUpdater.ModDownloaders;

namespace MCModUpdater.Helpers;

public static class ModloaderExtensions
{
    extension(IModDownloader downloader)
    {
        public string FailedModsFile => Path.Combine(downloader.DownloadPath, "Mods", "_failedmods.txt");
        public string InstalledModsFile => Path.Combine(downloader.DownloadPath, "Mods", "_installedmods.txt");
    }
}
