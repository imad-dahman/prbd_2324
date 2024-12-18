using PRBD_Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prbd_2324_a07.Model;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Windows.Input;
using System.Windows;

namespace prbd_2324_a07.ViewModel
{
    public class AddTricountViewModel : ViewModelCommon
    {
        public event Action<Tricount> NotifyParent;

        private Tricount _tricount;
        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value,OnRefreshData);
        }
        private bool _isNew;
        public bool IsNew {
           
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        private bool _isChangedListParticipant = false;
        public bool IsChangedListParticipant {
            get => _isChangedListParticipant;
            set => SetProperty(ref _isChangedListParticipant, value);
        }

        public AddTricountViewModel() {
            //Save = new RelayCommand(() => {
            //    NotifyParent.Invoke(Tricount);
            //});
            IsNew=true;

          
          
            RaisePropertyChanged();
          
        }
        public AddTricountViewModel(Tricount tricount) {
            //Save = new RelayCommand(() => {
            //    NotifyParent.Invoke(Tricount);
            //});
            Tricount = tricount;
            IsNew = true;
            RaisePropertyChanged();
        }

        private ObservableCollectionFast<ParticiPantsCardViewModel> _participant;
        public ObservableCollectionFast<ParticiPantsCardViewModel> Participant {
            get => _participant;
            set => SetProperty(ref _participant, value, OnRefreshData);
        }

        public string Title {
            get =>  Tricount.Title;
            set => SetProperty(Tricount.Title, value, Tricount, (m, v) => {
                m.Title = v;
                Validate();
            });
        }

        public string Description {
            get => Tricount.Description;
            set => SetProperty(Tricount.Description, value, Tricount, (m, v) =>
            
            {
                m.Description = v;
                Validate();
            }
            );
        }

        //public DateTime MinDate {
        //    get => Tricount.Created_at;
        //    set => SetProperty(Tricount.Created_at, value, Tricount, (m, v) => {
        //        m.Created_at = v;
        //    });
        //}
        private DateTime _maxDate ;

        public DateTime MaxDate {
            get => IsNew ? DateTime.Today : _maxDate ;
            set => SetProperty(ref _maxDate, value);

        }

        public DateTime CreatedOn {
            get =>  Tricount.Created_at;
            set => SetProperty(Tricount.Created_at, value, Tricount, (m, v) => {
                m.Created_at = v;
            });
        }
        public override bool Validate() {
            ClearErrors();
            // On délègue la validation à l'entité Member
            Tricount.Validate();
            // On ajoute les erreurs détectées par l'entité Member à notre propre liste d'erreurs
            AddErrors(Tricount.Errors);
            return !HasErrors;
        }
        public void Delete() 
        {
            MessageBoxResult result = MessageBox.Show("You're about to delete this Tricount. Do you confirm ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                Tricount.Delete();

            }
        }
       private bool CanSaveAction() {
          if (IsNew)
        return Tricount.Validate() && !HasErrors;
        return Tricount != null && Tricount.IsModified && !HasErrors;
}

        public override void SaveAction() {
           
            if (IsNew) 
                {
                Tricount.Creator = CurrentUser.Id;
                Console.WriteLine(Tricount);
                Context.Add(Tricount);
                Context.SaveChanges();
                AddParticipantsByTricount();
                RaisePropertyChanged();
                NotifyParent.Invoke(Tricount);
                NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Tricount);
                Console.WriteLine(Tricount.Participants.Count + " nbr Particiapant passé au vm" + Tricount.Title);


            } else {
                if(IsChangedListParticipant)
                { 
                 RemoveParticipantByTricount(Tricount.Id);
                AddParticipantsByTricount();
                Context.SaveChanges();
                RaisePropertyChanged();
                }
                NotifyParent.Invoke(Tricount);
                RaisePropertyChanged();
                Context.SaveChanges();
                NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Tricount);
                Console.WriteLine(Tricount.Participants.Count + " nbr Particiapant passé au vm" + Tricount.Title);

            }

        }

        private void RemoveParticipantByTricount(int tricountId) {
            var participants = Context.subscriptions.Where(s => s.TricountId == tricountId);
            Context.subscriptions.RemoveRange(participants);
            Context.SaveChanges();
        }

        private void AddParticipantsByTricount() {
            if (Participant is null) {
                Console.WriteLine("ici participant is null");
                Subscriptions subscriptions = new Subscriptions(CurrentUser.Id, Tricount.Id);
                Context.Add(subscriptions);
                Context.SaveChanges();
            }

            if (Participant != null) {
                Console.WriteLine("ici condition");               
                foreach (ParticiPantsCardViewModel participant in Participant) {
                    Subscriptions subscriptions1 = new Subscriptions(participant.Participant.Id, Tricount.Id);
                    Context.Add(subscriptions1);
                    Context.SaveChanges();

                }
            }
        }


        protected override void OnRefreshData() {
           
            RaisePropertyChanged();
        }

        public void Refresh() {
            ClearErrors();
            if (!IsNew) {
                MaxDate = Tricount?.getRecentOperation()?.Operation_date ?? DateTime.Today;
            }
            OnRefreshData();
        }
    }
}
