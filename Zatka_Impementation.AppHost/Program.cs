var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Zatka_Impementation_Testing>("zatka-impementation");

builder.Build().Run();
