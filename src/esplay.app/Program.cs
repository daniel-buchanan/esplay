// See https://aka.ms/new-console-template for more information

using esplay.aggregates;
using esplay.common;
using esplay.persistence;
using esplay.processing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();
services.AddEsPlay()
    .AddAggregate<Animal>();
    
var provider = services.BuildServiceProvider();
var repository = provider.GetRequiredService<IRepository<Animal>>();
var command = provider.GetRequiredService<ICommand<Animal>>();

await command.Add(new[] {
    new Animal() { EarTag = "12345", Sex = "F" },
    new Animal() { RfidTag = "9820000018263", Sex = "M" }
});

var a = await repository.Single(x => x.EarTag == "12345");

if (a == null)
{
    Console.WriteLine("NOT EXISTING");
    return;
}

a.RfidTag = "098284756292";
await command.Update(a);
a = await repository.Single(x => x.EarTag == "12345");

Console.WriteLine(JsonConvert.SerializeObject(a));