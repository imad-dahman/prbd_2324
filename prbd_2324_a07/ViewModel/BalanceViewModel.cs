using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prbd_2324_a07.Model;
using System.Collections.ObjectModel;
using PRBD_Framework;

namespace prbd_2324_a07.ViewModel
{
    public class BalanceViewModel : ViewModelCommon
    {

        private Tricount _tricount;

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

        private ObservableCollectionFast<BalanceCardViewModel> _participants;

        public ObservableCollectionFast<BalanceCardViewModel> Participants {
            get => _participants;
            set => SetProperty(ref _participants, value);
        }

  

        public BalanceViewModel(Tricount tricount) {

            Tricount = tricount;

            var participants = Context.subscriptions.Where(s => s.TricountId == tricount.Id).OrderBy(user => user.UserIds.Full_name).ToList();
            Participants = new ObservableCollectionFast<BalanceCardViewModel>(
                    participants.Select(p =>
                        new BalanceCardViewModel() {
                            Participant = p.UserIds,
                            Amount = Tricount.GetBalanceByUser(p.UserIds.Id),
                            IsLoggedUser = App.CurrentUser.Id == p.User,
                            IsNegative = Tricount.GetBalanceByUser(p.UserIds.Id)<0,
                            IsPositive = Tricount.GetBalanceByUser(p.UserIds.Id)>=0
                        }

                    )

                );

            RaisePropertyChanged();

            Register<Tricount>(App.Messages.MSG_OPERATION_CHANGED, Tricount => {
                Refresh();
            } );

        }
        public BalanceViewModel() { }


        protected override void OnRefreshData() {
            RaisePropertyChanged();
        }



        public  void Refresh() {
            var participants = Context.subscriptions.Where(s => s.TricountId == Tricount.Id).OrderBy(user => user.UserIds.Full_name).ToList();


            Participants = new ObservableCollectionFast<BalanceCardViewModel>(
                      participants.Select(p =>
                          new BalanceCardViewModel() {
                              Participant = p.UserIds,
                              Amount = Tricount.GetBalanceByUser(p.UserIds.Id),
                              IsLoggedUser = App.CurrentUser.Id == p.User,
                              IsNegative = Tricount.GetBalanceByUser(p.UserIds.Id) < 0,
                              IsPositive = Tricount.GetBalanceByUser(p.UserIds.Id) >= 0
                          }

                      )

                  );



            OnRefreshData();
        }


    }
}
