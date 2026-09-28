using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymMangment.BLL.Common;
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

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.StartDate >= model.EndDate) return Result.Validation("EndDate Must Be After StartDate ");

            if (model.StartDate <= DateTime.Now) return Result.Validation("StartDate Must Be In The Future"); 

            if (model.Capacity <1  || model.Capacity > 25) return Result.Validation("Capacity Must Be BetWeen 1 And 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category is null) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Specialty>(category.CategoryName ,true, out var trainerSpecialty);
            if (!isValid || trainer.Specialty != trainerSpecialty) return Result.Validation("Can not Create This Session For this Trainer");

            var session = _mapper.Map<CreateSessionViewModel,Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Session ");
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

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessioId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(sessioId, ct);

            if (session is null)
            {
                return Result<SessionViewModel>.NotFound("Session Not Found");
            }
            else
            {
                var sessionMapped = _mapper.Map<Session,SessionViewModel>(session);
                sessionMapped.AvailableSlots = sessionMapped.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookSlotsAsync(sessioId,ct);
                return Result<SessionViewModel>.Ok(sessionMapped);
            }

        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null)
                return Result<UpdateSessionViewModel>.NotFound("Session Not Foun");

            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Can not Update Session That Has Already Sarted ");

            var bookingCount =await _unitOfWork.SessionRepository.GetCountOfBookSlotsAsync(sessionId, ct);
            if(bookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Can not Update Session That Has Already Sarted ");

            var MappedSession = _mapper.Map<Session, UpdateSessionViewModel>(session);

            return Result<UpdateSessionViewModel>.Ok(MappedSession);

        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null)
                return Result.NotFound("Session Not Foun");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can not Update Session That Has Already Sarted ");

            if (model.EndDate <= model.StartDate)
                return Result.Validation("EndDate Must Be After StartDate");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("StartDate Must Be In The Future ");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookSlotsAsync(sessionId, ct);
            if (bookingCount > 0)
                return Result.Fail("Can not update a session that has bookings.");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);
            if (category is null) return Result.NotFound("Category Not Found");

            var isValid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var trainerSpecialty);
            if (!isValid || trainer.Specialty != trainerSpecialty) return Result.Validation("Can not Create This Session For this Trainer");

            _mapper.Map(model,session);
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.Update(session);
            var result= await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failes To Update Session");
        }

        public async Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null)
                return Result.NotFound("Session Not Foun");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Can not Deleted Session That Has Not Ended Yet ");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookSlotsAsync(sessionId, ct);
            if (bookingCount > 0)
                return Result.Fail("Can not update a session that has bookings.");

            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Session");
        }
    }
}
