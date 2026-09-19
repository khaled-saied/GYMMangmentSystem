using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.ViewModels.TrainerViewModels;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct= default);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct= default);
        Task<TrainerToUpdateViewModel> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct= default);
        Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct= default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct= default);
        Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct= default);
    }
}
