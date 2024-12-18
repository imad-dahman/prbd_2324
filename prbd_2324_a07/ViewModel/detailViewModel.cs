using Azure;
using prbd_2324_a07.Model;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a07.ViewModel
{
    public class detailViewModel : ViewModelCommon
    {
        public ObservableCollectionFast<User> Initiator { get; } = new();
        private ObservableCollectionFast<Tricount> _tricountView = new();
        public ObservableCollectionFast<Tricount> TricountView {
            get => _tricountView;
            set => SetProperty(ref _tricountView, value);
        }

        private ObservableCollectionFast<TricountBalceViewModel> _balance = new();

        public ObservableCollectionFast<TricountBalceViewModel> Balance {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }
        private User _selectedUser;

        public User SelectedUser {
            get { return _selectedUser; }

            set => SetProperty(ref _selectedUser, value, () => {
                OnRefreshData();
            });
        }

        public detailViewModel() {
            Initiator.RefreshFromModel(Context.Users.Select(s => s).OrderBy(u => u.Full_name));
            OnRefreshData();
        }
        
        protected override void OnRefreshData() {

             RaisePropertyChanged();
            if (SelectedUser != null) {
                //TricountView.RefreshFromModel(Context.Tricounts.Where(t => t.Participants.Any(p => p.Id == SelectedUser.Id)));

                var tricounts = Context.Tricounts.Where(t => t.Participants.Any(p => p.Id == SelectedUser.Id));
                Balance = new ObservableCollectionFast<TricountBalceViewModel>(
                        tricounts.Select(t =>
                            new TricountBalceViewModel() {
                                Title = t.Title,
                                Description = t.Description,
                                Amount = t.GetBalanceByUser(SelectedUser.Id),
                                CreatorId = t.CreatorId,

                            }

                        )

                    );
            }




        }
    }
}
