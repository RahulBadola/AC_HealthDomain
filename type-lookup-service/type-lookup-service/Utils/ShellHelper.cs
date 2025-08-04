using Medallion.Shell;
using System;
using type_lookup_service.Model;

namespace type_lookup_service.Utils
{
    public static class ShellHelper
    {
        public static ShellExecutionResult ExecuteBashCommand(string command)
        {
            try
            {

                var executeCommand = Command.Run("bash", "-c", command);

                var result = executeCommand.Result;

                if (result.Success)
                {
                    return new ShellExecutionResult
                    {
                        ExitCode = result.ExitCode,
                        Json = result.StandardOutput
                    };
                }
                else
                {
                    return new ShellExecutionResult
                    {
                        ExitCode = result.ExitCode,
                        ErrorMessage = result.StandardError
                    };
                }
            }
            catch (Exception ex)
            {
                return new ShellExecutionResult
                {
                    ExitCode = -99,
                    ErrorMessage = ex.Message
                };
            }

        }

    }
}
