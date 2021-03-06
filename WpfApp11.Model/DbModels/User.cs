using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp11.Model.DbModels
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        /// <summary>
        /// Изображение в формате PNG.
        /// </summary>
        public byte[] Photo { get; set; }
    }
}
