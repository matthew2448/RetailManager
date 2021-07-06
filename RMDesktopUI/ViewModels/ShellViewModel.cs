using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        //private LoginViewModel _loginVM;
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        //private SimpleContainer _container;

        [Obsolete]
        public ShellViewModel( IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user, IAPIHelper apiHelper)
            //SimpleContainer container)
        {
            _events = events;
            //_loginVM = loginVM;
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;
            //_container = container;

            _events.Subscribe(this);

            //ActivateItemAsync(_loginVM);
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            NotifyOfPropertyChange(() => IsLoggedIn);
            return ActivateItemAsync(_salesVM);
            
            //_loginVM = (LoginViewModel)_container.GetAllInstances<LoginViewModel>();

        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }
        public void UserManagement()
        {
            ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
        }
        public Boolean IsLoggedIn
        {
            get
            {
                bool output = false;
                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    return true;
                }
                return output;
                //return _isErrorVisible; 
            }
        }
        public void LogOut()
        {
            _user.LogOffUser();
            _apiHelper.LogOffUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
