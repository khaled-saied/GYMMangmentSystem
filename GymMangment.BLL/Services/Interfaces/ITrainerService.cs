using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Common;
using GymMangment.BLL.ViewModels.TrainerViewModels;

namespace GymMangment.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<Result<IEnumerable<TrainerViewModel>>> GetAllTrainersAsync(CancellationToken ct= default);
        Task<Result<TrainerViewModel?>> GetTrainerDetailsAsync(int trainerId, CancellationToken ct= default);
        Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct= default);
        Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct= default);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct= default);
        Task<Result> DeleteTrainerAsync(int trainerId, CancellationToken ct= default);
    }
}
