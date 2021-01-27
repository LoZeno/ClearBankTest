using System.Configuration;
using ClearBank.DeveloperTest.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class DataStoreInitializationTest
    {
        [Fact]
        public void WhenDataStoreInConfigIsBackup_InitializesBackup()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "Backup";

            var ioc = new InjectionContainer();

            var accountDataStore = ioc.GetProvider().GetRequiredService<IAccountDataStore>();

            Assert.IsType<BackupAccountDataStore>(accountDataStore);
        }
        
        [Fact]
        public void WhenDataStoreInConfigIsMain_InitializesDataStore()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "Main";

            var ioc = new InjectionContainer();

            var accountDataStore = ioc.GetProvider().GetRequiredService<IAccountDataStore>();

            Assert.IsType<AccountDataStore>(accountDataStore);
        }
        
        [Fact]
        public void WhenDataStoreInConfigIsNotValid_ThrowsConfigurationErrorsException()
        {
            ConfigurationManager.AppSettings["DataStoreType"] = "NonValid";

            Assert.Throws<ConfigurationErrorsException>(() => new InjectionContainer());
        }
    }
}