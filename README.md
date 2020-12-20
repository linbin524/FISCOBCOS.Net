English | [中文](https://github.com/linbin524/FISCOBCOS.Net/blob/main/README.zh.md)



#### Introduction

Fiscobcos blockchain net version SDK currently uses the JSON RPC API interface and the blockchain bottom layer (Standard Version) for adaptation.



#### Software architecture



Software architecture description



FISCOBCOS.Net SDK adopts net core 3.1, and the supporting development tools are vs Code and visual studio 2019.




#### Function introduction



1. Implement RPC Synchronous request / asynchronous request

2. Realize the generation of FISCO bcos public and private keys and accounts, expand the generation of webase front, import user JSON, and directly import webase middleware.

3. Implement contract operation encapsulation, such as contract deployment, request parameter construction, transaction signature, RLP code conversion, etc.

4. Realize contract deployment, contract transaction, contract call operation, contract transaction receipt acquisition, etc.

5. Implement contract input, output, event analysis.

6. Configure the corresponding unit test demo for all operations. Reference can be made to copy.

Note: send transaction and return transaction receipt test synchronously, there is a certain probability that it will be empty, because the underlying transaction is repackaged, and the consensus has not been completed.

#### Installation tutorial



1. Download the source code and restore the vs2019 nuget package.

2. Vs Code installs the solidness plug-in and creates a folder in vs code to store the original sol contract.

3. Vs code executes the compile command "compile current solid contract" by pressing F5 to generate ABI and bin corresponding to the contract.

4. Compile the above ABI and bin and put them into your project for related operations.



reference resources:

![Image text]( https://github.com/linbin524/FISCOBCOS.Net/blob/main/Img/how-to-use-console-generator1.gif )



#### Instructions for use

1. In fisobcos_ The netsdk class library is configured with the baseconfiguration file, and the corresponding underlying request defaulturl is configured, such as: http://127.0.0.1 :8545 。

2. Use Contractservice and QueryApiservice for related business operations.

3. Contractservice mainly encapsulates contract calls and other operations. For details, please refer to the ContractTest.cs 。

4. Apiservice is the underlying non transactional JSON RPC API package, which can refer to the unit test ApiServiceTest.cs 。

Note: the common JSON RPC API is relatively simple and does not encapsulate the corresponding dto entity. During operation, the entity can be generated through online JSON for business combination.


#### Iteration plan

1. Implement channel protocol.

2. Realize the adaptation of national secret version.

3. Extension of common components such as business aggregation.




#### Participation contribution

1. Fork warehouse

2. New feat_ XXX branch

3. Submit code

4. Create a new pull request
