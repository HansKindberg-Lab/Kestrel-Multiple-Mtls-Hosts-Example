using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections.Features;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(kestrelServerOptions =>
{
	kestrelServerOptions.ConfigureHttpsDefaults(httpsConnectionAdapterOptions =>
	{
		httpsConnectionAdapterOptions.OnAuthenticate = (connectionContext, sslServerAuthenticationOptions) =>
		{
			if(!sslServerAuthenticationOptions.ClientCertificateRequired)
				return;

			if(sslServerAuthenticationOptions.ServerCertificate is not X509Certificate2 serverCertificate)
				throw new InvalidOperationException("The server-certificate is invalid.");

			sslServerAuthenticationOptions.CertificateChainPolicy = new X509ChainPolicy
			{
				/*
					RevocationMode = X509RevocationMode.NoCheck
					We only set this value because we are using certificates created by ourselves. The default value for RevocationMode is X509RevocationMode.Online
					and that value should be used when using "real" certificates. If we use X509RevocationMode.Online when using the certificates in this solution we
					will get client-certificate validation errors.
				*/
				RevocationMode = X509RevocationMode.NoCheck,
				/*
					TrustMode = X509ChainTrustMode.System
					We use the system for trust. As we have imported the intermediate- and root-certificates to the container it should work. The other value
					is X509ChainTrustMode.CustomRootTrust. I do not know what is required to do when using that value. But using the system-trust feels more safe.
				*/
				TrustMode = X509ChainTrustMode.System
			};

			var hostName = connectionContext.Features.Get<ITlsHandshakeFeature>()!.HostName;
			var linuxTrustList = new List<string>();
			var windowsStoreName = string.Empty;

			if(hostName.StartsWith("mtls.", StringComparison.OrdinalIgnoreCase))
			{
				linuxTrustList.Add("/etc/ssl/certs/intermediate-1.crt");
				linuxTrustList.Add("/etc/ssl/certs/intermediate-2.crt");
				linuxTrustList.Add("/etc/ssl/certs/intermediate-3.crt");
				linuxTrustList.Add("/etc/ssl/certs/intermediate-4.crt");
				windowsStoreName = "Store-a9fb8818-9294-450a-a124-e1d552e83b21";
			}
			else if(hostName.StartsWith("client-1-", StringComparison.OrdinalIgnoreCase))
			{
				linuxTrustList.Add("/etc/ssl/certs/intermediate-1.crt");
				windowsStoreName = "Store-b538096c-4a03-4696-a9d1-f2434cf1cc64";
			}
			else if(hostName.StartsWith("client-2-", StringComparison.OrdinalIgnoreCase))
			{
				linuxTrustList.Add("/etc/ssl/certs/intermediate-2.crt");
				windowsStoreName = "Store-c561f80e-574f-4aad-aa56-900452c7eec8";
			}
			else if(hostName.StartsWith("client-3-", StringComparison.OrdinalIgnoreCase))
			{
				linuxTrustList.Add("/etc/ssl/certs/intermediate-3.crt");
				windowsStoreName = "Store-d57bbe91-87b7-4b43-984f-8012533a1129";
			}
			else if(hostName.StartsWith("client-4-", StringComparison.OrdinalIgnoreCase))
			{
				linuxTrustList.Add("/etc/ssl/certs/intermediate-4.crt");
				windowsStoreName = "Store-e60c228c-0906-4143-afbd-a0b462b7fde0";
			}

			SslCertificateTrust? sslCertificateTrust;

			if(OperatingSystem.IsWindows())
			{
				/*
					On Windows we can only set the SSL-certificate-trust to the LocalMachine store. If not we get an exception.

					System.PlatformNotSupportedException: 'Only LocalMachine stores are supported on Windows.':

					#if TARGET_WINDOWS
						if (sendTrustInHandshake && store.Location != StoreLocation.LocalMachine)
						{
							throw new PlatformNotSupportedException(SR.net_ssl_trust_store);
						}
					#endif
				*/
				using(var store = new X509Store(windowsStoreName, StoreLocation.LocalMachine)) // This store must have been set up in the Windows Certificate Manager. You can set it upp with .windows-certificate-management/Setup in this solution.
				{
					store.Open(OpenFlags.ReadOnly);

					sslCertificateTrust = SslCertificateTrust.CreateForX509Store(store, true);
				}
			}
			else
			{
				var certificates = new X509Certificate2Collection();

				foreach(var path in linuxTrustList)
				{
					certificates.ImportFromPemFile(path);
				}

				sslCertificateTrust = SslCertificateTrust.CreateForX509Collection(certificates, true);
			}

			sslServerAuthenticationOptions.ServerCertificateContext = SslStreamCertificateContext.Create(serverCertificate, null, false, sslCertificateTrust);
		};
	});
});

builder.Services.AddRazorPages();

var app = builder.Build();
app.UseRouting();
app.MapRazorPages();
app.Run();