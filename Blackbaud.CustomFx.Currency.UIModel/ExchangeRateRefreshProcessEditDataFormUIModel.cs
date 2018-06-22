namespace Blackbaud.CustomFx.Currency.UIModel
{
    public partial class ExchangeRateRefreshProcessEditDataFormUIModel
    {
        private ExchangeRateRefreshProcessAddEditHandler _handler;

        private void ExchangeRateRefreshProcessEditDataFormUIModel_Loaded(object sender, Blackbaud.AppFx.UIModeling.Core.LoadedEventArgs e)
        {
            ExchangeRateRefreshProcessAddEditHandlerArgs args = new ExchangeRateRefreshProcessAddEditHandlerArgs();
            args.DateCode = this.DATECODE;
            args.HistoricalDate = this.HISTORICALDATE;

            _handler = new ExchangeRateRefreshProcessAddEditHandler(args);
            _handler.Initialize();
        }

        #region "Event handlers"

        partial void OnCreated()
        {
            this.Loaded += ExchangeRateRefreshProcessEditDataFormUIModel_Loaded;
        }

        #endregion
    }
}