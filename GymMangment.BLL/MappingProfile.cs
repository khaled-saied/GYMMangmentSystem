using AutoMapper;
using AutoMapper.Execution;
using GymMangment.BLL.ViewModels.MemberViewModels;
using GymMangment.DAL.Data.Models;
using Member = GymMangment.DAL.Data.Models.Member;

namespace GymMangment.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.Street} - {src.Address.BulidingNumber} - {src.Address.City}"))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));


            CreateMap<HealthRecord, HealthRecordViewModel>();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BulidingNumber))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Phone, opt => opt.Ignore())
                //.ForMember(dest=> dest.Address ,opt=> opt.MapFrom(src => src))
                .AfterMap((src, dest) =>
                {
                    dest.Address.BulidingNumber = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                });


            CreateMap<CreateMemberViewModel,Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    BulidingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => new HealthRecord
                {
                    BloodType = src.HealthRecordViewModel.BloodType,
                    Weight = src.HealthRecordViewModel.Weight,
                    Height = src.HealthRecordViewModel.Height,
                    Note = src.HealthRecordViewModel.Note
                }));


        }
    }
}
