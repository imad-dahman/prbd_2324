using prbd_2324_a07.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_2324_a07.ViewModel
{
    public abstract class ViewModelCommon : ViewModelBase<User, PridContext>
    {
        public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Administrator;

        public static bool IsNotAdmin => !IsAdmin;

    }
}
