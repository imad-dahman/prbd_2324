using prbd_2324_a07.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a07.ViewModel
{
    public class BalanceCardViewModel : ViewModelCommon
    {
        private User _participant;

        public User Participant {

            get { return _participant; }

            set { SetProperty(ref _participant, value); }

        }
        private double _amount;

        public  double Amount {
            get => _amount; 
            set=> SetProperty(ref _amount, value);
        }

        private bool _isLoggedUser;

        public bool IsLoggedUser {

            get { return _isLoggedUser; }

            set { SetProperty(ref _isLoggedUser, value); }

        }

        private bool _isNegative;

        public bool IsNegative {

            get { return _isNegative; }

            set { SetProperty(ref _isNegative, value); }

        }

        private bool _isPositive;

        public bool IsPositive {

            get { return _isPositive; }

            set { SetProperty(ref _isPositive, value); }

        }

        public BalanceCardViewModel() { }
    }
}
