//  -------------------------------------------------------------------------------------------------
//  <copyright file="MessagePackOutputFormatter.cs" company="RHEA System S.A.">
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

namespace MessagePackTest.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Carter;
    using Carter.Response;
    
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    
    using Model;
    
    /// <summary>
    /// The Home <see cref="CarterModule"/>
    /// </summary>
    public class HomeModule : CarterModule
    {
        /// <summary>
        /// The (injected) <see cref="ILogger"/>
        /// </summary>
        private readonly ILogger<HomeModule> logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="HomeModule"/> class
		/// </summary>
		/// <param name="logger">
		/// The injected <see cref="ILogger"/>
		/// </param>
		public HomeModule(ILogger<HomeModule> logger)
	    {
            this.logger = logger;
	    }

		/// <summary>
		/// Invoked at startup to add routes to the HTTP pipeline
		/// </summary>
		/// <param name="app">
		/// An instance of <see cref="IEndpointRouteBuilder"/>
		/// </param>
		public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", () => "MessagePack TEST using Carter");

            app.MapGet("/things/{amount:int}", (int amount, HttpResponse res) =>
            {
	            var things = this.GenerateThings(amount);

	            var sw = Stopwatch.StartNew();

	            res.Negotiate(things);

	            this.logger.LogInformation("{amount} returned in {elapsed} [ms]", amount, sw.ElapsedMilliseconds);
            });
        }

        /// <summary>
        /// Generates a specified amount <see cref="Thing"/> objects
        /// </summary>
        /// <param name="amount">
        /// The amount of <see cref="Thing"/>s to create
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{Thing}"/>
        /// </returns>
        private IEnumerable<Thing> GenerateThings(int amount)
        {
            var things = new List<Thing>();

            for (int i = 0; i < amount; i++)
            {
                var parameter = new Parameter(Guid.NewGuid())
                {
                    Name = $"p_{i}",
                    Value = i * 1
                };

                things.Add(parameter);

                var definition = new ElementDefinition(Guid.NewGuid())
                {
                    Name = $"ed_{i}",
                    Description = $"this is an element definition description {i} in which the quick brown fox jumps over the lazy dog",
                    Aliases = new List<string>() { $"alias_{i}", "another alias"},
                    Parameters = new List<Guid> { parameter.Iid }
                };

                things.Add(definition);
			}

            return things;
        }
    }
}
