using MCModUpdater.ModDownloaders;

var moddownloaders = typeof(Program).Assembly
    .GetTypes()
    .Where(t => typeof(IModDownloader).IsAssignableFrom(t) && !t.IsInterface)
    .Select(t => (IModDownloader)Activator.CreateInstance(t)!)
    .ToList();

List<string> mods = ["clientsort", "appleskin"];


foreach(var downloader in moddownloaders)
{
    if (downloader is null) continue;
    await downloader.Download(mods, "26.2");
}