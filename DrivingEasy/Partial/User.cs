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
        //[JsonIgnore]
        //public virtual Role Role
        //{
        //    get
        //    {
        //        return DBConnection.Roles.FirstOrDefault(x => x.Id == RoleId);
        //    }
        //    set
        //    {
        //        RoleId = value.Id;
        //    }
        //}
        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {Patronymic}";
            }
        }

    }
}
