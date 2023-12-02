using Grpc.Core;
using Grpc1.Data;
using Grpc1.Models;
using Microsoft.EntityFrameworkCore;

namespace Grpc1.Services;

    public class ToDoService : ToDoIt.ToDoItBase
    {
        private readonly AppDbContext dbContext;

        public ToDoService(AppDbContext dbContext)
        {
           this.dbContext = dbContext;
        }

        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if(request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Supply a valid object"));
                
                var grpc1Item = new Grpc1Item 
                {
                    Title = request.Title,
                    Description = request.Description
                };

                await dbContext.AddAsync(grpc1Item);
                await dbContext.SaveChangesAsync();

                return await Task.FromResult(new CreateToDoResponse
                {
                    Id = grpc1Item.Id
                });
            
        }

        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
        {
            if(request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resource index must be greater than 0"));

            var grpc1Item = await dbContext.Grpc1Items.FirstOrDefaultAsync(t=>t.Id == request.Id);

            if(grpc1Item != null)
            {
                return await Task.FromResult(new ReadToDoResponse{
                    Id = grpc1Item.Id,
                    Title = grpc1Item.Title,
                    Description = grpc1Item.Description,
                    ToDoStatus = grpc1Item.ToDoStatus
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var grpc1Items = await dbContext.Grpc1Items.ToListAsync();

            foreach(var toDo in grpc1Items)
            {
                response.ToDo.Add(new ReadToDoResponse{
                    Id = toDo.Id,
                    Title = toDo.Title,
                    Description = toDo.Description,
                    ToDoStatus = toDo.ToDoStatus
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {
            if(request.Id <= 0 || request.Title == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));

                var grpc1Item = await dbContext.Grpc1Items.FirstOrDefaultAsync(t => t.Id == request.Id);

                if(grpc1Item == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));

                grpc1Item.Title = request.Title;
                grpc1Item.Description = request.Description;
                grpc1Item.ToDoStatus = request.ToDoStatus;

                await dbContext.SaveChangesAsync();

                return await Task.FromResult(new UpdateToDoResponse{
                    Id = grpc1Item.Id
                });
        }

                public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
                {
                    if (request.Id <= 0)
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index must be greater than 0"));

                    var grpc1Item = await dbContext.Grpc1Items.FirstOrDefaultAsync(t => t.Id == request.Id);

                    if (grpc1Item == null)
                        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));
                    
                    dbContext.Remove(grpc1Item);

                    await dbContext.SaveChangesAsync();

                    return await Task.FromResult(new DeleteToDoResponse{
                        Id = grpc1Item.Id
                    });
                }
        }
