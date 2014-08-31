using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Festify.ViewModels;
using Festify.Dependency;

namespace Festify.Views
{
    public class SessionView : Grid, IDisposable
    {
        private readonly SessionHeader _viewModel;

        private ChildManager _childManager = new ChildManager();

        public SessionView(SessionHeader viewModel)
        {
            _viewModel = viewModel;

            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(80, GridUnitType.Absolute) },
                new ColumnDefinition { Width = new GridLength(240, GridUnitType.Absolute) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            };

            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) },
                new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) }
            };

            var image = new Image();
            _childManager.Add(image.BindSource(() => _viewModel.Image));
            Children.Add(image, 0, 1, 0, 2);

            var title = new Label();
            _childManager.Add(title.BindText(() => _viewModel.Title));
            Children.Add(title, 1, 3, 0, 1);

            var speaker = new Label();
            _childManager.Add(speaker.BindText(() => _viewModel.Speaker));
            Children.Add(speaker, 1, 1);

            var room = new Label();
            _childManager.Add(room.BindText(() => _viewModel.Room));
            Children.Add(room, 2, 1);
        }

        public void Dispose()
        {
            _childManager.DisposeAll();
        }
    }
}
