using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using GymMangment.BLL.ViewModels.AnalytaicsViewModels;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface IAnalyticService
    {
        Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct = default);
    }
}
