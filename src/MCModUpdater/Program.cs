using MCModUpdater.ModDownloaders;

var moddownloaders = typeof(Program).Assembly
    .GetTypes()
    .Where(t => typeof(IModDownloader).IsAssignableFrom(t) && !t.IsInterface)
    .Select(t => (IModDownloader)Activator.CreateInstance(t)!)
    .ToList();

List<string> mods = ["clientsort", "appleskin"];
var version = "26.2";
var modloader = "Fabric";

foreach (var downloader in moddownloaders)
{
    if (downloader is null) continue;
    await downloader.Download(mods, version, modloader);
}
