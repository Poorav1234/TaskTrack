using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MVC.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string method = context.Request.Method;
        string logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(logFolder);
        string logFile = Path.Combine(logFolder, date + ".txt");
        string log = time + " - " + method + Environment.NewLine;
        await File.AppendAllTextAsync(logFile, log);
        await _next(context);
    }
}