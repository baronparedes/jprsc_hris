using JPRSC.HRIS.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace JPRSC.HRIS.Infrastructure.Logging
{
    public static class Logger
    {
        public static void Log(LogEntry logEntry)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.Parameters.Add(new SqlParameter("@Action", logEntry.Action.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@Controller", logEntry.Controller.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@Level", logEntry.Level.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@LoggedOn", logEntry.LoggedOn.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@Message", logEntry.Message.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@Request", logEntry.Request.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@StackTrace", logEntry.StackTrace.AsSqlParameterValue()));
                command.Parameters.Add(new SqlParameter("@UserId", logEntry.UserId.AsSqlParameterValue()));
                command.CommandText = "INSERT INTO [dbo].[LogEntries] ([Action], [Controller], [Level], [LoggedOn], [Message], [Request], [StackTrace], [UserId]) VALUES (@Action, @Controller, @Level, @LoggedOn, @Message, @Request, @StackTrace, @UserId)";

                command.ExecuteNonQuery();
            }
        }
    }
}