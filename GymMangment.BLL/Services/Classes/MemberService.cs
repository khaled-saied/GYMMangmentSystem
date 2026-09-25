using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.MemberViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!Members.Any())
                return [];

            var MemberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(Members);
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
            var member = _mapper.Map<CreateMemberViewModel, Member>(model);

            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null)
                return null;
            var model = _mapper.Map<Member , MemberViewModel>(member);

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
                return _mapper.Map<HealthRecord, HealthRecordViewModel>(record);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return null;
            else return _mapper.Map<Member, MemberToUpdateViewModel>(member);
        }

        public async Task<bool> UpdateMemberDetailsAsync(int MemberId, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member=await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId ,ct);

            if (member == null) return false;

            var emailExist= await _unitOfWork.GetRepository<Member>().AnyAsync(x=> x.Email == model.Email && x.Id != MemberId);
            var phoneExist= await _unitOfWork.GetRepository<Member>().AnyAsync(x=> x.Phone == model.Phone && x.Id != MemberId);

            if(emailExist ||  phoneExist) return false;

            _mapper.Map(model, member);
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
