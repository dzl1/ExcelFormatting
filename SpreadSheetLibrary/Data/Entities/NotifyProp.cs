using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SpreadSheetLibrary.Data.Entities
{
    public class NotifyProp : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetProperty([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}