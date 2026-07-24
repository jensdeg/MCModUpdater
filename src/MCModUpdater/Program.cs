using MCModUpdater.Helpers;
using MCModUpdater.ModDownloaders;

var moddownloaders = typeof(Program).Assembly
    .GetTypes()
    .Where(t => typeof(IModDownloader).IsAssignableFrom(t) && !t.IsInterface)
    .Select(t => (IModDownloader)Activator.CreateInstance(t)!)
    .ToList();

List<string> value = [
    "Fabric Carpet",
    "Entity Culling",
    "Distant Horizons",
    "Axiom",
    "FerriteCore",
    "Item Scroller",
    "Iris",
    "Placeholder API",
    "Sodium Extra",
    "Yet Another Config Lib",
    "Tweakeroo",
    "Bobby",
    "ClientSort",
    "Cloth Config",
    "Continuity",
    "Dynamic FPS",
    "Fabric API",
    "Fabric Language Kotlin",
    "Freecam",
    "ImmediatelyFast",
    "LambDynamicLights",
    "Litematica",
    "MaLiLib",
    "Reese's Sodium Options",
    "Replay Mod",
    "Sodium",
    "Zoomify",
];
var version = "26.2";

List<string> mods = value;
var modloader = "Fabric";

foreach (var downloader in moddownloaders)
{
    if (downloader is null) continue;
    await downloader.Download(mods, version, modloader);

    // 1 rerun
    if (File.Exists(downloader.FailedModsFile))
    {
        Console.WriteLine($"{Environment.NewLine}Rerunning on failed mods");
        var failedmods = File.ReadAllLines(downloader.FailedModsFile).ToList();
        File.Delete(downloader.FailedModsFile);
        await downloader.Download(failedmods, version, modloader);
    }
}
