项目名
--
CraftNet计划

名字源于minecraft，我想开发一个网游版的minecraft，并摆脱来自微软的制约，当然C#这类开源语言不算。

proto代码生成
--
protoc --csharp_out=$pwd/proto ./ProtoSrc/*

数据库代码生成
--
dotnet ef dbcontext scaffold "Host=192.168.18.4;Database=game;Username=postgres;Password=liuyuran" Npgsql.EntityFrameworkCore.PostgreSQL -f -o database/generate