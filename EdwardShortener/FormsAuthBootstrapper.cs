﻿//https://github.com/GusBeare/Nancy-UserManager/blob/master/NancyUserManager/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdwardShortener
{
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Bootstrapper;
    using Nancy.Cryptography;
    using Nancy.TinyIoc;

    public class FormsAuthBootstrapper : DefaultNancyBootstrapper
    {
        public Nancy.Cryptography.CryptographyConfiguration cryptographyConfiguration;

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // We don't call "base" here to prevent auto-discovery of
            // types/dependencies
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // Here we register our user mapper as a per-request singleton.
            // As this is now per-request we could inject a request scoped
            // database "context" or other request scoped services.
            container.Register<IUserMapper, Objects.User>();
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // create the cryptography config
            StaticConfiguration.DisableErrorTraces = false;
            cryptographyConfiguration = new CryptographyConfiguration(
                       new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })),
                       new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })));

        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            // At request startup we modify the request pipelines to
            // include forms authentication - passing in our now request
            // scoped user name mapper.
            //
            // The pipelines passed in here are specific to this request,
            // so we can add/remove/update items in them as we please.
            
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration
                {
                    RedirectUrl = "/",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                    CryptographyConfiguration = cryptographyConfiguration
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);
        }
    }
}