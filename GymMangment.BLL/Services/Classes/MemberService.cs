using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!Members.Any())
                return [];

            var MemberViewModels = Members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Photo = m.Photo,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender.ToString()
            });
            return MemberViewModels;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //Check Email
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            //Check Phone
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);
            //Email or Phone exists Return false
            if (emailExists || phoneExists)
                return false;
            // Else Create Member and return true
            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    BulidingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note
                }
            };

            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null)
                return null;
            var model = new MemberViewModel
            {
                Photo = member.Photo,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.Street}, {member.Address.BulidingNumber}, {member.Address.City}",
            };

            var activeMemberShip = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefultAsync(x => x.MemberId == MemberId && x.EndDate > DateTime.Now);
        
            if (activeMemberShip != null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMemberShip.PlanId, ct);
                model.PlanaName = activePlan?.Name;

                model.MemberShipStartDate = activeMemberShip.CreatedAt.ToString();
                model.MemberShipEndDate = activeMemberShip.EndDate.ToString();
            }

            return model;

        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int MemberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefultAsync(x=> x.MemberId == MemberId , ct: ct);

            if (record == null)
                return null;
            else
                return new HealthRecordViewModel()
                {
                    Weight = record.Weight,
                    Height = record.Height,
                    BloodType = record.BloodType,
                    Note = record.Note,
                };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return null;
            else return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                Email = member.Email,
                BuildingNumber = member.Address.BulidingNumber,
                City = member.Address.City,
                Street = member.Address.Street,
                Photo = member.Photo
            };
        }

        public async Task<bool> UpdateMemberDetailsAsync(int MemberId, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member=await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId ,ct);

            if (member == null) return false;

            var emailExist= await _unitOfWork.GetRepository<Member>().AnyAsync(x=> x.Email == model.Email && x.Id != MemberId);
            var phoneExist= await _unitOfWork.GetRepository<Member>().AnyAsync(x=> x.Phone == model.Phone && x.Id != MemberId);

            if(emailExist ||  phoneExist) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.Address.BulidingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result >0;
        }

        public async Task<bool> DeleteMemberAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if(member == null) return false;

            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == MemberId && x.Session.StartDate > DateTime.Now, ct: ct);

            if (hasFutureBookings)
                return false;
            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
