using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.AnalytaicsViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class AnalyticService : IAnalyticService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;

            var upComingSessions =await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > now, ct);
            var ongoingSessions =await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= DateTime.Now && s.EndDate >= now, ct);
            var completedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate < now,ct);

            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct:ct);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct:ct);
            var activeMembers = await _unitOfWork.GetRepository<MemberShip>().CountAsync(m => m.EndDate > now, ct);

            return new AnalyticsViewModel()
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = upComingSessions,
                OngoingSessions = ongoingSessions,
                CompletedSessions = completedSessions
            };
        }
    }
}
