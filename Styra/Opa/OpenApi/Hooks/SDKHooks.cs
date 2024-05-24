//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by Speakeasy (https://speakeasyapi.dev). DO NOT EDIT.
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
namespace Styra.Opa.OpenApi.Hooks
{
    using Styra.Opa.OpenApi.Utils;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public sealed class FailEarlyException : Exception {}

    public class SDKHooks: IHooks
    {
        public List<ISDKInitHook> sdkInitHooks;
        public List<IBeforeRequestHook> beforeRequestHooks;
        public List<IAfterSuccessHook> afterSuccessHooks;
        public List<IAfterErrorHook> afterErrorHooks;

        public SDKHooks()
        {
            this.sdkInitHooks = new List<ISDKInitHook>();
            this.beforeRequestHooks = new List<IBeforeRequestHook>();
            this.afterSuccessHooks = new List<IAfterSuccessHook>();
            this.afterErrorHooks = new List<IAfterErrorHook>();
            HookRegistration.InitHooks(this);
        }

        public void RegisterSDKInitHook(ISDKInitHook hook)
        {
            this.sdkInitHooks.Add(hook);
        }

        public void RegisterBeforeRequestHook(IBeforeRequestHook hook)
        {
            this.beforeRequestHooks.Add(hook);
        }

        public void RegisterAfterSuccessHook(IAfterSuccessHook hook)
        {
            this.afterSuccessHooks.Add(hook);
        }

        public void RegisterAfterErrorHook(IAfterErrorHook hook)
        {
            this.afterErrorHooks.Add(hook);
        }

        public (string, ISpeakeasyHttpClient) SDKInit(string baseUrl, ISpeakeasyHttpClient client)
        {
            var urlAndClient = (baseUrl, client);
            foreach (var hook in this.sdkInitHooks)
            {
                try
                {
                    urlAndClient = hook.SDKInit(urlAndClient.Item1, urlAndClient.Item2);
                } catch (Exception ex)
                {
                    throw new Exception("An error occurred while calling SDKInit hook.", ex);
                }
            }
            return urlAndClient;
        }

        public async Task<HttpRequestMessage> BeforeRequestAsync(BeforeRequestContext hookCtx, HttpRequestMessage request)
        {
            foreach (var hook in this.beforeRequestHooks)
            {
                try
                {
                    request = await hook.BeforeRequestAsync(hookCtx, request);
                } catch (Exception ex)
                {
                    throw new Exception("An error occurred while calling BeforeRequestAsync hook", ex);
                }
            }
            return request;
        }

        public async Task<HttpResponseMessage> AfterSuccessAsync(AfterSuccessContext hookCtx, HttpResponseMessage response)
        {
            foreach (var hook in this.afterSuccessHooks)
            {
                try
                {
                    response = await hook.AfterSuccessAsync(hookCtx, response);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while calling AfterSuccessAsync hook.", ex);
                }
            }
            return response;
        }

        public async Task<HttpResponseMessage?> AfterErrorAsync(AfterErrorContext hookCtx, HttpResponseMessage? response, Exception? error)
        {

            (HttpResponseMessage?, Exception?) responseAndError = (response, error);
            foreach (var hook in this.afterErrorHooks)
            {
                try
                {
                    responseAndError = await hook.AfterErrorAsync(hookCtx, responseAndError.Item1, responseAndError.Item2);
                } catch (FailEarlyException)
                {
                    throw;
                } catch (Exception ex)
                {
                    throw new Exception("An error occurred while calling AfterErrorAsync hook", ex);
                }
            }

            if (responseAndError.Item2 != null)
            {
                throw responseAndError.Item2;
            }

            return responseAndError.Item1;
        }
    }
}