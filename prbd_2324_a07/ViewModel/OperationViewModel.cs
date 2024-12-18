using Microsoft.EntityFrameworkCore.Metadata;
using prbd_2324_a07.Model;
using prbd_2324_a07.View;
using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace prbd_2324_a07.ViewModel;

public class OperationViewModel : ViewModelCommon
{
    private ObservableCollection<OperationCardViewModel> operations;

    public ObservableCollection<OperationCardViewModel> Operations {
        get => operations;
        set 
        {
            SetProperty(ref operations, value);
            RaisePropertyChanged(nameof(IsNoOperationVisible));


        }
    }
    private bool isNoOperationVisible;
    public bool IsNoOperationVisible {
        get => isNoOperationVisible;
        set {
            SetProperty(ref isNoOperationVisible, value);
            RaisePropertyChanged(); 
        }
    }

    private Tricount _tricount;
    public Tricount Tricount {
        get => _tricount;
        set {
            SetProperty(ref _tricount, value);
            OnRefreshData();
        }
    }
   



    public ICommand ShowOperationDetailsCommand { get; }






    public OperationViewModel() : base() {
        Operations = new ObservableCollection<OperationCardViewModel>();

        ShowOperationDetailsCommand = new RelayCommand<OperationCardViewModel>(vm => {
            App.ShowDialog<Add_EditOperationViewModel, Operation, PridContext>(Tricount,vm.Operation);
        });
        



        OnRefreshData();


        Register<Tricount>(App.Messages.MSG_OPERATION_CHANGED, Tricount => OnRefreshData());

    }



    protected override void OnRefreshData() {

        if (Tricount == null) {
            return;
        }

        IQueryable<Operation> operationsQuery = Operation.GetAll();

        var filteredOperations = from o in operationsQuery
                                 where o.TricountId.TricountOperations.Any(o => o.Tricount == Tricount.Id)
                                 select o;

        Operations = new ObservableCollection<OperationCardViewModel>(filteredOperations.Select(o => new OperationCardViewModel(o)));
        IsNoOperationVisible = Operations.Count == 0;


    }
    public void Refresh() {
        OnRefreshData();
    }
}
