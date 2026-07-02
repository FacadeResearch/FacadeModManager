using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace FacadeModManager
{
    public class ReleaseData
    {
        public string? TagName { get; set; }
        public string? DownloadUrl { get; set; }
    }

    public static class GitHubHelper
    {
        private static HttpClient Client { get; set; }
        private static readonly string BaseURL = "https://api.github.com/repos/FacadeResearch/";

        static GitHubHelper()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("FacadeModManager/1.0 (Contact: contact@noia.site)");
        }

        public static async Task<ReleaseData?> GetLatestReleaseAsync(string repositoryName)
        {
            try
            {
                string jsonResponse = await Client.GetStringAsync(BaseURL + repositoryName + "/releases/latest");
                JObject releaseInfo = JObject.Parse(jsonResponse);

                string? tagName = releaseInfo["tag_name"]?.ToString();
                JArray? assets = releaseInfo["assets"] as JArray;

                string? downloadUrl = assets?.FirstOrDefault()?["browser_download_url"]?.ToString();

                return new ReleaseData { TagName = tagName, DownloadUrl = downloadUrl };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GitHub API Error: {ex.Message}");
                return null;
            }
        }

        public static async Task<List<Mod>> FetchAvailableModsAsync()
        {
            List<Mod> modsList = new List<Mod>();

            try
            {
                string url = "https://api.github.com/orgs/FacadeResearch/repos?per_page=100";
                string jsonResponse = await Client.GetStringAsync(url);

                JArray repos = JArray.Parse(jsonResponse);

                HashSet<string> excludedRepos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "FacadeModManager",
                    "FacadeModLoader",
                    "FacadePatcher",
                    "FacadeATool",
                    "ObjToFacade",
                    "FacadeDecompilation",
                    "Facade",
                    "Mods"
                };

                foreach (var repo in repos)
                {
                    string repoName = repo["name"]?.ToString() ?? "";
                    string pushed_at = repo["pushed_at"]?.ToString() ?? "";
                    bool isPrivate = repo["private"]?.Value<bool>() ?? false;

                    if (!string.IsNullOrEmpty(repoName) && !isPrivate && !excludedRepos.Contains(repoName) && !string.IsNullOrEmpty(pushed_at))
                    {
                        string description = repo["description"]?.ToString() ?? "No description available.";

                        modsList.Add(new Mod
                        {
                            Name = repoName,
                            Description = description,
                            Installed = false,
                            LastPushedDate = DateTime.Parse(pushed_at, null, System.Globalization.DateTimeStyles.AdjustToUniversal)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GitHub API Repos Error: {ex.Message}");
            }

            return modsList;
        }

        public static async Task DownloadDllAsync(string downloadUrl, string destinationPath)
        {
            using Stream stream = await Client.GetStreamAsync(downloadUrl);
            using FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(fs);
        }
    }
}
