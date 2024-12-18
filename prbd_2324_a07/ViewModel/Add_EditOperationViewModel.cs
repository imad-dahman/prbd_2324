using prbd_2324_a07.Model;
using PRBD_Framework;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;


namespace prbd_2324_a07.ViewModel;

public class Add_EditOperationViewModel : DialogViewModelBase<Operation, PridContext> {
    
    private User _selectedUser;
    private RepartitionViewModel _repartitionVM;




    public ICommand Cancel { get; set; }
    public ICommand Delete { get; set; }


    public string Title {
        get => Operation.Title;
        set => SetProperty(Operation.Title, value, Operation, (m, v) => {
            m.Title = v;
            Validate();

        });
    }
    public User User { get; }
    
    public double Amount {
        get => Operation.Amount;
        set => SetProperty(Operation.Amount, value, Operation, (m, v) => { 
            m.Amount = v;
            RecalculateAmountUser();
            Validate();
        });
    }
    private bool _repartitionsModified;


    public bool RepartitionsModified {
        get => _repartitionsModified;
        set {
            if (_repartitionsModified != value) {
                _repartitionsModified = value;
                RaisePropertyChanged(nameof(RepartitionsModified));
                Validate(); 
            }
        }
    }


    private Operation _operation;
    public Operation Operation {
        get => _operation;
        set => SetProperty(ref _operation,value);
    }
    private DateTime _maxDate;

    public DateTime MaxDate {
        get =>  _maxDate;
        set => SetProperty(ref _maxDate, value);

    }
    private DateTime _minDate;

    public DateTime MinDate {
        get => _minDate;
        set => SetProperty(ref _minDate, value);

    }




    public DateTime Date {
        get => Operation.Operation_date;
        set => SetProperty(Operation.Operation_date, value, Operation, (m, v) => {
            m.Operation_date = v;
            Validate();
        });
    }




    private Tricount _tricount;
    public Tricount Tricount {
        get => _tricount;
        set => SetProperty(ref _tricount, value);
    }
   

    public ObservableCollectionFast<User> Initiator { get; } = new();

    public ObservableCollectionFast<User> Participants { get; } = new();

    public ObservableCollection<RepartitionViewModel> RepartitionsViewModel { get ; private set; }


    public RepartitionViewModel RepartitionVM {
        get => _repartitionVM;
        set {
            if (_repartitionVM != value) {
                _repartitionVM = value;
                RaisePropertyChanged(nameof(RepartitionVM));
            }
        }
    }


    public ICommand IncreaseWeightCommand { get; private set; }
    public ICommand DecreaseWeightCommand { get; private set; }


    public void IncreaseWeight(RepartitionViewModel repartitionViewModel) {
        if (repartitionViewModel.Weight == 0) {
            repartitionViewModel.Weight = 1;
            repartitionViewModel.UserChecked = true;
            repartitionViewModel.IsVisible = true;
        } else if (repartitionViewModel.Weight >= 1) {
            repartitionViewModel.Weight++;

        }
        UpdateRepartitionsModified();
        RecalculateAmountUser();

    }

    public void DecreaseWeight(RepartitionViewModel repartitionViewModel) {
        if (repartitionViewModel.Weight == 1) {
            repartitionViewModel.Weight = 0;
            repartitionViewModel.UserChecked = false;
            repartitionViewModel.IsVisible = false;
        } else if (repartitionViewModel.Weight > 1) {
            repartitionViewModel.Weight--;

        }
        UpdateRepartitionsModified();
        RecalculateAmountUser();
    }

    public User SelectedUser {
        get => Operation.InitiatorId;
        set => SetProperty(Operation.InitiatorId, value, Operation, (m, v) => {
            m.InitiatorId = v;
            Validate();
        });
    }
    public ICommand ToggleUserCheckCommand { get; private set; }
    public bool _IsNewOperation;
    public bool IsNewOperation {
        get => _IsNewOperation;
        set => SetProperty(ref _IsNewOperation, value);
    }
    public bool IsEditOperation => !_IsNewOperation;

    private bool _editoperation;
    public bool EditOperation {
        get => _editoperation;
        set => SetProperty(ref _editoperation, value);
    }
    private bool _addoperation;
    public bool AddOperation {
        get => _addoperation;
        set => SetProperty(ref _addoperation, value);
    }
    public ICommand AddOrSave { get; set; }
    public string ButtonText => IsNewOperation ? "Add" : "Save";
   

    public Add_EditOperationViewModel(Tricount tricount,Operation operation) {
        Tricount = tricount;
        Operation = operation;
        SelectedUser = Operation.InitiatorId;
        IsNewOperation = false;
        EditOperation = true;
        MaxDate = DateTime.Today;
        MinDate = tricount.Created_at;
        Initiator.RefreshFromModel(tricount.Participants.Select(s => s).OrderBy(u => u.Full_name));




        var repartitions = operation.OperationRepartitions.OrderBy(r => r.UserId.Full_name).ToList();
        var participants = tricount.Participants.OrderBy(p => p.Full_name).ToList();
        double totalWeight = operation.OperationRepartitions.Sum(r => r.Weight);

        var repartitionsViewModel = new ObservableCollection<RepartitionViewModel>(
            participants.GroupJoin(
                repartitions,
                p => p.Full_name,
                r => r.UserId.Full_name,
                (p, rGroup) => new { Participant = p, Repartitions = rGroup }
            )
            .SelectMany(
                pr => pr.Repartitions.DefaultIfEmpty(),
                (pr, r) => new RepartitionViewModel {
                    ParticipantFullName = pr.Participant.Full_name,
                    AmountUser = r != null ? (operation.Amount * r.Weight) / totalWeight : 0,
                    UserChecked = r != null,
                    Weight = r != null ? r.Weight : 0
                }
            )
        );

        RepartitionsViewModel = repartitionsViewModel;



        var repartion = RepartitionsViewModel.Count(r=> r.UserChecked);
        IncreaseWeightCommand = new RelayCommand<RepartitionViewModel>(IncreaseWeight);

        DecreaseWeightCommand = new RelayCommand<RepartitionViewModel>(DecreaseWeight);


        ToggleUserCheckCommand = new RelayCommand<RepartitionViewModel>(ToggleUserCheck);
        Cancel = new RelayCommand(CancelAction, CanCancelAction);
        Delete = new RelayCommand(DeleteOperationWithConfirmation);
        AddOrSave = new RelayCommand(SaveAction,CanSave);
        RaisePropertyChanged();







    }
    private void DeleteOperationWithConfirmation() {
        MessageBoxResult result = MessageBox.Show("You're about to delete this Operation. Do you confirm ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes) {
            DeleteOperation();
        }
    }


    public Add_EditOperationViewModel(Tricount tricount, int user,Operation operation) {
        Operation = operation;
        Tricount = tricount;
        SelectedUser = User.GetUserById(user);
        IsNewOperation = true;
        AddOperation = true;
        MaxDate = DateTime.Today;
        MinDate = tricount.Created_at;




        var participants = tricount.Participants.OrderBy(p=>p.Full_name).ToList();

        

        RepartitionsViewModel = new ObservableCollection<RepartitionViewModel>( participants.Select(p =>
                new RepartitionViewModel() {
                    ParticipantFullName = p.Full_name,
                    AmountUser = 0,
                    UserChecked = true,
                    Weight = 1
                })
        );

        var repartion = RepartitionsViewModel.Count(r => r.UserChecked);



        IncreaseWeightCommand = new RelayCommand<RepartitionViewModel>(IncreaseWeight);

        DecreaseWeightCommand = new RelayCommand<RepartitionViewModel>(DecreaseWeight);


        ToggleUserCheckCommand = new RelayCommand<RepartitionViewModel>(ToggleUserCheck);
        
        Initiator.RefreshFromModel(tricount.Participants.Select(s => s).OrderBy(u => u.Full_name));



        Cancel = new RelayCommand(CancelAction,CanCancelAction);
        AddOrSave = new RelayCommand(SaveAction, CanSave);





        RaisePropertyChanged();
    }
    public override void CancelAction() {
        ClearErrors();

        Operation.Reload();
        DialogResult = SelectedUser;

        RaisePropertyChanged();

    }

    private bool CanCancelAction() {
        return Operation != null ;
    }
    private void UpdateRepartitionsModified() {
        RepartitionsModified = RepartitionsViewModel.Any(r => r.UserChecked);
    }








    private void ToggleUserCheck(RepartitionViewModel repartitionViewModel) {
        if (repartitionViewModel.UserChecked) {
            repartitionViewModel.Weight = 1;
            repartitionViewModel.IsVisible = true;
        } else {
            repartitionViewModel.Weight = 0;
            repartitionViewModel.IsVisible = false;
        }
            UpdateRepartitionsModified();

        RaisePropertyChanged(nameof(repartitionViewModel.UserChecked));
        RecalculateAmountUser();
    }





    private bool CanSave() {
        if (IsNewOperation) {
            return Validate() && !HasErrors;
        } else 
          {
            bool operationModified = Operation.IsModified || RepartitionsModified;

            
            return operationModified && !HasErrors;
        }
    }
    public override void SaveAction() {
        if (IsNewOperation) {
            Operation.Initiator = SelectedUser.Id;
            Operation.Tricount = Tricount.Id;
            IsNewOperation = false;
            Context.Add(Operation);
            Context.SaveChanges();
            AddRepartitions();

            NotifyColleagues(App.Messages.MSG_OPERATION_CHANGED, Tricount);
            NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Tricount);

            DialogResult = SelectedUser;
        } else {
            DeleteRepartitionsByOperationId(Operation.Id);
            AddRepartitions();
            Context.SaveChanges();

            NotifyColleagues(App.Messages.MSG_OPERATION_CHANGED, Tricount);
            NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Tricount);


            DialogResult = SelectedUser;
        }

        
    }
    private void AddRepartitions() {
        bool anyParticipantChecked = RepartitionsViewModel.Any(r => r.UserChecked);

        foreach (var repartitionViewModel in RepartitionsViewModel) {
            if (repartitionViewModel.UserChecked || !anyParticipantChecked) {
                User participantUser = Context.Users.FirstOrDefault(u => u.Full_name == repartitionViewModel.ParticipantFullName);

                if (participantUser != null) {
                    Repartition newRepartition = new Repartition {
                        OperationId = Operation,
                        UserId = participantUser,
                        Weight = repartitionViewModel.Weight
                    };

                    Context.Add(newRepartition);
                }
            }
        }

        Context.SaveChanges();
    }



    private void DeleteRepartitionsByOperationId(int operationId) {
        var repartitionsToDelete = Context.Repartitions.Where(r => r.Operation == operationId);
        Context.Repartitions.RemoveRange(repartitionsToDelete);
        Context.SaveChanges();
    }
    private void DeleteOperation() {
        DeleteRepartitionsByOperationId(Operation.Id);
        var operations = Context.Operations.Where(r => r.Id == Operation.Id);
        Context.Operations.RemoveRange(operations);
        Context.SaveChanges();
        NotifyColleagues(App.Messages.MSG_OPERATION_CHANGED, Tricount);
        NotifyColleagues(App.Messages.MSG_MEMBER_CHANGED, Tricount);

        DialogResult = SelectedUser;
    }








    public void RecalculateAmountUser() {
        double totalWeight = RepartitionsViewModel.Where(r => r.UserChecked ).Sum(r => r.Weight);
        double totalAmount = Amount;

        foreach (var repartition in RepartitionsViewModel) {
            if (repartition.UserChecked) {
                if (totalWeight != 0) {
                    repartition.AmountUser = (totalAmount * repartition.Weight) / totalWeight;
                } else {
                    repartition.AmountUser = 0; 
                }
            } else {
                repartition.AmountUser = 0; 
            }
        }
    }


    public override bool Validate() {
        ClearErrors();

        bool hasError = false;



        if (string.IsNullOrWhiteSpace(Title)) {
            AddError(nameof(Title), "required");
            hasError = true;

        }
        var existTitleOperation = Context.Tricounts
            .Where(t => t.Id == Tricount.Id)
            .Any(t => t.TricountOperations.Any(o => o.Title == Title && o.Id != Operation.Id));

        if (existTitleOperation) {
            AddError(nameof(Title), "Title already exists");
            hasError = true;
        }
        if (Title?.Length < 3) {
            AddError(nameof(Title), "length must be >= 3");
            hasError = true;
        }





        if (Amount < 0.01) {
            AddError(nameof(Amount), "minimum 1 cent");
            hasError = true;
        }
        if (RepartitionsViewModel == null || RepartitionsViewModel.All(r => !r.UserChecked)) {
            AddError(nameof(RepartitionsViewModel), "You must select at least one participant!");
            hasError = true;
        }




        return !hasError;
    }
    public Add_EditOperationViewModel( ) { }
   



    protected override void OnRefreshData() {

        RaisePropertyChanged();
    }



}