using System.Collections.Generic;

namespace SteamPageParser
{
    public class SteamApp
    {
        private SteamApp(int appId, string html)
        {
            AppId = appId;
            Html = html;

            PackageList = new List<AppPackage>();
        }

        public int AppId { get; }
        public string Html { get; }
        public string Title { get; internal set; }

        private List<AppPackage> PackageList { get; }

        public AppPackage[] Packages => PackageList.ToArray();

        internal AppPackage AddNewPackage()
        {
            var newPackage = AppPackage.NewAppPackage(this);

            PackageList.Add(newPackage);

            return newPackage;
        }

        public static SteamApp NewSteamApp(int appId, string html) => new SteamApp(appId, html);
    }
}