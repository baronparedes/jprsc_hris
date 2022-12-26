using AutoMapper;
using AutoMapper.Configuration;
using JPRSC.HRIS.Infrastructure.Data;
using SimpleInjector;
using System;

namespace JPRSC.HRIS.WebApp.Infrastructure.Mapping
{
    public class MapperProvider
    {
        private readonly Container _container;

        public MapperProvider(Container container)
        {
            _container = container;
        }

        public IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();
            mce.ConstructServicesUsing(_container.GetInstance);

            mce.AddMaps(typeof(MapperProvider).Assembly);
            mce.AddMaps(typeof(ApplicationDbContext).Assembly);

            var mc = new MapperConfiguration(mce);
            try
            {
                mc.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                // This is expected when validationg the configuration
                // Calling ForAllOtherMembers(opts => opts.Ignore()) breaks current mappings, so we cannot use this
            }

            IMapper m = new Mapper(mc, t => _container.GetInstance(t));

            return m;
        }
    }
}