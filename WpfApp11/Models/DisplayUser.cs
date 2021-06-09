using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp11.Extensions;
using WpfApp11.Model.DbModels;

namespace WpfApp11.Models
{
    public class DisplayUser
    {
        public User User { get; set; }

        public ImageSource PhotoImage { get; }

        public DisplayUser(User user)
        {
            User = user;
            PhotoImage = BitmapExtensions.GetImageSource(User.Photo);
        }
    }
}
