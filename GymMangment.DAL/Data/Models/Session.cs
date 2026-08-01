using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangment.DAL.Data.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        #region Relationships
        public Trainer Trainer { get; set; } = default!;
        public int TrainerId { get; set; } //FK

        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; } //FK

        public ICollection<Booking> MemberSessions { get; set; } = default!;
        #endregion
    }
}
