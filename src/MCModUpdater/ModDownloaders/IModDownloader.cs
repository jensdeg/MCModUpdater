namespace MCModUpdater.ModDownloaders;

public interface IModDownloader
{
    public string DownloadPath { get; set; }
    Task Download(List<string> mods, string MCVersion, string modLoader);
}
