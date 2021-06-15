using Caliburn.Micro;
using RMDesktopUI.EventModels;
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
        private SimpleContainer _container;

        [Obsolete]
        public ShellViewModel( IEventAggregator events, SalesViewModel salesVM,
            SimpleContainer container)
        {
            _events = events;
            //_loginVM = loginVM;
            _salesVM = salesVM;
            _container = container;

            _events.Subscribe(this);

            //ActivateItemAsync(_loginVM);
            ActivateItemAsync(_container.GetInstance<LoginViewModel>());
        }

        public Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            return ActivateItemAsync(_salesVM);
            //_loginVM = (LoginViewModel)_container.GetAllInstances<LoginViewModel>();
            
        }
    }
}
