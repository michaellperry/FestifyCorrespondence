using System;
using System.Collections.Generic;
using System.Text;
using Festify.Model;
using Xamarin.Forms;

namespace Festify.Views.Detail
{
    public class DetailPage : ContentPage
    {
        private readonly Session _session;

        public DetailPage(Session session)
        {
            _session = session;
        }
    }
}
