//  -------------------------------------------------------------------------------------------------
//  <copyright file="MessagePackResponseNegotiator.cs" company="RHEA System S.A.">
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

namespace MessagePackTest.Server.v1_0_0
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Carter;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;

    using Model;

    /// <summary>
    /// An <see cref="IResponseNegotiator"/> to support MessagePack responses
    /// </summary>
    public class MessagePackResponseNegotiator : IResponseNegotiator
    {
        /// <summary>
        /// Asserts whether the <see cref="MessagePackResponseNegotiator"/> can handle
        /// the request
        /// </summary>
        /// <param name="accept">
        /// The <see cref="MediaTypeHeaderValue"/>
        /// </param>
        /// <returns>
        /// true or false
        /// </returns>
        public bool CanHandle(MediaTypeHeaderValue accept)
        {
	        var mediaType = accept.ToString();
            
			var contentTypeArray = mediaType.Split(';');
            if (contentTypeArray[0].Trim() == "application/msgpack" 
                && contentTypeArray[1].Trim() == "ecss-e-tm-10-25"
                && contentTypeArray[2].Trim() == "version=1.0.0") 
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the content negotiation
        /// </summary>
        /// <typeparam name="T">
        /// The generic Type
        /// </typeparam>
        /// <param name="req">
        /// The <see cref="HttpRequest"/> 
        /// </param>
        /// <param name="res">
        /// The <see cref="HttpResponse"/>
        /// </param>
        /// <param name="model">
        /// The object that is to be handled
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/>
        /// </param>
        /// <returns></returns>
        public Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
        {
	        var things = model as List<Thing>;

	        if (things == null)
	        {
		        throw new NotSupportedException("Only List<Thing> is supported");
			}
            
            res.ContentType = "application/msgpack; ecss-e-tm-10-25; version=1.0.0";

            var serializer = new MessagePack.MessagePackSerializer();
            
            serializer.SerializeToPipeWriter(things, res.BodyWriter, cancellationToken);

            return Task.CompletedTask;
        }
    }
}
