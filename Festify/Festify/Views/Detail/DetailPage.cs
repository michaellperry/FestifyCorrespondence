﻿using System;
using System.Collections.Generic;
using System.Text;
using Festify.Model;
using Xamarin.Forms;
using Festify.ViewModels.Detail;
using System.Linq.Expressions;

namespace Festify.Views.Detail
{
    public class DetailPage : ContentPage
    {
        private readonly DetailViewModel _viewModel;

        public DetailPage(DetailViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            var content = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = 150 },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Padding = new Thickness(10.0)
            };

            content.Children.Add(HeaderFor(vm => vm.Name), 0, 2, 0, 1);

            Image speakerImage = new Image();
            speakerImage.SetBinding<DetailViewModel>(Image.SourceProperty, vm => vm.Image);
            content.Children.Add(speakerImage, 0, 1);

            content.Children.Add(new StackLayout
            {
                Children =
                {
                    LabelFor(vm => vm.Time),
                    LabelFor(vm => vm.Speaker),
                    LabelFor(vm => vm.Room),
                    LikeButton()
                }
            }, 1, 1);

            content.Children.Add(new StackLayout
            {
                Children =
                {
                    LabelFor(vm => vm.Description),
                    HeaderFor(vm => vm.Speaker),
                    LabelFor(vm => vm.Bio)
                }
            }, 0, 2, 2, 3);

            Content = new ScrollView { Content = content };
        }

        private static Label HeaderFor(Expression<Func<DetailViewModel, object>> property)
        {
            Label name = new Label()
            {
                LineBreakMode = LineBreakMode.WordWrap,
                Font = Font.SystemFontOfSize(NamedSize.Large, FontAttributes.Bold)
            };
            name.SetBinding<DetailViewModel>(Label.TextProperty, property);
            return name;
        }

        private static Label LabelFor(Expression<Func<DetailViewModel, object>> property)
        {
            Label name = new Label() { LineBreakMode=LineBreakMode.WordWrap };
            name.SetBinding<DetailViewModel>(Label.TextProperty, property);
            return name;
        }

        private static Button LikeButton()
        {
            Button button = new Button();
            button.SetBinding<DetailViewModel>(Button.TextProperty, vm => vm.LikeLabel);
            button.SetBinding<DetailViewModel>(Button.CommandProperty, vm => vm.Like);
            return button;
        }
    }
}
