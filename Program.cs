using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using System.Diagnostics.Metrics;

public class FolderManager

{
    SimpleMethods sm= new SimpleMethods();

    public static void Main(string[] args)
    {
       long startFplID = GetLeagueNumber("R2G");

        // six 153204
        // PovertyLeague 1089205
        // R2G 420969
        // h2h 153197 H
        // FPLwire 36074
        // Arsenal 36074
        // BetssonLeague 1173870
        // KasbyLeague 190771
        // ComicsLeague 1114702
        //  FantasyShow 56013
        string leagueName = GetLeagueNameByID(startFplID);
        Dictionary<string, string> leaguePlayerNames;

        if (leagueName == "h2h")
        {
            leaguePlayerNames = GetLeaguePlayerNamesDictionary(startFplID, "h");
            GetFplDetailsArray(leaguePlayerNames, 7, leagueName);
        }
        else
        {
            leaguePlayerNames = GetLeaguePlayerNamesDictionary(startFplID, "c");
            GetFplDetailsArray(leaguePlayerNames, 7, leagueName);
        }


    }


    // Dictionary to hold the key-value pairs (with long values)
    public static Dictionary<string, long> leagueData = new Dictionary<string, long>()
    {
        { "six", 153204L },
        { "PovertyLeague", 1089205L },
        { "R2G", 420969L },
        { "h2h", 153197L },
        { "India", 120 },
        { "FPLwire", 36074L },
        { "BetssonLeague", 1173870L },
        { "KasbyLeague", 190771L },
        { "ComicsLeague", 1114702L },
        { "Overall", 314 },
        { "Arsenal", 1 },
        { "FantasyShow", 56013L }
    };

    public static string GetLeagueNameByID(long leagueId)
    {
        foreach (var kvp in leagueData)
        {
            if (kvp.Value == leagueId)
            {
                return kvp.Key; // Return the key associated with the leagueId
            }
        }
        Console.WriteLine($"No league found with ID: {leagueId}");
        return null;
    }

    static long GetUnixTimestamp()
    {
        // Get the current time in UTC
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;

        // Return Unix time (seconds since January 1, 1970)
        return currentTime.ToUnixTimeSeconds();
    }


    // Method to check and create a folder if it doesn't exist
    public static void EnsureFolderExists(string basePath, string folderName)
    {
        // Combine the base path and folder name
        string folderPath = Path.Combine(basePath, folderName);

        // Check if the folder exists
        if (!Directory.Exists(folderPath))
        {
            // If not, create the folder
            Directory.CreateDirectory(folderPath);
            Console.WriteLine($"Folder created: {folderPath}");
        }
        else
        {
            Console.WriteLine("Folder already exists.");
        }

    }

    public static string ExtractCountryCode(string url)
    {
        string[] parts = url.Split('/');
        string countryCodeWithExtension = parts[parts.Length - 1];
        string countryCode = countryCodeWithExtension.Replace(".gif", "");
        return countryCode;
    }
    /*
    public string GenerateJsArray(List<JsonFPLMembers> members)
    {
        string jsonString = JsonSerializer.Serialize(members, new JsonSerializerOptions { WriteIndented = true });
        string jsOutput = $"var s = {jsonString};";
        long unixTime = jg.GetCurrentUnixTime();
        string dateFolderName = jg.GenerateDateFolderName();
        string mainPath = "/Users/sisu02/Documents/cloner/TenForBen.github.io/FPL/GW/GW5/DB/" + dateFolderName +
                          "/";
        string directoryPath = Path.GetDirectoryName(mainPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = directoryPath + "/" + unixTime + "_" + "_MaIN_league.js";
        File.WriteAllText(filePath, jsOutput);
        return jsOutput;
    }*/

    public static void GetFplDetailsArray(Dictionary<string, string> leaguePlayerNames, int gameweek ,string LeagueName)
    {
        List<JsonFPLMembers> fplDetailsList = new List<JsonFPLMembers>();
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Run the browser in headless mode

        using (var driver = new ChromeDriver(options))
        {
            int LogCounter=0;
            try
            {
                foreach (var entry in leaguePlayerNames)
                {
                    string teamName = entry.Value;
                    string managerId = entry.Key;

                    // Navigate to the manager's FPL details page for the specified gameweek
                    driver.Navigate().GoToUrl($"https://fantasy.premierleague.com/entry/{managerId}/event/{gameweek}");
                    Thread.Sleep(3000); // Wait for the page to load
                    options.AddArgument("--headless");
                    // Extract country code image URL
                    string xPathCountryImg = "//div[@class='sc-bdnxRM hbrYOM']/img";
                    string xPathLatestPoints = "//div[@class='EntryEvent__PrimaryValue-sc-l17rqm-4 jsdnqB']";

                    var element1 = driver.FindElement(By.XPath(xPathCountryImg));
                    var elementLp = driver.FindElement(By.XPath(xPathLatestPoints));
                    string Lp = elementLp.Text;
                    string src = element1.GetAttribute("src");
                    string countryCode = ExtractCountryCode(src); // Custom method to extract country code

                    // Extract player details
                    string ply = "(//span[contains(@class,'styles__PitchElementData-sc-hv19ot-7 huoEoG')])";
                    var elementsWithClassName = driver.FindElements(By.XPath(ply));
                    List<string> playerNames = new List<string>();

                    for (int index = 0; index < elementsWithClassName.Count; index++)
                    {
                        var element = elementsWithClassName[index];
                        string playerName = ConvertNewlineToSpace(element.Text); // Clean up player name
                        playerNames.Add(playerName);
                    }

                    // Ensure the list has at least 15 players before accessing elements by index
                    if (playerNames.Count >= 15)
                    {
                        fplDetailsList.Add(new JsonFPLMembers
                        {
                            manager_Name = managerId,
                            Teams = teamName,
                            Latp = Lp,
                            Player_1 = playerNames[0],
                            Player_2 = playerNames[1],
                            Player_3 = playerNames[2],
                            Player_4 = playerNames[3],
                            Player_5 = playerNames[4],
                            Player_6 = playerNames[5],
                            Player_7 = playerNames[6],
                            Player_8 = playerNames[7],
                            Player_9 = playerNames[8],
                            Player_10 = playerNames[9],
                            Player_11 = playerNames[10],
                            Player_12 = playerNames[11],
                            Player_13 = playerNames[12],
                            Player_14 = playerNames[13],
                            Player_15 = playerNames[14],
                            SXL = countryCode // Add country code as additional info


                        });
                        LogCounter = LogCounter + 1;
                        Console.WriteLine(LogCounter + " rank " + teamName);
                    }
                    /*
                     * OpenQA.Selenium.NoSuchElementException: 'no such element: Unable to locate element: {"method":"xpath","selector":"//div[@class='sc-bdnxRM hbrYOM']/img"}
                        (Session info: chrome=129.0.6668.91); For documentation on this error, please visit: https://www.selenium.dev/documentation/webdriver/troubleshooting/errors#no-such-element-exception'

                     * */
                }
            }
            catch (Exception ex)
            {
                // General catch for any other exceptions
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }

            finally
            {
                driver.Quit(); // Close the browser
                // Example: Generate JS output and write to a file
                string jsOutput = GenerateJsArray(fplDetailsList, LeagueName, gameweek);
            }
           
        }
    }
    public static string ConvertNewlineToSpace(string input)
    {
        // Replace '\n' with a space
        return input.Replace("\n", " ");
    }


    public  static string GenerateJsArray(List<JsonFPLMembers> members , string LigaNamen, int gw)
    {
        string jsonString = JsonSerializer.Serialize(members, new JsonSerializerOptions { WriteIndented = true });
        string jsOutput = $"var s = {jsonString};";
        //int gw = 6;
        long unixTime = GetUnixTimestamp();
        string dateFolderName = GenerateDateFolderName();
        string mainPath = "C:\\Users\\ss585\\IdeaProjects\\TenForBen.github.io\\FPL\\GW\\GW" +gw +"\\DB" +
                          "/";
        string directoryPath = Path.GetDirectoryName(mainPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = directoryPath + "/" + LigaNamen + ".js";
       // string filePath = directoryPath + "/" + LigaNamen + "_" +unixTime  + ".js";
        File.WriteAllText(filePath, jsOutput);
        return jsOutput;
    }

    public static string GenerateDateFolderName()
    {
        // Get the current date
        DateTime currentDate = DateTime.Now;

        // Format the date as "DD-MM-YYYY"
        return currentDate.ToString("dd-MM-yyyy");
    }

    public class JsonFPLBasicDetails
    {
        public string Team_Name { get; set; }
        public string Manager_Id { get; set; }

    }
    public class JsonFPLMembers
    {
        public string manager_Name { get; set; }
        public string Teams { get; set; }
        public string SXL { get; set; }
        public string Latp { get; set; }
        public string Player_1 { get; set; }
        public string Player_11 { get; set; }
        public string Player_10 { get; set; }
        public string Player_12 { get; set; }
        public string Player_13 { get; set; }
        public string Player_14 { get; set; }
        public string Player_7 { get; set; }
        public string Player_2 { get; set; }
        public string Player_5 { get; set; }
        public string Player_6 { get; set; }
        public string Player_3 { get; set; }
        public string Player_4 { get; set; }
        public string Player_8 { get; set; }
        public string Player_9 { get; set; }
        public string Player_15 { get; set; }

    }

    public static Dictionary<string, string> GetLeaguePlayerNamesDictionary(long Fpl_ID, string leagueType)
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        using (var driver = new ChromeDriver(options))
        {
            driver.Navigate().GoToUrl("https://fantasy.premierleague.com/leagues/" + Fpl_ID + "/standings/" + leagueType);
            Thread.Sleep(3000);

            var elements = driver.FindElements(By.XPath("//a[contains(@href, '/entry/')]"));
            Dictionary<string, string> playerData = new Dictionary<string, string>();
            foreach (var element1 in elements)
            {
                string href = element1.GetAttribute("href");
                string pName = element1.Text;
                string id = href.Split('/')[4];
                {
                    playerData.Add(id, pName);
                }

            }
            driver.Quit();
            return playerData;
        }
    }


    // Method to get the value by key (now returning long)
    public static long GetLeagueNumber(string leagueName)
    {
        // Check if the dictionary contains the key
        if (leagueData.TryGetValue(leagueName, out long leagueNumber))
        {
            return leagueNumber;
        }
        else
        {
            Console.WriteLine($"League name '{leagueName}' not found.");
            return 90980; // Return a sentinel value to indicate the key was not found
        }
    }

}
