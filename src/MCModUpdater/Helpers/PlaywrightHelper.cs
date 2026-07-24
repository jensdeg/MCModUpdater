using Microsoft.Playwright;

namespace MCModUpdater.Helpers;

public class PlaywrightHelper()
{
    private IBrowser _browser = null!;
    private IPlaywright _playwright = null!;
    public IPage Page { get; set; } = null!;

    private async Task Initialize()
    {
        _playwright ??= await Playwright.CreateAsync();
        _browser ??= await _playwright.Chromium.LaunchAsync(PlaywrightOptions.Launch);
    }

    public async Task Open(string url)
    {
        await Initialize();
        var context = await _browser.NewContextAsync(PlaywrightOptions.BrowserContext);
        Page = await context.NewPageAsync();

        await Page.GotoAsync(url, PlaywrightOptions.PageGoto);
    }

    public async Task Press(string text, AriaRole ariaRole = AriaRole.Button)
        => await Page.GetByRole(ariaRole).GetByText(text).ClickAsync(PlaywrightOptions.Click);


    public async Task<bool> SelectDropdownOptionCurseForge(string dropdownLabel, string optionText)
    {
        var dropdown = Page.Locator("div.dropdown", new() { Has = Page.Locator("div.subsection-title", new() { HasTextString = dropdownLabel }) });

        await dropdown.Locator("p.dropdown-selected-item").ClickAsync();

        var list = dropdown.Locator("ul.dropdown-list");
        await list.WaitForAsync();

        var option = list.GetByText(optionText, new() { Exact = true });

        var scrollBox = list.Locator("div[role='list']");
        for (int i = 0; i < 10 && await option.CountAsync() == 0; i++)
        {
            await scrollBox.EvaluateAsync("el => el.scrollTop += 200");
            await Page.WaitForTimeoutAsync(150);
        }

        if (await option.IsVisibleAsync()) 
        {
            await option.ClickAsync(); 
            return true;
        }
        
        return false;
    }

    public async Task Dispose()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
            await _browser.DisposeAsync();
        }
        _playwright?.Dispose();
    }
}
public static class PlaywrightOptions
{
    public static BrowserNewContextOptions BrowserContext => new()
    {
        Locale = "en-US",
    };

    public static BrowserTypeLaunchOptions Launch => new()
    {
        Headless = false,
    };

    public static PageGotoOptions PageGoto => new()
    {
        WaitUntil = WaitUntilState.Load,
    };

    public static LocatorClickOptions Click => new()
    {
        Timeout = 10000
    };
}


