using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Models.Enums;

namespace GymMangment.DAL.Models
{
    public class Trainer : GymUsey
    {
        //HireDate = CreatedAt Of BaseEntity

        public Specialty Specialty { get; set; }
    }
}
