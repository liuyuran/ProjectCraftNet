项目名
--
CraftNet计划

名字源于minecraft，我想开发一个网游版的minecraft，并摆脱来自微软的制约，当然C#这类开源语言不算。

你可以认为这是我的一个附带单元测试且自定义交互协议的服务端学习项目，就不要指望短时间真能有什么东西了吧。

客户端在别的工程里，勿念，UE5的C++路线想入门贼TM难，能买到的书没一本有用的。

项目构成
--

|       目录        |          作用           |
|:---------------:|:---------------------:|
| ProjectCraftNet |  主项目，用来实现不需要暴露的核心功能   |
|   ModManager    | 核心项目，几乎所有可定制化的内容都写在里面 |
|     CoreMod     |   模组项目，用来演示mod如何制作    |
|  BlackBoxTest   |  黑盒测试项目，防止重复出现同样的错误   |

proto代码生成
--
protoc --csharp_out=$pwd/proto ./ProtoSrc/*

数据库代码生成（对应两套开发环境）
--
dotnet ef dbcontext scaffold "Host=192.168.18.4;Database=game;Username=postgres;Password=liuyuran" Npgsql.EntityFrameworkCore.PostgreSQL -f -o database/generate

dotnet ef dbcontext scaffold "Host=127.0.0.1;Database=game;Username=postgres;Password=example" Npgsql.EntityFrameworkCore.PostgreSQL -f -o database/generate
