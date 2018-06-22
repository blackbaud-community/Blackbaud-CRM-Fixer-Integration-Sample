using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbaud.AppFx.UIModeling.Core;

namespace Blackbaud.CustomFx.Currency.UIModel
{
    internal class ExchangeRateRefreshProcessAddEditHandlerArgs
    {
        public ValueListField DateCode;
        public DateField HistoricalDate;
    }

    internal class ExchangeRateRefreshProcessAddEditHandler
    {
        private ValueListField _dateCode;
        private DateField _historicalDate;

        public ExchangeRateRefreshProcessAddEditHandler(ExchangeRateRefreshProcessAddEditHandlerArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (args.DateCode == null)
            {
                throw new ArgumentException("You must provide a date code field.");
            }

            if (args.HistoricalDate == null)
            {
                throw new ArgumentException("You must provide a historical date field.");
            }

            _dateCode = args.DateCode;
            _historicalDate = args.HistoricalDate;

            _dateCode.ValueChanged += _dateCode_ValueChanged;
        }

        public void Initialize()
        {
            SetFieldState();
        }

        private void _dateCode_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SetFieldState();
        }

        private void SetFieldState()
        {
            switch ((int)_dateCode.ValueObject)
            {
                case (int)Common.DateCode.Latest:
                    // Clear and hide field
                    _historicalDate.Value = DateTime.MinValue;
                    _historicalDate.Visible = false;
                    break;
                case (int)Common.DateCode.Historical:
                    // Show field
                    _historicalDate.Visible = true;
                    break;
            }
        }
    }
}
