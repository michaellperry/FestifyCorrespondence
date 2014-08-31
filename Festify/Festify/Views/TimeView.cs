using Festify.Dependency;
using Festify.Model;
using Festify.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Festify.Views
{
    public class TimeView : Grid, IDisposable
    {
        private readonly TimeHeader _viewModel;

        private ChildManager _childManager = new ChildManager();

        public TimeView(TimeHeader viewModel)
        {
            _viewModel = viewModel;

            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(100, GridUnitType.Absolute) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            };

            var label = new Label();
            _childManager.Add(label.BindText(() => _viewModel.Label));
            Children.Add(label, 0, 0);

            var details = new StackLayout();
            _childManager.Add(details.Repeat(() => _viewModel.Sessions, s => new SessionView(s)));
            Children.Add(details, 1, 0);
        }

        public void Dispose()
        {
            _childManager.DisposeAll();
        }
    }
}
