using prbd_2324_a07.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a07.ViewModel;

public class OperationCardViewModel : ViewModelCommon
{
   private readonly Operation _operation;
    public Operation Operation {
        get => _operation;
        private init=> SetProperty(ref _operation, value);
    }
    


    public string Title => Operation.Title;

    public String Initiator => Operation.InitiatorId.Full_name;
    public double Amount => Operation.Amount;
    public DateTime Date => Operation.Operation_date;

    
  

    public OperationCardViewModel(Operation operation) {
        Operation = operation;
    }



}
