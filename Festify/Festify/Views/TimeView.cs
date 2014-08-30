using Festify.Model;
using Festify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class TimeView : StackLayout
    {
        private readonly TimeHeader _viewModel;

        public TimeView(TimeHeader viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            var label = new Label();
            label.SetBinding<TimeHeader>(Label.TextProperty, h => h.Label);
            Children.Add(label);
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
