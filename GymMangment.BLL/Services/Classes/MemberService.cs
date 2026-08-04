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

        public MemberService(IGenericRepository<Member> memberRepository)
        {
            this._memberRepository = memberRepository;
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
    }
}
