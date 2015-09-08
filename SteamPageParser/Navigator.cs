using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamPageParser
{
    public static class Navigator
    {
        public static string SteamAppUrl => @"http://store.steampowered.com/app/";

        private static int MaximumAppId => 1000000;
        private static int MininumAppId => 0;
        private static int MaximumCoresToOccupy => 5;

        public static ParallelQuery<SteamApp> GetSteamApps()
        {
            var appIdsToRun = new List<int>();

            for (var i = MininumAppId; i < MaximumAppId; i++)
            {
                appIdsToRun.Add(i);
            }

            return appIdsToRun
                .AsParallel()
                .WithDegreeOfParallelism(MaximumCoresToOccupy)
                .Select(async appId => await GetSteamApp(appId))
                .Select(sa => sa.Result)
                .Where(sa => sa != null);
        }

        public static async Task<SteamApp> GetSteamApp(int appId)
        {
            SteamApp app = null;

            await Task.Run(() =>
            {
                try
                {
                    var response = GetResponse(appId);

                    var html = GetHtml(appId, response);

                    app = Parser.ParsePage(appId, html);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            });

            return app;
        }

        private static string GetHtml(int appId, HttpWebResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK) throw new InvalidAppException(appId);

            using (var receiveStream = response.GetResponseStream())
            {
                if (receiveStream == null) throw new InvalidAppException(appId);

                var readStream = response.CharacterSet == null
                    ? new StreamReader(receiveStream)
                    : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                var data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                return data;
            }
        }

        private static HttpWebResponse GetResponse(int appId)
        {
            var request = (HttpWebRequest) WebRequest.Create($"{SteamAppUrl}{appId}/");

            return (HttpWebResponse) request.GetResponse();
        }
    }
}