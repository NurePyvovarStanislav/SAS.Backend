using AutoMapper;
using SAS.Backend.Contracts.Alerts;
using SAS.Backend.Contracts.Fields;
using SAS.Backend.Contracts.Measurements;
using SAS.Backend.Contracts.Sensors;
using SAS.Backend.Domain.Entities;
using DomainEnums = SAS.Backend.Domain.Enums;
using ContractEnums = SAS.Backend.Contracts.Enums;

namespace SAS.Backend.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Field, FieldDto>().ReverseMap();
            CreateMap<FieldCreateDto, Field>();
            CreateMap<FieldUpdateDto, Field>();

            CreateMap<Sensor, SensorDto>()
                .ForMember(d => d.SensorType, opt => opt.MapFrom(s => Enum.Parse<ContractEnums.SensorType>(s.SensorType.ToString())));
            CreateMap<SensorCreateDto, Sensor>()
                .ForMember(d => d.SensorType, opt => opt.MapFrom(s => Enum.Parse<DomainEnums.SensorType>(s.SensorType.ToString())));
            CreateMap<SensorUpdateDto, Sensor>()
                .ForMember(d => d.SensorType, opt => opt.MapFrom(s => Enum.Parse<DomainEnums.SensorType>(s.SensorType.ToString())));

            CreateMap<Measurement, MeasurementDto>().ReverseMap();
            CreateMap<MeasurementCreateDto, Measurement>();

            CreateMap<Alert, AlertDto>()
                .ForMember(d => d.Level, opt => opt.MapFrom(a => Enum.Parse<ContractEnums.AlertLevel>(a.Level.ToString())));
            CreateMap<AlertDto, Alert>()
                .ForMember(d => d.Level, opt => opt.MapFrom(a => Enum.Parse<DomainEnums.AlertLevel>(a.Level.ToString())));
        }
    }
}

