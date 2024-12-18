using Microsoft.Extensions.FileSystemGlobbing.Internal;
using prbd_2324_a07.Model;
using prbd_2324_a07.View;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace prbd_2324_a07.ViewModel;

public class SignupViewModel : ViewModelCommon
{
    public ICommand SignupCommand { get; set; }
    public ICommand CancelCommand { get; set; }
    private string _fullName;

    public string FullName {
        get => _fullName;
        set => SetProperty(ref _fullName, value, () => Validate());
    }

    private string _email;

    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    private string _password;

    public string Password {
        get => _password;
        set => SetProperty(ref _password, value, () => Validate());
    }

    private string _passwordConfirm;

    public string PasswordConfirm {
        get => _passwordConfirm;
        set => SetProperty(ref _passwordConfirm, value, () => Validate());
    }

    public SignupViewModel() : base() {
        SignupCommand = new RelayCommand(SignupAction,
            () => {
                return _fullName != null && _email != null && _password != null && _passwordConfirm != null &&
                       !HasErrors;
            });
        CancelCommand = new RelayCommand(() => NotifyColleagues(App.Messages.MSG_LOGOUT));
    }
    private void SignupAction() {
        if (Validate()) {
        User.InsertUser(FullName, Email, SecretHasher.Hash(Password));
           var user = Context.Users.SingleOrDefault(u => u.Email == Email);
            NotifyColleagues(App.Messages.MSG_LOGIN, user);
     
        }
    }
  

    public override bool Validate() {
        ClearErrors();

        const string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        var user = Context.Users.SingleOrDefault(u => u.Email == Email);
        



        bool hasError = false;

        if (!string.IsNullOrEmpty(FullName)) {
            if (FullName.Length < 3) {
                AddError(nameof(FullName), "length must be >= 3");
                hasError = true;
            }
        }

        if (!string.IsNullOrEmpty(Email)) {
            if (!Regex.IsMatch(Email, emailPattern)) {
                AddError(nameof(Email), "Invalid email format");
                hasError = true;
            } else if (user != null) {
                AddError(nameof(Email), "This email exists");
                hasError = true;
            }
        }

        if (!string.IsNullOrEmpty(Password)) {
            if (Password.Length < 3) {
                AddError(nameof(Password), "length must be >= 3");
                hasError = true;
            }
        }

        if (!string.IsNullOrEmpty(PasswordConfirm)) {
            if (Password != PasswordConfirm) {
                AddError(nameof(PasswordConfirm), "Passwords is incorrect");
                hasError = true;
            }
        }

        return !hasError;
    }


    protected override void OnRefreshData() {
    }
}
