using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrivingEasy
{
    public partial class User
    {
        
        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {Patronymic}";
            }
        }

    }
}
