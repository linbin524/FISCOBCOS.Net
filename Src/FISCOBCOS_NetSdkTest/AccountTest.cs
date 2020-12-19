using FISOBCOS_NetSdk;
using FISOBCOS_NetSdk.Dto;
using FISOBCOS_NetSdk.Utis;
using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace FISCOBCOS_NetSdkTest
{
    public class AccountTest
    {
        public ITestOutputHelper _testOutput;//���������Ӧ�Ĳ��Խ��
        public AccountTest(ITestOutputHelper output)
        {
            _testOutput = output;
        }
        /// <summary>
        /// ����һ�Թ�˽Կ�����ɵ�json����copy ��txt�ļ���ֱ�ӵ���webase front �������
        /// </summary>
        [Fact]
        public void GeneratorAccountTest()
        {
            var account = AccountUtils.GeneratorAccount("adminUser" + new Random().Next(100000, 1000000).ToString());
            var accountString = account.ToJson();
            // Debug.WriteLine(accountString);
            _testOutput.WriteLine(accountString);
            Assert.True(accountString.ToObject<AccountDto>().PublicKey.Length > 0);
        }

        /// <summary>
        /// ͨ��˽Կ���ɹ�Կ
        /// </summary>
        [Fact]
        public void GeneratorPublicKeyByPrivateKeyTest()
        {
            var publicKey = AccountUtils.GeneratorPublicKeyByPrivateKey("25aa95ed437f8efaf37cf849a5a6ba212308d5d735105e03e38410542bf1d5ff");
            Assert.True(publicKey == "0x6a3b8f69e6860c1ad417944ae4d262930cf23ba0d1ee40ed09a4f165a2642be766901d0bd1b1d0510e0b9976ac314e961910a145073c21fdcb8cdaf8f4fbee56");
        }
    }
}
