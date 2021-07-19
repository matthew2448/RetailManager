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
        
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        //private SimpleContainer _container;

        [Obsolete]
        public ShellViewModel( IEventAggregator events, ILoggedInUserModel user, IAPIHelper apiHelper)
            //SimpleContainer container)
        {
            _events = events;
            //_loginVM = loginVM;
            
            _user = user;
            _apiHelper = apiHelper;
            //_container = container;

            _events.Subscribe(this);

            //ActivateItemAsync(_loginVM);
            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }
        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken());
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
        public async Task LogOut()
        {
            _user.LogOffUser();
            _apiHelper.LogOffUser();
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
