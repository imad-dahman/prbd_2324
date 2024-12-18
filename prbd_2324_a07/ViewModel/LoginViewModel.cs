using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using prbd_2324_a07.Model;
using PRBD_Framework;

namespace prbd_2324_a07.ViewModel
{
    public class LoginViewModel : ViewModelCommon
    {
        public ICommand LoginCommand { get; set; }
        public ICommand Benoit { get; }
        public ICommand Boris { get; }
        public ICommand Xavier { get; }
        public ICommand Admin { get; }
        public ICommand Signup { get; }

        private string _Email;

        
        public string Email {
            get => _Email;
            set => SetProperty(ref _Email, value, () => Validate());
        }
        private string _password;

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }
        private void LoginAction() {
            if (Validate()) {
                var user = Context.Users.SingleOrDefault(u => u.Email == Email);
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }

        private void LogBenoit() {
            var user = Context.Users.Find(2);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
        private void LogBoris() {
            var user = Context.Users.Find(1);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }

        private void LogXavier() {
            var user = Context.Users.Find(3);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
        }
        private void LogAdmin() {
            var user = Context.Users.Find(5);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);

        }
        public LoginViewModel() : base() {
            LoginCommand = new RelayCommand(LoginAction,
                () => { return _Email != null && _password != null && !HasErrors; });
            Benoit = new RelayCommand(LogBenoit);
            Boris = new RelayCommand(LogBoris);
            Xavier = new RelayCommand(LogXavier);
            Admin = new RelayCommand(LogAdmin);
            Signup = new RelayCommand(() => NotifyColleagues(App.Messages.MSG_SIGNUP)); 
        }

        public override bool Validate() {
            ClearErrors();

            var user = Context.Users.SingleOrDefault(u => u.Email == Email);

            if (string.IsNullOrEmpty(Email))
                AddError(nameof(Email), "required");
            else if (Email.Length < 3)
                AddError(nameof(Email), "length must be >= 3");
            else if (user == null)
                AddError(nameof(Email), "does not exist");
            else {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 3)
                    AddError(nameof(Password), "length must be >= 3");
                else if (user != null && !SecretHasher.Verify(Password, user.Hashed_Password))
                    AddError(nameof(Password), "wrong password");
            }

            return !HasErrors;
        }

        protected override void OnRefreshData() {
        }
    }
}
