using prbd_2324_a07.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_2324_a07.ViewModel;

public class TricountBalceViewModel : ViewModelCommon
{
    private string _title;

    public string Title {

        get { return _title; }

        set { SetProperty(ref _title, value); }

    }
    
    private Double _amount;

    public Double Amount {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }
    private User _creatorId;

    public User CreatorId {
        get => _creatorId;
        set => SetProperty(ref _creatorId, value);
    }
    private string _descritpion;

    public string Description {

        get { return _descritpion; }

        set { SetProperty(ref _descritpion, value); }

    }

    public TricountBalceViewModel() { }
    

}
