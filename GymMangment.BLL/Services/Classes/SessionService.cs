using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.SessionViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Data.Models.Enums;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.StartDate >= model.EndDate) return false;

            if (model.StartDate <= DateTime.Now) return false; 

            if (model.Capacity <1  || model.Capacity > 25) return false;

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return false;

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category is null) return false;

            var isValid = Enum.TryParse<Specialty>(category.CategoryName ,true, out var trainerSpecialty);
            if (!isValid || trainer.Specialty != trainerSpecialty) return false;

            var session = _mapper.Map<CreateSessionViewModel,Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var repo = _unitOfWork.SessionRepository;

            var sessions = await repo.GetAllSessionsWithTrainerAndCategory(ct);

            if (sessions == null || !sessions.Any()) return null;

            var mappedSessions = sessions.Select(s => new SessionViewModel()
            {
                Id = s.Id,
                Capacity = s.Capacity,
                CategoryName = s.Category.CategoryName,
                TrainerName = s.Trainer.Name,
                Description = s.Description,
                EndDate = s.EndDate,
                StartDate = s.StartDate, 
            });

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await repo.GetCountOfBookSlotsAsync(session.Id,ct);
            }

            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result); ;
        }
    }
}
