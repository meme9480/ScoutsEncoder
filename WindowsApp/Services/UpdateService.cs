﻿using Octokit;
using System;
using System.Diagnostics;
using System.Reflection;

namespace WindowsApp.Services
{
    public static class UpdateService
    {
        public static async void CheckForUpdates(Action<string, string> onUpdateAvailable)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                var owner = fileVersionInfo.CompanyName;
                var repo = fileVersionInfo.ProductName;

                var client = new GitHubClient(new ProductHeaderValue(repo));
                var latest = await client.Repository.Release.GetLatest(owner, repo);
                var version = new Version(fileVersionInfo.ProductVersion);
                var latestVersion = new Version(latest.TagName.Replace("v", string.Empty));

                var isUpToDate = version.CompareTo(latestVersion) >= 0;
                if (!isUpToDate) onUpdateAvailable(latest.TagName, latest.Assets[0].BrowserDownloadUrl);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Check for Updates!");
            }
        }
    }
}
