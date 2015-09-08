using System;
using System.Linq;
using HtmlAgilityPack;

namespace SteamPageParser
{
    public static class Parser
    {
        private static string AppTitleClass => "apphub_AppName";
        private static string PackageClass => "game_area_purchase_game_wrapper";
        private static string PackagePriceXPath => "game_purchase_price price";
        private static string PackageOriginalPriceXPath => "discount_original_price";
        private static string PackageDiscountPriceXPath => "discount_final_price";
        private static string PackageTitle => "h1";
        private static string ThousandsSeparator => ",";
        private static string CurrencySymbol => "$";

        public static SteamApp ParsePage(int appId, string html)
        {
            if (string.IsNullOrWhiteSpace(html)) throw new ArgumentNullException(nameof(html));

            try
            {
                var app = SteamApp.NewSteamApp(appId, html);

                var htmlDocument = new HtmlDocument();

                var htmlCleaned = html.Replace("\"", "'");

                htmlDocument.LoadHtml(htmlCleaned);

                var documentNode = htmlDocument.DocumentNode;

                var titleNode = documentNode.SelectSingleNode($"//div[@class='{AppTitleClass}']");

                app.Title = titleNode.InnerHtml.Trim();

                var packageNodes = documentNode.SelectNodes($"//div[@class='{PackageClass}']").ToArray();

                foreach (var packageNode in packageNodes)
                {
                    AddPackage(app, packageNode);
                }

                return app;
            }
            catch (Exception)
            {
                throw new InvalidAppException(appId);
            }
        }

        private static void AddPackage(SteamApp app, HtmlNode packageNode)
        {
            var package = app.AddNewPackage();

            var packageTitleNode = packageNode.SelectSingleNode($"//{PackageTitle}");

            package.Title = packageTitleNode.InnerHtml.Replace("Buy ", "").Trim();

            var priceNodes = packageNode.SelectNodes($"//div[@class='{PackagePriceXPath}']");

            if (priceNodes != null)
            {
                var priceNode = priceNodes[0];

                package.CurrentPrice = ParseNodeWithCurrencyToDecimal(priceNode);

                package.OriginalPrice = package.CurrentPrice;
            }
            else
            {
                var originalPriceNode = packageNode.SelectSingleNode($"//div[@class='{PackageOriginalPriceXPath}']");

                package.OriginalPrice = ParseNodeWithCurrencyToDecimal(originalPriceNode);

                var discountPriceNode = packageNode.SelectSingleNode($"//div[@class='{PackageDiscountPriceXPath}']");

                package.CurrentPrice = ParseNodeWithCurrencyToDecimal(discountPriceNode);
            }
        }

        private static decimal ParseNodeWithCurrencyToDecimal(HtmlNode node)
        {
            var stringValue = node.InnerHtml.Replace(CurrencySymbol, "").Replace(ThousandsSeparator, "");

            return decimal.Parse(stringValue);
        }
    }
}