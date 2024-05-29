using System;
using System.ServiceModel;
using System.Threading.Tasks;
using AdminService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdminTool_wpf.Testing
{
    [TestClass]
    public class IntegrationTests
    {
        private ChannelFactory<IAdminService> _channelFactory;
        private IAdminService _serviceClient;

        [TestInitialize]
        public void SetUp()
        {
            var binding = new NetTcpBinding();
            binding.SendTimeout = TimeSpan.FromSeconds(120);
            _channelFactory =
                new ChannelFactory<IAdminService>(binding,
                new EndpointAddress("net.tcp://localhost:8000/AdminService"));
            _serviceClient = _channelFactory.CreateChannel();
        }

        [TestCleanup]
        public void TearDown()
        {
            if (_channelFactory.State == CommunicationState.Opened)
            {
                _channelFactory.Close();
            }
        }

        [TestMethod]
        public async Task Test_ServiceMethodGetPass()
        {
            // Запуск теста взаимодействия с службой
            var result = await Task.Run(() => _serviceClient.GetPasswordByUsername("user1"));

            // Проверка правильного результата
            Assert.AreEqual("pa1", result);
        }

        [TestMethod]
        public async Task Test_ServiceMethodGetUserID()
        {
            // Запуск теста взаимодействия с службой
            var result = await Task.Run(() => _serviceClient.GetSelectedUserId("user1"));

            // Проверка правильного результата
            Assert.AreEqual(2, result);
        }
        
        [TestMethod]
        public async Task Test_ServiceMethodIsUserExist()
        {
            // Запуск теста взаимодействия с службой
            var result = await Task.Run(() => _serviceClient.IsUserExists("user1"));

            // Проверка правильного результата
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public async Task Test_ServiceMethodIsUserNotExist()
        {
            // Запуск теста взаимодействия с службой
            var result = await Task.Run(() => _serviceClient.IsUserExists("user"));

            // Проверка правильного результата
            Assert.AreEqual(false, result);
        }
    }
}