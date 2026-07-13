namespace MCModUpdater.ModDownloaders;

public interface IModDownloader
{
    Task Download(List<string> mods, string MCVersion);
}
