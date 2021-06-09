using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp11.Model;

namespace WpfApp11.Extensions.Models
{
    public class BoolVisibility : NotifyPropertyChanged
    {
        private readonly Visibility _falseVisibility;
        private readonly Visibility _trueVisibility;
        private bool _value;

        public bool Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Visibility));
                OnPropertyChanged(nameof(Invisibility));
            }
        }

        public Visibility Visibility => Value ? _trueVisibility : _falseVisibility;
        public Visibility Invisibility => Value ? _falseVisibility : _trueVisibility;

        public BoolVisibility(
            bool boolValue, 
            Visibility falseVisibility = Visibility.Collapsed, 
            Visibility trueVisibility = Visibility.Visible)
        {
            _falseVisibility = falseVisibility;
            _trueVisibility = trueVisibility;
            Value = boolValue;
        }

        public static implicit operator bool(BoolVisibility boolVisibility) => boolVisibility.Value;
        public static explicit operator BoolVisibility(bool boolValue) => new BoolVisibility(boolValue);
    }
}
