# FISCOBCOS.Net

#### 介绍
FISCOBCOS 区块链 Net版SDK，目前是采用Json RPC API 接口和区块链底层（标准版本）进行适配。


#### 软件架构
软件架构说明

FISCOBCOS.Net Sdk 采用 net core 3.1,配套开发vs Code 和Visual Studio 2019。


#### 功能介绍

1.实现 rpc 异步请求
2.实现FISCO BCOS公私钥、账户生成，拓展生成webase front导入用户json，
可以直接导入相关中间件。
3.实现合约操作封装，如：合约部署、请求参数构建、交易签名、RLP编码转换、交易签名等
4.实现合约部署、合约交易、合约Call操作、合约交易回执获取等
5.实现 合约input、output、Event等解析
6、所有操作配置对应的单元测试Demo。可以参考复制。

#### 安装教程

1.  下载源码，vs2019 nuget包还原。
2. vs code 安装solidity 插件，在vs code创建一个文件夹存放原始sol合约。
3. vs code 按 F5 执行编译命令 “compile current Solidity contract”,会生成合约对应的abi和bin
4、将上面编译得到abi和bin 放到你的项目中，进行相关操作。

#### 使用说明

1. 在 FISOBCOS_NetSdk类库配置 BaseConfig 文件，配置好对应的底层请求DefaultUrl，如：http://127.0.0.1:8545
2. 使用ContractService 和ApiService进行相关业务操作
3. ContractService 主要是合约调用等操作，详细看对应的单元测试中的ContractTest.cs
4、ApiService 是底层非交易Json RPC的API通过封装，可参考单元测试ApiServiceTest.cs
备注：通用的Json RPC API 相对简单，没有封装对应的DTO 实体，
操作时候可以通过在线json 生成实体进行业务结合。

#### 迭代计划

1. 实现channel 协议
2.  实现国密版本适配
3.  业务归集等通用组件扩展


#### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request


