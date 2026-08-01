using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.Models.Enums;

namespace GymMangment.DAL.Data.Models
{
    public class Trainer : GymUsey
    {
        //HireDate = CreatedAt Of BaseEntity

        public Specialty Specialty { get; set; }

        #region Relationships
        public ICollection<Session> TrainingSessions { get; set; } = default!;
        #endregion
    }
}
