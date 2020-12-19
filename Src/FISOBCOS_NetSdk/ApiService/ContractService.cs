using FISOBCOS_NetSdk.Core;
using FISOBCOS_NetSdk.Dto;
using FISOBCOS_NetSdk.Utils;
using FISOBCOS_NetSdk.Utis;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.JsonDeserialisation;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RLP;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FISOBCOS_NetSdk
{
    public class ContractService
    {

        public RpcClient RpcClient;

        /// <summary>
        /// 创建合约服务
        /// </summary>
        /// <param name="url">请求底层通信地址</param>
        /// <param name="rpcId">rpcId 默认为1</param>
        /// <param name="chainId">链Id</param>
        /// <param name="groupId">群组Id</param>
        /// <param name="privateKey">用私钥</param>
        public ContractService(string url, int rpcId, int chainId, int groupId, string privateKey)
        {
            RpcClient = new RpcClient(new Uri(url));
            this.RpcId = rpcId;
            this.ChainId = chainId;
            this.GroupId = groupId;
            this.GasPrice = BaseConfig.DefaultGasPrice;
            this.GasLimit = BaseConfig.DefaultGasLimit;
            this.RequestId = BaseConfig.DefaultRequestId;
            this.RequestObjectId = BaseConfig.DefaultRequestObjectId;
            this.TranscationValue = BaseConfig.DefaultTranscationsValue;
            this.PrivateKey = privateKey;
        }

        #region 基础属性
        protected int RpcId { get; set; }
        protected int GroupId { get; set; }
        protected int ChainId { get; set; }
        protected int GasLimit { get; set; }
        protected int GasPrice { get; set; }
        protected int RequestId { get; set; }
        protected int RequestObjectId { get; set; }
        protected int TranscationValue { get; set; }
        protected string PrivateKey { get; set; }
        #endregion


        /// <summary>
        /// 通用合约部署，只返回交易Hash
        /// </summary>
        /// <param name="binCode">合约内容</param>
        /// <returns>交易Hash</returns>
        public async Task<string> DeployContractAsync(string binCode)
        {
            var blockNumber = await GetBlockNumberAsync();
            var transParams = BuildTransactionParams(binCode, blockNumber, "");
            var tx = BuildRLPTranscation(transParams);
            tx.Sign(new EthECKey(this.PrivateKey.HexToByteArray(), true));
            var result = await SendRequestAysnc<string>(tx.Data, tx.Signature);
            return result;

        }

        /// <summary>
        /// 通用合约部署，返回交易回执
        /// </summary>
        /// <param name="binCode">合约内容</param>
        /// <returns>交易回执</returns>
        public async Task<ReceiptResultDto> DeployContractWithReceiptAsync(string binCode)
        {
            var txHash = await DeployContractAsync(binCode);
            var receiptResult = await GetTranscationReceiptAsync(txHash);
            return receiptResult;
        }



        /// <summary>
        /// 发送交易,返回交易回执
        /// </summary>
        /// <param name="abi">合约abi</param>
        /// <param name="contractAddress">合约地址</param>
        /// <param name="functionName">合约请求调用方法名称</param>
        /// <param name="inputsParameters">方法对应的 参数</param>
        /// <param name="value">请求参数值</param>
        /// <returns>交易回执</returns>
        public async Task<ReceiptResultDto> SendTranscationWithReceiptAsync(string abi, string contractAddress, string functionName, Parameter[] inputsParameters, params object[] value)
        {
            ReceiptResultDto receiptResult=new ReceiptResultDto();

            var des = new ABIDeserialiser();
            var contract = des.DeserialiseContract(abi);
            var function = contract.Functions.FirstOrDefault(x => x.Name == functionName);
            var sha3Signature = function.Sha3Signature;// "0x53ba0944";
            var functionCallEncoder = new FunctionCallEncoder();
            var result = functionCallEncoder.EncodeRequest(sha3Signature, inputsParameters,
                value);
            var blockNumber = await GetBlockNumberAsync();
            var transDto = BuildTransactionParams(result, blockNumber, contractAddress);
            var tx = BuildRLPTranscation(transDto);
            tx.Sign(new EthECKey(this.PrivateKey.HexToByteArray(), true));
            var txHash = await SendRequestAysnc<string>(tx.Data, tx.Signature);

            if (txHash != null)
            {
                receiptResult = await GetTranscationReceiptAsync(txHash);
                if(receiptResult==null)
                throw new Exception("txHash != null 的时候报错了：" + receiptResult.ToJson());
            }
            return receiptResult;
        }

        /// <summary>
        /// 获取交易回执
        /// </summary>
        /// <param name="tanscationHash">交易Hash</param>
        /// <returns></returns>
        public async Task<ReceiptResultDto> GetTranscationReceiptAsync(string tanscationHash)
        {
            //var request = new RpcRequest(this.RequestId, JsonRPCAPIConfig.GetTransactionReceipt, new object[] { this.RequestObjectId, tanscationHash });
            var getRequest = new RpcRequestMessage(this.RequestId, JsonRPCAPIConfig.GetTransactionReceipt, new object[] { this.RequestObjectId, tanscationHash });
            var result = await HttpUtils.RpcPost<ReceiptResultDto>(BaseConfig.DefaultUrl, getRequest);
            if (result == null) throw new Exception(" 获取交易回执方法报空：" + result.ToJson());
            return result;
        }

        /// <summary>
        /// Call 调用 适用于链上调用但不需要共识（通常用constant,view等修饰的合约方法）
        /// </summary>
        /// <param name="contractAddress">合约地址</param>
        /// <param name="abi">合约abi</param>
        /// <param name="callFunctionName">调用方法名称</param>
        /// <returns>返回交易回执</returns>
        public async Task<ReceiptResultDto> CallRequestAsync(string contractAddress, string abi, string callFunctionName)
        {
            CallInput callDto = new CallInput();
            callDto.From = new Account(this.PrivateKey).Address.ToLower();//address ;
            callDto.To = contractAddress;
            var contractAbi = new ABIDeserialiser().DeserialiseContract(abi);
            var function = contractAbi.Functions.FirstOrDefault(x => x.Name == callFunctionName);
            callDto.Value = new HexBigInteger(0);
            callDto.Data = "0x" + function.Sha3Signature;
            var getRequest = new RpcRequest(this.RequestId, JsonRPCAPIConfig.Call, new object[] { this.RequestObjectId, callDto });
            var result = await this.RpcClient.SendRequestAsync<ReceiptResultDto>(getRequest);
            //var getRequest = new RpcRequestMessage(this.RequestId, JsonRPCAPIConfig.Call, new object[] { this.RequestObjectId, callDto });
            //var result = HttpUtils.RpcPost<ReceiptResultDto>(BaseConfig.DefaultUrl, getRequest); //同步方法
            return result;

        }

        #region 内部方法

        /// <summary>
        /// 请求发送RPC交易
        /// </summary>
        /// <typeparam name="TResult">返回结果</typeparam>
        /// <param name="txData">交易数据（rlp）</param>
        /// <param name="txSignature">交易签名</param>
        /// <returns>返回交易结果</returns>
        protected async Task<TResult> SendRequestAysnc<TResult>(byte[][] txData, EthECDSASignature txSignature)
        {
            var rlpSignedEncoded = RLPEncoder.EncodeSigned(new SignedData(txData, txSignature), 10).ToHex();
            var request = new RpcRequest(this.RequestId, JsonRPCAPIConfig.SendRawTransaction, new object[] { this.RequestObjectId, rlpSignedEncoded });
            var response = await RpcClient.SendRequestAsync<TResult>(request);
            return response;
        }


        /// <summary>
        /// 构建交易参数
        /// </summary>
        /// <param name="txData">交易数据</param>
        /// <param name="blockNumber">区块高度</param>
        /// <param name="to">发送地址</param>
        /// <returns>交易参数实体</returns>
        protected TransactionDto BuildTransactionParams(string txData, long blockNumber, string to)
        {
            TransactionDto rawTransaction = new TransactionDto();

            rawTransaction.BlockLimit = blockNumber + 500;//交易防重上限，默认加500
            rawTransaction.Data = txData;//交易数据
            rawTransaction.ExtraData = "";//附加数据，默认为空字符串
            rawTransaction.FiscoChainId = this.ChainId;//链ID
            rawTransaction.GasLimit = this.GasLimit;//交易消耗gas上限，默认为30000000
            rawTransaction.GasPrice = this.GasPrice;//默认为30000000
            rawTransaction.Randomid = new Random().Next(10000000, 1000000000); ;
            rawTransaction.To = to;//合约部署默认为空
            rawTransaction.Value = this.TranscationValue;//默认为0
            rawTransaction.GroupId = this.GroupId;//群组ID 

            return rawTransaction;

        }

        /// <summary>
        /// 创建交易RLP
        /// </summary>
        /// <param name="rawTransaction">交易实体</param>
        /// <returns>RLPSigner</returns>
        protected RLPSigner BuildRLPTranscation(TransactionDto rawTransaction)
        {
            var tx = new RLPSigner(new[] {rawTransaction.Randomid.ToBytesForRLPEncoding(),rawTransaction.GasPrice.ToBytesForRLPEncoding(), rawTransaction.GasLimit.ToBytesForRLPEncoding(),rawTransaction.BlockLimit.ToBytesForRLPEncoding(), rawTransaction.To.HexToByteArray(),  rawTransaction.Value.ToBytesForRLPEncoding(),rawTransaction.Data.HexToByteArray(),rawTransaction.FiscoChainId.ToBytesForRLPEncoding(),
              rawTransaction.GroupId.ToBytesForRLPEncoding(),rawTransaction.ExtraData.HexToByteArray()});
            return tx;
        }

        /// <summary>
        /// 获取当前区块高度
        /// </summary>
        /// <param name="rpcId">rpcId</param>
        /// <param name="groupId">群组Id</param>
        /// <returns>当前区块高度</returns>
        protected async Task<long> GetBlockNumberAsync()
        {
            var request = new RpcRequest(this.RpcId, JsonRPCAPIConfig.GetBlockNumber, new object[] { this.GroupId });
            var responseResult = await RpcClient.SendRequestAsync<string>(request);
            long blockNumber = Convert.ToInt64(responseResult, 16);
            return blockNumber;
        }
        #endregion


    }
}
