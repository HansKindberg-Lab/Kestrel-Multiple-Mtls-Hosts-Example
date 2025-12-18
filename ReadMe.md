# Kestrel-Multiple-Mtls-Hosts-Example

Examples on multiple mTLS-endpoints. These are examples for local development in Visual Studio. To get mTLS work & feel like "production".

## 1 Development

To get it working we need settings, code and certificates. For the certificates, see [2 Environment](/ReadMe.md#2-environment).

### 1.1 Application-By-Host-Name

This is possible thanks to: [Support for the .localhost top-level domain](https://learn.microsoft.com/en-us/aspnet/core/test/localhost-tld).

Docker and Kestrel are setup with the same hosts and port. If you have run with Docker and then run with Kestrel you have to remove / stop the Docker container first to not get conflicts.

#### 1.1.1 Docker (Linux)

- [launchSettings.json](/Source/Application-By-Host-Name/Properties/launchSettings.json)
- [appsettings.Docker.json](/Source/Application-By-Host-Name/appsettings.Docker.json)
- [Project-file](/Source/Application-By-Host-Name/Application-By-Host-Name.csproj)
- [Program.cs](/Source/Application-By-Host-Name/Program.cs)

#### 1.1.2 Kestrel (Windows)

- [launchSettings.json](/Source/Application-By-Host-Name/Properties/launchSettings.json)
- [appsettings.Kestrel.json](/Source/Application-By-Host-Name/appsettings.Kestrel.json)
- [Program.cs](/Source/Application-By-Host-Name/Program.cs)

#### 1.1.3 And a little "hack"

We need to separate the main-endpoint from the mtls-endpoints. If not, once we go to https://application.example.localhost, all other requrests, no matter what https://*.application.example.localhost url, will not trigger mTLS. We can do it with different protocols. The main-endpoint have protocols "Http3" and all others, all mtls-endpoints, have protocols "Http1AndHttp2AndHttp3".

[appsettings.Docker.json (Application-By-Host-Name)](/Source/Application-By-Host-Name/appsettings.Docker.json#L11):

	...
	"Sni": {
		"*.application.example.localhost": {
			"ClientCertificateMode": "RequireCertificate",
			"Protocols": "Http1AndHttp2AndHttp3"

		},
		"*": {
			"ClientCertificateMode": "NoCertificate",
			"Protocols": "Http3"
		}
	},
	...

[appsettings.Kestrel.json (Application-By-Host-Name)](/Source/Application-By-Host-Name/appsettings.Kestrel.json#L9)

	...
	"EndpointDefaults": {
		"ClientCertificateMode": "RequireCertificate",
		"Protocols": "Http1AndHttp2AndHttp3"
	},
	...
	"Endpoints": {
		"All": {
			"Sni": {
				...
				"*": {
					"ClientCertificateMode": "NoCertificate",
					"Protocols": "Http3"
				}
			},
			...
		}
	}

### 1.2 Application-By-Port

#### 1.2.1 Docker (Linux)

- [launchSettings.json](/Source/Application-By-Port/Properties/launchSettings.json)
- [appsettings.Docker.json](/Source/Application-By-Port/appsettings.Docker.json)
- [Project-file](/Source/Application-By-Port/Application-By-Port.csproj)
- [Program.cs](/Source/Application-By-Port/Program.cs)

#### 1.2.2 Kestrel (Windows)

- [launchSettings.json](/Source/Application-By-Port/Properties/launchSettings.json)
- [appsettings.Kestrel.json](/Source/Application-By-Port/appsettings.Kestrel.json)
- [Program.cs](/Source/Application-By-Port/Program.cs)

## 2 Environment

To be able to run the application from Visual Studio when we develop there are some requirements to get it working:

- We need certificates in our certificate-store.
- We need the registry-key HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\SecurityProviders\\SCHANNEL\\SendTrustedIssuerList (value = 1, DWord).

### 2.1 Setup

- Run [Setup](/.windows-certificate-management/Setup) "as Administrator" to setup certificates and registry-key (SendTrustedIssuerList). You can do it by building the solution and then right-click [/.windows-certificate-management/Setup/bin/Debug/net10.0/Setup.exe](/.windows-certificate-management/Setup/bin/Debug/net10.0/Setup.exe) and choose "Run as Administrator".

Configuration: [/.windows-certificate-management/Management/appsettings.json](/.windows-certificate-management/Management/appsettings.json#L83)

- [Overview of TLS - SSL (Schannel SSP) / Management of trusted issuers for client authentication / SendTrustedIssuerList](https://learn.microsoft.com/en-us/windows-server/security/tls/what-s-new-in-tls-ssl-schannel-ssp-overview#BKMK_TrustedIssuers)

### 2.2 Cleanup

- Run [Cleanup](/.windows-certificate-management/Cleanup) "as Administrator" to cleanup/remove certificates and [possibly registry-key (SendTrustedIssuerList)](/.windows-certificate-management/Management/appsettings.json#L81). You can do it by building the solution and then right-click [/.windows-certificate-management/Cleanup/bin/Debug/net10.0/Cleanup.exe](/.windows-certificate-management/Cleanup/bin/Debug/net10.0/Cleanup.exe) and choose "Run as Administrator".

Configuration: [/.windows-certificate-management/Management/appsettings.json](/.windows-certificate-management/Management/appsettings.json#L2)

## 3 Certificates

The certificates are only for testing/laborating.

Example pesons:

- Alice Smith
- Bob Johnson
- Carol Williams
- Dave Brown

All the necessary certificate-files are included in this solution:

- [**client-1.pfx**](/.certificates/client-1.pfx) - *CERT:\\CurrentUser\\My* - password = **password**
- [**client-2.pfx**](/.certificates/client-2.pfx) - *CERT:\\CurrentUser\\My* - password = **password**
- [**client-3.pfx**](/.certificates/client-3.pfx) - *CERT:\\CurrentUser\\My* - password = **password**
- [**client-4.pfx**](/.certificates/client-4.pfx) - *CERT:\\CurrentUser\\My* - password = **password**
- [**https.crt**](/.certificates/https.crt) - used in [appsettings.Docker.json (Application-By-Host-Name)](/Source/Application-By-Host-Name/appsettings.Docker.json#L6), [appsettings.Kestrel.json (Application-By-Host-Name)](/Source/Application-By-Host-Name/appsettings.Kestrel.json#L6), [appsettings.Docker.json (Application-By-Port)](/Source/Application-By-Port/appsettings.Docker.json#L6) and [appsettings.Kestrel.json (Application-By-Port)](/Source/Application-By-Port/appsettings.Kestrel.json#L6) to configure the https-certificate
- [**https.key**](/.certificates/https.key) - used in [appsettings.Docker.json (Application-By-Host-Name)](/Source/Application-By-Host-Name/appsettings.Docker.json#L5), [appsettings.Kestrel.json (Application-By-Host-Name)](/Source/Application-By-Host-Name/appsettings.Kestrel.json#L5), [appsettings.Docker.json (Application-By-Port)](/Source/Application-By-Port/appsettings.Docker.json#L5) and [appsettings.Kestrel.json (Application-By-Port)](/Source/Application-By-Port/appsettings.Kestrel.json#L5) to configure the https-certificate
- [**https-1.crt**](/.certificates/https-1.crt)
- [**https-1.key**](/.certificates/https-1.key)
- [**https-2.crt**](/.certificates/https-2.crt)
- [**https-2.key**](/.certificates/https-2.key)
- [**https-3.crt**](/.certificates/https-3.crt)
- [**https-3.key**](/.certificates/https-3.key)
- [**https-4.crt**](/.certificates/https-4.crt)
- [**https-4.key**](/.certificates/https-4.key)
- [**intermediate-1.crt**](/.certificates/intermediate-1.crt) - *CERT:\\CurrentUser\\CA*, *CERT:\\LocalMachine\\Store-a9fb8818-9294-450a-a124-e1d552e83b21* and *CERT:\\LocalMachine\\Store-b538096c-4a03-4696-a9d1-f2434cf1cc64*
- [**intermediate-2.crt**](/.certificates/intermediate-2.crt) - *CERT:\\CurrentUser\\CA* and *CERT:\\LocalMachine\\Store-a9fb8818-9294-450a-a124-e1d552e83b21*
- [**intermediate-3.crt**](/.certificates/intermediate-3.crt) - *CERT:\\CurrentUser\\CA* and *CERT:\\LocalMachine\\Store-a9fb8818-9294-450a-a124-e1d552e83b21*
- [**intermediate-4.crt**](/.certificates/intermediate-4.crt) - *CERT:\\CurrentUser\\CA*
- [**root.crt**](/.certificates/root.crt) - *CERT:\\CurrentUser\\Root*

If you want to create them yourself, you need to create the following certificate-structure.

- root
	- https
	- https-1
	- https-2
	- https-3
	- https-4
	- intermediate-1
		- client-1
	- intermediate-2
		- client-2
	- intermediate-3
		- client-3
	- intermediate-4
		- client-4

The certificates in this solution are created by using this web-application, [Certificate-Factory](https://github.com/HansKindberg-Lab/Certificate-Factory). It is a web-application you can run in Visual Studio and then upload a json-certificate-file like below.

1. **File \*** (upload a json-file like below)

		{
			"Defaults": {
				"HashAlgorithm": "Sha256",
				"NotAfter": "9999-01-01"
			},
			"Roots": {
				"root": {
					"Certificate": {
						"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example Root CA"
					},
					"IssuedCertificates": {
						"https": {
							"Certificate": {
								"EnhancedKeyUsage": "ServerAuthentication",
								"KeyUsage": "DigitalSignature",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example https-certificate",
								"SubjectAlternativeName": {
									"DnsNames": [
										"application.example.localhost",
										"*.application.example.localhost",
										"localhost"
									],
									"IpAddresses": [
										"127.0.0.1"
									]
								}
							}
						},
						"https-1": {
							"Certificate": {
								"EnhancedKeyUsage": "ServerAuthentication",
								"KeyUsage": "DigitalSignature",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example https-certificate-1",
								"SubjectAlternativeName": {
									"DnsNames": [
										"application.example.localhost",
										"*.application.example.localhost",
										"localhost"
									],
									"IpAddresses": [
										"127.0.0.1"
									]
								}
							}
						},
						"https-2": {
							"Certificate": {
								"EnhancedKeyUsage": "ServerAuthentication",
								"KeyUsage": "DigitalSignature",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example https-certificate-2",
								"SubjectAlternativeName": {
									"DnsNames": [
										"application.example.localhost",
										"*.application.example.localhost",
										"localhost"
									],
									"IpAddresses": [
										"127.0.0.1"
									]
								}
							}
						},
						"https-3": {
							"Certificate": {
								"EnhancedKeyUsage": "ServerAuthentication",
								"KeyUsage": "DigitalSignature",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example https-certificate-3",
								"SubjectAlternativeName": {
									"DnsNames": [
										"application.example.localhost",
										"*.application.example.localhost",
										"localhost"
									],
									"IpAddresses": [
										"127.0.0.1"
									]
								}
							}
						},
						"https-4": {
							"Certificate": {
								"EnhancedKeyUsage": "ServerAuthentication",
								"KeyUsage": "DigitalSignature",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example https-certificate-4",
								"SubjectAlternativeName": {
									"DnsNames": [
										"application.example.localhost",
										"*.application.example.localhost",
										"localhost"
									],
									"IpAddresses": [
										"127.0.0.1"
									]
								}
							}
						},
						"intermediate-1": {
							"Certificate": {
								"CertificateAuthority": {
									"PathLengthConstraint": 0
								},
								"KeyUsage": "KeyCertSign",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example Intermediate CA 1"
							},
							"IssuedCertificates": {
								"client-1": {
									"Certificate": {
 										"EnhancedKeyUsage": "ClientAuthentication, SecureEmail",
										"KeyUsage": "DigitalSignature, KeyEncipherment",
										"Subject": "E=alice.smith@example.local, G=Alice, SN=Smith, CN=Alice Smith, O=Example, C=Local",
										"SubjectAlternativeName": {
											"EmailAddresses": [
												"alice.smith@example.local"
											],
											"UserPrincipalNames": [
												"alice.smith@example.local"
											]
										}
									}
								}
							}
						},
						"intermediate-2": {
							"Certificate": {
								"CertificateAuthority": {
									"PathLengthConstraint": 0
								},
								"KeyUsage": "KeyCertSign",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example Intermediate CA 2"
							},
							"IssuedCertificates": {
								"client-2": {
									"Certificate": {
 										"EnhancedKeyUsage": "ClientAuthentication, SecureEmail",
										"KeyUsage": "DigitalSignature, KeyEncipherment",
										"Subject": "E=bob.johnson@example.local, G=Bob, SN=Johnson, CN=Bob Johnson, O=Example, C=Local",
										"SubjectAlternativeName": {
											"EmailAddresses": [
												"bob.johnson@example.local"
											],
											"UserPrincipalNames": [
												"bob.johnson@example.local"
											]
										}
									}
								}
							}
						},
						"intermediate-3": {
							"Certificate": {
								"CertificateAuthority": {
									"PathLengthConstraint": 0
								},
								"KeyUsage": "KeyCertSign",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example Intermediate CA 3"
							},
							"IssuedCertificates": {
								"client-3": {
									"Certificate": {
 										"EnhancedKeyUsage": "ClientAuthentication, SecureEmail",
										"KeyUsage": "DigitalSignature, KeyEncipherment",
										"Subject": "E=carol.williams@example.local, G=Carol, SN=Williams, CN=Carol Williams, O=Example, C=Local",
										"SubjectAlternativeName": {
											"EmailAddresses": [
												"carol.williams@example.local"
											],
											"UserPrincipalNames": [
												"carol.williams@example.local"
											]
										}
									}
								}
							}
						},
						"intermediate-4": {
							"Certificate": {
								"CertificateAuthority": {
									"PathLengthConstraint": 0
								},
								"KeyUsage": "KeyCertSign",
								"Subject": "CN=Kestrel-Multiple-Mtls-Hosts-Example Intermediate CA 4"
							},
							"IssuedCertificates": {
								"client-4": {
									"Certificate": {
 										"EnhancedKeyUsage": "ClientAuthentication, SecureEmail",
										"KeyUsage": "DigitalSignature, KeyEncipherment",
										"Subject": "E=dave.brown@example.local, G=Dave, SN=Brown, CN=Dave Brown, O=Example, C=Local",
										"SubjectAlternativeName": {
											"EmailAddresses": [
												"dave.brown@example.local"
											],
											"UserPrincipalNames": [
												"dave.brown@example.local"
											]
										}
									}
								}
							}
						}
					}
				}
			},
			"RootsDefaults": {
				"CertificateAuthority": {
					"CertificateAuthority": true
				},
				"KeyUsage": "CrlSign, KeyCertSign"
			}
		}

2. **Archive kind \***

	Choose **\*.crt, \*.key and \*.pfx files**

3. **Flat archive**

	**Checked**

4. **Password \***

	Use **password** as password when creating the certificates.

You will then get a zip-file including all certificate-files.

## 4 Links

- [Configure endpoints for the ASP.NET Core Kestrel web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints)
- [Developers using Kestrel can configure the list of CAs per-hostname #45456](https://github.com/dotnet/runtime/issues/45456)
- [Overview of TLS - SSL (Schannel SSP)](https://learn.microsoft.com/en-us/windows-server/security/tls/what-s-new-in-tls-ssl-schannel-ssp-overview)
- [Access denied when trying to load X509Certificate2 on (upgraded) Windows 10 April 2018 Update #25](https://github.com/Microsoft/dotnet-framework-early-access/issues/25)
- [Default permissions for the MachineKeys folders](https://learn.microsoft.com/en-US/troubleshoot/windows-server/windows-security/default-permissions-machinekeys-folders)
- [Solving Access Denied in Crypto Machine Keys](https://odetocode.com/blogs/scott/archive/2020/01/12/solving-access-denied-in-crypto-machine-keys.aspx)
- [Support for the .localhost top-level domain](https://learn.microsoft.com/en-us/aspnet/core/test/localhost-tld)