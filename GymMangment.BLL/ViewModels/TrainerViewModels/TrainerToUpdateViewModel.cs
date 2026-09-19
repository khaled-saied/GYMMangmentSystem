using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.DAL.Data.Models.Enums;

namespace GymMangment.BLL.ViewModels.TrainerViewModels
{
    public class TrainerToUpdateViewModel
    {
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format")]    
        public string Phone { get; set; } = default!;


        [Required(ErrorMessage = "Specialties are required")]
        [Range(1, int.MaxValue, ErrorMessage = "Building number must be greater than 0")]
        public int BuildingNumber { get; set; }


        [Required(ErrorMessage = "City is required")]
        [StringLength(100,MinimumLength =2, ErrorMessage = "City must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can only contain letters and spaces")]
        public string City { get; set; } = default!;

        [Required(ErrorMessage = "Street is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 150 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Street can only contain letters, numbers, and spaces")]
        public string Street { get; set; } = default!;

        [Required(ErrorMessage = "Specialties are required")]
        public Specialty Specialties { get; set; } = default!;

    }
}
