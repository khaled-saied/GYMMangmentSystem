using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangment.DAL.Data.Models
{
    public class MemberShip : BaseEntity
    {
        public Member Member { get; set; }
        public int MemberId { get; set; } //FK

        public Plan Plan { get; set; } 
        public int PlanId { get; set; } //FK

        //StartDate == CreatedAt Of BaseEntity
        public DateTime EndDate { get; set; }

        public string Status => EndDate > DateTime.Now ? "Active" : "Expired"; //Active, Expired
        public bool IsActive => EndDate > DateTime.Now;  
    }
}
