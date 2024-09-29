using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;

class SimpleMethods
{
    private ChromeDriver _driver;

    // Constructor to initialize ChromeDriver only once
    public SimpleMethods()
    {/*
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Running in headless mode (without UI)
        _driver = new ChromeDriver(options); // Initialize the ChromeDriver once
        */
    }

    // Method to get player names based on Fpl_ID and gw (gameweek)
    public string[] GetPlayerNames(long Fpl_ID, int gw)
    {
        // Navigate to the specific FPL entry and gameweek page
        _driver.Navigate().GoToUrl($"https://fantasy.premierleague.com/entry/{Fpl_ID}/event/{gw}");

        // Wait for page load (you can replace this with a WebDriverWait for better control)
        Thread.Sleep(3000);

        // XPath to extract player names
        string xpath = "(//span[contains(@class,'styles__PitchElementData-sc-hv19ot-7 huoEoG')])";
        var elementsWithClassName = _driver.FindElements(By.XPath(xpath));

        List<string> playerNames = new List<string>();
        foreach (var element in elementsWithClassName)
        {
            string playerName = ConvertNewlineToSpace(element.Text);
            playerNames.Add(playerName);
        }

        return playerNames.ToArray();
    }
    public static long GetUnixTimestamp()
    {
        // Get the current time in UTC
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;

        // Return Unix time (seconds since January 1, 1970)
        return currentTime.ToUnixTimeSeconds();
    }

    public Dictionary<string, string> GetLeaguePlayerNamesDictionary(long Fpl_ID)
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");

        using (var driver = new ChromeDriver(options))
        {
            driver.Navigate().GoToUrl("https://fantasy.premierleague.com/leagues/" + Fpl_ID + "/standings/c");
            System.Threading.Thread.Sleep(3000); // You might want to use WebDriverWait instead of Thread.Sleep for better performance.

            // XPath to select the anchor elements that contain the player info
            var elements = driver.FindElements(By.XPath("//a[contains(@href, '/entry/')]"));

            // Dictionary to store player ID as the key and player name as the value
            Dictionary<string, string> playerData = new Dictionary<string, string>();

            // Extract IDs and player names from href attributes and text
            foreach (var element in elements)
            {
                string href = element.GetAttribute("href");
                string pName = element.Text;
                string id = href.Split('/')[4]; // Extract the player ID from the href

                if (!playerData.ContainsKey(id)) // Check if ID is already added to avoid duplicates
                {
                    playerData.Add(id, pName); // Add to dictionary
                }
            }

            driver.Quit();
            return playerData; // Return the dictionary of player IDs and names
        }
    }


    // Helper method to replace newlines with spaces in player names (if needed)
    private string ConvertNewlineToSpace(string input)
    {
        return input.Replace("\n", " ");
    }

    // Method to close the browser when done
    public void CloseBrowser()
    {
        if (_driver != null)
        {
            _driver.Quit(); // Quit the driver to close the browser
        }
    }
}
