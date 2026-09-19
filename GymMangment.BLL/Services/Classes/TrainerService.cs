using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.BLL.ViewModels.TrainerViewModels;
using GymMangment.DAL.Data.Models;
using GymMangment.DAL.Repositorities.Interfaces;

namespace GymMangment.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository, IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);
            return trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialties = t.Specialty.ToString()
            });
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer == null)
                return null;
            else
                return new TrainerViewModel()
                {
                    Name = trainer.Name,
                    Specialties = trainer.Specialty.ToString(),
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                    Address = $"{trainer.Address.BulidingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
                };
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (await _trainerRepository.AnyAsync(t => t.Email == model.Email, ct))
                return false;
            if (await _trainerRepository.AnyAsync(t => t.Phone == model.Phone, ct))
                return false;

            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Specialty = model.Specialties,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    City = model.City,
                    BulidingNumber = model.BuildingNumber,
                    Street = model.Street
                }
            };

            var result = await _trainerRepository.AddAsync(trainer);
            return result > 0;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer == null)
                return null;
            else
                return new TrainerToUpdateViewModel()
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    BuildingNumber = trainer.Address.BulidingNumber,
                    Street = trainer.Address.Street,
                    City = trainer.Address.City,
                    Specialties = trainer.Specialty
                };
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;

            var hasFutureSessions = await _sessionRepository.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return false;

            var result = await _trainerRepository.DeleteAsync(trainer, ct);
            return result > 0;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;

            if (await _trainerRepository.AnyAsync(t => t.Email == model.Email && t.Id != trainerId, ct))
                return false;
            if (await _trainerRepository.AnyAsync(t => t.Phone == model.Phone && t.Id != trainerId, ct))
                return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Address.BulidingNumber = model.BuildingNumber;
            trainer.Specialty = model.Specialties;
            trainer.UpdatedAt = DateTime.Now;

            var result = await _trainerRepository.UpdateAsync(trainer, ct);
            return result > 0;
        }

        public async Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer is null) return false;

            var hasFutureSessions = await _sessionRepository.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);
            if (hasFutureSessions)
                return false;

            var result = await _trainerRepository.DeleteAsync(trainer, ct);
            return result > 0;
        }
    }
}
