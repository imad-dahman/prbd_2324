using prbd_2324_a07.Model;
using prbd_2324_a07.View;
using PRBD_Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace prbd_2324_a07.ViewModel;

public class showdetailViewModel : DialogViewModelBase<Operation, PridContext>
{
    public ObservableCollectionFast<User> Initiator { get; } = new();

    private ObservableCollectionFast<Tricount> _tricountView = new();
    private ObservableCollectionFast<Operation> _operationView = new();


    public ICommand showDetail { get; set; }
    public ICommand showDetailOperation { get; set; }


    public ObservableCollectionFast<Tricount> TricountView {
        get => _tricountView;
        set => SetProperty(ref _tricountView, value);
    }
    public ObservableCollectionFast<Operation> OperationView {
        get => _operationView;
        set => SetProperty(ref _operationView, value);
    }


    private User _selectedUser;

    public User SelectedUser {
        get  { return _selectedUser; }
       
        set => SetProperty(ref _selectedUser, value,  () => {
           OnRefreshData();
        });
    }
    private Tricount _selectedTricount ;
    public Tricount SelectedTricount {
        get { return _selectedTricount; }
        set => SetProperty(ref _selectedTricount, value, () => {
            OnRefreshData();
        });
    }


    public showdetailViewModel() {

        Initiator.RefreshFromModel(Context.Users.Select(s => s).OrderBy(s => s.Full_name));
        //showDetail = new RelayCommand(showdetail, canceldetal);
        //showDetailOperation = new RelayCommand(showdetail2, canceldetal2);
        



    }

    private void showdetail() {
        TricountView.RefreshFromModel(Context.Tricounts.Where(t => t.Participants.Any(p => p.Id == SelectedUser.Id)));

        SelectedUser = null;
        OnRefreshData();

    }
    private void showdetail2() {
        //OperationView.RefreshFromModel(Context.Operations.Where(o => o.Tricount == SelectedTricount.Id));
        OperationView.RefreshFromModel(Context.Operations.Where(o => o.Initiator == SelectedUser.Id));



        SelectedTricount = null;
        OnRefreshData();

    }
    private bool canceldetal() {
        return SelectedUser != null;
    }
    private bool canceldetal2() {
        return SelectedTricount != null;
    }


    protected override void OnRefreshData() {
        //RaisePropertyChanged(nameof(TricountView));
        //RaisePropertyChanged(nameof(OperationView));
        RaisePropertyChanged();
       if (SelectedUser != null) {
           TricountView.RefreshFromModel(Context.Tricounts.Where(t=>t.Participants.All(p=>p.Id!=SelectedUser.Id)));
            OperationView.RefreshFromModel(Context.Operations.Where(o => o.Initiator == SelectedUser.Id));

        }
        /* if(SelectedTricount != null) {
              OperationView.RefreshFromModel(Context.Operations.Where(o=>o.Tricount == SelectedTricount.Id));

          }*/






    }

}
