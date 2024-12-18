using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using prbd_2324_a07.Model;
using System.Windows.Input;
using Castle.Components.DictionaryAdapter;

namespace prbd_2324_a07.ViewModel
{
    public class ParticiPantsCardViewModel : ViewModelCommon
    {
        
        private User _participant;

        public User Participant {

            get { return _participant; }

            set { SetProperty(ref _participant, value); }

        }
        private bool _showPoubelle;

        public bool ShowPoubelle {

            get { return _showPoubelle; }

            set { SetProperty(ref _showPoubelle, value); }

        }
        private bool _operationNumber;

        public bool OperationNumber {

            get { return _operationNumber; }

            set { SetProperty(ref _operationNumber, value); }

        }

        private bool _isCreator;

        public bool IsCreator {

            get { return _isCreator; }

            set { SetProperty(ref _isCreator, value); }

        }

        private int _numberOfOperations;


        public int NumberOfOperations {

            get { return _numberOfOperations; }

            set { SetProperty(ref _numberOfOperations, value); }

        }

     

        public ParticiPantsCardViewModel(Tricount tricount, User user) {

            Participant = user;

            NumberOfOperations = tricount.TricountOperations.Count();


        }
        public ParticiPantsCardViewModel() { }

    }
}
