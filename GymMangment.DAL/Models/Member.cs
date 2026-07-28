using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangment.DAL.Models
{
    public class Member : GymUsey
    {
        public string? Photo { get; set; }

        //JoinDate  = CreatedAt Of BaseEntity
    }
}
