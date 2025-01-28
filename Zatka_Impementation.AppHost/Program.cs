var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Zatka_Impementation>("zatka-impementation");

builder.Build().Run();
