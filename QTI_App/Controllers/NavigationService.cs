using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTI_App.Controllers
{
    public class NavigationService
    {
        private Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }
        public void NavigateTo<T>() where T : Page, new()
        {
            // Navigeer naar de opgegeven paginaklasse zonder parameters.
            _frame.Navigate(typeof(T));
        }

        public void NavigateToWithParameter<T>(object parameter) where T : Page, new()
        {
            // Navigeer naar de opgegeven paginaklasse met het meegegeven parameterobject.
            _frame.Navigate(typeof(T), parameter);
        }

        public void GoBack()
        {
            // Controleer of er een vorige pagina beschikbaar is en ga terug als dat het geval is.
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }
    }
}
