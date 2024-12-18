using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using prbd_2324_a07.Model;
using PRBD_Framework;

namespace prbd_2324_a07.ViewModel;

public class RepartitionViewModel : ViewModelCommon
{


    private string _participantFullName;

    public string ParticipantFullName {

        get { return _participantFullName; }

        set { SetProperty(ref _participantFullName, value); }

    }



    private bool _userChecked;

    public bool UserChecked {
        get => _userChecked;
        set {
            if (SetProperty(ref _userChecked, value)) {

                RaisePropertyChanged(nameof(UserChecked));

            }
        }
    }



    private bool _isVisible = true; 
    public bool IsVisible {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }
  

    

    private int _weight;

    public int Weight {
        get => _weight;
        set {
            if (SetProperty(ref _weight, value)) {
                RaisePropertyChanged(nameof(Weight));
                
            }


        }
    }

    private double _amountUser;

    public double AmountUser {
        get => _amountUser;
        set => SetProperty(ref _amountUser, value);
    }
  
   


    public RepartitionViewModel() {
        
    }


    //public RepartitionViewModel(ICollection<User> participants) {
       



    //}





}





