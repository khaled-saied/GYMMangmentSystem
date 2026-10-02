using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymMangment.BLL.Common;
using GymMangment.BLL.Services.Attachment;
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
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper,IAttachmentService attachmentService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._attachmentService = attachmentService;
        }

        public async Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var Members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!Members.Any())
                return Result<IEnumerable<MemberViewModel>>.NotFound("No members found.");

            var MemberViewModels = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(Members);
            return Result<IEnumerable<MemberViewModel>>.Ok(MemberViewModels);
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // Check Email format/domain
            var allowedEmailDomains = new[]
                        {
                "@gmail.com",
                "@yahoo.com",
                "@outlook.com",
                "@hotmail.com"
            };

            if (!allowedEmailDomains.Any(domain =>
                model.Email.EndsWith(domain, StringComparison.OrdinalIgnoreCase)))
            {
                return Result.Validation(
                    "Email must end with @gmail.com, @yahoo.com, @outlook.com, or @hotmail.com.");
            }
            //Check Email
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            //Check Phone
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);
            //Email or Phone exists Return false
            if (emailExists || phoneExists)
                return Result.Validation("Email or Phone already exists.");

            //Upload Photo 

            var storedPhotName = await _attachmentService.UploadFileAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto");
            if (string.IsNullOrEmpty(storedPhotName.ToString()))
                return Result.Fail("Failed to upload photo.");


            // Else Create Member and return true
            var member = _mapper.Map<CreateMemberViewModel, Member>(model);
            member.Photo = storedPhotName.ToString();

            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            
            if(result > 0)
            {
                return Result.Ok();
            }
            else
            {
                //Delete the uploaded photo if member creation failed
                return Result.Fail("Failed to create member.");
            }
        }

        public async Task<Result<MemberViewModel?>> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null)
                return Result<MemberViewModel?>.NotFound("Member not found.");
            var model = _mapper.Map<Member , MemberViewModel>(member);

            var activeMemberShip = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefultAsync(x => x.MemberId == MemberId && x.EndDate > DateTime.Now);
        
            if (activeMemberShip != null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMemberShip.PlanId, ct);
                model.PlanaName = activePlan?.Name;

                model.MemberShipStartDate = activeMemberShip.CreatedAt.ToString();
                model.MemberShipEndDate = activeMemberShip.EndDate.ToString();
            }

            return Result<MemberViewModel?>.Ok(model);

        }

        public async Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int MemberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefultAsync(x=> x.MemberId == MemberId , ct: ct);

            if (record == null)
                return Result<HealthRecordViewModel?>.NotFound("Health record not found.");
            else
                return Result<HealthRecordViewModel?>.Ok(_mapper.Map<HealthRecord, HealthRecordViewModel>(record));
        }

        public async Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if (member == null) return Result<MemberToUpdateViewModel?>.NotFound("Member not found.");
            else return Result<MemberToUpdateViewModel?>.Ok(_mapper.Map<Member, MemberToUpdateViewModel>(member));
        }

        public async Task<Result> UpdateMemberDetailsAsync(int MemberId, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member=await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId ,ct);

            if (member == null) return Result.NotFound("Member not found.");
            var emailExist= await _unitOfWork.GetRepository<Member>().AnyAsync(x=> x.Email == model.Email && x.Id != MemberId);
            var phoneExist= await _unitOfWork.GetRepository<Member>().AnyAsync(x=> x.Phone == model.Phone && x.Id != MemberId);

            if(emailExist ||  phoneExist) return Result.Fail("Email or phone number already exists.");

            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result >0 ? Result.Ok() : Result.Fail("Failed to update member.");
        }

        public async Task<Result> DeleteMemberAsync(int MemberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(MemberId, ct);
            if(member == null) return Result.NotFound("Member not found.");

            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == MemberId && x.Session.StartDate > DateTime.Now, ct: ct);

            if (hasFutureBookings)
                return Result.Fail("Member has future bookings.");
            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete member.");
        }
    }
}
