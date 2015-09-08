namespace SteamPageParser
{
    public class AppPackage
    {
        private AppPackage(SteamApp associatedSteamApp)
        {
            AssociatedSteamApp = associatedSteamApp;
        }

        public string Title { get; internal set; }
        public decimal OriginalPrice { get; internal set; }
        public decimal CurrentPrice { get; internal set; }

        public decimal CurrentPricePercentage => CurrentPrice/OriginalPrice;
        public decimal DiscountPercentage => 1 - CurrentPricePercentage;

        public SteamApp AssociatedSteamApp { get; }

        public static AppPackage NewAppPackage(SteamApp associatedSteamApp) => new AppPackage(associatedSteamApp);
    }
}