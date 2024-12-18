using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using prbd_2324_a07.Model;
using System.Windows.Input;
using PRBD_Framework;
using System.Collections.ObjectModel;

namespace prbd_2324_a07.ViewModel
{
    public class ParticipantsViewModel : ViewModelCommon
    {

        public event Action< ObservableCollectionFast<ParticiPantsCardViewModel>> NotifyParent;


        private Tricount _tricount;

        public Tricount Tricount {
            get => _tricount;
            set => SetProperty(ref _tricount, value);
        }

       

        private ObservableCollectionFast<User> _temporaryParticipantsVM;

        public ObservableCollectionFast<User> TemporaryParticipantsVM {
            get => _temporaryParticipantsVM;
            set => SetProperty(ref _temporaryParticipantsVM, value);
        }

        private ObservableCollectionFast<ParticiPantsCardViewModel> _participants ;

        public ObservableCollectionFast<ParticiPantsCardViewModel> Participants {
            get => _participants;
            set => SetProperty(ref _participants, value);
        }

       
        public ICommand AddNewPartiticipantCommand { get; }

        public User _selectedParticipant;

        public User SelectedParticipant {
            get { return _selectedParticipant; }
            set { SetProperty(ref _selectedParticipant, value); }
        }
        public ICommand AddMySelfCommand { get; set; }
        public ICommand AddMyEverybodyCommand { get; }

        public ICommand DeletePoubelleCommand { get; }



        public bool _comBoBoxEnable;

        public bool ComBoBoxEnable  {
            get { return _comBoBoxEnable; }
            set { SetProperty(ref _comBoBoxEnable, value); }
        }


        public bool _showNumberOfOperations;
        public bool ShowNumberOfOperations {
            get { return _showNumberOfOperations; }
            set { SetProperty(ref _showNumberOfOperations, value); }
        }
        private ObservableCollection<User> _usersNoParticipants;

        public ObservableCollection<User> UsersNoParticipants {
            get => _usersNoParticipants;
            set => SetProperty(ref _usersNoParticipants, value);
        }

        public ParticipantsViewModel(Tricount tricount,bool isNew) {

            Tricount = tricount;



            //_usersNoParticipants = new ObservableCollection<User>(
            //     Context.Users
            // .OrderBy(u => u.Full_name)
            // .Where(u => u.Id != CurrentUser.Id && !Participants.Any(p => p.Participant.Id == u.Id))
            // );

            _usersNoParticipants = new ObservableCollection<User>(
                 Context.Users
             .OrderBy(u => u.Full_name)
             .Where(u => u.Id != CurrentUser.Id && (!u.OwnerTricounts.Any(t => t.Id == Tricount.Id) && !u.CreatedTricounts.Any(t => t.Id == Tricount.Id) ))
             );


            ComBoBoxEnable = UsersNoParticipants.Count() > 0;
            

            AddNewPartiticipantCommand = new RelayCommand(AddNewParticipantAction, CanAddNewParticipantAction);

            AddMyEverybodyCommand = new RelayCommand(AddMyEverybodyAction, CanAddMyEverybodyAction);

            DeletePoubelleCommand = new RelayCommand<ParticiPantsCardViewModel>(DeleteParticipantAction);

            var participants = Context.subscriptions.Where(s=>s.TricountId== tricount.Id).OrderBy(user => user.UserIds.Full_name).ToList();

            AddMySelfCommand = new RelayCommand(AddMySelfAction, CanAddMySelfAction);

            Participants = new ObservableCollectionFast<ParticiPantsCardViewModel>(
            participants.Select(p =>
                new ParticiPantsCardViewModel() {
                    Participant = p.UserIds,
                    NumberOfOperations = p.ParticipantTricount.GetNumberOfOperationsForUser(p.UserIds.Id),
                    ShowPoubelle = p.ParticipantTricount.GetNumberOfOperationsForUser(p.UserIds.Id) == 0 && p.UserIds.Id != p.ParticipantTricount.CreatorId.Id,
                    OperationNumber = p.UserIds.Id != p.ParticipantTricount.CreatorId.Id && p.ParticipantTricount.GetNumberOfOperationsForUser(p.UserIds.Id) != 0,
                    IsCreator = Tricount.Creator == p.User
                }

            )
            
        );
           
           
           
            if (isNew) 
                {  
                var CurrentUserViewModel = new ParticiPantsCardViewModel {
                Participant = CurrentUser,
                NumberOfOperations = 0,
                ShowPoubelle = false,
                OperationNumber = false,
                IsCreator = true

                };

            Participants.Add(CurrentUserViewModel);
            
            }
          

            Participants.CollectionChanged += (sender, e) => {
                NotifyParent?.Invoke(Participants);
            };


            RaisePropertyChanged();

        }

        public ParticipantsViewModel() {

            //_usersNoParticipants = new ObservableCollection<string>(Context.Users.OrderBy(u=>u.Full_name).Where(u=>u.Id != CurrentUser.Id).Select(u =>u.Full_name));

            //ComBoBoxEnable = UsersNoParticipants.Count() > 0;
            //CanAddMySelfCommand = UsersNoParticipants.Contains(CurrentUser.Full_name);
            //AddNewPartiticipantCommand = new RelayCommand(AddNewParticipantAction,CanAddNewParticipantAction);

            //AddMyEverybodyCommand = new RelayCommand(AddMyEverybodyAction);

            //DeletePoubelleCommand = new RelayCommand<ParticiPantsCardViewModel>(DeleteParticipantAction);

            //var participants = Context.subscriptions.OrderBy(user => user.UserIds.Full_name).ToList();

            //Participants = new ObservableCollectionFast<ParticiPantsCardViewModel>();
            //var CurrentUserViewModel = new ParticiPantsCardViewModel {
            //    Participant = CurrentUser,
            //    NumberOfOperations = 0,
            //    ShowPoubelle=false
            //};

            //Participants.Add(CurrentUserViewModel);
        }

        
        private void DeleteParticipantAction(ParticiPantsCardViewModel participantViewModel)
        {

            Participants.Remove(participantViewModel);
            NotifyParent?.Invoke(Participants);
            UsersNoParticipants.Add(participantViewModel.Participant);
            UsersNoParticipants = new ObservableCollection<User> (UsersNoParticipants.OrderBy(u=>u.Full_name));
            ComBoBoxEnable = UsersNoParticipants.Count > 0;
            TriParticipant();

        }

        private void TriParticipant() {
            Participants = new ObservableCollectionFast<ParticiPantsCardViewModel>(Participants.OrderBy(p => p.Participant.Full_name));
            NotifyParent?.Invoke(Participants);

        }

        private bool CanAddMySelfAction() {
            if(CurrentUser != null)
            return !Participants.Any(p=>p.Participant.Id == CurrentUser.Id);

            return false;
        }

        private void AddMySelfAction() 
        {
            var CurrentUserViewModel = new ParticiPantsCardViewModel {
                Participant = CurrentUser,
                NumberOfOperations = 0,
                ShowPoubelle = true

            };
            Participants.Add(CurrentUserViewModel);
            TriParticipant();


        }

        private bool CanAddMyEverybodyAction() {
            return UsersNoParticipants.Count != 0;
        }

        private bool CanAddNewParticipantAction() {
           return SelectedParticipant != null;
        }
        private void AddNewParticipantAction() {
            User participant = SelectedParticipant;
            if (string.IsNullOrEmpty(participant.Full_name)) {
                return;
            }

            var participantViewModel = new ParticiPantsCardViewModel() {
                Participant = participant,
                NumberOfOperations = 0 ,
                ShowPoubelle = true,
                OperationNumber=false,
                 IsCreator = false

            };

            Participants.Add(participantViewModel);
            UsersNoParticipants.Remove(participant);
            SelectedParticipant = null;
            ComBoBoxEnable = UsersNoParticipants.Count > 0;
            TriParticipant();

        }

        public void AddMyEverybodyAction() {
            ObservableCollection<User> list = new ObservableCollection<User>();
            foreach (var user in UsersNoParticipants) {
                list.Add(user);
            }

            foreach (var part in list) {
                var participantViewModel = new ParticiPantsCardViewModel {
                    Participant = part,
                    NumberOfOperations = 0,
                    ShowPoubelle = true

                };
                Participants.Add(participantViewModel);
                UsersNoParticipants.Remove(part);
            }

            ComBoBoxEnable = UsersNoParticipants.Count > 0;

            TriParticipant();

        }

        protected override void OnRefreshData() {
            RaisePropertyChanged();
        }

        public  void Refresh() {
            var participants = Context.subscriptions.Where(s => s.TricountId == Tricount.Id).OrderBy(user => user.UserIds.Full_name).ToList();

            Participants = new ObservableCollectionFast<ParticiPantsCardViewModel>(

            participants.Select(p =>
                new ParticiPantsCardViewModel() {
                    Participant = p.UserIds,
                    NumberOfOperations = p.ParticipantTricount.GetNumberOfOperationsForUser(p.UserIds.Id),
                    ShowPoubelle = p.ParticipantTricount.GetNumberOfOperationsForUser(p.UserIds.Id) == 0 && p.UserIds.Id != p.ParticipantTricount.CreatorId.Id,
                    OperationNumber = p.UserIds.Id != p.ParticipantTricount.CreatorId.Id && p.ParticipantTricount.GetNumberOfOperationsForUser(p.UserIds.Id) != 0,
                    IsCreator = Tricount.Creator == p.User

                }
            )
        );
            _usersNoParticipants = new ObservableCollection<User>(
                Context.Users
            .OrderBy(u => u.Full_name)
            .Where(u => u.Id != CurrentUser.Id && (!u.OwnerTricounts.Any(t => t.Id == Tricount.Id) && !u.CreatedTricounts.Any(t => t.Id == Tricount.Id)))
            );

            ComBoBoxEnable = UsersNoParticipants.Count() > 0;
            Participants.CollectionChanged += (sender, e) => {
                NotifyParent?.Invoke(Participants);
            };

            OnRefreshData();
        }

        

    }
}
