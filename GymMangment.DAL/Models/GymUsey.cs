using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymMangment.DAL.Models
{
    public abstract class GymUsey : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }

        public Gender Gender { get; set; }
        public Address Address { get; set; }
    }

    [Owned]
    public class Address
    {
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public int BulidingNumber { get; set; } = default!;
    }

}
