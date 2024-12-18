using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using prbd_2324_a07.Model;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PRBD_Framework;
using System.Windows.Input;

namespace prbd_2324_a07.ViewModel
{

    public class TricountsViewModel : ViewModelCommon
    {
        private ObservableCollection<TricountCardViewModel> _tricounts;

        public ObservableCollection<TricountCardViewModel> Tricounts {
            get => _tricounts;
            set => SetProperty(ref _tricounts, value);
        }
        private string _filter;
        public string Filter {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        public ICommand ClearFilter { get; set; }

        public ICommand DisplayMemberDetails { get; set; }

        public ICommand NewTricount { get; set; }
        public ICommand NewDetail { get; set; }

        public TricountsViewModel() : base() {
            OnRefreshData();

            ClearFilter = new RelayCommand(() => Filter = "");


            NewTricount = new RelayCommand(() => {
                NotifyColleagues(App.Messages.MSG_NEW_MEMBER, new Tricount());
            });
            NewDetail = new RelayCommand(() => {
                NotifyColleagues(App.Messages.MSG_NEW_DETAIL, new Tricount());
            });

            DisplayMemberDetails = new RelayCommand<TricountCardViewModel>(vm => {
                NotifyColleagues(App.Messages.MSG_DISPLAY_MEMBER, vm.Tricount);
            });

            Register<Tricount>(App.Messages.MSG_MEMBER_CHANGED, tricount => OnRefreshData());

        }

        protected override void OnRefreshData() {
            IQueryable<Tricount> tricounts = string.IsNullOrEmpty(Filter) ? Tricount.GetAll() : Tricount.GetFiltered(Filter);
            var filteredTricounts = from m in tricounts
                                  where m.Participants.Any(u => CurrentUser != null && u.Id == CurrentUser.Id || IsAdmin)
                                  select m;

            Tricounts = new ObservableCollection<TricountCardViewModel>(filteredTricounts.Select(m => new TricountCardViewModel(m)));

        }

    }
}
