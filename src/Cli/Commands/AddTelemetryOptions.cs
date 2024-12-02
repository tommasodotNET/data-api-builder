// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Azure.DataApiBuilder.Config;
using Azure.DataApiBuilder.Config.ObjectModel;
using Azure.DataApiBuilder.Product;
using Cli.Constants;
using CommandLine;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using static Cli.Utils;

namespace Cli.Commands
{
    /// <summary>
    /// Telemetry command options
    /// </summary>
    [Verb("add-telemetry", isDefault: false, HelpText = "Add telemetry for Data Api builder Application", Hidden = false)]
    public class AddTelemetryOptions : Options
    {
        public AddTelemetryOptions(
            string? appInsightsConnString = null,
            CliBool? appInsightsEnabled = null,
            string? otelEndpoint = null,
            CliBool? otelEnabled = null,
            string? otelHeaders = null,
            OtlpExportProtocol? otelExportProtocol = null,
            string? serviceName = null,
            string? config = null) : base(config)
        {
            if (appInsightsEnabled is not null)
            {
                AppInsightsConnString = appInsightsConnString;
                AppInsightsEnabled = appInsightsEnabled;
            }

            if (otelEnabled is not null)
            {
                OpenTelemetryEndpoint = otelEndpoint;
                OpenTelemetryHeaders = otelHeaders;
                OpenTelemetryServiceName = serviceName;
                OpenTelemetryEnabled = otelEnabled;
                OpenTelemetryExportProtocol = otelExportProtocol;
            }
        }
        public AddTelemetryOptions(string appInsightsConnString, CliBool appInsightsEnabled, string? config) : base(config)
        {
            AppInsightsConnString = appInsightsConnString;
            AppInsightsEnabled = appInsightsEnabled;
        }

        // Connection string for the Application Insights resource to which telemetry data should be sent.
        // This option  is required and must be provided with a valid connection string.
        [Option("app-insights-conn-string", Required = false, HelpText = "Connection string for the Application Insights resource for telemetry data")]
        public string? AppInsightsConnString { get; }

        // To specify whether Application Insights telemetry should be enabled. This flag is optional and default value is true.
        [Option("app-insights-enabled", Default = CliBool.True, Required = false, HelpText = "(Default: true) Enable/Disable Application Insights")]
        public CliBool? AppInsightsEnabled { get; }

        // Connection string for the Open Telemetry resource to which telemetry data should be sent.
        // This option  is required and must be provided with a valid connection string.
        [Option("otel-endpoint", Required = false, HelpText = "Endpoint for Open Telemetry for telemetry data")]
        public string? OpenTelemetryEndpoint { get; }

        // Headers for the Open Telemetry resource to which telemetry data should be sent.
        [Option("otel-headers", Required = false, HelpText = "Headers for Open Telemetry for telemetry data")]
        public string? OpenTelemetryHeaders { get; }

        // Service Name for the Open Telemetry resource to which telemetry data should be sent. This flag is optional and default value is dab.
        [Option("otel-service-name", Default = "dab", Required = false, HelpText = "Headers for Open Telemetry for telemetry data")]
        public string? OpenTelemetryServiceName { get; }

        // To specify whether Open Telemetry telemetry should be enabled. This flag is optional and default value is true.
        [Option("otel-enabled", Default = CliBool.True, Required = false, HelpText = "(Default: true) Enable/Disable OTEL")]
        public CliBool? OpenTelemetryEnabled { get; }

        // Specify the Open Telemetry protocol. This flag is optional and default value is grpc.
        [Option("otel-protocol", Default = OtlpExportProtocol.Grpc, Required = false, HelpText = "(Default: grpc) Accepted: grpc/httpprotobuf")]
        public OtlpExportProtocol? OpenTelemetryExportProtocol { get; }

        public int Handler(ILogger logger, FileSystemRuntimeConfigLoader loader, IFileSystem fileSystem)
        {
            logger.LogInformation("{productName} {version}", PRODUCT_NAME, ProductInfo.GetProductVersion());

            bool isSuccess = ConfigGenerator.TryAddTelemetry(this, loader, fileSystem);

            if (isSuccess)
            {
                logger.LogInformation("Successfully added telemetry to the configuration file.");
            }
            else
            {
                logger.LogError("Failed to add telemetry to the configuration file.");
            }

            return isSuccess ? CliReturnCode.SUCCESS : CliReturnCode.GENERAL_ERROR;
        }
    }
}
