using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.StellarFederation.Features.Accounts.Models;
using Beamable.StellarFederation.Features.Contract.CliWrapper.Exceptions;

namespace Beamable.StellarFederation.Features.Contract.CliWrapper;

public class CliClient : IService
{
    private readonly Configuration _configuration;

    private bool _initialized;
    private const string WorkingDirectory = "/beamApp";
    private const string ContractsWorkingDirectory = $"{WorkingDirectory}/smart";
    private const string Executable = "stellar";
    private const string SorobanSdkVersion = "22.0.8";
    private const string SorobanMacroPackageVersion = "0.4.1";

    public const string ContractSourcePath = "/beamApp/sources";

    private const int ProcessTimeoutMs = 60000;

    public CliClient(Configuration configuration)
    {
        _configuration = configuration;
    }

    public void Initialize()
    {
        if (_initialized) return;

        try
        {
            if (!Directory.Exists(ContractsWorkingDirectory))
                Directory.CreateDirectory(ContractsWorkingDirectory);

            _initialized = true;
        }
        catch (Exception e)
        {
            throw new CliException($"Failed to initialize CLI client: {e.Message}");
        }
    }

    public async Task CreateProject(string moduleName)
    {
        await Execute(Executable, $"contract init --name {moduleName} .");
        await ExecuteShell($"sed -i 's|^soroban-sdk = .*$|soroban-sdk = \"{SorobanSdkVersion}\"|' Cargo.toml");
        await ExecuteShell($"""
                            sed -i '/^\[dependencies\]/a\
                            stellar-tokens = "={SorobanMacroPackageVersion}"\
                            stellar-access = "={SorobanMacroPackageVersion}"\
                            stellar-contract-utils = "={SorobanMacroPackageVersion}"\
                            stellar-macros = "={SorobanMacroPackageVersion}"
                            ' contracts/{moduleName}/Cargo.toml
                            """);
    }

    public async Task CopyContractCode(string moduleName)
    {
        await ExecuteShell($"rm -r contracts/{moduleName}/src/test.rs");
        await ExecuteShell($"rm -r contracts/{moduleName}/src/lib.rs");
        await ExecuteShell($"cp {ContractSourcePath}/{moduleName}.rc {ContractsWorkingDirectory}/contracts/{moduleName}/src/lib.rs");
    }

    public async Task CompileContract(string moduleName)
    {
        await Execute(Executable, $"contract build --package {moduleName}");
        await Execute(Executable, $"contract optimize --wasm target/wasm32v1-none/release/{moduleName}.wasm");
    }

    public async Task<string> DeployContract(string moduleName, Account ownerAccount)
    {
        return await Execute(Executable, $"contract deploy --wasm target/wasm32v1-none/release/{moduleName}.optimized.wasm --source-account {ownerAccount.SecretSeed} --network {await _configuration.StellarNetwork} --alias {moduleName} -- --initial_owner {ownerAccount.Address}");
    }

    private static async Task<string> ExecuteShell(string command, string workingDirectory = ContractsWorkingDirectory)
    {
        return await Execute("/bin/sh", $"-c \"{command.Replace("\"", "\\\"")}\"", workingDirectory);
    }

    private static async Task<string> Execute(string program, string args, string workingDirectory = ContractsWorkingDirectory, bool ignoreOutput = false, bool ignoreOutputError = true)
    {
        using var process = new Process();
        process.StartInfo =
            new ProcessStartInfo(program, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

        if (!string.IsNullOrWhiteSpace(workingDirectory))
            process.StartInfo.WorkingDirectory = workingDirectory;

        process.Start();

        if (!ignoreOutput)
        {

            var outputText = await process.StandardOutput.ReadToEndAsync();
            var outputError = await process.StandardError.ReadToEndAsync();

            process.WaitForExit(ProcessTimeoutMs);

            if (!string.IsNullOrEmpty(outputError) && !ignoreOutputError)
            {
                BeamableLogger.LogError("Process error: {processOutput}", outputError);
                throw new CliException(outputError);
            }

            BeamableLogger.Log("Process output: {processOutput}", outputText);
            return outputText;
        }
        process.WaitForExit(ProcessTimeoutMs);
        return string.Empty;
    }

}