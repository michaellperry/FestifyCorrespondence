using Festify.Model;
using Festify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class TimeView : Grid
    {
        private readonly TimeHeader _viewModel;

        public TimeView(TimeHeader viewModel)
        {
            _viewModel = viewModel;

            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            };

            var label = new Label();
            label.BindText(() => _viewModel.Label);
            Children.Add(label, 0, 0);

            var details = new Label();
            details.Text = "Breakout Session";
            Children.Add(details, 1, 0);
        }

        public TimeHeader ViewModel
        {
            get { return _viewModel; }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            var that = obj as TimeView;
            if (that == null)
                return false;

            return Object.Equals(this._viewModel, that._viewModel);
        }

        public override int GetHashCode()
        {
            return _viewModel.GetHashCode();
        }
    }
}
