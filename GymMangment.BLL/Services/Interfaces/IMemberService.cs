using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using GymMangment.BLL.ViewModels.MemberViewModels;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<Result<IEnumerable<MemberViewModel>>> GetAllMembersAsync(CancellationToken ct=default);
        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct=default);
        Task<Result<MemberViewModel?>> GetMemberDetailsByIdAsync(int MemberId, CancellationToken ct = default);
        Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync(int MemberId, CancellationToken ct=default);
        Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int MemberId, CancellationToken ct=default);
        Task<Result> UpdateMemberDetailsAsync(int MemberId,MemberToUpdateViewModel model, CancellationToken ct=default);
        Task<Result> DeleteMemberAsync(int MemberId, CancellationToken ct=default);

    }
}
