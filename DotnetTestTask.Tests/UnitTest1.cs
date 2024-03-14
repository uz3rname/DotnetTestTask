using System.Collections.Immutable;
using DotnetTestTask.Data;
using DotnetTestTask.Models;
using DotnetTestTask.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Tests;

public class UnitTest1
{
    private AppDbContext BuildContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase("DotnetTestTask");

        return new AppDbContext(optionsBuilder.Options);
    }

    [Fact]
    public async void TestStoreItems()
    {
        var dbContext = BuildContext();
        IStoreItemService storeItemService = new DbStoreItemService(dbContext);
        var item = await storeItemService.Save(new CreateStoreItemArguments {
            Amount = 10,
            Name = "pencil",
        });
        await storeItemService.Save(new CreateStoreItemArguments {
            Amount = 20,
            Name = "pen",
        });
        {
            var found = await storeItemService.GetAll();
            Assert.Equal(2, found.Count);
        }
        {
            var found = await storeItemService.GetById(item.Id);
            Assert.NotNull(found);
        }
        {
            item.Amount = 30;
            await storeItemService.Update(item);
            var found = await storeItemService.GetById(item.Id);
            Assert.NotNull(found);
            Assert.Equal(30, found.Amount);
            Assert.Equal("pencil", found.Name);
        }
    }
    
    [Fact]
    public async void TestInvoices()
    {
        var dbContext = BuildContext();
        IInvoiceService invoiceService = new DbInvoiceService(dbContext);
        IStoreItemService storeItemService = new DbStoreItemService(dbContext);
        IInvoiceItemService invoiceItemService = new DbInvoiceItemService(dbContext);
        IInvoiceCreatorService invoiceCreatorService = new DbInvoiceCreatorService(dbContext);

        var pencil = await storeItemService.Save(new CreateStoreItemArguments {
            Amount = 10,
            Name = "pencil",
        });
        var pen = await storeItemService.Save(new CreateStoreItemArguments {
            Amount = 20,
            Name = "pen",
        });
        
        var invoice = await invoiceService.Save(new CreateInvoiceArguments {
            InvoiceItems = [
                invoiceItemService.Create(new CreateInvoiceItemArguments {
                    StoreItem = pen,
                    Amount = 5,
                }),
                invoiceItemService.Create(new CreateInvoiceItemArguments {
                    StoreItem = pencil,
                    Amount = 4,
                }),
            ],
            InvoiceCreator = invoiceCreatorService.Create(new CreateInvoiceCreatorArguments {
                Name = "Test",
            })
        });
        
        var found = await invoiceService.GetAll();
        
        Assert.Single(found);
        Assert.Equal(2, found[0].InvoiceItems.Count);
        Assert.Equal("Test", found[0].InvoiceCreator.Name);
        Assert.Single(found[0].InvoiceItems.Where(x => x.StoreItem.Name == "pen"));
    }
}