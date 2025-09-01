using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using U4DosRandomizer;
using CommunityToolkit.Maui.Extensions;

namespace U4DosRandomizer.UI
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel(FolderPicker.Default, new PopupService());
        }

    }
    public interface IPopupService
    {
        void ShowPopup(String popupMessage);
        void ShowBusy();
        void StopBusy();
    }

    public class PopupService : IPopupService
    {
        public void ShowBusy()
        {
            Page page = Application.Current?.MainPage ?? throw new NullReferenceException();
            page.ShowPopupAsync(new ActivityIndicator { IsRunning = true}, new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false,
                Shape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(20, 20, 20, 20),
                    StrokeThickness = 2,
                    Stroke = Colors.LightGray
                }
            });
        }

        public void ShowPopup(String popupMessage)
        {
            Page page = Application.Current?.MainPage ?? throw new NullReferenceException();
            page.ShowPopupAsync(new Label
            {
                Text = popupMessage,
                BackgroundColor = Colors.LightGray
            }, new PopupOptions
            {
                Shape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(20, 20, 20, 20),
                    StrokeThickness = 2,
                    Stroke = Colors.LightGray
                }
            });
        }

        public void StopBusy()
        {
            Page page = Application.Current?.MainPage ?? throw new NullReferenceException();
            page.ClosePopupAsync();
        }
    }

}
