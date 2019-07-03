using System;
using SimpleRtspPlayer.GUI.Models;
using SimpleRtspPlayer.GUI.ViewModels;

namespace SimpleRtspPlayer
{
    class ViewModelLocator
    {
        public volatile static int VIEW_INDEX = 0;

        private readonly Lazy<MainWindowViewModel> _mainWindowViewModelLazy =
            new Lazy<MainWindowViewModel>(CreateMainWindowViewModel);

        public MainWindowViewModel MainWindowViewModel => _mainWindowViewModelLazy.Value;

        private static MainWindowViewModel CreateMainWindowViewModel()
        {
            var model = new MainWindowModel();
            return new MainWindowViewModel(model);
        }
    }
}