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
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IGenericRepository<MemberShip> _membershipRepository;
        private readonly IGenericRepository<Plan> _planRepository;

        public MemberService(IGenericRepository<Member> memberRepository
            , IGenericRepository<MemberShip> membershipRepository
            ,IGenericRepository<Plan> planRepository)
        {
            this._memberRepository = memberRepository;
            this._membershipRepository = membershipRepository;
            this._planRepository = planRepository;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var Members = await _memberRepository.GetAllAsync(ct: ct);

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
            var emailExists = await _memberRepository.AnyAsync(x => x.Email == model.Email, ct);
            //Check Phone
            var phoneExists = await _memberRepository.AnyAsync(x => x.Phone == model.Phone, ct);
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

            var result = await _memberRepository.AddAsync(member);
            return result > 0;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(MemberId, ct);
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

            var activeMemberShip = await _membershipRepository.FirstOrDefultAsync(x => x.MemberId == MemberId && x.EndDate > DateTime.Now);
        
            if (activeMemberShip != null)
            {
                var activePlan = await _planRepository.GetByIdAsync(activeMemberShip.PlanId, ct);
                model.PlanaName = activePlan?.Name;

                model.MemberShipStartDate = activeMemberShip.CreatedAt.ToString();
                model.MemberShipEndDate = activeMemberShip.EndDate.ToString();
            }

            return model;

        }
    }
}
