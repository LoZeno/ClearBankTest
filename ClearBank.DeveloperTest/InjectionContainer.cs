using System;
using System.Configuration;
using ClearBank.DeveloperTest.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ClearBank.DeveloperTest
{
    /// <summary>
    /// This class represent the ioc container for the (eventual) application 
    /// </summary>
    public class InjectionContainer
    {
        private readonly IServiceCollection _serviceCollection;

        public InjectionContainer()
        {
            _serviceCollection = new ServiceCollection();
            var dataStoreSetting = ConfigurationManager.AppSettings["DataStoreType"];

            _serviceCollection = dataStoreSetting switch
            {
                "Backup" => _serviceCollection.AddSingleton<IAccountDataStore, BackupAccountDataStore>(),
                "Main" => _serviceCollection.AddSingleton<IAccountDataStore, AccountDataStore>(),
                _ => throw new ConfigurationErrorsException("Invalid DataStoreType configuration")
            };
        }

        public IServiceProvider GetProvider()
        {
            return _serviceCollection.BuildServiceProvider();
        }
    }
}