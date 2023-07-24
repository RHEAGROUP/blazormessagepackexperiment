//  -------------------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="RHEA System S.A.">
// 
//    Copyright 2023 RHEA System S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

using Carter;
using MessagePackTest.Server.v1_0_0;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddCarter(configurator: c =>
{
    c.WithResponseNegotiator<MessagePackResponseNegotiator>();
    c.WithResponseNegotiator<JsonSerializerResponseNegotiator>();
});

var app = builder.Build();

app.UseCors(corsPolicyBuilder =>
{
	corsPolicyBuilder
		.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader();
});

app.MapCarter();

app.Run();