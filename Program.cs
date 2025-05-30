﻿using OpenQA.Selenium.Chrome;
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
       long startFplID = GetLeagueNumber("PovertyLeague");
        int gw = 32;

      
        string allRunner;
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
            leagueName = "all";
        Dictionary<string, string> leaguePlayerNames;

        if (leagueName == "h2h")
        {
            leaguePlayerNames = GetLeaguePlayerNamesDictionary(startFplID, "h");
            GetFplDetailsArray(leaguePlayerNames, gw, leagueName);
        }
        else if (leagueName == "all")
        {
            foreach (var entry in leagueData.Take(6))
            {
                Console.WriteLine($"Key: {entry.Key}, Value: {entry.Value}");
                 allRunner = entry.Key;
                startFplID = GetLeagueNumber(allRunner);
                if (allRunner == "h2h")
                {
                    leaguePlayerNames = GetLeaguePlayerNamesDictionary(startFplID, "h");
                    GetFplDetailsArray(leaguePlayerNames, gw, allRunner);
                }
                else
                {
                    leaguePlayerNames = GetLeaguePlayerNamesDictionary(startFplID, "c");
                    GetFplDetailsArray(leaguePlayerNames, gw, allRunner);
                }
            }
        }
        else
        {
            leaguePlayerNames = GetLeaguePlayerNamesDictionary(startFplID, "c");
            GetFplDetailsArray(leaguePlayerNames, gw, leagueName);
        }


    }


    // Dictionary to hold the key-value pairs (with long values)
    public static Dictionary<string, long> leagueData = new Dictionary<string, long>()
    {
        
       
        { "PovertyLeague", 1089205L },
        { "R2G", 420969L },
        { "h2h", 153197L },
        { "BetssonLeague", 1173870L },
        { "KasbyLeague", 190771L },
        { "Overall", 1114702L },
        { "Arsenal", 1 }/*
        
,
        { "BlastersLeague", 1817990 },
        { "India", 120 },
        { "FPLCLLeague", 1768929 },
        { "FFMLeague", 2675 },
        { "FPLwire", 36074L },
        { "KeralaGCEK", 935873 },
        { "DisneyLeague", 822612 },
        { "FPLpod", 4109 },
        { "Overall", 314 },
        { "Arsenal", 1 },
        { "six", 153204L },
        { "FantasyShow", 56013L },
        { "Random1", 45353 },   
        { "Canal", 2257375 }*/
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
                    string managerDetails;
                    string TransferDetails;

                    // Navigate to the manager's FPL details page for the specified gameweek
                    driver.Navigate().GoToUrl($"https://fantasy.premierleague.com/entry/{managerId}/event/{gameweek}");
                    Thread.Sleep(3000); // Wait for the page to load
                    options.AddArgument("--headless");
                    // Extract country code image URL
                    string xPathCountryImg = "//div[@class='sc-bdnxRM hbrYOM']/img";
                    string xPathLatestPoints = "//div[@class='EntryEvent__PrimaryValue-sc-l17rqm-4 jsdnqB']";
                    string xPathManagerName = "//div[contains(@class, 'Entry__EntryName-sc-1kf863-0 cMEsev')]";
                    string xPathTotalTransfer = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[5]";
                    string xPathOverallPoints = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[1]";
                    string xPathOverallRank = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[2]";
                    string xPathTotalPlayers = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[3]";
                    string xPathGameWeekpoints = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[4]";
                    string xPathInTheBank = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[6]";
                    string xPathSquadValue = "(//div[contains(@class, 'Entry__DataListValue-sc-1kf863-5 jUtEoF')])[7]";
                

                    //var element1 = driver.FindElement(By.XPath(xPathCountryImg));
                    var elementLp = driver.FindElement(By.XPath(xPathLatestPoints));
                    var elementManagerName = driver.FindElement(By.XPath(xPathManagerName)); 
                    var elementTotalTransfer = driver.FindElement(By.XPath(xPathTotalTransfer));
                    var elementSquadValue = driver.FindElement(By.XPath(xPathSquadValue));
                    var elementOverallRank = driver.FindElement(By.XPath(xPathOverallRank));
                    var elementOverallPoints = driver.FindElement(By.XPath(xPathOverallPoints));
                    string Lp = elementLp.Text;
                    string ManagerName = elementManagerName.Text;
                    string TotalTransfer = elementTotalTransfer.Text;
                    string OverallRank = elementOverallRank.Text;
                    string SquadValue = elementSquadValue.Text;
                    string OverallPoints = elementOverallPoints.Text;
                    managerDetails = teamName + "( " + ManagerName + " )";
                    TransferDetails = OverallPoints + "( " + OverallRank + " )" + " TotalXfr : " + TotalTransfer;
                    //string src = element1.GetAttribute("alt");
                   // string countryCode = src; // Custom method to extract country code

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
                            manager_Name = TransferDetails,
                            
                            Teams = managerDetails,
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
                            SXL = "countryCode" // Add country code as additional info


                        });
                        LogCounter = LogCounter + 1;
                        Console.WriteLine(LogCounter + " rank " + teamName + " Latest Points | " + Lp + "| Name : " +ManagerName +" | Overall Points " + OverallPoints+ " | Overall Rank " + OverallRank+ " | Nation : " + "src");
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
        ///Users/sibin/IdeaProjects/t4b/FPL/GW/GW19/DB/Overall.js
        string mainPath = "/Users/sibin/IdeaProjects/t4b/FPL/GW/GW" +gw +"/DB" +
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
