﻿namespace Leads.WebApi.Application.Infrastructure.Controllers.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using Requests.Results;
    using Responses;


    public static class ApiRequestControllerExtensions
    {
        public static ApiResponse Process<TController, TApiRequest>(
            this TController controller,
            TApiRequest request)
            where TController : Controller, IApiRequestController
            where TApiRequest : IApiRequest
        {
            try
            {
                controller
                    .ApiRequestHandlerFactory
                    .CreateHandler<TApiRequest>()
                    .Execute(request);

                controller.ExpectCommit.PerformCommit();

                return new ApiResponse();
            }
            catch (ApiException ex)
            {
                return new ApiResponse(ex);
            }
        }

        public static ApiResponse<TResponseResult> Process<TController, TApiRequest, TApiResult, TResponseResult>(
            this TController controller,
            TApiRequest request,
            Func<TApiResult, TResponseResult> mapResult)
            where TController : Controller, IApiRequestController
            where TApiRequest : IApiRequest<TApiResult>
            where TApiResult : IApiRequestResult
        {
            try
            {
                TApiResult result = controller
                    .ApiRequestHandlerFactory
                    .CreateHandler<TApiRequest, TApiResult>()
                    .Execute(request);

                controller.ExpectCommit.PerformCommit();

                return new ApiResponse<TResponseResult>(mapResult(result));
            }
            catch (ApiException ex)
            {
                return new ApiResponse<TResponseResult>(ex);
            }
        }
        
        public static async Task<ApiResponse> ProcessAsync<TController, TApiRequest>(
            this TController controller,
            TApiRequest request)
            where TController : Controller, IApiRequestController
            where TApiRequest : IApiRequest
        {
            try
            {
                await controller
                    .ApiRequestHandlerFactory
                    .CreateAsyncHandler<TApiRequest>()
                    .ExecuteAsync(request);

                controller.ExpectCommit.PerformCommit();

                return new ApiResponse();
            }
            catch (ApiException ex)
            {
                return new ApiResponse(ex);
            }
        }

        public static async Task<ApiResponse<TResponseResult>> ProcessAsync<TController, TApiRequest, TApiResult, TResponseResult>(
            this TController controller,
            TApiRequest request,
            Func<TApiResult, TResponseResult> mapResult)
            where TController : Controller, IApiRequestController
            where TApiRequest : IApiRequest<TApiResult>
            where TApiResult : IApiRequestResult
        {
            try
            {
                TApiResult result = await controller
                    .ApiRequestHandlerFactory
                    .CreateAsyncHandler<TApiRequest, TApiResult>()
                    .ExecuteAsync(request);

                controller.ExpectCommit.PerformCommit();

                return new ApiResponse<TResponseResult>(mapResult(result));
            }
            catch (ApiException ex)
            {
                return new ApiResponse<TResponseResult>(ex);
            }
        }
    }
}