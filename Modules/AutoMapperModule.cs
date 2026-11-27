using Autofac;
using AutoMapper;
using CSS_MagacinControl_App.Models.CsvModels;
using CSS_MagacinControl_App.Models.DboModels;
using CSS_MagacinControl_App.ViewModels;
using CSS_MagacinControl_App.ViewModels.Authentication;
using System;
using System.Globalization;

namespace CSS_MagacinControl_App.Modules
{
    public class AutoMapperModule : Module
    {
        private readonly string _statusWorkConst = "U radu";

        protected override void Load(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IdentDbo, IdentiViewModel>()
                    .ForMember(x => x.PripremljenaKolicina, opt => opt.MapFrom(src => src.PrimljenaKolicina));

                cfg.CreateMap<IdentiViewModel, IdentDbo>()
                    .ForMember(x => x.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                    .ForMember(x => x.PrimljenaKolicina, opt => opt.MapFrom(src => src.PripremljenaKolicina));

                cfg.CreateMap<FakturaDbo, FaktureViewModel>()
                    .ForMember(x => x.DatumFakture, opt => opt.MapFrom(src => src.DatumFakture.ToString("dd/MM/yyyy")))
                    .ForMember(x => x.Status, opt => opt.MapFrom(src => src.StatusFakture));

                cfg.CreateMap<FaktureViewModel, FakturaDbo>()
                    .ForMember(x => x.DatumFakture, opt => opt.MapFrom(src => ParseDatum(src.DatumFakture)))
                    .ForMember(x => x.StatusFakture, opt => opt.MapFrom(src => src.Status))
                    .ForMember(x => x.RobaZaPakovanjeItems, opt => opt.Ignore());

                cfg.CreateMap<UserDbo, UserModel>();
                cfg.CreateMap<UserModel, UserDbo>();

                // Csv models => ViewModels
                cfg.CreateMap<FakturaCsv, FaktureViewModel>()
                    .ForMember(x => x.Magacioner, opt => opt.MapFrom(src => App.Current.Properties["Username"]))
                    .ForMember(x => x.Status, opt => opt.MapFrom(src => _statusWorkConst));

                cfg.CreateMap<IdentiCsv, IdentiViewModel>()
                    .ForMember(x => x.KolicinaSaFakture, opt => opt.MapFrom(src => 0))
                    .ForMember(x => x.PripremljenaKolicina, opt => opt.MapFrom(src => 0))
                    .ForMember(x => x.Razlika, opt => opt.MapFrom(src => 0));
            });

            builder.RegisterInstance(config).As<IConfigurationProvider>().ExternallyOwned();
            builder.RegisterType<Mapper>().As<IMapper>();
        }

        // Helper used by AutoMapper expression to parse flexible date formats like "3/3/2026" or "03/03/2026".
        private static DateTime ParseDatum(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return DateTime.MinValue;

            var s = input.Trim();
            var formats = new[] { "d/M/yyyy", "dd/MM/yyyy" };

            if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                return parsed;

            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                return parsed;

            return DateTime.MinValue;
        }
    }
}