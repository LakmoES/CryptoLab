using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI
{
    public class ViewShower
    {
        public static void Show(Window view, bool isModal, Action<bool?> closeAction)
        {
            if (isModal)
            {
                view.Closed += (sender, args) => closeAction(view.DialogResult);
                view.ShowDialog();
            }
            else
                view.Show();
        }
    }
}
