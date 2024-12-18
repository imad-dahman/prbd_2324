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

    public class TricountDetailViewModel : ViewModelCommon
    {

        public ICommand NewOperation { get;  set; }
        public ICommand Show { get; set; }



        private Tricount _tricount;
        public OperationViewModel ListOperation { get;  } = new();
        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }

        public ICommand Delete { get; }

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value, () => { 
                AddNewTricount.Tricount = value;
                ListOperation.Tricount = value;

                });

        }
        public bool IsAdmine {
            get => CurrentUser?.Role == Role.Administrator;
        }



        public ICommand Edit { get; set; }
        private bool _editMode ;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }
        private bool _newOrEditMode;
        public bool NewOrEditMode {
            get => _newOrEditMode;
            set => SetProperty(ref _newOrEditMode, value);
        }
        private bool _isNew;
        public bool IsNew {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        private bool _isExisting;
        public bool IsExisting {
            get => _isExisting;
            set => SetProperty(ref _isExisting, value);
        }
        private bool _isExisting2;
        public bool IsExisting2 {
            get => _isExisting2;
            set => SetProperty(ref _isExisting2, value);
        }

        public string Title {
            get => IsNew? "<New Tricount>" : Tricount.Title;
            set => SetProperty(Tricount.Title, value, Tricount, (m, v) => {
                m.Title = v;
                _isNew = false;
                NotifyColleagues(App.Messages.MSG_PSEUDO_CHANGED, Tricount);

            }
            );
        }

        private bool _isChangedListParticipant = false;
        public bool IsChangedListParticipant {
            get => _isChangedListParticipant;
            set => SetProperty(ref _isChangedListParticipant, value);
        }

        public AddTricountViewModel AddNewTricount { get; set; } = new();

        public ParticipantsViewModel ParticipantsVm { get; set; }
        public BalanceViewModel BalanceVm { get; set; }

        public string Description {
            get => IsNew ? "<No DesCription>" : string.IsNullOrEmpty(Tricount.Description) ? " No Description" : Tricount.Description;
            set => SetProperty(Tricount.Description, value, Tricount, (m, v) => m.Description = v);
        }

        public string CreatedOn {
            get => IsNew? DateTime.Now.ToShortDateString() : "On " + (Tricount?.Created_at.ToShortDateString() ?? "");
            set => SetProperty(Tricount.Created_at.ToShortDateString(), value, Tricount, (m, v) => m.Created_at =DateTime.Parse(v));
        }

        public string Creator {
            get => IsNew? CurrentUser?.Full_name : Tricount?.CreatorId?.Full_name;
            set => SetProperty(Tricount.CreatorId.Full_name, value, Tricount, (m, v) => m.CreatorId.Full_name = v);
        }



        public TricountDetailViewModel(Tricount tricount, bool isNew) {
            Tricount = tricount;
            IsNew = isNew;
            NewOrEditMode = EditMode || IsNew;
            IsExisting= !IsNew && !EditMode;
            IsExisting2 = IsExisting && CurrentUser.Id == Tricount.Creator || CurrentUser.Role == Role.Administrator && IsExisting;
            ParticipantsVm = new ParticipantsViewModel(tricount,isNew);

            AddNewTricount.NotifyParent += Tricount => {
                this.Tricount = Tricount;
                IsNew = false;
                AddNewTricount.IsNew = false;
                IsExisting2 = !IsExisting2;
                BalanceVm.Refresh();
                Console.WriteLine("Parent has been notified with message: " + Tricount.Title);
                NotifyColleagues(App.Messages.MSG_PSEUDO_CHANGED, Tricount);
                RaisePropertyChanged();
            };
            
               ParticipantsVm.NotifyParent += Participants => {
                AddNewTricount.Participant = Participants;
                IsChangedListParticipant = true;
                AddNewTricount.IsChangedListParticipant = true;
                   Console.WriteLine("isList" + IsChangedListParticipant);
            };

            BalanceVm = new BalanceViewModel(Tricount);
           

            RaisePropertyChanged();

            Delete = new RelayCommand(DeleteAction);
            Save = new RelayCommand(SaveAction, CanSaveAction);
            Cancel = new RelayCommand(CancelAction, CanCancelAction);
            Edit = new RelayCommand(() => {
                EditMode = true;
                IsExisting = false;
                AddNewTricount.IsNew = false;
                NewOrEditMode = EditMode || IsNew;
                IsExisting2 = !IsExisting2;
                ParticipantsVm.Refresh();
                AddNewTricount.Refresh();
            }
            );

            NewOperation = new RelayCommand(() => {
                App.ShowDialog<Add_EditOperationViewModel, Operation, PridContext>(Tricount, CurrentUser.Id,new Operation()) ;
                UpdateBalances();
            });
            Show= new RelayCommand(() => {
                App.ShowDialog<showdetailViewModel, Operation, PridContext>();
                UpdateBalances();
            });
            



            Register<Tricount>(App.Messages.MSG_OPERATION_CHANGED, tricount => {
                if (tricount == Tricount) {
                    BalanceVm.Refresh();
                    ListOperation.Refresh();
                }
            });


        }
        public TricountDetailViewModel() { }
        
        private void DeleteAction() 
        {
            AddNewTricount.Delete();

            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);

            NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Tricount);
        }
        public override void CancelAction() {
            AddNewTricount.Refresh();
            RaiseErrors();
            if (IsNew) {
                IsNew = false;
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, Tricount);
            } else {
                IsExisting = !IsExisting;
                NewOrEditMode = !NewOrEditMode;
                Tricount.Reload();
                //AddNewTricount.Tricount.Reload();
                //ParticipantsVm.Tricount.Reload();
                AddNewTricount.Refresh();
                ParticipantsVm.Refresh();
                BalanceVm.Refresh();
                RaisePropertyChanged();
            }
            IsExisting2 = !IsExisting2;
            IsChangedListParticipant = false;
          }

        private bool CanCancelAction() {
            return Tricount != null || IsNew;
        }

        private bool CanSaveAction() {
            if (IsNew)
                return Tricount.Validate() && !HasErrors;
            return Tricount != null && Tricount.IsModified && Tricount.Validate() && !HasErrors  || IsChangedListParticipant && Tricount.Validate() ;
        }

        public override void SaveAction() { 
            AddNewTricount.SaveAction();
            NewOrEditMode = !NewOrEditMode;
            IsExisting=!IsExisting;
        }


        public override void Dispose() {
            AddNewTricount.Dispose();
            ParticipantsVm.Dispose();
            ListOperation.Dispose();
            BalanceVm.Dispose();
            base.Dispose();
        }


       
        protected override void OnRefreshData() {
            RaisePropertyChanged();
        }
        private void UpdateBalances() {
            BalanceVm.Refresh();
            ListOperation.Refresh();
        }



    }
}
