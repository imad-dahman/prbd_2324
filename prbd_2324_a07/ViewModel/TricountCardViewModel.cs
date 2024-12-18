using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using prbd_2324_a07.Model;
using System.Windows.Media;
using System.Windows;

namespace prbd_2324_a07.ViewModel
{
    public class TricountCardViewModel : ViewModelCommon
    {
        private readonly Tricount _tricount;

        public Tricount Tricount {
            get => _tricount;
            private init => SetProperty(ref _tricount, value);
        }

        public string Title => Tricount.Title;
        public string Description =>string.IsNullOrEmpty(Tricount.Description) ? " No Description" : Tricount.Description;

        public string CreatedOn => "On " + Tricount.Created_at.ToShortDateString();

        public bool isExistOperation => Tricount.TricountOperations.Count() > 0;
        public Brush BrushColor => GetColor();
        public String Creator => Tricount.CreatorId.Full_name;
        public String TotalExpenses => Tricount.GetTotalExpenses().ToString() + " €";
        public String MyExpenses => Tricount.GetMyExpenses().ToString() + " €";
        public String MyBalance => Tricount.GetMyBalance().ToString() + " €";

        public String LastOperation => "Last Operation on " + Tricount?.getLastOperation()?.Operation_date.ToShortDateString();
        public Brush MyBalanceColor => GetBalanceColor();
        public string NumberOFriends => Tricount.Participants.Count > 1 ? "with " + (Tricount.Participants.Count-1).ToString() + (Tricount.Participants.Count-1 == 1 ? " FRIEND" : " FRIENDS") : "no friend";

        public String NumberOfOperations => Tricount.TricountOperations.Count > 0 ? Tricount.TricountOperations.Count.ToString() + " Operations" : "No operation";

        public TricountCardViewModel(Tricount tricount) {
            Tricount = tricount;
        }

        private Brush GetColor() {
            if (Tricount.GetMyBalance() > 0)
                return Brushes.LightGreen;
            else if (Tricount.GetMyBalance() < 0)
                return new SolidColorBrush(Color.FromRgb(255, 187, 128)); // Light orange
            else
                return Brushes.LightGray;
        }
        private Brush GetBalanceColor() {
            if (Tricount.GetMyBalance() > 0)
                return Brushes.Green;
            else if (Tricount.GetMyBalance() < 0)
                return Brushes.Red;
            else
                return Brushes.Black;
        }


    }
}
 